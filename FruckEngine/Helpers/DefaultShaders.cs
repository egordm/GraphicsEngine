using FruckEngine.Graphics;

namespace FruckEngine.Helpers {
    public static class DefaultShaders {
        public static Shader CreateDebugDraw(bool quad) {
            var shader = Shader.Create(
                quad
                    ? "Assets/shaders/debug_draw/flat_draw_quad_vs.glsl"
                    : "Assets/shaders/debug_draw/flat_draw_vs.glsl",
                "Assets/shaders/debug_draw/flat_draw_fs.glsl");
            shader.AddUniformVar("mModel");
            shader.AddUniformVar("mView");
            shader.AddUniformVar("mProjection");
            return shader;
        }

        public static Shader CreateGeometry(bool pbr) {
            var shader = Shader.Create(
                pbr ? "Assets/shaders/pbr/geometry_vs.glsl" : "Assets/shaders/legacy/geometry_vs.glsl",
                pbr ? "Assets/shaders/pbr/geometry_fs.glsl" : "Assets/shaders/legacy/geometry_fs.glsl");

            shader.AddUniformVar("mModel");
            shader.AddUniformVar("mView");
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

        public static Shader CreateDeferred(bool pbr) {
            var shader = Shader.Create("Assets/shaders/plane_project_vs.glsl",
                "Assets/shaders/pbr/deferred_shading_fs.glsl");

            if (pbr) {
                shader.AddUniformVar("uPositionMetallic");
                shader.AddUniformVar("uNormalRoughness");
                shader.AddUniformVar("uAlbedoAO");

                shader.AddUniformVar("uIrradianceMap");
                shader.AddUniformVar("uPrefilterMap");
                shader.AddUniformVar("uBrdfLUT");
            } else {
                // TODO: legacy shaders
            }

            shader.AddUniformVar("uSSAO");
            shader.AddUniformVar("uViewPos");

            shader.AddUniformVar("uAmbientLight");

            shader.AddUniformVar("uPointLightCount");
            for (int i = 0; i < Constants.MAX_LIGHT_COUNT; ++i) {
                string name = $"uPointLights[{i}].";
                shader.AddUniformVar(name + "position");
                shader.AddUniformVar(name + "color");
                shader.AddUniformVar(name + "intensity");
            }

            return shader;
        }

        public static Shader CreateEnvironmentBox() {
            var shader = Shader.Create("Assets/shaders/cube_project_infinite_vs.glsl",
                "Assets/shaders/environment_box_fs.glsl");

            shader.AddUniformVar("mView");
            shader.AddUniformVar("mProjection");
            shader.AddUniformVar("uImage");

            return shader;
        }
    }
}