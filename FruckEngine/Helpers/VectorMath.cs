using OpenTK;

namespace FruckEngine.Helpers
{
    public static class VectorMath
    {
        public static Vector3 Normal(Vector3 a, Vector3 b, Vector3 c)
        {
            return Vector3.Cross(b - a, c - a);
        }
    }
}