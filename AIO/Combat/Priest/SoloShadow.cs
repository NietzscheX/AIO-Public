using AIO.Combat.Common;
using AIO.Framework;
using AIO.Helpers;
using AIO.Helpers.Caching;
using AIO.Settings;
using robotManager.Helpful;
using System;
using System.Collections.Generic;
using System.Linq;
using wManager.Wow.Helpers;
using wManager.Wow.ObjectManager;
using static AIO.Constants;
using Math = System.Math;

namespace AIO.Combat.Priest
{
    using Settings = PriestLevelSettings;

    internal class SoloShadow : BaseRotation
    {
        private static readonly HashSet<ulong> PartyGuids = new HashSet<ulong>();
        private static CancelableSpell _healSpell;
        private static WoWUnit _tank;
        private static WoWUnit _target;
        private static bool _haveShadowWeaving;
        private static bool _targetAttackable;

        public SoloShadow()
        {
            _healSpell = FindCorrectHealSpell();
            _haveShadowWeaving = TalentsManager.HaveTalent(3, 12);

            if (Me.Level < 20)
            {
                Rotation.Add(new RotationStep(new RotationSpell(SpellIds.Priest.Smite), 18.1f, (s, t) => true,
                    CQuickBotTarget, checkLoS: true));
            }
        }

