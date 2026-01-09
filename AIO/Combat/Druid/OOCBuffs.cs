using AIO.Combat.Addons;
using AIO.Framework;
using System.Collections.Generic;
using static AIO.Constants;

namespace AIO.Combat.Druid
{
    internal class OOCBuffs : IAddon
    {
        public bool RunOutsideCombat => true;
        public bool RunInCombat => false;

        public List<RotationStep> Rotation => new List<RotationStep> {
            new RotationStep(new RotationBuff(SpellIds.Druid.MarkOfTheWild), 1f, (s,t) => !Me.IsMounted && !t.HaveBuff(SpellIds.Druid.GiftOfTheWild) && !t.HaveBuff("Stamina") && !t.HaveBuff("Armor") && !t.HaveBuff("Agility") && !t.HaveBuff("Strength") && !t.HaveBuff("Spirit"), RotationCombatUtil.FindPartyMember),
            new RotationStep(new RotationBuff(SpellIds.Druid.MarkOfTheWild), 2f, (s,t) => !Me.IsMounted && !t.HaveBuff(SpellIds.Druid.GiftOfTheWild) && !t.HaveBuff("Stamina") && !t.HaveBuff("Armor") && !t.HaveBuff("Agility") && !t.HaveBuff("Strength") && !t.HaveBuff("Spirit"), RotationCombatUtil.FindMe),
            new RotationStep(new RotationBuff(SpellIds.Druid.Thorns), 3f,(s,t) => !Me.IsMounted && !t.HaveBuff(SpellIds.Druid.Thorns), RotationCombatUtil.FindTank),
            new RotationStep(new RotationBuff(SpellIds.Druid.Thorns), 4f, (s,t) => !Me.IsMounted, RotationCombatUtil.FindMe),
        };

        public void Initialize() { }
        public void Dispose() { }
    }
}
