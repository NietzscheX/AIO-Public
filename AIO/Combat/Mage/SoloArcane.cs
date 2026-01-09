using AIO.Combat.Common;
using AIO.Framework;
using AIO.Settings;
using System.Collections.Generic;
using static AIO.Constants;
using System.Linq;
using wManager.Wow.Enums;
using wManager.Wow.Helpers;



namespace AIO.Combat.Mage
{
    using Settings = MageLevelSettings;
    internal class SoloArcane : BaseRotation
    {
        protected override List<RotationStep> Rotation => new List<RotationStep> {
            new RotationStep(new RotationSpell(SpellIds.Mage.Shoot), 0.9f, (s,t) => Settings.Current.UseWand && Me.ManaPercentage < Settings.Current.UseWandTresh && !RotationCombatUtil.IsAutoRepeating(SpellManager.GetSpellInfo(SpellIds.Mage.Shoot)), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.Mage.AutoAttack), 1f, (s,t) => !Me.IsCast && !RotationCombatUtil.IsAutoAttacking() && !RotationCombatUtil.IsAutoRepeating(SpellManager.GetSpellInfo(SpellIds.Mage.Shoot)), RotationCombatUtil.BotTarget),
            // Only cast Polymorph if Sheep is enabled in settings
            new RotationStep(new RotationSpell(SpellIds.Mage.Polymorph), 2.1f, (s,t) => Settings.Current.SoloArcaneSheep 
            // Only cast Polymorph if more than one enemy is targeting the Mage
            && !t.IsMyTarget && RotationFramework.Enemies.Count(o => o.IsTargetingMe) > 1 
            // Make sure no enemies in 30 yard casting range are polymorphed right now
            && RotationFramework.Enemies.Count(o => o.GetDistance <= 30 && o.HaveBuff(SpellIds.Mage.Polymorph)) < 1
            // Only polymorph a valid target
            && (t.IsCreatureType(AIO.Enums.CreatureType.Humanoid) || t.IsCreatureType(AIO.Enums.CreatureType.Beast) || t.IsCreatureType(AIO.Enums.CreatureType.Critter)),
                RotationCombatUtil.FindEnemyTargetingMe),
            new RotationStep(new RotationSpell(SpellIds.Mage.IcyVeins), 3f, (s,t) => Me.BuffStack(SpellIds.Mage.ArcaneBlastBuff) >= 1 && BossList.MyTargetIsBoss, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.Mage.ArcanePower), 4f, (s,t) => Me.BuffStack(SpellIds.Mage.ArcaneBlastBuff) >= 1 && BossList.MyTargetIsBoss, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.Mage.MirrorImage), 5f, (s,t) => Me.BuffStack(SpellIds.Mage.ArcaneBlastBuff) >= 1 && BossList.MyTargetIsBoss, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.Mage.PresenceOfMind), 6f, (s,t) => Me.BuffStack(SpellIds.Mage.ArcaneBlastBuff) >=2 && BossList.MyTargetIsBoss, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.Mage.ArcaneMissiles), 7f, (s,t) =>Me.ManaPercentage > Settings.Current.UseWandTresh && Me.BuffStack(SpellIds.Mage.ArcaneBlastBuff) >=3 && Me.HaveBuff(SpellIds.Mage.MissileBarrage), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.Mage.ArcaneBlast), 8f, (s,t) => Me.ManaPercentage > Settings.Current.UseWandTresh, RotationCombatUtil.BotTarget)
        };
    }
}
