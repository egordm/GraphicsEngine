using FruckEngine.Structs;
using OpenTK.Graphics.OpenGL;


namespace FruckEngine.Graphics
{
    public interface IMaterialShader
    {
        void ApplyMaterial(Material material);
    }
    
    public class MaterialShader : Shader, IMaterialShader
    {
        public MaterialShader() : base("Assets/shaders/vs.glsl", "Assets/shaders/fs.glsl")
        {
            AddAttributeVar("vPosition");
            AddAttributeVar("vUV");
            AddAttributeVar("vNormal");
            AddUniformVar("transform");
            AddUniformVar("pixels");
        }

        public void ApplyMaterial(Material material)
        {
            if (material.Texture != null) {
                SetVar("pixels", 0);
                GL.ActiveTexture(TextureUnit.Texture0);
                GL.BindTexture(TextureTarget.Texture2D, material.Texture.Pointer);
            }
        }
    }
}