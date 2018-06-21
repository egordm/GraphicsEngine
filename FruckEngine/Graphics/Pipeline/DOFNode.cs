using FruckEngine.Objects;
using OpenTK.Graphics.OpenGL;

namespace FruckEngine.Graphics.Pipeline {
    public class DOFNode : GraphicsPipelineNode {
        private FrameBuffer FrameBuffer;
        private Shader Shader;

        public bool Enable = true;
        public bool Debug = false;
        public bool Vignetting = true;

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
            Shader.AddUniformVar("uDebug");
            Shader.AddUniformVar("uEnableVignetting");
            
            Shader.Use();
            Shader.SetVec2("uResolution", Width, Height);
            Shader.SetVec2("uTexel", 1f/Width, 1f/Height);
            Shader.SetInt("uColor", 0);
            Shader.SetInt("uDepth", 1);
            Shader.UnUse();
            
        }

        public Texture Apply(World world, Texture color, Texture depth) {
            if (!Enable) return color;
            
            // TODO: mayby move vignetting to the composer
            FrameBuffer.Bind(true, false);
            Shader.Use();
            Shader.SetFloat("uFocalLength", world.MainCamera.FocalLength); // TODO: get from fov?
            Shader.SetFloat("uFstop", world.MainCamera.FStop); // TODO: get from fov?
            Shader.SetBool("uDebug", Debug);
            Shader.SetBool("uEnableVignetting", Vignetting);
            color.Activate(0);
            depth.Activate(1);
            
            FrameBuffer.RenderToPlane();
            FrameBuffer.UnBind();

            return FrameBuffer.GetAttachment("color");
        }

        public override void Resize(int width, int height) {
            base.Resize(width, height);
            FrameBuffer.Resize(width, height);
            Shader.SetVec2("uResolution", Width, Height);
            Shader.SetVec2("uTexel", 1f/Width, 1f/Height);
        }
    }
}