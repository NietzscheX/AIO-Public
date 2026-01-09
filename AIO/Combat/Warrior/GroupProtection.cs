using AIO.Combat.Common;
using AIO.Framework;
using AIO.Helpers;
using AIO.Helpers.Caching;
using AIO.Settings;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using wManager.Wow.Class;
using wManager.Wow.ObjectManager;
using static AIO.Constants;

namespace AIO.Combat.Warrior
{
    using Settings = WarriorLevelSettings;
    internal class GroupProtection : BaseRotation
    {
        private WoWUnit[] EnemiesAttackingGroup = new WoWUnit[0];
        private Stopwatch watch = Stopwatch.StartNew();
        private readonly Spell _battleStanceSpell = new Spell(SpellManager.GetSpellInfo(SpellIds.Warrior.BattleStance));
        private readonly Spell _defensiveStanceSpell = new Spell(SpellManager.GetSpellInfo(SpellIds.Warrior.DefensiveStance));

        protected override List<RotationStep> Rotation => new List<RotationStep> {
            new RotationStep(new DebugSpell("Pre-Calculations"), 0.0f,
                (action, unit) => DoPreCalculations(), RotationCombatUtil.FindMe, checkRange: false, forceCast: true, ignoreGCD: true),
            new RotationStep(new RotationAction("Check stance", CheckStance), 0f, 5000),
            new RotationStep(new RotationSpell(SpellIds.Warrior.AutoAttack), 1f, (s,t) => !Me.IsCast && !RotationCombatUtil.IsAutoAttacking(), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.Warrior.LastStand), 1.1f, RotationCombatUtil.Always, _ => Me.CHealthPercent() < 15, RotationCombatUtil.FindMe, checkRange: false),
            new RotationStep(new RotationSpell(SpellIds.Warrior.ShieldWall), 2.11f, RotationCombatUtil.Always, _ => Me.CInCombat() && Me.CHealthPercent() < 65, RotationCombatUtil.FindMe, checkRange: false),
            // new RotationStep(new RotationSpell("Shield Wall"), 2.12f, (s,t) => BossList.MyTargetIsBoss || RotationFramework.Enemies.Count(o => o.CGetDistance() <=10) >=3, RotationCombatUtil.FindMe, checkRange: false),
            new RotationStep(new RotationSpell(SpellIds.Warrior.ShieldBlock), 2.15f, (s,t) => (t.CHealthPercent() > 70 || t.IsElite || BossList.MyTargetIsBoss) && EnemiesAttackingGroup.ContainsAtLeast(o => o.CGetDistance() <=10 && o.CIsTargetingMe(), Settings.Current.GroupProtectionShieldBlock), _ => Me.CHealthPercent() < 90, RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell(SpellIds.Warrior.EnragedRegeneration), 2.151f, RotationCombatUtil.Always, _ => Me.CHealthPercent() <= Settings.Current.GroupProtectionEnragedRegeneration && Me.CHaveBuff(SpellIds.Warrior.Bloodrage) && Me.CBuffTimeLeft(SpellIds.Warrior.Bloodrage) <= 2000, RotationCombatUtil.FindMe, checkRange: false),

            new RotationStep(new RotationSpell(SpellIds.Warrior.Intercept), 2.16f, (s,t) => Settings.Current.GroupProtectionIntercept && t.CGetDistance() > 8
                && RotationFramework.PartyMembers.Any(m => m.Position.DistanceTo(t.Position) < 7) && t.CIsTargetingMeOrMyPetOrPartyMember() && !t.CIsTargetingMe(), RotationCombatUtil.BotTargetFast, checkLoS: true),
            new RotationStep(new RotationSpell(SpellIds.Warrior.Charge), 2.17f, (s,t) => Settings.Current.GroupProtectionIntercept && t.CGetDistance() > 8
                && RotationFramework.PartyMembers.Any(m => m.Position.DistanceTo(t.Position) < 7)&& t.CIsTargetingMeOrMyPetOrPartyMember() && !t.CIsTargetingMe(), RotationCombatUtil.BotTargetFast, checkLoS: true, forcedTimerMS: 1000),

