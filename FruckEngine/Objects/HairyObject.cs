using System.Collections.Generic;
using FruckEngine.Graphics;
using FruckEngine.Structs;
using FruckEngine.Helpers;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;

namespace FruckEngine.Objects
{
    public class HairyObject : Object
    {
        public Material HairMaterial { get; private set; }
        public int HairSegmentCount;
        public float HairOffset;

        public HairyObject(Object o) : base(o)
        {
            HairMaterial = new PBRMaterial();
            HairMaterial.Textures.Add(TextureHelper.LoadFromImage("Assets/models/frog/hair.png", ShadeType.TEXTURE_TYPE_ALBEDO));
        }

        public override void Draw(CoordSystem coordSys, Shader shader, DrawProperties properties)
        {
            base.Draw(coordSys, shader, properties);

            if (Meshes.Count == 0) return;

            var modelM = GetMatrix(coordSys.Model);
            coordSys.Model = modelM;

            for (int i = 1; i < HairSegmentCount; i++)
            {
                shader.Use();

                HairMaterial.Apply(shader);
                shader.SetFloat("uOffset", i * HairOffset);

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
