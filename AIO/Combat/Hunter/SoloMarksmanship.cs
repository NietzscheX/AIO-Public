using AIO.Combat.Common;
using AIO.Framework;
using AIO.Settings;
using System.Collections.Generic;
using System.Linq;
using static AIO.Constants;

namespace AIO.Combat.Hunter
{
    using Settings = HunterLevelSettings;
    internal class SoloMarksmanship : BaseRotation
    {
        protected override List<RotationStep> Rotation => new List<RotationStep> {
            new RotationStep(new RotationSpell(SpellIds.Hunter.AutoAttack), 1f, (s,t) => !Me.IsCast && !RotationCombatUtil.IsAutoAttacking(), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.Hunter.FeignDeath), 2f, (s,t) => t.GetDistance < 5 && Me.HealthPercent < 50 && t.IsTargetingMe && Pet.IsAlive && Settings.Current.SoloMarksmanshipFD, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.Hunter.SilencingShot), 6f, (s,t) => t.GetDistance >= 5 && t.IsCast ,RotationCombatUtil.BotTarget),
            new RotationStep(new RotationBuff(SpellIds.Hunter.RapidFire), 7f, (s,t) =>(RotationFramework.Enemies.Count(o => o.IsTargetingMeOrMyPet) >=3 && !Me.IsInGroup) 
                    || (RotationFramework.Enemies.Count(o => o.IsTargetingMeOrMyPetOrPartyMember) >= Settings.Current.SoloMarksmanshipAOECount && Me.IsInGroup) 
                    || (t.IsElite && !Me.IsInGroup) 
                    || (Me.IsInGroup && BossList.MyTargetIsBoss), RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell(SpellIds.Hunter.Readiness), 8f, (s,t) => !Me.HaveBuff(SpellIds.Hunter.RapidFire) && (RotationFramework.Enemies.Count(o => o.IsTargetingMeOrMyPet) >=3 && !Me.IsInGroup)
                    || (RotationFramework.Enemies.Count(o => o.IsTargetingMeOrMyPetOrPartyMember) >= Settings.Current.SoloMarksmanshipAOECount && Me.IsInGroup)
                    || (t.IsElite && !Me.IsInGroup)
                    || (Me.IsInGroup && BossList.MyTargetIsBoss), RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell(SpellIds.Hunter.KillShot), 9f, (s,t) => t.GetDistance >= 5 && t.HealthPercent< 20, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.Hunter.Volley), 9.1f, (s,t) => RotationFramework.Enemies.Count(o => o.Position.DistanceTo(t.Position) <=10) >= Settings.Current.SoloMarksmanshipAOECount && Settings.Current.SoloMarksmanshipUseAOE, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.Hunter.HuntersMark), 10f, (s,t) => t.GetDistance >= 5 && !t.HaveMyBuff(SpellIds.Hunter.HuntersMark) && t.IsAlive && t.GetDistance >= 5 && t.HealthPercent > 50, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.Hunter.ViperSting), 11f, (s,t) => t.GetDistance >= 5 && t.HasMana() && Me.ManaPercentage <= 45, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.Hunter.SerpentSting), 12f, (s,t) => t.GetDistance >= 5 && !t.HaveMyBuff(SpellIds.Hunter.SerpentSting), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.Hunter.ChimeraShot), 13f, (s,t) => t.GetDistance >= 5 && t.HaveMyBuff(SpellIds.Hunter.SerpentSting, SpellIds.Hunter.ViperSting), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.Hunter.ArcaneShot), 14f, (s,t) => t.GetDistance >= 5 && Settings.Current.SoloMarksmanshipArcaneShot, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.Hunter.MultiShot), 15f, (s,t) => t.GetDistance >= 5 && Settings.Current.SoloMarksmanshipMultiShot && RotationFramework.Enemies.Count(o => o.IsTargetingMeOrMyPetOrPartyMember) >= Settings.Current.SoloMarksmanshipMultiShotCount, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.Hunter.AimedShot), 15.1f, (s,t) => t.GetDistance >= 5 && Settings.Current.SoloMarksmanshipAimedShot, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.Hunter.SteadyShot), 16f, (s,t) => t.GetDistance >= 5 && t.HaveMyBuff(SpellIds.Hunter.SerpentSting, SpellIds.Hunter.ViperSting), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.Hunter.RaptorStrike), 17f, (s,t) => t.GetDistance < 5, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.Hunter.Disengage), 18f, (s,t) => t.GetDistance < 5 && t.IsTargetingMe && Pet.IsAlive && Settings.Current.SoloMarksmanshipDisengage, RotationCombatUtil.BotTarget),
        };
    }
}
