using AIO.Combat.Common;
using AIO.Framework;
using AIO.Helpers;
using AIO.Helpers.Caching;
using AIO.Lists;
using AIO.Settings;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using wManager.Wow.Helpers;
using wManager.Wow.ObjectManager;
using static AIO.Constants;

namespace AIO.Combat.Paladin
{
    using Settings = PaladinLevelSettings;
    internal class GroupRetribution : BaseRotation
    {
        private WoWUnit[] EnemiesAttackingGroup = new WoWUnit[0];
        private Stopwatch watch = Stopwatch.StartNew();

        protected override List<RotationStep> Rotation => new List<RotationStep> 
        {
            new RotationStep(new DebugSpell("Pre-Calculations"), 0.0f,(action,unit) => DoPreCalculations(), RotationCombatUtil.FindMe, checkRange: false, forceCast: true, ignoreGCD: true),
            new RotationStep(new RotationAction("Cache debuffed party members", RotationCombatUtil.CacheLUADebuffedPartyMembersStep), 0f, 1000),
            new RotationStep(new RotationSpell(SpellIds.Paladin.AutoAttack), 1f, (s,t) => !Me.IsCast && !RotationCombatUtil.IsAutoAttacking(), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.Paladin.DivinePlea), 1.1f, (s, t) => Me.CManaPercentage() < Settings.Current.GeneralDivinePlea, RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell(SpellIds.Paladin.HandOfFreedom), 1.2f, (s, t) => Me.Rooted, RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell(SpellIds.Paladin.DivineProtection), 1.3f,  (s,t) => Settings.Current.DivineProtection && EnemiesAttackingGroup.ContainsAtLeast(enem=> enem.CIsTargetingMe(), 2), RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell(SpellIds.Paladin.SacredShield), 1.5f, (s,t) => !Me.CHaveBuff(SpellIds.Paladin.SacredShield), RotationCombatUtil.FindMe),
            
            new RotationStep(new RotationSpell(SpellIds.Paladin.Purify), 2f, (s,t) => 
                Settings.Current.GroupRetributionPurify 
                && RotationCombatUtil.IHaveCachedDebuff(new List<DebuffType>() { DebuffType.Disease, DebuffType.Poison }), 
                RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell(SpellIds.Paladin.Purify), 2.1f, (s,t) => 
                Settings.Current.GroupRetributionPurifyMember,
                p => RotationCombatUtil.GetPartyMemberWithCachedDebuff(p, new List<DebuffType>() { DebuffType.Disease, DebuffType.Poison }, true, 30)),
            
            new RotationStep(new RotationSpell(SpellIds.Paladin.DivinePlea), 3.5f, (s, t) => Me.CManaPercentage() < Settings.Current.GeneralDivinePlea && Settings.Current.DivinePleaIC, RotationCombatUtil.FindMe),

            new RotationStep(new RotationSpell(SpellIds.Paladin.FlashOfLight), 4f, (s,t) => Me.HaveBuff(SpellIds.Paladin.TheArtOfWar) && Me.HealthPercent <= 60 && Settings.Current.GroupRetributionHealInCombat , RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell(SpellIds.Paladin.HammerOfJustice), 5f, (s, t) => t.CIsCast() && t.CGetDistance() >= 10, RotationCombatUtil.BotTargetFast, checkLoS: true),
            new RotationStep(new RotationSpell(SpellIds.Paladin.HammerOfJustice), 5.1f, (s,t) => t.Fleeing, RotationCombatUtil.BotTargetFast, checkLoS: true),
            new RotationStep(new RotationSpell(SpellIds.Paladin.HammerOfWrath), 7f, (s,t) => t.CHealthPercent() <20 , RotationCombatUtil.FindEnemy, checkLoS: true),

            new RotationStep(new RotationSpell(SpellIds.Paladin.HolyLight), 9f, (s,t) => Me.CHealthPercent() <=  Settings.Current.GroupRetributionHL && Settings.Current.GroupRetributionHealInCombat, RotationCombatUtil.FindMe),      

            new RotationStep(new RotationSpell(SpellIds.Paladin.AvengingWrath), 13f, (s,t) => EnemiesAttackingGroup.ContainsAtLeast(enem=> enem.CGetDistance() <= 20, 3) &&  Settings.Current.GroupAvengingWrathRetribution, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.Paladin.JudgementOfLight), 14f, (s,t) => !SpellManager.KnowSpell(SpellIds.Paladin.JudgementOfWisdom), RotationCombatUtil.BotTargetFast, checkLoS: true),
            new RotationStep(new RotationSpell(SpellIds.Paladin.JudgementOfWisdom), 15f, RotationCombatUtil.Always, RotationCombatUtil.BotTargetFast, checkLoS: true),
            new RotationStep(new RotationSpell(SpellIds.Paladin.DivineStorm), 16f, RotationCombatUtil.Always, RotationCombatUtil.BotTargetFast, checkLoS: true),
            new RotationStep(new RotationSpell(SpellIds.Paladin.CrusaderStrike), 17f, RotationCombatUtil.Always, RotationCombatUtil.BotTargetFast, checkLoS: true),
            new RotationStep(new RotationSpell(SpellIds.Paladin.Consecration), 18f, RotationCombatUtil.Always, _ => EnemiesAttackingGroup.Count(unit => unit.CGetDistance() <=8) >= Settings.Current.GroupRetributionConsecration, RotationCombatUtil.FindMe, checkRange: false),
            new RotationStep(new RotationSpell(SpellIds.Paladin.Exorcism), 19f, (s,t) => (Me.CHaveBuff(SpellIds.Paladin.TheArtOfWar) && (t.CHealthPercent() > 10 || BossList.MyTargetIsBoss)) || !TalentsManager.HaveTalent(3, 17), RotationCombatUtil.BotTargetFast, checkRange: true),
            new RotationStep(new RotationSpell(SpellIds.Paladin.HolyWrath), 21f, RotationCombatUtil.Always, RotationCombatUtil.BotTargetFast, checkRange: true),
        };

        private bool DoPreCalculations()
        {
            if (LimitExecutionSpeed(100))
            {
                return true;
            }
            Cache.Reset();
            EnemiesAttackingGroup = RotationFramework.Enemies.Where(unit => unit.CIsTargetingMeOrMyPetOrPartyMember())
                .ToArray();
            return false;
        }

        private bool LimitExecutionSpeed(int delay)
        {
            if (watch.ElapsedMilliseconds > delay)
            {
                watch.Restart();
                return false;
            }
            return true;
        }

        public WoWUnit FindEnemyAttackingGroup(Func<WoWUnit, bool> predicate) => EnemiesAttackingGroup.FirstOrDefault(predicate);
    }
}
