using Arcen.HotM.Core;
using Arcen.Universal;

namespace Arcen.HotM.OrganicIntegration
{
    public class OrganicIntegrationObjectiveHandlers : IExtraCode_HandleExtraNPCCompletedObjectiveConsequences
    {
        public void HandleExtraNPCCompletedObjectiveConsequences( ExtraCodeHandler Handler, ISimNPCUnit Unit, NPCUnitObjective Objective, SquirrelRand Rand,
            NPCMission relatedMission, ISimMapActor objectiveActor, BaseBuilding objectiveBuilding )
        {
            if ( Handler == null || Handler.ID != "OI_PlayerWorkerOfferIntegration" || Unit == null || Objective == null || Rand == null )
                return;

            OrganicIntegrationCalculators.TryVoluntaryBuildingIntegration( objectiveBuilding, Rand );
        }
    }
}