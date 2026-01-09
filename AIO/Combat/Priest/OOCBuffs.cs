using AIO.Combat.Addons;
using AIO.Framework;
using System.Collections.Generic;
using wManager.Wow.Helpers;
using wManager.Wow.ObjectManager;
using static AIO.Constants;

namespace AIO.Combat.Priest
{
    internal class OOCBuffs : IAddon
    {
        private bool _hasCandle;
        public bool RunOutsideCombat => true;
        public bool RunInCombat => false;

        public List<RotationStep> Rotation => new List<RotationStep> {
            new RotationStep(new RotationAction("Candle Check", CandleCheck), 1500),
            new RotationStep(new RotationBuff(SpellIds.Priest.PrayerOfFortitude), 1f, (s,t) =>  !Me.IsMounted && _hasCandle && NeedsFort(t), RotationCombatUtil.FindPartyMember),
            new RotationStep(new RotationBuff(SpellIds.Priest.PrayerOfSpirit), 2f, (s,t) =>  !Me.IsMounted && _hasCandle && NeedsSpirit(t), RotationCombatUtil.FindPartyMember),
            new RotationStep(new RotationBuff(SpellIds.Priest.PrayerOfShadowProtection), 3f, (s,t) =>  !Me.IsMounted && _hasCandle && NeedsShadow(t), RotationCombatUtil.FindPartyMember),
            new RotationStep(new RotationBuff(SpellIds.Priest.PowerWordFortitude), 4f, (s,t) =>  !Me.IsMounted && NeedsFort(t), RotationCombatUtil.FindPartyMember),
            new RotationStep(new RotationBuff(SpellIds.Priest.DivineSpirit), 5f, (s,t) =>  !Me.IsMounted && NeedsSpirit(t), RotationCombatUtil.FindPartyMember),
            new RotationStep(new RotationBuff(SpellIds.Priest.ShadowProtection), 6f, (s,t) =>  !Me.IsMounted && NeedsShadow(t), RotationCombatUtil.FindPartyMember),
        };

        public void Initialize() { }
        public void Dispose() { }

        private bool CandleCheck()
        {
            _hasCandle = ItemsManager.HasItemById(17029) || ItemsManager.HasItemById(17028);
            return false;
        }

        private bool NeedsFort(WoWUnit target) => !target.HaveBuff(SpellIds.Priest.PowerWordFortitude) && !target.HaveBuff(SpellIds.Priest.PrayerOfFortitude);
        private bool NeedsSpirit(WoWUnit target) => !target.HaveBuff(SpellIds.Priest.DivineSpirit) && !target.HaveBuff(SpellIds.Priest.PrayerOfSpirit);
        private bool NeedsShadow(WoWUnit target) => !target.HaveBuff(SpellIds.Priest.ShadowProtection) && !target.HaveBuff(SpellIds.Priest.PrayerOfShadowProtection);
    }
}