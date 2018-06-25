using System;
using FruckEngine.Helpers;
using FruckEngine.Objects;
using FruckEngine.Structs;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace FruckEngine.Graphics.Pipeline {
    /// <summary>
    /// Graphics pipeline node for rendering god rays
    ///
    /// Workflow.
    /// 1. Draw light occluded by object or threshold the environment
    /// 2. Blur in direction determined from its position on the screen
    /// 3. Add to final image
    /// </summary>
    public class GodrayNode : GraphicsPipelineNode {
        private FrameBuffer LightFrameBuffer;
        private FrameBuffer[] GodPingPongBuffer;
        private Shader LightShader, EnvironmentShader, GodrayShader;

        private bool PingPongIdx = false;
        private bool First = true;

        public GodrayNode(int width, int height) : base(width, height) {
            CreateBuffers();
            CreateShaders();
        }

        /// <summary>
        /// Draw point light and apply godrays to it
        /// </summary>
        /// <param name="world"></param>
        /// <param name="light"></param>
        public void AddLight(World world, PointLight light) {
            if (Vector3.Dot(world.MainCamera.Direction, light.Position - world.MainCamera.Position) < 0) return;
            if (!light.HasGodRays) return;

            // Draw light only
            GL.Enable(EnableCap.DepthTest);
            LightFrameBuffer.Bind(false);
            GL.Clear(ClearBufferMask.ColorBufferBit);

            LightShader.Use();
            LightShader.SetVec3("uColor", light.Color * 10);
            LightShader.SetBool("uInfiniteFar", false);
            light.Draw(world.InitialCoordSystem(), LightShader, new DrawProperties(MaterialType.Any, false));

            LightFrameBuffer.UnBind();
            GL.Disable(EnableCap.DepthTest);

            // Calculate god ray volume
            var screenPos = world.InitialCoordSystem().GetPointOnScreen(light.Position, true);
            CalculateVolume(LightFrameBuffer.GetAttachment("color"), world, screenPos, light.Density, light.BlurWidth);
        }

        /// <summary>
        /// Create god rays from environment texture and sun position
        /// </summary>
        /// <param name="world"></param>
        public void AddEnvironment(World world) {
            if (Vector3.Dot(world.MainCamera.Direction, world.Environment.Sun.Position) < 0) return;
            if (!world.Environment.Sun.HasGodRays) return;

            // Draw environment and threshold it
            GL.Enable(EnableCap.DepthTest);
            LightFrameBuffer.Bind(false);
            GL.Clear(ClearBufferMask.ColorBufferBit);

            EnvironmentShader.Use();
            world.Environment.Draw(world.InitialCoordSystem(), EnvironmentShader, new DrawProperties());

            LightFrameBuffer.UnBind();
            GL.Disable(EnableCap.DepthTest);
            GL.DepthFunc(DepthFunction.Less);

            // Calculate god ray volume
            var screenPos = world.InitialCoordSystem().GetPointOnScreen(-world.Environment.Sun.Position, false);
            CalculateVolume(LightFrameBuffer.GetAttachment("color"), world, screenPos, world.Environment.Sun.Density,
                world.Environment.Sun.BlurWidth);
        }

        protected void CalculateVolume(Texture brightness, World world, Vector2 lightPos, float density = 0.3f,
            float blurWidth = 0.9f) {
            var buffer = GodPingPongBuffer[PingPongIdx ? 1 : 0];
            buffer.Bind(true);
            
            GodrayShader.Use();
            GodrayShader.SetVec2("uLightScreenPos", lightPos);
            GodrayShader.SetFloat("uDensity", density);
            GodrayShader.SetFloat("uBlurWidth", -blurWidth);

            // We use a ping pong buffer to concat all the godrays from different lights together
            if (First) {
                TextureHelper.GetZeroNull().Activate(0);
                First = !First;
            } else {
                GodPingPongBuffer[!PingPongIdx ? 1 : 0].GetAttachment("color").Activate(0);
            }
            brightness.Activate(1);

            buffer.RenderToPlane(); // Draw
            buffer.UnBind();

            PingPongIdx = !PingPongIdx;
        }

        /// <summary>
        /// Get the result of all concatenated god rays from ping pong buffer
        /// </summary>
        /// <returns></returns>
        public Texture GetResult() {
            return GodPingPongBuffer[!PingPongIdx ? 1 : 0].GetAttachment("color");
        }

        /// <summary>
        /// Clear all buffers and copy the depth to light frame buffer
        /// </summary>
        /// <param name="depthBuffer"></param>
        public void Clear(FrameBuffer depthBuffer) {
            LightFrameBuffer.Bind(true);
            LightFrameBuffer.BlitBuffer(depthBuffer, ClearBufferMask.DepthBufferBit);

            foreach (var frameBuffer in GodPingPongBuffer) frameBuffer.Bind(true);

            PingPongIdx = false;
            First = true;

            LightFrameBuffer.UnBind();
        }
        
        // Create all the shizzle

        private void CreateShaders() {
            LightShader = Shader.Create("Assets/shaders/default_vs.glsl", "Assets/shaders/default_fs.glsl");
            LightShader.AddUniformVar("mModel");
            LightShader.AddUniformVar("mView");
            LightShader.AddUniformVar("mProjection");
            LightShader.AddUniformVar("uColor");
            LightShader.AddUniformVar("uInfiniteFar");

            EnvironmentShader = Shader.Create("Assets/shaders/cube_project_infinite_vs.glsl",
                "Assets/shaders/utils/extract_brightness_cubemap.glsl");
            EnvironmentShader.AddUniformVar("mView");
            EnvironmentShader.AddUniformVar("mProjection");
            EnvironmentShader.AddUniformVar("uImage");

            EnvironmentShader.Use();
            EnvironmentShader.SetInt("uImage", 0);

            GodrayShader = Shader.Create("Assets/shaders/plane_project_vs.glsl",
                "Assets/shaders/effects/godray_fs.glsl");
            GodrayShader.AddUniformVar("uColor");
            GodrayShader.AddUniformVar("uBrightness");
            GodrayShader.AddUniformVar("uLightScreenPos");
            GodrayShader.AddUniformVar("uDensity");
            GodrayShader.AddUniformVar("uBlurWidth");

            GodrayShader.Use();
            GodrayShader.SetInt("uColor", 0);
            GodrayShader.SetInt("uBrightness", 1);
        }

        private void CreateBuffers() {
            LightFrameBuffer = new FrameBuffer(Width, Height);
            LightFrameBuffer.Bind(false);
            LightFrameBuffer.AddAttachment("color", PixelType.Float, PixelInternalFormat.Rgba16f, PixelFormat.Rgba);
            LightFrameBuffer.AddRenderBuffer(RenderbufferStorage.DepthComponent, FramebufferAttachment.DepthAttachment);
            LightFrameBuffer.AssertStatus();
            LightFrameBuffer.UnBind();

            GodPingPongBuffer = new FrameBuffer[2];
            for (int i = 0; i < 2; i++) {
                var buffer = new FrameBuffer(Width, Height);
                buffer.Bind(false);
                buffer.AddAttachment("color", PixelType.Float, PixelInternalFormat.Rgba16f, PixelFormat.Rgba);
                buffer.AssertStatus();
                buffer.UnBind();
                GodPingPongBuffer[i] = buffer;
            }
        }
    }
}