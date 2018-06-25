using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using FruckEngine.Graphics;
using ImageMagick;
using OpenTK.Graphics.OpenGL;
using System.Drawing;

namespace FruckEngine.Helpers {
    /// <summary>
    /// A helper for creatign and reusing textures
    /// </summary>
    public static class TextureHelper {
        /// <summary>
        /// Load a texture from image
        /// </summary>
        /// <param name="path"></param>
        /// <param name="type"></param>
        /// <param name="face"></param>
        /// <param name="exposureCorrect"></param>
        /// <returns></returns>
        public static Texture LoadFromImage(string path, ShadeType type = ShadeType.TEXTURE_TYPE_ALBEDO,
            TextureTarget face = 0, float exposureCorrect = 66878) {
            var texture = new Texture();
            texture.ShadeType = type;
            LoadFromImage(ref texture, path, face, exposureCorrect);
            return texture;
        }

        /// <summary>
        /// Load a texture from image
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="path"></param>
        /// <param name="face"></param>
        /// <param name="exposureCorrect"></param>
        public static void LoadFromImage(ref Texture texture, string path, TextureTarget face = 0,
            float exposureCorrect = 66878) {
            // Load image
            var img = new MagickImage(path);
            var isHDR = img.Format == MagickFormat.Hdr;
            var pixels = img.GetPixels();
            texture.Path = path;

            // Determine images properties to create texture accordingly
            FormatFromChannelCount(img.ChannelCount, isHDR, ref texture.InternalFormat, ref texture.Format);
            texture.PixelType = isHDR ? PixelType.Float : PixelType.UnsignedByte;
            if (face == 0) texture.Target = TextureTarget.Texture2D;

            if (isHDR) {
                // If hdr we make a float based texture
                var data = pixels.ToArray();
                for (int i = 0; i < data.Length; i++) data[i] /= exposureCorrect; // Correct the exposure a bit

                if (face == 0) LoadDataIntoTexture(texture, img.Width, img.Height, data);
                else LoadFaceDataIntoTexture(texture, img.Width, img.Height, face, data);
            } else {
                // If not then it is a unsigned byte base texture
                var data = pixels.ToByteArray(texture.Format == PixelFormat.Rgb
                    ? "RGB"
                    : (texture.Format == PixelFormat.Rgba ? "RGBA" : "R"));

                if (face == 0) LoadDataIntoTexture(texture, img.Width, img.Height, data);
                else LoadFaceDataIntoTexture(texture, img.Width, img.Height, face, data);
            }
        }

        /// <summary>
        /// Read image to a bitmap
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static Bitmap GetData(string path)
        {
            var img = new MagickImage(path);
            if(img.Format == MagickFormat.Hdr) return null;
            var pixels = img.GetPixels();
            return img.ToBitmap();
        }

        /// <summary>
        /// Create a image froma bitmap
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="bitmap"></param>
        public static void LoadFromBitmap(ref Texture texture, Bitmap bitmap)
        {
            var img = new MagickImage(bitmap);
            var isHDR = img.Format == MagickFormat.Hdr;
            var pixels = img.GetPixels();
            texture.Path = "";

            // Determine images properties to create texture accordingly
            FormatFromChannelCount(img.ChannelCount, isHDR, ref texture.InternalFormat, ref texture.Format);
            texture.PixelType = isHDR ? PixelType.Float : PixelType.UnsignedByte;
            texture.Target = TextureTarget.Texture2D;

            var data = pixels.ToByteArray(texture.Format == PixelFormat.Rgb
                    ? "RGB"
                    : (texture.Format == PixelFormat.Rgba ? "RGBA" : "R"));

            LoadDataIntoTexture(texture, img.Width, img.Height, data);
        }

        /// <summary>
        /// Load a cubemap from directory. This implies a cubemap is split in multiple faces with specific names like the
        /// ones below
        /// </summary>
        /// <param name="dir"></param>
        /// <param name="exposureCorrect"></param>
        /// <returns></returns>
        public static Texture LoadCubemapFromDir(string dir, float exposureCorrect = 66878) {
            return LoadFromCubemap(new List<string> {
                dir + "/_posx.hdr",
                dir + "/_negx.hdr",
                dir + "/_posy.hdr",
                dir + "/_negy.hdr",
                dir + "/_posz.hdr",
                dir + "/_negz.hdr"
            }, exposureCorrect);
        }

        /// <summary>
        /// Load each face into a cubemap texture
        /// </summary>
        /// <param name="faces"></param>
        /// <param name="exposureCorrect"></param>
        /// <returns></returns>
        public static Texture LoadFromCubemap(List<string> faces, float exposureCorrect = 66878) {
            var texture = new Texture();
            LoadFromCubemap(ref texture, faces, exposureCorrect);
            return texture;
        }

