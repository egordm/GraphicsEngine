using OpenTK;

namespace FruckEngine.Structs
{
    public struct TransformMatrix
    {
        public Matrix4 World;
        public Matrix4 View;
        public Matrix4 Model;

        public TransformMatrix(Matrix4 world, Matrix4 view, Matrix4 model)
        {
            World = world;
            View = view;
            Model = model;
        }
    }
}