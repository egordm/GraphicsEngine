using FruckEngine.Graphics;

namespace FruckEngine.Helpers {
    public static class DefaultShaders {
        public static Shader CreateDebugDraw(bool quad) {
            var shader = Shader.Create(
                quad
                    ? "Assets/shaders/debug_draw/flat_draw_quad_vs.glsl"
                    : "Assets/shaders/debug_draw/flat_draw_vs.glsl",
                "Assets/shaders/debug_draw/flat_draw_fs.glsl");
            shader.AddUniformVar("mView");
            shader.AddUniformVar("mModel");
            shader.AddUniformVar("mProjection");
            return shader;
        }

        public static Shader CreateGeometry(bool pbr = true) {
            var shader = Shader.Create(
                pbr ? "Assets/shaders/pbr/geometry_vs.glsl" : "Assets/shaders/legacy/geometry_vs.glsl",
                pbr ? "Assets/shaders/pbr/geometry_fs.glsl" : "Assets/shaders/legacy/geometry_fs.glsl");

            shader.AddUniformVar("mView");
            shader.AddUniformVar("mModel");
            shader.AddUniformVar("mProjection");

            if (pbr) {
                shader.AddUniformVar("uMaterial.albedo");
                shader.AddUniformVar("uMaterial.metallic");
                shader.AddUniformVar("uMaterial.roughness");
                shader.AddUniformVar("uMaterial.albedoTex0");
                shader.AddUniformVar("uMaterial.normalTex0");
                shader.AddUniformVar("uMaterial.metallicTex0");
                shader.AddUniformVar("uMaterial.roughnessTex0");
                shader.AddUniformVar("uMaterial.aoTex0");
            } else {
                // TODO: legacy shaders
            }
            
            return shader;
        }

        public static Shader CreateComposite() {
            var shader = Shader.Create("Assets/shaders/plane_project_vs.glsl", "Assets/shaders/composite_fs.glsl");
            
            shader.AddUniformVar("uShaded");

            return shader;
        }
    }
}