using System;
using Arcen.HotM.Core;
using Arcen.Universal;

namespace Arcen.HotM.OrganicIntegration
{
    public class OrganicIntegrationFeats : IActorFeatImplementation
    {
        private const int GreyGooDuration = 9999;

        //Thermocyte Lance: per-hit rider statuses (durations in turns; intensities scale off FeatAmount).
        private const int ThermocyteCorrodeTurns = 3;
        private const int ThermocyteCorrodePerFeatPoint = 20; //flat armor plating stripped per feat point
        private const int ThermocyteBlindTurns = 1;
        private const int ThermocyteBlindPerFeatPoint = 3; //Blindness intensity per feat point (-0.8 power, -0.5 range, -2 move per point)
        private const int ThermocyteBurnTurns = 3;
        private const int ThermocyteBurnPerFeatPoint = 20; //OnFire intensity per feat point (-0.1 HP, -1 morale per point per turn)

        //Systems Disruptor: FeatAmount is the WeaponsDisrupted percentage; armor corrosion is double that, flat.
        private const int DisruptorStatusTurns = 2;

        //Grey-Goo Cascade: bonus physical damage per existing goo stack on the target, and its cap.
        private const int CascadePercentPerStack = 6;
        private const int CascadeMaxBonusPercent = 150;

        //Active Ablative: charge reservoir limit; FeatAmount is the per-hit damage cap while a charge holds.
        private const int AblativeMaxCharges = 3;

        //Filament Carapace: Elemental Slurry cost per this much shield rebuilt by the feat's top-up.
        private const int CarapaceShieldPerSlurry = 10;

        //Reweave Catalyst: how long the mark lingers once the carrier moves away.
        private const int ReweaveMarkTurns = 2;
        //Reweave Catalyst: extra Grey Goo stacks a marked attacker lays on per application.
        private const int ReweaveApplicationBonus = 2;

        public void DoWhenDealingDamage_Full( ActorFeat Feat, float FeatAmount, ISimMapActor Attacker, ISimMapActor Target,
            ref int PhysicalAttackPowerSoFar, ref int FearAttackPowerSoFar, ref int ArgumentAttackPowerSoFar, ref int SystemsDisruptionSoFar,
            ArcenCharacterBufferBase PhysicalBufferOrNull, ArcenCharacterBufferBase FearBufferOrNull, ArcenCharacterBufferBase ArgumentBufferOrNull, ArcenCharacterBufferBase SystemsDisruptionBufferOrNull,
            ArcenCharacterBufferBase SecondaryBufferOrNull, bool IsAOESecondaryTarget, bool isAnyKindOfPrediction, out AttackStop StopType, SquirrelRand Rand )
        {
            StopType = AttackStop.None;
            if ( Feat == null || Target == null )
                return;
            if ( Target.GetIsPartOfPlayerForcesInAnyWay() || Target.GetIsAnAllyFromThePlayerPerspective() )
                return;

            switch ( Feat.ID )
            {
                case "OI_NanobotRounds":
                    {
                        if ( isAnyKindOfPrediction || PhysicalAttackPowerSoFar <= 0 )
                            return;

                        ActorStatus status = ActorStatusTable.Instance.GetRowByIDOrNullIfNotFound( "OI_GreyGoo" );
                        if ( status == null )
                            return;

                        int intensity = Math.Max( 1, (int)Math.Round( FeatAmount ) ) + GetReweaveGooBonus( Attacker );
                        Target.AddStatus( Attacker, status, intensity, GreyGooDuration, Attacker is ISimNPCUnit );
                    }
                    break;
                case "OI_ThermocyteLance":
                    HandleThermocyteLance( FeatAmount, Attacker, Target, PhysicalAttackPowerSoFar, isAnyKindOfPrediction );
                    break;
                case "OI_SystemsDisruptor":
                    HandleSystemsDisruptor( FeatAmount, Attacker, Target, isAnyKindOfPrediction );
                    break;
                case "OI_GreyGooCascade":
                    HandleGreyGooCascade( FeatAmount, Attacker, Target, ref PhysicalAttackPowerSoFar, PhysicalBufferOrNull, isAnyKindOfPrediction );
                    break;
                default:
                    break;
            }
        }