            new RotationStep(new RotationSpell(SpellIds.Warrior.ThunderClap), 2.18f, RotationCombatUtil.Always, _ => EnemiesAttackingGroup.Any(unit => unit.CGetDistance() < 8 && !unit.CHaveMyBuff(SpellIds.Warrior.ThunderClap)), RotationCombatUtil.FindMe, checkRange: false),
            new RotationStep(new RotationSpell(SpellIds.Warrior.ShieldBash), 2.19f, (s,t) => t.CCanInterruptCasting(), RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell(SpellIds.Warrior.SpellReflection), 2.2f, RotationCombatUtil.Always, _ => EnemiesAttackingGroup.Any(unit => unit.CIsTargetingMe() && unit.CIsCast()), RotationCombatUtil.FindMe, checkRange: false),
            new RotationStep(new RotationSpell(SpellIds.Warrior.ChallengingShout), 3.1f, RotationCombatUtil.Always, _ => Me.CIsInGroup() && EnemiesAttackingGroup.ContainsAtLeast(o => o.CGetDistance() <= 10 && !o.CIsTargetingMe(), 2), RotationCombatUtil.FindMe, checkRange: false),
            new RotationStep(new RotationSpell(SpellIds.Warrior.HeroicStrike), 3.2f, RotationCombatUtil.Always, _ => Me.CHaveBuff(SpellIds.Warrior.GlyphOfRevenge) && !RotationCombatUtil.IsCurrentSpell(SpellManager.GetSpellInfo(SpellIds.Warrior.HeroicStrike)), RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell(SpellIds.Warrior.MockingBlow), 3.21f, (s,t) => Me.CRage() >= 10 && !t.CIsTargetingMe() && t.CGetDistance() <= 7, RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell(SpellIds.Warrior.Taunt), 4f, (s,t) => Settings.Current.GroupProtectionTauntGroup, TauntUnitPrio),
            new RotationStep(new RotationSpell(SpellIds.Warrior.Taunt), 4.1f, (s,t) => !t.CIsTargetingMe(), _ => Settings.Current.GroupProtectionTauntGroup, FindEnemyAttackingGroup),
            new RotationStep(new RotationSpell(SpellIds.Warrior.MockingBlow), 4.15f, (s,t) => !t.CIsTargetingMe() && t.CGetDistance() <= 8, FindEnemyAttackingGroup),
            new RotationStep(new RotationSpell(SpellIds.Warrior.ShieldSlam), 5.1f, RotationCombatUtil.Always, _ => Me.CHaveBuff(SpellIds.Warrior.SwordAndBoard), RotationCombatUtil.BotTargetFast),
            // new RotationStep(new RotationSpell("Piercing Howl"), 6f, RotationCombatUtil.Always, _ => Me.CHealthPercent() < 40 && EnemiesAttackingGroup.ContainsAtLeast(o => o.CGetDistance() <=10 && !o.CHaveBuff("Piercing Howl"), 3), RotationCombatUtil.FindMe, checkRange: false),
            new RotationStep(new RotationSpell(SpellIds.Warrior.BerserkerRage), 7f, (s,t) => !Me.CHaveBuff(SpellIds.Warrior.EnragedRegeneration) && Me.CRage() < 50 && t.TargetObject.GetDistance < 8, RotationCombatUtil.FindMe, checkRange: false),
            new RotationStep(new RotationSpell(SpellIds.Warrior.Shockwave), 8f, RotationCombatUtil.Always, _ => EnemiesAttackingGroup.ContainsAtLeast(o => o.CGetDistance() < 10, 2), RotationCombatUtil.FindMe, checkRange: false),          
            // new RotationStep(new RotationSpell("Thunder Clap"), 10.1f, RotationCombatUtil.Always, _ => Me.CIsInGroup() && EnemiesAttackingGroup.Any(unit => unit.CGetDistance() < 8 && !unit.CHaveMyBuff("Thunder Clap")), RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell(SpellIds.Warrior.Revenge), 10.2f, RotationCombatUtil.Always, RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell(SpellIds.Warrior.ShieldSlam), 10.3f, RotationCombatUtil.Always, RotationCombatUtil.BotTargetFast),

            new RotationStep(new RotationSpell(SpellIds.Warrior.Devastate), 10.4f, (s,t) => (Me.CRage() > 70 || t.CMyBuffStack(SpellIds.Warrior.SunderArmor) < 5) && BossList.MyTargetIsBoss, RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell(SpellIds.Warrior.Devastate), 10.5f, (s,t) => Me.CRage() > 70 || !t.CHaveMyBuff(SpellIds.Warrior.SunderArmor), RotationCombatUtil.BotTargetFast),

