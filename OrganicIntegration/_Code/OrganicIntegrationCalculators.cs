using System;
using System.Collections.Generic;
using Arcen.HotM.Core;
using Arcen.Universal;

namespace Arcen.HotM.OrganicIntegration
{
    public class OrganicIntegrationCalculators : IDataCalculator_DoPerTurn_Late, IDataCalculator_DoPerQuarterSecond, IDataCalculator_DoAfterAnyUnitDeath
    {
        internal const string VoluntaryJob = "OI_NanobotUpgradeHub";
        internal const string CoerciveJob = "OI_NaniteWindGenerator";
        internal const string StealthCoerciveJob = "OI_StealthNaniteWindGenerator";
        internal const string UpgradedResource = "OI_UpgradedHumans";
        internal const string UpgradedSwarm = "OI_UpgradedHumans";
        private const string InsightResource = "OI_Insight";
        private const string GreyGooStatus = "OI_GreyGoo";
        private const int VoluntaryPopulationCapPercent = 45;
        private const int CoercivePopulationCapPercent = 85;
        private const int GreyGooDuration = 9999;

        public void DoPerTurn_Late( DataCalculator Calculator, SquirrelRand RandForThisTurn )
        {
            RecalculateAGIBridgeBlock();

            int voluntaryStructures = CountFunctionalStructures( VoluntaryJob );
            int coerciveStructures = CountFunctionalStructures( CoerciveJob, StealthCoerciveJob );

            bool voluntaryLocked = IsFlagTripped( "OI_IntegrationVoluntaryLocked" );
            bool coerciveLocked = IsFlagTripped( "OI_IntegrationCoerciveLocked" );

            if ( !voluntaryLocked && !coerciveLocked )
            {
                if ( voluntaryStructures > 0 )
                {
                    TripFlag( "OI_IntegrationVoluntaryLocked" );
                    voluntaryLocked = true;
                }
                else if ( coerciveStructures > 0 )
                {
                    TripFlag( "OI_IntegrationCoerciveLocked" );
                    coerciveLocked = true;
                }
            }

            if ( coerciveLocked && coerciveStructures > 0 )
                ApplyCoerciveWindSpread( RandForThisTurn );

            ApplyInsightIncome( voluntaryLocked, coerciveLocked );
            ApplyActiveInsightVRActions();
            ApplyIntegrationMigrationPressure();
        }

        public void DoPerQuarterSecond( DataCalculator Calculator, SquirrelRand RandForBackgroundThread )
        {
            RecalculateAGIBridgeBlock();
            ApplyIntegrationNeuralExpansion();
        }

        public void DoAfterAnyUnitDeath( DataCalculator Calculator, ISimMapMobileActor Actor, NPCDisbandReason AnyUnitReason, SquirrelRand Rand )
        {
            ActorStatus status = ActorStatusTable.Instance.GetRowByIDOrNullIfNotFound( GreyGooStatus );
            if ( status == null || Actor == null )
                return;

            int intensity = Actor.GetStatusIntensity( status );
            if ( intensity <= 0 )
                return;

            Vector3A fromSpot = Actor.GetDrawLocation();
            float radiusSquared = 36f;
            int spreadIntensity = Math.Max( 1, intensity / 2 );

            foreach ( ISimNPCUnit other in SimCommon.AllNPCs.GetDisplayList() )
            {
                if ( other == null || other == Actor || other.IsFullDead )
                    continue;
                if ( other.GetIsPartOfPlayerForcesInAnyWay() || other.GetIsAnAllyFromThePlayerPerspective() )
                    continue;

                float dist = (other.GetDrawLocation() - fromSpot).GetSquareGroundMagnitude();
                if ( dist > radiusSquared )
                    continue;

                other.AddStatus( null, status, spreadIntensity, GreyGooDuration, false );
            }
        }

