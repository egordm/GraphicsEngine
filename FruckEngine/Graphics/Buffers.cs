using System;
using System.Runtime.InteropServices;
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

    public interface IIndexBuffer : IUploadableBuffer, IRenderableBuffer {}

    /// <summary>
    /// A standard buffer. Contains a pointer to the buffer and its data
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Buffer<T> : IUploadableBuffer where T : struct
    {
        public int Pointer;
        public T[] Data;
        public int ComponentCount { get; private set; }

        protected Buffer(T[] data, int componentCount)
        {
            Data = data;
            ComponentCount = componentCount;
            Pointer = GL.GenBuffer();
        }

        public abstract void Upload(Shader s);
    }

    /// <summary>
    /// Index buffer wrapper
    /// </summary>
    public class IndexBuffer<T> : Buffer<T>, IIndexBuffer where T : struct
    {
        public PrimitiveType Type { get; private set; }

        public IndexBuffer(T[] data, int componentCount, PrimitiveType type) : base(data, componentCount)
        {
            Type = type;
        }

        public override void Upload(Shader s)
        {
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, Pointer);
            GL.BufferData(BufferTarget.ElementArrayBuffer, (IntPtr) (Data.Length * Generics.SizeOf(typeof(T))), Data,
                BufferUsageHint.StaticDraw);
        }

        public void Render(Shader s)
        {
            GL.BindBuffer( BufferTarget.ElementArrayBuffer, Pointer );
            GL.DrawArrays( Type, 0, Data.Length * ComponentCount );
        }
    }

    /// <summary>
    /// Attribute Buffer wrapper. Can be a vertex buffer or uv buffer or anything else and is supposed to
    /// be an attribute in the shader
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class AttribBuffer<T> : Buffer<T>, IRenderableBuffer where T : struct
    {
        public string AttributeName { private set; get; }
        public VertexAttribPointerType AttribPointerType { private set; get; }
        public bool Normalized { get; set; } = false;
        public int Stride { get; set; } = 0;
        public int Offset { get; set; } = 0;

        public AttribBuffer(T[] data, string attributeName, VertexAttribPointerType attribPointerType, int componentCount) 
            : base(data, componentCount)
        {
            AttributeName = attributeName;
            AttribPointerType = attribPointerType;
        }

        public override void Upload(Shader s)
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, Pointer);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr) (Data.Length * Generics.SizeOf(typeof(T))), Data,
                BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(s.GetVar(AttributeName), ComponentCount, AttribPointerType, Normalized, Stride, Offset);
        }

        public void Render(Shader s)
        {
            GL.EnableVertexAttribArray(s.GetVar(AttributeName));
        }
    }
}