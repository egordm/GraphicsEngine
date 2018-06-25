using FruckEngine.Graphics;
using OpenTK.Graphics.OpenGL;

namespace FruckEngine.Graphics.Pipeline {
    /// <summary>
    /// Blur node for the graphics pipeline. It applies gaussian blur to the given texture
    /// </summary>
    public class BlurNode : GraphicsPipelineNode {
        private FrameBuffer[] PingPongBuffer = new FrameBuffer[2]; // Cool name. Used to avoid read & write to same texture
        private Shader Shader;

        public BlurNode(int width, int height) : base(width, height) {
            Width = width;
            Height = height;
            
            // Create a poing pong buffer
            for (int i = 0; i < 2; i++) {
                PingPongBuffer[i] = new FrameBuffer(Width, Height);
                PingPongBuffer[i].Bind(false, false);
                PingPongBuffer[i].AddAttachment("result", PixelType.Float, PixelInternalFormat.Rgb16f, PixelFormat.Rgb);
                PingPongBuffer[i].AssertStatus();
                PingPongBuffer[i].UnBind();
            }
            
            
            // Create texture
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

        /// <summary>
        /// Apply blur to a given texture
        /// </summary>
        /// <param name="source"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public Texture Apply(Texture source, int amount = 10) {
            bool horizontal = true, first_iteration = true;
            Shader.Use();
            // Switch between horizontal and vertical blur. Amount
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
            
            // Give the last drawn to buffer tex as result
            return PingPongBuffer[!horizontal ? 1 : 0].GetAttachment("result");
        }
        
        /// <summary>
        /// Resize
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public override void Resize(int width, int height) {
            base.Resize(width, height);
            foreach (var frameBuffer in PingPongBuffer) {
                frameBuffer.Resize(width, height);
            }
        }
    }
}