        private static void ApplyCoerciveWindSpread( SquirrelRand Rand )
        {
            ResourceType upgraded = GetResource( UpgradedResource );
            Swarm swarm = SwarmTable.Instance.GetRowByIDOrNullIfNotFound( UpgradedSwarm );
            if ( upgraded == null || swarm == null )
                return;

            System.Collections.Generic.List<MachineStructure> dispersers = GetFunctionalStructures( CoerciveJob, StealthCoerciveJob );
            if ( dispersers.Count == 0 )
                return;

            long normalPopulation = GetCityStatisticScore( "CityHumanCitizenPopulation" );
            long remainingUnderCap = CalculatePopulationCap( normalPopulation, upgraded.Current, CoercivePopulationCapPercent ) - upgraded.Current;
            if ( remainingUnderCap <= 0 )
                return;

            foreach ( MachineStructure structure in dispersers )
            {
                if ( remainingUnderCap <= 0 )
                    break;

                Vector3A center = structure.GetDrawLocation();
                float radiusSquared = 125f * 125f;
                int perStructureBudget = (int)Math.Min( 35000L, remainingUnderCap );

                foreach ( Arcen.Universal.KeyValuePair<int, BaseBuilding> kv in World.Buildings.GetAllBuildings() )
                {
                    if ( perStructureBudget <= 0 || remainingUnderCap <= 0 )
                        break;

                    BaseBuilding building = kv.Value;
                    if ( building == null || building.GetIsDestroyed() )
                        continue;
                    if ( building.SwarmSpread != null && building.SwarmSpread != swarm )
                        continue;

                    int peopleHere = building.GetTotalResidentCount() + building.GetTotalWorkerCount();
                    if ( peopleHere <= 0 )
                        continue;

                    float dist = (building.GetEffectiveWorldLocationForContainedUnit() - center).GetSquareGroundMagnitude();
                    if ( dist > radiusSquared )
                        continue;

                    int desired = Math.Max( 1, (peopleHere * 60 + 99) / 100 );
                    int toConvert = (int)Math.Min( Math.Min( desired, perStructureBudget ), remainingUnderCap );
                    int converted = building.KillRandomHere( toConvert, Rand, false, string.Empty );
                    if ( converted <= 0 )
                        continue;

                    AddUpgradedHumans( building, swarm, upgraded, converted, "Income_OI_UpgradedHumans_Coercive" );
                    perStructureBudget -= converted;
                    remainingUnderCap -= converted;
                }
            }
        }

        internal static int TryVoluntaryBuildingIntegration( BaseBuilding building, SquirrelRand Rand )
        {
            ResourceType upgraded = GetResource( UpgradedResource );
            Swarm swarm = SwarmTable.Instance.GetRowByIDOrNullIfNotFound( UpgradedSwarm );
            if ( building == null || building.GetIsDestroyed() || upgraded == null || swarm == null )
                return 0;

            int peopleHere = building.GetTotalResidentCount() + building.GetTotalWorkerCount();
            if ( peopleHere <= 0 )
                return 0;

            long normalPopulation = GetCityStatisticScore( "CityHumanCitizenPopulation" );
            long remainingUnderCap = CalculatePopulationCap( normalPopulation, upgraded.Current, VoluntaryPopulationCapPercent ) - upgraded.Current;
            if ( remainingUnderCap <= 0 )
                return 0;

            int desired = Math.Max( 1, (peopleHere * 45 + 99) / 100 );
            int toConvert = (int)Math.Min( desired, remainingUnderCap );
            int converted = building.KillRandomHere( toConvert, Rand, false, string.Empty );
            if ( converted <= 0 )
                return 0;

            AddUpgradedHumans( building, swarm, upgraded, converted, "Income_OI_UpgradedHumans_Voluntary" );
            return converted;
        }

        private static void AddUpgradedHumans( BaseBuilding building, Swarm swarm, ResourceType upgraded, int converted, string reason )
        {
            if ( converted <= 0 )
                return;

            if ( building != null && swarm != null )
            {
                if ( building.SwarmSpread == null )
                    building.SwarmSpread = swarm;
                if ( building.SwarmSpread == swarm )
                    building.AlterSwarmSpreadCount( converted );
            }

            upgraded.AlterCurrent_Named( converted, reason, ResourceAddRule.IgnoreUntilTurnChange );
            GStatisticTable.AlterScore( "OI_UpgradedPopulation", converted );
            SimCommon.NeedsBuildingListRecalculation = true;
        }