            new RotationStep(new RotationSpell(SpellIds.Warrior.Bloodrage), 11f, (s,t) => Me.CRage() < 70, RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell(SpellIds.Warrior.Cleave), 13f, RotationCombatUtil.Always, _ => Me.CRage() > Settings.Current.GroupProtectionCleaveRageCount && !Me.CHaveBuff(SpellIds.Warrior.GlyphOfRevenge) && EnemiesAttackingGroup.ContainsAtLeast(o => o.CGetDistance() < 10, Settings.Current.GroupProtectionCleaveCount), RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell(SpellIds.Warrior.ConcussionBlow), 15f, (s,t) => t.CHealthPercent()  > 40, RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell(SpellIds.Warrior.DemoralizingShout), 19f, RotationCombatUtil.Always, _ => EnemiesAttackingGroup.ContainsAtLeast(unit => unit.CGetDistance() < 15 && unit.CHealthPercent() > 65 && !unit.CHaveBuff(SpellIds.Warrior.DemoralizingShout) && !unit.CHaveBuff(SpellIds.Druid.DemoralizingRoar), Settings.Current.GroupProtectionDemoralizingCount), RotationCombatUtil.FindMe, checkRange: false),
            new RotationStep(new RotationSpell(SpellIds.Warrior.Rend), 20f, (s,t) => t.CHealthPercent() >50 && !t.CHaveMyBuff(SpellIds.Warrior.Rend) && !t.IsCreatureType(wManager.Wow.Enums.CreatureType.Elemental), RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell(SpellIds.Warrior.VictoryRush), 21f, RotationCombatUtil.Always, _ => !Me.CHaveBuff(SpellIds.Warrior.DefensiveStance) && (Me.CHaveBuff(SpellIds.Warrior.BattleStance) || Me.CHaveBuff(SpellIds.Warrior.BerserkerStance)), RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell(SpellIds.Warrior.HeroicStrike), 22f, RotationCombatUtil.Always, _ => Me.CRage() >= 40 && Me.Level < 40 && !RotationCombatUtil.IsCurrentSpell(SpellManager.GetSpellInfo(SpellIds.Warrior.HeroicStrike)), RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell(SpellIds.Warrior.HeroicStrike), 23f, RotationCombatUtil.Always, _ => Me.CRage() >= 80 && Me.Level >= 40 && !RotationCombatUtil.IsCurrentSpell(SpellManager.GetSpellInfo(SpellIds.Warrior.HeroicStrike)), RotationCombatUtil.BotTargetFast),
        };

        private bool CheckStance()
        {
            if (!_defensiveStanceSpell.KnownSpell && wManager.Wow.Helpers.Lua.LuaDoString<int>("return GetShapeshiftForm()") != 1)
                _battleStanceSpell.Launch();
            if (_defensiveStanceSpell.KnownSpell && wManager.Wow.Helpers.Lua.LuaDoString<int>("return GetShapeshiftForm()") != 2)
                _defensiveStanceSpell.Launch();
            return false;
        }


        private bool DoPreCalculations()
        {
            if (LimitExecutionSpeed(100)) return true;
            Cache.Reset();
            EnemiesAttackingGroup = RotationFramework.Enemies.Where(unit => unit.CIsTargetingMeOrMyPetOrPartyMember())
                .ToArray();
            return false;
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

        public WoWUnit FindEnemyAttackingGroup(Func<WoWUnit, bool> predicate)
            => EnemiesAttackingGroup.FirstOrDefault(predicate);


        public WoWUnit TauntUnitPrio(Func<WoWUnit, bool> predicate)
        {
            List<WoWUnit> enemiesToTaunt = new List<WoWUnit>();
            foreach (WoWUnit unit in RotationFramework.PartyMembers)
            {
                foreach (WoWUnit attacker in EnemiesAttackingGroup)
                {
                    if (!attacker.CIsTargetingMe() && !enemiesToTaunt.Contains(attacker) && unit.CGetPosition().DistanceTo(attacker.CGetPosition()) > unit.CGetPosition().DistanceTo(Me.CGetPosition()))
                    {
                        enemiesToTaunt.AddSorted(attacker, a => unit.CGetPosition().DistanceTo(attacker.CGetPosition()));
                    }
                }
            }
            return enemiesToTaunt.FirstOrDefault();
        }
    }
}
