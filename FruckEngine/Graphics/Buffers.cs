using System;
using FruckEngine.Helpers;
using OpenTK.Graphics.OpenGL;


namespace FruckEngine.Graphics
{
    /// <summary>
    /// A uploadable buffer
    /// </summary>
    public interface IUploadableBuffer
    {
        void Upload(Shader s);
    }
    
    /// <summary>
    /// A renderable buffer
    /// </summary>
    public interface IRenderableBuffer
    {
        void Render(Shader s);
    }

    /// <summary>
    /// A standard buffer. Contains a pointer to the buffer and its data
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Buffer<T> : IUploadableBuffer where T : struct
    {
        public int Pointer;
        public T[] Data;

        public Buffer(T[] data)
        {
            Data = data;
            Pointer = GL.GenBuffer();
        }

        public abstract void Upload(Shader s);
    }

    /// <summary>
    /// Index buffer wrapper
    /// </summary>
    public class IndexBuffer : Buffer<uint>
    {
        public IndexBuffer(uint[] data) : base(data) { }

        public override void Upload(Shader s)
        {
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, Pointer);
            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr) (Data.Length * sizeof(uint)), Data,
                BufferUsageHint.StaticDraw);
        }
    }

    /// <summary>
    /// Attribute Buffer wrapper. Can be a vertex buffer or uv buffer or anything else and is supposed to
    /// be an attribute in the shader
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class AttribBuffer<T> : Buffer<T>, IUploadableBuffer, IRenderableBuffer where T : struct
    {
        public string AttributeName { private set; get; }

        public VertexAttribPointerType AttribPointerType { private set; get; }

        public AttribBuffer(T[] data, string attributeName, VertexAttribPointerType attribPointerType) : base(data)
        {
            AttributeName = attributeName;
            AttribPointerType = attribPointerType;
        }

        public override void Upload(Shader s)
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, Pointer);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr) (Data.Length * Generics.SizeOf(typeof(T))), Data,
                BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(s.GetVar(AttributeName), 3, AttribPointerType, false, 0, 0);
        }

        public void Render(Shader s)
        {
            GL.EnableVertexAttribArray(s.GetVar(AttributeName));
        }
    }
}