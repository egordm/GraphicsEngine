using FruckEngine.Graphics;
using FruckEngine.Helpers;
using FruckEngine.Objects;
using FruckEngine.Structs;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace FruckEngine.Structs {
    /// <summary>
    /// Environment holds all data about current environment in the world
    /// </summary>
    public class Environment : IDrawable {
        /// <summary>
        /// Environment map
        /// </summary>
        public Texture Texture { get; private set; } = new Texture();
        /// <summary>
        /// Precalculated irradiance map for the world. Needed for PBR and IBL
        /// </summary>
        public Texture IrradianceMap { get; private set; } = null;
        /// <summary>
        /// Precalculated prefilter map for the world. Needed for PBR and IBL
        /// </summary>
        public Texture PrefilteredMap { get; private set; } = null;

        /// <summary>
        /// Color lookup table for color grading. Everything orange/blue right?
        /// </summary>
        public Texture ColorLUT = TextureHelper.GetStandardColorLUTTexture();

        /// <summary>
        /// Sun is a directional light here. But a unusual one. This one has a position which is normalized
        /// als shows the direction the sun is at which can be rendered at behind everything at that direction
        /// </summary>
        public Sun Sun = new Sun(new Vector3(0.0f, -1, 0.5f), new Vector3(0.839f, 0.925f, 1), 0.2f);
        /// <summary>
        /// Ambient light is redunant for PBR since we use irradiance map but yep why not. 
        /// </summary>
        public Vector3 AmbientLight = Vector3.One;

        /// <summary>
        /// Set environment map and precalculate everything
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="generateIBL"></param>
        public void SetTexture(Texture texture, bool generateIBL) {
            Texture = texture;
            IrradianceMap = null;
            PrefilteredMap = null;

            if (generateIBL) {
                IrradianceMap = PBRHelper.CalculateIrradiance(texture);
                PrefilteredMap = PBRHelper.CalculatePrefilter(texture);
            }
        }

        /// <summary>
        /// Draw the environment.
        /// </summary>
        /// <param name="coordSys"></param>
        /// <param name="shader"></param>
        /// <param name="properties"></param>
        public void Draw(CoordSystem coordSys, Shader shader, DrawProperties properties) {
            if (!Texture.IsLoaded()) return;

            // If we are not we can convert. Right?
            if (Texture.Target != TextureTarget.TextureCubeMap) return;

            GL.DepthFunc(DepthFunction.Lequal);
            shader.Use();
            coordSys.Apply(shader);
            shader.SetInt("uImage", 0);
            Texture.Activate(0);

            Projection.ProjectCube();
            
            // TODO: also draw the sun?

            shader.UnUse();
            GL.DepthFunc(DepthFunction.Less);
        }
    }
}