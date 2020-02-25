using System;

namespace WorldOfEgon.Helper
{
    public static class Utils
    {
        public static float GenerateRandom(float minValue = -1.0f, float maxValue = 1.0f)
        {
            var random = new Random();
            return Convert.ToSingle(random.NextDouble() * (maxValue - minValue) + minValue);
        }
    }
}