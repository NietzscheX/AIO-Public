using AIO.Combat.Addons;
using AIO.Combat.Common;
using AIO.Framework;
using AIO.Helpers.Caching;
using AIO.Settings;
using System.Collections.Generic;
using System.Linq;
using wManager.Wow.Class;
using wManager.Wow.Helpers;
using wManager.Wow.ObjectManager;
using static AIO.Constants;

namespace AIO.Combat.Warrior
{
    using Settings = WarriorLevelSettings;

    internal class SoloArms : BaseRotation
    {
        private bool _heroicStrikeOn;
        private bool _cleaveOn;
        private int _nbEnemiesAroundMe;
        private int _nbEnemiesAroundMeCasting;
        private readonly Spell _battleStanceSpell = new Spell(SpellManager.GetSpellInfo(SpellIds.Warrior.BattleStance));
        List<WoWUnit> _enemiesAroundWithoutMyRend = new List<WoWUnit>();
        List<WoWUnit> _cleavableEnemies = new List<WoWUnit>();

        protected override List<RotationStep> Rotation => new List<RotationStep> {
            new RotationStep(new RotationAction("Precalculations", Precalculations), 0f, 500),
            new RotationStep(new RotationAction("Cache on spells", CacheActiveAbilities), 0f, 500),
            new RotationStep(new RotationAction("Check stance", CheckStance), 0f, 5000),

            new RotationStep(new RotationSpell(SpellIds.Warrior.Charge), 1f, (s,t) => t.GetDistance > 8 && !RangedPull.HasNearbyEnemies(t, 25), RotationCombatUtil.BotTargetFast, forcedTimerMS: 1000),
            new RotationStep(new RotationSpell(SpellIds.Warrior.VictoryRush), 2f, RotationCombatUtil.Always, RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell(SpellIds.Warrior.Overpower), 2.5f, RotationCombatUtil.Always, RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell(SpellIds.Warrior.Execute), 3f, RotationCombatUtil.Always, RotationCombatUtil.BotTargetFast),
            
            // Rage dumps
            new RotationStep(new RotationSpell(SpellIds.Warrior.Cleave), 4f, (s,t) => Me.CRage() > 50 && !_cleaveOn && !_heroicStrikeOn && _cleavableEnemies.Count > 0 && _nbEnemiesAroundMe >= Settings.Current.SoloArmsAoe, RotationCombatUtil.BotTargetFast, ignoreGCD: true),
            new RotationStep(new RotationSpell(SpellIds.Warrior.HeroicStrike), 5f, (s,t) => Me.CRage() > 50 && !_cleaveOn &&  !_heroicStrikeOn, RotationCombatUtil.BotTargetFast, ignoreGCD: true),
            
            // AOE
            new RotationStep(new RotationSpell(SpellIds.Warrior.Bladestorm), 5.5f, (s,t) => _nbEnemiesAroundMe >= Settings.Current.SoloArmsAoe, RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell(SpellIds.Warrior.SweepingStrikes), 6f, (s,t) => _nbEnemiesAroundMe >= Settings.Current.SoloArmsAoe, RotationCombatUtil.BotTargetFast, ignoreGCD: true),
            new RotationStep(new RotationSpell(SpellIds.Warrior.Rend), 7f, (s,t) => _nbEnemiesAroundMe >= Settings.Current.SoloArmsAoe && !t.CHaveMyBuff(SpellIds.Warrior.Rend), RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell(SpellIds.Warrior.Rend), 8.5f, (s,t) => Settings.Current.SoloArmsSpreadRend, p => _enemiesAroundWithoutMyRend.FirstOrDefault()),

            // Utility
            new RotationStep(new RotationSpell(SpellIds.Warrior.Hamstring), 9f, (s,t) => !t.CHaveBuff(SpellIds.Warrior.Hamstring) && t.CHealthPercent() < 40 && t.IsCreatureType(wManager.Wow.Enums.CreatureType.Humanoid) && !BossList.MyTargetIsBoss && Settings.Current.Hamstring, RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell(SpellIds.Warrior.EnragedRegeneration), 10f, (s,t) => Me.CHealthPercent() < Settings.Current.SoloArmsEnragedRegen, RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell(SpellIds.Warrior.BerserkerRage), 11f, (s,t) => Me.CHaveBuff("Fear"), RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell(SpellIds.Warrior.IntimidatingShout), 13f, (s,t) => Settings.Current.SoloArmsIntimShout && _nbEnemiesAroundMeCasting > 0, RotationCombatUtil.BotTargetFast),
            
            // Single Target Rotation
            new RotationStep(new RotationSpell(SpellIds.Warrior.HeroicThrow), 15f, RotationCombatUtil.Always, RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell(SpellIds.Warrior.Bloodrage), 16f, (s,t) => Me.CRage() < 50, RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell(SpellIds.Warrior.Rend), 17f, (s,t) => !t.CHaveMyBuff(SpellIds.Warrior.Rend), RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell(SpellIds.Warrior.MortalStrike), 18f, RotationCombatUtil.Always, RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell(SpellIds.Warrior.Slam), 19f, (s,t) => Settings.Current.SoloArmsSlam && Me.CRage() > 20, RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell(SpellIds.Warrior.ShatteringThrow), 20f, (s,t) => BossList.MyTargetIsBoss, RotationCombatUtil.BotTargetFast),
            new RotationStep(new RotationSpell(SpellIds.Warrior.Bladestorm), 21f, RotationCombatUtil.Always, RotationCombatUtil.BotTargetFast),

            new RotationStep(new RotationSpell(SpellIds.Warrior.DemoralizingShout), 22f, (s,t) => Settings.Current.SoloArmsDemoShout && !t.CHaveBuff(SpellIds.Warrior.DemoralizingShout), RotationCombatUtil.BotTargetFast),
        };

        private bool CheckStance()
        {
            if (_battleStanceSpell.KnownSpell && wManager.Wow.Helpers.Lua.LuaDoString<int>("return GetShapeshiftForm()") != 1)
                _battleStanceSpell.Launch();
            return false;
        }

        private bool CacheActiveAbilities()
        {
            bool[] result = Lua.LuaDoString<bool[]>($@"
                local result = {{}};
                local result = {{}};
                local cleavOn = IsCurrentSpell(GetSpellInfo({SpellIds.Warrior.Cleave})) == 1;
                local hsOn = IsCurrentSpell(GetSpellInfo({SpellIds.Warrior.HeroicStrike})) == 1;
                table.insert(result, cleavOn);
                table.insert(result, hsOn);
                return unpack(result);
            ");

            if (result.Length < 2) return false;

            _cleaveOn = result[0];
            _heroicStrikeOn = result[1];

            return false;
        }

        private bool Precalculations()
        {
            Cache.Reset();
            WoWUnit[] _enemiesAroundMe = RotationFramework.Enemies
                .Where(unit => unit.CIsTargetingMeOrMyPetOrPartyMember() && unit.CGetDistance() < 7)
                .ToArray();
            _nbEnemiesAroundMe = _enemiesAroundMe.Count();
            _nbEnemiesAroundMeCasting = _enemiesAroundMe
                .Where(enemy => enemy.CIsCast())
                .Count();
            _cleavableEnemies = _enemiesAroundMe
                .Where(enemy => Me.IsFacing(enemy.Position, 3))
                .ToList();
            _enemiesAroundWithoutMyRend = _cleavableEnemies
                .Where(enemy => enemy.CGetDistance() < 6 && !enemy.CHaveMyBuff(SpellIds.Warrior.Rend) && !enemy.Name.Contains("Totem"))
                .ToList();
            return false;
        }
    }
}
