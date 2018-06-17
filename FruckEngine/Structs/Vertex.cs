using System.Runtime.InteropServices;
using OpenTK;

namespace FruckEngine.Structs
{
    [StructLayout(LayoutKind.Sequential)] public struct Vertex
    {
        public Vector3 Position;
        public Vector2 UV;
        public Vector3 Normal;
        public Vector3 Tangent;
        public Vector3 Bitangent;
    }
}