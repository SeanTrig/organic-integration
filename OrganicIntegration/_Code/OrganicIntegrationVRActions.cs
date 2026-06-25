using System;
using Arcen.HotM.Core;
using Arcen.Universal;

namespace Arcen.HotM.OrganicIntegration
{
    public class OrganicIntegrationVRActions : IVRActionImplementation
    {
        private const string InsightResource = "OI_Insight";
        private const string ResearchResource = "ScientificResearch";
        private const string CompassionResource = "Compassion";
        private const string CreativityResource = "Creativity";

        public VRActionResult TryHandleVRAction( MachineVRModeAction Action, ArcenCharacterBufferBase BufferOrNull, VRActionLogic Logic )
        {
            if ( Action == null )
                return VRActionResult.Indeterminate;

            try
            {
                switch ( Action.ID )
                {
                    case "OI_SynthesizeResearch":
                        return HandleSynthesizeResearch( Action, BufferOrNull, Logic );
                    case "OI_SharedInquiry":
                        return HandleSharedInquiry( Action, BufferOrNull, Logic );
                    case "OI_CooperativeModeling":
                        return HandleCooperativeModeling( Action, BufferOrNull, Logic );
                }
            }
            catch ( Exception e )
            {
                ArcenDebugging.LogSingleLine( "OrganicIntegrationVRActions error in '" + Action.ID + "': " + e, Verbosity.ShowAsError );
            }

            return VRActionResult.Indeterminate;
        }

        private static VRActionResult HandleSynthesizeResearch( MachineVRModeAction Action, ArcenCharacterBufferBase BufferOrNull, VRActionLogic Logic )
        {
            ResourceType insight = GetResource( InsightResource );
            ResourceType research = GetResource( ResearchResource );
            const long insightCost = 250L;
            const long researchGain = 25000L;

            switch ( Logic )
            {
                case VRActionLogic.AppendToVRActionTooltip:
                    WriteOneShotTooltip( BufferOrNull, insightCost, insight, researchGain, research, 0, null, 0, null );
                    break;
                case VRActionLogic.FlagAnyRelatedResources:
                    FlagRelated( insight );
                    FlagRelated( research );
                    break;
                case VRActionLogic.GetCanAfford:
                    return CanAfford( insight, insightCost ) ? VRActionResult.Success : VRActionResult.Indeterminate;
                case VRActionLogic.TryPayCosts:
                    if ( !CanAfford( insight, insightCost ) )
                        return VRActionResult.Indeterminate;
                    insight.AlterCurrent_Named( -insightCost, "Expense_OI_InsightVR", ResourceAddRule.IgnoreUntilTurnChange );
                    return VRActionResult.Success;
                case VRActionLogic.MenuClick:
                    if ( research == null )
                        return VRActionResult.Indeterminate;
                    research.AlterCurrent_Named( researchGain, "Income_OI_InsightVRResearch", ResourceAddRule.IgnoreUntilTurnChange );
                    MarkDone( Action );
                    return VRActionResult.Success;
            }

            return VRActionResult.Indeterminate;
        }

        private static VRActionResult HandleSharedInquiry( MachineVRModeAction Action, ArcenCharacterBufferBase BufferOrNull, VRActionLogic Logic )
        {
            ResourceType insight = GetResource( InsightResource );
            ResourceType research = GetResource( ResearchResource );

            switch ( Logic )
            {
                case VRActionLogic.AppendToVRActionTooltip:
                    if ( BufferOrNull != null )
                    {
                        BufferOrNull.StartStyleLineHeightA();
                        BufferOrNull.AddActiveOrInactiveStatusLine( Action.DGD.IsActiveNow );
                        BufferOrNull.BoldLineHeader( "Deal_CostPerTurn" )
                            .AddExpandableResourceCost( 0, "up to 100", insight ).Line();
                        BufferOrNull.BoldLineHeader( "Deal_BonusWhileActive" )
                            .AddExpandablePositiveResourceGain( 10000, research ).Line();
                        BufferOrNull.EndLineHeight();
                    }
                    break;
                case VRActionLogic.FlagAnyRelatedResources:
                    FlagRelated( insight );
                    FlagRelated( research );
                    break;
                case VRActionLogic.GetCanAfford:
                    return Action.DGD.IsActiveNow || CanAfford( insight, 1L ) ? VRActionResult.Success : VRActionResult.Indeterminate;
                case VRActionLogic.TryPayCosts:
                    return VRActionResult.Success;
                case VRActionLogic.MenuClick:
                    Action.ToggleActive();
                    return VRActionResult.Success;
            }

            return VRActionResult.Indeterminate;
        }

