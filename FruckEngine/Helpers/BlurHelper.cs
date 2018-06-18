using FruckEngine.Graphics;
using OpenTK.Graphics.OpenGL;

namespace FruckEngine.Helpers {
    public class BlurHelper {
        private int Width, Height;
        private FrameBuffer[] PingPongBuffer = new FrameBuffer[2];
        private Shader Shader;

        public BlurHelper(int width, int height) {
            Width = width;
            Height = height;
            
            for (int i = 0; i < 2; i++) {
                PingPongBuffer[i] = new FrameBuffer(Width, Height);
                PingPongBuffer[i].Bind(false, false);
                PingPongBuffer[i].AddAttachment("result", PixelType.Float, PixelInternalFormat.Rgb16f, PixelFormat.Rgb);
                PingPongBuffer[i].AssertStatus();
                PingPongBuffer[i].UnBind();
            }
            
            Shader = Shader.Create("Assets/shaders/plane_project_vs.glsl", "Assets/shaders/effects/blur_fs.glsl");
            Shader.AddUniformVar("uImage");
            Shader.AddUniformVar("uHorizontal");
            for(int i = 0; i < Constants.BLUR_OFFSET_STEPS; ++i) {
                Shader.AddUniformVar($"uWeight[{i}]");
            }
            
            Shader.Use();
            Shader.SetInt("uImage", 0);
            Shader.UnUse();
        }

        public Texture Apply(Texture source, int amount = 10) {
            bool horizontal = true, first_iteration = true;
            Shader.Use();
            for (int i = 0; i < amount; ++i) { // This horizontal thingy worked better in c++ :(
                PingPongBuffer[horizontal ? 1 : 0].Bind(true, false);
                Shader.SetBool("uHorizontal", horizontal);
                
                if(first_iteration) source.Activate(0);
                else PingPongBuffer[!horizontal ? 1 : 0].GetAttachment("result").Activate(0);
                
                Projection.ProjectPlane();
                
                horizontal = !horizontal;
                if(first_iteration) first_iteration = false;
            }
            PingPongBuffer[0].UnBind();

            return PingPongBuffer[!horizontal ? 1 : 0].GetAttachment("result");
        }
        
        public void Resize(int width, int height) {
            Width = width;
            Height = height;
            foreach (var frameBuffer in PingPongBuffer) {
                frameBuffer.Resize(width, height);
            }
        }
    }
}