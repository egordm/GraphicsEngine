using FruckEngine;
using FruckEngine.Graphics;
using OpenTK;

namespace FruckEngine {
    public static class Constants {
        public const int UNCONSTRUCTED = 0;
        public const int NONEXISTING_ATTRIBUTE = -1;

        public const int GL_FAILURE = 0;

        public const int MAX_LIGHT_COUNT = 32;

        public static readonly string[] TEXTURE_TYPE_NAMES = new string[Texture.TextureTypeCount] {
            "diffuseTex",
            "specularTex",
            "normalTex",
            "ambientTex",
            "shinynessTex"
        };

        public static readonly string[] TEXTURE_TYPE_NAMES_PBR = new string[Texture.TextureTypeCount] {
            "albedoTex",
            "metallicTex",
            "normalTex",
            "aoTex",
            "roughnessTex"
        };

        public const int SSAO_KERNEL_SIZE = 64;
        public const int SSAO_NOISE_SIZE = 4;
        
        public const int BLUR_OFFSET_STEPS = 5;

        public const int IRRADIANCE_TEXTURE_SIZE = 32;
        public const int PREFILTER_TEXTURE_SIZE = 128;
        public const int PREFILTER_MIPMAP_LEVEL_COUNT = 5;
        public const int BRDF_LUT_SIZE = 512;


        public static readonly Matrix4 CUBEMAP_CAPTURE_PROJECTION =
            Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(90f), 1f, 0.1f, 10f);

        public static readonly Matrix4[] CUBEMAP_CAPTURE_VIEWS = {
            Matrix4.LookAt(Vector3.Zero, Vector3.UnitX, -Vector3.UnitY),
            Matrix4.LookAt(Vector3.Zero, -Vector3.UnitX, -Vector3.UnitY),
            Matrix4.LookAt(Vector3.Zero, Vector3.UnitY, Vector3.UnitZ),
            Matrix4.LookAt(Vector3.Zero, -Vector3.UnitY, -Vector3.UnitZ),
            Matrix4.LookAt(Vector3.Zero, Vector3.UnitZ, -Vector3.UnitY),
            Matrix4.LookAt(Vector3.Zero, -Vector3.UnitZ, -Vector3.UnitY)
        };
    }
}