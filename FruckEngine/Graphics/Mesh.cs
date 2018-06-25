using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using FruckEngine.Helpers;
using FruckEngine.Structs;
using FruckEngine.Utils;
using OpenTK.Graphics.OpenGL;

namespace FruckEngine.Graphics {
    /// <summary>
    /// Abstraction for a mesh. Our meshes are not much more than some 3d model data and its material.
    /// Meshes do not allow any other elements than triangle. SO make sure to convert any quads beforehand.
    /// </summary>
    public class Mesh {
        private int VAO, VBO, EBO;

        public Vertex[] Vertices;
        public uint[] Indices;
        public Material Material; // Matrial can be of any shading type. like pbr or legacy. This detemines when object is rendered
        public bool isSimple = false; // If mesh is simple it has no precalculated tangents and bitangents.

        /// <summary>
        /// Create mesh with all the minimum properties to render it. Otherwise no point in making one
        /// </summary>
        /// <param name="vertices"></param>
        /// <param name="indices"></param>
        /// <param name="material"></param>
        /// <param name="simple"></param>
        public Mesh(Vertex[] vertices, uint[] indices, Material material, bool simple = false) {
            Vertices = vertices;
            Indices = indices;
            Material = material;
            isSimple = simple;
            Init();
        }

        /// <summary>
        /// Inits the mesh and uploads all vertices to gpu
        /// </summary>
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
            
            // If is not simple then upload tangents and bitangents
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

        /// <summary>
        /// Draws the mesh if shading mode matches the material or is any.
        /// Also material will be bound of shading is enabled
        /// </summary>
        /// <param name="shader"></param>
        /// <param name="properties"></param>
        public virtual void Draw(Shader shader, DrawProperties properties) {
            if(Material.Type != properties.MaterialType && properties.MaterialType != MaterialType.Any) return;
            if(shader != null && properties.Shade && Material != null) Material.Apply(shader);
            
            // Draw
            GL.BindVertexArray(VAO);
            GL.DrawElements(BeginMode.Triangles, Indices.Length, DrawElementsType.UnsignedInt, 0);
            GL.BindVertexArray(0);
        }

        /// <summary>
        /// Deletes the mesh. Make sure that there are no clones in the world first.
        /// </summary>
        public virtual void Delete() {
            GL.DeleteBuffer(VAO);
            GL.DeleteBuffer(VBO);
            GL.DeleteBuffer(EBO);
        }

        /// <summary>
        /// Get material as physicslly based material. Warning: type is not checked
        /// </summary>
        /// <returns></returns>
        public PBRMaterial AsPBR() {
            return (PBRMaterial) Material;
        }
        
        /// <summary>
        /// Get material as phong based material. Warning: type is not checked
        /// </summary>
        /// <returns></returns>
        public LegacyMaterial AsLegacy() {
            return (LegacyMaterial) Material;
        }
    }
}