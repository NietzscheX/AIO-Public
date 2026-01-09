using AIO.Combat.Common;
using AIO.Framework;
using AIO.Settings;
using System.Collections.Generic;
using System.Linq;
using static AIO.Constants;

namespace AIO.Combat.DeathKnight
{
    using Settings = DeathKnightLevelSettings;
    internal class SoloFrost : BaseRotation
    {
        protected override List<RotationStep> Rotation => new List<RotationStep> {
            new RotationStep(new RotationSpell(SpellIds.DeathKnight.AutoAttack), 1f, (s,t) => !Me.IsCast && !RotationCombatUtil.IsAutoAttacking(), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.DeathKnight.MindFreeze), 2f, (s,t) => t.IsCast, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.DeathKnight.Strangulate), 3f, (s,t) => t.IsCast && t.IsTargetingMeOrMyPetOrPartyMember && t.GetDistance < 20, RotationCombatUtil.FindEnemyCasting),
            new RotationStep(new RotationSpell(SpellIds.DeathKnight.HowlingBlast), 4f, (s,t) => Me.HaveBuff(SpellIds.DeathKnight.KillingMachine) || Me.HaveBuff(SpellIds.DeathKnight.FreezingFog), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.DeathKnight.DeathAndDecay), 5f, (s,t) => RotationFramework.Enemies.Count(o => o.GetDistance < 15) >= Settings.Current.SoloFrostDnD, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.DeathKnight.IceboundFortitude), 6f, (s,t) => Me.HealthPercent < 80 && RotationFramework.Enemies.Count(o => o.IsTargetingMe && o.GetDistance <= 8) >=2, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.DeathKnight.FrostStrike), 7f, (s,t) => Me.RunicPower > 40, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.DeathKnight.IcyTouch), 8f, (s,t) => !t.HaveMyBuff(SpellIds.DeathKnight.FrostFever), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.DeathKnight.PlagueStrike), 9f, (s,t) => !t.HaveMyBuff(SpellIds.DeathKnight.BloodPlague), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.DeathKnight.Pestilence), 10f, (s,t) => t.HaveMyBuff(SpellIds.DeathKnight.BloodPlague, SpellIds.DeathKnight.FrostFever) && RotationFramework.Enemies.Count(o => o.GetDistance < 15 && !o.HaveMyBuff(SpellIds.DeathKnight.BloodPlague, SpellIds.DeathKnight.FrostFever)) >=2, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.DeathKnight.Obliterate), 11f, (s,t) => t.HaveMyBuff(SpellIds.DeathKnight.BloodPlague, SpellIds.DeathKnight.FrostFever), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.DeathKnight.BloodStrike), 12f, (s,t) => RotationFramework.Enemies.Count(o => o.GetDistance <= 10) == Settings.Current.SoloFrostBloodStrike, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.DeathKnight.HeartStrike), 13f, (s,t) => RotationFramework.Enemies.Count(o => o.GetDistance <= 10) >= Settings.Current.SoloFrostHearthStrike, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.DeathKnight.BloodBoil), 14f, (s,t) => RotationFramework.Enemies.Count(o => o.GetDistance <= 10) > Settings.Current.SoloFrostBloodBoil, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.DeathKnight.DeathStrike), 15f, RotationCombatUtil.Always, RotationCombatUtil.BotTarget)
        };
    }
}