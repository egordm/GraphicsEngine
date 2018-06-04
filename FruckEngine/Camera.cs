using OpenTK;

namespace FruckEngine
{
    public class Camera
    {
        public Vector3 Position { get; set; }
        public Quaternion Rotation { get; set; }
        public float Fovy { get; set; }
        public float Aspect { get; set; }
        public float ZNear { get; set; }
        public float ZFar { get; set; }
        
        public Camera() : this(Vector3.Zero, Quaternion.Identity) { }
        
        public Camera(Vector3 position, Quaternion rotation, float fovy = 1.6f, float aspect = 16f/9f, float zNear = .1f, float zFar = 100000000)
        {
            Position = position;
            Rotation = rotation;
            Fovy = fovy;
            Aspect = aspect;
            ZNear = zNear;
            ZFar = zFar;
        }
        
        public Matrix4 GetMatrix()
        {
            var matrix = Matrix4.CreateFromQuaternion(Rotation);
            matrix *= Matrix4.CreateTranslation(Position);
            matrix *= Matrix4.CreatePerspectiveFieldOfView(Fovy, Aspect, ZNear, ZFar);
            return matrix;
        }
        
        /// <summary>
        /// http://slideplayer.com/slide/5356366/17/images/42/OpenGL+Perspective+gluPerspective(fovy,+aspect,+near,+far);.jpg
        /// </summary>
        /// <param name="fovy"></param>
        /// <param name="aspect"></param>
        /// <param name="zNear"></param>
        /// <param name="zFar"></param>
        public void SetFOV(float fovy = 1.6f, float aspect = 16f / 9f, float zNear = .1f, float zFar = 100000000)
        {
            Fovy = fovy;
            Aspect = aspect;
            ZNear = zNear;
            ZFar = zFar;
        }
    }
}