using AIO.Combat.Common;
using AIO.Framework;
using AIO.Settings;
using System.Collections.Generic;
using System.Linq;
using wManager.Wow.Enums;
using wManager.Wow.Helpers;
using static AIO.Constants;

namespace AIO.Combat.Mage
{
    using Settings = MageLevelSettings;
    internal class SoloFrost : BaseRotation
    {
        protected override List<RotationStep> Rotation => new List<RotationStep> {
            new RotationStep(new RotationSpell(SpellIds.Mage.Shoot), 0.9f, (s,t) => Settings.Current.UseWand && Me.ManaPercentage < Settings.Current.UseWandTresh && !RotationCombatUtil.IsAutoRepeating(SpellManager.GetSpellInfo(SpellIds.Mage.Shoot)), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.Mage.AutoAttack), 1f, (s,t) => !Me.IsCast && !RotationCombatUtil.IsAutoAttacking() && !RotationCombatUtil.IsAutoRepeating(SpellManager.GetSpellInfo(SpellIds.Mage.Shoot)), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.Mage.ManaShield), 1.1f, (s,t) => Me.HealthPercent <= 60 && Me.ManaPercentage >= 30 && !Me.HaveBuff(SpellIds.Mage.ManaShield), RotationCombatUtil.FindMe),
            // Only cast Polymorph if Sheep is enabled in settings
            new RotationStep(new RotationSpell(SpellIds.Mage.Polymorph), 2.1f, (s,t) => Settings.Current.SoloFrostSheep 
            // Only cast Polymorph if more than one enemy is targeting the Mage
            && !t.IsMyTarget && RotationFramework.Enemies.Count(o => o.IsTargetingMe) > 1 
            // Make sure no enemies in 30 yard casting range are polymorphed right now
            && RotationFramework.Enemies.Count(o => o.GetDistance <= 30 && o.HaveBuff(SpellIds.Mage.Polymorph)) < 1
            // Only polymorph a valid target
            && (t.IsCreatureType(wManager.Wow.Enums.CreatureType.Humanoid) || t.IsCreatureType(wManager.Wow.Enums.CreatureType.Beast) || t.IsCreatureType(wManager.Wow.Enums.CreatureType.Critter)),
                RotationCombatUtil.FindEnemyTargetingMe),
            new RotationStep(new RotationSpell(SpellIds.Mage.FrostNova), 2.2f, (s,t) => t.GetDistance <= 6 && t.HealthPercent > 30 && !Me.IsInGroup, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationBuff(SpellIds.Mage.IceBarrier), 3f, (s,t) => t.HealthPercent < 95, RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell(SpellIds.Mage.IceBlock), 4f, (s,t) => (t.HealthPercent < 15 && !t.HaveMyBuff(SpellIds.Mage.IceBarrier)) || (Me.IsInGroup && Me.HealthPercent < 50 && RotationFramework.Enemies.Count(o => o.IsTargetingMe) > 0), RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell(SpellIds.Mage.ColdSnap), 5f, (s,t) => t.HealthPercent < 95 && !t.HaveMyBuff(SpellIds.Mage.IceBarrier), RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell(SpellIds.Mage.Counterspell), 6f, (s,t) => t.IsCast, RotationCombatUtil.FindEnemyCasting),
            new RotationStep(new RotationSpell(SpellIds.Mage.ConeOfCold), 7f, (s,t) => Me.IsInGroup && RotationFramework.Enemies.Count(o => o.IsTargetingMeOrMyPetOrPartyMember && o.Position.DistanceTo(t.Position) <=10 && o.Position.DistanceTo(Me.Position) <= 10) >= Settings.Current.SoloFrostAOEInstance && Settings.Current.SoloFrostUseAOE, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.Mage.Blizzard), 7f, (s,t) => Me.IsInGroup && RotationFramework.Enemies.Count(o => o.IsTargetingMeOrMyPetOrPartyMember && o.Position.DistanceTo(t.Position) <=10) >= Settings.Current.SoloFrostAOEInstance && Settings.Current.SoloFrostUseAOE, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.Mage.FrostfireBolt), 8f, (s,t) => Me.HaveMyBuff(SpellIds.Mage.FireballProc), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.Mage.ColdSnap), 9f, (s,t) => !Me.HaveBuff(SpellIds.Mage.IceBarrier) && Me.HealthPercent < 95, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.Mage.Evocation), 10f, (s,t) =>  Settings.Current.GlyphOfEvocation && t.HealthPercent < 20 && RotationFramework.Enemies.Count() >= 2, RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell(SpellIds.Mage.MirrorImage), 11f, (s,t) => (!Me.IsInGroup && RotationFramework.Enemies.Count() >= 3) || BossList.MyTargetIsBoss, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.Mage.IcyVeins), 12f, (s,t) => (!Me.IsInGroup && RotationFramework.Enemies.Count() >= 2) || BossList.MyTargetIsBoss, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.Mage.SummonWaterElemental), 13f, (s,t) => !Settings.Current.GlyphOfEternalWater && ((!Me.IsInGroup && RotationFramework.Enemies.Count() >= 2) || BossList.MyTargetIsBoss), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.Mage.DeepFreeze), 14f, (s,t) => Me.ManaPercentage > Settings.Current.UseWandTresh , RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.Mage.IceLance), 15f, (s,t) => Me.ManaPercentage > Settings.Current.UseWandTresh && (Me.BuffStack(SpellIds.Mage.FingersOfFrost) > 0 || t.HaveMyBuff(SpellIds.Mage.FrostNova)), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.Mage.Fireball), 16f, (s,t) => Me.ManaPercentage > Settings.Current.UseWandTresh  && !SpellManager.KnowSpell(SpellIds.Mage.Frostbolt), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.Mage.FireBlast), 17f, (s,t) => Me.ManaPercentage > Settings.Current.UseWandTresh  && t.HealthPercent < Settings.Current.SoloFrostFrostFireBlast && !t.HaveBuff(SpellIds.Mage.FrostNova), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.Mage.Frostbolt), 18f, (s,t) =>  Me.ManaPercentage > Settings.Current.UseWandTresh , RotationCombatUtil.BotTarget)
        };
    }
}
