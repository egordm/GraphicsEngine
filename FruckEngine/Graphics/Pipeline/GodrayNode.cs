using System;
using FruckEngine.Objects;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace FruckEngine.Graphics.Pipeline {
    public class GodrayNode : GraphicsPipelineNode {
        protected Shader Shader;
        protected FrameBuffer FrameBuffer;

        public GodrayNode(int width, int height) : base(width, height) {
            FrameBuffer = new FrameBuffer(Width, Height);
            FrameBuffer.Bind(false, false);
            FrameBuffer.AddAttachment("color", PixelType.Float, PixelInternalFormat.Rgba16, PixelFormat.Rgba);
            FrameBuffer.AssertStatus();
            FrameBuffer.UnBind();
            
            Shader = Shader.Create("Assets/shaders/plane_project_vs.glsl", "Assets/shaders/effects/godray_fs.glsl");
            Shader.AddUniformVar("uBrightness");
            Shader.AddUniformVar("uLightScreenPos");
            Shader.AddUniformVar("uVisible");
            
            Shader.Use();
            Shader.SetInt("uBrightness", 0);
        }

        public Texture Apply(World world, Texture brightness) {
            var coordSystem = world.InitialCoordSystem();
            
            FrameBuffer.Bind(true, false);
            Shader.Use();

            bool enable = false;
            foreach (var light in world.Lights) {
                if (light.Type == LightType.PointLight) {
                    var pointLight = ((PointLight) light);
                    
                    var positionScreen = new Vector4(pointLight.Position, 1) * coordSystem.Model * coordSystem.View * coordSystem.Projection;
                    var screenPos = new Vector2(positionScreen.X / positionScreen.Z, positionScreen.Y / (positionScreen.Z));
                    screenPos = (screenPos + Vector2.One) / 2f;

                    var dotCam = Vector3.Dot(world.MainCamera.Direction, pointLight.Position);
                    if (screenPos.X <= 1 && screenPos.X >= -1 && screenPos.Y <= 1 && screenPos.Y >= -1 && dotCam > 0) {
                       // Console.WriteLine($"Light pos {screenPos}");
                        enable = true;
                        Shader.SetVec2("uLightScreenPos", screenPos);
                    }
                }
            }
            Shader.SetBool("uVisible", enable);
            
            brightness.Activate(1);
            FrameBuffer.RenderToPlane();
            FrameBuffer.UnBind();

            return FrameBuffer.GetAttachment("color");
        }
    }
}