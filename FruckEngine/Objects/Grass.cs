using System;
using System.Collections.Generic;
using FruckEngine.Graphics;
using FruckEngine.Helpers;
using FruckEngine.Structs;
using OpenTK;
using System.Drawing;

namespace FruckEngine.Objects
{
    public class Grass : HairyObject
    {
        private Vector3 dirA, dirB;
        private double time = 0f;
        private Matrix4 baseM = Matrix4.Identity;

        public Grass(int size, Vector3 dirA, Vector3 dirB)
        {
            Meshes.Add(DefaultModels.GetGrassPlane(size));
            this.dirA = dirA;
            this.dirB = dirB;
        }

        public override void Init()
        {
            base.Init();
            int size = 512;
            Bitmap normal = new Bitmap(size, size);
            for (int x = 0; x < normal.Width; x++)
                for (int y = 0; y < normal.Height; y++)
                    normal.SetPixel(x, y, Color.Brown);

            Bitmap hairmap = new Bitmap(size, size);
            int step = Math.Max(0, HairInvDensity);
            int thickness = Math.Max(Math.Min(step, HairThickness), 0);
            for (int x = 0; x < hairmap.Width; x++)
                for (int y = 0; y < hairmap.Height; y++)
                {
                    if (x % step < thickness && y % step < thickness)
                        hairmap.SetPixel(x, y, Color.ForestGreen);
                    else
                        hairmap.SetPixel(x, y, Color.Transparent);
                }
            
            Texture normalTex = new Texture();
            TextureHelper.LoadFromBitmap(ref normalTex, normal);
            var material = Meshes[0].AsPBR();
            material.Textures.Add(normalTex);
            material.Albedo = Vector3.One;
            material.Metallic = 0.3f;
            material.Roughness = 0.7f;

            Texture hairTex = new Texture();
            hairTex.PixelType = OpenTK.Graphics.OpenGL.PixelType.UnsignedByte;
            hairTex.Format = OpenTK.Graphics.OpenGL.PixelFormat.Rgb;
            hairTex.WrapS = OpenTK.Graphics.OpenGL.TextureWrapMode.Repeat;
            hairTex.WrapT = OpenTK.Graphics.OpenGL.TextureWrapMode.Repeat;
            TextureHelper.LoadFromBitmap(ref hairTex, hairmap);
            HairMaterial.Textures.Add(hairTex);
        }
        
        public override void Update(double dt)
        {
            base.Update(dt);
            time += dt;
            Vector3 d = Vector3.Lerp(dirA, dirB, (float)(Math.Sin(time) + 1) * 0.5f);
            d *= 0.05f;
            float str = (float)(Math.Sin(time) + 1) * 0.5f * 0.5f + 0.5f;
            for (int i = 0; i < layers.Length; i++)
                layers[layers.Length - 1 - i] = baseM * Matrix4.CreateTranslation(str * d / (float)Math.Pow(1.1f, i));
        }

        public override void Draw(CoordSystem coordSys, Shader shader, DrawProperties properties)
        {
            baseM = GetMatrix(coordSys.Model);
            base.Draw(coordSys, shader, properties);
        }
    }

    public class DelayedHairyObject : HairyObject
    {
        private Matrix4[] history;
        private int timer = 0;

        public DelayedHairyObject(Object o) : base(o) { }


        public override void Init() {
            base.Init();
            Inited = true;
            history = new Matrix4[layers.Length];
        }

        public override void Draw(CoordSystem coordSys, Shader shader, DrawProperties properties)
        {
            var modelM = GetMatrix(coordSys.Model);
            history[timer % history.Length] = modelM;
            timer++;
            if (timer > HairSegmentCount)
                for (int i = 0; i < layers.Length; i++)
                    layers[i] = history[(timer - i) % HairSegmentCount];
                    
            base.Draw(coordSys, shader, properties);
        }
    }
}