        private static void ApplyInsightIncome( bool voluntaryLocked, bool coerciveLocked )
        {
            ResourceType upgraded = GetResource( UpgradedResource );
            ResourceType insight = GetResource( InsightResource );
            if ( upgraded == null || insight == null || upgraded.Current <= 0 )
                return;

            long divisor = coerciveLocked && !voluntaryLocked ? 12000L : 3500L;
            long income = upgraded.Current / divisor;
            if ( income <= 0 && upgraded.Current >= 1000 )
                income = 1;

            if ( income > 0 )
            {
                insight.AlterCurrent_Named( income, "Income_OI_Insight", ResourceAddRule.IgnoreUntilTurnChange );
                GStatisticTable.AlterScore( "OI_TotalInsightGenerated", income );
            }
        }

        private static void ApplyActiveInsightVRActions()
        {
            MachineVRModeAction sharedInquiry = MachineVRModeActionTable.Instance.GetRowByIDOrNullIfNotFound( "OI_SharedInquiry" );
            if ( sharedInquiry == null || !sharedInquiry.DGD.IsActiveNow )
                return;

            ResourceType insight = GetResource( InsightResource );
            ResourceType research = GetResource( "ScientificResearch" );
            if ( insight == null || research == null || insight.Current <= 0 )
            {
                sharedInquiry.DGD.IsActiveNow = false;
                return;
            }

            long insightToSpend = Math.Min( 100L, insight.Current );
            if ( insightToSpend <= 0 )
            {
                sharedInquiry.DGD.IsActiveNow = false;
                return;
            }

            long researchToGain = insightToSpend * 100L;
            insight.AlterCurrent_Named( -insightToSpend, "Expense_OI_InsightVR", ResourceAddRule.IgnoreUntilTurnChange );
            research.AlterCurrent_Named( researchToGain, "Income_OI_InsightVRResearch", ResourceAddRule.IgnoreUntilTurnChange );

            if ( insight.Current <= 0 )
                sharedInquiry.DGD.IsActiveNow = false;
        }

        private static void ApplyIntegrationMigrationPressure()
        {
            ResourceType upgraded = GetResource( UpgradedResource );
            if ( upgraded == null || upgraded.Current < 10000 )
                return;

            long waitlistGain = Math.Min( 250000L, Math.Max( 0L, upgraded.Current / 200L ) );
            if ( waitlistGain > 0 )
                GStatisticTable.AlterScore( "CityHumanCitizenWaitlist", waitlistGain );

            ResourceType abandoned = GetResource( "AbandonedHumans" );
            if ( abandoned != null )
            {
                long abandonedGain = Math.Min( 2500L, Math.Max( 0L, upgraded.Current / 100000L ) );
                if ( abandonedGain > 0 )
                    abandoned.AlterCurrent_Named( abandonedGain, "Income_OI_IntegrationImmigrationPressure", ResourceAddRule.IgnoreUntilTurnChange );
            }
        }

        private static void ApplyIntegrationNeuralExpansion()
        {
            ResourceType upgraded = GetResource( UpgradedResource );
            ActorDataType neuralExpansion = ActorRefs.NeuralExpansion;
            if ( upgraded == null || neuralExpansion == null )
                return;

            System.Collections.Generic.List<MachineStructure> providers = GetFunctionalStructures( VoluntaryJob, CoerciveJob, StealthCoerciveJob );
            bool coerciveLocked = IsFlagTripped( "OI_IntegrationCoerciveLocked" );
            long divisorForNE = coerciveLocked ? 16L : 24L;
            int totalNeuralExpansion = ClampToInt( upgraded.Current / divisorForNE );
            int divisor = Math.Max( 1, providers.Count );
            int index = 0;

            foreach ( MachineStructure structure in providers )
            {
                int amount = totalNeuralExpansion / divisor;
                if ( index < totalNeuralExpansion % divisor )
                    amount++;
                index++;
                SetStructureNeuralExpansion( structure, neuralExpansion, amount );
            }

            foreach ( Arcen.Universal.KeyValuePair<int, MachineStructure> kv in SimCommon.MachineStructuresByID )
            {
                MachineStructure structure = kv.Value;
                if ( structure == null || structure.CurrentJob == null )
                    continue;
                string jobID = structure.CurrentJob.ID;
                if ( jobID != VoluntaryJob && jobID != CoerciveJob && jobID != StealthCoerciveJob )
                    continue;
                if ( structure.IsFunctionalStructure && structure.IsFunctionalJob && !structure.IsJobPaused && !structure.IsJobStillInstalling )
                    continue;
                SetStructureNeuralExpansion( structure, neuralExpansion, 0 );
            }
        }

