using System.Collections.Generic;
using FruckEngine.Graphics;
using FruckEngine.Helpers;
using FruckEngine.Objects;
using OpenTK;
using FruckEngine.Game;

namespace FruckEngineDemo.Scenes {
    public class Car : Scene {
        protected override void Init(World world) {
            world.Environment.AmbientLight = Vector3.One*0.0f;
            world.Environment.Sun.Position = new Vector3(-0.6794168f, 0.7181264f, 0.1506226f);
            world.Environment.Sun.Intensity = 0;
            world.Environment.Sun.HasGodRays = false;
            
            
            var env = TextureHelper.LoadCubemapFromDir("Assets/cubemaps/Shanhai", 108000);
            world.Environment.SetTexture(env, true);
            
            world.MainCamera.Position = new Vector3(0, 0, 15);
            world.MainCamera.SetRotation(0, -180);
            
            const string directory = "Assets/models/car";
            var model = AssimpLoadHelper.LoadModel(directory + "/car.obj", true);
            var material = model.Meshes[0].AsPBR();
            material.Textures.Add(TextureHelper.LoadFromImage(directory+"/default_BaseColor.png", ShadeType.TEXTURE_TYPE_ALBEDO));
            material.Textures.Add(TextureHelper.LoadFromImage(directory+"/default_Metallic.png", ShadeType.TEXTURE_TYPE_METALLIC));
            material.Textures.Add(TextureHelper.LoadFromImage(directory+"/default_Normal_DirectX.png", ShadeType.TEXTURE_TYPE_NORMAL));
            material.Textures.Add(TextureHelper.LoadFromImage(directory+"/default_Roughness.png", ShadeType.TEXTURE_TYPE_ROUGHNESS));
            material.Textures.Add(TextureHelper.LoadFromImage(directory+"/car2_low_default_AmbientOcclusion.png", ShadeType.TEXTURE_TYPE_AO));
            material.Albedo = Vector3.One;
            material.Metallic = 1f;
            material.Roughness = 1f;
            model.Scale = Vector3.One * 0.8f;
            world.AddObject(model);
            
            //world.AddLight(new PointLight(  new Vector3(-20.0f, 50.0f, -45.0f), new Vector3(1.0342f, 1.0759f, 0.3f), 100000));
            //world.AddLight(new PointLight(  new Vector3(-20.0f, 50.0f, 45.0f), Vector3.One, 10000));

           
        }
    }
}