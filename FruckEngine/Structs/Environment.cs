using FruckEngine.Graphics;
using FruckEngine.Objects;
using FruckEngine.Structs;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace FruckEngine.Structs {
    public class Environment : IDrawable {
        public Texture Texture { get; set; } = new Texture();
        public DirectionalLight DirectionalLight = new DirectionalLight(new Vector3(0.0f, -1, 0.5f), new Vector3(0.839f, 0.925f, 1), 0.2f);
        public Vector3 AmbientLight = new Vector3(0.1f);

        public void Draw(CoordSystem coordSys, Shader shader, MaterialType materialType) {
            if(!Texture.IsLoaded()) return;

            // If we are not we can convert. Right?
            if (Texture.Target != TextureTarget.TextureCubeMap) return;
            
            GL.DepthFunc(DepthFunction.Lequal);
            shader.Use();
            coordSys.Apply(shader);
            shader.SetInt("uImage", 0);
            
            Projection.ProjectCube();
            
            shader.UnUse();
            GL.DepthFunc(DepthFunction.Less);
        }
    }
}