using AIO.Combat.Common;
using AIO.Framework;
using AIO.Settings;
using System.Collections.Generic;
using System.Linq;
using wManager.Wow.Enums;
using static AIO.Constants;

namespace AIO.Combat.Rogue
{
    using Settings = RogueLevelSettings;
    internal class SoloCombat : BaseRotation
    {
        protected override List<RotationStep> Rotation => new List<RotationStep> {
            new RotationStep(new RotationSpell(SpellIds.Rogue.AutoAttack), 1f, (s,t) => !Me.IsCast && !RotationCombatUtil.IsAutoAttacking(), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.Rogue.Sprint), 2f, (s,t) => t.GetDistance >= 15 && !Settings.Current.PullRanged, RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell(SpellIds.Rogue.Kick), 3f, (s,t) => t.IsCasting() && t.GetDistance < 7, RotationCombatUtil.FindEnemyCasting),
            new RotationStep(new RotationSpell(SpellIds.Rogue.Evasion), 3.1f, (s, t) => RotationFramework.Enemies.Count(o => o.GetDistance <=10 && o.IsTargetingMe) >=Settings.Current.SoloCombatEvasion || (Me.HealthPercent <= 30 && t.HealthPercent >70), RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell(SpellIds.Rogue.Evasion), 3.2f, (s, t) => !Me.IsInGroup && Target.IsElite, RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell(SpellIds.Rogue.Riposte), 4f, (s, t) => !Me.HaveBuff(SpellIds.Rogue.Stealth), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.Rogue.BladeFlurry), 2f, (s,t) =>t.HealthPercent> 70 && !Me.HaveBuff(SpellIds.Rogue.Stealth) && (RotationFramework.Enemies.Count(o => o.GetDistance <=10) >=Settings.Current.SoloCombatBladeFLurry), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.Rogue.AdrenalineRush), 6f, (s,t) =>!Me.HaveBuff(SpellIds.Rogue.Stealth) && RotationFramework.Enemies.Count(o => o.GetDistance <=10) >=Settings.Current.SoloCombatAdrenalineRush, RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell(SpellIds.Rogue.AdrenalineRush), 6.1f, (s,t) =>!Me.HaveBuff(SpellIds.Rogue.Stealth) && Target.IsElite && !Me.IsInGroup, RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell(SpellIds.Rogue.SliceAndDice), 7f, (s, t) => !Me.HaveBuff(SpellIds.Rogue.SliceAndDice) && Me.ComboPoint >= 1 && t.HealthPercent > 50, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.Rogue.Eviscerate), 8f, (s, t) =>!Me.HaveBuff(SpellIds.Rogue.Stealth) && Me.ComboPoint >= Settings.Current.SoloCombatEviscarate, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.Rogue.KillingSpree), 9f, (s, t) =>!Me.HaveBuff(SpellIds.Rogue.AdrenalineRush) && !Me.HaveBuff(SpellIds.Rogue.BladeFlurry) && !Me.HaveBuff(SpellIds.Rogue.Stealth) && RotationFramework.Enemies.Count(o => o.GetDistance <=10) >=Settings.Current.SoloCombatKillingSpree, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.Rogue.SinisterStrike), 10f, (s, t) =>!Me.HaveBuff(SpellIds.Rogue.Stealth), RotationCombatUtil.BotTarget),
        };
    }
}
