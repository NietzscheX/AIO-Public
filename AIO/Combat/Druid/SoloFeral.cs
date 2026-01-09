using AIO.Combat.Common;
using AIO.Framework;
using AIO.Settings;
using System.Collections.Generic;
using System.Linq;
using wManager.Wow.Helpers;
using static AIO.Constants;

namespace AIO.Combat.Druid
{
    using Settings = DruidLevelSettings;
    internal class SoloFeral : BaseRotation
    {
        protected override List<RotationStep> Rotation => new List<RotationStep> {
            // Rotation 11-20
            new RotationStep(new RotationSpell(SpellIds.Druid.AutoAttack), 1f, (s,t) => !Me.IsCast && !RotationCombatUtil.IsAutoAttacking(), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationBuff(SpellIds.Druid.Innervate), 1.1f, (s, t) => !Me.IsInGroup && !Me.HaveBuff(SpellIds.Druid.BearForm) && !Me.HaveBuff(SpellIds.Druid.DireBearForm) && !Me.HaveBuff(SpellIds.Druid.CatForm) && Me.ManaPercentage <= Settings.Current.Innervate, RotationCombatUtil.FindMe),
            new RotationStep(new RotationBuff(SpellIds.Druid.Innervate), 1.2f, (s, t) =>Me.IsInGroup && t.ManaPercentage <= Settings.Current.Innervate, RotationCombatUtil.FindHeal),
            new RotationStep(new RotationSpell(SpellIds.Druid.RemoveCurse), 1.3f, (s,t) => t.HaveBuff(SpellIds.Priest.VeilOfShadow) && Settings.Current.SoloFeralDecurse, RotationCombatUtil.FindPartyMember),
            new RotationStep(new RotationBuff(SpellIds.Druid.Barkskin), 1.4f, (s, t) => Me.HealthPercent <= 35, RotationCombatUtil.FindMe),
            new RotationStep(new RotationBuff(SpellIds.Druid.Regrowth), 1.50f, (s, t) => Settings.Current.SoloFeralRegrowthIC && Me.ManaPercentage > 15 && Me.HealthPercent <= Settings.Current.SoloFeralICHealThreshold && TalentsManager.HaveTalent(2,25) && (Me.Mana > ((DruidBehavior.TransformValue * 0.6) + DruidBehavior.RegrowthValue)) && !Me.HaveBuff(SpellIds.Druid.Rejuvenation), RotationCombatUtil.FindMe),
            new RotationStep(new RotationBuff(SpellIds.Druid.Regrowth), 1.51f, (s, t) => Settings.Current.SoloFeralRegrowthIC && Me.ManaPercentage > 15 && Me.HealthPercent <= Settings.Current.SoloFeralICHealThreshold && Me.Mana > (DruidBehavior.TransformValue + DruidBehavior.RegrowthValue) && !Me.HaveBuff(SpellIds.Druid.Regrowth) , RotationCombatUtil.FindMe),
            new RotationStep(new RotationBuff(SpellIds.Druid.Rejuvenation), 1.60f, (s, t) => Settings.Current.SoloFeralRejuvenationIC && Me.ManaPercentage > 15 && Me.HealthPercent <= Settings.Current.SoloFeralICHealThreshold && TalentsManager.HaveTalent(2,25) && Me.Mana > ((DruidBehavior.TransformValue * 0.6) + DruidBehavior.RejuvenationValue) && Me.HaveBuff(SpellIds.Druid.Regrowth) && Me.HaveBuff(SpellIds.Druid.Rejuvenation), RotationCombatUtil.FindMe),
            new RotationStep(new RotationBuff(SpellIds.Druid.Rejuvenation), 1.61f, (s, t) => Settings.Current.SoloFeralRejuvenationIC && Me.ManaPercentage > 15 && Me.HealthPercent <= Settings.Current.SoloFeralICHealThreshold && Me.Mana > (DruidBehavior.TransformValue + DruidBehavior.RejuvenationValue) && Me.HaveBuff(SpellIds.Druid.Regrowth) && !Me.HaveBuff(SpellIds.Druid.Rejuvenation), RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell(SpellIds.Druid.HealingTouch), 1.70f, (s, t) => Settings.Current.SoloFeralHealingTouchIC && !Me.IsInGroup && Me.ManaPercentage > 15 &&  Me.HealthPercent <= Settings.Current.SoloFeralICHealThreshold && Me.Mana > ((DruidBehavior.TransformValue * 0.6) + DruidBehavior.HealingTouchValue) && TalentsManager.HaveTalent(2,25) && Me.HaveBuff(SpellIds.Druid.Regrowth) && Me.HaveBuff(SpellIds.Druid.Rejuvenation), RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell(SpellIds.Druid.HealingTouch), 1.71f, (s, t) => Settings.Current.SoloFeralHealingTouchIC && !Me.IsInGroup && Me.ManaPercentage > 15 &&  Me.HealthPercent <= Settings.Current.SoloFeralICHealThreshold && Me.Mana > (DruidBehavior.TransformValue + DruidBehavior.HealingTouchValue) && Me.HaveBuff(SpellIds.Druid.Regrowth) && Me.HaveBuff(SpellIds.Druid.Rejuvenation), RotationCombatUtil.FindMe),                        
            new RotationStep(new RotationSpell(SpellIds.Druid.Berserk), 1.8f, (s,t) => (Me.HaveBuff(SpellIds.Druid.BearForm) || Me.HaveBuff(SpellIds.Druid.DireBearForm) || Me.HaveBuff(SpellIds.Druid.CatForm)) && (t.IsElite || RotationFramework.Enemies.Count(o => o.IsTargetingMe && o.Position.DistanceTo(t.Position) <= 20) >=2), RotationCombatUtil.FindMe),
            new RotationStep(new RotationBuff(SpellIds.Druid.BearForm), 2f, (s, t) => Me.Level > 9 && Me.Level < 20, RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell(SpellIds.Druid.Bash), 2.1f, (s, t) => t.IsCasting() && Me.Level > 9 && Me.Level < 20, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.Druid.DemoralizingRoar), 3f, (s, t) => !t.HaveMyBuff(SpellIds.Druid.DemoralizingRoar)&& Me.Level > 9 && Me.Level < 20, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.Druid.SwipeBear), 10f, (s, t) => RotationFramework.Enemies.Count(o => Me.IsFacing(o.Position, 3) && o.HasTarget && o.IsTargetingMe && o.Position.DistanceTo(Me.Position) <= 8) >= 2, RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell(SpellIds.Druid.Growl), 6f, (s, t) => !Me.IsInGroup && Me.Level > 9 && Me.Level < 20 && t.GetDistance > 20, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.Druid.Maul), 7f, (s, t) => Me.Rage >= 16 && Me.Level > 9 && Me.Level < 20 && t.GetDistance < 8, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.Druid.Moonfire), 8f, (s, t) => !SpellManager.KnowSpell(SpellIds.Druid.BearForm) && Me.Level > 9 && Me.Level < 20 && t.HealthPercent == 100 && !t.IsTargetingMe, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.Druid.Wrath), 9f, (s, t) => !SpellManager.KnowSpell(SpellIds.Druid.BearForm) && Me.Level > 9 && Me.Level < 20, RotationCombatUtil.BotTarget),
            // Rotation 20-80
            new RotationStep(new RotationSpell(SpellIds.Druid.Regrowth), 10f, (s, t) => Me.HaveBuff(SpellIds.Druid.PredatorsSwiftness) && Me.HealthPercent < 70 && Me.ManaPercentage > 40, RotationCombatUtil.FindMe),
            new RotationStep(new RotationBuff(SpellIds.Druid.DireBearForm), 11f, (s, t) =>!Me.IsInGroup && RotationFramework.Enemies.Count(o => o.IsTargetingMe && o.Position.DistanceTo(t.Position) <= 20) >= Settings.Current.SoloFeralBearCount, RotationCombatUtil.FindMe),
            new RotationStep(new RotationBuff(SpellIds.Druid.BearForm), 12f, (s, t) =>!Me.IsInGroup && !SpellManager.KnowSpell(SpellIds.Druid.DireBearForm) && RotationFramework.Enemies.Count(o => o.IsTargetingMe && o.Position.DistanceTo(t.Position) <= 20) >= Settings.Current.SoloFeralBearCount, RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell(SpellIds.Druid.FaerieFireFeral), 12.3f, (s, t) => (Me.HaveBuff(SpellIds.Druid.BearForm) || Me.HaveBuff(SpellIds.Druid.DireBearForm)) && !Me.HaveBuff(SpellIds.Druid.Prowl) && !t.HaveMyBuff(SpellIds.Druid.FaerieFireFeral) && Settings.Current.SoloFeralFaerieFire, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.Druid.Bash), 12.1f, (s, t) => t.IsCasting(), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.Druid.Enrage), 12.2f, (s, t) =>t.HealthPercent >= 35 && !Me.HaveBuff(SpellIds.Druid.Enrage), RotationCombatUtil.FindMe),
            new RotationStep(new RotationBuff(SpellIds.Druid.FrenziedRegeneration), 13f, (s, t) => Me.HealthPercent < 60 && Me.Rage > 25, RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell(SpellIds.Druid.MangleBear), 14f, (s, t) => !t.HaveMyBuff(SpellIds.Druid.MangleBear) && (Me.HaveBuff(SpellIds.Druid.DireBearForm) || Me.HaveBuff(SpellIds.Druid.BearForm)), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.Druid.Maul), 15f, (s, t) => Me.Rage > 16 && (Me.HaveBuff(SpellIds.Druid.DireBearForm) || Me.HaveBuff(SpellIds.Druid.BearForm)) && RotationFramework.Enemies.Count(o => o.Position.DistanceTo(t.Position) <= 20) >= Settings.Current.SoloFeralBearCount, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.Druid.DemoralizingRoar), 16f, (s, t) =>!t.HaveBuff(SpellIds.Warrior.DemoralizingShout) && !t.HaveMyBuff(SpellIds.Druid.DemoralizingRoar) && RotationFramework.Enemies.Count(o => o.Position.DistanceTo(t.Position) <= 20) >= Settings.Current.SoloFeralBearCount, RotationCombatUtil.BotTarget),
            // Cat Rotation
            new RotationStep(new RotationBuff(SpellIds.Druid.CatForm), 19f, (s, t) =>!Me.IsInGroup && (!Me.HaveBuff(SpellIds.Druid.BearForm) || !Me.HaveBuff(SpellIds.Druid.DireBearForm)) && RotationFramework.Enemies.Count(o => o.IsTargetingMe && o.Position.DistanceTo(t.Position) <=12) <= (Settings.Current.SoloFeralBearCount - 1), RotationCombatUtil.FindMe),
            new RotationStep(new RotationBuff(SpellIds.Druid.CatForm), 19.1f, (s, t) =>!Me.IsInGroup && (Me.HaveBuff(SpellIds.Druid.BearForm) || Me.HaveBuff(SpellIds.Druid.DireBearForm)) && RotationFramework.Enemies.Count(o => o.IsTargetingMe && o.Position.DistanceTo(t.Position) <=12) <= (Settings.Current.SoloFeralBearCount - 2), RotationCombatUtil.FindMe),
            new RotationStep(new RotationBuff(SpellIds.Druid.CatForm), 19.2f, (s, t) => Me.IsInGroup, RotationCombatUtil.FindMe),

            new RotationStep(new RotationSpell(SpellIds.Druid.Prowl), 20f, (s, t) => !Me.HaveBuff(SpellIds.Druid.Prowl) && Settings.Current.SoloFeralProwl && t.GetDistance < 12, RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell(SpellIds.Druid.FeralChargeCat), 21f, (s, t) => Me.InCombat && t.GetDistance >= 15 && t.GetDistance <= 25 && Me.HaveBuff(SpellIds.Druid.Prowl) && !t.IsTargetingMe && Me.HaveBuff(SpellIds.Druid.CatForm), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.Druid.Pounce), 22f, (s, t) => Me.HaveBuff(SpellIds.Druid.Prowl) && t.GetDistance <=5, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.Druid.FaerieFireFeral), 22.1f, (s, t) => Settings.Current.SoloFeralForceFaerie && !t.HaveMyBuff(SpellIds.Druid.FaerieFireFeral), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.Druid.Ravage), 23f, (s, t) => Me.HaveBuff(SpellIds.Druid.Prowl) && Me.IsBehind(t.Position, 1.8f) && t.GetDistance <=6, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.Druid.FaerieFireFeral), 23.1f, (s, t) => !Me.HaveBuff(SpellIds.Druid.Prowl) && !t.HaveMyBuff(SpellIds.Druid.FaerieFireFeral) && Settings.Current.SoloFeralFaerieFire, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.Druid.Dash), 24f, (s, t) => Me.HaveBuff(SpellIds.Druid.Prowl) && !Me.HaveBuff(SpellIds.Druid.Dash) && Settings.Current.SoloFeralDash && Me.HaveBuff(SpellIds.Druid.CatForm), RotationCombatUtil.FindMe),
            new RotationStep(new RotationBuff(SpellIds.Druid.TigersFury), 25f, (s, t) => Settings.Current.SoloFeralTigersFury && t.GetDistance < 6, RotationCombatUtil.BotTarget),
            //new RotationStep(new RotationBuff("Tiger's Fury"), 25f, (s, t) => Me.HealthPercent < 92 && Me.ManaPercentage > 40 && !TalentsManager.HaveTalent(2,25) && t.GetDistance < 6, RotationCombatUtil.BotTarget),
            //new RotationStep(new RotationBuff("Tiger's Fury"), 26f, (s, t) => Me.ComboPoint <=4 && t.HealthPercent>=40 && Settings.Current.SoloFeralTigersFury && Me.HaveBuff("Cat Form") && !TalentsManager.HaveTalent(2,25) && t.GetDistance < 6, RotationCombatUtil.FindMe),
            //new RotationStep(new RotationBuff("Tiger's Fury"), 26.1f, (s, t) => Me.HealthPercent < 92 && Me.ManaPercentage > 40 && TalentsManager.HaveTalent(2,25)&& Me.HaveBuff("Cat Form") && Me.Rage <= 40 && t.GetDistance < 6, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.Druid.Rake), 27f, (s, t) =>!Me.HaveBuff(SpellIds.Druid.Prowl) && Me.ComboPoint <=4 && !t.HaveBuff(SpellIds.Druid.Rake) && (t.HealthPercent >= 35 || t.HealthPercent >= 20 && BossList.MyTargetIsBoss) && !t.IsCreatureType(wManager.Wow.Enums.CreatureType.Elemental), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.Druid.Rip), 28f, (s,t) => Me.ComboPoint >= Settings.Current.SoloFeralFinisherComboPoints && !t.HaveMyBuff(SpellIds.Druid.Rip) && t.HealthPercent >= Settings.Current.SoloFeralRipHealth && !t.IsCreatureType(wManager.Wow.Enums.CreatureType.Elemental),  RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.Druid.FerociousBite), 29f, (s, t) => Me.ComboPoint >= Settings.Current.SoloFeralFinisherComboPoints, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.Druid.MangleCat), 30f, (s, t) => !Me.HaveBuff(SpellIds.Druid.Prowl), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.Druid.Claw), 31f,(s,t)  => !Me.HaveBuff(SpellIds.Druid.Prowl), RotationCombatUtil.BotTarget)
        };
    }
}