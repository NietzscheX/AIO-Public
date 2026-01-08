using AIO.Combat.Common;
using AIO.Framework;
using System.Collections.Generic;
using System.Linq;
using wManager.Wow.ObjectManager;

namespace AIO.Combat.Warrior
{
    internal class LowLevel : BaseRotation
    {
        protected override List<RotationStep> Rotation => new List<RotationStep> {
            new RotationStep(new RotationSpell(SpellIds.Warrior.AutoAttack), 1f, (s,t) => !ObjectManager.Me.IsCast && !RotationCombatUtil.IsAutoAttacking(), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.Warrior.Intercept), 2f, (s,t) => t.GetDistance > 7, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.Warrior.Charge), 3f, (s,t) => t.GetDistance > 8, RotationCombatUtil.BotTarget, forcedTimerMS: 1000),
            new RotationStep(new RotationSpell(SpellIds.Warrior.Rend), 4f, (s,t) => !t.HaveMyBuff(SpellIds.Warrior.Rend) && !t.IsCreatureType(wManager.Wow.Enums.CreatureType.Elemental), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.Warrior.VictoryRush), 5f, RotationCombatUtil.Always, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.Warrior.ThunderClap), 6f, (s,t) => RotationFramework.Enemies.Count(o => o.GetDistance <=10) >=2, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.Warrior.HeroicStrike), 7f, RotationCombatUtil.Always, RotationCombatUtil.BotTarget),
        };
    }
}