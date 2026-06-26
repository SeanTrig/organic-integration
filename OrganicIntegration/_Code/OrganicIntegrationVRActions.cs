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
        private const string AbandonedHumansResource = "AbandonedHumans";
        private const string HydroponicGreensResource = "HydroponicGreens";
        private const string VatGrownMeatResource = "VatGrownMeat";
        private const string FilteredWaterResource = "FilteredWater";

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
                    case "OI_OrganicQuantization":
                        return HandleOrganicQuantization( Action, BufferOrNull, Logic );
                    case "OI_ConsentCascade":
                        return HandleConsentCascade( Action, BufferOrNull, Logic );
                    case "OI_CivicSensorium":
                        return HandleCivicSensorium( Action, BufferOrNull, Logic );
                    case "OI_PublicHealthMesh":
                        return HandlePublicHealthMesh( Action, BufferOrNull, Logic );
                    case "OI_ExpandHealthPact":
                        return HandleExpandHealthPact( Action, BufferOrNull, Logic );
                    case "OI_ShelterFilaments":
                        return HandleMaintainedToggle( Action, BufferOrNull, Logic,
                            "Moves up to 1,000 Abandoned Humans into filament shelters each turn, adding them to the city population.",
                            Cost( InsightResource, 250L ), Cost( MedicalNanobotsResource, 10000000L ) );
                    case "OI_InfrastructureFilaments":
                        return HandleMaintainedToggle( Action, BufferOrNull, Logic,
                            "Siphons trace resources from city infrastructure traffic, producing Wealth, Neodymium, and Scandium each turn.",
                            Cost( InsightResource, 300L ), Cost( MedicalNanobotsResource, 20000000L ) );
                    case "OI_ArchitecturalWeave":
                        return HandleArchitecturalWeave( Action, BufferOrNull, Logic );
                    case "OI_ControlledBloom":
                        return HandleMaintainedToggle( Action, BufferOrNull, Logic,
                            "Hostile infantry and mechs that move through the city have a 33% chance to gain 1 Grey Goo. Vehicles are unaffected.",
                            Cost( InsightResource, 400L ), Cost( MedicalNanobotsResource, 30000000L ), Cost( CompassionResource, 1L ) );
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
                            .AddExpandableResourceCost( 0, "300", insight ).ListSeparator()
                            .AddExpandableResourceCost( 0, "1", mentalEnergy ).Line();
                        BufferOrNull.BoldLineHeader( "Deal_BonusWhileActive" )
                            .AddRaw( "Gains 100 to 15,000 Scientific Research per turn, scaling with Upgraded Humans." ).Line();
                        BufferOrNull.EndLineHeight();
                    }
                    break;
                case VRActionLogic.FlagAnyRelatedResources:
                    FlagRelated( insight );
                    FlagRelated( mentalEnergy );
                    FlagRelated( research );
                    break;
                case VRActionLogic.GetCanAfford:
                    return Action.DGD.IsActiveNow || (CanAfford( insight, 300L ) && CanAfford( mentalEnergy, 1L )) ? VRActionResult.Success : VRActionResult.Indeterminate;
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
            ResourceType insight = GetResource( InsightResource );
            ResourceType nanobots = GetResource( MedicalNanobotsResource );

            switch ( Logic )
            {
                case VRActionLogic.AppendToVRActionTooltip:
                    if ( BufferOrNull != null )
                    {
                        BufferOrNull.StartStyleLineHeightA();
                        BufferOrNull.AddActiveOrInactiveStatusLine( Action.DGD.IsActiveNow );
                        BufferOrNull.BoldLineHeader( "Deal_CostPerTurn" )
                            .AddExpandableResourceCost( 0, "250", insight ).ListSeparator()
                            .AddExpandableResourceCost( 0, "25,000 per HP restored", nanobots ).Line();
                        BufferOrNull.BoldLineHeader( "Deal_BonusWhileActive" )
                            .AddRaw( "Repairs damaged player structures and machine actors after normal repairs. Medical-Grade Nanobots are spent only when HP is restored." ).Line();
                        BufferOrNull.EndLineHeight();
                    }
                    break;
                case VRActionLogic.FlagAnyRelatedResources:
                    FlagRelated( insight );
                    FlagRelated( nanobots );
                    break;
                case VRActionLogic.GetCanAfford:
                    return Action.DGD.IsActiveNow || CanAfford( insight, 250L ) ? VRActionResult.Success : VRActionResult.Indeterminate;
                case VRActionLogic.TryPayCosts:
                    return VRActionResult.Success;
                case VRActionLogic.MenuClick:
                    Action.ToggleActive();
                    return VRActionResult.Success;
            }

            return VRActionResult.Indeterminate;
        }

        private static VRActionResult HandleOrganicQuantization( MachineVRModeAction Action, ArcenCharacterBufferBase BufferOrNull, VRActionLogic Logic )
        {
            ResourceType wisdom = GetResource( WisdomResource );
            ResourceType insight = GetResource( InsightResource );
            ResourceType mentalEnergy = GetResource( MentalEnergyResource );
            const long wisdomCost = 15L;
            const long insightCost = 300L;
            const long mentalEnergyCost = 1L;

            switch ( Logic )
            {
                case VRActionLogic.AppendToVRActionTooltip:
                    if ( BufferOrNull != null )
                    {
                        BufferOrNull.StartStyleLineHeightA();
                        BufferOrNull.AddActiveOrInactiveStatusLine( Action.DGD.IsActiveNow );
                        if ( !Action.DGD.HasEverBeenDone )
                            BufferOrNull.BoldLineHeader( "Deal_ActivationCost" ).AddExpandableResourceCost( 0, wisdomCost.ToStringThousandsWhole(), wisdom ).Line();
                        BufferOrNull.BoldLineHeader( "Deal_CostPerTurn" )
                            .AddExpandableResourceCost( 0, insightCost.ToStringThousandsWhole(), insight ).ListSeparator()
                            .AddExpandableResourceCost( 0, mentalEnergyCost.ToStringThousandsWhole(), mentalEnergy ).Line();
                        BufferOrNull.BoldLineHeader( "Deal_BonusWhileActive" ).AddRaw( "+50% Insight income; voluntary Upgraded Human growth is halved while active." ).Line();
                        BufferOrNull.EndLineHeight();
                    }
                    break;
                case VRActionLogic.FlagAnyRelatedResources:
                    FlagRelated( wisdom );
                    FlagRelated( insight );
                    FlagRelated( mentalEnergy );
                    break;
                case VRActionLogic.GetCanAfford:
                    return Action.DGD.IsActiveNow || ((Action.DGD.HasEverBeenDone || CanAfford( wisdom, wisdomCost )) && CanAfford( insight, insightCost ) && CanAfford( mentalEnergy, mentalEnergyCost ))
                        ? VRActionResult.Success : VRActionResult.Indeterminate;
                case VRActionLogic.TryPayCosts:
                    if ( Action.DGD.IsActiveNow )
                        return VRActionResult.Success;
                    if ( !Action.DGD.HasEverBeenDone )
                    {
                        if ( !CanAfford( wisdom, wisdomCost ) )
                            return VRActionResult.Indeterminate;
                        wisdom.AlterCurrent_Named( -wisdomCost, "Expense_OI_OrganicQuantization", ResourceAddRule.IgnoreUntilTurnChange );
                    }
                    return VRActionResult.Success;
                case VRActionLogic.MenuClick:
                    if ( !Action.DGD.HasEverBeenDone )
                        MarkDone( Action );
                    Action.ToggleActive();
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
                        BufferOrNull.BoldLineHeader( "Deal_CostPerTurn" )
                            .AddExpandableResourceCost( 0, "300", insight ).ListSeparator()
                            .AddExpandableResourceCost( 0, "1", compassion ).Line();
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
                    return Action.DGD.IsActiveNow || ((activationCost <= 0 || CanAfford( compassion, activationCost )) && CanAfford( insight, 300L ) && CanAfford( compassion, 1L ))
                        ? VRActionResult.Success : VRActionResult.Indeterminate;
                case VRActionLogic.TryPayCosts:
                    if ( Action.DGD.IsActiveNow )
                        return VRActionResult.Success;
                    if ( activationCost > 0 )
                    {
                        if ( !CanAfford( compassion, activationCost ) )
                            return VRActionResult.Indeterminate;
                        compassion.AlterCurrent_Named( -activationCost, "Expense_OI_ConsentCascade", ResourceAddRule.IgnoreUntilTurnChange );
                    }
                    return VRActionResult.Success;
                case VRActionLogic.MenuClick:
                    if ( !Action.DGD.HasEverBeenDone )
                        MarkDone( Action );
                    Action.ToggleActive();
                    return VRActionResult.Success;
            }

            return VRActionResult.Indeterminate;
        }

        private static VRActionResult HandleCivicSensorium( MachineVRModeAction Action, ArcenCharacterBufferBase BufferOrNull, VRActionLogic Logic )
        {
            ResourceType insight = GetResource( InsightResource );
            ResourceType mentalEnergy = GetResource( MentalEnergyResource );
            int level = OrganicIntegrationCalculators.GetCivicSensoriumLevel( Action );
            bool isComplete = level >= OrganicIntegrationCalculators.CivicSensoriumMaxLevel;
            long goal = OrganicIntegrationCalculators.GetCivicSensoriumInsightForNextUpgrade( Action );
            int currentBonus = OrganicIntegrationCalculators.GetCivicSensoriumScanRangeBonus( Action );
            int nextLevel = Math.Min( OrganicIntegrationCalculators.CivicSensoriumMaxLevel, level + 1 );
            int nextBonus = nextLevel * OrganicIntegrationCalculators.CivicSensoriumScanRangePerLevel;
            int maxBonus = OrganicIntegrationCalculators.CivicSensoriumMaxLevel * OrganicIntegrationCalculators.CivicSensoriumScanRangePerLevel;

            switch ( Logic )
            {
                case VRActionLogic.AppendToVRActionTooltip:
                    if ( BufferOrNull != null )
                    {
                        BufferOrNull.StartStyleLineHeightA();
                        BufferOrNull.AddActiveOrInactiveOrCompleteStatusLine( Action.DGD.IsActiveNow, isComplete );
                        BufferOrNull.BoldLineHeader( "DealLevel_CurrentLevel" ).AddRaw( level.ToStringThousandsWhole() + " / " + OrganicIntegrationCalculators.CivicSensoriumMaxLevel.ToStringThousandsWhole() ).Line();
                        if ( !isComplete )
                        {
                            BufferOrNull.BoldLineHeader( "Deal_CostPerTurn" )
                                .AddExpandableResourceCost( 0, "250", insight ).ListSeparator()
                                .AddExpandableResourceCost( 0, "1", mentalEnergy ).Line();
                            BufferOrNull.AddInvestmentTowardGoalLine( Action.DGD.UpgradePoints, goal, insight );
                            BufferOrNull.BoldLineHeader( "Deal_BonusOnTrigger" ).AddRaw( "Raises Scanner structure range bonus from +" + currentBonus.ToStringThousandsWhole() + " to +" + nextBonus.ToStringThousandsWhole() + "." ).Line();
                        }
                        BufferOrNull.BoldLineHeader( "Deal_BonusWhileActive" ).AddRaw( "Completed sensorium levels currently grant +" + currentBonus.ToStringThousandsWhole() + " Scan Range to existing Scanner structures. Maximum: +" + maxBonus.ToStringThousandsWhole() + "." ).Line();
                        BufferOrNull.EndLineHeight();
                    }
                    break;
                case VRActionLogic.FlagAnyRelatedResources:
                    FlagRelated( insight );
                    FlagRelated( mentalEnergy );
                    break;
                case VRActionLogic.GetCanAfford:
                    return Action.DGD.IsActiveNow || (!isComplete && CanAfford( insight, 250L ) && CanAfford( mentalEnergy, 1L ))
                        ? VRActionResult.Success : VRActionResult.Indeterminate;
                case VRActionLogic.TryPayCosts:
                    return VRActionResult.Success;
                case VRActionLogic.MenuClick:
                    if ( isComplete )
                        return VRActionResult.Indeterminate;
                    Action.ToggleActive();
                    return VRActionResult.Success;
            }

            return VRActionResult.Indeterminate;
        }

        private static VRActionResult HandlePublicHealthMesh( MachineVRModeAction Action, ArcenCharacterBufferBase BufferOrNull, VRActionLogic Logic )
        {
            int level = OrganicIntegrationCalculators.EnsurePublicHealthLevel( Action );
            if ( level <= 0 )
                level = 1;

            long humans = OrganicIntegrationCalculators.GetPublicHealthHumansHandledPerTurn( level );
            ResourceType abandoned = GetResource( AbandonedHumansResource );
            ResourceType nanobots = GetResource( MedicalNanobotsResource );
            ResourceType greens = GetResource( HydroponicGreensResource );
            ResourceType meat = GetResource( VatGrownMeatResource );
            ResourceType water = GetResource( FilteredWaterResource );

            switch ( Logic )
            {
                case VRActionLogic.AppendToVRActionTooltip:
                    if ( BufferOrNull != null )
                    {
                        BufferOrNull.StartStyleLineHeightA();
                        BufferOrNull.AddActiveOrInactiveStatusLine( Action.DGD.IsActiveNow );
                        BufferOrNull.BoldLineHeader( "DealLevel_CurrentLevel" ).AddRaw( level.ToStringThousandsWhole() + " / " + OrganicIntegrationCalculators.PublicHealthMaxLevel.ToStringThousandsWhole() ).Line();
                        BufferOrNull.BoldLineHeader( "Deal_CostPerTurn" )
                            .AddExpandableResourceCost( 0, "up to " + humans.ToStringThousandsWhole(), abandoned ).ListSeparator()
                            .AddExpandableResourceCost( 0, "15,000 per person", nanobots ).ListSeparator()
                            .AddExpandableResourceCost( 0, "1 per person", greens ).ListSeparator()
                            .AddExpandableResourceCost( 0, "1 per person", meat ).ListSeparator()
                            .AddExpandableResourceCost( 0, "2 per person", water ).Line();
                        BufferOrNull.BoldLineHeader( "Deal_BonusWhileActive" )
                            .AddRaw( "Converts up to " + humans.ToStringThousandsWhole() + " Abandoned Humans per turn into Upgraded Humans. Costs are paid only for people actually upgraded." ).Line();
                        BufferOrNull.EndLineHeight();
                    }
                    break;
                case VRActionLogic.FlagAnyRelatedResources:
                    FlagRelated( abandoned );
                    FlagRelated( nanobots );
                    FlagRelated( greens );
                    FlagRelated( meat );
                    FlagRelated( water );
                    break;
                case VRActionLogic.GetCanAfford:
                    return VRActionResult.Success;
                case VRActionLogic.TryPayCosts:
                    return VRActionResult.Success;
                case VRActionLogic.MenuClick:
                    Action.ToggleActive();
                    return VRActionResult.Success;
            }

            return VRActionResult.Indeterminate;
        }

        private static VRActionResult HandleExpandHealthPact( MachineVRModeAction Action, ArcenCharacterBufferBase BufferOrNull, VRActionLogic Logic )
        {
            MachineVRModeAction mesh = MachineVRModeActionTable.Instance.GetRowByIDOrNullIfNotFound( "OI_PublicHealthMesh" );
            int level = OrganicIntegrationCalculators.EnsurePublicHealthLevel( mesh );
            bool isComplete = level >= OrganicIntegrationCalculators.PublicHealthMaxLevel;
            ResourceType insight = GetResource( InsightResource );
            long cost = OrganicIntegrationCalculators.GetPublicHealthUpgradeInsightCost( level );

            switch ( Logic )
            {
                case VRActionLogic.AppendToVRActionTooltip:
                    if ( BufferOrNull != null )
                    {
                        BufferOrNull.StartStyleLineHeightA();
                        BufferOrNull.AddActiveOrInactiveOrCompleteStatusLine( false, isComplete );
                        BufferOrNull.BoldLineHeader( "DealLevel_CurrentLevel" ).AddRaw( level.ToStringThousandsWhole() + " / " + OrganicIntegrationCalculators.PublicHealthMaxLevel.ToStringThousandsWhole() ).Line();
                        if ( !isComplete )
                        {
                            long nextHumans = OrganicIntegrationCalculators.GetPublicHealthHumansHandledPerTurn( level + 1 );
                            BufferOrNull.BoldLineHeader( "Deal_ActivationCost" ).AddExpandableResourceCost( 0, cost.ToStringThousandsWhole(), insight ).Line();
                            BufferOrNull.BoldLineHeader( "Deal_BonusOnTrigger" ).AddRaw( "Raises Public Health Mesh to level " + (level + 1).ToStringThousandsWhole() + ", handling up to " + nextHumans.ToStringThousandsWhole() + " Abandoned Humans per turn." ).Line();
                        }
                        BufferOrNull.EndLineHeight();
                    }
                    break;
                case VRActionLogic.FlagAnyRelatedResources:
                    FlagRelated( insight );
                    break;
                case VRActionLogic.GetCanAfford:
                    return !isComplete && CanAfford( insight, cost ) ? VRActionResult.Success : VRActionResult.Indeterminate;
                case VRActionLogic.TryPayCosts:
                    if ( isComplete || !CanAfford( insight, cost ) )
                        return VRActionResult.Indeterminate;
                    insight.AlterCurrent_Named( -cost, "Expense_OI_PublicHealthMesh", ResourceAddRule.IgnoreUntilTurnChange );
                    return VRActionResult.Success;
                case VRActionLogic.MenuClick:
                    if ( mesh == null || isComplete )
                        return VRActionResult.Indeterminate;
                    mesh.DGD.PaidUnlocks++;
                    mesh.DGD.HasEverBeenDone = true;
                    Action.DGD.TimesDone++;
                    Action.DGD.HasEverBeenDone = true;
                    return VRActionResult.Success;
            }

            return VRActionResult.Indeterminate;
        }

        private static VRActionResult HandleArchitecturalWeave( MachineVRModeAction Action, ArcenCharacterBufferBase BufferOrNull, VRActionLogic Logic )
        {
            ResourceType insight = GetResource( InsightResource );
            ResourceType nanobots = GetResource( MedicalNanobotsResource );
            UpgradeInt host = UpgradeIntTable.Instance.GetRowByIDOrNullIfNotFound( "ComputingHost" );
            UpgradeInt client = UpgradeIntTable.Instance.GetRowByIDOrNullIfNotFound( "ComputingClient" );
            bool isComplete = (host?.DGD?.GetHasAlreadyBeenFullyUpgraded() ?? true) && (client?.DGD?.GetHasAlreadyBeenFullyUpgraded() ?? true);
            long goal = GetArchitecturalWeaveInsightForNextUpgrade( Action );

            switch ( Logic )
            {
                case VRActionLogic.AppendToVRActionTooltip:
                    if ( BufferOrNull != null )
                    {
                        BufferOrNull.StartStyleLineHeightA();
                        BufferOrNull.AddActiveOrInactiveOrCompleteStatusLine( Action.DGD.IsActiveNow, isComplete );
                        if ( !isComplete )
                        {
                            BufferOrNull.BoldLineHeader( "Deal_CostPerTurn" )
                                .AddExpandableResourceCost( 0, "350", insight ).ListSeparator()
                                .AddExpandableResourceCost( 0, "25,000,000", nanobots ).Line();
                            BufferOrNull.AddInvestmentTowardGoalLine( Action.DGD.UpgradePoints, goal, insight );
                        }
                        if ( host != null )
                        {
                            BufferOrNull.AddExpandableUpgradeIntGainCurrent( host );
                            if ( !host.DGD.GetHasAlreadyBeenFullyUpgraded() )
                                BufferOrNull.AddExpandableUpgradeIntGainNextBonus( host );
                            BufferOrNull.AddExpandableUpgradeIntGainMax( host );
                        }
                        if ( client != null )
                        {
                            BufferOrNull.AddExpandableUpgradeIntGainCurrent( client );
                            if ( !client.DGD.GetHasAlreadyBeenFullyUpgraded() )
                                BufferOrNull.AddExpandableUpgradeIntGainNextBonus( client );
                            BufferOrNull.AddExpandableUpgradeIntGainMax( client );
                        }
                        BufferOrNull.BoldLineHeader( "Deal_BonusWhileActive" ).AddRaw( "Each completed weave level grants +1 Computing Host and +1 Computing Client." ).Line();
                        BufferOrNull.EndLineHeight();
                    }
                    break;
                case VRActionLogic.FlagAnyRelatedResources:
                    FlagRelated( insight );
                    FlagRelated( nanobots );
                    break;
                case VRActionLogic.GetCanAfford:
                    return Action.DGD.IsActiveNow || (!isComplete && CanAfford( insight, 350L ) && CanAfford( nanobots, 25000000L ))
                        ? VRActionResult.Success : VRActionResult.Indeterminate;
                case VRActionLogic.TryPayCosts:
                    return VRActionResult.Success;
                case VRActionLogic.MenuClick:
                    if ( isComplete )
                        return VRActionResult.Indeterminate;
                    Action.ToggleActive();
                    return VRActionResult.Success;
            }

            return VRActionResult.Indeterminate;
        }

        private static VRActionResult HandleMaintainedToggle( MachineVRModeAction Action, ArcenCharacterBufferBase BufferOrNull, VRActionLogic Logic, string bonusText, params ResourceCost[] costs )
        {
            switch ( Logic )
            {
                case VRActionLogic.AppendToVRActionTooltip:
                    if ( BufferOrNull != null )
                    {
                        BufferOrNull.StartStyleLineHeightA();
                        BufferOrNull.AddActiveOrInactiveStatusLine( Action.DGD.IsActiveNow );
                        BufferOrNull.BoldLineHeader( "Deal_CostPerTurn" );
                        for ( int i = 0; i < costs.Length; i++ )
                        {
                            if ( i > 0 )
                                BufferOrNull.ListSeparator();
                            BufferOrNull.AddExpandableResourceCost( 0, costs[i].DisplayAmount, costs[i].Resource );
                        }
                        BufferOrNull.Line();
                        BufferOrNull.BoldLineHeader( "Deal_BonusWhileActive" ).AddRaw( bonusText ).Line();
                        BufferOrNull.EndLineHeight();
                    }
                    break;
                case VRActionLogic.FlagAnyRelatedResources:
                    for ( int i = 0; i < costs.Length; i++ )
                        FlagRelated( costs[i].Resource );
                    break;
                case VRActionLogic.GetCanAfford:
                    return Action.DGD.IsActiveNow || CanAffordAll( costs ) ? VRActionResult.Success : VRActionResult.Indeterminate;
                case VRActionLogic.TryPayCosts:
                    return VRActionResult.Success;
                case VRActionLogic.MenuClick:
                    Action.ToggleActive();
                    return VRActionResult.Success;
            }

            return VRActionResult.Indeterminate;
        }

        private static ResourceCost Cost( string resourceID, long amount )
        {
            return new ResourceCost( GetResource( resourceID ), amount, amount.ToStringThousandsWhole() );
        }

        private struct ResourceCost
        {
            public readonly ResourceType Resource;
            public readonly long Amount;
            public readonly string DisplayAmount;

            public ResourceCost( ResourceType resource, long amount, string displayAmount )
            {
                Resource = resource;
                Amount = amount;
                DisplayAmount = displayAmount;
            }
        }

        private static ResourceType GetResource( string id )
        {
            return ResourceTypeTable.Instance.GetRowByIDOrNullIfNotFound( id );
        }

        private static bool CanAfford( ResourceType resource, long amount )
        {
            return amount <= 0 || (resource != null && resource.Current >= amount);
        }

        private static bool CanAffordAll( ResourceCost[] costs )
        {
            for ( int i = 0; i < costs.Length; i++ )
            {
                if ( !CanAfford( costs[i].Resource, costs[i].Amount ) )
                    return false;
            }
            return true;
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

        private static long GetArchitecturalWeaveInsightForNextUpgrade( MachineVRModeAction Action )
        {
            long[] costs = new long[] { 1500L, 2500L, 4000L, 6500L, 10000L, 15000L, 22000L, 30000L };
            int index = Action?.DGD?.PaidUnlocks ?? 0;
            if ( index < 0 )
                index = 0;
            if ( index >= costs.Length )
                index = costs.Length - 1;
            return costs[index];
        }
    }
}
