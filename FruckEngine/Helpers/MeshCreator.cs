using System.Collections.Generic;
using FruckEngine.Graphics;
using FruckEngine.Structs;
using OpenTK.Graphics.OpenGL;

namespace FruckEngine.Helpers
{
    public static class MeshCreator
    {
        public static Mesh FromWavefront(string file)
        {
            return FromWavefront(WavefrontLoader.LoadOnce(file));
        }
        
        public static Mesh FromWavefront(WavefrontMesh wfm)
        {
            var indexBuffers = new List<IIndexBuffer>();
            indexBuffers.Add(new IndexBuffer<Triangle>(wfm.Triangles, 3, PrimitiveType.Triangles));
            if (wfm.Quads.Length > 0) indexBuffers.Add(new IndexBuffer<Quad>(wfm.Quads, 4, PrimitiveType.Quads));
            
            var attribBuffers = new List<IUploadableBuffer>();
            attribBuffers.Add(new AttribBuffer<WavefrontVertex>(wfm.Vertices, "vPosition", VertexAttribPointerType.Float, 3) {
                Stride = 32,
                Offset = 0
            });
            attribBuffers.Add(new AttribBuffer<WavefrontVertex>(wfm.Vertices, "vUV", VertexAttribPointerType.Float, 2) {
                Stride = 32,
                Offset = 3*4
            });
            attribBuffers.Add(new AttribBuffer<WavefrontVertex>(wfm.Vertices, "vNormal", VertexAttribPointerType.Float, 3) {
                Stride = 32,
                Offset = 5*4,
                Normalized = true
            });
            
            return new Mesh(attribBuffers, indexBuffers);
        }
    }
}