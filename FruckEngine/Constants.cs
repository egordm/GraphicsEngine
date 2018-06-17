using FruckEngine;
using FruckEngine.Graphics;

namespace FruckEngine {
    public static class Constants {
        public const int UNCONSTRUCTED = 0;
        public const int NONEXISTING_ATTRIBUTE = -1;
        
        public const int GL_FAILURE = 0;
        
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
    }
}