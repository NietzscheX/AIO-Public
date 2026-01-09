using AIO.Combat.Common;
using AIO.Framework;
using AIO.Settings;
using System.Collections.Generic;
using System.Linq;
using wManager.Wow.Class;
using static AIO.Constants;

namespace AIO.Combat.Druid
{
    using Settings = DruidLevelSettings;

    internal class GroupFeral : BaseRotation
    {
        private readonly Spell _catFormSpell = new Spell(SpellIds.Druid.CatForm);
        private readonly Spell _bearFormSpell = new Spell(SpellIds.Druid.BearForm);
        private readonly Spell _direBearFormSpell = new Spell(SpellIds.Druid.DireBearForm);
        private readonly int _nbPointsKotJTalent = TalentsManager.GetNbPointsTalent(2, 25);

        protected override List<RotationStep> Rotation => new List<RotationStep> {
            // Rotation 11-20
            new RotationStep(new RotationSpell(SpellIds.Druid.AutoAttack), 1f, (s,t) => !RotationCombatUtil.IsAutoAttacking(), RotationCombatUtil.BotTargetFast),

            // High prio
            new RotationStep(new RotationBuff(SpellIds.Druid.Innervate), 1.2f, (s, t) => t.ManaPercentage <= 25 && Settings.Current.GroupFeralInnervateHealer, RotationCombatUtil.FindHeal),            
            new RotationStep(new RotationBuff(SpellIds.Druid.Barkskin), 1.4f, (s, t) => Me.HealthPercent <= 35, RotationCombatUtil.FindMe),
            
            // bear
            new RotationStep(new RotationBuff(SpellIds.Druid.BearForm), 2f, (s, t) => !_catFormSpell.KnownSpell && !_direBearFormSpell.KnownSpell, RotationCombatUtil.FindMe),
            new RotationStep(new RotationBuff(SpellIds.Druid.DireBearForm), 3f, (s, t) => !_catFormSpell.KnownSpell, RotationCombatUtil.FindMe),
            new RotationStep(new RotationBuff(SpellIds.Druid.FrenziedRegeneration), 4f, (s, t) => Me.HealthPercent < 60 && Me.Rage > 25, RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell(SpellIds.Druid.FaerieFireFeral), 5f, (s, t) => !t.HaveMyBuff(SpellIds.Druid.FaerieFireFeral) && Settings.Current.GroupFeralUseFaerieFire, RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell(SpellIds.Druid.MangleBear), 6f, (s, t) => !t.HaveMyBuff(SpellIds.Druid.MangleBear), RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell(SpellIds.Druid.Enrage), 7f, (s, t) => t.HealthPercent >= 35, RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell(SpellIds.Druid.Bash), 8f, (s, t) => t.IsCasting(), RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell(SpellIds.Druid.DemoralizingRoar), 9f, (s, t) => !t.HaveMyBuff(SpellIds.Druid.DemoralizingRoar), RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell(SpellIds.Druid.SwipeBear), 10f, (s, t) => RotationFramework.Enemies.Count(o => Me.IsFacing(o.Position, 3) && o.HasTarget && !o.IsTargetingMe && o.Position.DistanceTo(Me.Position) <= 8) >= 2, RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell(SpellIds.Druid.Maul), 11f, (s, t) => Me.Rage >= 16, RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell(SpellIds.Druid.DemoralizingRoar), 12f, (s, t) => !t.HaveBuff(SpellIds.Warrior.DemoralizingShout) && Settings.Current.GroupFeralUseDemoralizingRoar, RotationCombatUtil.BotTargetFast),

            // human
            new RotationStep(new RotationSpell(SpellIds.Druid.Moonfire), 13f, (s, t) => !_direBearFormSpell.KnownSpell && !_bearFormSpell.KnownSpell && !_catFormSpell.KnownSpell && !t.HaveBuff(SpellIds.Druid.Moonfire), RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell(SpellIds.Druid.Wrath), 14f, (s, t) => !_direBearFormSpell.KnownSpell && !_bearFormSpell.KnownSpell && !_catFormSpell.KnownSpell, RotationCombatUtil.BotTargetFast),

            // meow
            new RotationStep(new RotationBuff(SpellIds.Druid.CatForm), 15f, (s, t) => Me.IsInGroup, RotationCombatUtil.FindMe),

            // stealth
            new RotationStep(new RotationSpell(SpellIds.Druid.Pounce), 16f, (s, t) => true, RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell(SpellIds.Druid.Ravage), 17f, (s, t) => true, RotationCombatUtil.BotTargetFast),

            new RotationStep(new RotationSpell(SpellIds.Druid.FeralChargeCat), 18f, (s, t) => t.GetDistance > 7 && RotationFramework.PartyMembers.Any(m => m.Position.DistanceTo(t.Position) < 7), RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell(SpellIds.Druid.Dash), 19f, (s, t) => t.GetDistance > 10 && RotationFramework.PartyMembers.Any(m => m.Position.DistanceTo(t.Position) < 7), RotationCombatUtil.FindMe),

            new RotationStep(new RotationSpell(SpellIds.Druid.Berserk), 20f, (s,t) => Me.HaveBuff(SpellIds.Druid.CatForm) && (RotationFramework.Enemies.Count(o => Me.IsFacing(o.Position, 3) && o.Position.DistanceTo(Me.Position) <= 8) >= 2 || t.IsBoss), RotationCombatUtil.FindMe),
            new RotationStep(new RotationBuff(SpellIds.Druid.TigersFury), 21f, (s, t) => Me.Energy < 100 - (_nbPointsKotJTalent * 20) && t.GetDistance < 10, RotationCombatUtil.BotTargetFast),

            // finisher
            new RotationStep(new RotationSpell(SpellIds.Druid.SavageRoar), 22f, (s,t) => !Me.HaveBuff(SpellIds.Druid.SavageRoar) && Me.ComboPoint >= Settings.Current.GroupFeralFinisherComboPoints, RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell(SpellIds.Druid.Rip), 23f, (s,t) => Me.ComboPoint >= Settings.Current.GroupFeralFinisherComboPoints && t.HealthPercent > 50 && !t.HaveMyBuff(SpellIds.Druid.Rip) && !t.IsCreatureType(wManager.Wow.Enums.CreatureType.Elemental),  RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell(SpellIds.Druid.FerociousBite), 24f, (s, t) => Me.ComboPoint >= Settings.Current.GroupFeralFinisherComboPoints, RotationCombatUtil.BotTargetFast),

            // combo points
            new RotationStep(new RotationSpell(SpellIds.Druid.Rake), 27f, (s, t) => !t.HaveBuff(SpellIds.Druid.Rake) && !t.IsCreatureType(wManager.Wow.Enums.CreatureType.Elemental), RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell(SpellIds.Druid.Shred), 29f, (s, t) => !t.IsFacing(Me.Position, 4), RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell(SpellIds.Druid.MangleCat), 30f, (s, t) => (t.IsFacing(Me.Position, 3) || !t.HaveBuff(SpellIds.Druid.MangleCat)) && !Me.HaveBuff(SpellIds.Druid.Prowl), RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell(SpellIds.Druid.Claw), 31f,(s,t)  => t.IsFacing(Me.Position, 3) && !Me.HaveBuff(SpellIds.Druid.Prowl), RotationCombatUtil.BotTargetFast)
        };
    }
}