using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Basic scene object. Aka scene graph node
    /// </summary>
    public class Object : IDrawable
    {
        public Vector3 Position { get; set; } = Vector3.Zero;
        public Quaternion Rotation { get; set; } = Quaternion.Identity;
        public Vector3 Scale { get; set; } = Vector3.One;
        public List<Mesh> Meshes = new List<Mesh>();
        private List<Object> children = new List<Object>();

        public ReadOnlyCollection<Object> Children => children.AsReadOnly();

        public Object() { }

        /// <summary>
        /// A copy constructor.
        /// </summary>
        /// <param name="obj"></param>
        public Object(Object obj)
        {
            Position = obj.Position;
            Rotation = obj.Rotation;
            Scale = obj.Scale;
            Meshes = obj.Meshes;
            children = new List<Object>(obj.Children);
        }

        /// <summary>
        /// Create from a list of meshes
        /// </summary>
        /// <param name="meshes"></param>
        public Object(List<Mesh> meshes) {
            Meshes = meshes;
        }

        /// <summary>
        /// Called when object is added to the scene
        /// </summary>
        public virtual void Init() { }

        /// <summary>
        /// Called every tick if object is in the scene
        /// </summary>
        public virtual void Update(double dt)
        {
            foreach (var child in Children) child.Update(dt);
        }

        /// <summary>
        /// Destroy object and delete things from vram
        /// </summary>
        public virtual void Destroy() {
            foreach (var child in Children) child.Destroy();
            foreach (var mesh in Meshes) mesh.Destroy();
        }

        /// <summary>
        /// Get object space matrix
        /// </summary>
        /// <returns></returns>
        public Matrix4 GetMatrix(Matrix4 parent)
        {
            var matrix = Matrix4.CreateScale(Scale);
            matrix *= Matrix4.CreateFromQuaternion(Rotation);
            matrix *= Matrix4.CreateTranslation(Position);
            return matrix * parent;
        }

        /// <summary>
        /// Set shader peroperties dependent on the object
        /// </summary>
        /// <param name="shader"></param>
        protected virtual void PrepareShader(Shader shader) { }

        public virtual void Draw(CoordSystem coordSys, Shader shader, DrawProperties properties) {
            var modelM = GetMatrix(coordSys.Model);
            coordSys.Model = modelM;

            // Draw children
            foreach (var child in Children) child.Draw(coordSys, shader, properties);

            // Draw self if meshes are present.
            if (Meshes.Count > 0) {
                shader.Use();
                coordSys.Apply(shader);
                PrepareShader(shader);

                foreach (var mesh in Meshes) mesh.Draw(shader, properties);
            }
        }

        /// <summary>
        /// Init child when add to scene
        /// </summary>
        /// <param name="obj"></param>
        public void AddChild(Object obj) {
            children.Add(obj);
            obj.Init();
        }
    }
}