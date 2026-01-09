using AIO.Combat.Common;
using AIO.Framework;
using System.Collections.Generic;
using static AIO.Constants;

namespace AIO.Combat.Hunter
{
    internal class LowLevel : BaseRotation
    {
        protected override List<RotationStep> Rotation => new List<RotationStep> {
            new RotationStep(new RotationSpell(SpellIds.Hunter.AutoAttack), 1f, (s,t) => Me.IsCast && !RotationCombatUtil.IsAutoAttacking(), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.Hunter.HuntersMark), 2f, (s,t) => !t.HaveMyBuff(SpellIds.Hunter.HuntersMark) && t.IsAlive && t.GetDistance >= 5 && t.HealthPercent > 50, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.Hunter.SerpentSting), 3f, (s,t) => t.GetDistance >= 5 && !t.HaveMyBuff(SpellIds.Hunter.SerpentSting), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.Hunter.ArcaneShot), 4f, (s,t) => t.GetDistance >= 5, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.Hunter.RaptorStrike), 5f, (s,t) => t.GetDistance < 5, RotationCombatUtil.BotTarget),
        };
    }
}
