using System;
using System.Drawing;
using System.Drawing.Imaging;
using FruckEngine.Helpers;
using OpenTK.Graphics.OpenGL;
using PixelFormat = System.Drawing.Imaging.PixelFormat;

namespace FruckEngine.Graphics {
    public class Raster {
        public int Width, Height;
        public int[] Pixels;
        public Texture Texture { get; private set; }

        public Raster(int width, int height) {
            Width = width;
            Height = height;
            Pixels = new int[Width * Height];
        }

        public Raster(string path) {
            var bmp = new Bitmap(path);
            Width = bmp.Width;
            Height = bmp.Height;
            Pixels = new int[Width * Height];
            var data = bmp.LockBits(new Rectangle(0, 0, Width, Height), ImageLockMode.ReadOnly,
                PixelFormat.Format32bppArgb);
            var ptr = data.Scan0;
            System.Runtime.InteropServices.Marshal.Copy(data.Scan0, Pixels, 0, Width * Height);
            bmp.UnlockBits(data);
        }

        public void AttachTexture() {
            Texture = new Texture();
            Texture.SetFilters(TextureMinFilter.Linear, TextureMagFilter.Linear);
            Texture.InternalFormat = PixelInternalFormat.Rgba;
            Texture.Format = OpenTK.Graphics.OpenGL.PixelFormat.Bgra;
            Texture.Target = TextureTarget.Texture2D;
            Texture.Format = OpenTK.Graphics.OpenGL.PixelFormat.Bgra;
            TextureHelper.LoadDataIntoTexture(Texture, Width, Height, Pixels);
            Texture.Bind();
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, Width, Height, 0,
                OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, Pixels);
        }

        public void PushToTexture() {
            if (Texture == null) return;
            Texture.Bind();
            GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, Width, Height, 0,
                OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, Pixels);
        }

        public void DrawText(string s, int x, int y, int c) {
            FontHelper.Print(this, s, x, y, c);
        }

        public void Clear(int c) {
            for (int s = Width * Height, p = 0; p < s; p++) Pixels[p] = c;
        }
    }

    public static class FontHelper {
        private static Raster FontAtlas = null;

        private static string Characters =
            "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*()_-+={}[];:<>,.?/\\ ";

        private static int[] FontRedir = null;

        private static void LoadCharacters() {
            FontAtlas = new Raster("Assets/font.png");
            FontRedir = new int[256];

            for (int i = 0; i < 256; i++) FontRedir[i] = 0;
            for (int i = 0; i < Characters.Length; i++) {
                int l = (int) Characters[i];
                FontRedir[l & 255] = i;
            }
        }

        public static void Print(Raster raster, string s, int x, int y, int c) {
            if (FontAtlas == null) LoadCharacters();
            for (int i = 0; i < s.Length; i++) {
                int f = FontRedir[(int) s[i] & 255];
                int dest = x + i * 12 + y * raster.Width;
                int src = f * 12;
                for (int v = 0; v < FontAtlas.Height; v++, src += FontAtlas.Width, dest += raster.Width)
                for (int u = 0; u < 12; u++) {
                    if ((FontAtlas.Pixels[src + u] & 0xffffff) != 0) raster.Pixels[dest + u] = c;
                }
            }
        }
    }
}