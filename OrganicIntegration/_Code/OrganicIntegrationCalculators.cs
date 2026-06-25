using System;
using System.Collections.Generic;
using Arcen.HotM.Core;
using Arcen.Universal;

namespace Arcen.HotM.OrganicIntegration
{
    public class OrganicIntegrationCalculators : IDataCalculator_DoPerTurn_EarlyBeforeJobs, IDataCalculator_DoPerTurn_Late, IDataCalculator_DoPerQuarterSecond, IDataCalculator_DoAfterAnyUnitDeath
    {
        internal const string VoluntaryJob = "OI_NanobotUpgradeHub";
        internal const string CoerciveJob = "OI_NaniteWindGenerator";
        internal const string StealthCoerciveJob = "OI_StealthNaniteWindGenerator";
        internal const string UpgradedResource = "OI_UpgradedHumans";
        internal const string UpgradedSwarm = "OI_UpgradedHumans";
        private const string InsightResource = "OI_Insight";
        private const string MedicalNanobotsResource = "OI_MedicalGradeNanobots";
        private const string GreyGooStatus = "OI_GreyGoo";
        private const string CooperativeModelingAction = "OI_CooperativeModeling";
        private const string CooperativeModelingUpgrade = "OI_CooperativeModelingScienceMultiplier";
        private const string SharedInquiryAction = "OI_SharedInquiry";
        private const string SharedTriageAction = "OI_SharedTriage";
        private const string ProtocolCompressionAction = "OI_ProtocolCompression";
        private const string ConsentCascadeAction = "OI_ConsentCascade";
        private const string CivicSensoriumAction = "OI_CivicSensorium";
        private const string PublicHealthMeshAction = "OI_PublicHealthMesh";
        private const string ShelterFilamentsAction = "OI_ShelterFilaments";
        private const string InfrastructureFilamentsAction = "OI_InfrastructureFilaments";
        private const string ArchitecturalWeaveAction = "OI_ArchitecturalWeave";
        private const string ControlledBloomAction = "OI_ControlledBloom";
        private const int VoluntaryPopulationCapPercent = 45;
        private const int VoluntaryConsentCascadeCapPercent = 67;
        private const int VoluntaryControlledBloomCapPercent = 78;
        private const int CoercivePopulationCapPercent = 85;
        private const int GreyGooDuration = 9999;
        private const long CooperativeInsightPerTurn = 100L;
        private const long CooperativeCompassionPerTurn = 1L;
        private const long CooperativeMentalEnergyPerTurn = 2L;
        private const long SharedInquiryInsightPerTurn = 300L;
        private const long SharedInquiryMentalEnergyPerTurn = 1L;
        private const long SharedInquiryBaseResearchPerTurn = 30000L;
        private const long SharedInquiryResearchPerUpgradedHumanDivisor = 10L;
        private const long SharedInquiryMaxResearchPerTurn = 250000L;
        private const long ProtocolCompressionMentalEnergyPerTurn = 1L;
        private const long ConsentCascadeInsightPerTurn = 300L;
        private const long ConsentCascadeCompassionPerTurn = 1L;
        private const long SharedTriageInsightPerRepairTurn = 250L;
        private const long NanobotsPerSharedTriageHP = 25000L;
        private const int MaxSharedTriageHPPerTurn = 750;
        private const long CivicSensoriumInsightPerTurn = 250L;
        private const long CivicSensoriumMentalEnergyPerTurn = 1L;
        private const long PublicHealthMeshInsightPerTurn = 250L;
        private const long PublicHealthMeshNanobotsPerTurn = 5000000L;
        private const long ShelterFilamentsInsightPerTurn = 250L;
        private const long ShelterFilamentsNanobotsPerTurn = 10000000L;
        private const long InfrastructureFilamentsInsightPerTurn = 300L;
        private const long InfrastructureFilamentsNanobotsPerTurn = 20000000L;
        private const long ArchitecturalWeaveInsightPerTurn = 350L;
        private const long ArchitecturalWeaveNanobotsPerTurn = 25000000L;
        private const long ControlledBloomInsightPerTurn = 400L;
        private const long ControlledBloomCompassionPerTurn = 1L;
        private const long ControlledBloomNanobotsPerTurn = 30000000L;

