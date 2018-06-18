using System;
using FruckEngine.Graphics;
using FruckEngine.Helpers;
using FruckEngine.Structs;
using OpenTK.Graphics.OpenGL;

namespace FruckEngine.Game {
    public class DeferredShadingGame : CoolGame {
        protected Shader GeometryShader, CompositeShader;
        protected FrameBuffer GeometryBuffer;
        
        
        public override void Init() {
            base.Init();
            // TODO: we only support pbr right now
            
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

            CompositeShader = DefaultShaders.CreateComposite();
        }

        public override void Render() {
            
            // Pass 1 Geometry
            GeometryBuffer.Bind(true, false);
            World.Draw(GeometryShader, new DrawProperties(MaterialType.PBR, true));
            GeometryBuffer.UnBind();
            
            
            
            // Pass 5 Final render compositing
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            CompositeShader.Use();
            CompositeShader.SetInt("uShaded", 0);
            GeometryBuffer.GetAttachment("color").Activate(0);
            Projection.ProjectPlane();
        }
    }
}