using System.Collections.Generic;
using FruckEngine.Graphics;
using FruckEngine.Helpers;
using OpenTK;

namespace FruckEngine.Structs {
    public enum MaterialType {
        Any,
        Legacy,
        PBR,
    }
    
    /// <summary>
    /// Matrial class has textures and is base for shading specific matrials
    /// </summary>
    public abstract class Material {
        public string Name = "";
        public List<string> Tags = new List<string>();
        public List<Texture> Textures = new List<Texture>();
        public MaterialType Type;

        protected Material(MaterialType type) {
            Type = type;
        }

        public void Destroy() {
            foreach (var texture in Textures) {
                AssimpLoadHelper.UncacheTexture(texture);
                texture.Destroy();
            }
        }

        /// <summary>
        /// Apply material to the shader. (set the uniforms etc)
        /// </summary>
        /// <param name="shader"></param>
        public void Apply(Shader shader) {
            ApplyProperties(shader);
            
            // Push all textures to the material 
            var textureCounter = new int[Texture.TextureTypeCount];
            uint textureUnit = 0; // Texture unit counter
            foreach (var texture in Textures) {
                if (ApplyTexture(shader, texture, textureCounter[(int) texture.ShadeType]++, textureUnit,
                    texture.ShadeType)) {
                    textureUnit++;
                }
            }
            
            // If some textures are not provided. We replace them by null textures which are used to multiply
            // The real properties.
            // This way we can use one shader for textured and untextured materials and everything inbetween.
            if (textureCounter[(int) ShadeType.TEXTURE_TYPE_ALBEDO] == 0) {
                if (ApplyTexture(shader, TextureHelper.GetOneNull(), 0, textureUnit, ShadeType.TEXTURE_TYPE_ALBEDO)) {
                    textureUnit++;
                }
            }
            
            if (textureCounter[(int) ShadeType.TEXTURE_TYPE_METALLIC] == 0) {
                if (ApplyTexture(shader, TextureHelper.GetOneNull(), 0, textureUnit, ShadeType.TEXTURE_TYPE_METALLIC)) {
                    textureUnit++;
                }
            }
            
            if (textureCounter[(int) ShadeType.TEXTURE_TYPE_NORMAL] == 0) {
                if (ApplyTexture(shader, TextureHelper.GetNormalNull(), 0, textureUnit, ShadeType.TEXTURE_TYPE_NORMAL)) {
                    textureUnit++;
                }
            }
            
            if (textureCounter[(int) ShadeType.TEXTURE_TYPE_AO] == 0) {
                if (ApplyTexture(shader, TextureHelper.GetOneNull(), 0, textureUnit, ShadeType.TEXTURE_TYPE_AO)) {
                    textureUnit++;
                }
            }
            
            if (textureCounter[(int) ShadeType.TEXTURE_TYPE_ROUGHNESS] == 0) {
                if (ApplyTexture(shader, TextureHelper.GetOneNull(), 0, textureUnit, ShadeType.TEXTURE_TYPE_ROUGHNESS)) {
                    textureUnit++;
                }
            }
        }

        /// <summary>
        /// Apply shading specific properties
        /// </summary>
        /// <param name="shader"></param>
        protected abstract void ApplyProperties(Shader shader);

        /// <summary>
        /// Apply texture since if different shading types textures have different naming
        /// </summary>
        /// <param name="shader"></param>
        /// <param name="texture"></param>
        /// <param name="index"></param>
        /// <param name="textureUnit"></param>
        /// <param name="shadeType"></param>
        /// <returns></returns>
        protected abstract bool ApplyTexture(Shader shader, Texture texture, int index, uint textureUnit,
            ShadeType shadeType);
    }

    /// <summary>
    /// Legacy material represents the old Phong shading model :P
    /// </summary>
    public class LegacyMaterial : Material {
        public Vector3 Diffuse = Vector3.One;
        public Vector3 Specular = Vector3.Zero;
        public float Shinyness = 16;

        public LegacyMaterial() : base(MaterialType.Legacy) { }

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

    /// <summary>
    /// PBR Material represents the modern physically based rendering shading model
    /// </summary>
    public class PBRMaterial : Material {
        public Vector3 Albedo = Vector3.One;
        public float Metallic = 0;
        public float Roughness = 1;

        public PBRMaterial() : base(MaterialType.PBR) { }

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