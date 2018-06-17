using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Runtime.Remoting.Messaging;
using ImageMagick;
using OpenTK.Graphics.OpenGL;
using OpenTK.Platform.Windows;
using PixelFormat = OpenTK.Graphics.OpenGL.PixelFormat;

namespace FruckEngine.Graphics {
    public enum ShadeType {
        TEXTURE_TYPE_DIFFUSE = 0,
        TEXTURE_TYPE_ALBEDO = 0,
        TEXTURE_TYPE_SPECULAR = 1,
        TEXTURE_TYPE_METALLIC = 1,
        TEXTURE_TYPE_NORMAL = 2,
        TEXTURE_TYPE_AMBIENT = 3,
        TEXTURE_TYPE_AO = 3,
        TEXTURE_TYPE_SHININESS = 4,
        TEXTURE_TYPE_ROUGHNESS = 4
    }

    public class Texture {
        public int Pointer = Constants.TEXTURE_UNCONSTRUCTED;
        public ShadeType ShadeType = ShadeType.TEXTURE_TYPE_DIFFUSE;
        public TextureTarget Target = TextureTarget.Texture2D;
        public PixelInternalFormat InternalFormat = PixelInternalFormat.Rgb;
        public PixelFormat Format = PixelFormat.Rgb;
        public PixelType PixelType = PixelType.UnsignedByte;
        public TextureMinFilter FilterMin = TextureMinFilter.Nearest;
        public TextureMagFilter FilterMag = TextureMagFilter.Nearest;
        public TextureWrapMode WrapS = TextureWrapMode.ClampToEdge;
        public TextureWrapMode WrapT = TextureWrapMode.ClampToEdge;
        public bool MipMap = true;

        public int Width = 0, Height = 0;

        public Texture() { }
       

        public void Load(int width, int height, IntPtr dataPointer) {
            Load(width, height, InternalFormat, Format, Target, PixelType, dataPointer);
        }

        public void Load(int width, int height, PixelInternalFormat internalFormat, PixelFormat format, TextureTarget target,
            PixelType pixelType, IntPtr dataPointer) {
            Width = width;
            Height = height;
            InternalFormat = internalFormat;
            Format = format;
            Target = target;
            PixelType = pixelType;

            if (Pointer == 0) Pointer = GL.GenTexture();

            Bind();
            SetWrapping(WrapS, WrapT, false);
            SetFilters(FilterMin, FilterMag, false);

            if (target == TextureTarget.Texture2D) {
                GL.TexImage2D(target, 0, InternalFormat, Width, Height, 0, Format, PixelType, dataPointer);
            }
            
            if(MipMap) GenMipMaps();
            
            UnBind();
        }

        public void LoadFace(int width, int height, TextureTarget FaceTarget, IntPtr dataPointer) {
            Width = width;
            Height = height;
            GL.TexImage2D(FaceTarget, 0, InternalFormat, width, height, 0, Format, PixelType, dataPointer);
        }

        public void Bind() {
            GL.BindTexture(Target, Pointer);
        }
        
        public void UnBind() {
            GL.BindTexture(Target, 0);
        }

        public void SetWrapping(TextureWrapMode wrapS = TextureWrapMode.ClampToEdge,
            TextureWrapMode wrapT = TextureWrapMode.ClampToEdge, bool doBind = false) {
            if (doBind) Bind();
            WrapS = wrapS;
            WrapT = wrapT;
            GL.TexParameter(Target, TextureParameterName.TextureWrapS, (int) wrapS);
            GL.TexParameter(Target, TextureParameterName.TextureWrapT, (int) wrapT);
        }
        
        public void SetFilters(TextureMinFilter minFilter = TextureMinFilter.Linear,
            TextureMagFilter magFilter = TextureMagFilter.Linear, bool doBind = false) {
            if (doBind) Bind();
            FilterMin = minFilter;
            FilterMag = magFilter;
            GL.TexParameter(Target, TextureParameterName.TextureMinFilter, (int) FilterMin);
            GL.TexParameter(Target, TextureParameterName.TextureMagFilter, (int) FilterMag);
        }

        public void GenMipMaps(bool doBind = false) {
            if (doBind) Bind();
            GL.GenerateMipmap((GenerateMipmapTarget) Target);
        }
    }
}