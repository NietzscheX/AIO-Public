using AIO.Combat.Common;
using AIO.Framework;
using AIO.Lists;
using AIO.Settings;
using System.Collections.Generic;
using System.Linq;
using wManager.Wow.Enums;
using wManager.Wow.Helpers;
using static AIO.Constants;

namespace AIO.Combat.Paladin
{
    using Settings = PaladinLevelSettings;
    internal class SoloProtection : BaseRotation
    {
        protected override List<RotationStep> Rotation => new List<RotationStep>
        {
            new RotationStep(new RotationAction("Cache debuffed party members", RotationCombatUtil.CacheLUADebuffedPartyMembersStep), 0f, 1000),
            new RotationStep(new RotationSpell(SpellIds.Paladin.AutoAttack), 1f, (s,t) => !Me.IsCast && !RotationCombatUtil.IsAutoAttacking(), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.Paladin.LayOnHands), 1.1f, (s,t) => t.HealthPercent <= Settings.Current.ProtectionLoH && !Me.HaveBuff(SpellIds.Paladin.Forbearance), RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell(SpellIds.Paladin.SacredShield), 1.5f, (s,t) => !Me.HaveBuff(SpellIds.Paladin.SacredShield), RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell(SpellIds.Paladin.Consecration), 2f, (s,t) => t.HealthPercent > 25 && RotationFramework.Enemies.Count(o => o.GetDistance <=15) >= Settings.Current.ProtConsecration, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.Paladin.DivinePlea), 2.5f, (s, t) => Me.ManaPercentage < Settings.Current.GeneralDivinePlea && Settings.Current.DivinePleaIC, RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell(SpellIds.Paladin.RighteousDefense), 3f, (s,t) => t.Name != Me.Name && RotationFramework.Enemies.Count(o => o.IsAttackable && !o.IsTargetingMe && o.IsTargetingPartyMember) >=2,RotationCombatUtil.FindPartyMember),
            new RotationStep(new RotationSpell(SpellIds.Paladin.HandOfReckoning), 4f, (s,t) => t.GetDistance <= 25 && !t.IsTargetingMe && !Me.IsInGroup && Settings.Current.SoloRetributionHOR, RotationCombatUtil.BotTarget),        
            new RotationStep(new RotationSpell(SpellIds.Paladin.HandOfReckoning), 4.5f, (s,t) => Me.IsInGroup && RotationFramework.Enemies.Count(o => o.IsAttackable && !o.IsTargetingMe && o.IsTargetingPartyMember) >= 1, RotationCombatUtil.FindEnemyAttackingGroup),
            
            new RotationStep(new RotationSpell(SpellIds.Paladin.Cleanse), 4.6f, (s,t) => 
                Settings.Current.ProtectionCleanse == "Group",
                p => RotationCombatUtil.GetPartyMemberWithCachedDebuff(p, new List<DebuffType>() { DebuffType.Disease, DebuffType.Poison, DebuffType.Magic }, true, 30)),
            new RotationStep(new RotationSpell(SpellIds.Paladin.Cleanse), 4.7f, (s,t) => 
                Settings.Current.ProtectionCleanse == "Me"
                && RotationCombatUtil.IHaveCachedDebuff(new List<DebuffType>() { DebuffType.Disease, DebuffType.Poison, DebuffType.Magic }),
                RotationCombatUtil.FindMe),
            
            new RotationStep(new RotationSpell(SpellIds.Paladin.DivinePlea), 5f, (s, t) => Me.ManaPercentage < Settings.Current.GeneralDivinePlea, RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell(SpellIds.Paladin.HandOfFreedom), 5.5f, (s, t) => Me.Rooted, RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell(SpellIds.Paladin.HolyLight), 6f, (s,t) => !Me.IsInGroup && Me.HealthPercent <= 50 && Settings.Current.ProtectionHolyLight, RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell(SpellIds.Paladin.DivineProtection), 7f, (s,t) => Settings.Current.DivineProtection && (RotationFramework.Enemies.Count(o => o.IsTargetingMe && o.GetDistance <=15) >= 3 || BossList.MyTargetIsBoss), RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell(SpellIds.Paladin.HammerOfJustice), 8f, (s,t) => t.HealthPercent >= 75 && RotationCombatUtil.EnemyAttackingCountCluster(20) >= 2 && Settings.Current.ProtectionHammerofJustice, RotationCombatUtil.FindEnemy),
            new RotationStep(new RotationSpell(SpellIds.Paladin.AvengersShield), 9f, RotationCombatUtil.Always, RotationCombatUtil.BotTarget),
            //new RotationStep(new RotationSpell("Hand of Salvation"), 7f, (s,t) =>  RotationFramework.AllUnits.Count(o => o.IsAttackable && !o.IsTargetingMe && o.IsTargetingPartyMember && !TraceLine.TraceLineGo(Me.Position, o.Position)) >=2, RotationCombatUtil.FindPartyMember),
            new RotationStep(new RotationSpell(SpellIds.Paladin.HandOfSalvation), 9.1f, (s,t) => t.InCombatFlagOnly && t.HealthPercent < 99, RotationCombatUtil.FindHeal),
            new RotationStep(new RotationSpell(SpellIds.Paladin.HandOfProtection), 9.2f, (s,t) => Settings.Current.ProtectionHoP && t.HealthPercent < 75 && (t.WowClass == WoWClass.Mage || t.WowClass == WoWClass.Warlock || t.WowClass == WoWClass.Priest || t.WowClass == WoWClass.Druid), RotationCombatUtil.FindPartyMember),
            new RotationStep(new RotationSpell(SpellIds.Paladin.AvengersShield), 10f, (s,t) => t.GetDistance <= 25 && Me.ManaPercentage > 20, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.Paladin.AvengingWrath), 11f, (s,t) => RotationFramework.Enemies.Count(o => o.GetDistance <=20) >=3 && Settings.Current.AvengingWrathProtection,RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.Paladin.JudgementOfLight), 12f, (s,t) => !SpellManager.KnowSpell(SpellIds.Paladin.JudgementOfWisdom), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.Paladin.JudgementOfWisdom), 13f,(s,t) => !t.HaveBuff(SpellIds.Paladin.JudgementOfWisdom), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.Paladin.JudgementOfLight), 13.1f,(s,t) => t.HaveBuff(SpellIds.Paladin.JudgementOfWisdom), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.Paladin.HammerOfWrath), 14f, (s,t) => t.HealthPercent < 20 && Me.ManaPercentage > 50 , RotationCombatUtil.FindEnemy),
            new RotationStep(new RotationSpell(SpellIds.Paladin.HammerOfTheRighteous), 16f, RotationCombatUtil.Always, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.Paladin.ShieldOfRighteousness), 17f, RotationCombatUtil.Always, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.Paladin.HolyShield), 18f, RotationCombatUtil.Always, RotationCombatUtil.BotTarget),
        };
    }
}
