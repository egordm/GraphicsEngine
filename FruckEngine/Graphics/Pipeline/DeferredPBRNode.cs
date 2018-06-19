using FruckEngine.Graphics;
using FruckEngine.Helpers;
using FruckEngine.Objects;
using FruckEngine.Structs;
using OpenTK.Graphics.OpenGL;

namespace FruckEngine.Graphics.Pipeline {
    public class DeferredPBRNode : GraphicsPipelineNode {
        protected Shader GeometryShader, DeferredShader;
        protected FrameBuffer GeometryBuffer, DeferredBuffer;

        public DeferredPBRNode(int width, int height, FrameBuffer deferredBuffer) : base(width, height) {
            DeferredBuffer = deferredBuffer;
            CreateShaders();
            CreateBuffer();
        }

        public FrameBuffer DrawGeometry(World world) {
            // TODO: copy the depth buffer of legacy node beforehand if needed
            GeometryBuffer.Bind(true, false);
            world.Draw(GeometryShader, new DrawProperties(MaterialType.PBR, true));
            GeometryBuffer.UnBind();

            return GeometryBuffer;
        }

        public void DrawShading(CoordSystem coordSystem, World world, Texture SSAOTex, Shader environmentShader = null) {
            DeferredShader.Use();
            DeferredShader.SetVec3("uViewPos", world.MainCamera.Position);
            DeferredShader.SetVec3("uAmbientLight", world.Environment.AmbientLight);
            
            world.Environment.DirectionalLight.Apply(DeferredShader, 0);
            
            // TODO: directional light
            int pointLightCounter = 0;
            foreach (var light in world.Lights) {
                if(light.Type == LightType.PointLight) light.Apply(DeferredShader, pointLightCounter++);
            }
            
            GeometryBuffer.GetAttachment("position").Activate(0);
            GeometryBuffer.GetAttachment("normal").Activate(1);
            GeometryBuffer.GetAttachment("color").Activate(2);
            SSAOTex.Activate(3);
            // IBL
            world.Environment.IrradianceMap.Activate(4);
            world.Environment.PrefilteredMap.Activate(5);
            PBRHelper.GetBRDFLUT().Activate(6);
            
            Projection.ProjectPlane();

            if (environmentShader != null) {
                DeferredBuffer.BlitBuffer(GeometryBuffer, ClearBufferMask.DepthBufferBit);
                DeferredBuffer.Bind(false);
                world.Environment.Draw(coordSystem, environmentShader, new DrawProperties());
            }
        }

        public override void Resize(int width, int height) {
            base.Resize(width, height);
            GeometryBuffer.Resize(Width, Height);
        }

        private void CreateShaders() {
            GeometryShader = DefaultShaders.CreateGeometry(true);
            DeferredShader = DefaultShaders.CreateDeferred(true);
            DeferredShader.Use();
            DeferredShader.SetInt("uPositionMetallic", 0);
            DeferredShader.SetInt("uNormalRoughness", 1);
            DeferredShader.SetInt("uAlbedoAO", 2);
            DeferredShader.SetInt("uSSAO", 3);
            DeferredShader.SetInt("uIrradianceMap", 4);
            DeferredShader.SetInt("uPrefilterMap", 5);
            DeferredShader.SetInt("uBrdfLUT", 6);
        }

        private void CreateBuffer() {
            GeometryBuffer = new FrameBuffer(Width, Height);
            GeometryBuffer.Bind(false);
            GeometryBuffer.AddAttachment("position", PixelType.Float, PixelInternalFormat.Rgba16f, PixelFormat.Rgba);
            GeometryBuffer.AddAttachment("normal", PixelType.Float, PixelInternalFormat.Rgba16f, PixelFormat.Rgba);
            GeometryBuffer.AddAttachment("color", PixelType.Float, PixelInternalFormat.Rgba16f, PixelFormat.Rgba);
            GeometryBuffer.DrawBuffers(); // Use with mutliple draw targets. Dont forget to add "layout (location = x)"
            GeometryBuffer.AddRenderBuffer(RenderbufferStorage.DepthComponent, FramebufferAttachment.DepthAttachment);
            GeometryBuffer.AssertStatus();
            GeometryBuffer.UnBind();
        }
    }
}