        #region HandleThermocyteLance
        /// <summary>
        /// The lance's riders: armor corrosion, a short blind, and a burn. All are execution-only side
        /// effects (never applied during predictions); the damage channels are untouched here - the item's
        /// big Terrify lives in its ActorFearAttackPower stat, not in this hook.
        /// </summary>
        private static void HandleThermocyteLance( float FeatAmount, ISimMapActor Attacker, ISimMapActor Target,
            int PhysicalAttackPowerSoFar, bool isAnyKindOfPrediction )
        {
            if ( isAnyKindOfPrediction || FeatAmount <= 0f )
                return;

            bool addExtraTurn = Attacker is ISimNPCUnit;
            int featPoints = Math.Max( 1, (int)Math.Round( FeatAmount ) );

            ActorStatus corroded = ActorStatusTable.Instance.GetRowByIDOrNullIfNotFound( "ArmorPlatingCorroded" );
            if ( corroded != null )
                Target.AddStatus( Attacker, corroded, featPoints * ThermocyteCorrodePerFeatPoint, ThermocyteCorrodeTurns, addExtraTurn );

            ActorStatus blindness = ActorStatusTable.Instance.GetRowByIDOrNullIfNotFound( "Blindness" );
            if ( blindness != null )
                Target.AddStatus( Attacker, blindness, featPoints * ThermocyteBlindPerFeatPoint, ThermocyteBlindTurns, addExtraTurn );

            //the burn: the base game's OnFire DoT (health plus morale per turn) is exactly the fire rider we want
            ActorStatus onFire = ActorStatusTable.Instance.GetRowByIDOrNullIfNotFound( "OnFire" );
            if ( onFire != null )
                Target.AddStatus( Attacker, onFire, featPoints * ThermocyteBurnPerFeatPoint, ThermocyteBurnTurns, addExtraTurn );
        }
        #endregion

        #region HandleSystemsDisruptor
        /// <summary>
        /// Insight nonviolence: WeaponsDisrupted is a percentage ActorPower reduction (the disarm), plus a
        /// flat armor-plating corrosion so follow-up systems volleys land harder. Execution-only side effects.
        /// </summary>
        private static void HandleSystemsDisruptor( float FeatAmount, ISimMapActor Attacker, ISimMapActor Target,
            bool isAnyKindOfPrediction )
        {
            if ( isAnyKindOfPrediction || FeatAmount <= 0f )
                return;

            bool addExtraTurn = Attacker is ISimNPCUnit;
            int disarmPercent = Math.Max( 1, (int)Math.Round( FeatAmount ) );

            ActorStatus disrupted = ActorStatusTable.Instance.GetRowByIDOrNullIfNotFound( "WeaponsDisrupted" );
            if ( disrupted != null )
                Target.AddStatus( Attacker, disrupted, disarmPercent, DisruptorStatusTurns, addExtraTurn );

            ActorStatus corroded = ActorStatusTable.Instance.GetRowByIDOrNullIfNotFound( "ArmorPlatingCorroded" );
            if ( corroded != null )
                Target.AddStatus( Attacker, corroded, disarmPercent * 2, DisruptorStatusTurns, addExtraTurn );
        }
        #endregion

