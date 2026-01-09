using AIO.Combat.Common;
using AIO.Framework;
using AIO.Settings;
using System.Collections.Generic;
using System.Linq;
using static AIO.Constants;

namespace AIO.Combat.Hunter
{
    using Settings = HunterLevelSettings;
    internal class SoloSurvival : BaseRotation
    {
        protected override List<RotationStep> Rotation => new List<RotationStep> {
            new RotationStep(new RotationSpell(SpellIds.Hunter.AutoAttack), 1f, (s,t) => !Me.IsCast && !RotationCombatUtil.IsAutoAttacking(), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.Hunter.HuntersMark), 6f, (s,t) => t.GetDistance >= 5 && !t.HaveMyBuff(SpellIds.Hunter.HuntersMark) && t.IsAlive && t.GetDistance >= 5 && t.HealthPercent > 50, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.Hunter.KillCommand), 6.1f, (s,t) => !Me.HaveBuff(SpellIds.Hunter.KillCommand), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.Hunter.SerpentSting), 7f, (s,t) => t.GetDistance >= 5 && !t.HaveMyBuff(SpellIds.Hunter.SerpentSting) , RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.Hunter.ArcaneShot), 7.1f, (s,t) => t.GetDistance >= 5, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.Hunter.MultiShot), 7.2f, (s,t) => t.GetDistance >= 5 && Settings.Current.SoloSurvivalUseMultiShot && RotationFramework.Enemies.Count(o => o.IsTargetingMeOrMyPetOrPartyMember) >= Settings.Current.SoloSurvivalMultiShotCount, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.Hunter.BlackArrow), 8f, (s,t) => t.GetDistance >= 5, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.Hunter.ExplosiveShot), 9f, (s,t) => t.GetDistance >= 5, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.Hunter.AimedShot), 10f, (s,t) => t.GetDistance >= 5, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.Hunter.SteadyShot), 11f, (s,t) => t.GetDistance >= 5 , RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.Hunter.RaptorStrike), 12f, (s,t) => t.GetDistance < 5, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.Hunter.Disengage), 13f, (s,t) => t.GetDistance < 5 && t.IsTargetingMe && Pet.IsAlive && Settings.Current.SoloMarksmanshipDisengage, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.Hunter.FeignDeath), 14f, (s,t) => t.GetDistance < 5 && Me.HealthPercent < 50 && t.IsTargetingMe && Pet.IsAlive && Settings.Current.SoloMarksmanshipFD, RotationCombatUtil.BotTarget),
        };
    }
}
