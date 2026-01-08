using AIO.Combat.Common;
using AIO.Framework;
using AIO.Helpers;
using AIO.Helpers.Caching;
using AIO.Lists;
using AIO.Settings;
using robotManager.Helpful;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using wManager.Wow.Enums;
using wManager.Wow.Helpers;
using wManager.Wow.ObjectManager;
using static AIO.Constants;

namespace AIO.Combat.Paladin
{
    using Settings = PaladinLevelSettings;
    internal class SoloRetribution : BaseRotation
    {
        private WoWUnit[] EnemiesAttackingGroup = new WoWUnit[0];
        private Stopwatch watch = Stopwatch.StartNew();

        protected override List<RotationStep> Rotation => new List<RotationStep> 
        {
            new RotationStep(new RotationAction("Cache debuffed party members", RotationCombatUtil.CacheLUADebuffedPartyMembersStep), 0f, 1000),
            new RotationStep(new RotationSpell("Auto Attack"), 1f, (s,t) => !Me.IsCast && !RotationCombatUtil.IsAutoAttacking(), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.Paladin.DivinePlea), 1.1f, (s, t) => Me.ManaPercentage < Settings.Current.GeneralDivinePlea, RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell(SpellIds.Paladin.HandOfFreedom), 1.2f, (s, t) => Me.Rooted, RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell(SpellIds.Paladin.DivineProtection), 1.3f,  (s,t) => Settings.Current.DivineProtection && RotationFramework.Enemies.Count(u=> u.IsTargetingMe) >=2, RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell(SpellIds.Paladin.SacredShield), 1.5f, (s,t) => !Me.HaveBuff(SpellIds.Paladin.SacredShield), RotationCombatUtil.FindMe),

            new RotationStep(new RotationSpell(SpellIds.Paladin.Purify), 2f, (s,t) =>
                RotationCombatUtil.IHaveCachedDebuff(new List<DebuffType>() { DebuffType.Disease, DebuffType.Poison }),
                RotationCombatUtil.FindMe),

            new RotationStep(new RotationSpell(SpellIds.Paladin.DivinePlea), 3.5f, (s, t) => Me.ManaPercentage < Settings.Current.GeneralDivinePlea && Settings.Current.DivinePleaIC, RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell(SpellIds.Paladin.FlashOfLight), 4f, (s,t) => (!Me.IsInGroup &&  Me.HaveBuff(SpellIds.Paladin.TheArtOfWar) && Me.HealthPercent <= 60) || (Me.IsInGroup &&  Me.HaveBuff(SpellIds.Paladin.TheArtOfWar) && Me.HealthPercent <= 60 && Settings.Current.SoloRetributionHealInCombat) , RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell(SpellIds.Paladin.FlashOfLight), 4.1f, (s,t) => Settings.Current.SoloRetributionHealGroup &&  Me.HaveBuff(SpellIds.Paladin.TheArtOfWar) && t.HealthPercent <= 60 && Settings.Current.SoloRetributionHealInCombat , RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell(SpellIds.Paladin.HammerOfJustice), 5f, (s, t) => t.IsCasting() , RotationCombatUtil.FindEnemyCasting),
            new RotationStep(new RotationSpell(SpellIds.Paladin.HammerOfJustice), 5.1f, (s,t) => t.Fleeing, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.Paladin.HammerOfJustice), 6f, (s, t) => RotationFramework.Enemies.Count(o => o.GetDistance <=5) >=2 , RotationCombatUtil.FindEnemy),
            new RotationStep(new RotationSpell(SpellIds.Paladin.HammerOfWrath), 7f, (s,t) => t.HealthPercent <20 , RotationCombatUtil.FindEnemy),
            new RotationStep(new RotationSpell(SpellIds.Paladin.HammerOfWrath), 8f, RotationCombatUtil.Always, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.Paladin.HolyLight), 9f, (s,t) => (!Me.IsInGroup && Me.HealthPercent <=  Settings.Current.SoloRetributionHL) || (Me.IsInGroup && Me.HealthPercent <=  Settings.Current.SoloRetributionHL && Settings.Current.SoloRetributionHealInCombat), RotationCombatUtil.FindMe),      
            new RotationStep(new RotationSpell(SpellIds.Paladin.FlashOfLight), 10f, (s,t) =>!Me.IsInGroup && Me.HealthPercent <=  Settings.Current.SoloRetributionFL && Settings.Current.SoloRetributionHealInCombat, RotationCombatUtil.FindMe),

            new RotationStep(new RotationSpell(SpellIds.Paladin.HolyLight), 10.1f, (s,t) => Settings.Current.SoloRetributionHealGroup && t.HealthPercent <= Settings.Current.SoloRetributionHL && Settings.Current.SoloRetributionHealInCombat, RotationCombatUtil.FindPartyMember),
            new RotationStep(new RotationSpell(SpellIds.Paladin.FlashOfLight), 10.2f, (s,t) =>Settings.Current.SoloRetributionHealGroup && t.HealthPercent <= Settings.Current.SoloRetributionFL && Settings.Current.SoloRetributionHealInCombat, RotationCombatUtil.FindPartyMember),

            new RotationStep(new RotationSpell(SpellIds.Paladin.HandOfReckoning), 11f, (s,t) => t.GetDistance <= 25 && !t.IsTargetingMe && !Me.IsInGroup &&  Settings.Current.SoloRetributionHOR, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.Paladin.AvengersShield), 12f, (s,t) => t.GetDistance <= 25 && Me.ManaPercentage > 20, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.Paladin.AvengingWrath), 13f, (s,t) => RotationFramework.Enemies.Count(o => o.GetDistance <=20) >=3 &&  Settings.Current.SoloAvengingWrathRetribution,RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.Paladin.JudgementOfLight), 14f, (s,t) => !Extension.KnowSpell(SpellIds.Paladin.JudgementOfWisdom), RotationCombatUtil.BotTarget), // Warning: SpellManager.KnowSpell also needs ID support potentially
            new RotationStep(new RotationSpell(SpellIds.Paladin.JudgementOfWisdom), 15f, RotationCombatUtil.Always, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.Paladin.DivineStorm), 16f, RotationCombatUtil.Always, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.Paladin.CrusaderStrike), 17f, (s,t) => true, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.Paladin.Consecration), 18f, (s,t) => EnemiesAttackingGroup.Count(ene => ene.CGetDistance() <= 10) >= Settings.Current.SoloRetributionConsecration, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.Paladin.Exorcism), 19f, (s,t) => (Me.HaveBuff(SpellIds.Paladin.TheArtOfWar) && (t.HealthPercent > 10 || BossList.MyTargetIsBoss)) || !TalentsManager.HaveTalent(3, 17), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.Paladin.Exorcism), 20f, (s,t) => !TalentsManager.HaveTalent(3, 17), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.Paladin.HolyWrath), 21f, RotationCombatUtil.Always, RotationCombatUtil.BotTarget),
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