        #region HandleGreyGooCascade
        /// <summary>
        /// Two halves: (1) bonus physical damage scaling with the goo already on the target - a deterministic
        /// status read, so it runs during predictions too and the preview matches the hit; (2) fresh goo
        /// stacks, execution-only. Note that RapidFire multiplies the damage of a single hook invocation
        /// rather than re-running feats per shot, so the FeatAmount on the item is tuned as the whole
        /// burst's payload, not a per-bullet one.
        /// </summary>
        private static void HandleGreyGooCascade( float FeatAmount, ISimMapActor Attacker, ISimMapActor Target,
            ref int PhysicalAttackPowerSoFar, ArcenCharacterBufferBase PhysicalBufferOrNull, bool isAnyKindOfPrediction )
        {
            ActorStatus goo = ActorStatusTable.Instance.GetRowByIDOrNullIfNotFound( "OI_GreyGoo" );
            if ( goo == null )
                return;

            int existingStacks = Target.GetStatusIntensity( goo );
            if ( existingStacks > 0 && PhysicalAttackPowerSoFar > 0 )
            {
                int bonusPercent = Math.Min( existingStacks * CascadePercentPerStack, CascadeMaxBonusPercent );
                int bonus = (PhysicalAttackPowerSoFar * bonusPercent) / 100;
                if ( bonus > 0 )
                {
                    PhysicalAttackPowerSoFar += bonus;
                    if ( PhysicalBufferOrNull != null )
                        PhysicalBufferOrNull.StartSize80().AddRaw( "Grey-Goo Cascade ", ColorTheme.DataLabelWhite )
                            .AddNumberPlusOrMinus( true, bonus.ToStringThousandsWhole(), ColorTheme.DataBlue ).EndSize().Line();
                }
            }

            if ( !isAnyKindOfPrediction && FeatAmount > 0f )
            {
                int stacks = Math.Max( 1, (int)Math.Round( FeatAmount ) ) + GetReweaveGooBonus( Attacker );
                Target.AddStatus( Attacker, goo, stacks, GreyGooDuration, Attacker is ISimNPCUnit );
            }
        }

        // A Grey Goo applier standing in a Reweave Catalyst field (carrying OI_ReweaveMark) lays on extra stacks.
        private static int GetReweaveGooBonus( ISimMapActor Attacker )
        {
            if ( Attacker == null )
                return 0;
            ActorStatus mark = ActorStatusTable.Instance.GetRowByIDOrNullIfNotFound( "OI_ReweaveMark" );
            if ( mark == null || Attacker.GetStatusIntensity( mark ) <= 0 )
                return 0;
            return ReweaveApplicationBonus;
        }
        #endregion

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
            if ( Feat == null || Target == null )
                return;

            switch ( Feat.ID )
            {
                case "OI_ActiveAblative":
                    HandleActiveAblativeHit( FeatAmount, Target, ref PhysicalAttackPowerSoFar, PhysicalBufferOrNull, isAnyKindOfPrediction );
                    break;
                case "OI_ReactiveGoo":
                    HandleReactiveGoo( FeatAmount, Attacker, Target, isAnyKindOfPrediction );
                    break;
                default:
                    break;
            }
        }

        #region HandleActiveAblativeHit
        /// <summary>
        /// While a charge is held, an incoming hit's physical damage is capped to FeatAmount and one charge
        /// is consumed. The charge count is a deterministic status read, so the cap applies during predictions
        /// too (the preview matches the hit); the charge is only consumed at execution.
        /// </summary>
        private static void HandleActiveAblativeHit( float FeatAmount, ISimMapActor Target,
            ref int PhysicalAttackPowerSoFar, ArcenCharacterBufferBase PhysicalBufferOrNull, bool isAnyKindOfPrediction )
        {
            int damageCap = (int)Math.Round( FeatAmount );
            if ( damageCap <= 0 || PhysicalAttackPowerSoFar <= damageCap )
                return; //hit already below the cap: no charge spent, nothing absorbed

            ActorStatus chargeStatus = ActorStatusTable.Instance.GetRowByIDOrNullIfNotFound( "OI_AblativeCharge" );
            if ( chargeStatus == null )
                return;

            int charges = Target.GetStatusIntensity( chargeStatus );
            if ( charges <= 0 )
                return;

            int absorbed = PhysicalAttackPowerSoFar - damageCap;
            PhysicalAttackPowerSoFar = damageCap;

            if ( PhysicalBufferOrNull != null )
                PhysicalBufferOrNull.StartSize80().AddRaw( "Ablative Charge ", ColorTheme.DataLabelWhite )
                    .AddNumberPlusOrMinus( false, absorbed.ToStringThousandsWhole(), ColorTheme.DataBlue ).EndSize().Line();

            if ( !isAnyKindOfPrediction )
            {
                //consume one charge: clear-and-re-add is the mod's standard exact-intensity idiom
                Target.ClearStatus( chargeStatus );
                if ( charges > 1 )
                    Target.AddStatus( null, chargeStatus, charges - 1, GreyGooDuration, false );
            }
        }
        #endregion

