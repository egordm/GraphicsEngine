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
        public List<Object> Children = new List<Object>();
        public bool Broken { get; private set; }

        public Object(Object obj)
        {
            Position = obj.Position;
            Rotation = obj.Rotation;
            Scale = obj.Scale;
            Meshes = obj.Meshes;
            Children = obj.Children;
            Broken = obj.Broken;
        }

        public Object(List<Mesh> meshes) {
            Meshes = meshes;
        }

        public Object(bool broken = false) {
            Meshes = new List<Mesh>();
            Broken = broken;
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
        public Matrix4 GetMatrix(Matrix4 parent)
        {
            var matrix = Matrix4.CreateScale(Scale);
            matrix *= Matrix4.CreateFromQuaternion(Rotation);
            matrix *= Matrix4.CreateTranslation(Position);
            return  matrix * parent;
        }

        protected virtual void PrepareShader(Shader shader) { }

        public virtual void Draw(CoordSystem coordSys, Shader shader, DrawProperties properties) {
            var modelM = GetMatrix(coordSys.Model);
            coordSys.Model = modelM;

            foreach (var child in Children) child.Draw(coordSys, shader, properties);

            if (Meshes.Count > 0) {
                shader.Use();
                coordSys.Apply(shader);
                PrepareShader(shader);

                foreach (var mesh in Meshes) mesh.Draw(shader, properties);
            }
        }
    }
}