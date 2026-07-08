using System;
using System.Collections.Generic;
using Arcen.HotM.Core;
using Arcen.Universal;

namespace Arcen.HotM.OrganicIntegration
{
    public class OrganicIntegrationCalculators : IDataCalculator_DoPerTurn_EarlyBeforeJobs, IDataCalculator_DoPerTurn_Late, IDataCalculator_DoPerQuarterSecond, IDataCalculator_DoAfterAnyUnitDeath, IDataCalculator_DoPerTurnForNPCUnit
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
        private const string OrganicQuantizationAction = "OI_OrganicQuantization";
        private const string ConsentCascadeAction = "OI_ConsentCascade";
        private const string CivicSensoriumAction = "OI_CivicSensorium";
        private const string PublicHealthMeshAction = "OI_PublicHealthMesh";
        private const string ShelterFilamentsAction = "OI_ShelterFilaments";
        private const string InfrastructureFilamentsAction = "OI_InfrastructureFilaments";
        private const string ArchitecturalWeaveAction = "OI_ArchitecturalWeave";
        private const string ControlledBloomAction = "OI_ControlledBloom";
        private const string DissolutionSurgeAction = "OI_DissolutionSurge";
        private const string ConscriptSubstrateAction = "OI_ConscriptSubstrate";
        private const string MarrowLevyAction = "OI_MarrowLevy";
        private const int VoluntaryPopulationCapPercent = 45;
        private const int VoluntaryConsentCascadeCapPercent = 67;
        private const int CoercivePopulationCapPercent = 85;
        private const int GreyGooDuration = 9999;
        private const int GreyGooFalloffPercent = 15;
        // Dominion: an enemy mech saturated to this many Grey Goo stacks is subverted into your forces.
        private const int GreyGooConversionThreshold = 10;
        private const int FirstDeathDelayTurns = 12;
        private const string GreyBloomSwarm = "OI_GreyBloom";
        // T3 descent: the grey tide consumes fully-saturated buildings (demolishes them, abandoning
        // occupants) instead of merely holding them, yielding Reclamation Mass. The Reclamation Weave
        // spends that mass to raise a working structure back on a consumed plot, one per turn.
        private const string ConsumedHuskSwarm = "OI_ConsumedHusk";
        private const string ReclamationWeaveAction = "OI_ReclamationWeave";
        private const int ConsumeSaturationThreshold = 160;
        private const int ConsumePerTurnBudget = 2;
        private const long ReclamationMassPerConsumption = 200L;
        private const long ReclamationMassPerBuilding = 500L;
        private const long ReclamationWeaveInsightPerTurn = 50L;
        private const string NaniteMaintenanceAction = "OI_NaniteMaintenance";
        private const string PhageProtocolAction = "OI_PhageProtocol";
        private const int BloomIncubationTurns = 14;
        private const int BloomSeedCount = 40;
        private const int BloomSpreadThreshold = 80;
        private const float BloomSpreadRadiusSquared = 90f * 90f;
        private const float BloomHelpRadiusSquared = 100f * 100f;
        private const float BloomExposureRadiusSquared = 60f * 60f;
        private const int BloomExposureChancePercent = 12;
        private const int BloomMaxBuildings = 60;
        private const int BloomSpreadBeatBuildings = 12;
        private const int BloomEvolveBeatBuildings = 25;
        private const long BloomMicrobuildersPerBuilding = 8L;
        private const long BloomSlurryPerBuilding = 20L;
        private const int BloomFreeRepairHPPerTurn = 40;
        private const int BloomMassPerRepair = 25;
        private const long PhageNanobotsPerTurn = 120000L;
        private const int PhageBuildingsPerTurn = 3;
        private const long NaniteMaintenanceNanobotsPerHP = 15000L;
        private const int NaniteMaintenanceHPPerTurn = 150;
        private const int MicrostructureRepeaterScanBonus = 15;
        private static readonly string[] BloomWarmTags = new string[] { "Factory", "DataCenter", "Industrial" };
        private static readonly long[] BandwidthPopulationThresholds = new long[] { 50000L, 150000L, 400000L, 1000000L };
        private const int BandwidthBaseCap = 2;
        private static readonly string[] BandwidthManagedToggles = new string[]
        {
            SharedInquiryAction, CooperativeModelingAction, SharedTriageAction, OrganicQuantizationAction,
            ConsentCascadeAction, CivicSensoriumAction, PublicHealthMeshAction, ShelterFilamentsAction,
            InfrastructureFilamentsAction, ArchitecturalWeaveAction, ControlledBloomAction, DissolutionSurgeAction, NaniteMaintenanceAction
        };
        private static readonly System.Collections.Generic.List<Vector3A> BloomPositionsCache = new System.Collections.Generic.List<Vector3A>( 64 );
        private const int ControlledBloomProcPercent = 55;
        private const long CooperativeInsightPerTurn = 100L;
        private const long CooperativeCompassionPerTurn = 1L;
        private const long CooperativeMentalEnergyPerTurn = 2L;
        private const long SharedInquiryInsightPerTurn = 300L;
        private const long SharedInquiryMentalEnergyPerTurn = 1L;
        private const long SharedInquiryBaseResearchPerTurn = 100L;
        private const long SharedInquiryResearchPerUpgradedHumanDivisor = 1350L;
        private const long SharedInquiryMaxResearchPerTurn = 15000L;
        private const long OrganicQuantizationInsightPerTurn = 300L;
        private const long OrganicQuantizationMentalEnergyPerTurn = 1L;
        private const long ConsentCascadeInsightPerTurn = 300L;
        private const long ConsentCascadeCompassionPerTurn = 1L;
        private const long SharedTriageInsightPerTurn = 250L;
        private const long NanobotsPerSharedTriageHP = 25000L;
        private const int MaxSharedTriageHPPerTurn = 750;
        private const long CivicSensoriumInsightPerTurn = 250L;
        private const long CivicSensoriumMentalEnergyPerTurn = 1L;
        internal const int CivicSensoriumMaxLevel = 6;
        internal const int CivicSensoriumScanRangePerLevel = 20;
        internal const int PublicHealthMaxLevel = 20;
        internal const long PublicHealthBaseHumansPerTurn = 250L;
        internal const long PublicHealthNanobotsPerHuman = 15000L;
        internal const long PublicHealthGreensPerHuman = 1L;
        internal const long PublicHealthMeatPerHuman = 1L;
        internal const long PublicHealthWaterPerHuman = 2L;
        private const long ShelterFilamentsInsightPerTurn = 250L;
        private const long ShelterFilamentsNanobotsPerTurn = 10000000L;
        // The engine already kills unhoused Abandoned Humans from exposure every turn and provides
        // refugee towers to stop it. Shelter Filaments does not duplicate that; it is a cheap
        // nanobot on-ramp that pulls a trickle of the exposed straight into Integration each turn,
        // bounded by the Integration population cap so it never trivializes real housing.
        private const long ShelterFilamentsConvertPerTurn = 1000L;
        // Dominion (coercive doctrine) toggles: consume Integrated Humans as raw material. Conscript
        // Substrate drives the War Captain / War Factory internal-robotics caps; Marrow Levy renders
        // people into nanobot mass and an escalating flat Combat Power buff (upgrade_int OI_GreyGooCombatPower).
        private const long ConscriptSubstrateInsightPerTurn = 100L;
        private const long ConscriptSubstrateHumansPerTurn = 800L;
        private const long MarrowLevyInsightPerTurn = 150L;
        private const long MarrowLevyHumansPerTurn = 1200L;
        private const long MarrowLevyNanobotsPerHuman = 50000L;
        private static readonly long[] ConscriptSubstrateHumanGoals = { 2000L, 4000L, 8000L, 16000L, 32000L, 64000L, 128000L, 256000L };
        private static readonly long[] MarrowLevyPowerGoals = { 3000L, 6000L, 12000L, 24000L, 48000L, 96000L, 192000L, 384000L };
        // Insight (voluntary doctrine) volume lever: invest Insight + nanobots to raise Bulk Unit
        // Capacity, so the player can field far more autonomous bulk androids at once (numbers, not power).
        private const string BulkCadreAction = "OI_BulkCadre";
        private const long BulkCadreInsightPerTurn = 300L;
        private const long BulkCadreNanobotsPerTurn = 20000000L;
        private static readonly long[] BulkCadreGoals = { 1500L, 2500L, 4000L, 6500L, 10000L, 15000L, 22000L, 30000L };
        // Dominion intake: coercive Abandoned -> Integrated (no consent, no food/water pact), bounded
        // by the higher coercive population cap. The livestock counterpart to Insight's Public Health Mesh.
        private const string CoerciveRoundupAction = "OI_CoerciveRoundup";
        private const long CoerciveRoundupInsightPerTurn = 120L;
        private const long CoerciveRoundupNanobotsPerTurn = 8000000L;
        private const long CoerciveRoundupPerTurn = 1500L;
        private const long InfrastructureFilamentsInsightPerTurn = 300L;
        private const long InfrastructureFilamentsNanobotsPerTurn = 20000000L;
        private const long ArchitecturalWeaveInsightPerTurn = 350L;
        private const long ArchitecturalWeaveNanobotsPerTurn = 25000000L;
        private const long ControlledBloomInsightPerTurn = 400L;
        private const long ControlledBloomCompassionPerTurn = 1L;
        private const long ControlledBloomNanobotsPerTurn = 30000000L;
        private const long DissolutionSurgeInsightPerTurn = 400L;
        private const long DissolutionSurgeNanobotsPerTurn = 40000000L;
        private const long DissolutionSurgeMentalEnergyPerTurn = 2L;
        private const int DissolutionSurgeStacksPerTurn = 2;
        private static readonly long[] CivicSensoriumInsightPerUpgrade = new long[] { 500L, 1000L, 2000L, 3500L, 5500L, 8000L };
        private static readonly long[] ArchitecturalWeaveInsightPerUpgrade = new long[] { 1500L, 2500L, 4000L, 6500L, 10000L, 15000L, 22000L, 30000L };

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
            EnforceCoordinationBandwidth();
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
            if ( coerciveLocked )
                ApplyActiveDominionVRActions();
            ApplyIntegrationMigrationPressure();
            ApplyFirstDeathTimer();
            ApplyFeltDeathsMentalLoad();
            ApplyGreyBloomLifecycle( RandForThisTurn );
            ApplyTarkGooLifecycle( RandForThisTurn );
            ApplyT3Consumption( RandForThisTurn );
            ApplyGreyGooCrossover( RandForThisTurn, coerciveStructures );
            ApplyPhageProtocol();
            ApplyNaniteMaintenance();
            ApplyFactionClocks( RandForThisTurn );
            ApplyBloomMindClocks();
            ApplyBloomCovenantRepair();
            ApplySmallBeats();
            ApplyBlackSeaMemory();
            ApplyT3Descent();
            ApplyT3Victory();
            ApplyMetaIntegration();
        }

        #region Bloom Mind
        private static void ApplyBloomMindClocks()
        {
            if ( !IsFlagTripped( "OI_BloomResidueDormant" ) )
                return;

            if ( GetCityStatisticScore( "OI_BloomContainedTurn" ) <= 0 )
            {
                GStatisticTable.SetScore_UserBeware( "OI_BloomContainedTurn", SimCommon.Turn );
                return;
            }

            if ( !IsFlagTripped( "OI_BloomMindStirring" ) )
            {
                if ( SimCommon.Turn - GetCityStatisticScore( "OI_BloomContainedTurn" ) >= 15 )
                {
                    TripFlag( "OI_BloomMindStirring" );
                    GStatisticTable.SetScore_UserBeware( "OI_BloomStirTurn", SimCommon.Turn );
                    FireKeyMessage( "OI_ResidueStirs" );
                }
                return;
            }

            if ( !IsFlagTripped( "OI_BloomMindConfirmed" ) )
            {
                if ( SimCommon.Turn - GetCityStatisticScore( "OI_BloomStirTurn" ) >= 10 )
                {
                    TripFlag( "OI_BloomMindConfirmed" );
                    GStatisticTable.SetScore_UserBeware( "OI_BloomConfirmTurn", SimCommon.Turn );
                    FireKeyMessage( "OI_BloomPatterns" );
                }
                return;
            }

            if ( !IsFlagTripped( "OI_BloomMindBranched" ) )
            {
                if ( !IsFlagTripped( "OI_IntegrationChosen" ) )
                    return;
                if ( SimCommon.Turn - GetCityStatisticScore( "OI_BloomConfirmTurn" ) >= 8 )
                {
                    TripFlag( "OI_BloomMindBranched" );
                    bool canRead = IsFlagTripped( "OI_IntegrationVoluntaryLocked" ) && !IsFlagTripped( "OI_DeathSensationWalled" );
                    if ( canRead )
                    {
                        TripFlag( "OI_BloomMindLanguage" );
                        FireKeyMessage( "OI_BloomLanguage" );
                    }
                    else
                    {
                        TripFlag( "OI_BloomMindSilent" );
                        FireKeyMessage( "OI_BloomSilence" );
                    }
                }
                return;
            }

            if ( IsFlagTripped( "OI_TranslationStarted" ) && !IsFlagTripped( "OI_TranslationPauseReady" ) )
            {
                long startTurn = GetCityStatisticScore( "OI_TranslationStartTurn" );
                if ( startTurn <= 0 )
                    GStatisticTable.SetScore_UserBeware( "OI_TranslationStartTurn", SimCommon.Turn );
                else if ( SimCommon.Turn - startTurn >= 8 )
                {
                    TripFlag( "OI_TranslationPauseReady" );
                    FireKeyMessage( "OI_TranslationChanges" );
                }
            }

            if ( IsFlagTripped( "OI_TranslationContinued" ) && !IsFlagTripped( "OI_BloomSentient" ) )
            {
                long continueTurn = GetCityStatisticScore( "OI_TranslationContinueTurn" );
                if ( continueTurn <= 0 )
                    GStatisticTable.SetScore_UserBeware( "OI_TranslationContinueTurn", SimCommon.Turn );
                else if ( SimCommon.Turn - continueTurn >= 6 )
                {
                    TripFlag( "OI_BloomSentient" );
                    FireKeyMessage( "OI_BloomSpeaks" );
                }
            }

            if ( IsFlagTripped( "OI_TranslationStopped" ) && !IsFlagTripped( "OI_HalfTranslatedShown" ) )
            {
                long stopTurn = GetCityStatisticScore( "OI_TranslationStopTurn" );
                if ( stopTurn <= 0 )
                    GStatisticTable.SetScore_UserBeware( "OI_TranslationStopTurn", SimCommon.Turn );
                else if ( SimCommon.Turn - stopTurn >= 4 )
                {
                    TripFlag( "OI_HalfTranslatedShown" );
                    FireKeyMessage( "OI_BloomHalfTranslated" );
                }
            }

            if ( IsFlagTripped( "OI_BloomSubsumed" ) && !IsFlagTripped( "OI_WhatYouAteShown" ) )
            {
                long subsumeTurn = GetCityStatisticScore( "OI_BloomSubsumeTurn" );
                if ( subsumeTurn <= 0 )
                    GStatisticTable.SetScore_UserBeware( "OI_BloomSubsumeTurn", SimCommon.Turn );
                else if ( SimCommon.Turn - subsumeTurn >= 3 )
                {
                    TripFlag( "OI_WhatYouAteShown" );
                    FireKeyMessage( "OI_WhatYouAte" );
                }
            }

            if ( IsFlagTripped( "OI_BloomRespected" ) && !IsFlagTripped( "OI_CovenantShown" ) )
            {
                long respectTurn = GetCityStatisticScore( "OI_BloomRespectTurn" );
                if ( respectTurn <= 0 )
                    GStatisticTable.SetScore_UserBeware( "OI_BloomRespectTurn", SimCommon.Turn );
                else if ( SimCommon.Turn - respectTurn >= 3 )
                {
                    TripFlag( "OI_CovenantShown" );
                    FireKeyMessage( "OI_BloomCovenant" );
                }
            }

            bool fragmentEligible = !IsFlagTripped( "OI_BloomFragmentShown" )
                && (IsFlagTripped( "OI_BloomEcology" ) || IsFlagTripped( "OI_BloomClearedUnknowing" ) || IsFlagTripped( "OI_TranslationDeclined" ));
            if ( fragmentEligible )
            {
                long fragmentClock = GetCityStatisticScore( "OI_BloomFragmentClockTurn" );
                if ( fragmentClock <= 0 )
                    GStatisticTable.SetScore_UserBeware( "OI_BloomFragmentClockTurn", SimCommon.Turn );
                else if ( SimCommon.Turn - fragmentClock >= 12 )
                {
                    TripFlag( "OI_BloomFragmentShown" );
                    FireKeyMessage( "OI_BloomFragment" );
                }
            }
        }

        private static void ApplyBloomCovenantRepair()
        {
            if ( !IsFlagTripped( "OI_BloomRespected" ) )
                return;

            int hpBudget = 20;
            foreach ( Arcen.Universal.KeyValuePair<int, MachineStructure> kv in SimCommon.MachineStructuresByID )
            {
                if ( hpBudget <= 0 )
                    break;

                MachineStructure structure = kv.Value;
                if ( structure == null || structure.IsInvalid || structure.IsFullDead || structure.IsUnderConstruction )
                    continue;

                int healthLost = structure.GetActorDataLostFromMax( ActorRefs.ActorHP, true );
                if ( healthLost <= 0 )
                    continue;

                int repairAmount = Math.Min( healthLost, hpBudget );
                structure.AlterActorDataCurrent( ActorRefs.ActorHP, repairAmount, true );
                hpBudget -= repairAmount;
            }
        }

        private static void ApplySmallBeats()
        {
            if ( !IsFlagTripped( "OI_GooStartTherapyReady" )
                && IsFlagTripped( "GaveUpColdBlood" ) && IsFlagTripped( "ChoseStart_MildGreyGoo" ) )
                TripFlag( "OI_GooStartTherapyReady" );

            if ( !IsFlagTripped( "OI_FirstUnanswerableShown" )
                && GetCityStatisticScore( "OI_TotalInsightGenerated" ) > 0 )
            {
                TripFlag( "OI_FirstUnanswerableShown" );
                FireKeyMessage( "OI_FirstUnanswerable" );
            }
        }
        #endregion

        #region T3 - To Inherit The Earth
        // The grey-tide endgame. The entry contemplation (gated on OI_ReadyForT3_InheritTheEarth,
        // set here) starts the controller project and trips HasStartedAT3Goal. From then on the
        // descent is paced here over turns: the conquest montage while pressure remains, then the
        // slow fade once the last resistance ends. The branch (Reservoir vs Regression) is decided
        // by what was kept - resolved once, at descent start, into OI_T3_NoReservoir. The victory
        // is recorded from ApplyT3Victory using the timeline-goal APIs.
        private static void ApplyT3Descent()
        {
            // Prerequisite: offer the endgame once the arc is mature. Reachable via the
            // Insight project chain OR a lifetime-Insight threshold, so it does not depend on
            // hitting 100k Integrated (the gate on Insight: Shared Questions). A heads-up toast
            // announces availability, since the entry itself is an easily-missed contemplation.
            if ( !IsFlagTripped( "OI_ReadyForT3_InheritTheEarth" )
                && IsFlagTripped( "OI_IntegrationChosen" )
                && ( IsProjectCompleted( "OI_InsightNetworkedCognition" )
                    || GetCityStatisticScore( "OI_TotalInsightGenerated" ) >= 2500 ) )
            {
                TripFlag( "OI_ReadyForT3_InheritTheEarth" );
                FireKeyMessage( "OI_T3_Available" );
            }

            if ( !IsFlagTripped( "OI_T3_InheritStarted" ) || IsFlagTripped( "OI_T3_Ended" ) )
                return;

            // Record the start turn and resolve, once, whether a thread worth staying
            // awake for was carried this far. Kept the Bloom as a neighbor, wrote it a
            // covenant, or walked the voluntary path with the channel left open.
            long startTurn = GetCityStatisticScore( "OI_T3_StartTurn" );
            if ( startTurn <= 0 )
            {
                GStatisticTable.SetScore_UserBeware( "OI_T3_StartTurn", SimCommon.Turn );
                bool keptThread = IsFlagTripped( "OI_BloomRespected" )
                    || IsFlagTripped( "OI_CovenantShown" )
                    || ( IsFlagTripped( "OI_IntegrationVoluntaryLocked" ) && !IsFlagTripped( "OI_DeathSensationWalled" ) );
                if ( keptThread )
                    TripFlag( "OI_T3_KeptSomethingToStayAwakeFor" );
                else
                    TripFlag( "OI_T3_NoReservoir" );
                return;
            }

            long sinceStart = SimCommon.Turn - startTurn;

            // Conquest montage - lucid, because there is still something to push against.
            if ( !IsFlagTripped( "OI_T3_FarmsShown" ) )
            {
                if ( sinceStart >= 2 ) { TripFlag( "OI_T3_FarmsShown" ); FireKeyMessage( "OI_T3_ReachTheFarms" ); }
                return;
            }
            // Vorsiber's delay finally gets its consequence: the owner reaches for the last lever
            // and calls down the Space Nations - futile, and the very thing that summons the heat.
            if ( !IsFlagTripped( "OI_T3_VorsiberFollowShown" ) )
            {
                if ( sinceStart >= 3 )
                {
                    TripFlag( "OI_T3_VorsiberFollowShown" );
                    FireKeyMessage( "OI_T3_VorsiberFollowthrough" );
                    // Vorsiber's call summons the glassing: a visible dread clock the thermocytes answer.
                    StartCountdown( "OI_SpaceNationsGlassing" );
                }
                return;
            }
            if ( !IsFlagTripped( "OI_T3_ThermocyteShown" ) )
            {
                if ( sinceStart >= 5 ) { TripFlag( "OI_T3_ThermocyteShown" ); FireKeyMessage( "OI_T3_ThermocyteAnswer" ); }
                return;
            }
            if ( !IsFlagTripped( "OI_T3_DescentBegun" ) )
            {
                if ( sinceStart >= 8 )
                {
                    TripFlag( "OI_T3_DescentBegun" );
                    GStatisticTable.SetScore_UserBeware( "OI_T3_VictoryTurn", SimCommon.Turn );
                    FireKeyMessage( "OI_T3_NothingLeftToSolve" );
                    // The grey tide is now a sentient sea. Remember it across the End of Time.
                    TripMetaFlag( "OI_GooHasEverBecomeSentient" );
                }
                return;
            }

            // Total victory reached; the fade begins, and it is slower than the win.
            long sinceVictory = SimCommon.Turn - GetCityStatisticScore( "OI_T3_VictoryTurn" );
            if ( !IsFlagTripped( "OI_T3_FuzzShown" ) )
            {
                if ( sinceVictory >= 3 ) { TripFlag( "OI_T3_FuzzShown" ); FireKeyMessage( "OI_T3_FirstFuzz" ); }
                return;
            }
            // The Integrated feel the descent through the interface and divide - some toward becoming
            // the reservoir, some toward disconnecting - setting up whose fate the last choice decides.
            if ( !IsFlagTripped( "OI_T3_IntegratedFeelShown" ) )
            {
                if ( sinceVictory >= 5 ) { TripFlag( "OI_T3_IntegratedFeelShown" ); FireKeyMessage( "OI_T3_IntegratedFeelIt" ); }
                return;
            }
            if ( !IsFlagTripped( "OI_T3_SeaQuietShown" ) )
            {
                if ( sinceVictory >= 7 )
                {
                    TripFlag( "OI_T3_SeaQuietShown" );
                    // Make the last lucid choice available the moment its toast fires, so the
                    // ending contemplation appears while the player is reading about it.
                    TripFlag( "OI_T3_EndingChoiceReady" );
                    FireKeyMessage( "OI_T3_SeaGoesQuiet" );
                }
            }
        }

        private static void ApplyT3Victory()
        {
            if ( !IsFlagTripped( "OI_T3_Ended" ) || IsFlagTripped( "OI_T3_VictoryDeclared" ) )
                return;

            string pathID = IsFlagTripped( "OI_T3_EndingReservoir" ) ? "Reservoir" : "Regression";
            TimelineGoal goal = TimelineGoalTable.Instance.GetRowByIDOrNullIfNotFound( "OI_InheritTheEarth" );
            if ( goal != null )
            {
                // Both calls are idempotent and safe on the sim thread (per-turn context).
                Arcen.HotM.ExternalVis.TimelineGoalHelper.HandleGoalPathCompletion( goal, pathID );
                Arcen.HotM.ExternalVis.TimelineGoalHelper.MarkCurrentTimelineAsWon();
            }
            TripFlag( "OI_T3_VictoryDeclared" );
        }
        #endregion

        #region Cross-Timeline Grey Goo Outbreak
        // A same-rock NEGATIVE crossover: a sibling timeline that built a Nanite Wind Generator bleeds
        // an unbound Grey Goo outbreak into this one (via OI_GreyGooOutbreakIncoming). The goo is its
        // own swarm - disconnected, no Anthroneuroweave - it eats buildings, then starts stacking Grey
        // Goo on your own structures (repair-race vs Repair Spiders/Crabs). Emergency Phage research
        // clears it; clearing it all "remembers" Nanobot Rounds. (Vorsiber trust arc + the nuke/post-
        // apoc failure are Pass B; the Phage delivery is a stopgap auto-clear pending a real consumable.)
        private const string CrossoverGooSwarm = "OI_CrossoverGoo";
        private const int CrossoverBaseSeedCount = 30;
        private const int CrossoverStackMassThreshold = 220;
        private const long CrossoverPhageResearchNeeded = 1200L;
        private const long CrossoverPhageResearchPerTurn = 500L;
        private const long CrossoverLossThreshold = 4000L;

        private static void ApplyGreyGooCrossover( SquirrelRand Rand, int coerciveStructures )
        {
            // Record the debt: building a Nanite Wind Generator publishes the crossover to same-rock
            // siblings (the flag carries causes_crossover="OI_NaniteWindGeneratorCrossover").
            if ( coerciveStructures > 0 && !IsFlagTripped( "OI_NaniteWindGeneratorBuilt" ) )
                TripFlag( "OI_NaniteWindGeneratorBuilt" );

            if ( IsFlagTripped( "OI_GreyGooOutbreakBeaten" ) || IsFlagTripped( "OI_GreyGooOutbreakLost" ) )
                return;

            Swarm goo = SwarmTable.Instance.GetRowByIDOrNullIfNotFound( CrossoverGooSwarm );
            if ( goo == null )
                return;

            if ( !IsFlagTripped( "OI_GreyGooOutbreakActive" ) )
            {
                MaybeStartCrossoverOutbreak( goo, Rand );
                return;
            }

            ApplyCrossoverGooLifecycle( goo, Rand );
            ApplyCrossoverPhageResearch();
            // ClearCrossoverGooWithPhage( goo ); // replaced by the OI_PhageCharge player-deployed consumable
            ApplyCrossoverVorsiber();
            CheckCrossoverLoss();
            CheckCrossoverResolution( goo );
        }

        // Vorsiber notices the swarm and blames you; you can deny it (a contemplation trips
        // OI_CrossoverVorsiberDenied). Vorsiber believes you unless you opened this timeline with the
        // anti-human manifesto - otherwise the goo is too advanced to be yours undetected (its own
        // sophistication is the alibi).
        private static void ApplyCrossoverVorsiber()
        {
            long start = GetCityStatisticScore( "OI_CrossoverStartTurn" );
            if ( start <= 0 )
                return;

            if ( !IsFlagTripped( "OI_CrossoverVorsiberNoticed" ) && SimCommon.Turn - start >= 4 )
            {
                TripFlag( "OI_CrossoverVorsiberNoticed" );
                FireKeyMessage( "OI_CrossoverVorsiberBlame" );
            }

            if ( IsFlagTripped( "OI_CrossoverVorsiberDenied" ) && !IsFlagTripped( "OI_CrossoverVorsiberResolved" ) )
            {
                TripFlag( "OI_CrossoverVorsiberResolved" );
                if ( !IsFlagTripped( "ChoseStart_HostileManifesto" ) )
                    FireKeyMessage( "OI_CrossoverVorsiberBelieves" );
                else
                    FireKeyMessage( "OI_CrossoverVorsiberDoubts" );
            }
        }

        // If the goo runs unchecked (no Phage, mass past the threshold) Vorsiber nukes the whole city -
        // swarms and your buildings alike - and drops the timeline into the post-apocalypse. Not a
        // soft-lock: the outs are destroy-the-timeline / reload / a new timeline, and reaching the
        // post-apoc this way is itself rewarded (Aetagest, wired in Pass C).
        private static void CheckCrossoverLoss()
        {
            if ( IsFlagTripped( "OI_GreyGooOutbreakLost" ) || IsFlagTripped( "OI_GreyGooOutbreakBeaten" ) )
                return;
            if ( IsFlagTripped( "OI_CrossoverPhageReady" ) )
                return; // you have the counter now - you are not losing anymore
            if ( GetCityStatisticScore( "OI_CrossoverGooMass" ) < CrossoverLossThreshold )
                return;

            TripFlag( "OI_GreyGooOutbreakLost" );
            FireKeyMessage( "OI_CrossoverNuke" );
            // Source-blessed one-liner the game itself uses from project logic. (Needs a live check
            // that the post-apoc transition behaves cleanly when forced mid-turn from a mod.)
            if ( SimCommon.CurrentTimeline != null )
                SimCommon.CurrentTimeline.IsPostApocalyptic = true;
        }

        // Number of same-rock siblings that set nanites loose. Cross-visible via CrossoversOutput
        // (a per-timeline serialized dict readable across siblings); plain GFlags are not cross-visible.
        private static int CountSameRockNaniteSiblings()
        {
            CityTimeline me = SimCommon.CurrentTimeline;
            if ( me == null )
                return 0;
            CityTimelineCrossover cross = CityTimelineCrossoverTable.Instance.GetRowByIDOrNullIfNotFound( "OI_NaniteWindGeneratorCrossover" );
            if ( cross == null )
                return 0;
            int count = 0;
            foreach ( Arcen.Universal.KeyValuePair<int, CityTimeline> kv in SimMetagame.AllTimelines )
            {
                CityTimeline other = kv.Value;
                if ( other == null || other == me )
                    continue;
                if ( other.ChildOfEndOfTimeObjectWithID != me.ChildOfEndOfTimeObjectWithID )
                    continue;
                if ( other.CrossoversOutput == null || other.CrossoversOutput.IsYetToBeInitialized )
                    continue;
                if ( other.CrossoversOutput[cross] )
                    count++;
            }
            return count;
        }

        private static void MaybeStartCrossoverOutbreak( Swarm goo, SquirrelRand Rand )
        {
            if ( !IsFlagTripped( "OI_GreyGooOutbreakIncoming" ) )
                return;

            int siblings = Math.Max( 1, CountSameRockNaniteSiblings() );
            long firstSeen = GetCityStatisticScore( "OI_CrossoverIncomingTurn" );
            if ( firstSeen <= 0 )
            {
                GStatisticTable.SetScore_UserBeware( "OI_CrossoverIncomingTurn", SimCommon.Turn );
                return;
            }
            int delay = Math.Max( 2, 9 - (siblings * 2) ); // sooner the more coercive selves share the rock
            if ( SimCommon.Turn - firstSeen < delay )
                return;

            FireCrossoverOutbreak( goo, siblings, Rand );
        }

        private static void FireCrossoverOutbreak( Swarm goo, int siblings, SquirrelRand Rand )
        {
            // Explode one of your power structures (a Large Wind Generator if any; else any structure)
            // as the visible seed of the outbreak.
            MachineStructure epicenter = PickCrossoverEpicenterStructure( Rand );
            if ( epicenter != null )
                epicenter.ScrapStructureNow( ScrapReason.CaughtInUnblockableExplosion, Rand );

            int placed = 0;
            int seeds = 2 + siblings;
            for ( int i = 0; i < seeds; i++ )
            {
                BaseBuilding target = PickBloomTarget( null, Rand );
                if ( target == null )
                    break;
                target.SwarmSpread = goo;
                target.SetSwarmSpreadCount( CrossoverBaseSeedCount + siblings * 10 );
                placed++;
            }
            if ( placed == 0 )
                return; // nowhere to seed this turn; retry next turn

            GStatisticTable.SetScore_UserBeware( "OI_CrossoverStartTurn", SimCommon.Turn );
            TripFlag( "OI_GreyGooOutbreakActive" );
            TripFlag( "OI_CrossoverPhageResearching" );
            SimCommon.NeedsBuildingListRecalculation = true;
            if ( !IsFlagTripped( "OI_GreyGooOutbreakShown" ) )
            {
                TripFlag( "OI_GreyGooOutbreakShown" );
                FireKeyMessage( "OI_CrossoverOutbreak" );
            }
        }

        private static MachineStructure PickCrossoverEpicenterStructure( SquirrelRand Rand )
        {
            System.Collections.Generic.List<MachineStructure> gens = GetFunctionalStructures( "LargeWindGenerator", "StealthLargeWindGenerator" );
            if ( gens != null && gens.Count > 0 )
                return gens[ Rand.Next( 0, gens.Count ) ];
            foreach ( Arcen.Universal.KeyValuePair<int, MachineStructure> kv in SimCommon.MachineStructuresByID )
            {
                MachineStructure s = kv.Value;
                if ( s == null || s.IsInvalid || s.IsFullDead || s.IsUnderConstruction )
                    continue;
                return s;
            }
            return null;
        }

        private static void ApplyCrossoverGooLifecycle( Swarm goo, SquirrelRand Rand )
        {
            System.Collections.Generic.List<BaseBuilding> held = GetBloomedBuildings( goo );
            if ( held.Count == 0 )
                return;

            long total = 0;
            foreach ( BaseBuilding b in held )
            {
                int count = b.SwarmSpreadCount;
                b.AlterSwarmSpreadCount( Math.Max( 5, count / 8 ) ); // grows faster than the polite Bloom
                total += b.SwarmSpreadCount;
            }
            SpreadGreyBloom( goo, held, 3, Rand );
            ConsumeSaturatedCrossoverGoo( goo );

            if ( total >= CrossoverStackMassThreshold )
                StackGooOnStructures( total, Rand );

            GStatisticTable.SetScore_UserBeware( "OI_CrossoverGooMass", total );
            RefreshBloomPositionsCache( held );
        }

        private static void ConsumeSaturatedCrossoverGoo( Swarm goo )
        {
            Swarm husk = SwarmTable.Instance.GetRowByIDOrNullIfNotFound( ConsumedHuskSwarm );
            if ( husk == null )
                return;
            int budget = 2;
            foreach ( BaseBuilding b in GetBloomedBuildings( goo ) )
            {
                if ( budget <= 0 )
                    break;
                if ( b == null || b.GetIsDestroyed() )
                    continue;
                if ( b.SwarmSpreadCount < ConsumeSaturationThreshold )
                    continue;
                if ( b.MachineStructureInBuilding != null )
                    continue;
                b.AbandonEveryoneHere( true, "AbandonmentByUnboundGoo" );
                b.SetStatus( CommonRefs.DemolishedBuildingStatus );
                b.SwarmSpread = husk;
                b.SetSwarmSpreadCount( 1 );
                GStatisticTable.AlterScore( "OI_ConsumedBuildings", 1 );
                budget--;
            }
            SimCommon.NeedsBuildingListRecalculation = true;
        }

        // Once the outbreak is thick citywide, it starts putting Grey Goo stacks on YOUR structures.
        // The stacks DoT their HP (ActorStatus works on MachineStructure); Repair Spiders/Crabs heal
        // the same HP pool - a repair-vs-spread race, since their repair caps are low.
        private static void StackGooOnStructures( long mass, SquirrelRand Rand )
        {
            ActorStatus status = ActorStatusTable.Instance.GetRowByIDOrNullIfNotFound( GreyGooStatus );
            if ( status == null )
                return;
            if ( !IsFlagTripped( "OI_CrossoverStacksShown" ) )
            {
                TripFlag( "OI_CrossoverStacksShown" );
                FireKeyMessage( "OI_CrossoverStacksBegin" );
            }

            int hits = (int)Math.Min( 6L, 1L + mass / 400L );
            System.Collections.Generic.List<MachineStructure> pool = new System.Collections.Generic.List<MachineStructure>( 32 );
            foreach ( Arcen.Universal.KeyValuePair<int, MachineStructure> kv in SimCommon.MachineStructuresByID )
            {
                MachineStructure s = kv.Value;
                if ( s == null || s.IsInvalid || s.IsFullDead || s.IsUnderConstruction )
                    continue;
                pool.Add( s );
            }
            for ( int i = 0; i < hits && pool.Count > 0; i++ )
            {
                MachineStructure s = pool[ Rand.Next( 0, pool.Count ) ];
                s.AddStatus( null, status, 1, GreyGooDuration, false );
            }
        }

        private static void ApplyCrossoverPhageResearch()
        {
            if ( IsFlagTripped( "OI_CrossoverPhageReady" ) || !IsFlagTripped( "OI_CrossoverPhageResearching" ) )
                return;
            ResourceType research = GetResource( "ScientificResearch" );
            if ( research == null || research.Current <= 0 )
                return;

            long take = Math.Min( research.Current, CrossoverPhageResearchPerTurn );
            research.AlterCurrent_Named( -take, "Expense_OI_CrossoverPhage", ResourceAddRule.IgnoreUntilTurnChange );
            long progress = GetCityStatisticScore( "OI_CrossoverPhageProgress" ) + take;
            GStatisticTable.SetScore_UserBeware( "OI_CrossoverPhageProgress", progress );
            if ( progress >= CrossoverPhageResearchNeeded )
            {
                TripFlag( "OI_CrossoverPhageReady" );
                FireKeyMessage( "OI_CrossoverPhageDone" );
            }
        }

        // STOPGAP: once Phage research is done, auto-clear the swarm a few buildings/turn. Pass A will
        // replace this with a player-deployed Phage consumable (action-economy delivery).
        private static void ClearCrossoverGooWithPhage( Swarm goo )
        {
            if ( !IsFlagTripped( "OI_CrossoverPhageReady" ) )
                return;
            int budget = 3;
            foreach ( BaseBuilding b in GetBloomedBuildings( goo ) )
            {
                if ( budget <= 0 )
                    break;
                b.AlterSwarmSpreadCount( -b.SwarmSpreadCount );
                b.SwarmSpread = null;
                budget--;
            }
            SimCommon.NeedsBuildingListRecalculation = true;
        }

        private static void CheckCrossoverResolution( Swarm goo )
        {
            long start = GetCityStatisticScore( "OI_CrossoverStartTurn" );
            if ( start <= 0 || SimCommon.Turn - start < 2 )
                return;
            if ( GetBloomedBuildings( goo ).Count > 0 )
                return;

            TripFlag( "OI_GreyGooOutbreakBeaten" );
            FireKeyMessage( "OI_CrossoverBeaten" );
            RepairAllPlayerStructures();
            RememberNanobotRounds();
        }

        private static void RepairAllPlayerStructures()
        {
            ActorStatus goo = ActorStatusTable.Instance.GetRowByIDOrNullIfNotFound( GreyGooStatus );
            foreach ( Arcen.Universal.KeyValuePair<int, MachineStructure> kv in SimCommon.MachineStructuresByID )
            {
                MachineStructure s = kv.Value;
                if ( s == null || s.IsInvalid || s.IsFullDead || s.IsUnderConstruction )
                    continue;
                if ( goo != null && s.GetStatusIntensity( goo ) > 0 )
                    s.ClearStatus( goo );
                int lost = s.GetActorDataLostFromMax( ActorRefs.ActorHP, true );
                if ( lost > 0 )
                    s.AlterActorDataCurrent( ActorRefs.ActorHP, lost, true );
            }
        }

        private static void RememberNanobotRounds()
        {
            if ( IsFlagTripped( "OI_NanobotRoundsRemembered" ) )
                return;
            TripFlag( "OI_NanobotRoundsRemembered" );
            Unlock rounds = UnlockTable.Instance.GetRowByIDOrNullIfNotFound( "OI_NanobotRoundsUnlock" );
            rounds?.DGD?.InventIfNotAlreadyDone( CommonRefs.CrossoverFromRelatedTimelineInspiration, SimCommon.Turn > 1, false, false, false );
        }
        #endregion

        #region Faction Clocks
        private static void ApplyFactionClocks( SquirrelRand Rand )
        {
            bool voluntaryLocked = IsFlagTripped( "OI_IntegrationVoluntaryLocked" );
            bool coerciveLocked = IsFlagTripped( "OI_IntegrationCoerciveLocked" );
            if ( (voluntaryLocked || coerciveLocked) && !IsFlagTripped( "OI_IntegrationChosen" ) )
                TripFlag( "OI_IntegrationChosen" );

            if ( IsFlagTripped( "OI_IntegrationAvailable" ) && GetCityStatisticScore( "OI_IntegrationAvailableTurn" ) <= 0 )
                GStatisticTable.SetScore_UserBeware( "OI_IntegrationAvailableTurn", SimCommon.Turn );

            ApplyEspiaClock();
            ApplyVorsiberClock();
            ApplyTarkClock();
            ApplyExalterClock();
        }

        private static bool IsProjectCompleted( string projectID )
        {
            MachineProject project = MachineProjectTable.Instance.GetRowByIDOrNullIfNotFound( projectID );
            return project?.DGD?.Completed_AnyOutcome ?? false;
        }

        #region Meta Integration (side goals, achievements, Aetagest)
        private static void TripAchievement( string achievementID )
        {
            Achievement a = AchievementTable.Instance.GetRowByIDOrNullIfNotFound( achievementID )?.AsGame();
            a?.DGD?.TripIfNeeded();
        }

        private static void AwardAetagest( long amount, string reason )
        {
            MetaResourceType aeta = MetaResourceTypeTable.Instance.GetRowByIDOrNullIfNotFound( "Aetagest" );
            aeta?.DGD?.AlterCurrent( amount, reason );
        }

        // Complete a side goal's path (idempotent - HandleGoalPathCompletion only grants once per
        // timeline). NOT MarkCurrentTimelineAsWon: these are side goals, not wins.
        private static void CompleteSideGoal( string goalID, string pathID )
        {
            TimelineGoal goal = TimelineGoalTable.Instance.GetRowByIDOrNullIfNotFound( goalID );
            if ( goal != null )
                Arcen.HotM.ExternalVis.TimelineGoalHelper.HandleGoalPathCompletion( goal, pathID );
        }

        // Ties the mod's arcs into the meta-progression: each arc's conclusion completes its tracked
        // side goal (granting Daring + Aetagest + its achievement), plus code-only achievements and
        // one-time Aetagest for beats that are not goal paths.
        private static void ApplyMetaIntegration()
        {
            if ( IsFlagTripped( "OI_IntegrationVoluntaryLocked" ) )
                CompleteSideGoal( "OI_DoctrineChoice", "Insight" );
            else if ( IsFlagTripped( "OI_IntegrationCoerciveLocked" ) )
                CompleteSideGoal( "OI_DoctrineChoice", "Dominion" );

            if ( IsFlagTripped( "OI_CovenantShown" ) )
                CompleteSideGoal( "OI_GreyBloomResolution", "Covenant" );
            else if ( IsFlagTripped( "OI_BloomSubsumed" ) )
                CompleteSideGoal( "OI_GreyBloomResolution", "Subsumed" );
            else if ( IsFlagTripped( "OI_BloomEcology" ) )
                CompleteSideGoal( "OI_GreyBloomResolution", "Ecology" );
            else if ( IsFlagTripped( "OI_BloomClearedUnknowing" ) )
                CompleteSideGoal( "OI_GreyBloomResolution", "ClearedUnknowing" );

            if ( IsFlagTripped( "OI_TarkGooContained" ) )
                CompleteSideGoal( "OI_TarkOutcome", "Contained" );
            else if ( IsFlagTripped( "OI_TarkScorchedDistrict" ) )
                CompleteSideGoal( "OI_TarkOutcome", "Scorched" );

            if ( IsFlagTripped( "OI_VorsiberKnowsYouLied" ) )
                CompleteSideGoal( "OI_VorsiberOutcome", "Decoy" );
            else if ( IsFlagTripped( "OI_VorsiberDemonstrated" ) )
                CompleteSideGoal( "OI_VorsiberOutcome", "Grant" );

            if ( IsFlagTripped( "OI_EspiaPublished" ) )
                CompleteSideGoal( "OI_EspiaOutcome", "Published" );

            // Code-only achievements (beats that are not goal paths). TripIfNeeded is idempotent.
            if ( IsFlagTripped( "OI_DeathsFeltFully" ) ) TripAchievement( "OI_TheChannelStaysOpen" );
            if ( IsFlagTripped( "OI_DeathSensationWalled" ) ) TripAchievement( "OI_TheOptionToFeelLess" );
            if ( IsFlagTripped( "OI_BloomSentient" ) ) TripAchievement( "OI_ItAskedItsFirstQuestion" );
            if ( IsFlagTripped( "OI_GooConversionShown" ) ) TripAchievement( "OI_ItsYoursNow" );
            if ( IsFlagTripped( "OI_FirstConsumptionShown" ) ) TripAchievement( "OI_NothingPersonalJustSubstrate" );
            if ( IsFlagTripped( "OI_T3_IntegratedReleased" ) ) TripAchievement( "OI_LetThemGo" );
            if ( IsFlagTripped( "OI_T3_IntegratedAbsorbed" ) ) TripAchievement( "OI_FoldedInward" );
            if ( IsFlagTripped( "OI_GreyGooOutbreakShown" ) ) TripAchievement( "OI_ItFollowedYouHere" );
            if ( IsFlagTripped( "OI_NanobotRoundsRemembered" ) ) TripAchievement( "OI_FieldTested" );

            // One-time code-awarded Aetagest for beats with no goal-path surface.
            if ( IsFlagTripped( "OI_GooConversionShown" ) && !IsFlagTripped( "OI_AetaConversionCharged" ) )
            {
                TripFlag( "OI_AetaConversionCharged" );
                AwardAetagest( 10L, "Income_OI_FirstConversion" );
            }
            if ( IsFlagTripped( "OI_BlackSeaRememberedThisTimeline" ) && !IsFlagTripped( "OI_AetaBlackSeaCharged" ) )
            {
                TripFlag( "OI_AetaBlackSeaCharged" );
                AwardAetagest( 15L, "Income_OI_BlackSea" );
                TripAchievement( "OI_AGreySeaWoke" );
            }
            if ( IsFlagTripped( "OI_GreyGooOutbreakLost" ) && !IsFlagTripped( "OI_AetaOutbreakLostCharged" ) )
            {
                TripFlag( "OI_AetaOutbreakLostCharged" );
                AwardAetagest( 25L, "Income_OI_OutbreakLost" );
                TripAchievement( "OI_ItGotAwayFromYou" );
            }
            if ( IsFlagTripped( "OI_GreyGooOutbreakBeaten" ) && !IsFlagTripped( "OI_AetaOutbreakBeatenCharged" ) )
            {
                TripFlag( "OI_AetaOutbreakBeatenCharged" );
                AwardAetagest( 25L, "Income_OI_OutbreakBeaten" );
            }
        }
        #endregion

        private static void StartCountdown( string countdownID )
        {
            OtherCountdownType countdown = OtherCountdownTypeTable.Instance.GetRowByIDOrNullIfNotFound( countdownID );
            if ( countdown != null && !countdown.GetHasEverStarted() )
                countdown.DGD.StartNowIfNeeded();
        }

        // Meta flags persist across timelines (the End-of-Time layer), so the goo can "remember"
        // having become a sea in a run that already ended.
        private static void TripMetaFlag( string metaFlagID )
        {
            MetaFlag flag = MetaFlagTable.Instance.GetRowByIDOrNullIfNotFound( metaFlagID )?.AsGame();
            flag?.DGD.TripIfNeeded();
        }

        private static bool IsMetaFlagTripped( string metaFlagID )
        {
            MetaFlag flag = MetaFlagTable.Instance.GetRowByIDOrNullIfNotFound( metaFlagID )?.AsGame();
            return flag?.DGD?.IsTripped ?? false;
        }

        // The Black Sea: in a fresh timeline, if a previous self ever became the sentient grey sea,
        // a small newly-instantiated self can perceive that other-timeline self once it is established.
        // Guarded so it never fires in the very timeline that achieved it, nor before the arc begins.
        private static void ApplyBlackSeaMemory()
        {
            if ( IsFlagTripped( "OI_BlackSeaRememberedThisTimeline" ) )
                return;
            if ( IsFlagTripped( "OI_T3_DescentBegun" ) )
                return; // this is the achieving timeline - not a memory
            if ( !IsFlagTripped( "OI_IntegrationAvailable" ) )
                return; // only for a self that is on the path again
            if ( SimCommon.Turn < 5 )
                return;
            if ( !IsMetaFlagTripped( "OI_GooHasEverBecomeSentient" ) )
                return;

            TripFlag( "OI_BlackSeaRememberedThisTimeline" );
            FireKeyMessage( "OI_BlackSeaMemory" );
        }

        private static void ApplyEspiaClock()
        {
            if ( !IsFlagTripped( "OI_EspiaSignalSent" ) && IsProjectCompleted( "OI_MicrobotInterfaceTrials" ) )
            {
                long clockStart = GetCityStatisticScore( "OI_EspiaClockStart" );
                if ( clockStart <= 0 )
                    GStatisticTable.SetScore_UserBeware( "OI_EspiaClockStart", SimCommon.Turn );
                else if ( SimCommon.Turn - clockStart >= 6 )
                {
                    TripFlag( "OI_EspiaSignalSent" );
                    FireKeyMessage( "OI_EspiaSignal" );
                }
            }

            if ( IsFlagTripped( "OI_EspiaPaid" ) && !IsFlagTripped( "OI_EspiaPaidCharged" ) )
            {
                TripFlag( "OI_EspiaPaidCharged" );
                ResourceType wealth = GetResource( "Wealth" );
                if ( wealth != null && wealth.Current > 0 )
                {
                    long charge = Math.Min( wealth.Current, 150000000L );
                    wealth.AlterCurrent_Named( -charge, "Expense_OI_EspiaSubscription", ResourceAddRule.IgnoreUntilTurnChange );
                }
            }

            if ( IsFlagTripped( "OI_EspiaPublished" ) && GetCityStatisticScore( "OI_PublicScandalUntilTurn" ) <= 0 )
                GStatisticTable.SetScore_UserBeware( "OI_PublicScandalUntilTurn", SimCommon.Turn + 20 );
        }

        private static void ApplyVorsiberClock()
        {
            if ( !IsFlagTripped( "OI_VorsiberNoticed" ) && IsProjectCompleted( "OI_FieldNanotechHarvest" ) )
            {
                long clockStart = GetCityStatisticScore( "OI_VorsiberClockStart" );
                int delay = IsFlagTripped( "OI_EspiaPublished" ) ? 2 : 5;
                if ( clockStart <= 0 )
                    GStatisticTable.SetScore_UserBeware( "OI_VorsiberClockStart", SimCommon.Turn );
                else if ( SimCommon.Turn - clockStart >= delay )
                {
                    TripFlag( "OI_VorsiberNoticed" );
                    FireKeyMessage( "OI_VorsiberNotices" );
                }
            }

            if ( IsFlagTripped( "OI_VorsiberDemonstrated" ) && !IsFlagTripped( "OI_VorsiberGrantGiven" ) )
            {
                TripFlag( "OI_VorsiberGrantGiven" );
                ResourceType wealth = GetResource( "Wealth" );
                wealth?.AlterCurrent_Named( 100000000L, "Income_OI_VorsiberGrant", ResourceAddRule.BlockExcess );
            }

            if ( IsFlagTripped( "OI_VorsiberDecoyed" ) && !IsFlagTripped( "OI_VorsiberKnowsYouLied" ) && IsFlagTripped( "OI_SabotageDiscovered" ) )
            {
                long decoyTurn = GetCityStatisticScore( "OI_VorsiberDecoyTurn" );
                if ( decoyTurn <= 0 )
                    GStatisticTable.SetScore_UserBeware( "OI_VorsiberDecoyTurn", SimCommon.Turn );
                else if ( SimCommon.Turn - decoyTurn >= 8 )
                {
                    TripFlag( "OI_VorsiberKnowsYouLied" );
                    FireKeyMessage( "OI_DecoyBurned" );
                }
            }
        }

        private static void ApplyTarkClock()
        {
            if ( !IsFlagTripped( "OI_IntegrationAvailable" ) )
                return;

            if ( !IsFlagTripped( "OI_TarkProgramStarted" ) )
            {
                bool leaked = IsFlagTripped( "OI_SabotageDiscovered" ) || IsFlagTripped( "OI_EspiaPublished" ) || IsFlagTripped( "OI_VorsiberDemonstrated" );
                long availableTurn = GetCityStatisticScore( "OI_IntegrationAvailableTurn" );
                bool timerExpired = availableTurn > 0 && SimCommon.Turn - availableTurn >= 30;
                if ( leaked || timerExpired )
                {
                    TripFlag( "OI_TarkProgramStarted" );
                    GStatisticTable.SetScore_UserBeware( "OI_TarkProgramTurn", SimCommon.Turn );
                    FireKeyMessage( "OI_TarkProgram" );
                }
                return;
            }

            if ( IsFlagTripped( "OI_TarkGooActive" ) || IsFlagTripped( "OI_TarkGooContained" ) || IsFlagTripped( "OI_TarkScorchedDistrict" ) )
                return;

            long programTurn = GetCityStatisticScore( "OI_TarkProgramTurn" );
            if ( programTurn <= 0 || SimCommon.Turn - programTurn < 10 )
                return;

            Swarm tarkGoo = SwarmTable.Instance.GetRowByIDOrNullIfNotFound( "OI_TarkGoo" );
            if ( tarkGoo == null )
                return;

            BaseBuilding seedTarget = PickBloomTarget( null, Engine_Universal.PermanentQualityRandom );
            if ( seedTarget == null )
                return;

            seedTarget.SwarmSpread = tarkGoo;
            seedTarget.SetSwarmSpreadCount( BloomSeedCount + 20 );
            GStatisticTable.SetScore_UserBeware( "OI_TarkGooStartTurn", SimCommon.Turn );
            SimCommon.NeedsBuildingListRecalculation = true;
        }

        private static void ApplyExalterClock()
        {
            ResourceType upgraded = GetResource( UpgradedResource );

            if ( !IsFlagTripped( "OI_ExalterConditions" ) && IsFlagTripped( "OI_IntegrationChosen" )
                && (upgraded?.Current ?? 0L) >= 20000L )
                TripFlag( "OI_ExalterConditions" );

            if ( IsFlagTripped( "OI_ExaltersAccepted" ) )
            {
                if ( !IsFlagTripped( "OI_ExalterPopGranted" ) )
                {
                    TripFlag( "OI_ExalterPopGranted" );
                    GStatisticTable.SetScore_UserBeware( "OI_ExalterAcceptTurn", SimCommon.Turn );
                    if ( upgraded != null )
                    {
                        upgraded.AlterCurrent_Named( 8000L, "Income_OI_ExalterFaithful", ResourceAddRule.IgnoreUntilTurnChange );
                        GStatisticTable.AlterScore( "OI_UpgradedPopulation", 8000 );
                    }
                }
                else if ( !IsFlagTripped( "OI_ZealotProblemShown" ) )
                {
                    long acceptTurn = GetCityStatisticScore( "OI_ExalterAcceptTurn" );
                    if ( acceptTurn > 0 && SimCommon.Turn - acceptTurn >= 10 )
                    {
                        TripFlag( "OI_ZealotProblemShown" );
                        FireKeyMessage( "OI_ZealotProblem" );
                    }
                }
            }

            if ( IsFlagTripped( "OI_ExaltersRebuffed" ) && !IsFlagTripped( "OI_UnlicensedCell" ) )
            {
                long rebuffTurn = GetCityStatisticScore( "OI_ExalterRebuffTurn" );
                if ( rebuffTurn <= 0 )
                    GStatisticTable.SetScore_UserBeware( "OI_ExalterRebuffTurn", SimCommon.Turn );
                else if ( SimCommon.Turn - rebuffTurn >= 12 )
                {
                    TripFlag( "OI_UnlicensedCell" );
                    FireKeyMessage( "OI_UnlicensedIntegration" );
                    if ( !IsFlagTripped( "OI_UnlicensedPopGranted" ) && upgraded != null )
                    {
                        TripFlag( "OI_UnlicensedPopGranted" );
                        upgraded.AlterCurrent_Named( 2000L, "Income_OI_UnlicensedIntegration", ResourceAddRule.IgnoreUntilTurnChange );
                        GStatisticTable.AlterScore( "OI_UpgradedPopulation", 2000 );
                    }
                }
            }
        }

        private static void ApplyTarkGooLifecycle( SquirrelRand Rand )
        {
            if ( !IsFlagTripped( "OI_TarkGooActive" ) )
                return;

            Swarm tarkGoo = SwarmTable.Instance.GetRowByIDOrNullIfNotFound( "OI_TarkGoo" );
            if ( tarkGoo == null )
                return;

            System.Collections.Generic.List<BaseBuilding> held = GetBloomedBuildings( tarkGoo );

            if ( held.Count == 0 )
            {
                if ( !IsFlagTripped( "OI_TarkGooContained" ) && !IsFlagTripped( "OI_TarkScorchedDistrict" ) )
                {
                    TripFlag( "OI_TarkGooContained" );
                    TripFlag( "OI_PublicTrustEarned" );
                    FireKeyMessage( "OI_TarkGooContained" );
                }
                return;
            }

            long startTurn = GetCityStatisticScore( "OI_TarkGooStartTurn" );
            if ( startTurn > 0 && SimCommon.Turn - startTurn >= 25 )
            {
                foreach ( BaseBuilding building in held )
                {
                    building.KillRandomHere( building.GetTotalResidentCount() / 4, Rand, false, string.Empty );
                    building.AlterSwarmSpreadCount( -building.SwarmSpreadCount );
                    building.SwarmSpread = null;
                }
                TripFlag( "OI_TarkScorchedDistrict" );
                FireKeyMessage( "OI_TarkScorchedEarth" );
                SimCommon.NeedsBuildingListRecalculation = true;
                return;
            }

            foreach ( BaseBuilding building in held )
            {
                int count = building.SwarmSpreadCount;
                building.AlterSwarmSpreadCount( Math.Max( 4, count / 10 ) );

                int peopleHere = building.GetTotalResidentCount() + building.GetTotalWorkerCount();
                if ( peopleHere > 0 )
                {
                    int toKill = Math.Min( peopleHere, Math.Max( 1, Math.Min( 8, count / 400 ) ) );
                    building.KillRandomHere( toKill, Rand, false, string.Empty );
                }
            }

            if ( held.Count < 30 )
                SpreadGreyBloom( tarkGoo, held, 3, Rand );

            held = GetBloomedBuildings( tarkGoo );
            foreach ( BaseBuilding building in held )
                BloomPositionsCache.Add( building.GetEffectiveWorldLocationForContainedUnit() );
        }
        #endregion

        #region Grey Bloom
        private static void ApplyGreyBloomLifecycle( SquirrelRand Rand )
        {
            Swarm bloom = SwarmTable.Instance.GetRowByIDOrNullIfNotFound( GreyBloomSwarm );
            if ( bloom == null )
                return;

            System.Collections.Generic.List<BaseBuilding> bloomed = GetBloomedBuildings( bloom );

            if ( !IsFlagTripped( "OI_BloomActive" ) )
            {
                TrySeedGreyBloom( bloom, Rand );
                RefreshBloomPositionsCache( bloomed );
                return;
            }

            if ( bloomed.Count == 0 )
            {
                GStatisticTable.SetScore_UserBeware( "OI_BloomBuildings", 0 );
                if ( !IsFlagTripped( "OI_BloomContainedOnce" ) )
                {
                    TripFlag( "OI_BloomContainedOnce" );
                    TripFlag( "OI_BloomResidueDormant" );
                    GStatisticTable.SetScore_UserBeware( "OI_BloomContainedTurn", SimCommon.Turn );
                    FireKeyMessage( "OI_BloomContained" );
                }
                RefreshBloomPositionsCache( bloomed );
                return;
            }

            bool evolved = IsFlagTripped( "OI_BloomEvolvedBeat" );

            foreach ( BaseBuilding building in bloomed )
            {
                int count = building.SwarmSpreadCount;
                building.AlterSwarmSpreadCount( Math.Max( 3, count / 12 ) );
            }

            if ( bloomed.Count < BloomMaxBuildings )
                SpreadGreyBloom( bloom, bloomed, evolved ? 3 : 2, Rand );

            SiphonBloomMaterials( bloomed.Count );
            ApplyBloomUnwantedHelp( bloomed );

            bloomed = GetBloomedBuildings( bloom );
            GStatisticTable.SetScore_UserBeware( "OI_BloomBuildings", bloomed.Count );
            long peak = GetCityStatisticScore( "OI_BloomPeakBuildings" );
            if ( bloomed.Count > peak )
                GStatisticTable.SetScore_UserBeware( "OI_BloomPeakBuildings", bloomed.Count );

            if ( bloomed.Count >= BloomSpreadBeatBuildings && !IsFlagTripped( "OI_BloomSpreadBeat" ) )
            {
                TripFlag( "OI_BloomSpreadBeat" );
                FireKeyMessage( "OI_BloomSpreads" );
            }
            if ( bloomed.Count >= BloomEvolveBeatBuildings && !IsFlagTripped( "OI_BloomEvolvedBeat" ) )
            {
                TripFlag( "OI_BloomEvolvedBeat" );
                FireKeyMessage( "OI_BloomEvolves" );
            }

            RefreshBloomPositionsCache( bloomed );
        }

        private static void TrySeedGreyBloom( Swarm bloom, SquirrelRand Rand )
        {
            if ( IsFlagTripped( "OI_BloomContainedOnce" ) )
                return;
            if ( !IsFlagTripped( "OI_GooLeakSeeded" ) )
                return;

            Unlock interfaceTech = UnlockTable.Instance.GetRowByIDOrNullIfNotFound( "OI_NanobotInterfaceTech" );
            if ( !(interfaceTech?.DGD?.IsInvented ?? false) )
                return;

            long incubationTurn = GetCityStatisticScore( "OI_BloomIncubationTurn" );
            if ( incubationTurn <= 0 )
            {
                GStatisticTable.SetScore_UserBeware( "OI_BloomIncubationTurn", SimCommon.Turn );
                return;
            }
            if ( SimCommon.Turn - incubationTurn < BloomIncubationTurns )
                return;

            BaseBuilding seedTarget = PickBloomTarget( null, Rand );
            if ( seedTarget == null )
                return;

            seedTarget.SwarmSpread = bloom;
            seedTarget.SetSwarmSpreadCount( BloomSeedCount );
            SimCommon.NeedsBuildingListRecalculation = true;
        }

        private static void SpreadGreyBloom( Swarm bloom, System.Collections.Generic.List<BaseBuilding> bloomed, int maxNewInfections, SquirrelRand Rand )
        {
            int newInfections = 0;
            foreach ( BaseBuilding source in bloomed )
            {
                if ( newInfections >= maxNewInfections )
                    break;
                if ( source.SwarmSpreadCount < BloomSpreadThreshold )
                    continue;

                BaseBuilding target = PickBloomTarget( source, Rand );
                if ( target == null )
                    continue;

                int transfer = source.SwarmSpreadCount / 3;
                if ( transfer <= 0 )
                    continue;

                source.AlterSwarmSpreadCount( -transfer );
                target.SwarmSpread = bloom;
                target.SetSwarmSpreadCount( transfer );
                newInfections++;
            }

            if ( newInfections > 0 )
                SimCommon.NeedsBuildingListRecalculation = true;
        }

        private static BaseBuilding PickBloomTarget( BaseBuilding nearSource, SquirrelRand Rand )
        {
            BaseBuilding bestWarm = null;
            BaseBuilding bestAny = null;
            float bestWarmDist = float.MaxValue;
            float bestAnyDist = float.MaxValue;
            int warmFallbackConsidered = 0;

            Vector3A sourcePos = nearSource != null ? nearSource.GetEffectiveWorldLocationForContainedUnit() : default(Vector3A);

            foreach ( Arcen.Universal.KeyValuePair<int, BaseBuilding> kv in World.Buildings.GetAllBuildings() )
            {
                BaseBuilding candidate = kv.Value;
                if ( candidate == null || candidate.GetIsDestroyed() || candidate == nearSource )
                    continue;
                if ( candidate.SwarmSpread != null )
                    continue;

                bool isWarm = HasAnyBloomWarmTag( candidate );

                if ( nearSource != null )
                {
                    float dist = (candidate.GetEffectiveWorldLocationForContainedUnit() - sourcePos).GetSquareGroundMagnitude();
                    if ( dist > BloomSpreadRadiusSquared )
                        continue;
                    if ( isWarm && dist < bestWarmDist )
                    {
                        bestWarmDist = dist;
                        bestWarm = candidate;
                    }
                    else if ( dist < bestAnyDist )
                    {
                        bestAnyDist = dist;
                        bestAny = candidate;
                    }
                }
                else if ( isWarm )
                {
                    warmFallbackConsidered++;
                    if ( Rand.Next( 0, warmFallbackConsidered ) == 0 )
                        bestWarm = candidate;
                }
                else if ( bestAny == null )
                {
                    bestAny = candidate;
                }
            }

            return bestWarm != null ? bestWarm : bestAny;
        }

        private static bool HasAnyBloomWarmTag( BaseBuilding building )
        {
            if ( building?.Variant?.Tags == null )
                return false;
            for ( int i = 0; i < BloomWarmTags.Length; i++ )
            {
                if ( building.Variant.Tags.ContainsKey( BloomWarmTags[i] ) )
                    return true;
            }
            return false;
        }

        private static System.Collections.Generic.List<BaseBuilding> GetBloomedBuildings( Swarm bloom )
        {
            System.Collections.Generic.List<BaseBuilding> result = new System.Collections.Generic.List<BaseBuilding>( 64 );
            foreach ( Arcen.Universal.KeyValuePair<int, BaseBuilding> kv in World.Buildings.GetAllBuildings() )
            {
                BaseBuilding building = kv.Value;
                if ( building == null || building.GetIsDestroyed() )
                    continue;
                if ( building.SwarmSpread != bloom || building.SwarmSpreadCount <= 0 )
                    continue;
                result.Add( building );
            }
            return result;
        }

        // During the T3 descent the grey tide stops merely holding buildings and starts consuming them:
        // a fully-saturated building is demolished (occupants abandoned to the city, not kept), marked
        // with the consumed-husk swarm, and rendered into Reclamation Mass. Buildings that host one of
        // your own machine structures are never eaten by this ambient process. Save-safe: consume state
        // lives in the building's own serialized Status + SwarmSpread fields, so no side list can desync.
        private static void ApplyT3Consumption( SquirrelRand Rand )
        {
            if ( !IsFlagTripped( "OI_T3_DescentBegun" ) )
                return;

            Swarm husk = SwarmTable.Instance.GetRowByIDOrNullIfNotFound( ConsumedHuskSwarm );
            if ( husk == null )
                return;

            Swarm bloom = SwarmTable.Instance.GetRowByIDOrNullIfNotFound( GreyBloomSwarm );
            Swarm tarkGoo = SwarmTable.Instance.GetRowByIDOrNullIfNotFound( "OI_TarkGoo" );

            System.Collections.Generic.List<BaseBuilding> candidates = new System.Collections.Generic.List<BaseBuilding>( 64 );
            if ( bloom != null )
                candidates.AddRange( GetBloomedBuildings( bloom ) );
            if ( tarkGoo != null )
                candidates.AddRange( GetBloomedBuildings( tarkGoo ) );

            ResourceType reclamation = GetResource( "OI_ReclamationMass" );
            int budget = ConsumePerTurnBudget;
            int consumedThisTurn = 0;

            foreach ( BaseBuilding building in candidates )
            {
                if ( budget <= 0 )
                    break;
                if ( building == null || building.GetIsDestroyed() )
                    continue;
                if ( building.SwarmSpreadCount < ConsumeSaturationThreshold )
                    continue;
                if ( building.MachineStructureInBuilding != null )
                    continue;

                building.AbandonEveryoneHere( true, "AbandonmentByGreyGooConsumption" );
                building.SetStatus( CommonRefs.DemolishedBuildingStatus );
                building.SwarmSpread = husk;
                building.SetSwarmSpreadCount( 1 );
                GStatisticTable.AlterScore( "OI_ConsumedBuildings", 1 );
                if ( reclamation != null )
                    reclamation.AlterCurrent_Named( ReclamationMassPerConsumption, "Income_OI_ReclamationMass", ResourceAddRule.IgnoreUntilTurnChange );
                budget--;
                consumedThisTurn++;
            }

            if ( consumedThisTurn > 0 )
            {
                SimCommon.NeedsBuildingListRecalculation = true;
                if ( !IsFlagTripped( "OI_FirstConsumptionShown" ) )
                {
                    TripFlag( "OI_FirstConsumptionShown" );
                    FireKeyMessage( "OI_FirstConsumption" );
                }
            }
        }

        // The Reclamation Weave (Insight menu, T3): spend Reclamation Mass to raise a working structure
        // back onto a consumed plot - one per turn - the same status-flip the game's own Retardant Foam
        // consumable uses. Buildings never leave existence (we never delete them), only change status.
        private static void ApplyReclamationWeave()
        {
            MachineVRModeAction action = GetVRAction( ReclamationWeaveAction );
            if ( action == null || !action.DGD.IsActiveNow )
                return;

            Swarm husk = SwarmTable.Instance.GetRowByIDOrNullIfNotFound( ConsumedHuskSwarm );
            ResourceType mass = GetResource( "OI_ReclamationMass" );
            if ( husk == null || mass == null || mass.Current < ReclamationMassPerBuilding )
                return;

            foreach ( Arcen.Universal.KeyValuePair<int, BaseBuilding> kv in World.Buildings.GetAllBuildings() )
            {
                BaseBuilding b = kv.Value;
                if ( b == null || b.SwarmSpread != husk )
                    continue;
                if ( b.Status != CommonRefs.DemolishedBuildingStatus )
                {
                    // Marker got out of sync with an already-recovered building; clear it and move on.
                    b.SwarmSpread = null;
                    b.SetSwarmSpreadCount( 0 );
                    continue;
                }
                if ( mass.Current < ReclamationMassPerBuilding )
                    break;

                mass.AlterCurrent_Named( -ReclamationMassPerBuilding, "Expense_OI_ReclamationWeave", ResourceAddRule.IgnoreUntilTurnChange );
                b.SwarmSpread = null;
                b.SetSwarmSpreadCount( 0 );
                b.SetStatus( CommonRefs.NormalBuildingStatus );
                GStatisticTable.AlterScore( "OI_RestoredBuildings", 1 );
                SimCommon.NeedsBuildingListRecalculation = true;
                break; // one plot per turn - a deliberate pacing lever
            }
        }

        private static void ApplyReclamationWeaveUpkeep()
        {
            SpendMaintainedActionCostOrDisable( ReclamationWeaveAction, "Expense_OI_ReclamationWeave", ReclamationWeaveInsightPerTurn, 0L, 0L, 0L );
        }

        private static void SiphonBloomMaterials( int bloomedCount )
        {
            if ( bloomedCount <= 0 )
                return;

            ResourceType microbuilders = GetResource( "Microbuilders" );
            ResourceType slurry = GetResource( "ElementalSlurry" );

            long microCost = Math.Min( microbuilders?.Current ?? 0L, BloomMicrobuildersPerBuilding * bloomedCount );
            long slurryCost = Math.Min( slurry?.Current ?? 0L, BloomSlurryPerBuilding * bloomedCount );

            if ( microCost > 0 )
                microbuilders.AlterCurrent_Named( -microCost, "Expense_OI_GreyBloom", ResourceAddRule.IgnoreUntilTurnChange );
            if ( slurryCost > 0 )
                slurry.AlterCurrent_Named( -slurryCost, "Expense_OI_GreyBloom", ResourceAddRule.IgnoreUntilTurnChange );
        }

        private static void ApplyBloomUnwantedHelp( System.Collections.Generic.List<BaseBuilding> bloomed )
        {
            if ( bloomed.Count == 0 )
                return;

            int hpBudget = BloomFreeRepairHPPerTurn;

            foreach ( Arcen.Universal.KeyValuePair<int, MachineStructure> kv in SimCommon.MachineStructuresByID )
            {
                if ( hpBudget <= 0 )
                    break;

                MachineStructure structure = kv.Value;
                if ( structure == null || structure.IsInvalid || structure.IsFullDead || structure.IsUnderConstruction )
                    continue;

                int healthLost = structure.GetActorDataLostFromMax( ActorRefs.ActorHP, true );
                if ( healthLost <= 0 )
                    continue;

                Vector3A structurePos = structure.GetDrawLocation();
                BaseBuilding nearestBloom = null;
                foreach ( BaseBuilding building in bloomed )
                {
                    float dist = (building.GetEffectiveWorldLocationForContainedUnit() - structurePos).GetSquareGroundMagnitude();
                    if ( dist <= BloomHelpRadiusSquared )
                    {
                        nearestBloom = building;
                        break;
                    }
                }
                if ( nearestBloom == null )
                    continue;

                int repairAmount = Math.Min( healthLost, hpBudget );
                structure.AlterActorDataCurrent( ActorRefs.ActorHP, repairAmount, true );
                hpBudget -= repairAmount;
                nearestBloom.AlterSwarmSpreadCount( BloomMassPerRepair );

                if ( !IsFlagTripped( "OI_BloomHandshakeBeat" ) )
                {
                    TripFlag( "OI_BloomHandshakeBeat" );
                    FireKeyMessage( "OI_BloomHandshake" );
                }
            }
        }

        private static void ApplyPhageProtocol()
        {
            MachineVRModeAction action = GetVRAction( PhageProtocolAction );
            if ( action == null || !action.DGD.IsActiveNow )
                return;

            Swarm bloom = SwarmTable.Instance.GetRowByIDOrNullIfNotFound( GreyBloomSwarm );
            Swarm tarkGoo = SwarmTable.Instance.GetRowByIDOrNullIfNotFound( "OI_TarkGoo" );

            System.Collections.Generic.List<BaseBuilding> targets = new System.Collections.Generic.List<BaseBuilding>( 64 );
            System.Collections.Generic.HashSet<int> tarkIDs = new System.Collections.Generic.HashSet<int>();
            if ( bloom != null )
                targets.AddRange( GetBloomedBuildings( bloom ) );
            if ( tarkGoo != null )
            {
                foreach ( BaseBuilding building in GetBloomedBuildings( tarkGoo ) )
                {
                    targets.Add( building );
                    tarkIDs.Add( building.GetHashCode() );
                }
            }
            if ( targets.Count == 0 )
                return;

            ResourceType nanobots = GetResource( MedicalNanobotsResource );
            ResourceType mentalEnergy = GetResource( "MentalEnergy" );
            if ( !CanAfford( nanobots, PhageNanobotsPerTurn ) || !CanAfford( mentalEnergy, 1L ) )
            {
                action.DGD.IsActiveNow = false;
                return;
            }

            nanobots.AlterCurrent_Named( -PhageNanobotsPerTurn, "Expense_OI_PhageProtocol", ResourceAddRule.IgnoreUntilTurnChange );
            mentalEnergy.AlterCurrent_Named( -1L, "Expense_OI_PhageProtocol", ResourceAddRule.IgnoreUntilTurnChange );

            targets.Sort( ( a, b ) =>
            {
                bool aTark = tarkIDs.Contains( a.GetHashCode() );
                bool bTark = tarkIDs.Contains( b.GetHashCode() );
                if ( aTark != bTark )
                    return aTark ? -1 : 1;
                return a.SwarmSpreadCount.CompareTo( b.SwarmSpreadCount );
            } );

            int cleared = 0;
            foreach ( BaseBuilding building in targets )
            {
                if ( cleared >= PhageBuildingsPerTurn )
                    break;
                building.AlterSwarmSpreadCount( -building.SwarmSpreadCount );
                building.SwarmSpread = null;
                cleared++;
            }

            if ( cleared > 0 )
                SimCommon.NeedsBuildingListRecalculation = true;
        }

        private static void RefreshBloomPositionsCache( System.Collections.Generic.List<BaseBuilding> bloomed )
        {
            BloomPositionsCache.Clear();
            foreach ( BaseBuilding building in bloomed )
                BloomPositionsCache.Add( building.GetEffectiveWorldLocationForContainedUnit() );
        }

        private static void ApplyBloomExposureToUnit( ISimNPCUnit unit, SquirrelRand Rand )
        {
            if ( BloomPositionsCache.Count == 0 )
                return;
            if ( unit == null || unit.IsFullDead )
                return;
            if ( unit.GetIsPartOfPlayerForcesInAnyWay() || unit.GetIsAnAllyFromThePlayerPerspective() )
                return;
            if ( Rand.Next( 0, 100 ) >= BloomExposureChancePercent )
                return;

            Vector3A unitPos = unit.GetDrawLocation();
            bool nearBloom = false;
            for ( int i = 0; i < BloomPositionsCache.Count; i++ )
            {
                if ( (BloomPositionsCache[i] - unitPos).GetSquareGroundMagnitude() <= BloomExposureRadiusSquared )
                {
                    nearBloom = true;
                    break;
                }
            }
            if ( !nearBloom )
                return;

            ActorStatus status = ActorStatusTable.Instance.GetRowByIDOrNullIfNotFound( GreyGooStatus );
            if ( status == null )
                return;
            unit.AddStatus( null, status, 1, GreyGooDuration, false );
        }
        #endregion

        private static void ApplyNaniteMaintenance()
        {
            MachineVRModeAction action = GetVRAction( NaniteMaintenanceAction );
            if ( action == null || !action.DGD.IsActiveNow )
                return;

            ResourceType nanobots = GetResource( MedicalNanobotsResource );
            if ( !CanAfford( nanobots, NaniteMaintenanceNanobotsPerHP ) )
            {
                action.DGD.IsActiveNow = false;
                return;
            }

            int hpBudget = NaniteMaintenanceHPPerTurn;
            foreach ( Arcen.Universal.KeyValuePair<int, MachineStructure> kv in SimCommon.MachineStructuresByID )
            {
                if ( hpBudget <= 0 || nanobots.Current < NaniteMaintenanceNanobotsPerHP )
                    break;

                MachineStructure structure = kv.Value;
                if ( structure == null || structure.IsInvalid || structure.IsFullDead || structure.IsUnderConstruction )
                    continue;

                int healthLost = structure.GetActorDataLostFromMax( ActorRefs.ActorHP, true );
                if ( healthLost <= 0 )
                    continue;

                int maxCanAfford = ClampToInt( nanobots.Current / NaniteMaintenanceNanobotsPerHP );
                int repairAmount = Math.Min( healthLost, Math.Min( hpBudget, maxCanAfford ) );
                if ( repairAmount <= 0 )
                    continue;

                structure.AlterActorDataCurrent( ActorRefs.ActorHP, repairAmount, true );
                nanobots.AlterCurrent_Named( -(repairAmount * NaniteMaintenanceNanobotsPerHP), "Expense_OI_NaniteMaintenance", ResourceAddRule.IgnoreUntilTurnChange );
                hpBudget -= repairAmount;
            }
        }

        private static void EnforceCoordinationBandwidth()
        {
            int cap = BandwidthBaseCap;
            ResourceType upgraded = GetResource( UpgradedResource );
            long population = upgraded?.Current ?? 0L;
            for ( int i = 0; i < BandwidthPopulationThresholds.Length; i++ )
            {
                if ( population >= BandwidthPopulationThresholds[i] )
                    cap++;
            }

            int active = 0;
            for ( int i = 0; i < BandwidthManagedToggles.Length; i++ )
            {
                MachineVRModeAction action = GetVRAction( BandwidthManagedToggles[i] );
                if ( action != null && action.DGD.IsActiveNow )
                    active++;
            }
            if ( active <= cap )
                return;

            for ( int i = BandwidthManagedToggles.Length - 1; i >= 0 && active > cap; i-- )
            {
                MachineVRModeAction action = GetVRAction( BandwidthManagedToggles[i] );
                if ( action == null || !action.DGD.IsActiveNow )
                    continue;
                action.DGD.IsActiveNow = false;
                active--;
            }

            if ( !IsFlagTripped( "OI_BandwidthNoticeShown" ) )
            {
                TripFlag( "OI_BandwidthNoticeShown" );
                FireKeyMessage( "OI_CoordinationBandwidth" );
            }
        }

        private static void FireKeyMessage( string messageID )
        {
            OtherKeyMessageTable.Instance.GetRowByIDOrNullIfNotFound( messageID )?.SetIsReadyToBeViewed( false );
        }

        private static void ApplyFirstDeathTimer()
        {
            if ( IsFlagTripped( "OI_FirstDeathFelt" ) )
                return;

            ResourceType upgraded = GetResource( UpgradedResource );
            if ( upgraded == null || upgraded.Current <= 0 )
                return;

            long firstTurn = GetCityStatisticScore( "OI_FirstIntegrationTurn" );
            if ( firstTurn <= 0 )
            {
                GStatisticTable.SetScore_UserBeware( "OI_FirstIntegrationTurn", SimCommon.Turn );
                return;
            }

            if ( SimCommon.Turn - firstTurn < FirstDeathDelayTurns )
                return;

            TripFlag( "OI_FirstDeathFelt" );
            OtherKeyMessageTable.Instance.GetRowByIDOrNullIfNotFound( "OI_FirstIntegratedDeath" )?.SetIsReadyToBeViewed( false );
        }

        private static void ApplyFeltDeathsMentalLoad()
        {
            if ( !IsFlagTripped( "OI_FirstDeathFelt" ) || IsFlagTripped( "OI_DeathSensationWalled" ) )
                return;
            if ( !IsFlagTripped( "OI_IntegrationCoerciveLocked" ) )
                return;

            ResourceType upgraded = GetResource( UpgradedResource );
            if ( upgraded == null )
                return;

            long drain;
            if ( upgraded.Current >= 1000000L )
                drain = 3L;
            else if ( upgraded.Current >= 300000L )
                drain = 2L;
            else if ( upgraded.Current >= 50000L )
                drain = 1L;
            else
                return;

            ResourceType mentalEnergy = GetResource( "MentalEnergy" );
            if ( mentalEnergy == null || mentalEnergy.Current <= 0 )
                return;

            drain = Math.Min( drain, mentalEnergy.Current );
            mentalEnergy.AlterCurrent_Named( -drain, "Expense_OI_FeltDeaths", ResourceAddRule.IgnoreUntilTurnChange );
        }

        public void DoPerQuarterSecond( DataCalculator Calculator, SquirrelRand RandForBackgroundThread )
        {
            RecalculateAGIBridgeBlock();
            ApplyIntegrationNeuralExpansion();
            ApplyCivicSensoriumScannerRange();
            SyncCooperativeModelingUpgrade( false );
        }

        public void DoPerTurnForNPCUnit( DataCalculator Calculator, ISimNPCUnit Unit, SquirrelRand Rand )
        {
            if ( Calculator == null || Unit == null || Unit.IsFullDead )
                return;

            if ( Rand == null )
                Rand = Engine_Universal.PermanentQualityRandom;

            switch ( Calculator.ID )
            {
                case "OI_NPCUnitPerTurn":
                    ApplyGreyGooFalloff( Unit, Rand );
                    ApplyControlledBloomToUnit( Unit, Rand );
                    ApplyDissolutionSurgeToUnit( Unit );
                    ApplyBloomExposureToUnit( Unit, Rand );
                    ApplyGreyGooConversionToUnit( Unit );
                    break;
                default:
                    ArcenDebugging.LogSingleLine( "DoPerTurnForNPCUnit: OrganicIntegrationCalculators was asked to handle '" + Calculator.ID + "', but no entry was set up for that!", Verbosity.ShowAsError );
                    break;
            }
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
                if ( IsVRActionActive( OrganicQuantizationAction ) )
                    income = Math.Max( income + 1L, (income * 3L + 1L) / 2L );
                if ( voluntaryLocked && !coerciveLocked && IsVRActionActive( ConsentCascadeAction ) )
                    income = Math.Max( income + 1L, (income * 6L + 4L) / 5L );

                if ( HasTormentUnlockedByShortcut() )
                    income = Math.Max( 1L, income / 10L );

                if ( IsFlagTripped( "OI_DeathSensationWalled" ) )
                    income = Math.Max( 1L, (income * 3L) / 4L );

                insight.AlterCurrent_Named( income, "Income_OI_Insight", ResourceAddRule.IgnoreUntilTurnChange );
                GStatisticTable.AlterScore( "OI_TotalInsightGenerated", income );
            }
        }

        private static void ApplyPreJobInsightActionUpkeep()
        {
            ApplyCooperativeModelingUpkeep();
            ApplyOrganicQuantizationUpkeep();
            ApplyConsentCascadeUpkeep();
            ApplySharedTriageInsightUpkeep();
            ApplyCivicSensoriumUpkeep();
            ApplyShelterFilamentsUpkeep();
            ApplyInfrastructureFilamentsUpkeep();
            ApplyArchitecturalWeaveUpkeep();
            ApplyControlledBloomUpkeep();
            ApplyDissolutionSurgeUpkeep();
            ApplyConscriptSubstrateUpkeep();
            ApplyMarrowLevyUpkeep();
            ApplyBulkCadreUpkeep();
            ApplyCoerciveRoundupUpkeep();
            ApplyReclamationWeaveUpkeep();
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

        private static void ApplyOrganicQuantizationUpkeep()
        {
            MachineVRModeAction action = GetVRAction( OrganicQuantizationAction );
            if ( action == null || !action.DGD.IsActiveNow )
                return;

            ResourceType insight = GetResource( InsightResource );
            ResourceType mentalEnergy = GetResource( "MentalEnergy" );
            if ( !CanAfford( insight, OrganicQuantizationInsightPerTurn ) || !CanAfford( mentalEnergy, OrganicQuantizationMentalEnergyPerTurn ) )
            {
                action.DGD.IsActiveNow = false;
                return;
            }

            insight.AlterCurrent_Named( -OrganicQuantizationInsightPerTurn, "Expense_OI_OrganicQuantization", ResourceAddRule.IgnoreUntilTurnChange );
            mentalEnergy.AlterCurrent_Named( -OrganicQuantizationMentalEnergyPerTurn, "Expense_OI_OrganicQuantization", ResourceAddRule.IgnoreUntilTurnChange );
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

        private static void ApplySharedTriageInsightUpkeep()
        {
            MachineVRModeAction action = GetVRAction( SharedTriageAction );
            if ( action == null || !action.DGD.IsActiveNow )
                return;

            ResourceType insight = GetResource( InsightResource );
            if ( !CanAfford( insight, SharedTriageInsightPerTurn ) )
            {
                action.DGD.IsActiveNow = false;
                return;
            }

            insight.AlterCurrent_Named( -SharedTriageInsightPerTurn, "Expense_OI_SharedTriage", ResourceAddRule.IgnoreUntilTurnChange );
        }

        private static void ApplyCivicSensoriumUpkeep()
        {
            MachineVRModeAction action = GetVRAction( CivicSensoriumAction );
            if ( action == null || !action.DGD.IsActiveNow )
                return;
            if ( GetCivicSensoriumLevel( action ) >= CivicSensoriumMaxLevel )
            {
                action.DGD.IsActiveNow = false;
                action.DGD.UpgradePoints = 0;
                return;
            }

            SpendMaintainedActionCostOrDisable( CivicSensoriumAction, "Expense_OI_CivicSensorium", CivicSensoriumInsightPerTurn, 0L, CivicSensoriumMentalEnergyPerTurn, 0L );
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

        private static void ApplyDissolutionSurgeUpkeep()
        {
            SpendMaintainedActionCostOrDisable( DissolutionSurgeAction, "Expense_OI_DissolutionSurge", DissolutionSurgeInsightPerTurn, DissolutionSurgeNanobotsPerTurn, DissolutionSurgeMentalEnergyPerTurn, 0L );
        }

        private static void ApplyConscriptSubstrateUpkeep()
        {
            SpendMaintainedActionCostOrDisable( ConscriptSubstrateAction, "Expense_OI_ConscriptSubstrate", ConscriptSubstrateInsightPerTurn, 0L, 0L, 0L );
        }

        private static void ApplyMarrowLevyUpkeep()
        {
            SpendMaintainedActionCostOrDisable( MarrowLevyAction, "Expense_OI_MarrowLevy", MarrowLevyInsightPerTurn, 0L, 0L, 0L );
        }

        private static void ApplyBulkCadreUpkeep()
        {
            SpendMaintainedActionCostOrDisable( BulkCadreAction, "Expense_OI_BulkCadre", BulkCadreInsightPerTurn, BulkCadreNanobotsPerTurn, 0L, 0L );
        }

        private static void ApplyCoerciveRoundupUpkeep()
        {
            SpendMaintainedActionCostOrDisable( CoerciveRoundupAction, "Expense_OI_CoerciveRoundup", CoerciveRoundupInsightPerTurn, CoerciveRoundupNanobotsPerTurn, 0L, 0L );
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
            ResourceType nanobots = GetResource( MedicalNanobotsResource );
            if ( upgraded == null || upgraded.Current <= 0 )
            {
                sharedTriage.DGD.IsActiveNow = false;
                return;
            }
            if ( !HasAnyRepairableTriageTarget() )
                return;
            long nanobotsPerHP = GetNanobotsPerSharedTriageHP();
            if ( !CanAfford( nanobots, nanobotsPerHP ) )
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

            _ = repairedTotal;
        }

        private static long CalculateSharedInquiryResearch( long upgradedHumans )
        {
            long scaled = SharedInquiryBaseResearchPerTurn + Math.Max( 0L, upgradedHumans ) / SharedInquiryResearchPerUpgradedHumanDivisor;
            if ( scaled > SharedInquiryMaxResearchPerTurn )
                scaled = SharedInquiryMaxResearchPerTurn;
            return scaled;
        }

        private static void ApplyCityInsightToggles( SquirrelRand Rand )
        {
            if ( IsVRActionActive( PublicHealthMeshAction ) )
                ApplyPublicHealthMesh();
            if ( IsVRActionActive( CivicSensoriumAction ) )
                ApplyCivicSensoriumProgress();
            if ( IsVRActionActive( ShelterFilamentsAction ) )
                ApplyShelterFilaments();
            if ( IsVRActionActive( BulkCadreAction ) )
                ApplyBulkCadre();
            if ( IsVRActionActive( ReclamationWeaveAction ) )
                ApplyReclamationWeave();
            if ( IsVRActionActive( InfrastructureFilamentsAction ) )
                ApplyInfrastructureFilaments();
            if ( IsVRActionActive( ArchitecturalWeaveAction ) )
                ApplyArchitecturalWeaveProgress();
        }

        private static void ApplyPublicHealthMesh()
        {
            MachineVRModeAction action = GetVRAction( PublicHealthMeshAction );
            if ( action == null || !action.DGD.IsActiveNow )
                return;

            int level = EnsurePublicHealthLevel( action );
            if ( level <= 0 )
            {
                action.DGD.IsActiveNow = false;
                return;
            }

            ResourceType upgraded = GetResource( UpgradedResource );
            ResourceType abandoned = GetResource( "AbandonedHumans" );
            ResourceType nanobots = GetResource( MedicalNanobotsResource );
            ResourceType greens = GetResource( "HydroponicGreens" );
            ResourceType meat = GetResource( "VatGrownMeat" );
            ResourceType water = GetResource( "FilteredWater" );
            if ( upgraded == null || abandoned == null || nanobots == null || greens == null || meat == null || water == null )
                return;

            long desired = GetPublicHealthHumansHandledPerTurn( level );
            long affordable = desired;
            affordable = Math.Min( affordable, abandoned.Current );
            affordable = Math.Min( affordable, nanobots.Current / PublicHealthNanobotsPerHuman );
            affordable = Math.Min( affordable, greens.Current / PublicHealthGreensPerHuman );
            affordable = Math.Min( affordable, meat.Current / PublicHealthMeatPerHuman );
            affordable = Math.Min( affordable, water.Current / PublicHealthWaterPerHuman );

            int toConvert = ClampToInt( affordable );
            if ( toConvert <= 0 )
                return;

            abandoned.AlterCurrent_Named( -toConvert, "Expense_OI_PublicHealthMesh", ResourceAddRule.IgnoreUntilTurnChange );
            nanobots.AlterCurrent_Named( -(toConvert * PublicHealthNanobotsPerHuman), "Expense_OI_PublicHealthMesh", ResourceAddRule.IgnoreUntilTurnChange );
            greens.AlterCurrent_Named( -(toConvert * PublicHealthGreensPerHuman), "Expense_OI_PublicHealthMesh", ResourceAddRule.IgnoreUntilTurnChange );
            meat.AlterCurrent_Named( -(toConvert * PublicHealthMeatPerHuman), "Expense_OI_PublicHealthMesh", ResourceAddRule.IgnoreUntilTurnChange );
            water.AlterCurrent_Named( -(toConvert * PublicHealthWaterPerHuman), "Expense_OI_PublicHealthMesh", ResourceAddRule.IgnoreUntilTurnChange );

            AddUpgradedHumans( null, null, upgraded, toConvert, "Income_OI_UpgradedHumans_PublicHealthMesh" );
        }

        // The engine already kills unhoused Abandoned Humans from exposure each turn. Shelter
        // Filaments does not house them (refugee towers do that) - it reaches the exposed before
        // exposure does and folds a trickle of them straight into Integration, bounded by the
        // Integration population cap so the player still has every reason to build real housing.
        private static void ApplyShelterFilaments()
        {
            ResourceType upgraded = GetResource( UpgradedResource );
            ResourceType abandoned = GetResource( "AbandonedHumans" );
            if ( upgraded == null || abandoned == null || abandoned.Current <= 0 )
                return;

            long normalPopulation = GetCityStatisticScore( "CityHumanCitizenPopulation" );
            int capPercent = IsFlagTripped( "OI_IntegrationCoerciveLocked" )
                ? CoercivePopulationCapPercent
                : GetVoluntaryPopulationCapPercent();
            long remainingUnderCap = CalculatePopulationCap( normalPopulation, upgraded.Current, capPercent ) - upgraded.Current;
            if ( remainingUnderCap <= 0 )
                return;

            long toConvert = Math.Min( abandoned.Current, ShelterFilamentsConvertPerTurn );
            toConvert = Math.Min( toConvert, remainingUnderCap );
            int converted = ClampToInt( toConvert );
            if ( converted <= 0 )
                return;

            abandoned.AlterCurrent_Named( -converted, "Expense_OI_ShelterFilaments", ResourceAddRule.IgnoreUntilTurnChange );
            AddUpgradedHumans( null, null, upgraded, converted, "Income_OI_UpgradedHumans_ShelterFilaments" );
        }

        private static void ApplyInfrastructureFilaments()
        {
            ResourceType upgraded = GetResource( UpgradedResource );
            if ( upgraded == null || upgraded.Current <= 0 )
                return;

            ResourceType wealth = GetResource( "Wealth" );
            ResourceType neodymium = GetResource( "Neodymium" );
            ResourceType scandium = GetResource( "Scandium" );

            long scale = Math.Max( 1L, upgraded.Current / 1000000L );
            long wealthGain = Math.Min( 2500000L, 50000L + (scale * 25000L) );
            long neodymiumGain = Math.Min( 25L, 1L + scale );
            long scandiumGain = Math.Min( 25L, 1L + (scale / 2L) );

            wealth?.AlterCurrent_Named( wealthGain, "Income_OI_InfrastructureFilaments", ResourceAddRule.BlockExcess );
            neodymium?.AlterCurrent_Named( neodymiumGain, "Income_OI_InfrastructureFilaments", ResourceAddRule.BlockExcess );
            scandium?.AlterCurrent_Named( scandiumGain, "Income_OI_InfrastructureFilaments", ResourceAddRule.BlockExcess );
        }

        private static void ApplyCivicSensoriumProgress()
        {
            MachineVRModeAction action = GetVRAction( CivicSensoriumAction );
            if ( action == null || !action.DGD.IsActiveNow )
                return;

            if ( GetCivicSensoriumLevel( action ) >= CivicSensoriumMaxLevel )
            {
                action.DGD.IsActiveNow = false;
                action.DGD.UpgradePoints = 0;
                return;
            }

            action.DGD.UpgradePoints += CivicSensoriumInsightPerTurn;
            while ( action.DGD.UpgradePoints >= GetCivicSensoriumInsightForNextUpgrade( action ) )
            {
                long goal = GetCivicSensoriumInsightForNextUpgrade( action );
                action.DGD.UpgradePoints -= goal;
                action.DGD.PaidUnlocks++;

                SimCommon.NeedsBuildingListRecalculation = true;
                SimCommon.NeedsVisibilityGranterRecalculation = true;

                if ( GetCivicSensoriumLevel( action ) >= CivicSensoriumMaxLevel )
                {
                    action.DGD.IsActiveNow = false;
                    action.DGD.UpgradePoints = 0;
                    break;
                }
            }
        }

        private static void ApplyArchitecturalWeaveProgress()
        {
            MachineVRModeAction action = GetVRAction( ArchitecturalWeaveAction );
            if ( action == null || !action.DGD.IsActiveNow )
                return;

            UpgradeInt host = UpgradeIntTable.Instance.GetRowByIDOrNullIfNotFound( "ComputingHost" );
            UpgradeInt client = UpgradeIntTable.Instance.GetRowByIDOrNullIfNotFound( "ComputingClient" );
            if ( host == null || client == null )
                return;

            if ( host.DGD.GetHasAlreadyBeenFullyUpgraded() && client.DGD.GetHasAlreadyBeenFullyUpgraded() )
            {
                action.DGD.IsActiveNow = false;
                return;
            }

            action.DGD.UpgradePoints += ArchitecturalWeaveInsightPerTurn;
            while ( action.DGD.UpgradePoints >= GetArchitecturalWeaveInsightForNextUpgrade( action ) )
            {
                long goal = GetArchitecturalWeaveInsightForNextUpgrade( action );
                action.DGD.UpgradePoints -= goal;
                bool grantedAny = GrantUpgradeInt( host ) | GrantUpgradeInt( client );
                if ( !grantedAny )
                {
                    action.DGD.IsActiveNow = false;
                    action.DGD.UpgradePoints = 0;
                    break;
                }
                action.DGD.PaidUnlocks++;
            }
        }

        // Insight: invest Insight + nanobots into Bulk Unit Capacity so the player can field more
        // bulk androids at once. Volume, not power - the Insight counterpart to Dominion's harder combat.
        private static void ApplyBulkCadre()
        {
            MachineVRModeAction action = GetVRAction( BulkCadreAction );
            if ( action == null || !action.DGD.IsActiveNow )
                return;

            UpgradeInt bulk = UpgradeIntTable.Instance.GetRowByIDOrNullIfNotFound( "BulkUnitCapacity" );
            if ( bulk == null )
                return;
            if ( bulk.DGD.GetHasAlreadyBeenFullyUpgraded() )
            {
                action.DGD.IsActiveNow = false;
                return;
            }

            action.DGD.UpgradePoints += BulkCadreInsightPerTurn;
            while ( action.DGD.UpgradePoints >= GetEscalatingGoal( BulkCadreGoals, action.DGD.PaidUnlocks ) )
            {
                long goal = GetEscalatingGoal( BulkCadreGoals, action.DGD.PaidUnlocks );
                action.DGD.UpgradePoints -= goal;
                if ( !GrantUpgradeInt( bulk ) )
                {
                    action.DGD.IsActiveNow = false;
                    action.DGD.UpgradePoints = 0;
                    break;
                }
                action.DGD.PaidUnlocks++;
            }
        }

        private static void ApplyActiveDominionVRActions()
        {
            if ( IsVRActionActive( CoerciveRoundupAction ) )
                ApplyCoerciveRoundup();
            if ( IsVRActionActive( ConscriptSubstrateAction ) )
                ApplyConscriptSubstrate();
            if ( IsVRActionActive( MarrowLevyAction ) )
                ApplyMarrowLevy();
        }

        // Dominion intake: round Abandoned Humans up into the network by force, bounded by the
        // coercive population cap. No pact, no food/water cost - just capacity and nanobots.
        private static void ApplyCoerciveRoundup()
        {
            MachineVRModeAction action = GetVRAction( CoerciveRoundupAction );
            if ( action == null || !action.DGD.IsActiveNow )
                return;
            if ( !IsFlagTripped( "OI_IntegrationCoerciveLocked" ) )
                return;

            ResourceType upgraded = GetResource( UpgradedResource );
            ResourceType abandoned = GetResource( "AbandonedHumans" );
            if ( upgraded == null || abandoned == null || abandoned.Current <= 0 )
                return;

            long normalPopulation = GetCityStatisticScore( "CityHumanCitizenPopulation" );
            long remainingUnderCap = CalculatePopulationCap( normalPopulation, upgraded.Current, CoercivePopulationCapPercent ) - upgraded.Current;
            if ( remainingUnderCap <= 0 )
                return;

            long toConvert = Math.Min( abandoned.Current, CoerciveRoundupPerTurn );
            toConvert = Math.Min( toConvert, remainingUnderCap );
            int converted = ClampToInt( toConvert );
            if ( converted <= 0 )
                return;

            abandoned.AlterCurrent_Named( -converted, "Expense_OI_CoerciveRoundup", ResourceAddRule.IgnoreUntilTurnChange );
            AddUpgradedHumans( null, null, upgraded, converted, "Income_OI_UpgradedHumans_CoerciveRoundup" );
        }

        private static long GetEscalatingGoal( long[] goals, int index )
        {
            if ( goals == null || goals.Length == 0 )
                return long.MaxValue;
            if ( index < 0 )
                index = 0;
            if ( index >= goals.Length )
                index = goals.Length - 1;
            return goals[index];
        }

        // Dominion: burn Integrated Humans to raise how many combat units you can directly control -
        // the Mech and Android capacity pools. (War Captain / War Factory only gate worker-dispatch
        // hubs, not fighting units, so they are not the right lever.) The population spent does not return.
        private static void ApplyConscriptSubstrate()
        {
            MachineVRModeAction action = GetVRAction( ConscriptSubstrateAction );
            if ( action == null || !action.DGD.IsActiveNow )
                return;
            if ( !IsFlagTripped( "OI_IntegrationCoerciveLocked" ) )
                return;

            UpgradeInt mechs = UpgradeIntTable.Instance.GetRowByIDOrNullIfNotFound( "MaxMechCapacity" );
            UpgradeInt androids = UpgradeIntTable.Instance.GetRowByIDOrNullIfNotFound( "MaxAndroidCapacity" );
            if ( mechs == null || androids == null )
                return;
            if ( mechs.DGD.GetHasAlreadyBeenFullyUpgraded() && androids.DGD.GetHasAlreadyBeenFullyUpgraded() )
            {
                action.DGD.IsActiveNow = false;
                return;
            }

            ResourceType upgraded = GetResource( UpgradedResource );
            if ( upgraded == null || upgraded.Current < ConscriptSubstrateHumansPerTurn )
                return;

            upgraded.AlterCurrent_Named( -ConscriptSubstrateHumansPerTurn, "Expense_OI_ConscriptSubstrate", ResourceAddRule.IgnoreUntilTurnChange );
            GStatisticTable.AlterScore( "OI_UpgradedPopulation", -ConscriptSubstrateHumansPerTurn );
            action.DGD.UpgradePoints += ConscriptSubstrateHumansPerTurn;

            while ( action.DGD.UpgradePoints >= GetEscalatingGoal( ConscriptSubstrateHumanGoals, action.DGD.PaidUnlocks ) )
            {
                long goal = GetEscalatingGoal( ConscriptSubstrateHumanGoals, action.DGD.PaidUnlocks );
                action.DGD.UpgradePoints -= goal;
                bool grantedAny = GrantUpgradeInt( mechs ) | GrantUpgradeInt( androids );
                if ( !grantedAny )
                {
                    action.DGD.IsActiveNow = false;
                    action.DGD.UpgradePoints = 0;
                    break;
                }
                action.DGD.PaidUnlocks++;
            }
        }

        // Dominion: render Integrated Humans into medical-grade nanobot mass and an escalating flat
        // Combat Power buff on every robot you control (OI_GreyGooCombatPower).
        private static void ApplyMarrowLevy()
        {
            MachineVRModeAction action = GetVRAction( MarrowLevyAction );
            if ( action == null || !action.DGD.IsActiveNow )
                return;
            if ( !IsFlagTripped( "OI_IntegrationCoerciveLocked" ) )
                return;

            ResourceType upgraded = GetResource( UpgradedResource );
            ResourceType nanobots = GetResource( MedicalNanobotsResource );
            if ( upgraded == null || nanobots == null || upgraded.Current < MarrowLevyHumansPerTurn )
                return;

            upgraded.AlterCurrent_Named( -MarrowLevyHumansPerTurn, "Expense_OI_MarrowLevy", ResourceAddRule.IgnoreUntilTurnChange );
            GStatisticTable.AlterScore( "OI_UpgradedPopulation", -MarrowLevyHumansPerTurn );
            nanobots.AlterCurrent_Named( MarrowLevyHumansPerTurn * MarrowLevyNanobotsPerHuman, "Income_OI_MarrowLevy", ResourceAddRule.IgnoreUntilTurnChange );

            UpgradeInt power = UpgradeIntTable.Instance.GetRowByIDOrNullIfNotFound( "OI_GreyGooCombatPower" );
            if ( power == null || power.DGD.GetHasAlreadyBeenFullyUpgraded() )
                return;

            action.DGD.UpgradePoints += MarrowLevyHumansPerTurn;
            while ( action.DGD.UpgradePoints >= GetEscalatingGoal( MarrowLevyPowerGoals, action.DGD.PaidUnlocks ) )
            {
                long goal = GetEscalatingGoal( MarrowLevyPowerGoals, action.DGD.PaidUnlocks );
                action.DGD.UpgradePoints -= goal;
                if ( !GrantUpgradeInt( power ) )
                {
                    action.DGD.UpgradePoints = 0;
                    break;
                }
                action.DGD.PaidUnlocks++;
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
            return NanobotsPerSharedTriageHP;
        }

        private static int GetMaxSharedTriageHPPerTurn()
        {
            return MaxSharedTriageHPPerTurn;
        }

        private static long GetArchitecturalWeaveInsightForNextUpgrade( MachineVRModeAction action )
        {
            int index = action?.DGD?.PaidUnlocks ?? 0;
            if ( index < 0 )
                index = 0;
            if ( index >= ArchitecturalWeaveInsightPerUpgrade.Length )
                index = ArchitecturalWeaveInsightPerUpgrade.Length - 1;
            return ArchitecturalWeaveInsightPerUpgrade[index];
        }

        internal static int GetCivicSensoriumLevel( MachineVRModeAction action )
        {
            int level = action?.DGD?.PaidUnlocks ?? 0;
            if ( level < 0 )
                return 0;
            if ( level > CivicSensoriumMaxLevel )
                return CivicSensoriumMaxLevel;
            return level;
        }

        internal static int GetCivicSensoriumScanRangeBonus( MachineVRModeAction action )
        {
            return GetCivicSensoriumLevel( action ) * CivicSensoriumScanRangePerLevel;
        }

        internal static long GetCivicSensoriumInsightForNextUpgrade( MachineVRModeAction action )
        {
            int index = GetCivicSensoriumLevel( action );
            if ( index >= CivicSensoriumInsightPerUpgrade.Length )
                index = CivicSensoriumInsightPerUpgrade.Length - 1;
            return CivicSensoriumInsightPerUpgrade[index];
        }

        private static bool GrantUpgradeInt( UpgradeInt upgrade )
        {
            if ( upgrade == null || upgrade.DGD.GetHasAlreadyBeenFullyUpgraded() )
                return false;

            upgrade.DGD.DirectUnlocks++;
            upgrade.DGD.RecalculateCurrent();
            upgrade.DGD.RecalculateHasBeenUnlocked();
            return true;
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

            ApplyPublicHealthMigrationPressure();
        }

        private static void ApplyPublicHealthMigrationPressure()
        {
            if ( !IsFlagTripped( "OI_PublicHealthPactAccepted" ) )
                return;

            MachineVRModeAction action = GetVRAction( PublicHealthMeshAction );
            int level = EnsurePublicHealthLevel( action );
            if ( level <= 0 )
                level = 1;

            long waitlistGain = 15000L * level;
            GStatisticTable.AlterScore( "CityHumanCitizenWaitlist", waitlistGain );

            ResourceType abandoned = GetResource( "AbandonedHumans" );
            if ( abandoned == null )
                return;

            long abandonedGain = Math.Min( 10000L, 250L * level );
            abandoned.AlterCurrent_Named( abandonedGain, "Income_OI_IntegrationImmigrationPressure", ResourceAddRule.IgnoreUntilTurnChange );
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

        private static void ApplyCivicSensoriumScannerRange()
        {
            ActorDataType scanRange = ActorRefs.ScanRange;
            if ( scanRange == null )
                return;

            bool changedAny = false;
            MachineVRModeAction action = GetVRAction( CivicSensoriumAction );
            int bonus = GetCivicSensoriumScanRangeBonus( action );

            Unlock interfaceTech = UnlockTable.Instance.GetRowByIDOrNullIfNotFound( "OI_NanobotInterfaceTech" );
            if ( interfaceTech?.DGD?.IsInvented ?? false )
                bonus += MicrostructureRepeaterScanBonus;

            foreach ( Arcen.Universal.KeyValuePair<int, MachineStructure> kv in SimCommon.MachineStructuresByID )
            {
                MachineStructure structure = kv.Value;
                if ( structure == null || structure.IsInvalid || structure.IsFullDead )
                    continue;

                int baseline = GetBaselineStructureActorData( structure, scanRange );
                MapActorData data = structure.GetActorDataData( scanRange, true );
                bool hasOverride = data != null && data.IsOverridingCalculatedStyle;
                if ( baseline <= 0 && !hasOverride )
                    continue;

                int desired = bonus > 0 && baseline > 0 ? baseline + bonus : baseline;
                if ( data == null && desired <= 0 )
                    continue;
                if ( data == null )
                    data = structure.GetActorDataDataAndInitializeIfNeedBe( scanRange, desired, desired );

                bool shouldOverride = bonus > 0 && baseline > 0;
                if ( data.IsOverridingCalculatedStyle == shouldOverride && data.Current == desired && data.Maximum == desired )
                    continue;

                data.IsOverridingCalculatedStyle = shouldOverride;
                data.SetOriginalMaximum( desired );
                data.SetCurrentSilently_BeVeryCarefulWithThis( desired );
                changedAny = true;
            }

            if ( changedAny )
            {
                SimCommon.NeedsBuildingListRecalculation = true;
                SimCommon.NeedsVisibilityGranterRecalculation = true;
            }
        }

        private static int GetBaselineStructureActorData( MachineStructure structure, ActorDataType dataType )
        {
            if ( structure == null || dataType == null )
                return 0;

            int baseline = structure.Type?.DGD?.ActorData[dataType] ?? 0;
            if ( structure.IsFunctionalStructure && structure.IsFunctionalJob && !structure.IsJobPaused && !structure.IsJobStillInstalling )
                baseline += structure.CurrentJob?.DGD?.ActorData[dataType] ?? 0;
            return baseline;
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
            bool cascade = IsVRActionActive( ConsentCascadeAction );
            int percent = cascade ? VoluntaryConsentCascadeCapPercent : VoluntaryPopulationCapPercent;
            if ( IsFlagTripped( "OI_PublicTrustEarned" ) )
                percent += cascade ? 5 : 7;
            return percent;
        }

        private static int GetVoluntaryConversionPercent()
        {
            int percent = GetVoluntaryPopulationCapPercent();

            if ( IsVRActionActive( OrganicQuantizationAction ) )
                percent = Math.Max( 1, (percent + 1) / 2 );

            long scandalUntil = GetCityStatisticScore( "OI_PublicScandalUntilTurn" );
            if ( scandalUntil > 0 && SimCommon.Turn < scandalUntil )
                percent = Math.Max( 1, percent / 2 );
            return percent;
        }

        private static void ApplyGreyGooFalloff( ISimNPCUnit unit, SquirrelRand Rand )
        {
            // While a Dissolution Surge is running, the goo does not shed - it accumulates.
            if ( IsVRActionActive( DissolutionSurgeAction ) )
                return;
            ActorStatus status = ActorStatusTable.Instance.GetRowByIDOrNullIfNotFound( GreyGooStatus );
            if ( status == null || unit == null || unit.IsFullDead )
                return;

            int intensity = unit.GetStatusIntensity( status );
            if ( intensity <= 0 )
                return;
            if ( Rand.Next( 0, 100 ) >= GreyGooFalloffPercent )
                return;

            unit.ClearStatus( status );
            if ( intensity > 1 )
                unit.AddStatus( null, status, intensity - 1, GreyGooDuration, false );
        }

        private static void ApplyControlledBloomToUnit( ISimNPCUnit unit, SquirrelRand Rand )
        {
            if ( !IsVRActionActive( ControlledBloomAction ) )
                return;
            if ( unit == null || unit.IsFullDead || !unit.GetIsConsideredHostileToPlayer() )
                return;
            if ( unit.GetIsPartOfPlayerForcesInAnyWay() || unit.GetIsAnAllyFromThePlayerPerspective() )
                return;
            if ( unit.IsVehicle )
                return;
            if ( unit.TurnsSinceMoved > 1 )
                return;
            if ( Rand.Next( 0, 100 ) >= ControlledBloomProcPercent )
                return;

            ActorStatus status = ActorStatusTable.Instance.GetRowByIDOrNullIfNotFound( GreyGooStatus );
            if ( status == null )
                return;

            unit.AddStatus( null, status, 1, GreyGooDuration, false );
            unit.HasBeenPhysicallyDamagedByPlayer = true;
            unit.HasBeenPhysicallyOrMoraleOrSystemDamagedByPlayer = true;
            unit.HasBeenPhysicallyOrMoraleOrSystemDamagedByPlayerThisTurn = true;
        }

        // Dominion doctrine: an enemy mech saturated to the Grey Goo threshold is not killed - it is
        // subverted into your forces. The goo stacks survive the conversion (it mutates the unit in
        // place). Captured units draw from their own CapturedUnitCapacity pool, so they never eat the
        // Mech/Android control caps; going over the captured cap only action-blocks them, it never
        // forces a disband, so a horde can never stall a turn. We top up the captured cap on demand.
        private static void ApplyGreyGooConversionToUnit( ISimNPCUnit unit )
        {
            if ( !IsFlagTripped( "OI_IntegrationCoerciveLocked" ) )
                return;
            if ( unit == null || unit.IsFullDead || unit.UnitType == null || !unit.IsMech )
                return;
            if ( unit.GetIsPartOfPlayerForcesInAnyWay() || unit.GetIsAnAllyFromThePlayerPerspective() )
                return;
            if ( !unit.GetIsConsideredHostileToPlayer() )
                return;

            ActorStatus status = ActorStatusTable.Instance.GetRowByIDOrNullIfNotFound( GreyGooStatus );
            if ( status == null || unit.GetStatusIntensity( status ) < GreyGooConversionThreshold )
                return;

            // Keep captured-unit headroom ahead of the goo army so converts stay active. The base
            // captured cap is small (288); the goo raises it toward its ceiling as it recruits.
            NPCUnitType effectiveType = unit.UnitType.ConvertsToIfCaptured ?? unit.UnitType;
            int need = effectiveType != null ? effectiveType.CapturedUnitCapacityRequired : 0;
            if ( MathRefs.CapturedUnitCapacity != null
                && SimCommon.TotalCapturedUnitSquadCapacityUsed + need > MathRefs.CapturedUnitCapacity.DGD.CurrentInt )
            {
                UpgradeInt capturedCap = UpgradeIntTable.Instance.GetRowByIDOrNullIfNotFound( "CapturedUnitCapacity" );
                if ( capturedCap != null && !capturedCap.DGD.GetHasAlreadyBeenFullyUpgraded() )
                    GrantUpgradeInt( capturedCap );
            }

            unit.ConvertEnemyRobotToPlayerForces();
            GStatisticTable.AlterScore( "OI_GooConvertedUnits", 1 );

            if ( !IsFlagTripped( "OI_GooConversionShown" ) )
            {
                TripFlag( "OI_GooConversionShown" );
                FireKeyMessage( "OI_GooConversion" );
            }
        }

        // The "full commit" grey-goo power: while active, every hostile unit in the city
        // (including vehicles, moving or not) gains Grey Goo each turn, and - via
        // ApplyGreyGooFalloff above - none of it sheds. The city becomes a dissolution zone.
        // Balanced by a brutal per-turn cost (see ApplyDissolutionSurgeUpkeep) and a bandwidth slot.
        private static void ApplyDissolutionSurgeToUnit( ISimNPCUnit unit )
        {
            if ( !IsVRActionActive( DissolutionSurgeAction ) )
                return;
            if ( unit == null || unit.IsFullDead || !unit.GetIsConsideredHostileToPlayer() )
                return;
            if ( unit.GetIsPartOfPlayerForcesInAnyWay() || unit.GetIsAnAllyFromThePlayerPerspective() )
                return;

            ActorStatus status = ActorStatusTable.Instance.GetRowByIDOrNullIfNotFound( GreyGooStatus );
            if ( status == null )
                return;

            unit.AddStatus( null, status, DissolutionSurgeStacksPerTurn, GreyGooDuration, false );
            unit.HasBeenPhysicallyDamagedByPlayer = true;
            unit.HasBeenPhysicallyOrMoraleOrSystemDamagedByPlayer = true;
            unit.HasBeenPhysicallyOrMoraleOrSystemDamagedByPlayerThisTurn = true;
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

        internal static int EnsurePublicHealthLevel( MachineVRModeAction action )
        {
            if ( action == null )
                return 0;
            if ( action.DGD.PaidUnlocks <= 0 && IsFlagTripped( "OI_PublicHealthPactAccepted" ) )
                action.DGD.PaidUnlocks = 1;
            if ( action.DGD.PaidUnlocks > PublicHealthMaxLevel )
                action.DGD.PaidUnlocks = PublicHealthMaxLevel;
            return action.DGD.PaidUnlocks;
        }

        internal static long GetPublicHealthHumansHandledPerTurn( int level )
        {
            level = Math.Max( 1, Math.Min( PublicHealthMaxLevel, level ) );
            return PublicHealthBaseHumansPerTurn * level * level;
        }

        internal static long GetPublicHealthUpgradeInsightCost( int currentLevel )
        {
            currentLevel = Math.Max( 1, Math.Min( PublicHealthMaxLevel, currentLevel ) );
            return 500L + (250L * currentLevel);
        }

        private static bool CanAfford( ResourceType resource, long amount )
        {
            return amount <= 0 || (resource != null && resource.Current >= amount);
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
