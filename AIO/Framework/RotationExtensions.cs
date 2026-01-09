using AIO.Lists;
using robotManager.Helpful;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using wManager.Wow.Class;
using wManager.Wow.Enums;
using wManager.Wow.Helpers;
using wManager.Wow.ObjectManager;
using static AIO.Constants;

namespace AIO.Framework
{
    public static class Extensions
    {
        private static readonly ConcurrentDictionary<int, string> CreatureTypeCache = new ConcurrentDictionary<int, string>();
        /*
         * The reason I removed this is because it was oftentime used repeatedly
         * in a loop on group members, severely slowing down the entire rotation
         * Check RotationCombatUtils _cachedDebuffedPlayers instead
        public static bool HasDebuffType(this WoWUnit unit, params string[] types)
        {
            return RotationCombatUtil.ExecuteActionOnUnit(unit, (luaUnitId) =>
            {
                var conditions = types.Select(type => $@"(debuffType == ""{type}"")").Aggregate((current, next) => $@"{current} or {next}");
                string luaString = $@"
                    for i=1,10 do
                        local name, rank, iconTexture, count, debuffType, duration, timeLeft = UnitDebuff(""{luaUnitId}"", i);
                        if ({conditions}) then
                            return true;
                        end
                    end
                    return false;";
                return Lua.LuaDoString<bool>(luaString);
            });
        }
        */
        public static bool IsCasting(this WoWUnit unit)
        {
            return RotationCombatUtil.ExecuteActionOnUnit(unit, (luaUnitId) =>
            {
                string luaString = $@"return (UnitCastingInfo(""{luaUnitId}"") ~= nil or UnitChannelInfo(""{luaUnitId}"") ~= nil)";
                return Lua.LuaDoString<bool>(luaString);
            });
        }

        public static bool IsCreatureType(this WoWUnit unit, string creatureType) =>
            CreatureTypeCache.GetOrAdd(unit.Entry, k =>
                RotationCombatUtil.ExecuteActionOnUnit(unit, (luaUnitId) =>
                {
                    var luaString = $@"return UnitCreatureType(""{luaUnitId}"")";
                    return Lua.LuaDoString<string>(luaString);
                })
            ) == creatureType;

        public static bool IsCreatureType(this WoWUnit unit, AIO.Enums.CreatureType creatureType)
        {
            string globalStringKey = null;
            switch (creatureType)
            {
                case AIO.Enums.CreatureType.Humanoid: globalStringKey = "CREATURE_TYPE_HUMANOID"; break;
                case AIO.Enums.CreatureType.Undead: globalStringKey = "CREATURE_TYPE_UNDEAD"; break;
                case AIO.Enums.CreatureType.Beast: globalStringKey = "CREATURE_TYPE_BEAST"; break;
                case AIO.Enums.CreatureType.Demon: globalStringKey = "CREATURE_TYPE_DEMON"; break;
                case AIO.Enums.CreatureType.Dragonkin: globalStringKey = "CREATURE_TYPE_DRAGONKIN"; break;
                case AIO.Enums.CreatureType.Elemental: globalStringKey = "CREATURE_TYPE_ELEMENTAL"; break;
                case AIO.Enums.CreatureType.Giant: globalStringKey = "CREATURE_TYPE_GIANT"; break;
                case AIO.Enums.CreatureType.Mechanical: globalStringKey = "CREATURE_TYPE_MECHANICAL"; break;
                case AIO.Enums.CreatureType.Critter: globalStringKey = "CREATURE_TYPE_CRITTER"; break;
                default: return false;
            }

            string targetType = CreatureTypeCache.GetOrAdd(unit.Entry, k =>
                RotationCombatUtil.ExecuteActionOnUnit(unit, (luaUnitId) =>
                {
                    var luaString = $@"return UnitCreatureType(""{luaUnitId}"")";
                    return Lua.LuaDoString<string>(luaString);
                })
            );

            if (string.IsNullOrEmpty(targetType)) return false;

            string localizedType = Lua.LuaDoString<string>($"return _G['{globalStringKey}']");
            return targetType == localizedType;
        }

        public static bool HasMana(this WoWUnit unit) => unit is WoWPlayer wUnit && wUnit.PowerType == PowerType.Mana
                                                         || !(unit is WoWPlayer) && unit.MaxMana > 1;

        public static IEnumerable<Aura> GetMyAuras(this WoWUnit unit) => BuffManager.GetAuras(unit.GetBaseAddress).Where(a => a.Owner == Me.Guid);

        public static IEnumerable<Aura> GetMyBuffs(this WoWUnit unit, params string[] names) => GetMyAuras(unit).Where(a => names.Any(a.GetSpell.Name.Equals));

        public static bool HaveMyBuffStack(this WoWUnit unit, string name, int stack) => GetMyBuffs(unit, name).Any(b => b.Stack == stack);

        public static bool HaveMyBuff(this WoWUnit unit, params string[] names) => GetMyBuffs(unit, names).Any();

        public static bool IsEnemy(this WoWUnit unit) => unit != null && unit.Reaction <= Reaction.Neutral;

        public static bool IsResting(this WoWUnit unit) => unit.HaveBuff("Food") || unit.HaveBuff("Drink");

        public static bool HaveImportantPoison(this WoWUnit unit) => SpecialSpells.ImportantPoison.Any(unit.HaveBuff);

        public static bool HaveImportantCurse(this WoWUnit unit) => SpecialSpells.ImportantCurse.Any(unit.HaveBuff);

        public static bool HaveImportantDisease(this WoWUnit unit) => SpecialSpells.ImportantDisease.Any(unit.HaveBuff);

        public static bool HaveImportantMagic(this WoWUnit unit) => SpecialSpells.ImportantMagic.Any(unit.HaveBuff);

        public static bool HaveImportantSlow(this WoWUnit unit) => SpecialSpells.ImportantSlow.Any(unit.HaveBuff);

        public static bool HaveImportantRoot(this WoWUnit unit) => SpecialSpells.ImportantRoot.Any(unit.HaveBuff);

    }
}