        /// <summary>
        /// Load each face into a cubemap texture
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="faces"></param>
        /// <param name="exposureCorrect"></param>
        /// <exception cref="Exception"></exception>
        public static void LoadFromCubemap(ref Texture texture, List<string> faces, float exposureCorrect = 66878) {
            if (faces.Count != 6) throw new Exception("Cube map must have 6 faces!");

            // Set texture properties
            texture.Target = TextureTarget.TextureCubeMap;
            if (texture.Pointer == 0) texture.Pointer = GL.GenTexture();
            texture.Bind();

            // Load each face as target
            for (int i = 0; i < faces.Count; ++i) {
                LoadFromImage(ref texture, faces[i],
                    (TextureTarget) ((uint) TextureTarget.TextureCubeMapPositiveX + (uint) i), exposureCorrect);
            }

            if (texture.MipMap) texture.GenMipMaps(false);
            texture.UnBind();
        }

        /// <summary>
        /// Get texture fromat from the channel count and pixel type
        /// </summary>
        /// <param name="nChannels"></param>
        /// <param name="isFloat"></param>
        /// <param name="internalFormat"></param>
        /// <param name="format"></param>
        private static void FormatFromChannelCount(int nChannels, bool isFloat,
            ref PixelInternalFormat internalFormat, ref PixelFormat format) {
            switch (nChannels) {
                case 1:
                    format = PixelFormat.Red;
                    internalFormat = PixelInternalFormat.R8;
                    break;
                case 3:
                    format = PixelFormat.Rgb;
                    internalFormat = isFloat ? PixelInternalFormat.Rgb32f : PixelInternalFormat.Rgb;
                    break;
                case 4:
                    format = PixelFormat.Rgba;
                    internalFormat = isFloat ? PixelInternalFormat.Rgba32f : PixelInternalFormat.Rgba;
                    break;
            }
        }

        /// <summary>
        /// Loads given data into the texture since a pointer to memory is needed which is easy to obtain in c++
        /// not so in c#
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="data"></param>
        /// <typeparam name="T"></typeparam>
        public static void LoadDataIntoTexture<T>(Texture texture, int width, int height, T[] data) {
            var gch = GCHandle.Alloc(data, GCHandleType.Pinned);
            try {
                var addr = gch.AddrOfPinnedObject();
                texture.Load(width, height, addr);
            } finally {
                gch.Free();
            }
        }

        /// <summary>
        /// Loads given data into the face texture since a pointer to memory is needed which is easy to obtain in c++
        /// not so in c#
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="faceTarget"></param>
        /// <param name="data"></param>
        /// <typeparam name="T"></typeparam>
        private static void LoadFaceDataIntoTexture<T>(Texture texture, int width, int height, TextureTarget faceTarget,
            T[] data) {
            var gch = GCHandle.Alloc(data, GCHandleType.Pinned);
            try {
                var addr = gch.AddrOfPinnedObject();
                texture.LoadFace(width, height, faceTarget, addr);
            } finally {
                gch.Free();
            }
        }
        
        
        // Static textures which are highly reused

        private static Texture NormalNullTexture = new Texture();
        private static Texture OneNullTexture = new Texture();
        private static Texture ZeroNullTexture = new Texture();
        private static Texture StandardColorLUTTexture = new Texture();

        /// <summary>
        /// Normal null texture which is unit Z since it is in tangent space
        /// </summary>
        /// <returns></returns>
        public static Texture GetNormalNull() {
            if (NormalNullTexture.Pointer == Constants.UNCONSTRUCTED)
                InitNullTex(NormalNullTexture, new byte[] {0x7F, 0x7F, 0xFF});

            return NormalNullTexture;
        }

        /// <summary>
        /// Texture of Ones
        /// </summary>
        /// <returns></returns>
        public static Texture GetOneNull() {
            if (OneNullTexture.Pointer == Constants.UNCONSTRUCTED)
                InitNullTex(OneNullTexture, new byte[] {0xFF, 0xFF, 0xFF});

            return OneNullTexture;
        }

        /// <summary>
        /// Texture of Zeros
        /// </summary>
        /// <returns></returns>
        public static Texture GetZeroNull() {
            if (ZeroNullTexture.Pointer == Constants.UNCONSTRUCTED)
                InitNullTex(ZeroNullTexture, new byte[] {0, 0, 0});

            return ZeroNullTexture;
        }
        
        /// <summary>
        /// Standard color lookup table
        /// </summary>
        /// <returns></returns>
        public static Texture GetStandardColorLUTTexture() {
            if (StandardColorLUTTexture.Pointer == Constants.UNCONSTRUCTED) {
                StandardColorLUTTexture.FilterMin = TextureMinFilter.Linear;
                StandardColorLUTTexture.FilterMag = TextureMagFilter.Linear;
                LoadFromImage(ref StandardColorLUTTexture, "Assets/color_lut.png");
            }

            return StandardColorLUTTexture;
        }

        /// <summary>
        /// Function to load null textures. Which filtered to neares pixel and have no mipmaps and only 3 components
        /// </summary>
        /// <param name="texture"></param>
        /// <param name="data"></param>
        private static void InitNullTex(Texture texture, byte[] data) {
            texture.FilterMin = TextureMinFilter.Nearest;
            texture.FilterMag = TextureMagFilter.Nearest;
            texture.MipMap = false;
            texture.InternalFormat = PixelInternalFormat.Rgb;
            texture.Format = PixelFormat.Rgb;
            texture.Target = TextureTarget.Texture2D;
            texture.PixelType = PixelType.UnsignedByte;
            LoadDataIntoTexture(texture, 1, 1, data);
        }
    }
}