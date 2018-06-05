using System;
using System.Drawing;
using System.Drawing.Imaging;
using OpenTK.Graphics.OpenGL;

namespace FruckEngine.Graphics
{
    public class Texture
    {
        public int Pointer;

        public Texture(int pointer)
        {
            Pointer = pointer;
        }

        public void Bind()
        {
            GL.BindTexture( TextureTarget.Texture2D, Pointer );
        }

        public static Texture LoadTexture(string filename)
        {
            if (string.IsNullOrEmpty( filename )) throw new ArgumentException( filename );
            var ret = new Texture(GL.GenTexture());
            ret.Bind();
            
            // We will not upload mipmaps, so disable mipmapping (otherwise the texture will not appear).
            // We can use GL.GenerateMipmaps() or GL.Ext.GenerateMipmaps() to create
            // mipmaps automatically. In that case, use TextureMinFilter.LinearMipmapLinear to enable them.
            GL.TexParameter( TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear );
            GL.TexParameter( TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear );
            
            var bmp = new Bitmap( filename );
            var bmp_data = bmp.LockBits( new Rectangle( 0, 0, bmp.Width, bmp.Height ), ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb );
            GL.TexImage2D( TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, bmp_data.Width, bmp_data.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, bmp_data.Scan0 );
            bmp.UnlockBits( bmp_data );

            return ret;
        }
    }
}