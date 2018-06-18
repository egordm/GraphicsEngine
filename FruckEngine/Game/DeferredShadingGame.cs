using System;
using FruckEngine.Graphics;
using FruckEngine.Helpers;
using FruckEngine.Objects;
using FruckEngine.Structs;
using OpenTK.Graphics.OpenGL;

namespace FruckEngine.Game {
    public class DeferredShadingGame : CoolGame {
        protected Shader GeometryShader, DeferredShader, CompositeShader, EnvironmentShader;
        protected FrameBuffer GeometryBuffer, DeferredBuffer;
        
        
        public override void Init() {
            base.Init();

            PBRHelper.GetBRDFLUT();
            
            // -- Geometry Buffer
            GeometryShader = DefaultShaders.CreateGeometry(true);
            
            GeometryBuffer = new FrameBuffer(Width, Height);
            GeometryBuffer.Bind(false);
            GeometryBuffer.AddAttachment("position", PixelType.Float, PixelInternalFormat.Rgba16f, PixelFormat.Rgba);
            GeometryBuffer.AddAttachment("normal", PixelType.Float, PixelInternalFormat.Rgba16f, PixelFormat.Rgba);
            GeometryBuffer.AddAttachment("color", PixelType.Float, PixelInternalFormat.Rgba16f, PixelFormat.Rgba);
            GeometryBuffer.DrawBuffers(); // Use with mutliple draw targets. Dont forget to add "layout (location = x)"
            GeometryBuffer.AddRenderBuffer(RenderbufferStorage.DepthComponent, FramebufferAttachment.DepthAttachment);
            GeometryBuffer.AssertStatus();
            GeometryBuffer.UnBind();

            // -- Deferred Shading Buffer
            DeferredBuffer = new FrameBuffer(Width, Height);
            DeferredBuffer.Bind(false);
            DeferredBuffer.AddAttachment("color", PixelType.Float, PixelInternalFormat.Rgb16f, PixelFormat.Rgb);
            //DeferredBuffer.AddAttachment("brightness", PixelType.Float, PixelInternalFormat.Rgb16f, PixelFormat.Rgb);
            //DeferredBuffer.DrawBuffers();
            DeferredBuffer.AddRenderBuffer(RenderbufferStorage.DepthComponent, FramebufferAttachment.DepthAttachment);
            DeferredBuffer.AssertStatus();
            DeferredBuffer.UnBind();

            DeferredShader = DefaultShaders.CreateDeferred(true);
            DeferredShader.Use();
            DeferredShader.SetInt("uPositionMetallic", 0);
            DeferredShader.SetInt("uNormalRoughness", 1);
            DeferredShader.SetInt("uAlbedoAO", 2);
            DeferredShader.SetInt("uSSAO", 3);
            DeferredShader.SetInt("uIrradianceMap", 4);
            DeferredShader.SetInt("uPrefilterMap", 5);
            DeferredShader.SetInt("uBrdfLUT", 6);
            
            // -- Environment Box
            EnvironmentShader = DefaultShaders.CreateEnvironmentBox();
            
            // -- Compositing
            CompositeShader = DefaultShaders.CreateComposite();
            CompositeShader.Use();
            CompositeShader.SetInt("uShaded", 0);
        }

        public override void Render() {
            // Pass 1 Geometry
            GeometryBuffer.Bind(true, false);
            World.Draw(GeometryShader, new DrawProperties(MaterialType.PBR, true));
            GeometryBuffer.UnBind();
            
            // Pass 3 Shading
            DeferredBuffer.Bind(true, false);
            
            DeferredShader.Use();
            DeferredShader.SetVec3("uViewPos", World.MainCamera.Position);
            DeferredShader.SetVec3("uAmbientLight", World.Environment.AmbientLight);
            // TODO: directional light
            int pointLightCounter = 0;
            foreach (var light in World.Lights) {
                if(light.Type == LightType.PointLight) light.Apply(DeferredShader, pointLightCounter++);
            }
            GeometryBuffer.GetAttachment("position").Activate(0);
            GeometryBuffer.GetAttachment("normal").Activate(1);
            GeometryBuffer.GetAttachment("color").Activate(2);
            TextureHelper.GetOneNull().Activate(3); // TODO: for now now SSAO
            // IBL
            World.Environment.IrradianceMap.Activate(4);
            World.Environment.PrefilteredMap.Activate(5);
            PBRHelper.GetBRDFLUT().Activate(6);
            
            Projection.ProjectPlane();
            
            DeferredBuffer.BlitBuffer(GeometryBuffer, ClearBufferMask.DepthBufferBit);
            DeferredBuffer.Bind(false);
            World.Environment.Draw(World.InitialCoordSystem(), EnvironmentShader, new DrawProperties());
            
            DeferredBuffer.UnBind();

            // Pass 5 Final render compositing
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            CompositeShader.Use();
            DeferredBuffer.GetAttachment("color").Activate(0);
            Projection.ProjectPlane();
        }
    }
}