        protected sealed override List<RotationStep> Rotation => new List<RotationStep> {
            new RotationStep(new DebugSpell("Pre-Calculations"), 0f,
                (action, unit) => DoPreCalculations(), RotationCombatUtil.FindMe, checkRange: false, forceCast: true, ignoreGCD: true),

            new RotationStep(new RotationSpell(SpellIds.Priest.PowerWordShield), 1f,
                (s, t) => (Settings.Current.SoloShadowUseHeaInGrp || !Me.CIsInGroup()) &&
                          Me.CHealthPercent() <= Settings.Current.SoloShadowUseShieldTresh &&
                          !Me.CHaveBuff(SpellIds.Priest.PowerWordShield) && !Me.CHaveBuff(SpellIds.Priest.WeakenedSoul),
                RotationCombatUtil.FindMe, checkRange: false, forceCast: true),
            
            // new RotationStep(new RotationSpell("Fade"), 1.1f, RotationCombatUtil.Always,
            //     action => Me.CIsInGroup() && Me.GetCachedThreatSituation() > 1, RotationCombatUtil.FindMe, checkRange: false, forceCast: true),
            
            new RotationStep(_healSpell, 2f,
                (s, t) => (Settings.Current.SoloShadowUseHeaInGrp || !Me.CIsInGroup()) &&
                          Me.CHealthPercent() < Settings.Current.SoloShadowUseHealTresh, RotationCombatUtil.FindMe, checkRange: false),
            
            new RotationStep(new CancelableSpell(SpellIds.Priest.FlashHeal, unit => unit.CHealthPercent() > Settings.Current.SoloShadowUseFlashTresh + 10), 3f,
                (s, t) => (Settings.Current.SoloShadowUseHeaInGrp || !Me.CIsInGroup()) &&
                          Me.CHealthPercent() < Settings.Current.SoloShadowUseFlashTresh ||
                          Me.CHealthPercent() < Math.Min(Settings.Current.SoloShadowUseFlashTresh + 25, 99) && !Me.CHaveBuff(SpellIds.Priest.Shadowform),
                RotationCombatUtil.FindMe, checkRange: false),

            new RotationStep(new RotationSpell(SpellIds.Priest.Renew), 4f,
                (s, t) => (Settings.Current.SoloShadowUseHeaInGrp || !Me.CIsInGroup()) && 
                          !Me.CHaveBuff(SpellIds.Priest.Shadowform) && Me.CHealthPercent() < Settings.Current.SoloShadowUseRenewTresh && 
                          Me.CManaPercentage() > 40 && t.CBuffTimeLeft(SpellIds.Priest.Renew) < 1000, RotationCombatUtil.FindMe, checkRange: false),

            new RotationStep(new RotationSpell(SpellIds.Priest.PsychicScream), 5f,
                (s, t) => !Me.CIsInGroup() && Me.CHealthPercent() < 80 &&
                          RotationFramework.Enemies.Count(o => o.Target == Me.Guid && o.CGetDistance() <= 6) >= 2,
                RotationCombatUtil.FindMe, checkRange: false),

            new RotationStep(new RotationSpell(SpellIds.Priest.PowerWordShield), 6f,
                (s, t) => (Settings.Current.SoloShadowUseHeaInGrp || !Me.CIsInGroup()) && 
                          Settings.Current.SoloShadowUseShieldParty && t.CHealthPercent() < Settings.Current.SoloShadowUseShieldTresh &&
                          t.GetCachedThreatSituation() > 1 && !t.CHaveBuff(SpellIds.Priest.PowerWordShield) &&
                          !t.CHaveBuff(SpellIds.Priest.WeakenedSoul), pred => RotationFramework.PartyMembers.CFindInRange(unit => unit.CIsAlive() && pred(unit), 40, 5), checkLoS: true, checkRange: false),
            
            // Cast Shadowfiend on tank's combat partner to gain some mana
            new RotationStep(new RotationSpell(SpellIds.Priest.Shadowfiend), 7f,
                (action, target) => target.IsEnemy() && target.IsAttackable && target.Target == _tank.Guid,
                action => _tank != null && Me.CManaPercentage() < Settings.Current.SoloShadowShadowfiend + 10 && _tank.CInCombat(),
                predicate => predicate(_tank?.TargetObject) ? _tank?.TargetObject : null, checkLoS: true),
            
            // Cast Shadowfiend on high HP combat partner to gain some Mana
            new RotationStep(new RotationSpell(SpellIds.Priest.Shadowfiend), 8f,
                (action, target) => target.IsEnemy() && target.IsAttackable,
                action => Me.CManaPercentage() < Settings.Current.SoloShadowShadowfiend + 10,
                RotationCombatUtil.CGetHighestHpPartyMemberTarget, checkLoS: true),
            
            // Cast Shadowfiend on basically anything targeting me to gain some Mana
            new RotationStep(new RotationSpell(SpellIds.Priest.Shadowfiend), 9f,
                (action, target) => target.Target == Me.Guid && target.IsAttackable,
                action => Me.CManaPercentage() < Settings.Current.SoloShadowShadowfiend && Me.CInCombat(),
                predicate => RotationFramework.Enemies.FirstOrDefault(predicate), checkLoS: true),

            new RotationStep(new RotationSpell(SpellIds.Priest.Dispersion), 10f,
                (s, t) => Me.CManaPercentage() <= Settings.Current.SoloShadowDispersion && !Me.CHaveBuff(SpellIds.Priest.Dispersion) &&
                (Pet?.CreatedBySpell ?? 0) != SpellIds.Priest.Shadowfiend,
                RotationCombatUtil.FindMe, checkRange: false),

            new RotationStep(new RotationSpell(SpellIds.Priest.Shadowform), 11f, (s, t) => !Me.CHaveBuff(SpellIds.Priest.Shadowform),
                RotationCombatUtil.FindMe, checkRange: false),

            new RotationStep(new RotationSpell(SpellIds.Priest.Shoot), 12f,
                (s, t) => Settings.Current.UseWand &&
                          (t.CHealthPercent() <= Settings.Current.UseWandTresh || Me.CManaPercentage() < 5) &&
                          Extension.HaveRangedWeaponEquipped && !RotationCombatUtil.IsAutoRepeating(SpellIds.Priest.Shoot), CQuickBotTarget,
                checkLoS: true),

            new RotationStep(new RotationSpell(SpellIds.Priest.MindSear), 13f,
                (action, target) => {
                    ushort count = 0;
                    Vector3 targetPosition = target.PositionWithoutType;
                    int length = Math.Min(10, RotationFramework.Enemies.Length);
                    for (var i = 0; i < length; i++) {
                        WoWUnit enemy = RotationFramework.Enemies[i];
                        if (target.GetBaseAddress != enemy.GetBaseAddress && targetPosition.DistanceTo(enemy.PositionWithoutType) <= 11)
                            count++;
                        if (count >= 2) return true;
                    }

                    return false;
                }, action => Me.CManaPercentage() > 65 && !Me.GetMove,
                CQuickBotTarget, checkLoS: true),

            new RotationStep(new RotationSpell(SpellIds.Priest.VampiricTouch), 14f,
                (s, t) => t.CMyBuffTimeLeft(SpellIds.Priest.VampiricTouch) < 1300, CQuickBotTarget, checkLoS: true),

            new RotationStep(new RotationSpell(SpellIds.Priest.DevouringPlague), 15f,
                (s, t) => ((t.CHealthPercent() > 40 || t.IsBoss && t.CHealthPercent() > 15) &&
                          t.CMyBuffTimeLeft(SpellIds.Priest.DevouringPlague) < 2590) && Settings.Current.SoloShadowDPUse, CQuickBotTarget,
                checkLoS: true),

            new RotationStep(new RotationSpell(SpellIds.Priest.MindBlast), 16f, (s, t) => true,
                CQuickBotTarget, checkLoS: true),

            new RotationStep(new RotationSpell(SpellIds.Priest.ShadowWordPain), 17f,
                (action, target) => (!_haveShadowWeaving || Me.CBuffStack(SpellIds.Priest.ShadowWeaving) >= 5) && target.CMyBuffTimeLeft(SpellIds.Priest.ShadowWordPain) < 2800,
            CQuickBotTarget, checkLoS: true),

            // new RotationStep(new RotationSpell("Shadow Word: Pain"), 17.1f,
            //     (s, t) => PartyGuids.Contains(t.Target) && !t.CHaveMyBuff("Shadow Word: Pain"),
            //     action => Settings.Current.ShadowDotOff, 
            //     predicate => RotationFramework.Enemies.CFindInRange(predicate, 36f, 5), checkLoS: true),
            
            new RotationStep(new RotationSpell(SpellIds.Priest.MindFlay), 18f,
                (s, t) => Settings.Current.SoloShadowUseMindflay && (t.CHaveMyBuff(SpellIds.Priest.ShadowWordPain) ||
                          t.CHaveMyBuff(SpellIds.Priest.DevouringPlague)), action => !Me.GetMove, CQuickBotTarget, checkLoS: true),

            new RotationStep(new RotationSpell(SpellIds.Priest.VampiricEmbrace), 19f,
                (s, t) => t.CBuffTimeLeft(SpellIds.Priest.VampiricEmbrace) < 1000 * 60 * 5, RotationCombatUtil.FindMe,
                checkRange: false),
        };

