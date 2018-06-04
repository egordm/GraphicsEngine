using OpenTK;

namespace FruckEngine.Helpers
{
    public static class VectorUtils
    {
        public static Vector3 ArrayToVec(float[] arr, int start)
        {
            return new Vector3(arr[start + 0], arr[start + 1], arr[start + 2]);
        }
    }
}