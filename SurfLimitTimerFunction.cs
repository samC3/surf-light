using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using static SurfLightFunctions.Helpers.LifxHelpers;
using static SurfLightFunctions.Helpers.BomHelpers;
using static SurfLightFunctions.Helpers.QldDataHelpers;
using SurfLightFunctions.Models;

namespace SurfLightFunctions
{
    public static class SurfLimitTimerFunction
    {
        [FunctionName("SurfLimitTimerFunction")]
        public static async Task Run([TimerTrigger("0 10 5 * * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"STARTING FUNCTION APP RUN");
            log.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            var swellData = await GetSwellData();
            var latestSwellData = ReadSwellData(swellData);

            log.LogInformation($"Got swell data. period: {latestSwellData.TimePeriod}; height: {latestSwellData.WaveHeight}");

            var bomData = await GetWindData();
            var latestWindData = ReadBomData(bomData);

            log.LogInformation($"Got wind data. speed: {latestWindData.WindSpeed}; direction: {latestWindData.WindDirection.ToString()}");

            var windDirGood = (int)latestWindData.WindDirection <= 5;
            var windSpeedGood = latestWindData.WindSpeed < 15;

            var wavePeriodGood = latestSwellData.TimePeriod >= 7;
            var waveHeight = latestSwellData.WaveHeight >= 0.50;

            var lightStatus = windDirGood && windSpeedGood && wavePeriodGood && waveHeight
                ? new LifxPayload { Power = "on", Color = "#f8e5c2", Brightness = "1", Duration = "10" }
                : new LifxPayload { Power = "on", Color = "#f08848", Brightness = "0.5", Duration = "20" };

            await SetLight(lightStatus);

            log.LogInformation("FINSIHED EXECUTING");

            return;
        }
    }
}
