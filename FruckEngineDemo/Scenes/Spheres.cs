using System.Collections.Generic;
using FruckEngine.Game;
using FruckEngine.Graphics;
using FruckEngine.Helpers;
using FruckEngine.Objects;
using OpenTK;

namespace FruckEngineDemo.Scenes {
    public class Spheres : Scene {
        protected override void Init(World world) {
            world.Environment.AmbientLight = Vector3.One;
            world.Environment.Sun.Position = new Vector3(0, 1, 2f);
            world.Environment.Sun.Intensity = 0;

            var env = TextureHelper.LoadCubemapFromDir("Assets/cubemaps/Home");
            world.Environment.SetTexture(env, true);
            
            world.MainCamera.Position = new Vector3(0.37f, 0.15f, -8.91f);
            world.MainCamera.SetRotation(-4, -359);

            string directory = "Assets/textures";
            
            // Gold
            var model = new Object(new List<Mesh>(){DefaultModels.GetSphere()});
            var material = model.Meshes[0].AsPBR(); 
            material.Textures.Add(TextureHelper.LoadFromImage(directory + "/gold/albedo.png", ShadeType.TEXTURE_TYPE_ALBEDO));
            material.Textures.Add(TextureHelper.LoadFromImage(directory + "/gold/ao.png", ShadeType.TEXTURE_TYPE_AO));
            material.Textures.Add(TextureHelper.LoadFromImage(directory + "/gold/metallic.png", ShadeType.TEXTURE_TYPE_METALLIC));
            material.Textures.Add(TextureHelper.LoadFromImage(directory + "/gold/normal.png", ShadeType.TEXTURE_TYPE_NORMAL));
            material.Textures.Add(TextureHelper.LoadFromImage(directory + "/gold/roughness.png", ShadeType.TEXTURE_TYPE_ROUGHNESS));
            material.Albedo = Vector3.One;
            material.Metallic = 1;
            material.Roughness = 1;
            world.AddObject(model);
            
            model = new Object(new List<Mesh>(){DefaultModels.GetSphere()});
            material = model.Meshes[0].AsPBR(); 
            material.Textures.Add(TextureHelper.LoadFromImage(directory + "/grass/albedo.png", ShadeType.TEXTURE_TYPE_ALBEDO));
            material.Textures.Add(TextureHelper.LoadFromImage(directory + "/grass/ao.png", ShadeType.TEXTURE_TYPE_AO));
            material.Textures.Add(TextureHelper.LoadFromImage(directory + "/grass/metallic.png", ShadeType.TEXTURE_TYPE_METALLIC));
            material.Textures.Add(TextureHelper.LoadFromImage(directory + "/grass/normal.png", ShadeType.TEXTURE_TYPE_NORMAL));
            material.Textures.Add(TextureHelper.LoadFromImage(directory + "/grass/roughness.png", ShadeType.TEXTURE_TYPE_ROUGHNESS));
            material.Albedo = Vector3.One;
            material.Metallic = 1;
            material.Roughness = 1;
            model.Position = Vector3.UnitX * -3;
            world.AddObject(model);
            
            model = new Object(new List<Mesh>(){DefaultModels.GetSphere()});
            material = model.Meshes[0].AsPBR(); 
            material.Textures.Add(TextureHelper.LoadFromImage(directory + "/plastic/albedo.png", ShadeType.TEXTURE_TYPE_ALBEDO));
            material.Textures.Add(TextureHelper.LoadFromImage(directory + "/plastic/ao.png", ShadeType.TEXTURE_TYPE_AO));
            material.Textures.Add(TextureHelper.LoadFromImage(directory + "/plastic/metallic.png", ShadeType.TEXTURE_TYPE_METALLIC));
            material.Textures.Add(TextureHelper.LoadFromImage(directory + "/plastic/normal.png", ShadeType.TEXTURE_TYPE_NORMAL));
            material.Textures.Add(TextureHelper.LoadFromImage(directory + "/plastic/roughness.png", ShadeType.TEXTURE_TYPE_ROUGHNESS));
            material.Albedo = Vector3.One;
            material.Metallic = 1;
            material.Roughness = 1;
            model.Position = Vector3.UnitX * 3;
            world.AddObject(model);
            
            model = new Object(new List<Mesh>(){DefaultModels.GetSphere()});
            material = model.Meshes[0].AsPBR(); 
            material.Textures.Add(TextureHelper.LoadFromImage(directory + "/rusted_iron/albedo.png", ShadeType.TEXTURE_TYPE_ALBEDO));
            material.Textures.Add(TextureHelper.LoadFromImage(directory + "/rusted_iron/ao.png", ShadeType.TEXTURE_TYPE_AO));
            material.Textures.Add(TextureHelper.LoadFromImage(directory + "/rusted_iron/metallic.png", ShadeType.TEXTURE_TYPE_METALLIC));
            material.Textures.Add(TextureHelper.LoadFromImage(directory + "/rusted_iron/normal.png", ShadeType.TEXTURE_TYPE_NORMAL));
            material.Textures.Add(TextureHelper.LoadFromImage(directory + "/rusted_iron/roughness.png", ShadeType.TEXTURE_TYPE_ROUGHNESS));
            material.Albedo = Vector3.One;
            material.Metallic = 1;
            material.Roughness = 1;
            model.Position = Vector3.UnitY * 3;
            world.AddObject(model);
            
            model = new Object(new List<Mesh>(){DefaultModels.GetSphere()});
            material = model.Meshes[0].AsPBR(); 
            material.Textures.Add(TextureHelper.LoadFromImage(directory + "/wall/albedo.png", ShadeType.TEXTURE_TYPE_ALBEDO));
            material.Textures.Add(TextureHelper.LoadFromImage(directory + "/wall/ao.png", ShadeType.TEXTURE_TYPE_AO));
            material.Textures.Add(TextureHelper.LoadFromImage(directory + "/wall/metallic.png", ShadeType.TEXTURE_TYPE_METALLIC));
            material.Textures.Add(TextureHelper.LoadFromImage(directory + "/wall/normal.png", ShadeType.TEXTURE_TYPE_NORMAL));
            material.Textures.Add(TextureHelper.LoadFromImage(directory + "/wall/roughness.png", ShadeType.TEXTURE_TYPE_ROUGHNESS));
            material.Albedo = Vector3.One;
            material.Metallic = 1;
            material.Roughness = 1;
            model.Position = Vector3.UnitY * -3;
            world.AddObject(model);
            
            model = new Object(new List<Mesh>(){DefaultModels.GetSphere()});
            material = model.Meshes[0].AsPBR();
            material.Albedo = Vector3.One;
            material.Metallic = 1;
            material.Roughness = 0;
            model.Position = new Vector3(-3, 3, 0);
            world.AddObject(model);
            
            model = new Object(new List<Mesh>(){DefaultModels.GetSphere()});
            material = model.Meshes[0].AsPBR();
            material.Albedo = Vector3.One;
            material.Metallic = 0.6f;
            material.Roughness = 1;
            model.Position = new Vector3(3, 3, 0);
            world.AddObject(model);
            
            model = new Object(new List<Mesh>(){DefaultModels.GetSphere()});
            material = model.Meshes[0].AsPBR();
            material.Albedo = Vector3.One;
            material.Metallic = 0.4f;
            material.Roughness = 0.3f;
            model.Position = new Vector3(3, -3, 0);
            world.AddObject(model);
            
            model = new Object(new List<Mesh>(){DefaultModels.GetSphere()});
            material = model.Meshes[0].AsPBR();
            material.Albedo = Vector3.One;
            material.Metallic = 0.01f;
            material.Roughness = 0.7f;
            model.Position = new Vector3(-3, -3, 0);
            world.AddObject(model);
            
            
        }
    }
}