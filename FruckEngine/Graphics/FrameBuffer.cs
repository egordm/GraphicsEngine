using System;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace FruckEngine.Graphics {
    /// <summary>
    /// Framebuffer abstraction class
    /// </summary>
    public class FrameBuffer {
        private int Width, Height;
        public int Pointer, RBOPointer;
        public Dictionary<string, Texture> Attachments = new Dictionary<string, Texture>();
        private List<string> AttachmentOrder = new List<string>();

        /// <summary>
        /// Creates framebuffer and sets its base size
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public FrameBuffer(int width, int height) {
            Width = width;
            Height = height;

            Pointer = GL.GenFramebuffer();
        }

        /// <summary>
        /// Adds an attachment(texture) to the framebuffer with given format. It is assumed that its a texture 2d
        /// </summary>
        /// <param name="name"></param>
        /// <param name="pixelType"></param>
        /// <param name="internalFormat"></param>
        /// <param name="format"></param>
        /// <param name="filter"></param>
        /// <param name="depth">Wether attachemnt is a depth attachment.</param>
        public void AddAttachment(string name, PixelType pixelType = PixelType.Float,
            PixelInternalFormat internalFormat = PixelInternalFormat.Rgba16f,
            PixelFormat format = PixelFormat.Rgba, TextureMinFilter filter = TextureMinFilter.Nearest,
            bool depth = false) {
            // Create a blank texure
            var texture = new Texture() {
                FilterMin = filter,
                FilterMag = (TextureMagFilter) filter,
                WrapS = TextureWrapMode.ClampToEdge,
                WrapT = TextureWrapMode.ClampToEdge,
                MipMap = false
            };
            texture.Load(Width, Height, internalFormat, format, TextureTarget.Texture2D, pixelType, (IntPtr) 0);

            // Attach the texture
            var attachment = depth
                ? FramebufferAttachment.DepthAttachment
                : FramebufferAttachment.ColorAttachment0 + AttachmentOrder.Count;
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, attachment, TextureTarget.Texture2D,
                texture.Pointer, 0);

            texture.UnBind();
            Attachments.Add(name, texture);
            AttachmentOrder.Add(name);
        }

        /// <summary>
        /// Adds a depth attachment with default properties
        /// </summary>
        public void AddDepthAttachment() {
            AddAttachment("depth", PixelType.Float, PixelInternalFormat.DepthComponent, PixelFormat.DepthComponent,
                TextureMinFilter.Linear, true);
        }

        /// <summary>
        /// Adds a cube attachment. A cube texture is created with all faces initialized
        /// </summary>
        /// <param name="name"></param>
        /// <param name="pixelType"></param>
        /// <param name="internalFormat"></param>
        /// <param name="format"></param>
        /// <param name="filterMin"></param>
        /// <param name="filterMag"></param>
        public void AddCubeAttachment(string name, PixelType pixelType = PixelType.Float,
            PixelInternalFormat internalFormat = PixelInternalFormat.Rgba16f, PixelFormat format = PixelFormat.Rgba,
            TextureMinFilter filterMin = TextureMinFilter.Nearest,
            TextureMagFilter filterMag = TextureMagFilter.Nearest) {
            // Blank texture
            var texture = new Texture {
                Pointer = GL.GenTexture(),
                Target = TextureTarget.TextureCubeMap,
                PixelType = pixelType,
                InternalFormat = internalFormat,
                Format = format
            };
            texture.Bind();

            // Load every face as blank texture
            for (int i = 0; i < 6; i++) {
                texture.LoadFace(Width, Height, TextureTarget.TextureCubeMapPositiveX + i, IntPtr.Zero);
            }

            // Set wrapping so we have no edges
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapS,
                (int) TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapT,
                (int) TextureWrapMode.ClampToEdge);
            GL.TexParameter(TextureTarget.TextureCubeMap, TextureParameterName.TextureWrapR,
                (int) TextureWrapMode.ClampToEdge);
            texture.SetFilters(filterMin, filterMag);

            texture.UnBind();
            Attachments.Add(name, texture);
            AttachmentOrder.Add(name);
        }

        /// <summary>
        /// Add render buffer. By default used to add a renderbuffer for depth.
        /// Use this if you want to have depth in your frame buffer but dont want to render it to a texture.
        /// Dont forget to blit to copy depth if needed
        /// </summary>
        /// <param name="storage"></param>
        /// <param name="attachment"></param>
        public void AddRenderBuffer(RenderbufferStorage storage = RenderbufferStorage.DepthComponent,
            FramebufferAttachment attachment = FramebufferAttachment.DepthAttachment) {
            RBOPointer = GL.GenRenderbuffer();
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, RBOPointer);
            GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, storage, Width, Height);
            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, attachment, RenderbufferTarget.Renderbuffer,
                RBOPointer);
        }

        /// <summary>
        /// Gets attachment texture
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Texture GetAttachment(string name) {
            return Attachments[name];
        }

        /// <summary>
        /// Name is bit misleading.  It basically tels the buffer which color buffers to use.
        /// Use this if you have more than one attachment after you attached all the textures.
        /// </summary>
        public void DrawBuffers() {
            var colorAttachments = new DrawBuffersEnum[AttachmentOrder.Count];
            int colorCounter = 0;
            for (int i = 0; i < AttachmentOrder.Count; i++) {
                if (AttachmentOrder[i] != "depth") {
                    colorAttachments[i] = DrawBuffersEnum.ColorAttachment0 + colorCounter++;
                }
            }

            GL.DrawBuffers(AttachmentOrder.Count, colorAttachments);
        }

        /// <summary>
        /// Bind the framebuffer. You also have the option to clear and change the viewport.
        /// <b>Warning:</b> clear also clears the depth buffer which might not always be beneficial when doing multiple passes
        /// Dont clear when you are still building the framebuffer or you will get a gl error that framebuffer is not yet complete
        /// </summary>
        /// <param name="clear"></param>
        /// <param name="setViewport"></param>
        public void Bind(bool clear = true, bool setViewport = false) {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, Pointer);
            if (clear) GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            if (setViewport) GL.Viewport(0, 0, Width, Height);
        }

        /// <summary>
        /// Unbind the frame buffer
        /// </summary>
        public void UnBind() {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }

        /// <summary>
        /// Resize framebuffer and all its attachments
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void Resize(int width, int height) {
            Width = width;
            Height = height;
            foreach (var attachment in Attachments) attachment.Value.Resize(Width, Height);
        }

        /// <summary>
        /// Assert if framebuffer is complete
        /// </summary>
        /// <exception cref="Exception"></exception>
        public void AssertStatus() {
            if (GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != FramebufferErrorCode.FramebufferComplete) {
                throw new Exception("Framebuffer not complete!");
            }
        }

        /// <summary>
        /// Detach an attachment. Dont forget to reattach something again
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Texture Detach(string name) {
            var ret = Attachments[name];
            Attachments.Remove(name);
            AttachmentOrder.Remove(name);
            return ret;
        }

        /*public Texture ExtractDepth() {
            var ret = new Texture() {
                FilterMin = TextureMinFilter.Linear,
                FilterMag = TextureMagFilter.Linear,
                WrapS = TextureWrapMode.ClampToEdge,
                WrapT = TextureWrapMode.ClampToEdge,
                MipMap = false
            };

            ret.Load(Width, Height, PixelInternalFormat.DepthComponent, PixelFormat.DepthComponent,
                TextureTarget.Texture2D, PixelType.Float, IntPtr.Zero);
            Bind(false, false);
            ret.Bind();
            GL.CopyTexImage2D(TextureTarget.Texture2D, 0, InternalFormat.DepthComponent, 0, 0, Width, Height, 0);
        }*/

        /// <summary>
        /// Copies things from buffer to buffer. By default it is used to copy the depth
        /// </summary>
        /// <param name="frameBuffer"></param>
        /// <param name="bufferMask"></param>
        /// <param name="filter"></param>
        public void BlitBuffer(FrameBuffer frameBuffer, ClearBufferMask bufferMask = ClearBufferMask.DepthBufferBit,
            BlitFramebufferFilter filter = BlitFramebufferFilter.Nearest) {
            GL.BindFramebuffer(FramebufferTarget.ReadFramebuffer, frameBuffer.Pointer);
            GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, Pointer);
            GL.BlitFramebuffer(0, 0, frameBuffer.Width, frameBuffer.Height, 0, 0, Width, Height, bufferMask, filter);
            UnBind();
        }

        /// <summary>
        /// Shorthand or actually just easy to read. Render shit to a plane. Make sure to have appropriate shaders
        /// </summary>
        public void RenderToPlane() {
            Projection.ProjectPlane();
        }

        /// <summary>
        /// Render to cube map. This means all 6 sides.
        /// </summary>
        /// <param name="shader"></param>
        /// <param name="views"></param>
        /// <param name="attachment"></param>
        /// <param name="mipmapLevel"></param>
        public void RenderToCube(Shader shader, Matrix4[] views, string attachment, int mipmapLevel = 0) {
            shader.SetMat4("mProjection", Constants.CUBEMAP_CAPTURE_PROJECTION); // Projection matrix that fits the side

            for (int i = 0; i < views.Length; ++i) {
                shader.SetMat4("mView", views[i]); // Sets the view matrix that targets the right side

                // Bind the right side as texture and render to it. One can achieve same with quad. but angle matters
                // sometimes
                GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0,
                    TextureTarget.TextureCubeMapPositiveX + i, Attachments[attachment].Pointer, mipmapLevel);
                GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
                Projection.ProjectCube();
            }
        }
    }
}