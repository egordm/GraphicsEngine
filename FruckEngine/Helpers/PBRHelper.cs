using System;
using FruckEngine.Graphics;
using OpenTK.Graphics.OpenGL;

namespace FruckEngine.Helpers {
    public static class PBRHelper {
        private static FrameBuffer IrradianceBuffer, PrefilterBuffer, BRDFBuffer;
        private static Shader IrradianceShader, PrefilterShader, BRDFShader;
        private static Texture BRDF_LUT = null;

        public static Texture CalculateIrradiance(Texture environment) {
            if (IrradianceBuffer == null) {
                IrradianceShader = Shader.Create("Assets/shaders/cube_project_vs.glsl",
                    "Assets/shaders/pbr/irradiance_convolution_fs.glsl");
                IrradianceShader.AddUniformVar("mView");
                IrradianceShader.AddUniformVar("mProjection");
                IrradianceShader.AddUniformVar("uImage");

                IrradianceBuffer = new FrameBuffer(Constants.IRRADIANCE_TEXTURE_SIZE, Constants.IRRADIANCE_TEXTURE_SIZE);
                IrradianceBuffer.Bind(false, false);
                IrradianceBuffer.AddCubeAttachment("irradiance", PixelType.Float, PixelInternalFormat.Rgb16f,
                    PixelFormat.Rgb, TextureMinFilter.Linear, TextureMagFilter.Linear);
            }
            
            IrradianceBuffer.Bind(true, true);
            IrradianceShader.Use();
            IrradianceShader.SetInt("uImage", 0);
            environment.Activate(0);
            IrradianceBuffer.RenderToCube(IrradianceShader, Constants.CUBEMAP_CAPTURE_VIEWS, "irradiance");
            IrradianceBuffer.UnBind();

            return IrradianceBuffer.GetAttachment("irradiance");
        }

        public static Texture CalculatePrefilter(Texture environment) {
            if (PrefilterBuffer == null) {
                PrefilterShader = Shader.Create("Assets/shaders/cube_project_vs.glsl",
                    "Assets/shaders/pbr/prefilter_fs.glsl");
                PrefilterShader.AddUniformVar("mView");
                PrefilterShader.AddUniformVar("mProjection");
                PrefilterShader.AddUniformVar("uImage");
                PrefilterShader.AddUniformVar("uRoughness");
                
                PrefilterBuffer = new FrameBuffer(Constants.PREFILTER_TEXTURE_SIZE, Constants.PREFILTER_TEXTURE_SIZE);
                PrefilterBuffer.Bind(false, false);
                PrefilterBuffer.AddCubeAttachment("prefilter", PixelType.Float, PixelInternalFormat.Rgb16f,
                    PixelFormat.Rgb, TextureMinFilter.LinearMipmapLinear, TextureMagFilter.Linear);
                PrefilterBuffer.GetAttachment("prefilter").GenMipMaps(true);
            }
            
            PrefilterBuffer.Bind(true, true);
            PrefilterShader.Use();
            PrefilterShader.SetInt("uImage", 0);
            environment.Activate(0);

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
            
            return PrefilterBuffer.GetAttachment("prefilter");
        }

        public static Texture GetBRDFLUT() {
            if (BRDF_LUT == null) {
                BRDFShader = Shader.Create("Assets/shaders/plane_project_vs.glsl", "Assets/shaders/pbr/brdf_fs.glsl");
                
                BRDFBuffer = new FrameBuffer(Constants.BRDF_LUT_SIZE, Constants.BRDF_LUT_SIZE);
                BRDFBuffer.Bind(false, false);
                BRDFBuffer.AddCubeAttachment("brdf", PixelType.Float, PixelInternalFormat.Rg16f,
                    PixelFormat.Rg, TextureMinFilter.Linear, TextureMagFilter.Linear);
                
                BRDFBuffer.Bind(true, true);
                BRDFShader.Use();
                BRDFBuffer.RenderToPlane();
                BRDFBuffer.UnBind();

                BRDF_LUT = BRDFBuffer.GetAttachment("brdf");
            }

            return BRDF_LUT;
        }
    }
}