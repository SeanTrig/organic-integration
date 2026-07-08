using System;
using Arcen.Universal;
using UnityEngine;
using Arcen.HotM.Core;
using Arcen.HotM.External;
using Arcen.HotM.ExternalVis;
using Arcen.HotM.Visualization;

namespace Arcen.HotM.OrganicIntegration
{
    public class OrganicIntegrationConsumables : IResourceConsumableImplementation
    {
        private const string CrossoverGooSwarmID = "OI_CrossoverGoo";
        private const int MinPhageRange = 30;

        public bool TryHandleConsumableHardTargeting( ISimMachineActor Actor, ResourceConsumable Consumable,
            Vector3A center, float attackRange, float moveRange, bool SkipLinesIfOverNothing )
        {
            if ( Consumable == null || Actor == null )
                return false;

            NovelTooltipBuffer novel = NovelTooltipBuffer.Instance;
            int debugStage = 0;
            try
            {
                debugStage = 100;
                float groundLevel = Engine_HotM.CurrentGameMode.GroundLineDrawLevel;
                Vector3A groundCenter = center.ReplaceY( groundLevel );

                switch ( Consumable.ID )
                {
                    case "OI_PhageCharge":
                    {
                        debugStage = 1000;
                        Swarm goo = SwarmTable.Instance.GetRowByIDOrNullIfNotFound( CrossoverGooSwarmID );
                        if ( goo == null )
                            return false;

                        if ( attackRange < MinPhageRange )
                            attackRange = MinPhageRange;

                        foreach ( BaseBuilding Building in TargetingHelper.BuildingsOfTagWithinRangeOfCamera( null ) )
                        {
                            if ( Building == null || Building.SwarmSpread != goo || Building.SwarmSpreadCount <= 0 )
                                continue;
                            MapItem item = Building.Item;
                            if ( item == null )
                                continue;
                            if ( item.LastFramePrepRendered_StructureHighlight >= RenderManager.FramesPrepped )
                                continue;
                            item.LastFramePrepRendered_StructureHighlight = RenderManager.FramesPrepped;
                            MapCell cell = item.ParentCell;
                            if ( cell == null || !cell.IsConsideredInCameraView )
                                continue;
                            SharedRenderManagerData.DrawBeaconRingCenteredOnBuildingItselfRaw( item, ColorRefs.MachineUnitHelpLine );
                        }

                        BaseBuilding gooBuilding = MouseHelper.VisibleBuildingNoFilterUnderCursor;
                        if ( gooBuilding != null && (gooBuilding.SwarmSpread != goo || gooBuilding.SwarmSpreadCount <= 0) )
                            gooBuilding = null;

                        if ( gooBuilding == null )
                            return true;

                        debugStage = 2000;
                        DrawHelper.RenderRangeCircle( groundCenter, attackRange, ColorRefs.MachineUnitHelpLine.ColorHDR );

                        Vector3A destinationPoint = gooBuilding.Item.CenterPoint;
                        if ( Engine_Universal.IsMouseOverGUI || Engine_HotM.IsGameWorldMouseInteractionBlockedByWindow_General ||
                            Engine_Universal.IsMouseOutsideGameWindow || !destinationPoint.IsValid( true ) )
                            return false;

                        float attackRangePlusTargetRadius = attackRange + (gooBuilding.Item?.OBBCache?.GetCheapRadiusFromExtents() ?? 0);
                        bool isInRange = (destinationPoint - center).GetSquareGroundMagnitude()
                            <= attackRangePlusTargetRadius * attackRangePlusTargetRadius;

                        if ( !isInRange )
                        {
                            if ( SkipLinesIfOverNothing || MouseHelper.GetShouldSkipOutOfRangeNotice( destinationPoint ) )
                                return false;
                            DrawHelper.RenderCatmullLine( Actor.GetCollisionCenter(), destinationPoint,
                                Color.red, 1f, CatmullSlotType.Move, CatmullSlope.AndroidTargeting );
                            CursorHelper.RenderSpecificMouseCursorAtSpot( true, IconRefs.Mouse_OutOfRange, destinationPoint, 0.2f );
                            if ( novel.TryStartSmallerTooltip( TooltipID.Create( Consumable ), null, SideClamp.Any, TooltipNovelWidth.Simple ) )
                            {
                                novel.Icon = IconRefs.Mouse_OutOfRange.Icon;
                                novel.ShouldTooltipBeRed = true;
                                novel.TitleUpperLeft.AddLang( "Move_OutOfRange" );
                                novel.Main.AddRaw( gooBuilding.GetDisplayName() );
                            }
                            return false;
                        }

                        debugStage = 3000;
                        bool canAfford = Consumable.CalculateMaxCouldCreate( true ) > 0;

                        DrawHelper.RenderCatmullLine( Actor.GetCollisionCenter(), destinationPoint,
                            ColorRefs.MachineUnitHelpLine.ColorHDR, 1.5f, CatmullSlotType.Move, CatmullSlope.AndroidTargeting );
                        CursorHelper.RenderSpecificMouseCursorAtSpot( true, Consumable.Icon, IconRefs.MouseMoveMode_ProvideHelp, destinationPoint );

                        if ( novel.TryStartSmallerTooltip( TooltipID.Create( Consumable ), null, SideClamp.Any, TooltipNovelWidth.Simple ) )
                        {
                            novel.Icon = Consumable.Icon;
                            novel.TitleUpperLeft.AddRaw( "Purge Grey Goo  " ).AddRaw( Lang.GetRightClickText() );
                            novel.Main.AddRaw( gooBuilding.GetDisplayName() );
                            if ( !canAfford )
                            {
                                novel.ShouldTooltipBeRed = true;
                                novel.Main.Line().AddLang( "ConsumableBlocked_CannotAfford", ColorTheme.RedOrange2 );
                            }
                        }

                        SharedRenderManagerData.DrawBeaconRingCenteredOnBuildingItselfRaw( gooBuilding.Item, ColorRefs.MachineUnitHelpLine );

                        debugStage = 4000;
                        if ( canAfford && !Actor.GetIsMovingRightNow() && SimCommon.CurrentlyDoingThisManyAttackOfOpportunity <= 0 &&
                            !Actor.GetIsBlockedFromActingForAbility( Actor.IsInTargetingModeForAbility, true ) &&
                            ArcenInput.RightMouseNonUI.GetIsBrieflyClicked_AndConsume() )
                        {
                            if ( Consumable.TryPayForOneNow() )
                            {
                                gooBuilding.SetSwarmSpreadCount( 0 );
                                gooBuilding.SwarmSpread = null;
                                SimCommon.NeedsBuildingListRecalculation = true;
                                Consumable.DirectUseConsumable.OnDirectUse.DGD.PlayAtLocation( destinationPoint, null, true );
                                Consumable.DirectUseConsumable.OnDirectUse.DGD.PlaySoundOnlyAtCamera();
                                Consumable.DGD.AlterScore( 1 );
                                Actor.ApplyVisibilityFromAction( ActionVisibility.IsInoffensive );
                                if ( !InputCaching.ShouldKeepDoingAction && Consumable.MustHoldShiftToStayInMode )
                                    Actor.SetTargetingMode( null, null );
                            }
                        }
                        return true;
                    }
                    default:
                        return false;
                }
            }
            catch ( System.Threading.ThreadAbortException ) { throw; }
            catch ( Exception e )
            {
                ArcenDebugging.LogDebugStageWithStack( "OrganicIntegrationConsumables.TryHandleConsumableHardTargeting",
                    debugStage, Consumable?.ID ?? "[null]", e, Verbosity.ShowAsError );
                return false;
            }
        }
    }
}
