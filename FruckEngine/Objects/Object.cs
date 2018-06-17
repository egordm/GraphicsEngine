using FruckEngine.Structs;
using OpenTK;

namespace FruckEngine.Objects
{
    public interface IRenderable
    {
        /// <summary>
        /// Used to render the object on canvas.
        /// </summary>
        void Render(CoordSystem matrix);
    }
    
    /// <summary>
    /// Basic scene object
    /// </summary>
    public class Object
    {
        public virtual Vector3 Position { get; set; }
        public virtual Quaternion Rotation { get; set; }
        public virtual Vector3 Scale { get; set; }

        public Object() : this(Vector3.Zero, Quaternion.Identity, Vector3.One) { }

        public Object(Vector3 position, Quaternion rotation, Vector3 scale)
        {
            Position = position;
            Rotation = rotation;
            Scale = scale;
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
            var matrix = Matrix4.CreateFromQuaternion(Rotation);
            matrix *= Matrix4.CreateScale(Scale);
            matrix *= Matrix4.CreateTranslation(Position);
            return matrix;
        }
    }
}