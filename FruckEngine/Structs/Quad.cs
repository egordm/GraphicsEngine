using System.Runtime.InteropServices;

namespace FruckEngine.Structs
{
    [StructLayout(LayoutKind.Sequential)] public struct Quad
    {
        public int v1, v2, v3, v4;

        public Quad(int v1, int v2, int v3, int v4)
        {
            this.v1 = v1;
            this.v2 = v2;
            this.v3 = v3;
            this.v4 = v4;
        }
    }
}