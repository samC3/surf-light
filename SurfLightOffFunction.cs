using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using static SurfLightFunctions.Helpers.LifxHelpers;

namespace SurfLightFunctions
{
    public static class SurfLightOffFunction
    {
        [FunctionName("SurfLightOffFunction")]
        public static async Task Run([TimerTrigger("0 45 5 * * *")]TimerInfo myTimer, ILogger log)
        {
            await SetLight(new { power = "off" });
            return;
        }
    }
}
