using AIO.Combat.Addons;
using AIO.Framework;
using AIO.Helpers.Caching;
using System;
using System.Collections.Generic;
using System.Linq;
using wManager.Wow.Enums;
using wManager.Wow.ObjectManager;
using static AIO.Constants;
using AIO.Lists;

namespace AIO.Combat.Mage
{
    internal class OOCBuffs : IAddon
    {
        public bool RunOutsideCombat => true;
        public bool RunInCombat => false;

        private readonly List<WoWClass> _classesForFocusMagic = new List<WoWClass>()
        {
            WoWClass.Mage,
            WoWClass.Warlock,
            WoWClass.Paladin,
            WoWClass.Shaman,
            WoWClass.Priest,
            WoWClass.Druid
        };

        public List<RotationStep> Rotation => new List<RotationStep> {
            new RotationStep(new RotationAction("Reset cache", DoPreCalculations), 0f, 500),
            new RotationStep(new RotationBuff(SpellIds.Mage.ArcaneIntellect), 5f, (s,t) => !Me.IsMounted && !t.CHaveBuff(SpellIds.Warlock.FelIntelligence) && !t.CHaveBuff(SpellIds.Mage.ArcaneBrilliance), RotationCombatUtil.FindPartyMember),
            new RotationStep(new RotationBuff(SpellIds.Mage.ArcaneIntellect), 6f, (s,t) => !Me.IsMounted && !t.CHaveBuff(SpellIds.Warlock.FelIntelligence) && !t.CHaveBuff(SpellIds.Mage.ArcaneBrilliance), RotationCombatUtil.FindMe),
            new RotationStep(new RotationBuff(SpellIds.Mage.FocusMagic), 7f, (s,t) => !Me.IsMounted, FindPartyMemberForFocusMagic),
        };

        private bool DoPreCalculations()
        {
            Cache.Reset();
            return false;
        }

        private WoWPlayer FindPartyMemberForFocusMagic(Func<WoWUnit, bool> predicate)
        {
            if (RotationFramework.PartyMembers.Any(m => m.CHaveMyBuff(SpellIds.Mage.FocusMagic)))
                return null;

            WoWPlayer player = RotationFramework.PartyMembers
                .Where(m => m.Name != Me.Name && _classesForFocusMagic.Contains(m.WowClass))
                .OrderByDescending(m => m.MaxMana)
                .FirstOrDefault();

            if (player != null && player.GetDistance < 30 && player.IsAlive)
                return player;

            return null;
        }

        public void Initialize() { }
        public void Dispose() { }
    }
}


