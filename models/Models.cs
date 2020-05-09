using System.Text.Json.Serialization;

namespace SurfLightFunctions.Models
{
    public class WaveDataRecord
    {
        [JsonPropertyName("Tp")]
        public double TimePeriod { get; set; }
        [JsonPropertyName("Hmax")]
        public double WaveHeight { get; set; }
    }

    public class LifxPayload
    {
        [JsonPropertyName("power")]
        public string Power { get; set; }
        [JsonPropertyName("color")]
        public string Color { get; set; }
        [JsonPropertyName("brightness")]
        public string Brightness { get; set; }
        [JsonPropertyName("duration")]
        public string Duration { get; set; }
    }

    public class BomResponseData
    {
        [JsonPropertyName("wind_dir")]
        public WindDirection WindDirection { get; set; }
        [JsonPropertyName("wind_spd_km")]
        public int WindSpeed { get; set; }
    }

    public enum WindDirection
    {
        CALM, S, SSW, SW, WSW, W, WNW, NW, NNW, N, NNE, NE, ENE, E, ESE, SE, SSE
    }
}