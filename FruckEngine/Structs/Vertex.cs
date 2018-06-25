using System.Runtime.InteropServices;
using OpenTK;

namespace FruckEngine.Structs
{
    /// <summary>
    /// A vertex with a few precalculated properties like tangent and bitangent
    /// </summary>
    [StructLayout(LayoutKind.Sequential)] public struct Vertex
    {
        public Vector3 Position;
        public Vector2 UV;
        public Vector3 Normal;
        public Vector3 Tangent;
        public Vector3 Bitangent;

        public Vertex(Vector3 position, Vector2 uv, Vector3 normal, Vector3 tangent, Vector3 bitangent) {
            Position = position;
            UV = uv;
            Normal = normal;
            Tangent = tangent;
            Bitangent = bitangent;
        }
    }
}