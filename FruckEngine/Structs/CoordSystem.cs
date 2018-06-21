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
            if(shader.HasVar("mModel")) shader.SetMat4("mModel", Model);
        }

        public Vector2 GetPointOnScreen(Vector3 point, bool translate = true) {
            var positionScreen = new Vector4(point, translate ? 1 : 0) * Model * View * Projection;
            var screenPos = new Vector2(positionScreen.X / positionScreen.Z, positionScreen.Y / (positionScreen.Z));
            return (screenPos + Vector2.One) / 2f;
        }
    }
}