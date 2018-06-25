namespace FruckEngine.Utils
{
    public static class MathFuncs
    {
        /// <summary>
        /// https://en.wikipedia.org/wiki/Smoothness
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static float Smooth(float t)
        {
            return t * t * t * (t * (t * 6 - 15) + 10);
        }

        /// <summary>
        /// https://en.wikipedia.org/wiki/Linear_interpolation
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="x"></param>
        /// <returns></returns>
        public static float Lerp(float a, float b, float x)
        {
            return a + x * (b - a);
        }
    }
}