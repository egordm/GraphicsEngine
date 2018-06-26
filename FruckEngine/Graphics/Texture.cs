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
    /// <summary>
    /// Different possible texcture types used for shading.
    /// Sometimes type might not fit. Use albedo then.
    /// Some texture types are conversible between shading types.
    /// </summary>
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

    /// <summary>
    /// Abstraction of opengl texture with own twist
    /// </summary>
    public class Texture : ICloneable {
        public const int TextureTypeCount = 5;
        
        /// <summary>
        /// Pointer to the texture of opengl
        /// </summary>
        public int Pointer = Constants.UNCONSTRUCTED;
        
        /// <summary>
        /// Shadetype the texture is of
        /// </summary>
        public ShadeType ShadeType = ShadeType.TEXTURE_TYPE_DIFFUSE;
        
        // Texture properties for quick access
        public TextureTarget Target = TextureTarget.Texture2D;
        public PixelInternalFormat InternalFormat = PixelInternalFormat.Rgb;
        public PixelFormat Format = PixelFormat.Rgb;
        public PixelType PixelType = PixelType.UnsignedByte;
        public TextureMinFilter FilterMin = TextureMinFilter.Nearest;
        public TextureMagFilter FilterMag = TextureMagFilter.Nearest;
        public TextureWrapMode WrapS = TextureWrapMode.ClampToEdge;
        public TextureWrapMode WrapT = TextureWrapMode.ClampToEdge;
        public bool MipMap = true;
        
        /// <summary>
        /// Path to image file. Cody needs it :P
        /// </summary>
        public string Path;

        /// <summary>
        /// Size of the texture
        /// </summary>
        public int Width = 0, Height = 0;

        /// <summary>
        /// Texture is empty by default and should be initialized
        /// </summary>
        public Texture() { }
       
        /// <summary>
        /// Initialize the texture and load data into it if needed.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="dataPointer"></param>
        public void Load(int width, int height, IntPtr dataPointer) {
            Load(width, height, InternalFormat, Format, Target, PixelType, dataPointer);
        }

        /// <summary>
        /// Initialize the texture and load data into it if needed. 
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="internalFormat"></param>
        /// <param name="format"></param>
        /// <param name="target"></param>
        /// <param name="pixelType"></param>
        /// <param name="dataPointer"></param>
        public void Load(int width, int height, PixelInternalFormat internalFormat, PixelFormat format, TextureTarget target,
            PixelType pixelType, IntPtr dataPointer) {
            Width = width;
            Height = height;
            InternalFormat = internalFormat;
            Format = format;
            Target = target;
            PixelType = pixelType;

            //Generate texture if it is unconstructed
            if (Pointer == Constants.UNCONSTRUCTED) Pointer = GL.GenTexture();

            // Change the properties
            Bind();
            SetWrapping(WrapS, WrapT, false);
            SetFilters(FilterMin, FilterMag, false);

            // Load data into texture if it is a texture 2d. It is bit more complex for other types
            if (target == TextureTarget.Texture2D) {
                GL.TexImage2D(target, 0, InternalFormat, Width, Height, 0, Format, PixelType, dataPointer);
            }
            
            if(MipMap) GenMipMaps();
            
            UnBind();
        }

        /// <summary>
        /// Load a face into a cube map texture
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="FaceTarget"></param>
        /// <param name="dataPointer"></param>
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

        /// <summary>
        /// Activate the texture for a given unit
        /// </summary>
        /// <param name="pos"></param>
        public void Activate(uint pos) {
            GL.ActiveTexture((TextureUnit) ((uint)TextureUnit.Texture0 + pos));
            Bind();
        }

        /// <summary>
        /// Resize the texture 
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void Resize(int width, int height) {
            if(Target != TextureTarget.Texture2D) return;
            Width = width;
            Height = height;
            Bind();
            GL.TexImage2D(Target, 0, InternalFormat, width, height, 0, Format, PixelType, (IntPtr) 0);
        }

        /// <summary>
        /// Is texture loaded
        /// </summary>
        /// <returns></returns>
        public bool IsLoaded() {
            return Pointer != Constants.UNCONSTRUCTED;
        }

        /// <summary>
        /// Update wrapping parameters
        /// </summary>
        /// <param name="wrapS"></param>
        /// <param name="wrapT"></param>
        /// <param name="doBind"></param>
        public void SetWrapping(TextureWrapMode wrapS = TextureWrapMode.ClampToEdge,
            TextureWrapMode wrapT = TextureWrapMode.ClampToEdge, bool doBind = false) {
            if (doBind) Bind();
            WrapS = wrapS;
            WrapT = wrapT;
            if (Pointer != Constants.UNCONSTRUCTED) {
                GL.TexParameter(Target, TextureParameterName.TextureWrapS, (int) wrapS);
                GL.TexParameter(Target, TextureParameterName.TextureWrapT, (int) wrapT);
            }
        }
        
        /// <summary>
        /// Update wrapping parameters
        /// </summary>
        /// <param name="minFilter"></param>
        /// <param name="magFilter"></param>
        /// <param name="doBind"></param>
        public void SetFilters(TextureMinFilter minFilter = TextureMinFilter.Linear,
            TextureMagFilter magFilter = TextureMagFilter.Linear, bool doBind = false) {
            if (doBind) Bind();
            FilterMin = minFilter;
            FilterMag = magFilter;
            if (Pointer != Constants.UNCONSTRUCTED) {
                GL.TexParameter(Target, TextureParameterName.TextureMinFilter, (int) FilterMin);
                GL.TexParameter(Target, TextureParameterName.TextureMagFilter, (int) FilterMag);
            }
        }

        /// <summary>
        /// Generate mipmaps
        /// </summary>
        /// <param name="doBind"></param>
        public void GenMipMaps(bool doBind = false) {
            if (doBind) Bind();
            GL.GenerateMipmap((GenerateMipmapTarget) Target);
        }

        /// <summary>
        /// Clone the texture
        /// </summary>
        /// <returns></returns>
        public object Clone() {
            return this.MemberwiseClone();
        }

        /// <summary>
        /// Delete from vram
        /// </summary>
        public void Destroy() {
            GL.DeleteTexture(Pointer);
            Pointer = Constants.UNCONSTRUCTED;
        }
    }
}