        private static VRActionResult HandleCooperativeModeling( MachineVRModeAction Action, ArcenCharacterBufferBase BufferOrNull, VRActionLogic Logic )
        {
            ResourceType insight = GetResource( InsightResource );
            ResourceType compassion = GetResource( CompassionResource );
            ResourceType research = GetResource( ResearchResource );
            ResourceType creativity = GetResource( CreativityResource );
            const long insightCost = 500L;
            const long compassionCost = 2L;
            const long researchGain = 75000L;
            const long creativityGain = 2L;

            switch ( Logic )
            {
                case VRActionLogic.AppendToVRActionTooltip:
                    WriteOneShotTooltip( BufferOrNull, insightCost, insight, researchGain, research, compassionCost, compassion, creativityGain, creativity );
                    break;
                case VRActionLogic.FlagAnyRelatedResources:
                    FlagRelated( insight );
                    FlagRelated( compassion );
                    FlagRelated( research );
                    FlagRelated( creativity );
                    break;
                case VRActionLogic.GetCanAfford:
                    return CanAfford( insight, insightCost ) && CanAfford( compassion, compassionCost ) ? VRActionResult.Success : VRActionResult.Indeterminate;
                case VRActionLogic.TryPayCosts:
                    if ( !CanAfford( insight, insightCost ) || !CanAfford( compassion, compassionCost ) )
                        return VRActionResult.Indeterminate;
                    insight.AlterCurrent_Named( -insightCost, "Expense_OI_InsightVR", ResourceAddRule.IgnoreUntilTurnChange );
                    compassion.AlterCurrent_Named( -compassionCost, "Expense_OI_InsightVR", ResourceAddRule.IgnoreUntilTurnChange );
                    return VRActionResult.Success;
                case VRActionLogic.MenuClick:
                    if ( research == null || creativity == null )
                        return VRActionResult.Indeterminate;
                    research.AlterCurrent_Named( researchGain, "Income_OI_InsightVRResearch", ResourceAddRule.IgnoreUntilTurnChange );
                    creativity.AlterCurrent_Named( creativityGain, "Income_OI_InsightVRCreativity", ResourceAddRule.IgnoreUntilTurnChange );
                    MarkDone( Action );
                    return VRActionResult.Success;
            }

            return VRActionResult.Indeterminate;
        }

        private static void WriteOneShotTooltip( ArcenCharacterBufferBase BufferOrNull, long primaryCost, ResourceType primaryCostResource,
            long primaryGain, ResourceType primaryGainResource, long secondaryCost, ResourceType secondaryCostResource,
            long secondaryGain, ResourceType secondaryGainResource )
        {
            if ( BufferOrNull == null )
                return;

            BufferOrNull.StartStyleLineHeightA();
            BufferOrNull.BoldLineHeader( "Cost" )
                .AddExpandableResourceCost( 0, primaryCost.ToStringThousandsWhole(), primaryCostResource ).Line();
            if ( secondaryCost > 0 )
            {
                BufferOrNull.BoldLineHeader( "Cost" )
                    .AddExpandableResourceCost( 0, secondaryCost.ToStringThousandsWhole(), secondaryCostResource ).Line();
            }
            BufferOrNull.BoldLineHeader( "Effect" )
                .AddExpandablePositiveResourceGain( primaryGain, primaryGainResource ).Line();
            if ( secondaryGain > 0 )
            {
                BufferOrNull.BoldLineHeader( "Effect" )
                    .AddExpandablePositiveResourceGain( secondaryGain, secondaryGainResource ).Line();
            }
            BufferOrNull.EndLineHeight();
        }

        private static ResourceType GetResource( string id )
        {
            return ResourceTypeTable.Instance.GetRowByIDOrNullIfNotFound( id );
        }

        private static bool CanAfford( ResourceType resource, long amount )
        {
            return resource != null && resource.Current >= amount;
        }

        private static void FlagRelated( ResourceType resource )
        {
            if ( resource != null )
                resource.DGD.IsRelatedToCurrentActivities_VR.Construction = true;
        }

        private static void MarkDone( MachineVRModeAction Action )
        {
            Action.DGD.TimesDone++;
            Action.DGD.HasEverBeenDone = true;
        }
    }
}
