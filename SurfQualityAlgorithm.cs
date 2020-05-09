using SurfLightFunctions.Models;

namespace SurfLightFunctions
{
    public static class SurfQualityAlgorithm
    {
        public static bool GoSurfing(WaveDataRecord swell, BomResponseData wind)
        {
            var windDirGood = (int)wind.WindDirection <= 5;
            var windSpeedGood = wind.WindSpeed < 15;

            var wavePeriodGood = swell.TimePeriod >= 7;
            var waveHeight = swell.WaveHeight >= 0.50;

            return windDirGood && windSpeedGood && wavePeriodGood && waveHeight;
        }
    }
}