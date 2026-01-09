using AIO.Combat.Common;
using AIO.Framework;
using AIO.Settings;
using System.Collections.Generic;
using System.Linq;
using static AIO.Constants;

namespace AIO.Combat.DeathKnight
{
    using Settings = DeathKnightLevelSettings;
    internal class SoloUnholy : BaseRotation
    {
        protected override List<RotationStep> Rotation => new List<RotationStep> {
            new RotationStep(new RotationSpell(SpellIds.DeathKnight.AutoAttack), 1f, (s,t) => !Me.IsCast && !RotationCombatUtil.IsAutoAttacking(), RotationCombatUtil.BotTarget),
            //new RotationStep(new RotationSpell("Raise Dead"), 2f, (s,t) => !Pet.IsAlive && Me.RunicPower > 80 , RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.DeathKnight.MindFreeze), 3.1f, (s,t) => t.IsCasting(), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.DeathKnight.Strangulate), 4f, (s,t) => t.IsCasting() && t.IsTargetingMeOrMyPetOrPartyMember && t.GetDistance < 20, RotationCombatUtil.FindEnemyCasting),
            new RotationStep(new RotationSpell(SpellIds.DeathKnight.DeathAndDecay), 5f, (s,t) => RotationFramework.Enemies.Count(o => o.GetDistance < 15) >= Settings.Current.SoloUnholyDnD, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.DeathKnight.IcyTouch), 6f, (s,t) => !t.HaveMyBuff(SpellIds.DeathKnight.FrostFever), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.DeathKnight.PlagueStrike), 7f, (s,t) => !t.HaveMyBuff(SpellIds.DeathKnight.BloodPlague), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.DeathKnight.SummonGargoyle), 4.0f, (s,t) => BossList.MyTargetIsBoss, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.DeathKnight.Pestilence), 8f, (s,t) => t.HaveMyBuff(SpellIds.DeathKnight.BloodPlague, SpellIds.DeathKnight.FrostFever) && RotationFramework.Enemies.Count(o => o.GetDistance < 15 && !o.HaveMyBuff(SpellIds.DeathKnight.BloodPlague, SpellIds.DeathKnight.FrostFever)) >=2, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.DeathKnight.BloodStrike), 9f, (s,t) => RotationFramework.Enemies.Count(o => o.GetDistance <= 10) == Settings.Current.SoloUnholyBloodStrike, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.DeathKnight.HeartStrike), 10f, (s,t) => RotationFramework.Enemies.Count(o => o.GetDistance <= 10) >= Settings.Current.SoloUnholyHearthStrike, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.DeathKnight.BloodBoil), 11f, (s,t) => RotationFramework.Enemies.Count(o => o.GetDistance <= 10) > Settings.Current.SoloUnholyBloodBoil, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.DeathKnight.DeathStrike), 12f, (s,t) => Me.HealthPercent < 50, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.DeathKnight.ScourgeStrike), 13f, (s,t) => t.HaveMyBuff(SpellIds.DeathKnight.BloodPlague, SpellIds.DeathKnight.FrostFever), RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.DeathKnight.DeathCoil), 14f, (s,t) => Me.RunicPower > 80, RotationCombatUtil.BotTarget),
            new RotationStep(new RotationSpell(SpellIds.DeathKnight.BloodStrike), 15f, RotationCombatUtil.Always, RotationCombatUtil.BotTarget)
        };
    }
}
