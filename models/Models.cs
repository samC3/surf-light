using System.Text.Json.Serialization;

namespace SurfLightFunctions
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