        private static bool DoPreCalculations()
        {
            Cache.Reset();
            PartyGuids.Clear();
            foreach (WoWPlayer partyMember in RotationFramework.PartyMembers) PartyGuids.Add(partyMember.Guid);

            _tank = RotationCombatUtil.CFindTank(unit => true);

            const bool lazyTarget = false;

            _target = ObjectManager.Target;
            if (lazyTarget && (_tank?.IsValid ?? false) && !(_target?.IsValid ?? false) && _tank.CInCombat())
            {
                WoWUnit tmpTarget = null;
                long tmpHealth = 0;
                ulong tankGuid = _tank.Guid;
                foreach (WoWUnit enemy in RotationFramework.Enemies)
                {
                    if (enemy.Target == tankGuid)
                    {
                        long veryTmpHealth = enemy.Health;
                        if (veryTmpHealth > tmpHealth)
                        {
                            tmpTarget = enemy;
                            tmpHealth = veryTmpHealth;
                        }
                    }
                }

                if (tmpTarget != null)
                {
                    Me.Target = tmpTarget.Guid;
                }

                _target = ObjectManager.Target;
            }

            _targetAttackable = _target?.IsAttackable ?? false;
            return false;
        }

        private static CancelableSpell FindCorrectHealSpell()
        {
            if (SpellManager.KnowSpell(SpellIds.Priest.GreaterHeal))
                return new CancelableSpell(SpellIds.Priest.GreaterHeal,
                    unit => unit.CHealthPercent() > Settings.Current.SoloShadowUseHealTresh + 10);
            return SpellManager.KnowSpell(SpellIds.Priest.Heal)
                ? new CancelableSpell(SpellIds.Priest.Heal,
                    unit => unit.CHealthPercent() > Settings.Current.SoloShadowUseHealTresh + 10)
                : new CancelableSpell(SpellIds.Priest.LesserHeal,
                    unit => unit.CHealthPercent() > Settings.Current.SoloShadowUseHealTresh + 10);
        }

        private static WoWUnit CQuickBotTarget(Func<WoWUnit, bool> predicate)
        {
            return _target != null && _targetAttackable && predicate(_target) ? _target : null;
        }
    }
}
