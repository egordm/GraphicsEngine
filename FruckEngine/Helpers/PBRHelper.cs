using System;
using FruckEngine.Graphics;
using OpenTK.Graphics.OpenGL;

namespace FruckEngine.Helpers {
    public static class PBRHelper {
        private static FrameBuffer IrradianceBuffer, PrefilterBuffer, BRDFBuffer;
        private static Shader IrradianceShader, PrefilterShader, BRDFShader;
        private static Texture BRDF_LUT = null;

        /// <summary>
        /// Calculates the irradiance map for IBL from environment map.
        /// This is a cubemap texture
        /// </summary>
        /// <param name="environment"></param>
        /// <returns></returns>
        public static Texture CalculateIrradiance(Texture environment) {
            // Initialize the buffer and shader if needed
            if (IrradianceBuffer == null) {
                IrradianceShader = Shader.Create("Assets/shaders/cube_project_vs.glsl",
                    "Assets/shaders/pbr/irradiance_convolution_fs.glsl");
                IrradianceShader.AddUniformVar("mView");
                IrradianceShader.AddUniformVar("mProjection");
                IrradianceShader.AddUniformVar("uImage");

                IrradianceBuffer = new FrameBuffer(Constants.IRRADIANCE_TEXTURE_SIZE, Constants.IRRADIANCE_TEXTURE_SIZE);
                IrradianceBuffer.Bind(false, false);
            }

            // Save the viewport to restore later
            var viewport = new int[4];
            GL.GetInteger(GetPName.Viewport, viewport);
            
            // Apply shader and environemnt map. And render to every side of the cube
            IrradianceBuffer.Bind(true, true);
            IrradianceBuffer.AddCubeAttachment("irradiance", PixelType.Float, PixelInternalFormat.Rgb16f,
                PixelFormat.Rgb, TextureMinFilter.Linear, TextureMagFilter.Linear);
            
            IrradianceShader.Use();
            IrradianceShader.SetInt("uImage", 0);
            environment.Activate(0);
            IrradianceBuffer.RenderToCube(IrradianceShader, Constants.CUBEMAP_CAPTURE_VIEWS, "irradiance");
            IrradianceBuffer.UnBind();
            
            // Restore the viewport
            GL.Viewport(viewport[0], viewport[1], viewport[2], viewport[3]);

            return IrradianceBuffer.Detach("irradiance");
        }

        /// <summary>
        /// Calculates the precalculated map (for reflections) for IBL from environment map.
        /// This is a cubemap texture 
        /// </summary>
        /// <param name="environment"></param>
        /// <returns></returns>
        public static Texture CalculatePrefilter(Texture environment) {
            // Initialize the buffer and shader if needed
            if (PrefilterBuffer == null) {
                PrefilterShader = Shader.Create("Assets/shaders/cube_project_vs.glsl",
                    "Assets/shaders/pbr/prefilter_fs.glsl");
                PrefilterShader.AddUniformVar("mView");
                PrefilterShader.AddUniformVar("mProjection");
                PrefilterShader.AddUniformVar("uImage");
                PrefilterShader.AddUniformVar("uRoughness");
                
                PrefilterBuffer = new FrameBuffer(Constants.PREFILTER_TEXTURE_SIZE, Constants.PREFILTER_TEXTURE_SIZE);
                PrefilterBuffer.Bind(false, false);
            }
            
            // Save the viewport to restore later
            var viewport = new int[4];
            GL.GetInteger(GetPName.Viewport, viewport);
            
            // Apply shader and environemnt map. And render to every side of the cube
            PrefilterBuffer.Bind(true, true);
            PrefilterBuffer.AddCubeAttachment("prefilter", PixelType.Float, PixelInternalFormat.Rgb16f,
                PixelFormat.Rgb, TextureMinFilter.LinearMipmapLinear, TextureMagFilter.Linear);
            PrefilterBuffer.GetAttachment("prefilter").GenMipMaps(true);
            
            PrefilterShader.Use();
            PrefilterShader.SetInt("uImage", 0);
            environment.Activate(0);

            // We also use multiple different mip map levels for different roughnesses to easily switch between them
            int maxMipLevels = Constants.PREFILTER_MIPMAP_LEVEL_COUNT;
            for (int mip = 0; mip < maxMipLevels; ++mip) {
                int mipWidth = (int) (Constants.PREFILTER_TEXTURE_SIZE * Math.Pow(.5f, mip));
                int mipHeight = (int) (Constants.PREFILTER_TEXTURE_SIZE * Math.Pow(.5f, mip));
                GL.Viewport(0, 0, mipWidth, mipHeight);
                
                float roughness = mip / (float)(maxMipLevels - 1);
                PrefilterShader.SetFloat("uRoughness", roughness);
                
                PrefilterBuffer.RenderToCube(PrefilterShader, Constants.CUBEMAP_CAPTURE_VIEWS, "prefilter", mip);
            }
            PrefilterBuffer.UnBind();
            
            // Restore the viewport
            GL.Viewport(viewport[0], viewport[1], viewport[2], viewport[3]);
            
            return PrefilterBuffer.Detach("prefilter");
        }

        /// <summary>
        /// Creates a brdf lookup table so we dont need to calculate brdf every time.
        /// This is  a 2d texture
        /// </summary>
        /// <returns></returns>
        public static Texture GetBRDFLUT() {
            if (BRDF_LUT == null) {
                BRDFShader = Shader.Create("Assets/shaders/plane_project_vs.glsl", "Assets/shaders/pbr/brdf_fs.glsl");
                
                BRDFBuffer = new FrameBuffer(Constants.BRDF_LUT_SIZE, Constants.BRDF_LUT_SIZE);
                BRDFBuffer.Bind(false, false);
                BRDFBuffer.AddAttachment("brdf", PixelType.Float, PixelInternalFormat.Rg16f,
                    PixelFormat.Rg, TextureMinFilter.Linear);
                
                // Save the viewport to restore later
                var viewport = new int[4];
                GL.GetInteger(GetPName.Viewport, viewport);
                
                // render to plane 
                BRDFBuffer.Bind(true, true);
                BRDFShader.Use();
                BRDFBuffer.RenderToPlane();
                BRDFBuffer.UnBind();
                
                // Restore the viewport
                GL.Viewport(viewport[0], viewport[1], viewport[2], viewport[3]);

                BRDF_LUT = BRDFBuffer.GetAttachment("brdf");
            }

            return BRDF_LUT;
        }
    }
}