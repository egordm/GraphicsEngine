using System.Collections.Generic;
using FruckEngine.Graphics;
using FruckEngine.Structs;
using OpenTK;

namespace FruckEngine.Objects
{
    public interface IDrawable
    {
        /// <summary>
        /// Used to render the object on canvas.
        /// </summary>
        void Draw(CoordSystem coordSys, Shader shader, DrawProperties properties);
    }
    
    /// <summary>
    /// Basic scene object
    /// </summary>
    public class Object : IDrawable
    {
        public Vector3 Position { get; set; } = Vector3.Zero;
        public Quaternion Rotation { get; set; } = Quaternion.Identity;
        public Vector3 Scale { get; set; } = Vector3.One;
        public List<Mesh> Meshes;
        private List<int> childs;
        public Object Parent = null;

        public Object(List<Mesh> meshes) {
            Meshes = meshes;
            childs = new List<int>();
        }

        public Object()
        {
            Meshes = new List<Mesh>();
        }

        /// <summary>
        /// Called when object is added to the scene
        /// </summary>
        public virtual void Init() { }

        /// <summary>
        /// Called every tick if object is in the scene
        /// </summary>
        public virtual void Update(double dt) { }

        /// <summary>
        /// Get object space matrix
        /// </summary>
        /// <returns></returns>
        public Matrix4 GetMatrix()
        {
            var matrix = Matrix4.Identity;
            if (Parent != null)
                matrix = Parent.GetMatrix();

            matrix *= Matrix4.CreateScale(new Vector3(1f / Scale.X, 1f / Scale.Y, 1f / Scale.Z));
            matrix *= Matrix4.CreateTranslation(-Position);
            Matrix4 m = Matrix4.CreateFromQuaternion(Rotation);
            m.Transpose();
            matrix *= m;

            return matrix;
        }

        protected virtual void PrepareShader(Shader shader) { }

        public void Draw(CoordSystem coordSys, Shader shader, DrawProperties properties) {
            var modelM = GetMatrix().Inverted();
            coordSys.Model = modelM;
            shader.Use();
            coordSys.Apply(shader);
            PrepareShader(shader);

            foreach (var mesh in Meshes) mesh.Draw(shader, properties);
        }

        public void AddChild(int c)
        {
            childs.Add(c);
        }
    }
}