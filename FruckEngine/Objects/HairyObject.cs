using System.Collections.Generic;
using FruckEngine.Graphics;
using FruckEngine.Structs;
using FruckEngine.Helpers;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Runtime.InteropServices;
using System.Drawing;

namespace FruckEngine.Objects
{
    public class HairyObject : Object
    {
        public PBRMaterial HairMaterial { get; private set; }
        public int HairSegmentCount;
        public float HairSegmentOffset;
        public int HairInvDensity = 3;
        public int HairThickness = 1;
        private bool inited = false;

        public HairyObject(Object o) : base(o)
        {
            HairMaterial = new PBRMaterial();
        }

        public void InitHair()
        {
            inited = true;
            HairMaterial = new PBRMaterial();
            if (Meshes.Count == 0) return;
            PBRMaterial normalMaterial = Meshes[0].AsPBR();
            HairMaterial.Albedo = normalMaterial.Albedo;
            HairMaterial.Roughness = normalMaterial.Roughness;
            HairMaterial.Metallic = normalMaterial.Metallic;
            if (normalMaterial.Textures.Count == 0) return;
            Texture hairTex = (Texture)normalMaterial.Textures[0].Clone();
            hairTex.Pointer = Constants.UNCONSTRUCTED;
            Bitmap bitmap = TextureHelper.GetData(hairTex.Path);
            Color gum = Color.FromArgb(0, Color.Black);

            int step = Math.Max(0, HairInvDensity);
            int thickness = Math.Max(Math.Min(step, HairThickness), 0);
            for (int x = 0; x < bitmap.Width; x ++)
                for (int y = 0; y < bitmap.Height; y++)
                {
                    if (x % step < thickness && y % step < thickness) continue;
                    bitmap.SetPixel(x, y, gum);
                } 

            bitmap.MakeTransparent(gum);
            TextureHelper.LoadFromBitmap(ref hairTex, bitmap);
            HairMaterial.Textures.Add(hairTex);
        }

        public override void Draw(CoordSystem coordSys, Shader shader, DrawProperties properties)
        {
            base.Draw(coordSys, shader, properties);

            if (Meshes.Count == 0) return;

            var modelM = GetMatrix(coordSys.Model);
            coordSys.Model = modelM;

            if (!inited) InitHair();
            for (int i = 1; i < HairSegmentCount; i++)
            {
                shader.Use();

                HairMaterial.Apply(shader);
                shader.SetFloat("uOffset", i * HairSegmentOffset);

                foreach (var mesh in Meshes)
                {
                    var tmp = mesh.Material;
                    mesh.Material = HairMaterial;
                    mesh.Draw(shader, properties);
                    mesh.Material = tmp;
                }
            }
            shader.SetFloat("uOffset", 0);
            Console.WriteLine("j");

        }
    }
}
