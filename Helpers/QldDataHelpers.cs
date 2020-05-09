using System;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using SurfLightFunctions.Models;

namespace SurfLightFunctions.Helpers
{
    public static class QldDataHelpers
    {
        private static string _qldWaveDataUrl = Environment.GetEnvironmentVariable("QldWaveDataUrl");
        private static string _qldWaveDataResourceId = Environment.GetEnvironmentVariable("QldWaveDataResourceId");
        private static string _waveDataLocation = Environment.GetEnvironmentVariable("WaveDataLocation");

        public static async Task<string> GetSwellData()
        {
            using HttpClient httpClient = new HttpClient();

            var requestPayload = new
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
                Content = new StringContent(
                    JsonSerializer.Serialize(requestPayload),
                    Encoding.UTF8,
                    "application/json"
                )
            };

            var response = await httpClient.SendAsync(request);
            var result = await response.Content.ReadAsStringAsync();

            return result;
        }

        public static WaveDataRecord ReadSwellData(string data)
        {
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