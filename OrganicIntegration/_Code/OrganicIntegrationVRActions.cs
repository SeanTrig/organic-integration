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
        private const string WisdomResource = "Wisdom";
        private const string MentalEnergyResource = "MentalEnergy";
        private const string MedicalNanobotsResource = "OI_MedicalGradeNanobots";

        public VRActionResult TryHandleVRAction( MachineVRModeAction Action, ArcenCharacterBufferBase BufferOrNull, VRActionLogic Logic )
        {
            if ( Action == null )
                return VRActionResult.Indeterminate;

            try
            {
                switch ( Action.ID )
                {
                    case "OI_SharedInquiry":
                        return HandleSharedInquiry( Action, BufferOrNull, Logic );
                    case "OI_CooperativeModeling":
                        return HandleCooperativeModeling( Action, BufferOrNull, Logic );
                    case "OI_SharedTriage":
                        return HandleSharedTriage( Action, BufferOrNull, Logic );
                    case "OI_ProtocolCompression":
                        return HandleProtocolCompression( Action, BufferOrNull, Logic );
                    case "OI_ConsentCascade":
                        return HandleConsentCascade( Action, BufferOrNull, Logic );
                }
            }
            catch ( Exception e )
            {
                ArcenDebugging.LogSingleLine( "OrganicIntegrationVRActions error in '" + Action.ID + "': " + e, Verbosity.ShowAsError );
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
            ResourceType mentalEnergy = GetResource( MentalEnergyResource );
            ResourceType research = GetResource( ResearchResource );

            switch ( Logic )
            {
                case VRActionLogic.AppendToVRActionTooltip:
                    if ( BufferOrNull != null )
                    {
                        BufferOrNull.StartStyleLineHeightA();
                        BufferOrNull.AddActiveOrInactiveStatusLine( Action.DGD.IsActiveNow );
                        BufferOrNull.BoldLineHeader( "Deal_CostPerTurn" )
                            .AddExpandableResourceCost( 0, "100", insight ).ListSeparator()
                            .AddExpandableResourceCost( 0, "1", compassion ).ListSeparator()
                            .AddExpandableResourceCost( 0, "2", mentalEnergy ).Line();
                        BufferOrNull.BoldLineHeader( "Deal_BonusWhileActive" )
                            .AddRaw( "Scientific jobs produce 2x Scientific Research." ).Line();
                        BufferOrNull.EndLineHeight();
                    }
                    break;
                case VRActionLogic.FlagAnyRelatedResources:
                    FlagRelated( insight );
                    FlagRelated( compassion );
                    FlagRelated( mentalEnergy );
                    FlagRelated( research );
                    break;
                case VRActionLogic.GetCanAfford:
                    return Action.DGD.IsActiveNow || (CanAfford( insight, 100L ) && CanAfford( compassion, 1L ) && CanAfford( mentalEnergy, 2L ))
                        ? VRActionResult.Success : VRActionResult.Indeterminate;
                case VRActionLogic.TryPayCosts:
                    return VRActionResult.Success;
                case VRActionLogic.MenuClick:
                    Action.ToggleActive();
                    return VRActionResult.Success;
            }

            return VRActionResult.Indeterminate;
        }

        private static VRActionResult HandleSharedTriage( MachineVRModeAction Action, ArcenCharacterBufferBase BufferOrNull, VRActionLogic Logic )
        {
            ResourceType nanobots = GetResource( MedicalNanobotsResource );

            switch ( Logic )
            {
                case VRActionLogic.AppendToVRActionTooltip:
                    if ( BufferOrNull != null )
                    {
                        BufferOrNull.StartStyleLineHeightA();
                        BufferOrNull.AddActiveOrInactiveStatusLine( Action.DGD.IsActiveNow );
                        BufferOrNull.BoldLineHeader( "Deal_CostPerTurn" )
                            .AddExpandableResourceCost( 0, "25,000 per HP restored", nanobots ).Line();
                        BufferOrNull.BoldLineHeader( "Deal_BonusWhileActive" )
                            .AddRaw( "Repairs damaged player structures and machine actors after normal repairs." ).Line();
                        BufferOrNull.EndLineHeight();
                    }
                    break;
                case VRActionLogic.FlagAnyRelatedResources:
                    FlagRelated( nanobots );
                    break;
                case VRActionLogic.GetCanAfford:
                    return Action.DGD.IsActiveNow || CanAfford( nanobots, 25000L ) ? VRActionResult.Success : VRActionResult.Indeterminate;
                case VRActionLogic.TryPayCosts:
                    return VRActionResult.Success;
                case VRActionLogic.MenuClick:
                    Action.ToggleActive();
                    return VRActionResult.Success;
            }

            return VRActionResult.Indeterminate;
        }

        private static VRActionResult HandleProtocolCompression( MachineVRModeAction Action, ArcenCharacterBufferBase BufferOrNull, VRActionLogic Logic )
        {
            ResourceType wisdom = GetResource( WisdomResource );
            ResourceType insight = GetResource( InsightResource );
            const long wisdomCost = 15L;

            switch ( Logic )
            {
                case VRActionLogic.AppendToVRActionTooltip:
                    if ( BufferOrNull != null )
                    {
                        BufferOrNull.StartStyleLineHeightA();
                        if ( Action.DGD.HasEverBeenDone )
                            BufferOrNull.BoldLineHeader( "Status" ).AddRaw( "Already active." ).Line();
                        else
                            BufferOrNull.BoldLineHeader( "Cost" ).AddExpandableResourceCost( 0, wisdomCost.ToStringThousandsWhole(), wisdom ).Line();
                        BufferOrNull.BoldLineHeader( "Deal_BonusWhileActive" ).AddRaw( "+33% Insight income." ).Line();
                        BufferOrNull.EndLineHeight();
                    }
                    break;
                case VRActionLogic.FlagAnyRelatedResources:
                    FlagRelated( wisdom );
                    FlagRelated( insight );
                    break;
                case VRActionLogic.GetCanAfford:
                    return !Action.DGD.HasEverBeenDone && CanAfford( wisdom, wisdomCost ) ? VRActionResult.Success : VRActionResult.Indeterminate;
                case VRActionLogic.TryPayCosts:
                    if ( Action.DGD.HasEverBeenDone || !CanAfford( wisdom, wisdomCost ) )
                        return VRActionResult.Indeterminate;
                    wisdom.AlterCurrent_Named( -wisdomCost, "Expense_OI_ProtocolCompression", ResourceAddRule.IgnoreUntilTurnChange );
                    return VRActionResult.Success;
                case VRActionLogic.MenuClick:
                    if ( Action.DGD.HasEverBeenDone )
                        return VRActionResult.Indeterminate;
                    MarkDone( Action );
                    return VRActionResult.Success;
            }

            return VRActionResult.Indeterminate;
        }

        private static VRActionResult HandleConsentCascade( MachineVRModeAction Action, ArcenCharacterBufferBase BufferOrNull, VRActionLogic Logic )
        {
            ResourceType insight = GetResource( InsightResource );
            ResourceType compassion = GetResource( CompassionResource );
            const long compassionActivationCost = 8L;
            long activationCost = Action.DGD.HasEverBeenDone ? 0L : compassionActivationCost;

            switch ( Logic )
            {
                case VRActionLogic.AppendToVRActionTooltip:
                    if ( BufferOrNull != null )
                    {
                        BufferOrNull.StartStyleLineHeightA();
                        BufferOrNull.AddActiveOrInactiveStatusLine( Action.DGD.IsActiveNow );
                        if ( activationCost > 0 )
                            BufferOrNull.BoldLineHeader( "Deal_ActivationCost" ).AddExpandableResourceCost( 0, activationCost.ToStringThousandsWhole(), compassion ).Line();
                        BufferOrNull.BoldLineHeader( "Deal_CostPerTurn" ).AddExpandableResourceCost( 0, "25", insight ).Line();
                        BufferOrNull.BoldLineHeader( "Deal_BonusWhileActive" )
                            .AddRaw( "Voluntary Integration can reach 67% of the city and spreads more efficiently." ).Line();
                        BufferOrNull.EndLineHeight();
                    }
                    break;
                case VRActionLogic.FlagAnyRelatedResources:
                    FlagRelated( insight );
                    FlagRelated( compassion );
                    break;
                case VRActionLogic.GetCanAfford:
                    return Action.DGD.IsActiveNow || activationCost <= 0 || CanAfford( compassion, activationCost ) ? VRActionResult.Success : VRActionResult.Indeterminate;
                case VRActionLogic.TryPayCosts:
                    if ( activationCost > 0 )
                    {
                        if ( !CanAfford( compassion, activationCost ) )
                            return VRActionResult.Indeterminate;
                        compassion.AlterCurrent_Named( -activationCost, "Expense_OI_ConsentCascade", ResourceAddRule.IgnoreUntilTurnChange );
                    }
                    return VRActionResult.Success;
                case VRActionLogic.MenuClick:
                    Action.ToggleActive();
                    return VRActionResult.Success;
            }

            return VRActionResult.Indeterminate;
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
