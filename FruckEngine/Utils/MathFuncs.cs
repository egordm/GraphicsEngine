namespace FruckEngine.Utils
{
    public static class MathFuncs
    {
        public static float Smooth(float t)
        {
            return t * t * t * (t * (t * 6 - 15) + 10);
        }

        public static float Lerp(float a, float b, float x)
        {
            return a + x * (b - a);
        }
    }
}