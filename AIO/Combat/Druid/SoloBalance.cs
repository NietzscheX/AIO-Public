using AIO.Combat.Common;
using AIO.Framework;
using AIO.Settings;
using System.Collections.Generic;
using System.Linq;
using wManager.Wow.ObjectManager;
using static AIO.Constants;

namespace AIO.Combat.Druid
{
    using Settings = DruidLevelSettings;
    internal class SoloBalance : BaseRotation
    {
        protected override List<RotationStep> Rotation => new List<RotationStep> {
            new RotationStep(new RotationSpell(SpellIds.Druid.AutoAttack), 1f, (s,t) => !Me.IsCast && !RotationCombatUtil.IsAutoAttacking(), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationBuff(SpellIds.Druid.Tranquility), 3f, UseTranquility, RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell(SpellIds.Druid.Regrowth), 4f, (s,t) => !Me.HaveBuff(SpellIds.Druid.Regrowth) && Me.HealthPercent < Settings.Current.SoloBalanceRegrowth, RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell(SpellIds.Druid.HealingTouch), 5f, (s ,t) => Me.HealthPercent < Settings.Current.SoloBalanceHealingTouch, RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell(SpellIds.Druid.Regrowth), 6f, (s,t) => !t.HaveBuff(SpellIds.Druid.Regrowth) && t.HealthPercent < Settings.Current.SoloBalanceRegrowth, RotationCombatUtil.FindPartyMember),
            new RotationStep(new RotationSpell(SpellIds.Druid.HealingTouch), 7f, (s ,t) => t.HealthPercent < Settings.Current.SoloBalanceHealingTouch, RotationCombatUtil.FindPartyMember),
            new RotationStep(new RotationBuff(SpellIds.Druid.Innervate), 8f, (s,t) => Me.IsInGroup && t.Name == RotationFramework.HealName && t.ManaPercentage < Settings.Current.Innervate, RotationCombatUtil.FindPartyMember),
            new RotationStep(new RotationBuff(SpellIds.Druid.Innervate), 9f, (s,t) => Me.ManaPercentage <= Settings.Current.Innervate, RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell(SpellIds.Druid.MoonkinForm), 10f, (s, t) => !Me.HaveBuff(SpellIds.Druid.MoonkinForm), RotationCombatUtil.FindMe),
            new RotationStep(new RotationBuff(SpellIds.Druid.Barkskin), 11f, (s, t) => RotationFramework.Enemies.Count(o => o.IsTargetingMe && o.Position.DistanceTo(t.Position) <= 20) >= 2 || (!Me.IsInGroup && Me.InCombat), RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell(SpellIds.Druid.AbolishPoison), 12f, (s,t) =>  !t.HaveMyBuff(SpellIds.Druid.AbolishPoison) && t.HaveImportantPoison(), RotationCombatUtil.FindPartyMember),
            new RotationStep(new RotationSpell(SpellIds.Druid.RemoveCurse), 13f, (s,t) => t.HaveImportantCurse(), RotationCombatUtil.FindPartyMember),
            new RotationStep(new RotationSpell(SpellIds.Druid.Starfall), 13.5f, (s,t) => (BossList.MyTargetIsBoss && t.HealthPercent > 25) || (RotationFramework.Enemies.Count(o => o.IsElite && o.Position.DistanceTo(t.Position) <= 33) >= 3 && Settings.Current.SoloBalanceUseStarfall), RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell(SpellIds.Druid.Typhoon), 14f, (s, t) => t.GetDistance < 30 && RotationFramework.Enemies.Count(u => u.IsTargetingMeOrMyPetOrPartyMember) >= 3, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.Druid.Starfall), 14.5f, (s,t) => !Me.IsInGroup && t.HealthPercent >= 50 && (RotationFramework.AllUnits.Count(o => o.IsAlive && o.IsTargetingMeOrMyPet) >= 2) && Settings.Current.SoloBalanceUseStarfall, RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell(SpellIds.Druid.ForceOfNature), 15f, (s, t) => BossList.MyTargetIsBoss || (Me.IsInGroup && RotationFramework.Enemies.Count(o => o.IsTargetingMeOrMyPetOrPartyMember && o.IsElite) >= Settings.Current.SoloBalanceAOETargets), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.Druid.ForceOfNature), 16f, (s, t) => !Me.IsInGroup && t.HealthPercent >= 50 && (RotationFramework.Enemies.Count(o => o.IsTargetingMeOrMyPetOrPartyMember) >= 2), RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell(SpellIds.Druid.Hurricane), 17f, (s,t) => Me.IsInGroup && RotationFramework.Enemies.Count(o => o.IsTargetingMeOrMyPetOrPartyMember && o.Position.DistanceTo(t.Position) <=10) >= Settings.Current.SoloBalanceAOETargets && Settings.Current.SoloBalanceUseAOE, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.Druid.Starfire),18f, (s, t) => t.HealthPercent == 100 && !t.IsTargetingMe, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.Druid.InsectSwarm), 19f, (s, t) => !t.HaveMyBuff(SpellIds.Druid.InsectSwarm) && (t.Health > 45 || BossList.MyTargetIsBoss) && !Me.HaveBuff(SpellIds.Druid.EclipseLunar) && !Me.HaveBuff(SpellIds.Druid.EclipseSolar), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.Druid.FaerieFire), 20f, (s, t) => !t.HaveBuff(SpellIds.Druid.FaerieFire) && BossList.MyTargetIsBoss, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.Druid.Moonfire), 21f, (s, t) => Settings.Current.SoloBalanceUseMoonfire && BossList.MyTargetIsBoss && !t.HaveMyBuff(SpellIds.Druid.Moonfire) && t.HealthPercent > 35 || !Settings.Current.SoloBalanceUseMoonfire && !t.HaveMyBuff(SpellIds.Druid.Moonfire) && t.HealthPercent >= 60 && !Me.HaveBuff(SpellIds.Druid.EclipseLunar) && !Me.HaveBuff(SpellIds.Druid.EclipseSolar), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.Druid.Starfire), 22f, (s, t) => t.HealthPercent >= 10 && !Me.HaveBuff(SpellIds.Druid.EclipseSolar) && Me.HaveBuff(SpellIds.Druid.NaturesGrace) || Me.HaveBuff(SpellIds.Druid.EclipseLunar), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.Druid.Wrath), 23f, (s, t) => Me.HaveBuff(SpellIds.Druid.EclipseSolar) && Me.ManaPercentage > 5, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.Druid.Wrath), 24f, (s, t) => !Me.HaveBuff(SpellIds.Druid.EclipseSolar) && !Me.HaveBuff(SpellIds.Druid.EclipseLunar), RotationCombatUtil.BotTarget, checkLoS: true),
        };


        private bool UseTranquility(IRotationAction s, WoWUnit t)
        {
            var nearbyFriendlies = RotationFramework.PartyMembers.Where(o => o.IsAlive && o.GetDistance <= 40).ToList();

            var under40 = nearbyFriendlies.Count(o => o.HealthPercent <= 40);
            var under55 = nearbyFriendlies.Count(o => o.HealthPercent <= 55);
            var under65 = nearbyFriendlies.Count(o => o.HealthPercent <= 65);

            return Me.IsInGroup && RotationFramework.PartyMembers.Count(u => u.IsAlive) >= 1 && (under40 >= 2 || under55 >= 3 || under65 >= 4);
        }
    }
}