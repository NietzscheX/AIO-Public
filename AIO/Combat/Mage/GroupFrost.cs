using AIO.Combat.Common;
using AIO.Framework;
using AIO.Helpers.Caching;
using AIO.Settings;
using robotManager.Helpful;
using System;
using System.Collections.Generic;
using System.Linq;
using wManager.Wow.Class;
using wManager.Wow.Helpers;
using wManager.Wow.ObjectManager;
using static AIO.Constants;

namespace AIO.Combat.Mage
{
    using Settings = MageLevelSettings;
    internal class GroupFrost : BaseRotation
    {
        private WoWUnit[] _enemiesAttackingGroup = new WoWUnit[0];
        private readonly bool _knowsFrostFireBolt = SpellManager.KnowSpell(SpellIds.Mage.FrostfireBolt);
        private readonly bool _knowsFrostBolt = SpellManager.KnowSpell(SpellIds.Mage.Frostbolt);
        private int _fingersOfFrostStacks;

        protected override List<RotationStep> Rotation => new List<RotationStep> {
            new RotationStep(new RotationAction("Pre-Calculations", DoPreCalculations), 0f, 500),
            new RotationStep(new RotationSpell(SpellIds.Mage.AutoAttack), 1f, (s,t) => !Me.CIsCast() && !RotationCombatUtil.IsAutoAttacking() && !RotationCombatUtil.IsAutoRepeating(SpellManager.GetSpellInfo(SpellIds.Mage.Shoot)), RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell(SpellIds.Mage.ManaShield), 1.1f, (s,t) => Me.CHealthPercent() <= 40 && Me.CManaPercentage() >= 30 && !Me.HaveBuff(SpellIds.Mage.ManaShield), RotationCombatUtil.FindMe),
            new RotationStep(new RotationBuff(SpellIds.Mage.IceBarrier), 3f, (s,t) => t.CHealthPercent() < 60, RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell(SpellIds.Mage.ColdSnap), 3.5f, (s,t) => !Me.CHaveBuff(SpellIds.Mage.IceBarrier) && Me.CHealthPercent() < 60, RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell(SpellIds.Mage.IceBlock), 4f, (s,t) =>  Me.CHealthPercent() < 30 && _enemiesAttackingGroup.ContainsAtLeast(u => u.CGetDistance() < 10 && u.CIsTargetingMe(), 1), RotationCombatUtil.FindMe),

            new RotationStep(new RotationSpell(SpellIds.Mage.Counterspell), 5f, (s,t) => t.CIsCast(), RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell(SpellIds.Mage.ConeOfCold), 6f, (s,t) => _enemiesAttackingGroup.ContainsAtLeast(u => u.CGetDistance() < 10, Settings.Current.GroupFrostAOEInstance) && Settings.Current.GroupFrostUseAOE, RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell(SpellIds.Mage.Blizzard), 7f, (s,t) => _enemiesAttackingGroup.ContainsAtLeast(u => u.CGetDistance() < 45 && !_enemiesAttackingGroup.Any(ene => ene.CIsTargetingMe() ), Settings.Current.GroupFrostAOEInstance) && Settings.Current.GroupFrostUseAOE, FindBlizzardCluster),

            new RotationStep(new RotationSpell(SpellIds.Mage.Evocation), 8f, (s,t) =>  Settings.Current.GlyphOfEvocation && t.CHealthPercent() < 20, RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell(SpellIds.Mage.Evocation), 9f, (s,t) =>  t.CManaPercentage() < 20, RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell(SpellIds.Mage.MirrorImage), 10f, (s,t) => _enemiesAttackingGroup.ContainsAtLeast(u => u.CGetDistance() < 30, 3) || BossList.MyTargetIsBoss, RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell(SpellIds.Mage.IcyVeins), 11f, (s,t) => _enemiesAttackingGroup.ContainsAtLeast(u => u.CGetDistance() < 30, 3) || BossList.MyTargetIsBoss, RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell(SpellIds.Mage.SummonWaterElemental), 12f, (s,t) => !Settings.Current.GlyphOfEternalWater && _enemiesAttackingGroup.ContainsAtLeast(u => u.CGetDistance() < 30, 3) || BossList.MyTargetIsBoss, RotationCombatUtil.BotTargetFast),
            
            // Brain Freeze
            new RotationStep(new RotationSpell(SpellIds.Mage.FrostfireBolt), 13f, (s,t) => Me.CHaveBuff(SpellIds.Mage.FireballProc), RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell(SpellIds.Mage.Fireball), 14f, (s,t) => !_knowsFrostFireBolt && Me.CHaveBuff(SpellIds.Mage.FireballProc), RotationCombatUtil.BotTargetFast),

            // Fingers of Frost/Frost Nova
            new RotationStep(new RotationSpell(SpellIds.Mage.DeepFreeze), 15f, (s,t) => _fingersOfFrostStacks == 1 || t.CHaveMyBuff(SpellIds.Mage.FrostNova), RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell(SpellIds.Mage.IceLance), 16f, (s,t) => _fingersOfFrostStacks == 1 || t.CHaveMyBuff(SpellIds.Mage.FrostNova), RotationCombatUtil.BotTargetFast),

            new RotationStep(new RotationSpell(SpellIds.Mage.FireBlast), 17f, (s,t) => t.CHealthPercent() < Settings.Current.GroupFrostFrostFireBlast , RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell(SpellIds.Mage.Frostbolt), 18f, (s,t) =>  true, RotationCombatUtil.BotTargetFast, checkLoS: true),

            new RotationStep(new RotationSpell(SpellIds.Mage.Fireball), 20f, (s,t) => !_knowsFrostBolt, RotationCombatUtil.BotTargetFast, checkLoS: true),
            new RotationStep(new RotationSpell(SpellIds.Mage.Shoot), 25f, (s,t) => Me.CManaPercentage() < 5 && !RotationCombatUtil.IsAutoRepeating(SpellManager.GetSpellInfo(SpellIds.Mage.Shoot)), RotationCombatUtil.BotTargetFast, checkLoS: true),
        };

        private bool DoPreCalculations()
        {
            Cache.Reset();
            _enemiesAttackingGroup = RotationFramework.Enemies.Where(unit => unit.CIsTargetingMeOrMyPetOrPartyMember())
                .ToArray();
            // Getting auras manually because CBuffStack doesn't seem to return correct values
            foreach (Aura aura in BuffManager.GetAuras(Me.GetBaseAddress).ToList())
            {
                if (aura.SpellId == SpellIds.Mage.FingersOfFrost)
                {
                    _fingersOfFrostStacks = aura.Stack;
                    return false;
                }
            }
            _fingersOfFrostStacks = 0;
            return false;
        }

        private static WoWUnit FindBlizzardCluster(Func<WoWUnit, bool> predicate)
        {
            WoWUnit largestCenter = null;
            int largestCount = 2;
            for (var i = 0; i < RotationFramework.Enemies.Length; i++)
            {
                WoWUnit originUnit = RotationFramework.Enemies[i];
                if (!predicate(originUnit))
                {
                    continue;
                }
                Vector3 originPos = originUnit.CGetPosition();
                int localCount = RotationFramework.Enemies.Count(enemy => enemy.CIsAlive() && enemy.CGetPosition().DistanceTo(originPos) < 10 && enemy.CIsTargetingMeOrMyPetOrPartyMember());

                if (localCount > largestCount)
                {
                    largestCenter = originUnit;
                    largestCount = localCount;
                }
            }
            return largestCenter;
        }
    }
}
