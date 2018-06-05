using System;
using FruckEngine.Helpers;
using FruckEngine.Structs;
using OpenTK.Graphics.OpenGL;

namespace FruckEngine.Graphics
{
    public class VertexBuffer : AttribBuffer<Vertex>
    {
        public VertexBuffer(Vertex[] data)
            : base(data, null, VertexAttribPointerType.Float, 0) { }

        public override void Upload(Shader s)
        {
            GL.EnableClientState(ArrayCap.VertexArray);
            GL.BindBuffer(BufferTarget.ArrayBuffer, Pointer);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr) (Data.Length * Generics.SizeOf(typeof(Vertex))), Data,
                BufferUsageHint.StaticDraw);
            GL.InterleavedArrays(InterleavedArrayFormat.T2fN3fV3f, Generics.SizeOf(typeof(Vertex)), IntPtr.Zero);

            GL.VertexAttribPointer(s.GetVar("vUV"), 2, VertexAttribPointerType.Float, false, 32, 0);
            GL.VertexAttribPointer(s.GetVar("vNormal"), 3, VertexAttribPointerType.Float, true, 32, 2 * 4);
            GL.VertexAttribPointer(s.GetVar("vPosition"), 3, VertexAttribPointerType.Float, false, 32, 5 * 4);
        }

        public override void Render(Shader s)
        {
            GL.EnableVertexAttribArray(s.GetVar("vUV"));
            GL.EnableVertexAttribArray(s.GetVar("vNormal"));
            GL.EnableVertexAttribArray(s.GetVar("vPosition"));
        }
    }
}