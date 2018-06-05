using System.Runtime.InteropServices;
using OpenTK;

namespace FruckEngine.Structs
{
    [StructLayout(LayoutKind.Sequential)] public struct Vertex
    {
        public Vector2 TexCoord;
        public Vector3 Normal;
        public Vector3 Pos;
    }
}