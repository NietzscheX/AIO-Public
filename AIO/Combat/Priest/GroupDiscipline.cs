using AIO.Combat.Common;
using AIO.Framework;
using AIO.Helpers;
using AIO.Helpers.Caching;
using AIO.Settings;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using wManager.Wow.Helpers;
using wManager.Wow.ObjectManager;
using static AIO.Constants;

namespace AIO.Combat.Priest
{
    using Settings = PriestLevelSettings;
    internal class GroupDiscipline : BaseRotation
    {
        private WoWUnit[] EnemiesAttackingGroup = new WoWUnit[0];
        private Stopwatch watch = Stopwatch.StartNew();
        private List<WoWPlayer> _hurtPartyMembers = new List<WoWPlayer>(0);

        protected override List<RotationStep> Rotation => new List<RotationStep> {
            new RotationStep(new DebugSpell("Pre-Calculations"), 0.0f,(action, unit) => DoPreCalculations(), RotationCombatUtil.FindMe, checkRange : false, forceCast : true, ignoreGCD : true),
            new RotationStep(new RotationSpell(SpellIds.Priest.AutoAttack), 1f, (s,t) => !Me.IsCast && !RotationCombatUtil.IsAutoAttacking(), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.Priest.PowerWordShield), 2f, (action,tank)  => !tank.CHaveBuff(SpellIds.Priest.PowerWordShield) && !tank.CHaveBuff(SpellIds.Priest.WeakenedSoul) && tank.InCombat, RotationCombatUtil.FindTank, checkLoS: true),
            new RotationStep(new RotationSpell(SpellIds.Priest.PowerWordShield), 2.1f, (action,me)  => !me.CHaveBuff(SpellIds.Priest.PowerWordShield) && !me.CHaveBuff(SpellIds.Priest.WeakenedSoul) && me.CHealthPercent() < 100, RotationCombatUtil.FindMe, checkLoS: true),
            new RotationStep(new RotationSpell(SpellIds.Priest.PowerWordShield), 2.2f, (s,t) => !t.CHaveBuff(SpellIds.Priest.PowerWordShield) && !t.CHaveBuff(SpellIds.Priest.WeakenedSoul) && t.CHealthPercent() <= 80, RotationCombatUtil.FindPartyMember, checkLoS: true),
            //Heal Over Time
            new RotationStep(new RotationSpell(SpellIds.Priest.Renew), 3f, (action,tank)  => !tank.CHaveMyBuff(SpellIds.Priest.Renew) && tank.CHealthPercent() <= 90, RotationCombatUtil.FindTank, checkLoS: true),
            new RotationStep(new RotationSpell(SpellIds.Priest.Renew), 3.1f, (action,me)  => !me.CHaveMyBuff(SpellIds.Priest.Renew) && me.CHealthPercent() <= 90, RotationCombatUtil.FindMe, checkLoS: true),
            new RotationStep(new RotationSpell(SpellIds.Priest.Renew), 3.2f, (s,t) => !t.CHaveMyBuff(SpellIds.Priest.Renew) && t.CHealthPercent() <= 90, RotationCombatUtil.FindPartyMember, checkLoS: true),
            //Prayer of Mending
            new RotationStep(new RotationSpell(SpellIds.Priest.PrayerOfMending), 3.5f, (action,tank) => 
                tank.CHealthPercent() <= 80 && tank.CHaveMyBuff(SpellIds.Priest.PrayerOfMending), RotationCombatUtil.FindTank, checkLoS: true),
            //Oh Shit Heals
            new RotationStep(new RotationSpell(SpellIds.Priest.InnerFocus), 3.5f, (s, t) => _hurtPartyMembers.ContainsAtLeast(p=> p.CHealthPercent() <= 60, 1),RotationCombatUtil.FindMe, checkLoS: false), 
            new RotationStep(new RotationSpell(SpellIds.Priest.DivineHymn), 3.6f, (s,t) => Me.CHaveBuff(SpellIds.Priest.InnerFocus) && _hurtPartyMembers.ContainsAtLeast(p=> p.CHealthPercent() <= 60, 2), RotationCombatUtil.FindMe, checkLoS: false),
            new RotationStep(new RotationSpell(SpellIds.Priest.Penance), 4f, (action, tank)  => tank.CHealthPercent() <= 60, RotationCombatUtil.FindTank, checkLoS: true),
            new RotationStep(new RotationSpell(SpellIds.Priest.Penance), 4.1f, (s, t)  => t.CHealthPercent() <= 60, FindLowestPartyMember, checkLoS: true),            
            //Heals
            new RotationStep(new RotationSpell(SpellIds.Priest.BindingHeal), 5f, (action, tank)  => tank.CHealthPercent() <= 80 && Me.CHealthPercent() <= 90, RotationCombatUtil.FindTank, checkLoS: true),
            new RotationStep(new RotationSpell(SpellIds.Priest.BindingHeal), 5.1f, (s, t)  => t.CHealthPercent() <= 80 && Me.CHealthPercent() <= 90, RotationCombatUtil.FindPartyMember, checkLoS: true),
            new RotationStep(new RotationSpell(SpellIds.Priest.FlashHeal), 6f, (action, tank)  => tank.CHealthPercent() <= 80, RotationCombatUtil.FindTank, checkLoS: true),
            new RotationStep(new RotationSpell(SpellIds.Priest.FlashHeal), 6.1f, (s, t)  => t.CHealthPercent() <= 80, RotationCombatUtil.FindPartyMember, checkLoS: true),

        };

        private bool DoPreCalculations()
        {
            if (LimitExecutionSpeed(100))
            {
                return true;
            }
            Cache.Reset();
            EnemiesAttackingGroup = RotationFramework.Enemies.Where(unit => unit.CIsTargetingMeOrMyPetOrPartyMember())
                .ToArray();
            ClearLists();
            BuildLists();
            return false;
        }

        private void BuildLists()
        {
            for (int i = 0; i < RotationFramework.PartyMembers.Count(); i++)
            {
                WoWPlayer Partymember = RotationFramework.PartyMembers[i];
                if (Partymember.CHealthPercent() < 99)
                {
                    _hurtPartyMembers.Add(Partymember);
                }
            }
        }

        //clear prebuilded Lists
        private void ClearLists()
        {
            _hurtPartyMembers.Clear();
        }
        private bool LimitExecutionSpeed(int delay)
        {
            if (watch.ElapsedMilliseconds > delay)
            {
                watch.Restart();
                return false;
            }
            return true;
        }

        public WoWUnit FindEnemyAttackingGroup(Func<WoWUnit, bool> predicate) => EnemiesAttackingGroup.FirstOrDefault(predicate);

        public WoWUnit FindLowestPartyMember(Func<WoWUnit, bool> predicate) => _hurtPartyMembers.FirstOrDefault(predicate);
    }
}
