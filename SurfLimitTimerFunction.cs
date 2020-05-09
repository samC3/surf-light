using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using static SurfLightFunctions.Helpers.LifxHelpers;
using static SurfLightFunctions.Helpers.BomHelpers;
using static SurfLightFunctions.Helpers.QldDataHelpers;
using static SurfLightFunctions.SurfQualityAlgorithm;

namespace SurfLightFunctions
{
    public static class SurfLimitTimerFunction
    {
        [FunctionName("SurfLimitTimerFunction")]
        public static async Task Run([TimerTrigger("0 10 19 * * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"STARTING FUNCTION APP RUN");
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            var swellData = await GetSwellData();
            var latestSwellData = ReadSwellData(swellData);

            log.LogInformation($"Got swell data. period: {latestSwellData.TimePeriod}; height: {latestSwellData.WaveHeight}");

            var bomData = await GetWindData();
            var latestWindData = ReadBomData(bomData);

            log.LogInformation($"Got wind data. speed: {latestWindData.WindSpeed}; direction: {latestWindData.WindDirection.ToString()}");

            var lightStatus = GoSurfing(latestSwellData, latestWindData) ? goSurfing : skipSurfing;

            await SetLight(lightStatus);

            log.LogInformation("FINSIHED EXECUTING");

            return;
        }
    }
}
