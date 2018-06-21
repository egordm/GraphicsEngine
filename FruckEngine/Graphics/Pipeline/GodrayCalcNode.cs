using FruckEngine.Helpers;
using FruckEngine.Objects;
using FruckEngine.Structs;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace FruckEngine.Graphics.Pipeline {
    public class GodrayCalcNode : GraphicsPipelineNode {
        private FrameBuffer LightFrameBuffer;
        private FrameBuffer[] GodPingPongBuffer;
        private Shader LightShader, EnvironmentShader, GodrayShader;

        private bool PingPongIdx = false;
        private bool First = true;
        
        public GodrayCalcNode(int width, int height) : base(width, height) {
            CreateBuffers();
            CreateShaders();
        }

        public void AddLight(World world, PointLight light) {
            GL.Enable(EnableCap.DepthTest);
            LightFrameBuffer.Bind(false);
            GL.Clear(ClearBufferMask.ColorBufferBit);
            
            LightShader.Use();
            LightShader.SetVec3("uColor", light.Color * 10);
            light.Draw(world.InitialCoordSystem(), LightShader, new DrawProperties(MaterialType.Any, false));

            LightFrameBuffer.UnBind();
            GL.Disable(EnableCap.DepthTest);
            
            CalculateVolume(LightFrameBuffer.GetAttachment("color"), world, light);
        }

        public void AddEnvironment(World world, PointLight light) {
            GL.Enable(EnableCap.DepthTest);
            LightFrameBuffer.Bind(false);
            GL.Clear(ClearBufferMask.ColorBufferBit);

            EnvironmentShader.Use();
            world.Environment.Draw(world.InitialCoordSystem(), EnvironmentShader, new DrawProperties());
            
            LightFrameBuffer.UnBind();
            GL.Disable(EnableCap.DepthTest);
            
            CalculateVolume(LightFrameBuffer.GetAttachment("color"), world, light);
        }

        protected void CalculateVolume(Texture brightness, World world, PointLight light) {
            var coordSystem = world.InitialCoordSystem();
            var positionScreen = new Vector4(light.Position, 1) * coordSystem.Model * coordSystem.View * coordSystem.Projection;
            var screenPos = new Vector2(positionScreen.X / positionScreen.Z, positionScreen.Y / (positionScreen.Z));
            screenPos = (screenPos + Vector2.One) / 2f;
            
            var buffer = GodPingPongBuffer[PingPongIdx ? 1 : 0];
            buffer.Bind(true);
            
            GodrayShader.Use();
            GodrayShader.SetVec2("uLightScreenPos", screenPos);
            /*GodrayShader.SetFloat("uDensity", 0.3f);
            GodrayShader.SetFloat("uBlurWidth", -0.85f);*/
            
            GodrayShader.SetFloat("uDensity", 0.3f);
            GodrayShader.SetFloat("uBlurWidth", -0.9f);
            
            
            
            if (First) {
                TextureHelper.GetZeroNull().Activate(0);
                First = !First;
            } else {
                GodPingPongBuffer[!PingPongIdx ? 1 : 0].GetAttachment("color").Activate(0);
            }
            brightness.Activate(1);
            
            buffer.RenderToPlane();
            buffer.UnBind();

            PingPongIdx = !PingPongIdx;
        }

        public Texture GetResult() {
            return GodPingPongBuffer[!PingPongIdx ? 1 : 0].GetAttachment("color");
        }

        public void Clear(FrameBuffer depthBuffer) {
            LightFrameBuffer.Bind(true);
            LightFrameBuffer.BlitBuffer(depthBuffer, ClearBufferMask.DepthBufferBit);

            foreach (var frameBuffer in GodPingPongBuffer) frameBuffer.Bind(true);

            PingPongIdx = false;
            First = true;
            
            LightFrameBuffer.UnBind();
        }

        private void CreateShaders() {
            LightShader = Shader.Create("Assets/shaders/default_vs.glsl", "Assets/shaders/default_fs.glsl");
            LightShader.AddUniformVar("mModel");
            LightShader.AddUniformVar("mView");
            LightShader.AddUniformVar("mProjection");
            LightShader.AddUniformVar("uColor");
            
            EnvironmentShader = Shader.Create("Assets/shaders/cube_project_infinite_vs.glsl", "Assets/shaders/utils/extract_brightness_cubemap.glsl");
            EnvironmentShader.AddUniformVar("mView");
            EnvironmentShader.AddUniformVar("mProjection");
            EnvironmentShader.AddUniformVar("uImage");
            
            EnvironmentShader.Use();
            EnvironmentShader.SetInt("uImage", 0);
            
            GodrayShader = Shader.Create("Assets/shaders/plane_project_vs.glsl", "Assets/shaders/effects/godray_fs.glsl");
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