using AIO.Combat.Common;
using AIO.Framework;
using System.Collections.Generic;
using static AIO.Constants;

namespace AIO.Combat.Paladin
{
    internal class LowLevel : BaseRotation
    {
        protected override List<RotationStep> Rotation => new List<RotationStep> {
            new RotationStep(new RotationSpell(SpellIds.Paladin.AutoAttack), 1f, (s,t) => !Me.IsCast && !RotationCombatUtil.IsAutoAttacking(), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.Paladin.HolyLight), 2f, (s,t) => !Me.IsInGroup && Me.HealthPercent < 50, RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell(SpellIds.Paladin.HolyLight), 3f, (s,t) => t.HealthPercent <50 , RotationCombatUtil.FindPartyMember),
            new RotationStep(new RotationBuff(SpellIds.Paladin.SealOfRighteousness), 4f, RotationCombatUtil.Always, RotationCombatUtil.FindMe),
            new RotationStep(new RotationBuff(SpellIds.Paladin.BlessingOfMight), 5f, RotationCombatUtil.Always, RotationCombatUtil.FindMe),
            new RotationStep(new RotationBuff(SpellIds.Paladin.DevotionAura), 6f, RotationCombatUtil.Always, RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell(SpellIds.Paladin.JudgementOfLight), 7f, RotationCombatUtil.Always, RotationCombatUtil.BotTarget),
        };
    }
}