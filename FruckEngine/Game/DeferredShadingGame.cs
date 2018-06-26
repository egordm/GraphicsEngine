using System;
using FruckEngine.Graphics;
using FruckEngine.Graphics.Pipeline;
using FruckEngine.Helpers;
using FruckEngine.Objects;
using FruckEngine.Structs;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace FruckEngine.Game {
    /// <summary>
    /// Deferred shading game does more than the name suggests.
    /// It uses exclusively PBR (Physically Based rendering) (metallic and roughness instead of shinyness and the bs)
    /// </summary>
    public class DeferredShadingGame : CoolGame {
        // Graphic pipeline nodes
        protected Shader CompositeShader, EnvironmentShader;
        protected FrameBuffer DeferredBuffer;
        protected SSAONode SSAONode;
        protected BlurNode BlurNode;
        protected DOFNode DofNode;
        protected DeferredPBRNode DeferredPBRNode;
        protected GodrayNode GodrayNode;

        // Some flags to toggle effects
        protected bool EnableBloom = true;
        protected bool EnableGodrays = true;
        protected bool EnableColorGrade = true;
        protected bool EnableMyPCIsShit = false;

        public override void Init() {
            base.Init();

            // Register input listeners
            InputHelper.CreateClickListener(Key.O);
            InputHelper.CreateClickListener(Key.I);
            InputHelper.CreateClickListener(Key.J);
            InputHelper.CreateClickListener(Key.K);
            InputHelper.CreateClickListener(Key.L);
            InputHelper.CreateClickListener(Key.G);
            InputHelper.CreateClickListener(Key.N);
            InputHelper.CreateClickListener(Key.M);
            InputHelper.CreateClickListener(Key.B);

            // Add a BRDF lookup table
            PBRHelper.GetBRDFLUT();
            // Create graphic pipeline nodes
            SSAONode = new SSAONode(Width, Height);
            BlurNode = new BlurNode(Width, Height);
            DofNode = new DOFNode(Width, Height);
            GodrayNode = new GodrayNode(Width, Height);

            // -- Deferred Shading Buffer
            // Shading buffer is shared between shading types there fore it is not in PBR node
            DeferredBuffer = new FrameBuffer(Width, Height);
            DeferredBuffer.Bind(false);
            DeferredBuffer.AddAttachment("color", PixelType.Float, PixelInternalFormat.Rgb16f, PixelFormat.Rgb);
            DeferredBuffer.AddAttachment("brightness", PixelType.Float, PixelInternalFormat.Rgb16f, PixelFormat.Rgb);
            // Warning some (old)gpus dont seem to support storing depth in texture and testing for it at the same time
            // So toggle the commented thingie. (DOF wont work though)
            if (!EnableMyPCIsShit) DeferredBuffer.AddDepthAttachment();
            else
                DeferredBuffer.AddRenderBuffer(RenderbufferStorage.DepthComponent,
                    FramebufferAttachment.DepthAttachment);
            DeferredBuffer.DrawBuffers();
            DeferredBuffer.AssertStatus();
            DeferredBuffer.UnBind();

            DeferredPBRNode = new DeferredPBRNode(Width, Height, DeferredBuffer);

            // -- Environment Box
            EnvironmentShader = DefaultShaders.CreateEnvironmentBox();

            // -- Compositing
            CompositeShader = DefaultShaders.CreateComposite();
            CompositeShader.Use();
            CompositeShader.SetInt("uShaded", 0);
            CompositeShader.SetInt("uBloom", 1);
            CompositeShader.SetInt("uGodrays", 2);
            CompositeShader.SetInt("uColorLUT", 3);
            CompositeShader.SetInt("uUI", 4);
        }

        public override void Render(double dt) {
            base.Render(dt);

            if (Scenes.CurrentWorld == null) return;
            // Create a basic coordinate system
            var coordSystem = Scenes.CurrentWorld.InitialCoordSystem();

            // Pass 1 Geometry
            var PBRGeometry = DeferredPBRNode.DrawGeometry(Scenes.CurrentWorld);

            // Pass 2 SSAO
            var SSAOTex = SSAONode.CalculateAO(coordSystem, PBRGeometry.GetAttachment("position"),
                PBRGeometry.GetAttachment("normal"));
            SSAOTex = BlurNode.Apply(SSAOTex, 2);

            // Pass 3 Shading
            DeferredBuffer.Bind(true, false);
            DeferredPBRNode.DrawShading(coordSystem, Scenes.CurrentWorld, SSAOTex, EnvironmentShader);
            DeferredBuffer.UnBind();

            // Pass 4 Blur Bloom
            var bloomTex = EnableBloom
                ? BlurNode.Apply(DeferredBuffer.GetAttachment("brightness"))
                : TextureHelper.GetZeroNull();

            // Pass 4.1 God rays
            Texture godrays;
            if (EnableGodrays) {
                GodrayNode.Clear(PBRGeometry);
                GodrayNode.AddEnvironment(World);
                foreach (var light in World.Lights) {
                    if (light is PointLight) GodrayNode.AddLight(World, (PointLight) light);
                }

                godrays = GodrayNode.GetResult();
            } else {
                godrays = TextureHelper.GetZeroNull();
            }

            // Pass 5 DOF
            Texture dof;
            if (!EnableMyPCIsShit) {
                dof = DofNode.Apply(World, DeferredBuffer.GetAttachment("color"),
                    DeferredBuffer.GetAttachment("depth"));
            } else {
                dof = DeferredBuffer.GetAttachment("color");
            }

            // Pass 6 Final render compositing. Combine everything
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            CompositeShader.Use();
            CompositeShader.SetBool("uApplyBloom", true);
            CompositeShader.SetFloat("uExposure", 1.0f);
            CompositeShader.SetInt("uColorLUTSize", World.Environment.ColorLUT.Height);
            CompositeShader.SetBool("uApplyColorGrade", EnableColorGrade);
            dof.Activate(0);
            bloomTex.Activate(1);
            godrays.Activate(2);
            World.Environment.ColorLUT.Activate(3); // Color cube / color lookup texture
            UI.Texture.Activate(4);

            Projection.ProjectPlane();
        }

        public override void RenderUI(double dt) {
            base.RenderUI(dt);
            UI.Clear(0);
            int y = 8;
            UI.DrawText("Show UI (Toggle: M): " + (EnableColorGrade ? "Enabled" : "Disabled"), 8, y, 0xFDDDDFF);
            y += 22;
            UI.DrawText("SSAO (Toggle: O): " + (SSAONode.Enable ? "Enabled" : "Disabled"), 8, y, 0xFDDDDFF);
            y += 22;
            UI.DrawText("DOF  (Toggle: J, Debug: K): " + (DofNode.Enable ? "Enabled" : "Disabled") + " Debug: " +
                        (DofNode.Debug ? "True" : "False"), 8, y, 0xFDDDDFF);
            y += 22;
            UI.DrawText("Vignette (Toggle: L): " + (DofNode.Vignetting && DofNode.Enable ? "Enabled" : "Disabled"), 8,
                y, 0xFDDDDFF);
            y += 22;
            UI.DrawText("Bloom (Toggle: I): " + (EnableBloom ? "Enabled" : "Disabled"), 8, y, 0xFDDDDFF);
            y += 22;
            UI.DrawText("God rays (Toggle: G): " + (EnableGodrays ? "Enabled" : "Disabled"), 8, y, 0xFDDDDFF);
            y += 22;
            UI.DrawText("Color Grade (Toggle: N): " + (EnableColorGrade ? "Enabled" : "Disabled"), 8, y, 0xFDDDDFF);
            y += 22;
            UI.DrawText("My pc is shit (Toggle: B): " + (EnableMyPCIsShit ? "Enabled" : "Disabled"), 8, y, 0xFDDDDFF);
            y += 22;
            UI.DrawText("Switch between different scenes: Keys 1 till 9 ", 8, y, 0xFFFDD66);

            int frameTime = (int) MathHelper.Clamp(dt * 1000, 0, 999);
            int fps = (int) MathHelper.Clamp(1 / dt, 0, 99);
            string frameCounter = $"{frameTime}ms (FPS: {fps})";
            UI.DrawText(frameCounter, Width - 12 * frameCounter.Length - 8, 8, 0xFDDDDFF);
            string camera =
                $"({World.MainCamera.Position.X:0.00}, {World.MainCamera.Position.Y:0.00}, {World.MainCamera.Position.Z:0.00}) ({(int)World.MainCamera.Pitch}, {(int)World.MainCamera.Yaw})";
            UI.DrawText(camera, Width - 12 * camera.Length - 8, 30, 0xFDDDDFF);
        }

        public override void OnKeyboardUpdate(KeyboardState state) {
            base.OnKeyboardUpdate(state);
            InputHelper.Update(state);

            //Console.WriteLine(World.MainCamera.Direction);

            // Toogle the effects with key buttons
            if (InputHelper.IsClicked(Key.O)) {
                SSAONode.Enable = !SSAONode.Enable;
            }

            if (InputHelper.IsClicked(Key.J)) {
                DofNode.Enable = !DofNode.Enable;
            }

            if (InputHelper.IsClicked(Key.K)) {
                DofNode.Debug = !DofNode.Debug;
            }

            if (InputHelper.IsClicked(Key.L)) {
                DofNode.Vignetting = !DofNode.Vignetting;
            }

            if (InputHelper.IsClicked(Key.I)) {
                EnableBloom = !EnableBloom;
            }

            if (InputHelper.IsClicked(Key.G)) {
                EnableGodrays = !EnableGodrays;
            }

            if (InputHelper.IsClicked(Key.N)) {
                EnableColorGrade = !EnableColorGrade;
            }

            if (InputHelper.IsClicked(Key.M)) {
                EnableUI = !EnableUI;
            }

            if (InputHelper.IsClicked(Key.B)) {
                EnableMyPCIsShit = !EnableMyPCIsShit;
            }
        }
    }
}