        #region HandleReactiveGoo
        /// <summary>
        /// Thorns, Dominion-style: whoever hits the carrier gains Grey Goo stacks. Execution-only, and never
        /// goos the player's own forces (a friendly-fire splash should not seed your own units).
        /// </summary>
        private static void HandleReactiveGoo( float FeatAmount, ISimMapActor Attacker, ISimMapActor Target,
            bool isAnyKindOfPrediction )
        {
            if ( isAnyKindOfPrediction || FeatAmount <= 0f )
                return;
            if ( Attacker == null || Attacker.IsFullDead )
                return;
            if ( Attacker.GetIsPartOfPlayerForcesInAnyWay() || Attacker.GetIsAnAllyFromThePlayerPerspective() )
                return;

            ActorStatus goo = ActorStatusTable.Instance.GetRowByIDOrNullIfNotFound( "OI_GreyGoo" );
            if ( goo == null )
                return;

            int stacks = Math.Max( 1, (int)Math.Round( FeatAmount ) );
            Attacker.AddStatus( Target, goo, stacks, GreyGooDuration, Target is ISimNPCUnit );

            if ( Attacker is ISimNPCUnit npc )
            {
                //credit the exposure to the player, so deaths from the goo count as player kills
                npc.HasBeenPhysicallyDamagedByPlayer = true;
                npc.HasBeenPhysicallyOrMoraleOrSystemDamagedByPlayer = true;
                npc.HasBeenPhysicallyOrMoraleOrSystemDamagedByPlayerThisTurn = true;
            }
        }
        #endregion

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
            if ( Feat == null || Actor == null || Actor.IsFullDead )
                return;

