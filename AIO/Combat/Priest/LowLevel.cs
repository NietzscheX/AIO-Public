using AIO.Combat.Common;
using AIO.Framework;
using AIO.Settings;
using System.Collections.Generic;
using static AIO.Constants;
namespace AIO.Combat.Priest
{
    using Settings = PriestLevelSettings;
    internal class LowLevel : BaseRotation
    {
        protected override List<RotationStep> Rotation => new List<RotationStep> {
            new RotationStep(new RotationSpell(SpellIds.Priest.Shoot), 0.9f, (s,t) => Settings.Current.UseWand && Me.ManaPercentage < Settings.Current.UseWandTresh && !RotationCombatUtil.IsAutoRepeating(SpellIds.Priest.Shoot), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.Priest.AutoAttack), 1f, (s,t) => !Me.IsCast && !RotationCombatUtil.IsAutoAttacking() && !RotationCombatUtil.IsAutoRepeating(SpellIds.Priest.Shoot), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationBuff(SpellIds.Priest.PowerWordFortitude), 2.1f, (s,t) => !Me.HaveBuff(SpellIds.Priest.PowerWordFortitude) && Me.ManaPercentage > 50, RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell(SpellIds.Priest.PowerWordShield), 3f, (s,t) => Me.HealthPercent < 99 && !Me.HaveBuff(SpellIds.Priest.PowerWordShield), RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell(SpellIds.Priest.LesserHeal), 5f, (s,t) => !Me.HaveBuff(SpellIds.Priest.LesserHeal) && Me.HealthPercent < 75, RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell(SpellIds.Priest.Renew), 7f, (s,t) =>  Me.HealthPercent < 90 && !Me.HaveBuff(SpellIds.Priest.Renew) , RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell(SpellIds.Priest.FlashHeal), 9f, (s,t) => Me.HealthPercent < 60, RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell(SpellIds.Priest.ShadowWordPain), 11f, (s,t) => (Target.HealthPercent > Settings.Current.UseWandTresh || Me.ManaPercentage < 5) && !t.HaveMyBuff(SpellIds.Priest.ShadowWordPain), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.Priest.Smite), 12f, (s,t) => Target.HealthPercent > Settings.Current.UseWandTresh || Me.ManaPercentage < 5, RotationCombatUtil.BotTarget),
        };
    }
}
