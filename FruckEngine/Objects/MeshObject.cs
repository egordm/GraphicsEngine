using FruckEngine.Graphics;
using FruckEngine.Structs;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace FruckEngine.Objects
{
    public class MeshObject : Object, IRenderable
    {
        public Mesh Mesh { get; set; } = null;
        public Shader Shader { get; set; } = null;
        
        
        public MeshObject(Mesh mesh, Shader shader) : this(mesh, shader, Vector3.Zero, Quaternion.Identity,
            Vector3.One) { }

        public MeshObject(Mesh mesh, Shader shader, Vector3 position, Quaternion rotation, Vector3 scale)
            : base(position, rotation, scale)
        {
            Mesh = mesh;
            Shader = shader;
        }

        /// <summary>
        /// Used to prepare all the shaders before drawing the object.
        /// Here shader should be enabled and all the uniforms should be set
        /// </summary>
        /// <param name="matrix"></param>
        protected virtual void PrepareShaders(TransformMatrix matrix)
        {
            if (Shader == null) return;

            var transform = matrix.Model * matrix.View * matrix.World;
            Shader.SetVar("transform", ref transform);
        }

        public void Render(TransformMatrix matrix)
        {
            // safety dance
            GL.PushClientAttrib( ClientAttribMask.ClientVertexArrayBit );
            
            matrix.Model = GetMatrix();
            
            if (Mesh != null && Shader != null) {
                Shader.Use();
                PrepareShaders(matrix);
                Mesh.Render(Shader);
            }
            
            // restore previous OpenGL state
            GL.UseProgram( 0 );
            GL.PopClientAttrib();
        }
    }
}