using OpenTK;

namespace FruckEngine.Structs
{
    public struct Transformation
    {
        public Matrix4 World;
        public Matrix4 View;
        public Matrix4 Model;
    }
}