using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace SurfLightFunctions
{
    public static class SurfLimitTimerFunction
    {
        private static ILogger _logger { get; set; }

        private const string _lifxApiUrl = "https://api.lifx.com/v1/lights/";
        private const string _lightSelectorId = "selector";
        private const string _lifxApiToken = "api-token";

        private const string _bomApiUrl = "http://www.bom.gov.au/fwo/IDQ60801/IDQ60801.94592.json";

        private const string _qldWaveDataUrl = "https://www.data.qld.gov.au/api/3/action/datastore_search";
        private const string _qldWaveDataResourceId = "2bbef99e-9974-49b9-a316-57402b00609c";
        private const string _waveDataLocation = "Tweed Heads Mk4";

        [FunctionName("SurfLimitTimerFunction")]
        public static async Task Run([TimerTrigger("0 10 5 * * *")]TimerInfo myTimer, ILogger _log)
        {
            _logger = _log;
            _logger.LogInformation($"STARTING FUNCTION APP RUN");
            _logger.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");

            var swellData = await GetSwellData();
            var latestSwellData = ReadSwellData(swellData);

            _logger.LogInformation($"Got swell data. period: {latestSwellData.TimePeriod}; height: {latestSwellData.WaveHeight}");

            var bomData = await GetWindData();
            var latestWindData = ReadBomData(bomData);

            _logger.LogInformation($"Got wind data. speed: {latestWindData.WindSpeed}; direction: {latestWindData.WindDirection.ToString()}");

            var windDirGood = (int)latestWindData.WindDirection <= 5;
            var windSpeedGood = latestWindData.WindSpeed < 15;

            var wavePeriodGood = latestSwellData.TimePeriod >= 7;
            var waveHeight = latestSwellData.WaveHeight >= 0.50;

            var lightStatus = windDirGood && windSpeedGood && wavePeriodGood && waveHeight
                ? new { power = "on", color = "green" }
                : new { power = "on", color = "red" };

            await SetLight(lightStatus);

            _logger.LogInformation("FINSIHED EXECUTING");

            return;
        }

        private static async Task SetLight(object lightStatus)
        {
            using HttpClient httpClient = new HttpClient();

            var request = new HttpRequestMessage
            {
                RequestUri = new Uri(_lifxApiUrl + _lightSelectorId + "/state"),
                Method = HttpMethod.Put,
                Content = new StringContent(JsonSerializer.Serialize(lightStatus), Encoding.UTF8, "application/json")
            };
            request.Headers.Add("Authorization", $"Bearer {_lifxApiToken}");

            var response = await httpClient.SendAsync(request);

            return;
        }

        private static async Task<string> GetWindData()
        {
            _logger.LogInformation($"Reading swell data");

            using HttpClient httpClient = new HttpClient();

            var request = new HttpRequestMessage
            {
                RequestUri = new Uri(_bomApiUrl),
                Method = HttpMethod.Get
            };

            request.Headers.Add("User-Agent", $"Api-Agent");

            var response = await httpClient.SendAsync(request);

            return await response.Content.ReadAsStringAsync();
        }

        private static BomResponseData ReadBomData(string data)
        {
            _logger.LogInformation($"Reading bom data");

            using JsonDocument jsonDoc = JsonDocument.Parse(data);

            JsonElement root = jsonDoc.RootElement;
            JsonElement dataArray = root.GetProperty("observations").GetProperty("data");

            JsonElement latestData = dataArray.EnumerateArray().First();

            var windDir = latestData.GetProperty("wind_dir");
            var windSp = latestData.GetProperty("wind_spd_kmh");

            return new BomResponseData { WindSpeed = windSp.GetInt32(), WindDirection = Enum.Parse<WindDirection>(windDir.GetString()) };
        }

        private static async Task<string> GetSwellData()
        {
            _logger.LogInformation($"Getting swell data");

            using HttpClient httpClient = new HttpClient();

            var x = new
            {
                limit = "100",
                offset = "300",
                resource_id = _qldWaveDataResourceId,
                filters = $"{{\"Site\":[\"{_waveDataLocation}\"]}}"
            };

            var request = new HttpRequestMessage
            {
                RequestUri = new Uri(_qldWaveDataUrl),
                Method = HttpMethod.Post,
                Content = new StringContent(JsonSerializer.Serialize(x), Encoding.UTF8, "application/json")
            };

            var response = await httpClient.SendAsync(request);

            _logger.LogInformation($"Got swell data");

            var result = await response.Content.ReadAsStringAsync();
            _logger.LogInformation(result);

            return result;
        }

        private static WaveDataRecord ReadSwellData(string data)
        {
            _logger.LogInformation($"Reading swell data");

            using JsonDocument jsonDoc = JsonDocument.Parse(data);

            JsonElement root = jsonDoc.RootElement;
            JsonElement dataArray = root.GetProperty("result").GetProperty("records");

            JsonElement latestData = dataArray.EnumerateArray().Last();

            var time = latestData.GetProperty("Tp");
            var height = latestData.GetProperty("Hmax");

            return new WaveDataRecord { TimePeriod = time.GetDouble(), WaveHeight = height.GetDouble() };
        }
    }
}
