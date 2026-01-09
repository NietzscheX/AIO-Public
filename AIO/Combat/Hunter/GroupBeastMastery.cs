using AIO.Combat.Common;
using AIO.Framework;
using AIO.Helpers.Caching;
using AIO.Settings;
using robotManager.Helpful;
using System;
using System.Collections.Generic;
using System.Linq;
using wManager.Wow.ObjectManager;
using static AIO.Constants;

namespace AIO.Combat.Hunter
{
    using Settings = HunterLevelSettings;
    internal class GroupBeastMastery : BaseRotation
    {
        private WoWUnit[] EnemiesAttackingGroup = new WoWUnit[0];
        protected override List<RotationStep> Rotation => new List<RotationStep>
        {
            new RotationStep(new RotationAction("Pre-Calculations", DoPreCalculations), 0f, 500),
            new RotationStep(new RotationSpell(SpellIds.Hunter.AutoAttack), 1f, (s,t) => !Me.IsCast && !RotationCombatUtil.IsAutoAttacking() && !RotationCombatUtil.IsAutoRepeating(SpellManager.GetSpellInfo(SpellIds.Hunter.AutoShoot)), RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell(SpellIds.Hunter.FeignDeath), 2f, (s,t) => Me.CHealthPercent() < 50 && EnemiesAttackingGroup.ContainsAtLeast(u => u.CGetDistance() < 10 && u.CIsTargetingMe(), 1) && Settings.Current.GroupBeastMasteryFD, RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell(SpellIds.Hunter.Deterrence), 2.1f, (s,t) => Me.CHealthPercent() < 80 && EnemiesAttackingGroup.ContainsAtLeast(u => u.CGetDistance() < 25 && u.CIsTargetingMe(), 1) && Settings.Current.GroupBeastMasteryDeterrence, RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell(SpellIds.Hunter.Intimidation), 3f, (s,t) => Pet.Target != 0 && Pet.CGetPosition().DistanceTo(t.CGetPosition()) <= 6 && t.CIsCast(), RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell(SpellIds.Hunter.ConcussiveShot), 3.1f, (s,t) => t.Fleeing && !t.CHaveBuff(SpellIds.Hunter.ConcussiveShot), RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell(SpellIds.Hunter.Misdirection), 3.3f,
                (action, tank) => Settings.Current.GroupBeastMasteryMisdirection && !Me.CHaveBuff(SpellIds.Hunter.Misdirection) && tank.CInCombat() && tank.CIsAlive() , RotationCombatUtil.FindTank, checkLoS: true), 
            //Push Aggro to Tank
            new RotationStep(new RotationSpell(SpellIds.Hunter.MultiShot), 3.4f, (s,t) => Me.CHaveBuff(SpellIds.Hunter.Misdirection), RotationCombatUtil.BotTargetFast, checkLoS: true),
            new RotationStep(new RotationSpell(SpellIds.Hunter.Volley), 4f,
                (s,t) => EnemiesAttackingGroup.ContainsAtLeast(u => u.CGetDistance() < 45 && !EnemiesAttackingGroup.Any(ene => ene.CIsTargetingMe()), Settings.Current.GroupBeastMasteryAOECount) && Settings.Current.GroupBeastMasteryUseAOE, FindVolleyCluster, checkLoS:true),
            new RotationStep(new RotationSpell(SpellIds.Hunter.KillShot), 5f, (s,t) => t.CGetDistance() >= 5 && t.CHealthPercent() < 20, RotationCombatUtil.BotTargetFast, checkLoS: true),
            new RotationStep(new RotationSpell(SpellIds.Hunter.HuntersMark), 9f, (s,t) => t.CGetDistance() >= 5 && !t.CHaveMyBuff(SpellIds.Hunter.HuntersMark) && t.CIsAlive() &&  t.CHealthPercent() > 60, RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell(SpellIds.Hunter.BestialWrath), 10f, (s,t) => BossList.MyTargetIsBoss || EnemiesAttackingGroup.Count() > 3, RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell(SpellIds.Hunter.RapidFire), 11f, (s,t) => BossList.MyTargetIsBoss || EnemiesAttackingGroup.Count() > 3, RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell(SpellIds.Hunter.KillCommand), 12f, (s,t) => !Me.CHaveBuff(SpellIds.Hunter.KillCommand), RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell(SpellIds.Hunter.SerpentSting), 13f, (s,t) => t.CGetDistance() >= 5 && !t.CHaveMyBuff(SpellIds.Hunter.SerpentSting) && (t.CHealthPercent() >= 70 || (BossList.MyTargetIsBoss && t.CHealthPercent() >= 20)), RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell(SpellIds.Hunter.ArcaneShot), 14f, (s,t) => t.CGetDistance() >= 5, RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell(SpellIds.Hunter.MultiShot), 15f, (s,t) => t.CGetDistance() >= 5 && Settings.Current.GroupBeastMasteryMultiShot && EnemiesAttackingGroup.Count() >= Settings.Current.GroupBeastMasteryMultiShotCount, RotationCombatUtil.BotTargetFast, checkLoS:true),
            new RotationStep(new RotationSpell(SpellIds.Hunter.SteadyShot), 15.1f, (s,t) => !Me.IsCast && !Me.GetMove && t.CGetDistance() >= 5, RotationCombatUtil.BotTargetFast, checkLoS: true),
            new RotationStep(new RotationSpell(SpellIds.Hunter.RaptorStrike), 16f, (s,t) => t.CGetDistance() < 5, RotationCombatUtil.BotTargetFast),
        };

        private bool DoPreCalculations()
        {
            Cache.Reset();
            EnemiesAttackingGroup = RotationFramework.Enemies.Where(unit => unit.CIsTargetingMeOrMyPetOrPartyMember())
                .ToArray();
            return false;
        }

        private static WoWUnit FindVolleyCluster(Func<WoWUnit, bool> predicate)
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

