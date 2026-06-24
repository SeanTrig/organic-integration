using Arcen.HotM.Core;
using Arcen.Universal;

namespace Arcen.HotM.OrganicIntegration
{
    public class InitialSetupForDLL : IArcenExternalDllInitialLoadCall
    {
        public void RunAfterAllTableImportsComplete( ArcenExternalDllInitialLoadCall Loader )
        {
            ArcenDebugging.LogSingleLine( "OrganicIntegration loaded.", Verbosity.DoNotShow );
        }

        public void RunImmediatelyOnHandlerProcessed( ArcenExternalDllInitialLoadCall Loader )
        {
        }

        public void RunOnFirstTimeExternalAssemblyLoaded()
        {
        }
    }
}
