using System;
using FruckEngine.Graphics;
using FruckEngine.Graphics.Pipeline;
using FruckEngine.Helpers;
using FruckEngine.Objects;
using FruckEngine.Structs;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace FruckEngine.Game {
    public class DeferredShadingGame : CoolGame {
        protected Shader CompositeShader, EnvironmentShader;
        protected FrameBuffer DeferredBuffer;
        protected SSAONode SSAONode;
        protected BlurNode BlurNode;
        protected DOFNode DofNode;
        protected DeferredPBRNode DeferredPBRNode;
        protected GodrayNode GodrayNode;

        protected bool EnableBloom = true;
        protected bool EnableGodrays = true;
        protected bool EnableColorGrade = true;
        
        public override void Init() {
            base.Init();
            
            InputHelper.CreateClickListener(Key.O);
            InputHelper.CreateClickListener(Key.I);
            InputHelper.CreateClickListener(Key.J);
            InputHelper.CreateClickListener(Key.K);
            InputHelper.CreateClickListener(Key.L);
            InputHelper.CreateClickListener(Key.G);
            InputHelper.CreateClickListener(Key.N);

            PBRHelper.GetBRDFLUT();
            SSAONode = new SSAONode(Width, Height);
            BlurNode = new BlurNode(Width, Height);
            DofNode = new DOFNode(Width, Height);
            GodrayNode = new GodrayNode(Width, Height);

            // -- Deferred Shading Buffer
            DeferredBuffer = new FrameBuffer(Width, Height);
            DeferredBuffer.Bind(false);
            DeferredBuffer.AddAttachment("color", PixelType.Float, PixelInternalFormat.Rgb16f, PixelFormat.Rgb);
            DeferredBuffer.AddAttachment("brightness", PixelType.Float, PixelInternalFormat.Rgb16f, PixelFormat.Rgb);
            DeferredBuffer.AddDepthAttachment(); // Substitutes the frame buffer
            //DeferredBuffer.AddRenderBuffer(RenderbufferStorage.DepthComponent, FramebufferAttachment.DepthAttachment);
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
        }

        public override void Render() {
            if (Scenes.CurrentWorld == null) return;
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
            var bloomTex = EnableBloom ? BlurNode.Apply(DeferredBuffer.GetAttachment("brightness")) : TextureHelper.GetZeroNull();
            
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

            // Pass 4.5 DOF
            var dof = DofNode.Apply(World, DeferredBuffer.GetAttachment("color"), DeferredBuffer.GetAttachment("depth"));
            
            // Pass 5 Final render compositing
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            CompositeShader.Use();
            CompositeShader.SetBool("uApplyBloom", true);
            CompositeShader.SetFloat("uExposure", 1.0f);
            CompositeShader.SetInt("uColorLUTSize", World.Environment.ColorLUT.Height);
            CompositeShader.SetBool("uApplyColorGrade", EnableColorGrade);
            //DeferredBuffer.GetAttachment("depth").Activate(0);
            //DeferredBuffer.GetAttachment("color").Activate(0);
            dof.Activate(0);
            bloomTex.Activate(1);
            godrays.Activate(2);
            World.Environment.ColorLUT.Activate(3);
            
            Projection.ProjectPlane();
        }

        public override void OnKeyboardUpdate(KeyboardState state) {
            base.OnKeyboardUpdate(state);
            InputHelper.Update(state);
            
            //Console.WriteLine(World.MainCamera.Direction);

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
        }
    }
}