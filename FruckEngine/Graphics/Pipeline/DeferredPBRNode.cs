using FruckEngine.Graphics;
using FruckEngine.Helpers;
using FruckEngine.Objects;
using FruckEngine.Structs;
using OpenTK.Graphics.OpenGL;

namespace FruckEngine.Graphics.Pipeline {
    /// <summary>
    /// Graphics pipeline node to do physically based rendering.
    ///
    /// This has 2 passes. One renders everything to geometry.
    /// Second uses geometry to shade everything. Therefore "deferred shading"
    ///
    /// If you combine multiple shading types dont forget to copy depth buffer from geometry
    /// </summary>
    public class DeferredPBRNode : GraphicsPipelineNode {
        protected Shader GeometryShader, DeferredShader; // Geometry and Shading
        protected FrameBuffer GeometryBuffer, DeferredBuffer;

        public DeferredPBRNode(int width, int height, FrameBuffer deferredBuffer) : base(width, height) {
            DeferredBuffer = deferredBuffer;
            CreateShaders();
            CreateBuffer();
        }

        /// <summary>
        /// Draw everything to geometry.
        /// Albedo, position, normal etc
        /// </summary>
        /// <param name="world"></param>
        /// <returns></returns>
        public FrameBuffer DrawGeometry(World world) {
            // TODO: copy the depth buffer of legacy node beforehand if needed
            GL.Enable(EnableCap.DepthTest);
            GeometryBuffer.Bind(true, false);
            world.Draw(GeometryShader, new DrawProperties(MaterialType.PBR, true));
            GeometryBuffer.UnBind();
            GL.Disable(EnableCap.DepthTest);

            return GeometryBuffer;
        }

        /// <summary>
        /// Shade the shizzle.
        /// This requires some world data
        /// </summary>
        /// <param name="coordSystem"></param>
        /// <param name="world"></param>
        /// <param name="SSAOTex"></param>
        /// <param name="environmentShader"></param>
        public void DrawShading(CoordSystem coordSystem, World world, Texture SSAOTex, Shader environmentShader = null) {
            // Update view position and ambient light
            DeferredShader.Use();
            DeferredShader.SetVec3("uViewPos", world.MainCamera.Position);
            DeferredShader.SetVec3("uAmbientLight", world.Environment.AmbientLight);
            
            // Send light info to the shader
            world.Environment.Sun.Apply(DeferredShader, 0);
            int pointLightCounter = 0;
            foreach (var light in world.Lights) {
                if(light.Type == LightType.PointLight) light.Apply(DeferredShader, pointLightCounter++);
            }
            
            
            // Attach the geometry textures
            GeometryBuffer.GetAttachment("position").Activate(0);
            GeometryBuffer.GetAttachment("normal").Activate(1);
            GeometryBuffer.GetAttachment("color").Activate(2);
            SSAOTex.Activate(3);
            // IBL
            world.Environment.IrradianceMap.Activate(4);
            world.Environment.PrefilteredMap.Activate(5);
            PBRHelper.GetBRDFLUT().Activate(6);
            
            // Render
            Projection.ProjectPlane();

            // Copy depth to shading buffer. First we rendered to plane. Now we need to draw a cube behind everything for environment
            GL.Enable(EnableCap.DepthTest);
            DeferredBuffer.BlitBuffer(GeometryBuffer, ClearBufferMask.DepthBufferBit);
            DeferredBuffer.Bind(false);
            
            // If we have environment draw everything
            if (environmentShader != null) world.Environment.Draw(coordSystem, environmentShader, new DrawProperties());
            GL.Disable(EnableCap.DepthTest);
        }

        public override void Resize(int width, int height) {
            base.Resize(width, height);
            GeometryBuffer.Resize(Width, Height);
        }

        private void CreateShaders() {
            GeometryShader = DefaultShaders.CreateGeometry(true);
            GeometryShader.AddUniformVar("uOffset");

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