            switch ( Feat.ID )
            {
                case "OI_RestorativeWeave":
                    HandleRestorativeWeave( Actor, FeatAmount );
                    break;
                case "OI_ActiveAblative":
                    HandleAblativeChargeRegen( Actor );
                    break;
                case "OI_FilamentCarapace":
                    HandleFilamentCarapace( Actor, FeatAmount );
                    break;
                case "OI_DissolutionField":
                    HandleDissolutionField( Actor, FeatAmount );
                    break;
                case "OI_SubversionNode":
                    HandleSubversionNode( Actor, FeatAmount );
                    break;
                case "OI_RepairBeacon":
                    HandleRepairBeacon( Actor, FeatAmount );
                    break;
                case "OI_ReweaveCatalyst":
                    HandleReweaveCatalyst( Actor, FeatAmount );
                    break;
                default:
                    break;
            }
        }

        #region HandleRestorativeWeave
        /// <summary>
        /// Heals the wearer FeatAmount percent of its maximum health at turn start (the KindledHope repair
        /// idiom, but percentage-based and unconditional). Turn-start hooks are never part of attack
        /// prediction, so side effects are safe here.
        /// </summary>
        private static void HandleRestorativeWeave( ISimMapActor Actor, float FeatAmount )
        {
            if ( FeatAmount <= 0f )
                return;

            int missingHP = Actor.GetActorDataLostFromMax( ActorRefs.ActorHP, true );
            if ( missingHP <= 0 )
                return; //already whole

            int maxHP = Actor.GetActorDataMaximum( ActorRefs.ActorHP, true );
            int healAmount = Math.Max( 1, (int)Math.Round( maxHP * (FeatAmount * 0.01f) ) );
            Actor.AlterActorDataCurrent( ActorRefs.ActorHP, Math.Min( healAmount, missingHP ), false );
        }
        #endregion

        #region HandleAblativeChargeRegen
        /// <summary>
        /// Regrows one ablative charge per turn, up to the reservoir limit. Clear-and-re-add is the mod's
        /// standard exact-intensity idiom for charge-like statuses.
        /// </summary>
        private static void HandleAblativeChargeRegen( ISimMapActor Actor )
        {
            ActorStatus chargeStatus = ActorStatusTable.Instance.GetRowByIDOrNullIfNotFound( "OI_AblativeCharge" );
            if ( chargeStatus == null )
                return;

            int charges = Actor.GetStatusIntensity( chargeStatus );
            if ( charges >= AblativeMaxCharges )
                return;

            if ( charges > 0 )
                Actor.ClearStatus( chargeStatus );
            Actor.AddStatus( null, chargeStatus, charges + 1, GreyGooDuration, false );
        }
        #endregion

        #region HandleFilamentCarapace
        /// <summary>
        /// On top of the XML-side ShieldRegenPerTurn (which the per-turn base logic runs before turn-start
        /// feats), the carapace rebuilds up to FeatAmount more of the missing EnergyShield pool, paying
        /// Elemental Slurry for the privilege (1 per CarapaceShieldPerSlurry rebuilt). Player-forces carriers
        /// only: an enemy wearing looted gear must not drain the player's stockpile.
        /// </summary>
        private static void HandleFilamentCarapace( ISimMapActor Actor, float FeatAmount )
        {
            int topUpLimit = (int)Math.Round( FeatAmount );
            if ( topUpLimit <= 0 )
                return;
            if ( !Actor.GetIsPartOfPlayerForcesInAnyWay() )
                return;

            //actors deserialized from older saves may lack the EnergyShield row; cheap no-op when present
            Actor.GetActorDataDataAndInitializeIfNeedBe( ActorRefs.EnergyShield, 0, 0 );
            MapActorData shieldData = Actor.GetActorDataData( ActorRefs.EnergyShield, true );
            if ( shieldData == null || shieldData.Maximum <= 0 )
                return;

            int missing = shieldData.Maximum - shieldData.Current;
            if ( missing <= 0 )
                return;

            int topUp = Math.Min( missing, topUpLimit );
            ResourceType slurry = ResourceTypeTable.Instance.GetRowByIDOrNullIfNotFound( "ElementalSlurry" );
            if ( slurry == null )
                return;

            long slurryCost = (topUp + CarapaceShieldPerSlurry - 1) / CarapaceShieldPerSlurry;
            if ( slurry.Current < slurryCost )
            {
                //rebuild only what the stockpile can pay for
                topUp = (int)Math.Min( (long)topUp, slurry.Current * CarapaceShieldPerSlurry );
                if ( topUp <= 0 )
                    return;
                slurryCost = (topUp + CarapaceShieldPerSlurry - 1) / CarapaceShieldPerSlurry;
            }

            slurry.AlterCurrent_Named( -slurryCost, "Expense_OI_FilamentCarapace", ResourceAddRule.IgnoreUntilTurnChange );
            shieldData.AlterCurrent( topUp );
        }
        #endregion

        #region HandleDissolutionField
        /// <summary>
        /// Turn-start aura: every hostile unit within FeatAmount radius gains one Grey Goo stack. Mirrors the
        /// EscortLattice enumeration shape (same-tile ActorsWithinMaxNPCAttackRange plus a radius filter).
        /// </summary>
        private static void HandleDissolutionField( ISimMapActor Carrier, float Radius )
        {
            if ( Radius <= 0f )
                return;

            ActorStatus goo = ActorStatusTable.Instance.GetRowByIDOrNullIfNotFound( "OI_GreyGoo" );
            if ( goo == null )
                return;

            MapTile tile = Carrier.CalculateMapCell()?.ParentTile;
            if ( tile == null )
                return;

            float radiusSquared = Radius * Radius;
            Vector3A myLocation = Carrier.GetDrawLocation();
            bool addExtraTurn = Carrier is ISimNPCUnit;

            foreach ( ISimMapActor prospect in tile.ActorsWithinMaxNPCAttackRange )
            {
                if ( prospect is MachineStructure )
                    continue; //units only
                if ( prospect.ActorID == Carrier.ActorID || prospect.IsFullDead )
                    continue;
                if ( !(prospect is ISimNPCUnit npc) )
                    continue; //hostility is an npc-unit concept
                if ( npc.GetIsPartOfPlayerForcesInAnyWay() || npc.GetIsAnAllyFromThePlayerPerspective() )
                    continue;
                if ( !npc.GetIsConsideredHostileToPlayer() )
                    continue;
                if ( (prospect.GetDrawLocation() - myLocation).GetSquareGroundMagnitude() > radiusSquared )
                    continue;

                npc.AddStatus( Carrier, goo, 1, GreyGooDuration, addExtraTurn );

                //credit the exposure to the player, so deaths from the goo count as player kills
                npc.HasBeenPhysicallyDamagedByPlayer = true;
                npc.HasBeenPhysicallyOrMoraleOrSystemDamagedByPlayer = true;
                npc.HasBeenPhysicallyOrMoraleOrSystemDamagedByPlayerThisTurn = true;
            }
        }
        #endregion

        #region HandleSubversionNode
        /// <summary>
        /// Turn-start aura: converts ONE hostile unit within FeatAmount radius to player forces per turn per
        /// node - ConvertEnemyRobotToPlayerForces has no robot check, so organics and unhackables convert
        /// too. The captured-cap headroom pattern lives in OrganicIntegrationCalculators (shared with the
        /// grey-goo saturation conversion). Prefers the most goo-saturated candidate in range, for flavor
        /// and to synergize with the rest of the set.
        /// </summary>
        private static void HandleSubversionNode( ISimMapActor Carrier, float Radius )
        {
            if ( Radius <= 0f )
                return;

            MapTile tile = Carrier.CalculateMapCell()?.ParentTile;
            if ( tile == null )
                return;

            ActorStatus goo = ActorStatusTable.Instance.GetRowByIDOrNullIfNotFound( "OI_GreyGoo" );

            float radiusSquared = Radius * Radius;
            Vector3A myLocation = Carrier.GetDrawLocation();

            ISimNPCUnit bestCandidate = null;
            int bestGoo = -1;
            foreach ( ISimMapActor prospect in tile.ActorsWithinMaxNPCAttackRange )
            {
                if ( prospect is MachineStructure )
                    continue;
                if ( prospect.ActorID == Carrier.ActorID || prospect.IsFullDead )
                    continue;
                if ( !(prospect is ISimNPCUnit npc) || npc.UnitType == null )
                    continue;
                if ( npc.GetIsPartOfPlayerForcesInAnyWay() || npc.GetIsAnAllyFromThePlayerPerspective() )
                    continue;
                if ( !npc.GetIsConsideredHostileToPlayer() )
                    continue;
                if ( (prospect.GetDrawLocation() - myLocation).GetSquareGroundMagnitude() > radiusSquared )
                    continue;

                int gooHere = goo != null ? npc.GetStatusIntensity( goo ) : 0;
                if ( gooHere > bestGoo )
                {
                    bestGoo = gooHere;
                    bestCandidate = npc;
                }
            }

            if ( bestCandidate != null )
                OrganicIntegrationCalculators.TrySubvertUnitToPlayerForces( bestCandidate );
        }
        #endregion

        #region HandleRepairBeacon
        /// <summary>
        /// Turn-start aura: heals every friendly unit within the carrier's CURRENT AttackRange (the beacon
        /// rides the targeting network, so range gear extends it) for FeatAmount percent of that unit's own
        /// maximum health. The carrier itself is included - it is inside its own network.
        /// </summary>
        private static void HandleRepairBeacon( ISimMapActor Carrier, float FeatAmount )
        {
            if ( FeatAmount <= 0f )
                return;

            int range = Carrier.GetActorDataCurrent( ActorRefs.AttackRange, true );
            if ( range <= 0 )
                return;

            MapTile tile = Carrier.CalculateMapCell()?.ParentTile;
            if ( tile == null )
                return;

            float radiusSquared = (float)range * (float)range;
            Vector3A myLocation = Carrier.GetDrawLocation();

            //the carrier heals itself too, then the loop covers everyone else nearby
            HealPercentOfMax( Carrier, FeatAmount );

            foreach ( ISimMapActor prospect in tile.ActorsWithinMaxNPCAttackRange )
            {
                if ( prospect is MachineStructure )
                    continue; //units only
                if ( prospect.ActorID == Carrier.ActorID || prospect.IsFullDead )
                    continue;
                if ( !prospect.GetIsPartOfPlayerForcesInAnyWay() )
                    continue;
                if ( (prospect.GetDrawLocation() - myLocation).GetSquareGroundMagnitude() > radiusSquared )
                    continue;

                HealPercentOfMax( prospect, FeatAmount );
            }
        }

        private static void HealPercentOfMax( ISimMapActor Actor, float Percent )
        {
            int missingHP = Actor.GetActorDataLostFromMax( ActorRefs.ActorHP, true );
            if ( missingHP <= 0 )
                return;
            int maxHP = Actor.GetActorDataMaximum( ActorRefs.ActorHP, true );
            int healAmount = Math.Max( 1, (int)Math.Round( maxHP * (Percent * 0.01f) ) );
            Actor.AlterActorDataCurrent( ActorRefs.ActorHP, Math.Min( healAmount, missingHP ), false );
        }
        #endregion

        #region HandleReweaveCatalyst
        /// <summary>
        /// Turn-start aura: marks every unit within FeatAmount radius (friend and foe) with OI_ReweaveMark.
        /// Marked friendly attackers lay on extra Grey Goo (GetReweaveGooBonus); marked enemies shed Grey Goo
        /// more slowly (ApplyGreyGooFalloff). The field both accelerates saturation and makes it stick.
        /// </summary>
        private static void HandleReweaveCatalyst( ISimMapActor Carrier, float Radius )
        {
            if ( Radius <= 0f )
                return;

            ActorStatus mark = ActorStatusTable.Instance.GetRowByIDOrNullIfNotFound( "OI_ReweaveMark" );
            if ( mark == null )
                return;

            MapTile tile = Carrier.CalculateMapCell()?.ParentTile;
            if ( tile == null )
                return;

            float radiusSquared = Radius * Radius;
            Vector3A myLocation = Carrier.GetDrawLocation();
            bool addExtraTurn = Carrier is ISimNPCUnit;

            //the carrier is primed too
            Carrier.AddStatus( Carrier, mark, 1, ReweaveMarkTurns, addExtraTurn );

            foreach ( ISimMapActor prospect in tile.ActorsWithinMaxNPCAttackRange )
            {
                if ( prospect is MachineStructure )
                    continue; //units only
                if ( prospect.ActorID == Carrier.ActorID || prospect.IsFullDead )
                    continue;
                // Mark friend and foe alike: friendly attackers get the application bonus, enemies get the slowed shed.
                if ( (prospect.GetDrawLocation() - myLocation).GetSquareGroundMagnitude() > radiusSquared )
                    continue;

                prospect.AddStatus( Carrier, mark, 1, ReweaveMarkTurns, addExtraTurn );
            }
        }
        #endregion

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
            switch ( Feat?.ID )
            {
                case "OI_RepairBeacon":
                    //the beacon's reach is the carrier's live attack range, not the feat value
                    return FeatHolder != null ? FeatHolder.GetActorDataCurrent( ActorRefs.AttackRange, true ) : FeatAmount;
                default:
                    return FeatAmount;
            }
        }
    }
}
