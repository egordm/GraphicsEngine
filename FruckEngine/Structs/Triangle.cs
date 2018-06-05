using System.Runtime.InteropServices;

namespace FruckEngine.Structs
{
    [StructLayout(LayoutKind.Sequential)] public struct Triangle
    {
        public int v1, v2, v3;

        public Triangle(int v1, int v2, int v3)
        {
            this.v1 = v1;
            this.v2 = v2;
            this.v3 = v3;
        }
    }
}