using FruckEngine.Graphics;

namespace FruckEngine.Helpers {
    public static class DefaultShaders {
        public static Shader CreateDebugDraw(bool quad) {
            var shader = Shader.Create(
                quad ? "Assets/shaders/debug_draw/flat_draw_quad_vs.glsl" : "Assets/shaders/debug_draw/flat_draw_vs.glsl",
                "Assets/shaders/debug_draw/flat_draw_fs.glsl");
            shader.AddUniformVar("mView");
            shader.AddUniformVar("mModel");
            shader.AddUniformVar("mProjection");
            return shader;
        }
    }
}