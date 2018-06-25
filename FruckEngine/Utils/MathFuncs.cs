using OpenTK;

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

        /// <summary>
        /// Calculate a tangent form a normal vector
        /// </summary>
        /// <param name="normal"></param>
        /// <returns></returns>
        public static Vector3 CalculateTangent(Vector3 normal) {
            var c1 = Vector3.Cross(normal, Vector3.UnitZ); 
            var c2 = Vector3.Cross(normal, Vector3.UnitY);
            if (c1.LengthSquared > c2.LengthSquared) return c1;
            return c2;
        }
    }
}