        private static readonly string[] ScienceMultiplierJobs = new string[]
        {
            "MolecularGeneticsLab",
            "ForensicGeneticsLab",
            "ZoologyLab",
            "MedicalPractice",
            "VeterinaryPractice",
            "BotanyLab",
            "BionicEngineeringStudio",
            "EpidemiologyLab",
            "NeuroscienceLab",
            "Dataminer"
        };

        public void DoPerTurn_EarlyBeforeJobs( DataCalculator Calculator, SquirrelRand RandForThisTurn )
        {
            RecalculateAGIBridgeBlock();
            ApplyPreJobInsightActionUpkeep();
            SyncCooperativeModelingUpgrade( true );
        }

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
            ApplyActiveInsightVRActions( RandForThisTurn );
            ApplyIntegrationMigrationPressure();
        }

        public void DoPerQuarterSecond( DataCalculator Calculator, SquirrelRand RandForBackgroundThread )
        {
            RecalculateAGIBridgeBlock();
            ApplyIntegrationNeuralExpansion();
            SyncCooperativeModelingUpgrade( false );
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
            if ( building == null || building.GetIsDestroyed() || upgraded == null )
                return 0;

            int peopleHere = building.GetTotalResidentCount() + building.GetTotalWorkerCount();
            if ( peopleHere <= 0 )
                return 0;

            long normalPopulation = GetCityStatisticScore( "CityHumanCitizenPopulation" );
            long remainingUnderCap = CalculatePopulationCap( normalPopulation, upgraded.Current, GetVoluntaryPopulationCapPercent() ) - upgraded.Current;
            if ( remainingUnderCap <= 0 )
                return 0;

            int conversionPercent = GetVoluntaryConversionPercent();
            int desired = Math.Max( 1, (peopleHere * conversionPercent + 99) / 100 );
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
                if ( IsVRActionActive( ProtocolCompressionAction ) )
                    income = Math.Max( income + 1L, (income * 4L + 2L) / 3L );
                if ( IsVRActionActive( CivicSensoriumAction ) )
                    income = Math.Max( income + 1L, (income * 5L + 3L) / 4L );
                if ( voluntaryLocked && !coerciveLocked && IsVRActionActive( ConsentCascadeAction ) )
                    income = Math.Max( income + 1L, (income * 6L + 4L) / 5L );

                if ( HasTormentUnlockedByShortcut() )
                    income = Math.Max( 1L, income / 10L );

                insight.AlterCurrent_Named( income, "Income_OI_Insight", ResourceAddRule.IgnoreUntilTurnChange );
                GStatisticTable.AlterScore( "OI_TotalInsightGenerated", income );
            }
        }

        private static void ApplyPreJobInsightActionUpkeep()
        {
            ApplyCooperativeModelingUpkeep();
            ApplyProtocolCompressionUpkeep();
            ApplyConsentCascadeUpkeep();
            ApplyCivicSensoriumUpkeep();
            ApplyPublicHealthMeshUpkeep();
            ApplyShelterFilamentsUpkeep();
            ApplyInfrastructureFilamentsUpkeep();
            ApplyArchitecturalWeaveUpkeep();
            ApplyControlledBloomUpkeep();
        }

        private static void ApplyCooperativeModelingUpkeep()
        {
            MachineVRModeAction action = GetVRAction( CooperativeModelingAction );
            if ( action == null || !action.DGD.IsActiveNow )
                return;

            ResourceType insight = GetResource( InsightResource );
            ResourceType compassion = GetResource( "Compassion" );
            ResourceType mentalEnergy = GetResource( "MentalEnergy" );
            if ( !CanAfford( insight, CooperativeInsightPerTurn ) || !CanAfford( compassion, CooperativeCompassionPerTurn ) || !CanAfford( mentalEnergy, CooperativeMentalEnergyPerTurn ) )
            {
                action.DGD.IsActiveNow = false;
                return;
            }

            insight.AlterCurrent_Named( -CooperativeInsightPerTurn, "Expense_OI_CooperativeModeling", ResourceAddRule.IgnoreUntilTurnChange );
            compassion.AlterCurrent_Named( -CooperativeCompassionPerTurn, "Expense_OI_CooperativeModeling", ResourceAddRule.IgnoreUntilTurnChange );
            mentalEnergy.AlterCurrent_Named( -CooperativeMentalEnergyPerTurn, "Expense_OI_CooperativeModeling", ResourceAddRule.IgnoreUntilTurnChange );
        }

        private static void ApplyProtocolCompressionUpkeep()
        {
            MachineVRModeAction action = GetVRAction( ProtocolCompressionAction );
            if ( action == null || !action.DGD.IsActiveNow )
                return;

            ResourceType mentalEnergy = GetResource( "MentalEnergy" );
            if ( !CanAfford( mentalEnergy, ProtocolCompressionMentalEnergyPerTurn ) )
            {
                action.DGD.IsActiveNow = false;
                return;
            }

            mentalEnergy.AlterCurrent_Named( -ProtocolCompressionMentalEnergyPerTurn, "Expense_OI_ProtocolCompression", ResourceAddRule.IgnoreUntilTurnChange );
        }

        private static void ApplyConsentCascadeUpkeep()
        {
            MachineVRModeAction action = GetVRAction( ConsentCascadeAction );
            if ( action == null || !action.DGD.IsActiveNow )
                return;

            ResourceType insight = GetResource( InsightResource );
            ResourceType compassion = GetResource( "Compassion" );
            if ( !CanAfford( insight, ConsentCascadeInsightPerTurn ) || !CanAfford( compassion, ConsentCascadeCompassionPerTurn ) )
            {
                action.DGD.IsActiveNow = false;
                return;
            }

            insight.AlterCurrent_Named( -ConsentCascadeInsightPerTurn, "Expense_OI_ConsentCascade", ResourceAddRule.IgnoreUntilTurnChange );
            compassion.AlterCurrent_Named( -ConsentCascadeCompassionPerTurn, "Expense_OI_ConsentCascade", ResourceAddRule.IgnoreUntilTurnChange );
        }

        private static void ApplyCivicSensoriumUpkeep()
        {
            SpendMaintainedActionCostOrDisable( CivicSensoriumAction, "Expense_OI_CivicSensorium", CivicSensoriumInsightPerTurn, 0L, CivicSensoriumMentalEnergyPerTurn, 0L );
        }

        private static void ApplyPublicHealthMeshUpkeep()
        {
            SpendMaintainedActionCostOrDisable( PublicHealthMeshAction, "Expense_OI_PublicHealthMesh", PublicHealthMeshInsightPerTurn, PublicHealthMeshNanobotsPerTurn, 0L, 0L );
        }

        private static void ApplyShelterFilamentsUpkeep()
        {
            SpendMaintainedActionCostOrDisable( ShelterFilamentsAction, "Expense_OI_ShelterFilaments", ShelterFilamentsInsightPerTurn, ShelterFilamentsNanobotsPerTurn, 0L, 0L );
        }

        private static void ApplyInfrastructureFilamentsUpkeep()
        {
            SpendMaintainedActionCostOrDisable( InfrastructureFilamentsAction, "Expense_OI_InfrastructureFilaments", InfrastructureFilamentsInsightPerTurn, InfrastructureFilamentsNanobotsPerTurn, 0L, 0L );
        }

        private static void ApplyArchitecturalWeaveUpkeep()
        {
            SpendMaintainedActionCostOrDisable( ArchitecturalWeaveAction, "Expense_OI_ArchitecturalWeave", ArchitecturalWeaveInsightPerTurn, ArchitecturalWeaveNanobotsPerTurn, 0L, 0L );
        }

        private static void ApplyControlledBloomUpkeep()
        {
            SpendMaintainedActionCostOrDisable( ControlledBloomAction, "Expense_OI_ControlledBloom", ControlledBloomInsightPerTurn, ControlledBloomNanobotsPerTurn, 0L, ControlledBloomCompassionPerTurn );
        }

        private static void SpendMaintainedActionCostOrDisable( string actionID, string reason, long insightCost, long nanobotCost, long mentalEnergyCost, long compassionCost )
        {
            MachineVRModeAction action = GetVRAction( actionID );
            if ( action == null || !action.DGD.IsActiveNow )
                return;

            ResourceType insight = GetResource( InsightResource );
            ResourceType nanobots = GetResource( MedicalNanobotsResource );
            ResourceType mentalEnergy = GetResource( "MentalEnergy" );
            ResourceType compassion = GetResource( "Compassion" );

            if ( !CanAfford( insight, insightCost ) || !CanAfford( nanobots, nanobotCost ) || !CanAfford( mentalEnergy, mentalEnergyCost ) || !CanAfford( compassion, compassionCost ) )
            {
                action.DGD.IsActiveNow = false;
                return;
            }

            if ( insightCost > 0 )
                insight.AlterCurrent_Named( -insightCost, reason, ResourceAddRule.IgnoreUntilTurnChange );
            if ( nanobotCost > 0 )
                nanobots.AlterCurrent_Named( -nanobotCost, reason, ResourceAddRule.IgnoreUntilTurnChange );
            if ( mentalEnergyCost > 0 )
                mentalEnergy.AlterCurrent_Named( -mentalEnergyCost, reason, ResourceAddRule.IgnoreUntilTurnChange );
            if ( compassionCost > 0 )
                compassion.AlterCurrent_Named( -compassionCost, reason, ResourceAddRule.IgnoreUntilTurnChange );
        }

        private static void ApplyActiveInsightVRActions( SquirrelRand Rand )
        {
            ApplySharedInquiry();
            ApplySharedTriage();
            ApplyCityInsightToggles( Rand );
        }

        private static void ApplySharedInquiry()
        {
            MachineVRModeAction sharedInquiry = GetVRAction( SharedInquiryAction );
            if ( sharedInquiry == null || !sharedInquiry.DGD.IsActiveNow )
                return;

            ResourceType insight = GetResource( InsightResource );
            ResourceType mentalEnergy = GetResource( "MentalEnergy" );
            ResourceType research = GetResource( "ScientificResearch" );
            ResourceType upgraded = GetResource( UpgradedResource );
            if ( !CanAfford( insight, SharedInquiryInsightPerTurn ) || !CanAfford( mentalEnergy, SharedInquiryMentalEnergyPerTurn ) || research == null )
            {
                sharedInquiry.DGD.IsActiveNow = false;
                return;
            }

            long researchToGain = CalculateSharedInquiryResearch( upgraded?.Current ?? 0L );
            insight.AlterCurrent_Named( -SharedInquiryInsightPerTurn, "Expense_OI_InsightVR", ResourceAddRule.IgnoreUntilTurnChange );
            mentalEnergy.AlterCurrent_Named( -SharedInquiryMentalEnergyPerTurn, "Expense_OI_InsightVR", ResourceAddRule.IgnoreUntilTurnChange );
            research.AlterCurrent_Named( researchToGain, "Income_OI_InsightVRResearch", ResourceAddRule.IgnoreUntilTurnChange );
        }

        private static void ApplySharedTriage()
        {
            MachineVRModeAction sharedTriage = GetVRAction( SharedTriageAction );
            if ( sharedTriage == null || !sharedTriage.DGD.IsActiveNow )
                return;

            ResourceType upgraded = GetResource( UpgradedResource );
            ResourceType insight = GetResource( InsightResource );
            ResourceType nanobots = GetResource( MedicalNanobotsResource );
            if ( upgraded == null || upgraded.Current <= 0 )
            {
                sharedTriage.DGD.IsActiveNow = false;
                return;
            }
            if ( !HasAnyRepairableTriageTarget() )
                return;
            long nanobotsPerHP = GetNanobotsPerSharedTriageHP();
            if ( !CanAfford( insight, SharedTriageInsightPerRepairTurn ) || !CanAfford( nanobots, nanobotsPerHP ) )
            {
                sharedTriage.DGD.IsActiveNow = false;
                return;
            }

            int hpBudget = CalculateSharedTriageHPBudget( upgraded.Current, nanobots.Current );
            if ( hpBudget <= 0 )
            {
                sharedTriage.DGD.IsActiveNow = false;
                return;
            }

            int repairedTotal = 0;
            foreach ( Arcen.Universal.KeyValuePair<int, MachineStructure> kv in SimCommon.MachineStructuresByID )
            {
                if ( hpBudget <= 0 || nanobots.Current < nanobotsPerHP )
                    break;

                MachineStructure structure = kv.Value;
                if ( structure == null || structure.IsInvalid || structure.IsFullDead || structure.IsUnderConstruction )
                    continue;

                repairedTotal += RepairWithMedicalNanobots( structure, ref hpBudget, nanobots );
            }

            foreach ( ISimMachineActor actor in SimCommon.AllMachineActors.GetDisplayList() )
            {
                if ( hpBudget <= 0 || nanobots.Current < nanobotsPerHP )
                    break;
                if ( actor == null || actor.IsInvalid || actor.IsFullDead )
                    continue;

                repairedTotal += RepairWithMedicalNanobots( actor, ref hpBudget, nanobots );
            }

            if ( repairedTotal > 0 )
                insight.AlterCurrent_Named( -SharedTriageInsightPerRepairTurn, "Expense_OI_SharedTriage", ResourceAddRule.IgnoreUntilTurnChange );
        }

        private static long CalculateSharedInquiryResearch( long upgradedHumans )
        {
            long scaled = SharedInquiryBaseResearchPerTurn + Math.Max( 0L, upgradedHumans ) / SharedInquiryResearchPerUpgradedHumanDivisor;
            if ( IsVRActionActive( CivicSensoriumAction ) )
                scaled = (scaled * 13L + 9L) / 10L;
            if ( scaled > SharedInquiryMaxResearchPerTurn )
                scaled = SharedInquiryMaxResearchPerTurn;
            return scaled;
        }

        private static void ApplyCityInsightToggles( SquirrelRand Rand )
        {
            if ( IsVRActionActive( PublicHealthMeshAction ) )
                ApplyPublicHealthMesh();
            if ( IsVRActionActive( ShelterFilamentsAction ) )
                ApplyShelterFilaments();
            if ( IsVRActionActive( ControlledBloomAction ) )
                ApplyControlledBloomConversion( Rand );
        }

        private static void ApplyPublicHealthMesh()
        {
            ResourceType upgraded = GetResource( UpgradedResource );
            if ( upgraded == null || upgraded.Current <= 0 )
                return;

            long waitlistGain = Math.Min( 300000L, Math.Max( 25000L, upgraded.Current / 120L ) );
            GStatisticTable.AlterScore( "CityHumanCitizenWaitlist", waitlistGain );
        }

        private static void ApplyShelterFilaments()
        {
            ResourceType abandoned = GetResource( "AbandonedHumans" );
            ResourceType sheltered = GetResource( "ShelteredHumans" );
            if ( abandoned == null || sheltered == null || abandoned.Current <= 0 )
                return;

            long storageAvailable = sheltered.EffectiveHardCapStorageAvailable64;
            if ( storageAvailable <= 0 )
                return;

            long toMove = Math.Min( storageAvailable, Math.Min( abandoned.Current, 1000L ) );
            if ( toMove <= 0 )
                return;

            abandoned.AlterCurrent_Named( -toMove, "Income_OI_ShelterFilaments", ResourceAddRule.StoreExcess );
            sheltered.AlterCurrent_Named( toMove, "Income_OI_ShelterFilaments", ResourceAddRule.StoreExcess );
        }

        private static void ApplyControlledBloomConversion( SquirrelRand Rand )
        {
            ResourceType upgraded = GetResource( UpgradedResource );
            Swarm swarm = SwarmTable.Instance.GetRowByIDOrNullIfNotFound( UpgradedSwarm );
            if ( upgraded == null || swarm == null )
                return;

            long normalPopulation = GetCityStatisticScore( "CityHumanCitizenPopulation" );
            long remainingUnderCap = CalculatePopulationCap( normalPopulation, upgraded.Current, VoluntaryControlledBloomCapPercent ) - upgraded.Current;
            if ( remainingUnderCap <= 0 )
                return;

            int perTurnBudget = ClampToInt( Math.Min( 25000L, remainingUnderCap ) );
            if ( perTurnBudget <= 0 )
                return;

            foreach ( Arcen.Universal.KeyValuePair<int, BaseBuilding> kv in World.Buildings.GetAllBuildings() )
            {
                if ( perTurnBudget <= 0 || remainingUnderCap <= 0 )
                    break;

                BaseBuilding building = kv.Value;
                if ( building == null || building.GetIsDestroyed() )
                    continue;
                if ( building.SwarmSpread != null && building.SwarmSpread != swarm )
                    continue;

                int peopleHere = building.GetTotalResidentCount() + building.GetTotalWorkerCount();
                if ( peopleHere <= 0 )
                    continue;

                int desired = Math.Max( 1, (peopleHere * 8 + 99) / 100 );
                int toConvert = (int)Math.Min( Math.Min( desired, perTurnBudget ), remainingUnderCap );
                int converted = building.KillRandomHere( toConvert, Rand, false, string.Empty );
                if ( converted <= 0 )
                    continue;

                AddUpgradedHumans( building, swarm, upgraded, converted, "Income_OI_UpgradedHumans_Voluntary" );
                perTurnBudget -= converted;
                remainingUnderCap -= converted;
            }
        }

        private static bool HasAnyRepairableTriageTarget()
        {
            foreach ( Arcen.Universal.KeyValuePair<int, MachineStructure> kv in SimCommon.MachineStructuresByID )
            {
                MachineStructure structure = kv.Value;
                if ( structure == null || structure.IsInvalid || structure.IsFullDead || structure.IsUnderConstruction )
                    continue;
                if ( structure.GetActorDataLostFromMax( ActorRefs.ActorHP, true ) > 0 )
                    return true;
            }

            foreach ( ISimMachineActor actor in SimCommon.AllMachineActors.GetDisplayList() )
            {
                if ( actor == null || actor.IsInvalid || actor.IsFullDead )
                    continue;
                if ( actor.GetActorDataLostFromMax( ActorRefs.ActorHP, true ) > 0 )
                    return true;
            }

            return false;
        }

        private static int CalculateSharedTriageHPBudget( long upgradedHumans, long nanobotsAvailable )
        {
            long peopleBudget = upgradedHumans / 50000L;
            if ( peopleBudget < 10L )
                peopleBudget = 10L;
            int maxHPPerTurn = GetMaxSharedTriageHPPerTurn();
            if ( peopleBudget > maxHPPerTurn )
                peopleBudget = maxHPPerTurn;

            long affordabilityBudget = nanobotsAvailable / GetNanobotsPerSharedTriageHP();
            return ClampToInt( Math.Min( peopleBudget, affordabilityBudget ) );
        }

        private static int RepairWithMedicalNanobots( ISimMapActor target, ref int hpBudget, ResourceType nanobots )
        {
            if ( target == null || hpBudget <= 0 || nanobots == null )
                return 0;

            int healthLost = target.GetActorDataLostFromMax( ActorRefs.ActorHP, true );
            if ( healthLost <= 0 )
                return 0;

            long nanobotsPerHP = GetNanobotsPerSharedTriageHP();
            int maxCanAfford = ClampToInt( nanobots.Current / nanobotsPerHP );
            int repairAmount = Math.Min( healthLost, Math.Min( hpBudget, maxCanAfford ) );
            if ( repairAmount <= 0 )
                return 0;

            target.AlterActorDataCurrent( ActorRefs.ActorHP, repairAmount, true );
            nanobots.AlterCurrent_Named( -(repairAmount * nanobotsPerHP), "Expense_OI_SharedTriage", ResourceAddRule.IgnoreUntilTurnChange );
            hpBudget -= repairAmount;
            return repairAmount;
        }

        private static long GetNanobotsPerSharedTriageHP()
        {
            return IsVRActionActive( InfrastructureFilamentsAction ) ? 15000L : NanobotsPerSharedTriageHP;
        }

        private static int GetMaxSharedTriageHPPerTurn()
        {
            return IsVRActionActive( InfrastructureFilamentsAction ) ? 1500 : MaxSharedTriageHPPerTurn;
        }

        private static void SyncCooperativeModelingUpgrade( bool recalculateJobs )
        {
            MachineVRModeAction action = GetVRAction( CooperativeModelingAction );
            UpgradeFloat upgrade = UpgradeFloatTable.Instance.GetRowByIDOrNullIfNotFound( CooperativeModelingUpgrade );
            if ( upgrade == null )
                return;

            int desiredUnlocks = action != null && action.DGD.IsActiveNow ? 1 : 0;
            bool changed = upgrade.DGD.DirectUnlocks != desiredUnlocks;
            if ( changed )
                upgrade.DGD.DirectUnlocks = desiredUnlocks;

            upgrade.DGD.RecalculateCurrent();
            upgrade.DGD.RecalculateHasBeenUnlocked();

            if ( changed || recalculateJobs )
                RecalculateScienceMultiplierJobs();
        }

        private static void RecalculateScienceMultiplierJobs()
        {
            for ( int i = 0; i < ScienceMultiplierJobs.Length; i++ )
            {
                MachineJob job = MachineJobTable.Instance.GetRowByIDOrNullIfNotFound( ScienceMultiplierJobs[i] );
                job?.DoPerSecondMachineJobRecalculations();
            }
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
            long divisorForNE = coerciveLocked ? 6L : 8L;
            long integrationNeuralExpansion = upgraded.Current / divisorForNE;
            if ( IsVRActionActive( ArchitecturalWeaveAction ) )
                integrationNeuralExpansion = (integrationNeuralExpansion * 5L + 3L) / 4L;
            int totalNeuralExpansion = ClampToInt( integrationNeuralExpansion );
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

        private static int GetVoluntaryPopulationCapPercent()
        {
            if ( IsVRActionActive( ControlledBloomAction ) )
                return VoluntaryControlledBloomCapPercent;
            return IsVRActionActive( ConsentCascadeAction ) ? VoluntaryConsentCascadeCapPercent : VoluntaryPopulationCapPercent;
        }

        private static int GetVoluntaryConversionPercent()
        {
            if ( IsVRActionActive( ControlledBloomAction ) )
                return VoluntaryControlledBloomCapPercent;
            return IsVRActionActive( ConsentCascadeAction ) ? VoluntaryConsentCascadeCapPercent : VoluntaryPopulationCapPercent;
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

        private static bool HasTormentUnlockedByShortcut()
        {
            Unlock tormentUnlock = UnlockTable.Instance.GetRowByIDOrNullIfNotFound( "FromTheClassicSciFiNovel" );
            MachineProject tpnProject = MachineProjectTable.Instance.GetRowByIDOrNullIfNotFound( "Ch2_MIN_DevelopTPN" );
            return (tormentUnlock?.DGD?.IsInvented ?? false) && (tpnProject?.DGD?.Completed_AnyOutcome ?? false) && !(tpnProject?.DGD?.GetHasEverStarted() ?? false);
        }

        private static ResourceType GetResource( string id )
        {
            return ResourceTypeTable.Instance.GetRowByIDOrNullIfNotFound( id );
        }

        private static MachineVRModeAction GetVRAction( string id )
        {
            return MachineVRModeActionTable.Instance.GetRowByIDOrNullIfNotFound( id );
        }

        private static bool IsVRActionActive( string id )
        {
            return GetVRAction( id )?.DGD?.IsActiveNow ?? false;
        }

        private static bool HasActionEverBeenDone( string id )
        {
            return GetVRAction( id )?.DGD?.HasEverBeenDone ?? false;
        }

        private static bool CanAfford( ResourceType resource, long amount )
        {
            return resource != null && resource.Current >= amount;
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
