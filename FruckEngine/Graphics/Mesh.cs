using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using FruckEngine.Helpers;
using FruckEngine.Structs;
using OpenTK.Graphics.OpenGL;

namespace FruckEngine.Graphics {
    public class Mesh {
        private int VAO, VBO, EBO;

        public Vertex[] Vertices;
        public uint[] Indices;
        public Material Material;
        public bool isSimple = false;

        public Mesh(Vertex[] vertices, uint[] indices, Material material, bool simple = false) {
            Vertices = vertices;
            Indices = indices;
            Material = material;
            isSimple = simple;
            Init();
        }

        protected virtual void Init() {
            VAO = GL.GenVertexArray();
            VBO = GL.GenBuffer();
            EBO = GL.GenBuffer();


            GL.BindVertexArray(VAO);

            // Upload vertices array object
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr) (Vertices.Length * Mem.SizeOf(typeof(Vertex))), Vertices,
                BufferUsageHint.StaticDraw);

            // Upload indices
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, EBO);
            GL.BufferData(BufferTarget.ElementArrayBuffer, Indices.Length * Mem.SizeOf(typeof(uint)), Indices,
                BufferUsageHint.StaticDraw);
            
            // vertex positions
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, Mem.SizeOf(typeof(Vertex)), Marshal.OffsetOf(typeof(Vertex), "Position"));
            // vertex normals
            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, Mem.SizeOf(typeof(Vertex)), Marshal.OffsetOf(typeof(Vertex), "Normal"));
            // vertex texture coords
            GL.EnableVertexAttribArray(2);
            GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, Mem.SizeOf(typeof(Vertex)), Marshal.OffsetOf(typeof(Vertex), "UV"));
            if (!isSimple) {
                // vertex tangent
                GL.EnableVertexAttribArray(3);
                GL.VertexAttribPointer(3, 3, VertexAttribPointerType.Float, false, Mem.SizeOf(typeof(Vertex)),
                    Marshal.OffsetOf(typeof(Vertex), "Tangent"));
                // vertex bitangent
                GL.EnableVertexAttribArray(4);
                GL.VertexAttribPointer(4, 3, VertexAttribPointerType.Float, false, Mem.SizeOf(typeof(Vertex)),
                    Marshal.OffsetOf(typeof(Vertex), "Bitangent"));
            }
            
            GL.BindVertexArray(0);
        }

        public virtual void Draw(Shader shader) {
            Material.Apply(shader);
            
            GL.BindVertexArray(VAO);
            GL.DrawElements(BeginMode.Triangles, Indices.Length, DrawElementsType.UnsignedInt, 0);
            GL.BindVertexArray(0);
        }
    }
}