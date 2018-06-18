using System;
using System.Collections.Generic;
using FruckEngine.Graphics;
using FruckEngine.Helpers;
using FruckEngine.Structs;
using FruckEngine.Utils;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Buffer = System.Buffer;

namespace FruckEngine.Graphics.Pipeline {
    class UniformDistribution : Random {
        public float GetSample() {
            return (float) Sample();
        }
    }

    public class SSAONode : GraphicsPipelineNode {
        private UniformDistribution Distrib = new UniformDistribution();

        public List<Vector3> Kernel = new List<Vector3>();
        public Texture NoiseTexture { get; private set; } = new Texture();
        private Shader Shader;
        private FrameBuffer FrameBuffer;

        public float KernelRadius = 0.5f;
        public int KernelSize = 64;
        public float Strength = 5;
        public bool Enable = true;
        
        public SSAONode(int width, int height) : base(width, height) {
            GenerateKernel();
            GenerateNoise();
            LoadShader();
            
            FrameBuffer = new FrameBuffer(width, height);
            FrameBuffer.Bind(false, false);
            FrameBuffer.AddAttachment("ao", PixelType.Float, PixelInternalFormat.R16f, PixelFormat.Rgb);
            FrameBuffer.AssertStatus();
            FrameBuffer.UnBind();
        }

        public Texture CalculateAO(CoordSystem coordSystem, Texture positions, Texture normals) {
            if (!Enable) return TextureHelper.GetOneNull();
            FrameBuffer.Bind(true, false);
            Shader.Use();
            Shader.SetFloat("uStrength", Strength);
            Shader.SetInt("uKernelSize", KernelSize);
            Shader.SetFloat("uKernelRadius", KernelRadius);
            Shader.SetVec2("uNoiseScale", Width / 4f, Height / 4f);
            for (int i = 0; i < Constants.SSAO_KERNEL_SIZE; ++i) {
                Shader.SetVec3($"uKernelSamples[{i}]", Kernel[i]);
            }
            coordSystem.Apply(Shader);
            
            positions.Activate(0);
            normals.Activate(1);
            NoiseTexture.Activate(2);
            
            FrameBuffer.RenderToPlane();
            FrameBuffer.UnBind();

            return FrameBuffer.GetAttachment("ao");
        }

        public override void Resize(int width, int height) {
            base.Resize(width, height);
            FrameBuffer.Resize(width, height);
        }

        private void LoadShader() {
            Shader = Shader.Create("Assets/shaders/plane_project_vs.glsl", "Assets/shaders/effects/ssao_fs.glsl");
            Shader.AddUniformVar("mView");
            Shader.AddUniformVar("mProjection");
            Shader.AddUniformVar("uPositions");
            Shader.AddUniformVar("uNormals");
            Shader.AddUniformVar("uTexNoise");
            Shader.AddUniformVar("uNoiseScale");
            Shader.AddUniformVar("uKernelSize");
            Shader.AddUniformVar("uKernelRadius");
            Shader.AddUniformVar("uStrength");
            
            for (int i = 0; i < Constants.SSAO_KERNEL_SIZE; ++i) {
                Shader.AddUniformVar($"uKernelSamples[{i}]");
            }
            
            Shader.Use();
            Shader.SetInt("uPositions", 0);
            Shader.SetInt("uNormals", 1);
            Shader.SetInt("uTexNoise", 2);
            Shader.UnUse();
        }

        private void GenerateKernel() {
            for (int i = 0; i < Constants.SSAO_KERNEL_SIZE; ++i) {
                var sample = new Vector3(Distrib.GetSample() * 2f - 1f, Distrib.GetSample() * 2f - 1f,
                    Distrib.GetSample());
                sample.Normalize();
                sample *= Distrib.GetSample();

                float scale = i / (float) Constants.SSAO_KERNEL_SIZE;
                scale = MathFuncs.Lerp(0.1f, 1.0f, scale * scale);
                sample *= scale;
                Kernel.Add(sample);
            }
        }

        private void GenerateNoise() {
            const int sample_count = Constants.SSAO_NOISE_SIZE * Constants.SSAO_NOISE_SIZE;
            var noise = new List<Vector3>();
            for (int i = 0; i < sample_count; ++i) {
                var sample = new Vector3(Distrib.GetSample() * 2f - 1f, Distrib.GetSample() * 2f - 1f, 0.0f);
                noise.Add(sample);
            }

            NoiseTexture.FilterMin = TextureMinFilter.Nearest;
            NoiseTexture.FilterMag = TextureMagFilter.Nearest;
            NoiseTexture.WrapS = TextureWrapMode.Repeat;
            NoiseTexture.WrapT = TextureWrapMode.Repeat;
            NoiseTexture.MipMap = false;
            TextureHelper.LoadDataIntoTexture(NoiseTexture, Constants.SSAO_NOISE_SIZE, Constants.SSAO_NOISE_SIZE, noise.ToArray());
        }
    }
}