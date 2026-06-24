using System;
using Arcen.HotM.Core;
using Arcen.Universal;

namespace Arcen.HotM.OrganicIntegration
{
    public class OrganicIntegrationFeats : IActorFeatImplementation
    {
        private const int GreyGooDuration = 9999;

        public void DoWhenDealingDamage_Full( ActorFeat Feat, float FeatAmount, ISimMapActor Attacker, ISimMapActor Target,
            ref int PhysicalAttackPowerSoFar, ref int FearAttackPowerSoFar, ref int ArgumentAttackPowerSoFar, ref int SystemsDisruptionSoFar,
            ArcenCharacterBufferBase PhysicalBufferOrNull, ArcenCharacterBufferBase FearBufferOrNull, ArcenCharacterBufferBase ArgumentBufferOrNull, ArcenCharacterBufferBase SystemsDisruptionBufferOrNull,
            ArcenCharacterBufferBase SecondaryBufferOrNull, bool IsAOESecondaryTarget, bool isAnyKindOfPrediction, out AttackStop StopType, SquirrelRand Rand )
        {
            StopType = AttackStop.None;
            if ( Feat?.ID != "OI_NanobotRounds" || isAnyKindOfPrediction || Target == null || PhysicalAttackPowerSoFar <= 0 )
                return;
            if ( Target.GetIsPartOfPlayerForcesInAnyWay() || Target.GetIsAnAllyFromThePlayerPerspective() )
                return;

            ActorStatus status = ActorStatusTable.Instance.GetRowByIDOrNullIfNotFound( "OI_GreyGoo" );
            if ( status == null )
                return;

            int intensity = Math.Max( 1, (int)Math.Round( FeatAmount ) );
            Target.AddStatus( Attacker, status, intensity, GreyGooDuration, Attacker is ISimNPCUnit );
        }

        public void DoWhenDealingDamage_Precalculated( ActorFeat Feat, float FeatAmount, ISimMapActor Attacker, ISimMapActor Target,
            int PhysicalDamage, int MoraleDamage, int SystemDisruption, bool IsAOESecondaryTarget, bool isAnyKindOfPrediction )
        {
        }

        public void DoWhenAboutToFireProjectile_Precalculated( ActorFeat Feat, float FeatAmount, ISimMapActor Attacker, ISimMapActor Target,
            int PhysicalDamage, int MoraleDamage, int SystemDisruption, bool IsAOESecondaryTarget )
        {
        }

        public void DoWhenTakingDamage_Full( ActorFeat Feat, float FeatAmount, ISimMapActor Attacker, ISimMapActor Target,
            ref int PhysicalAttackPowerSoFar, ref int FearAttackPowerSoFar, ref int ArgumentAttackPowerSoFar, ref int SystemsDisruptionSoFar,
            ArcenCharacterBufferBase PhysicalBufferOrNull, ArcenCharacterBufferBase FearBufferOrNull, ArcenCharacterBufferBase ArgumentBufferOrNull, ArcenCharacterBufferBase SystemsDisruptionBufferOrNull,
            ArcenCharacterBufferBase SecondaryBufferOrNull, bool IsAOESecondaryTarget, bool isAnyKindOfPrediction, out AttackStop StopType, SquirrelRand Rand )
        {
            StopType = AttackStop.None;
        }

        public void DoWhenTakingDamage_Precalculated( ActorFeat Feat, float FeatAmount, ISimMapActor Attacker, ISimMapActor Target,
            int PhysicalDamage, int MoraleDamage, int SystemDisruption, bool IsAOESecondaryTarget, bool isAnyKindOfPrediction )
        {
        }

        public void DoWhenConsideringAttackOfOpportunity_AsAttacker( ActorFeat Feat, float FeatAmount, ISimMapActor Attacker, ISimMapActor Victim,
            ref int PhysicalAttackPowerSoFar, bool isAnyKindOfPrediction, out AttackStop StopType, SquirrelRand RandIfNotPrediction )
        {
            StopType = AttackStop.None;
        }

        public void DoWhenConsideringAttackOfOpportunity_AsVictim( ActorFeat Feat, float FeatAmount, ISimMapActor Attacker, ISimMapActor Victim,
            ref int PhysicalAttackPowerSoFar, bool isAnyKindOfPrediction, out AttackStop StopType, SquirrelRand RandIfNotPrediction )
        {
            StopType = AttackStop.None;
        }

        public void DoWhenKilling( ActorFeat Feat, float FeatAmount, ISimMapActor Attacker, ISimMapActor Target,
            int PhysicalAttackPowerUsed, int FearAttackPowerUsed, int ArgumentAttackPowerUsed, int SystemsDisruptionUsed, bool IsPhysicalDeath,
            SquirrelRand Rand )
        {
        }

        public void DoWhenDying( ActorFeat Feat, float FeatAmount, ISimMapActor Attacker, ISimMapActor Target,
            int PhysicalAttackPowerUsed, int FearAttackPowerUsed, int ArgumentAttackPowerUsed, int SystemsDisruptionUsed, bool IsPhysicalDeath,
            SquirrelRand Rand )
        {
        }

        public void DoAtTurnStart( ActorFeat Feat, float FeatAmount, ISimMapActor Actor, SquirrelRand Rand )
        {
        }

        public void DoWhenIncapacitatingUnit( ActorFeat Feat, float FeatAmount, ISimMapActor Incapacitator, ISimMapActor Victim,
            SquirrelRand Rand )
        {
        }

        public void DoWhenNearbyUnitIncapacitated( ActorFeat Feat, float FeatAmount, ISimMapActor FeatHolder, ISimMapActor Victim,
            ISimMapActor IncapacitatorOrNull, SquirrelRand Rand )
        {
        }

        public float GetFeatRangeForRing( ActorFeat Feat, float FeatAmount, ISimMapActor FeatHolder )
        {
            return FeatAmount;
        }
    }
}