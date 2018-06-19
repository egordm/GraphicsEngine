using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using FruckEngine.Graphics;
using ImageMagick;
using OpenTK.Graphics.OpenGL;

namespace FruckEngine.Helpers {
    public static class TextureHelper {
        public static Texture LoadFromImage(string path, ShadeType type = ShadeType.TEXTURE_TYPE_ALBEDO,
            TextureTarget face = 0, float exposureCorrect = 66878) {
            var texture = new Texture();
            texture.ShadeType = type;
            LoadFromImage(ref texture, path, face, exposureCorrect);
            return texture;
        }

        public static void LoadFromImage(ref Texture texture, string path, TextureTarget face = 0,
            float exposureCorrect = 66878) {
            var img = new MagickImage(path);
            var isHDR = img.Format == MagickFormat.Hdr;
            var pixels = img.GetPixels();

            FormatFromChannelCount(img.ChannelCount, isHDR, ref texture.InternalFormat, ref texture.Format);
            texture.PixelType = isHDR ? PixelType.Float : PixelType.UnsignedByte;
            if (face == 0) texture.Target = TextureTarget.Texture2D;

            if (isHDR) {
                var data = pixels.ToArray();
                for (int i = 0; i < data.Length; i++) data[i] /= exposureCorrect; // Correct the exposure a bit

                if (face == 0) LoadDataIntoTexture(texture, img.Width, img.Height, data);
                else LoadFaceDataIntoTexture(texture, img.Width, img.Height, face, data);
            } else {
                var data = pixels.ToByteArray(texture.Format == PixelFormat.Rgb
                    ? "RGB"
                    : (texture.Format == PixelFormat.Rgba ? "RGBA" : "R"));

                if (face == 0) LoadDataIntoTexture(texture, img.Width, img.Height, data);
                else LoadFaceDataIntoTexture(texture, img.Width, img.Height, face, data);
            }
        }

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

        public static Texture LoadFromCubemap(List<string> faces, float exposureCorrect = 66878) {
            var texture = new Texture();
            LoadFromCubemap(ref texture, faces, exposureCorrect);
            return texture;
        }

        public static void LoadFromCubemap(ref Texture texture, List<string> faces, float exposureCorrect = 66878) {
            if (faces.Count != 6) throw new Exception("Cube map must have 6 faces!");

            texture.Target = TextureTarget.TextureCubeMap;
            if (texture.Pointer == 0) texture.Pointer = GL.GenTexture();
            texture.Bind();

            for (int i = 0; i < faces.Count; ++i) {
                LoadFromImage(ref texture, faces[i],
                    (TextureTarget) ((uint) TextureTarget.TextureCubeMapPositiveX + (uint) i), exposureCorrect);
            }

            if (texture.MipMap) texture.GenMipMaps(false);
            texture.UnBind();
        }

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

        public static void LoadDataIntoTexture<T>(Texture texture, int width, int height, T[] data) {
            var gch = GCHandle.Alloc(data, GCHandleType.Pinned);
            try {
                var addr = gch.AddrOfPinnedObject();
                texture.Load(width, height, addr);
            } finally {
                gch.Free();
            }
        }

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

        private static Texture NormalNullTexture = new Texture();
        private static Texture OneNullTexture = new Texture();
        private static Texture ZeroNullTexture = new Texture();

        public static Texture GetNormalNull() {
            if (NormalNullTexture.Pointer == Constants.UNCONSTRUCTED)
                InitNullTex(NormalNullTexture, new byte[] {0x7F, 0x7F, 0xFF});

            return NormalNullTexture;
        }

        public static Texture GetOneNull() {
            if (OneNullTexture.Pointer == Constants.UNCONSTRUCTED)
                InitNullTex(OneNullTexture, new byte[] {0xFF, 0xFF, 0xFF});

            return OneNullTexture;
        }

        public static Texture GetZeroNull() {
            if (ZeroNullTexture.Pointer == Constants.UNCONSTRUCTED)
                InitNullTex(ZeroNullTexture, new byte[] {0, 0, 0});

            return ZeroNullTexture;
        }

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