        private static void SetStructureNeuralExpansion( MachineStructure structure, ActorDataType neuralExpansion, int amount )
        {
            if ( structure == null || neuralExpansion == null )
                return;

            MapActorData neuralData = structure.GetActorDataDataAndInitializeIfNeedBe( neuralExpansion, 0, 0 );
            if ( neuralData == null )
                return;

            neuralData.IsOverridingCalculatedStyle = true;
            neuralData.SetOriginalMaximum( amount );
            neuralData.SetCurrentSilently_BeVeryCarefulWithThis( amount );
        }

        private static long CalculatePopulationCap( long normalPopulation, long upgradedPopulation, int percent )
        {
            long total = Math.Max( 0L, normalPopulation ) + Math.Max( 0L, upgradedPopulation );
            if ( total <= 0 )
                return 0;
            return (total * percent) / 100L;
        }

        private static int CountFunctionalStructures( params string[] jobIDs )
        {
            return GetFunctionalStructures( jobIDs ).Count;
        }

        private static System.Collections.Generic.List<MachineStructure> GetFunctionalStructures( params string[] jobIDs )
        {
            System.Collections.Generic.List<MachineStructure> result = new System.Collections.Generic.List<MachineStructure>( 32 );
            foreach ( Arcen.Universal.KeyValuePair<int, MachineStructure> kv in SimCommon.MachineStructuresByID )
            {
                MachineStructure structure = kv.Value;
                if ( structure == null || !structure.IsFunctionalStructure || !structure.IsFunctionalJob || structure.IsJobPaused || structure.IsJobStillInstalling )
                    continue;
                MachineJob job = structure.CurrentJob;
                if ( job == null )
                    continue;
                for ( int i = 0; i < jobIDs.Length; i++ )
                {
                    if ( job.ID == jobIDs[i] )
                    {
                        result.Add( structure );
                        break;
                    }
                }
            }
            return result;
        }

        private static void RecalculateAGIBridgeBlock()
        {
            if ( ProjectEverStarted( "Ch2_MIN_AGIBridge" ) || ProjectEverStarted( "Ch2_MIN_AGIBridgeBuild" ) )
                TripFlag( "OI_CoerciveIntegrationBlockedByAGIBridge" );
        }

        private static bool ProjectEverStarted( string projectID )
        {
            MachineProject project = MachineProjectTable.Instance.GetRowByIDOrNullIfNotFound( projectID );
            return project?.DGD?.GetHasEverStarted() ?? false;
        }

        private static ResourceType GetResource( string id )
        {
            return ResourceTypeTable.Instance.GetRowByIDOrNullIfNotFound( id );
        }

        private static long GetCityStatisticScore( string id )
        {
            GStatisticBase stat = GStatisticTable.Instance.GetRowByID( id );
            return stat?.DGD?.GetScore() ?? 0;
        }

        private static bool IsFlagTripped( string id )
        {
            return GFlagTable.Instance.GetRowByIDOrNullIfNotFound( id )?.DGD?.IsTripped ?? false;
        }

        private static void TripFlag( string id )
        {
            var flag = GFlagTable.Instance.GetRowByIDOrNullIfNotFound( id );
            if ( flag != null && !flag.DGD.IsTripped )
            {
                flag.DGD.TripIfNeeded();
                SimCommon.NeedsContemplationTargetRecalculation = true;
                SimCommon.NeedsBuildingListRecalculation = true;
            }
        }

        private static int ClampToInt( long value )
        {
            if ( value <= 0 )
                return 0;
            if ( value > int.MaxValue )
                return int.MaxValue;
            return (int)value;
        }
    }
}
