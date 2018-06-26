using System;
using FruckEngine.Helpers;
using FruckEngine.Objects;
using OpenTK;
using FruckEngine.Game;
using FruckEngine.Graphics;
using OpenTK.Graphics.OpenGL;

namespace FruckEngineDemo.Scenes {
    public class MarioCart : Scene {
        protected override void Init(World world) {
            world.Environment.AmbientLight = Vector3.One*0.2f;
            world.Environment.Sun.Position = new Vector3(0.842257f, 0.5299188f, 0.0989397f);
            world.Environment.Sun.Density = 0.3f;
            world.Environment.Sun.Intensity = 12;

            world.Velocity = 0.03f;
            
            world.Environment.ColorLUT = new Texture();
            world.Environment.ColorLUT.SetFilters(TextureMinFilter.Linear, TextureMagFilter.Linear);
            TextureHelper.LoadFromImage(ref world.Environment.ColorLUT, "Assets/lut/mario_lut.png");
            
            var env = TextureHelper.LoadCubemapFromDir("Assets/cubemaps/Mountains", 20000);
            world.Environment.SetTexture(env, true);
            
            world.MainCamera.Position = new Vector3(-5.48f, 9.20f, 2.6f);
            world.MainCamera.SetRotation(-3, -295);
            world.MainCamera.FStop = world.MainCamera.FocalLength / 2;
            
            // Track
            string directory = "Assets/models/mario_circuit";
            var model = AssimpLoadHelper.LoadModel(directory + "/Mario Circuit.obj", true);

            foreach (var mesh in model.Meshes) {
                var material = mesh.AsPBR();
                material.Albedo = Vector3.One;
                material.Metallic = 0.4f;
                material.Roughness = 0.7f;
            }
            
            model.Scale = Vector3.One * 0.15f;
            world.AddObject(model);
            
            // Bowser
            directory = "Assets/models/Bowser";
            model = AssimpLoadHelper.LoadModel(directory + "/bowser.obj", true);
            model.Position  = new Vector3(-1.533677f, 7.642062f, 2.187414f)*1.5f;
            model.Scale = Vector3.One * 0.015f;
            model.Meshes.RemoveAt(0);
            model.Meshes.RemoveAt(0);

            foreach (var mesh in model.Meshes) {
                var material = mesh.AsPBR();
                
                material.Albedo = Vector3.One;
                material.Metallic = 0.5f;
                material.Roughness = 0.7f;
            }
            
            world.AddObject(model);
            
            directory = "Assets/models/Shy Guy";
            model = AssimpLoadHelper.LoadModel(directory + "/shyguy.obj", true);
            model.Position  = new Vector3(-1.333677f, 7.642062f, 2.187414f)*1.5f;
            model.Scale = Vector3.One * 0.015f;

            foreach (var mesh in model.Meshes) {
                var material = mesh.AsPBR();
                
                material.Albedo = Vector3.One;
                material.Metallic = 0.5f;
                material.Roughness = 0.7f;
            }
            
            world.AddObject(model);
            
            /*world.AddLight(new PointLight(  new Vector3(-20.0f, 50.0f, -45.0f), new Vector3(1.0342f, 1.0759f, 0.3f), 100000));
            world.AddLight(new PointLight(  new Vector3(-20.0f, 50.0f, 45.0f), Vector3.One, 10000));*/

        }
    }
}