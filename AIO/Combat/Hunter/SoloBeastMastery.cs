using AIO.Combat.Common;
using AIO.Framework;
using AIO.Settings;
using System.Collections.Generic;
using System.Linq;
using static AIO.Constants;

namespace AIO.Combat.Hunter
{
    using Settings = HunterLevelSettings;
    internal class SoloBeastMastery : BaseRotation
    {
        protected override List<RotationStep> Rotation => new List<RotationStep> {
            new RotationStep(new RotationSpell(SpellIds.Hunter.AutoAttack), 1f, (s,t) => !Me.IsCast && !RotationCombatUtil.IsAutoAttacking(), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.Hunter.FeignDeath), 2f, (s,t) => t.GetDistance < 5 && Me.HealthPercent < 50 && t.IsTargetingMe && Pet.IsAlive && Settings.Current.SoloBeastMasteryFD, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.Hunter.Deterrence), 2.1f, (s,t) => t.IsTargetingMe && Me.HealthPercent < 50, RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell(SpellIds.Hunter.Intimidation), 3f, (s,t) => Pet.Target == Me.Target && Pet.Position.DistanceTo(t.Position) <= 6 && t.IsCasting(), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.Hunter.ConcussiveShot), 3.1f, (s,t) => t.Fleeing && !t.HaveBuff(SpellIds.Hunter.ConcussiveShot), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.Hunter.Misdirection), 3.2f, (s,t) => Settings.Current.SoloBeastMasteryMisdirection && !Me.IsInGroup && !Me.HaveBuff(SpellIds.Hunter.Misdirection) && Pet.IsAlive && t.IsMyPet && RotationFramework.Enemies.Count(u => u.IsTargetingMe) >=1 , RotationCombatUtil.FindPet),
            new RotationStep(new RotationSpell(SpellIds.Hunter.Misdirection), 3.3f, (s,t) => Settings.Current.SoloBeastMasteryMisdirection && Me.IsInGroup && !Me.HaveBuff(SpellIds.Hunter.Misdirection) && t.IsAlive , RotationCombatUtil.FindTank),
            new RotationStep(new RotationSpell(SpellIds.Hunter.Volley), 4f, (s,t) => Settings.Current.SoloBeastMasteryUseAOE && RotationFramework.Enemies.Count(o => o.Position.DistanceTo(t.Position) <=10) >= Settings.Current.SoloBeastMasteryAOECount, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.Hunter.KillShot), 5f, (s,t) => t.GetDistance >= 5 && t.HealthPercent< 20, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.Hunter.HuntersMark), 9f, (s,t) => !t.HaveMyBuff(SpellIds.Hunter.HuntersMark) && t.IsAlive && t.GetDistance >= 5 && t.HealthPercent > 50, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.Hunter.BestialWrath), 10f, (s,t) => (RotationFramework.Enemies.Count(o => o.IsTargetingMeOrMyPet) >=2 && !Me.IsInGroup) || (t.IsElite && !Me.IsInGroup) || (Me.IsInGroup && t.IsBoss), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.Hunter.RapidFire), 11f, (s,t) => (RotationFramework.Enemies.Count(o => o.IsTargetingMeOrMyPet) >= Settings.Current.SoloBeastMasteryAOECount && !Me.IsInGroup)||  (RotationFramework.Enemies.Count(o => o.IsTargetingMeOrMyPetOrPartyMember) >= Settings.Current.SoloBeastMasteryAOECount && Me.IsInGroup)  || (t.IsElite && !Me.IsInGroup) || (Me.IsInGroup && BossList.MyTargetIsBoss), RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell(SpellIds.Hunter.KillCommand), 12f, (s,t) => !Me.HaveBuff(SpellIds.Hunter.KillCommand), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.Hunter.SerpentSting), 13f, (s,t) => t.GetDistance >= 5 && !t.HaveMyBuff(SpellIds.Hunter.SerpentSting) && (t.HealthPercent >= 70 || (BossList.MyTargetIsBoss && t.HealthPercent >= 20)), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.Hunter.ArcaneShot), 14f, (s,t) => t.GetDistance >= 5, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.Hunter.MultiShot), 15f, (s,t) => t.GetDistance >= 5 && Settings.Current.SoloBeastMasteryMultiShot && RotationFramework.Enemies.Count(o => o.IsAttackable && o.IsTargetingMeOrMyPetOrPartyMember) >= Settings.Current.SoloBeastMasteryMultiSCount, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.Hunter.SteadyShot), 15.1f, (s,t) => !Me.GetMove && t.GetDistance >= 5, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.Hunter.RaptorStrike), 16f, (s,t) => t.GetDistance < 5, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.Hunter.Disengage), 17f, (s,t) => t.GetDistance < 5 && t.IsTargetingMe && Pet.IsAlive && Settings.Current.SoloBeastMasteryDisengage, RotationCombatUtil.BotTarget),
        };
    }
}
