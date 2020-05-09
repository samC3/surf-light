using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using SurfLightFunctions.Models;

namespace SurfLightFunctions.Helpers
{
    public static class LifxHelpers
    {
        public static LifxPayload goSurfing = new LifxPayload
        {
            Power = "on",
            Color = "#f8e5c2",
            Brightness = "1",
            Duration = "10"
        };

        public static LifxPayload skipSurfing = new LifxPayload
        {
            Power = "on",
            Color = "#f08848",
            Brightness = "0.3",
            Duration = "20"
        };

        private static string _lifxApiUrl = Environment.GetEnvironmentVariable("LifxApiUrl");
        private static string _lightSelectorId = Environment.GetEnvironmentVariable("LifxSelectorId");
        private static string _lifxApiToken = Environment.GetEnvironmentVariable("LifxApiToken");

        public static async Task SetLight(LifxPayload lightStatus)
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
    }
}