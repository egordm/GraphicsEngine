using System;
using System.Collections.Generic;
using FruckEngine.Objects;
using OpenTK;
using FruckEngine.Helpers;
using FruckEngine.Graphics;

namespace FruckEngineDemo.Scenes
{
    public class Cody : Scene {
        public double Time = 0;
        private FruckEngine.Objects.Object frog1, frog2;
        
        public override void Init(World world)
        {
            world.Environment.AmbientLight = Vector3.One;
            world.Environment.DirectionalLight.Intensity = 0;

            var env = TextureHelper.LoadCubemapFromDir("Assets/cubemaps/Home");
            world.Environment.SetTexture(env, true);

            world.MainCamera.Position = new Vector3(0, 0, 5);
            world.MainCamera.SetRotation(0, -180);

            /*const string directory = "Assets/models/frog";
            var model = AssimpLoadHelper.LoadModel(directory + "/frog.obj", true);
            var material = model.Meshes[0].AsPBR();
            material.Textures.Clear();
            material.Textures.Add(TextureHelper.LoadFromImage(directory + "/diffuse.png", ShadeType.TEXTURE_TYPE_ALBEDO));
            material.Albedo = Vector3.One;
            material.Metallic = 0.99f;
            material.Roughness = 0.01f;
            model.Scale = Vector3.One * 0.1f;
            world.AddObject(model);*/
            
            var sm = DefaultModels.GetSphere();
            var orig = new FruckEngine.Objects.Object();
            orig.Meshes.Add(sm);
            var mat = sm.AsPBR();
            mat.Metallic = 0f;
            mat.Roughness = 1f;
            mat.Albedo = new Vector3(1, 0, 0);
            orig.Position = Vector3.Zero;
            orig.Rotation = Quaternion.Identity;
            orig.Scale = Vector3.One * 0.1f;
            world.AddObject(orig);
            
            FruckEngine.Objects.Object child, grandchild;
            {
                const string directory = "Assets/models/frog";
                frog1 = AssimpLoadHelper.LoadModel(directory + "/frog.obj", true);
                var material = frog1.Meshes[0].AsPBR();
                material.Textures.Clear();
                material.Textures.Add(TextureHelper.LoadFromImage(directory + "/diffuse.png", ShadeType.TEXTURE_TYPE_ALBEDO));
                material.Albedo = Vector3.One;
                material.Metallic = 0.99f;
                material.Roughness = 0.01f;
                frog1.Scale = Vector3.One * 0.1f;
                frog1.Position = Vector3.UnitY * 0;
                world.AddObject(frog1);
            }
            {
                const string directory = "Assets/models/frog";
                frog2 = AssimpLoadHelper.LoadModel(directory + "/frog.obj", true);
                var material = frog1.Meshes[0].AsPBR();
                material.Textures.Clear();
                material.Textures.Add(TextureHelper.LoadFromImage(directory + "/diffuse.png", ShadeType.TEXTURE_TYPE_ALBEDO));
                material.Albedo = Vector3.One;
                material.Metallic = 0.99f;
                material.Roughness = 0.01f;
                frog2.Scale = Vector3.One;
                //child.Rotation = Quaternion.FromAxisAngle(Vector3.UnitZ, 45f);
                frog2.Position = Vector3.UnitY * 60;
                frog1.Children.Add(frog2);
            }

            var lights = new List<Vector3>(){
                new Vector3(-20.0f, 20.0f, 45.0f),
                new Vector3(20.0f, 20.0f, 45.0f),
                new Vector3(-20.0f, -20.0f, 45.0f),
                new Vector3(20.0f, -20.0f, 45.0f),
            };

            foreach (var lightPos in lights)
            {
                world.AddLight(new PointLight(lightPos, Vector3.One, 15000));
            }
        }

        public override void Update(World world, double dt) {
            base.Update(world, dt);

            Time += dt;
            
            frog1.Position = new Vector3((float)Math.Sin(Time) * 5, 0, (float)Math.Cos(Time) * 5);
            frog2.Rotation = Quaternion.FromAxisAngle(Vector3.UnitY, (float) Time);
        }
    }
}