using System;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace SurfLightFunctions.Helpers
{
    public static class BomHelpers
    {
        private static string _bomApiUrl = Environment.GetEnvironmentVariable("BomApiUrl");

        public static async Task<string> GetWindData()
        {
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

        public static BomResponseData ReadBomData(string data)
        {
            using JsonDocument jsonDoc = JsonDocument.Parse(data);

            JsonElement root = jsonDoc.RootElement;
            JsonElement dataArray = root.GetProperty("observations").GetProperty("data");

            JsonElement latestData = dataArray.EnumerateArray().First();

            var windDir = latestData.GetProperty("wind_dir");
            var windSp = latestData.GetProperty("wind_spd_kmh");

            return new BomResponseData { WindSpeed = windSp.GetInt32(), WindDirection = Enum.Parse<WindDirection>(windDir.GetString()) };
        }
    }
}