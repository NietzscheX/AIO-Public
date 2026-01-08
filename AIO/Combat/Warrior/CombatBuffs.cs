using AIO.Combat.Addons;
using AIO.Combat.Common;
using AIO.Framework;
using AIO.Lists;
using System.Collections.Generic;
using wManager.Wow.Helpers;
using static AIO.Constants;

namespace AIO.Combat.Warrior
{
    internal class CombatBuffs : IAddon
    {
        private readonly BaseCombatClass CombatClass;
        private bool KnowBerserkerStance;
        private Spec Spec => CombatClass.Specialisation;

        public bool RunOutsideCombat => true;
        public bool RunInCombat => true;

        internal CombatBuffs(BaseCombatClass combatClass)
        {
            CombatClass = combatClass;
            KnowBerserkerStance = SpellManager.KnowSpell("Berserker Stance");
        }

        public List<RotationStep> Rotation => new List<RotationStep> {
            new RotationStep(new RotationBuff(SpellIds.Warrior.Vigilance), 1f,(s,t) => !Me.IsMounted && !t.HaveBuff(SpellIds.Warrior.Vigilance), RotationCombatUtil.FindHeal),
            new RotationStep(new RotationBuff(SpellIds.Warrior.BattleShout), 2f, (s,t) => !Me.IsMounted && !t.HaveBuff(SpellIds.Paladin.GreaterBlessingOfMight), RotationCombatUtil.FindMe),
            new RotationStep(new RotationBuff(SpellIds.Warrior.DefensiveStance), 3f, (s,t) => !Me.IsMounted && Spec == Spec.Warrior_GroupProtection, RotationCombatUtil.FindMe),
            new RotationStep(new RotationBuff(SpellIds.Warrior.BattleStance), 4f, (s,t) => !Me.IsMounted && Spec == Spec.Warrior_SoloArms, RotationCombatUtil.FindMe),
            new RotationStep(new RotationBuff(SpellIds.Warrior.BerserkerStance), 4f, (s,t) => !Me.IsMounted && (Spec == Spec.Warrior_SoloFury || Spec == Spec.Warrior_GroupFury), RotationCombatUtil.FindMe),
            // Fallback for fury
            new RotationStep(new RotationBuff(SpellIds.Warrior.BattleStance), 4f, (s,t) => !Me.IsMounted && (Spec == Spec.Warrior_SoloFury|| Spec == Spec.Warrior_GroupFury) && !KnowBerserkerStance, RotationCombatUtil.FindMe),
        };

        public void Initialize() { }
        public void Dispose() { }
    }
}
