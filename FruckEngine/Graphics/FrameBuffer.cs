using System;
using System.Collections.Generic;
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
            PixelFormat format = PixelFormat.Rgba, TextureMinFilter filter = TextureMinFilter.Nearest) {
            var texture = new Texture() {
                FilterMin = filter,
                FilterMag = (TextureMagFilter) filter,
                WrapS = TextureWrapMode.ClampToEdge,
                WrapT = TextureWrapMode.ClampToEdge,
                MipMap = false
            };
            texture.Load(Width, Height, internalFormat, format, TextureTarget.Texture2D, pixelType, (IntPtr) 0);
            GL.FramebufferTexture2D(FramebufferTarget.Framebuffer,
                FramebufferAttachment.ColorAttachment0 + AttachmentOrder.Count, TextureTarget.Texture2D,
                texture.Pointer, 0);

            texture.UnBind();
            Attachments.Add(name, texture);
            AttachmentOrder.Add(name);
        }

        public void AddCubeAttachment(string name, PixelType pixelType = PixelType.Float,
            PixelInternalFormat internalFormat = PixelInternalFormat.Rgba16f, PixelFormat format = PixelFormat.Rgba,
            TextureMinFilter filter = TextureMinFilter.Nearest) {
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
            texture.SetFilters(filter, (TextureMagFilter) filter);

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
            for (int i = 0; i < AttachmentOrder.Count; i++) colorAttachments[i] = DrawBuffersEnum.ColorAttachment0 + i;
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
    }
}