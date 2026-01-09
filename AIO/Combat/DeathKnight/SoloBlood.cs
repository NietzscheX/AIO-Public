using AIO.Combat.Common;
using AIO.Framework;
using AIO.Settings;
using System.Collections.Generic;
using System.Linq;
using wManager.Wow.Helpers;
using static AIO.Constants;

namespace AIO.Combat.DeathKnight
{
    using Settings = DeathKnightLevelSettings;
    internal class SoloBlood : BaseRotation
    {
        protected override List<RotationStep> Rotation => new List<RotationStep> {
            //Taunt Offtargets
            new RotationStep(new RotationSpell(SpellIds.DeathKnight.AutoAttack), 1f, (s,t) => !Me.IsCast && !RotationCombatUtil.IsAutoAttacking(), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.DeathKnight.DarkCommand), 1.1f, (s,t) => Me.IsInGroup && Settings.Current.SoloBloodDarkCommand &&  RotationFramework.Enemies.Count(o => !o.IsTargetingMe && o.IsTargetingPartyMember) >=1, RotationCombatUtil.FindEnemyAttackingGroup),
            new RotationStep(new RotationSpell(SpellIds.DeathKnight.DeathGrip), 1.2f, (s,t) => Settings.Current.SoloBloodDeathGrip && Me.IsInGroup && RotationFramework.Enemies.Count(o => !o.IsTargetingMe && o.IsTargetingPartyMember) >=1,RotationCombatUtil.FindEnemyAttackingGroup),
           
            //Interrupt
            new RotationStep(new RotationSpell(SpellIds.DeathKnight.MindFreeze), 2f, (s,t) => t.IsCast, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.DeathKnight.Strangulate), 2.1f, (s,t) => t.IsCast && t.IsTargetingMeOrMyPetOrPartyMember && t.GetDistance < 20, RotationCombatUtil.FindEnemyCasting),
            new RotationStep(new RotationSpell(SpellIds.DeathKnight.DeathGrip), 2.2f, (s,t) => Settings.Current.SoloBloodDeathGrip && t.IsCast && t.IsTargetingMeOrMyPetOrPartyMember && t.GetDistance < 20, RotationCombatUtil.FindEnemyCasting),
            // Defensive Shell on myself
            new RotationStep(new RotationSpell(SpellIds.DeathKnight.AntiMagicShell), 3.1f, (s,t) => RotationFramework.Enemies.Count(o => o.IsCast && o.IsTargetingMe) >=1, RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell(SpellIds.DeathKnight.VampiricBlood), 3.2f, (s,t) => Me.HealthPercent <= 30, RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell(SpellIds.DeathKnight.RuneTap), 3.3f, (s,t) => Me.HealthPercent <= Settings.Current.SoloBloodRuneTap, RotationCombatUtil.FindMe),
            // other useful  Spells
            new RotationStep(new RotationSpell(SpellIds.DeathKnight.EmpowerRuneWeapon), 3.5f, (s,t) => Me.RunesReadyCount() <= 2, RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell(SpellIds.DeathKnight.ChainsOfIce), 3.7f, (s,t) => t.Fleeing, RotationCombatUtil.BotTarget),
            //Damage Part
            new RotationStep(new RotationSpell(SpellIds.DeathKnight.DeathAndDecay), 4f, (s,t) => RotationFramework.Enemies.Count(o => o.GetDistance < 15) >= Settings.Current.SoloBloodDnD, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.DeathKnight.BloodTap), 4.1f, (s,t) => Me.RuneIsReady(1) || Me.RuneIsReady(2), RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell(SpellIds.DeathKnight.IceboundFortitude), 5.1f, (s,t) => Me.HealthPercent < 80 && RotationFramework.Enemies.Count(o => o.IsTargetingMe && o.GetDistance <= 8) >=2, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.DeathKnight.MarkOfBlood), 6f, (s,t)  => BossList.MyTargetIsBoss ||(t.IsElite && Me.IsInGroup), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.DeathKnight.DancingRuneWeapon), 7f, (s,t)  => BossList.MyTargetIsBoss ||(t.IsElite && !Me.IsInGroup) || (RotationFramework.Enemies.Count(o => o.GetDistance <= 10) >=2 && !Me.IsInGroup), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.DeathKnight.DeathGrip), 8.1f, (s,t) => !Me.IsInGroup && t.IsAttackable && !t.IsTargetingMe && t.IsMyTarget && !TraceLine.TraceLineGo(Me.Position, t.Position) && t.GetDistance >= 7 && Settings.Current.SoloBloodDeathGrip, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.DeathKnight.IcyTouch), 10f, (s,t) => !t.HaveMyBuff(SpellIds.DeathKnight.FrostFever), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.DeathKnight.PlagueStrike), 11f, (s,t) => !t.HaveMyBuff(SpellIds.DeathKnight.BloodPlague), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.DeathKnight.Pestilence), 12f, (s,t) => t.HaveMyBuff(SpellIds.DeathKnight.BloodPlague, SpellIds.DeathKnight.FrostFever) && RotationFramework.Enemies.Count(o => o.GetDistance < 15 && !o.HaveMyBuff(SpellIds.DeathKnight.BloodPlague, SpellIds.DeathKnight.FrostFever)) >=2, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.DeathKnight.BloodStrike), 13f, (s,t) => RotationFramework.Enemies.Count(o => o.GetDistance <= 10) == Settings.Current.SoloBloodBloodStrike, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.DeathKnight.HeartStrike), 14f, (s,t) => RotationFramework.Enemies.Count(o => o.GetDistance <= 10) >= Settings.Current.SoloBloodHearthStrike, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.DeathKnight.BloodBoil), 15f, (s,t) => RotationFramework.Enemies.Count(o => o.GetDistance <= 10) > Settings.Current.SoloBloodBloodBoil, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.DeathKnight.DeathStrike), 16f, RotationCombatUtil.Always, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.DeathKnight.DeathCoil), 17f, (s,t) => Me.RunicPower >= 40, RotationCombatUtil.BotTarget)
        };
    }
}
