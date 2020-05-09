using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using SurfLightFunctions.Models;
using static SurfLightFunctions.Helpers.LifxHelpers;

namespace SurfLightFunctions
{
    public static class SurfLightOffFunction
    {
        [FunctionName("SurfLightOffFunction")]
        public static async Task Run([TimerTrigger("0 45 19 * * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation("TURNING OFF SURF LIGHT");
            await SetLight(new LifxPayload { Power = "off", Duration = 3 });
            return;
        }
    }
}
