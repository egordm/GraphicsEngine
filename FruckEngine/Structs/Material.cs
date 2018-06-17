using System.Collections.Generic;
using FruckEngine.Graphics;
using OpenTK;

namespace FruckEngine.Structs {
    public abstract class Material {
        public List<Texture> Textures;

        public void Apply(Shader shader) {
            ApplyProperties(shader);

            var textureCounter = new int[Texture.TextureTypeCount];
            uint textureUnit = 0;
            foreach (var texture in Textures) {
                if (ApplyTexture(shader, texture, textureCounter[(int) texture.ShadeType]++, textureUnit,
                    texture.ShadeType)) {
                    textureUnit++;
                }
            }
        }

        protected abstract void ApplyProperties(Shader shader);

        protected abstract bool ApplyTexture(Shader shader, Texture texture, int index, uint textureUnit,
            ShadeType shadeType);
    }

    public class LegacyMaterial : Material {
        public Vector3 Diffuse = Vector3.One;
        public Vector3 Specular = Vector3.Zero;
        public float Shinyness = 16;

        protected override void ApplyProperties(Shader shader) {
            shader.SetVec3("uMaterial.diffuse", Diffuse);
            shader.SetVec3("uMaterial.specular", Specular);
            shader.SetFloat("uMaterial.shinyness", Shinyness);
        }

        protected override bool ApplyTexture(Shader shader, Texture texture, int index, uint textureUnit,
            ShadeType shadeType) {
            string name = "uMaterial." + Constants.TEXTURE_TYPE_NAMES[(int) shadeType] + index;
            if (shader.GetVar(name) == Constants.NONEXISTING_ATTRIBUTE) return false;

            texture.Activate(textureUnit);
            shader.SetInt(name, (int) textureUnit);
            return true;
        }
    }

    public class PBRMaterial : Material {
        public Vector3 Albedo = Vector3.One;
        public float Metallic = 0;
        public float Roughness = 1;

        protected override void ApplyProperties(Shader shader) {
            shader.SetVec3("uMaterial.albedo", Albedo);
            shader.SetFloat("uMaterial.metallic", Metallic);
            shader.SetFloat("uMaterial.roughness", Roughness);
        }
        
        protected override bool ApplyTexture(Shader shader, Texture texture, int index, uint textureUnit,
            ShadeType shadeType) {
            string name = "uMaterial." + Constants.TEXTURE_TYPE_NAMES_PBR[(int) shadeType] + index;
            if (shader.GetVar(name) == Constants.NONEXISTING_ATTRIBUTE) return false;

            texture.Activate(textureUnit);
            shader.SetInt(name, (int) textureUnit);
            return true;
        }
    }
}