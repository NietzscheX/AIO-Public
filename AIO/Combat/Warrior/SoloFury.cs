using AIO.Combat.Common;
using AIO.Framework;
using AIO.Settings;
using System.Collections.Generic;
using System.Linq;
using wManager.Wow.Class;
using wManager.Wow.Helpers;
using static AIO.Constants;

namespace AIO.Combat.Warrior
{
    using Settings = WarriorLevelSettings;
    internal class SoloFury : BaseRotation
    {
        private readonly bool KnowIntercept = Extension.KnowSpell(SpellIds.Warrior.Intercept);
        private readonly Spell _battleStanceSpell = new Spell(SpellManager.GetSpellInfo(SpellIds.Warrior.BattleStance));
        private readonly Spell _berserkerStanceSpell = new Spell(SpellManager.GetSpellInfo(SpellIds.Warrior.BerserkerStance));
        protected override List<RotationStep> Rotation => new List<RotationStep>
        {
            new RotationStep(new RotationAction("Check stance", CheckStance), 0f, 5000),
            new RotationStep(new RotationSpell(SpellIds.Warrior.AutoAttack), 1f, (s,t) => !Me.IsCast && !RotationCombatUtil.IsAutoAttacking(), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.Warrior.Pummel), 2f, (s,t) => t.IsCasting(), RotationCombatUtil.FindEnemyCasting),
            new RotationStep(new RotationSpell(SpellIds.Warrior.Hamstring), 3f, (s,t) => !t.HaveBuff(SpellIds.Warrior.Hamstring) && t.HealthPercent < 40 && t.IsCreatureType(wManager.Wow.Enums.CreatureType.Humanoid) && !BossList.MyTargetIsBoss && Settings.Current.Hamstring, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.Warrior.PiercingHowl), 4f, (s,t) => t.HealthPercent < 40 && RotationFramework.Enemies.Count(o => o.GetDistance <=10) >=3, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.Warrior.Bloodrage), 5f, (s,t) => t.GetDistance < 7, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.Warrior.Slam), 6f, (s,t) => Me.HaveBuff("Slam!"), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.Warrior.Bloodthirst), 7f, (s,t) => Me.Rage > 30 && Me.HealthPercent <= 80, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.Warrior.DeathWish), 8f, (s,t) => Me.Rage> 10, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.Warrior.Execute), 9f, (s1,t) => t.HealthPercent < 20, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.Warrior.VictoryRush), 10f, RotationCombatUtil.Always, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.Warrior.Rend), 11f, (s,t) => !t.HaveMyBuff(SpellIds.Warrior.Rend) && !t.IsCreatureType(wManager.Wow.Enums.CreatureType.Elemental), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.Warrior.Intercept), 12f, (s,t) => Settings.Current.SoloFuryIntercept && Me.Rage > 10 && t.GetDistance > 7 && t.GetDistance <= 24, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.Warrior.Charge), 13f, (s,t) => !KnowIntercept && Settings.Current.SoloFuryIntercept && t.GetDistance > 8, RotationCombatUtil.BotTarget, forcedTimerMS: 1000),
            new RotationStep(new RotationSpell(SpellIds.Warrior.ThunderClap), 14f, (s,t) => RotationFramework.Enemies.Count(o => o.GetDistance <=10) >=2, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.Warrior.Whirlwind), 15f, (s,t) => RotationFramework.Enemies.Count(o => o.GetDistance <=10) >=2, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.Warrior.Cleave), 16f, (s,t) => RotationFramework.Enemies.Count(o => o.GetDistance <=10) >=2, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.Warrior.HeroicStrike), 17f, (s,t) => Me.Rage > 40, RotationCombatUtil.BotTarget),
        };

        private bool CheckStance()
        {
            if (!_berserkerStanceSpell.KnownSpell && wManager.Wow.Helpers.Lua.LuaDoString<int>("return GetShapeshiftForm()") != 1)
                _battleStanceSpell.Launch();
            if (_berserkerStanceSpell.KnownSpell && wManager.Wow.Helpers.Lua.LuaDoString<int>("return GetShapeshiftForm()") != 3)
                _berserkerStanceSpell.Launch();
            return false;
        }
    }
}
