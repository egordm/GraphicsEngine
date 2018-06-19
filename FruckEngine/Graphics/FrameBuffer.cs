using System;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace FruckEngine.Graphics {
    public class FrameBuffer {
        private int Width, Height;
        public int Pointer, RBOPointer;
        public Dictionary<string, Texture> Attachments = new Dictionary<string, Texture>();
        private List<string> AttachmentOrder = new List<string>();

        public FrameBuffer(int width, int height) {
            Width = width;
            Height = height;

            Pointer = GL.GenFramebuffer();
        }

        public void AddAttachment(string name, PixelType pixelType = PixelType.Float,
            PixelInternalFormat internalFormat = PixelInternalFormat.Rgba16f,
            PixelFormat format = PixelFormat.Rgba, TextureMinFilter filter = TextureMinFilter.Nearest,
            bool depth = false) {
            var texture = new Texture() {
                FilterMin = filter,
                FilterMag = (TextureMagFilter) filter,
                WrapS = TextureWrapMode.ClampToEdge,
                WrapT = TextureWrapMode.ClampToEdge,
                MipMap = false
            };
            texture.Load(Width, Height, internalFormat, format, TextureTarget.Texture2D, pixelType, (IntPtr) 0);
            var attachment = depth
                ? FramebufferAttachment.DepthAttachment
                : FramebufferAttachment.ColorAttachment0 + AttachmentOrder.Count;
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, attachment, TextureTarget.Texture2D,
                texture.Pointer, 0);

            texture.UnBind();
            Attachments.Add(name, texture);
            AttachmentOrder.Add(name);
        }

        public void AddDepthAttachment() {
            AddAttachment("depth", PixelType.Float, PixelInternalFormat.DepthComponent, PixelFormat.DepthComponent,
                TextureMinFilter.Linear, true);
        }

        public void AddCubeAttachment(string name, PixelType pixelType = PixelType.Float,
            PixelInternalFormat internalFormat = PixelInternalFormat.Rgba16f, PixelFormat format = PixelFormat.Rgba,
            TextureMinFilter filterMin = TextureMinFilter.Nearest,
            TextureMagFilter filterMag = TextureMagFilter.Nearest) {
            var texture = new Texture {
                Pointer = GL.GenTexture(),
                Target = TextureTarget.TextureCubeMap,
                PixelType = pixelType,
                InternalFormat = internalFormat,
                Format = format
            };
            texture.Bind();

            for (int i = 0; i < 6; i++) {
                texture.LoadFace(Width, Height, TextureTarget.TextureCubeMapPositiveX + i, IntPtr.Zero);
            }

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

        public void AddRenderBuffer(RenderbufferStorage storage = RenderbufferStorage.DepthComponent,
            FramebufferAttachment attachment = FramebufferAttachment.DepthAttachment) {
            RBOPointer = GL.GenRenderbuffer();
            GL.BindRenderbuffer(RenderbufferTarget.Renderbuffer, RBOPointer);
            GL.RenderbufferStorage(RenderbufferTarget.Renderbuffer, storage, Width, Height);
            GL.FramebufferRenderbuffer(FramebufferTarget.Framebuffer, attachment, RenderbufferTarget.Renderbuffer,
                RBOPointer);
        }

        public Texture GetAttachment(string name) {
            return Attachments[name];
        }

        public void DrawBuffers() {
            var colorAttachments = new DrawBuffersEnum[AttachmentOrder.Count];
            int colorCounter = 0;
            for (int i = 0; i < AttachmentOrder.Count; i++) {
                if (AttachmentOrder[i] == "depth") {
                    //colorAttachments[i] = 
                } else {
                    colorAttachments[i] = DrawBuffersEnum.ColorAttachment0 + colorCounter++;
                }
                
            }
            GL.DrawBuffers(AttachmentOrder.Count, colorAttachments);
        }

        public void Bind(bool clear = true, bool setViewport = false) {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, Pointer);
            if (clear) GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            if (setViewport) GL.Viewport(0, 0, Width, Height);
        }

        public void UnBind() {
            GL.BindFramebuffer(FramebufferTarget.Framebuffer, 0);
        }

        public void Resize(int width, int height) {
            Width = width;
            Height = height;
            foreach (var attachment in Attachments) attachment.Value.Resize(Width, Height);
        }

        public void AssertStatus() {
            if (GL.CheckFramebufferStatus(FramebufferTarget.Framebuffer) != FramebufferErrorCode.FramebufferComplete) {
                throw new Exception("Framebuffer not complete!");
            }
        }

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

        public void BlitBuffer(FrameBuffer frameBuffer, ClearBufferMask bufferMask,
            BlitFramebufferFilter filter = BlitFramebufferFilter.Nearest) {
            GL.BindFramebuffer(FramebufferTarget.ReadFramebuffer, frameBuffer.Pointer);
            GL.BindFramebuffer(FramebufferTarget.DrawFramebuffer, Pointer);
            GL.BlitFramebuffer(0, 0, frameBuffer.Width, frameBuffer.Height, 0, 0, Width, Height, bufferMask, filter);
            UnBind();
        }

        public void RenderToPlane() {
            Projection.ProjectPlane();
        }

        public void RenderToCube(Shader shader, Matrix4[] views, string attachment, int mipmapLevel = 0) {
            shader.SetMat4("mProjection", Constants.CUBEMAP_CAPTURE_PROJECTION);

            for (int i = 0; i < views.Length; ++i) {
                shader.SetMat4("mView", views[i]);

                GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0,
                    TextureTarget.TextureCubeMapPositiveX + i, Attachments[attachment].Pointer, mipmapLevel);
                GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
                Projection.ProjectCube();
            }
        }
    }
}