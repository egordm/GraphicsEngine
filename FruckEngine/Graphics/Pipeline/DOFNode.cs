using OpenTK.Graphics.OpenGL;

namespace FruckEngine.Graphics.Pipeline {
    public class DOFNode : GraphicsPipelineNode {
        private FrameBuffer FrameBuffer;
        private Shader Shader;
        
        public DOFNode(int width, int height) : base(width, height) {
            FrameBuffer = new FrameBuffer(Width, Height);
            FrameBuffer.Bind(false, false);
            FrameBuffer.AddAttachment("color", PixelType.Float, PixelInternalFormat.Rgba16, PixelFormat.Rgba);
            FrameBuffer.AssertStatus();
            FrameBuffer.UnBind();
            
            Shader = Shader.Create("Assets/shaders/plane_project_vs.glsl", "Assets/shaders/effects/dof_fs.glsl");
            Shader.AddUniformVar("uColor");
            Shader.AddUniformVar("uDepth");
            Shader.AddUniformVar("uResolution");
            Shader.AddUniformVar("uTexel");
            Shader.AddUniformVar("uFocalLength");
            Shader.AddUniformVar("uFstop");
            Shader.AddUniformVar("uShowFocus");
            
            Shader.Use();
            Shader.SetVec2("uResolution", Width, Height);
            Shader.SetVec2("uTexel", 1f/Width, 1f/Height);
            Shader.SetFloat("uFocalLength", 28); // TODO: get from fov?
            Shader.SetFloat("uFstop", 28/2f); // TODO: get from fov?
            Shader.SetBool("uShowFocus", false);
            Shader.SetInt("uColor", 0);
            Shader.SetInt("uDepth", 1);
            Shader.UnUse();
            
        }

        public Texture Apply(Texture color, Texture depth) {
            FrameBuffer.Bind(true, false);
            Shader.Use();
            
            color.Activate(0);
            depth.Activate(1);
            
            FrameBuffer.RenderToPlane();
            FrameBuffer.UnBind();

            return FrameBuffer.GetAttachment("color");
        }
    }
}