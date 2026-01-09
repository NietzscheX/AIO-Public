using AIO.Combat.Common;
using AIO.Framework;
using System.Collections.Generic;
using static AIO.Constants;

namespace AIO.Combat.Druid
{
    internal class LowLevel : BaseRotation
    {
        protected override List<RotationStep> Rotation => new List<RotationStep> {
            new RotationStep(new RotationSpell(SpellIds.Druid.AutoAttack), 1f, (s,t) => !Me.IsCast && !RotationCombatUtil.IsAutoAttacking(), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.Druid.HealingTouch), 2f, (s, t) => Me.HealthPercent <= 30, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.Druid.Starfire), 3f, (s, t) => t.HealthPercent == 100 && !t.IsTargetingMe, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.Druid.Moonfire), 4f, (s, t) => !t.HaveMyBuff(SpellIds.Druid.Moonfire), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.Druid.Wrath), 5f, RotationCombatUtil.Always, RotationCombatUtil.BotTarget),
        };
    }
}
