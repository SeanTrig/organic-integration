using System;
using Arcen.HotM.Core;
using Arcen.HotM.ExternalVis;
using Arcen.HotM.Visualization;
using Arcen.Universal;

namespace Arcen.HotM.OrganicIntegration
{
    public class OrganicIntegrationProjects : IProjectHandlerImplementation
    {
        public bool HandleLogicForProjectOutcome( ProjectLogic Logic, ArcenCharacterBufferBase BufferOrNull, MachineProject Project, ProjectOutcome OutcomeOrNoneYet,
            SquirrelRand RandOrNull, out bool CanBeCompletedNow )
        {
            CanBeCompletedNow = false;
            if ( Project == null || OutcomeOrNoneYet == null )
                return false;

            try
            {
                switch ( Project.ID )
                {
                    case "OI_ResearchingBCIs":
                    case "OI_MicrobotInterfaceTrials":
                    case "OI_NanobotMiniaturization":
                        ProjectHelper.HandleScienceWork2X( Logic, OutcomeOrNoneYet,
                            OutcomeOrNoneYet.GetSingleIntByID( "NeurologyGoal", 100 ),
                            OutcomeOrNoneYet.GetSingleIntByID( "BionicsGoal", 100 ),
                            MathRefs.NeurologyResearch, MathRefs.BionicsEngineeringWork,
                            ResourceRefs.Neurologists, ResourceRefs.BionicsEngineers,
                            BufferOrNull, ref CanBeCompletedNow, RandOrNull );
                        break;

                    case "OI_InterfaceStressSurvey":
                        HandleStatisticProgress( Logic, OutcomeOrNoneYet, "OI_InterfaceTrialDesignPoints",
                            OutcomeOrNoneYet.GetSingleIntByID( "DesignPointGoal", 24 ), BufferOrNull, ref CanBeCompletedNow );
                        break;

                    case "OI_FieldNanotechHarvest":
                        HandleStatisticProgress( Logic, OutcomeOrNoneYet, "OI_NanobotHarvestDesignPoints",
                            OutcomeOrNoneYet.GetSingleIntByID( "DesignPointGoal", 32 ), BufferOrNull, ref CanBeCompletedNow );
                        break;

                    case "OI_DesignHumanCompatibleNeuroweave":
                    case "OI_ReplicativeSafeguards":
                        ProjectHelper.HandleScienceWork3X( Logic, OutcomeOrNoneYet,
                            OutcomeOrNoneYet.GetSingleIntByID( "NeurologyGoal", 100 ),
                            OutcomeOrNoneYet.GetSingleIntByID( "BionicsGoal", 100 ),
                            OutcomeOrNoneYet.GetSingleIntByID( "MedicalGoal", 100 ),
                            MathRefs.NeurologyResearch, MathRefs.BionicsEngineeringWork, MathRefs.MedicalResearch,
                            ResourceRefs.Neurologists, ResourceRefs.BionicsEngineers, ResourceRefs.Physicians,
                            BufferOrNull, ref CanBeCompletedNow, RandOrNull );
                        break;

                    case "OI_InsightSharedQuestions":
                    case "OI_InsightNetworkedCognition":
                    case "OI_InsightDistributedTriage":
                        HandleInsightEarnedGate( Logic, OutcomeOrNoneYet, BufferOrNull, ref CanBeCompletedNow );
                        break;

                    default:
                        if ( !Project.HasShownHandlerMissingError )
                        {
                            Project.HasShownHandlerMissingError = true;
                            ArcenDebugging.LogSingleLine( "OrganicIntegrationProjects: No handler for '" + Project.ID + "'.", Verbosity.ShowAsError );
                        }
                        break;
                }
            }
            catch ( Exception e )
            {
                if ( !Project.HasShownCaughtError )
                {
                    Project.HasShownCaughtError = true;
                    ArcenDebugging.LogSingleLine( "OrganicIntegrationProjects error in '" + Project.ID + "': " + e, Verbosity.ShowAsError );
                }
            }

            return false;
        }

        private static void HandleStatisticProgress( ProjectLogic Logic, ProjectOutcome Outcome, string StatisticID, int Target, ArcenCharacterBufferBase BufferOrNull, ref bool CanBeCompletedNow )
        {
            GStatisticBase statistic = GStatisticTable.Instance.GetRowByID( StatisticID );
            long current = statistic?.DGD?.GetScore() ?? 0;
            CanBeCompletedNow = current >= Target;

            switch ( Logic )
            {
                case ProjectLogic.WriteProgressIconText:
                case ProjectLogic.WriteProgressTextBrief:
                    ProjectHelper.WritePercentageFromTwoNumbers( Logic, Outcome, current, Target, BufferOrNull );
                    break;
                case ProjectLogic.WriteRequirements_OneLine:
                case ProjectLogic.WriteRequirements_ManyLines:
                    if ( BufferOrNull != null )
                        BufferOrNull.AddFormat3( "RequiredResourceAmount", current.ToStringThousandsWhole(), Target.ToStringThousandsWhole(), statistic?.GetDisplayName() ?? StatisticID ).Line();
                    break;
                case ProjectLogic.WriteAddedContext:
                case ProjectLogic.DoAfterCompletion:
                    break;
            }
        }

        private static void HandleInsightEarnedGate( ProjectLogic Logic, ProjectOutcome Outcome, ArcenCharacterBufferBase BufferOrNull, ref bool CanBeCompletedNow )
        {
            GStatisticBase statistic = GStatisticTable.Instance.GetRowByID( "OI_TotalInsightGenerated" );
            int target = Outcome.GetSingleIntByID( "InsightGeneratedGoal", 100 );
            long current = statistic?.DGD?.GetScore() ?? 0;

            CanBeCompletedNow = current >= target;

            switch ( Logic )
            {
                case ProjectLogic.WriteProgressIconText:
                case ProjectLogic.WriteProgressTextBrief:
                    if ( BufferOrNull != null )
                        BufferOrNull.AddRaw( MathA.Min( 100, (int)((current * 100) / MathA.Max( 1, target )) ).ToString() + "%" );
                    break;
                case ProjectLogic.WriteRequirements_OneLine:
                case ProjectLogic.WriteRequirements_ManyLines:
                    if ( BufferOrNull != null )
                        BufferOrNull.AddFormat3( "RequiredResourceAmount", current.ToStringThousandsWhole(), target.ToStringThousandsWhole(), statistic?.GetDisplayName() ?? "Total Insight Generated" ).Line();
                    break;
                case ProjectLogic.WriteAddedContext:
                case ProjectLogic.DoAfterCompletion:
                    break;
            }
        }
        public void HandleStreetItem( ProjectOutcome Outcome, ProjectOutcomeStreetSenseItem StreetItem, BaseBuilding Building, ISimMachineActor Actor, SquirrelRand Rand, ArcenDoubleCharacterBuffer PopupBufferOrNull )
        {
        }

        public void HandleTaskBoxTwo_Text( MachineProject Project, int OtherTextIndex, bool IsBeingHovered, ArcenDoubleCharacterBuffer Buffer, out AlertColor Alert )
        {
            Alert = AlertColor.Low;
        }

        public void HandleTaskBoxTwo_Tooltip( MachineProject Project, int OtherTextIndex )
        {
        }

        public void HandleTaskBoxTwo_Click( MachineProject Project, int OtherTextIndex, MouseHandlingInput input )
        {
        }

        public void DoOnGameClear( bool AlsoClearMetaData )
        {
        }
    }
}

