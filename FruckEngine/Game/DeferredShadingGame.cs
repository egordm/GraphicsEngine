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
        protected DeferredPBRNode DeferredPBRNode;
        
        public override void Init() {
            base.Init();

            PBRHelper.GetBRDFLUT();
            SSAONode = new SSAONode(Width, Height);
            BlurNode = new BlurNode(Width, Height);

            // -- Deferred Shading Buffer
            DeferredBuffer = new FrameBuffer(Width, Height);
            DeferredBuffer.Bind(false);
            DeferredBuffer.AddAttachment("color", PixelType.Float, PixelInternalFormat.Rgb16f, PixelFormat.Rgb);
            DeferredBuffer.AddAttachment("brightness", PixelType.Float, PixelInternalFormat.Rgb16f, PixelFormat.Rgb);
            DeferredBuffer.DrawBuffers();
            DeferredBuffer.AddRenderBuffer(RenderbufferStorage.DepthComponent, FramebufferAttachment.DepthAttachment);
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
        }

        public override void Render() {
            if (Scenes.currentWorld == null) return;
            var coordSystem = Scenes.currentWorld.InitialCoordSystem();
            
            // Pass 1 Geometry
            var PBRGeometry = DeferredPBRNode.DrawGeometry(Scenes.currentWorld);
            
            // Pass 2 SSAO
            var SSAOTex = SSAONode.CalculateAO(coordSystem, PBRGeometry.GetAttachment("position"),
                PBRGeometry.GetAttachment("normal"));
            
            SSAOTex = BlurNode.Apply(SSAOTex, 2);
            
            // Pass 3 Shading
            DeferredBuffer.Bind(true, false);
            DeferredPBRNode.DrawShading(coordSystem, Scenes.currentWorld, SSAOTex, EnvironmentShader);
            DeferredBuffer.UnBind();
            
            // Pass 4 Blur Bloom
            var bloomTex = BlurNode.Apply(DeferredBuffer.GetAttachment("brightness"));

            // Pass 5 Final render compositing
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            CompositeShader.Use();
            CompositeShader.SetBool("uApplyBloom", true);
            CompositeShader.SetFloat("uExposure", 1.0f);
            DeferredBuffer.GetAttachment("color").Activate(0);
            TextureHelper.GetZeroNull().Activate(1);
            //bloomTex.Activate(1);
            Projection.ProjectPlane();
        }

        private bool PrevOState = false;
        public override void OnKeyboardUpdate(KeyboardState state) {
            base.OnKeyboardUpdate(state);

            if (state[Key.O]) {
                if (!PrevOState) {
                    PrevOState = true;
                    SSAONode.Enable = !SSAONode.Enable;
                }
            } else {
                PrevOState = false;
            }
        }
    }
}