using AIO.Combat.Common;
using AIO.Framework;
using AIO.Settings;
using System.Collections.Generic;
using System.Linq;
using wManager.Wow.Enums;
using static AIO.Constants;

namespace AIO.Combat.Mage
{
    using Settings = MageLevelSettings;
    internal class SoloFire : BaseRotation
    {
        const int scorchTimeout = 1500;

        protected override List<RotationStep> Rotation => new List<RotationStep> {
            new RotationStep(new RotationSpell(SpellIds.Mage.Shoot), 0.9f, (s,t) => Settings.Current.UseWand && Me.ManaPercentage < Settings.Current.UseWandTresh && !RotationCombatUtil.IsAutoRepeating(SpellManager.GetSpellInfo(SpellIds.Mage.Shoot)), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.Mage.AutoAttack), 1f, (s,t) => !Me.IsCast && !RotationCombatUtil.IsAutoAttacking() && !RotationCombatUtil.IsAutoRepeating(SpellManager.GetSpellInfo(SpellIds.Mage.Shoot)), RotationCombatUtil.BotTarget),
            // Only cast Polymorph if Sheep is enabled in settings
            new RotationStep(new RotationSpell(SpellIds.Mage.Polymorph), 2.1f, (s,t) => Settings.Current.SoloFireSheep 
            // Only cast Polymorph if more than one enemy is targeting the Mage
            && !t.IsMyTarget && RotationFramework.Enemies.Count(o => o.IsTargetingMe) > 1 
            // Make sure no enemies in 30 yard casting range are polymorphed right now
            && RotationFramework.Enemies.Count(o => o.GetDistance <= 30 && o.HaveBuff(SpellIds.Mage.Polymorph)) < 1
            // Only polymorph a valid target
            && (t.IsCreatureType(AIO.Enums.CreatureType.Humanoid) || t.IsCreatureType(AIO.Enums.CreatureType.Beast) || t.IsCreatureType(AIO.Enums.CreatureType.Critter)),
                RotationCombatUtil.FindEnemyTargetingMe),
            new RotationStep(new RotationSpell(SpellIds.Mage.FrostNova), 2.2f, (s,t) => t.GetDistance <= 6 && t.HealthPercent > 30 && !Me.IsInGroup, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.Mage.IceBlock), 3f, (s,t) => (t.HealthPercent < 15 && !t.HaveMyBuff(SpellIds.Mage.IceBarrier)) || (Me.IsInGroup && Me.HealthPercent < 85), RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell(SpellIds.Mage.Evocation), 3.5f, (s,t) => Me.ManaPercentage < 35, RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell(SpellIds.Mage.Pyroblast), 4f, (s,t) => Me.ManaPercentage > Settings.Current.UseWandTresh && Me.HaveBuff(SpellIds.Mage.HotStreak) && t.HealthPercent > 10, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.Mage.LivingBomb), 5f, (s,t) => !t.HaveMyBuff(SpellIds.Mage.LivingBomb) && RotationFramework.Enemies.Count() >= 2, RotationCombatUtil.FindEnemyAttackingGroup),
            new RotationStep(new RotationSpell(SpellIds.Mage.Flamestrike), 6f, (s,t) => Settings.Current.SoloFireFlamestrikeWithoutFire && !t.HaveMyBuff(SpellIds.Mage.Flamestrike) && RotationFramework.Enemies.Count(o => o.Position.DistanceTo(t.Position) <=10) >= Settings.Current.SoloFireFlamestrikeWithoutCountFire && Settings.Current.SoloFireUseAOE, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.Mage.Blizzard), 7f, (s,t) => Me.IsInGroup && RotationFramework.Enemies.Count(o => o.Position.DistanceTo(t.Position) <=10) >= Settings.Current.SoloFireAOEInstance && Settings.Current.SoloFireUseAOE, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.Mage.Scorch), 9f, (s,t) =>Me.ManaPercentage > Settings.Current.UseWandTresh && TalentsManager.HaveTalent(2,11) &&  !t.HaveMyBuff(SpellIds.Mage.ImprovedScorch), RotationCombatUtil.BotTarget, forcedTimerMS: scorchTimeout),
            new RotationStep(new RotationSpell(SpellIds.Mage.Combustion), 10f, (s,t) => t.HaveMyBuff(SpellIds.Mage.Combustion), RotationCombatUtil.FindMe),
            new RotationStep(new RotationSpell(SpellIds.Mage.BlastWave), 11f, (s,t) =>Me.ManaPercentage > Settings.Current.UseWandTresh &&  t.GetDistance < 7 && RotationFramework.Enemies.Count(o => o.Position.DistanceTo(t.Position) <= 15) > 1, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.Mage.DragonsBreath), 12f, (s,t) =>Me.ManaPercentage > Settings.Current.UseWandTresh &&  t.GetDistance < 7 && RotationFramework.Enemies.Count(o => o.Position.DistanceTo(t.Position) <= 15) > 1, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.Mage.LivingBomb), 13f, (s,t) =>Me.ManaPercentage > Settings.Current.UseWandTresh &&  !t.HaveMyBuff(SpellIds.Mage.LivingBomb), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.Mage.FireBlast), 14f, (s,t) =>Me.ManaPercentage > Settings.Current.UseWandTresh &&  t.HealthPercent < 10, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.Mage.Fireball), 15f, (s,t) =>Me.ManaPercentage > Settings.Current.UseWandTresh &&  (t.HealthPercent > 55 || BossList.MyTargetIsBoss), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.Mage.Scorch), 16f, (s,t) =>Me.ManaPercentage > Settings.Current.UseWandTresh &&  (t.HealthPercent < 35 || BossList.MyTargetIsBoss) , RotationCombatUtil.BotTarget),
        };
    }
}
