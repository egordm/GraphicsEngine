using FruckEngine.Graphics;
using OpenTK;

namespace FruckEngine.Structs
{
    public struct CoordSystem
    {
        public Matrix4 Projection;
        public Matrix4 View;
        public Matrix4 Model;

        public CoordSystem(Matrix4 projection, Matrix4 view, Matrix4 model)
        {
            Projection = projection;
            View = view;
            Model = model;
        }

        public void Apply(Shader shader) {
            shader.SetMat4("mProjection", Projection);
            shader.SetMat4("mView", View);
            shader.SetMat4("mModel", Model);
        }
    }
}