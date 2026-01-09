using AIO.Combat.Addons;
using AIO.Framework;
using AIO.Settings;
using System.Collections.Generic;
using static AIO.Constants;

namespace AIO.Combat.Priest
{
    using Settings = PriestLevelSettings;

    internal class CombatBuffs : IAddon
    {
        public bool RunOutsideCombat => true;
        public bool RunInCombat => true;

        public List<RotationStep> Rotation => new List<RotationStep> {
            new RotationStep(new RotationBuff(SpellIds.Priest.InnerFire, minimumStacks: 2), 1f, (s, t) => Settings.Current.InnerFire && !Me.IsMounted, RotationCombatUtil.FindMe),
            new RotationStep(new RotationBuff(SpellIds.Priest.Shadowform), 2f, (s, t) => !Me.IsMounted, RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell(SpellIds.Priest.VampiricEmbrace), 7f, (s, t) => !Me.IsMounted && !Me.HaveBuff(SpellIds.Priest.VampiricEmbrace), RotationCombatUtil.FindMe),
        };

        public void Initialize() { }
        public void Dispose() { }
    }
}