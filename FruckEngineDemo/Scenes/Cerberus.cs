using System.Collections.Generic;
using FruckEngine.Graphics;
using FruckEngine.Helpers;
using FruckEngine.Objects;
using OpenTK;

namespace FruckEngineDemo.Scenes {
    public class Cerberus : Scene {
        public override void Init(World world) {
            world.Environment.AmbientLight = Vector3.One;
            world.Environment.DirectionalLight.Intensity = 0;
            
            var env = TextureHelper.LoadFromCubemap(new List<string> {
                "Assets/cubemaps/Home/_posx.hdr",
                "Assets/cubemaps/Home/_negx.hdr",
                "Assets/cubemaps/Home/_posy.hdr",
                "Assets/cubemaps/Home/_negy.hdr",
                "Assets/cubemaps/Home/_posz.hdr",
                "Assets/cubemaps/Home/_negz.hdr"
            });
            world.Environment.SetTexture(env, true);
            
            world.MainCamera.Position = new Vector3(0, 0, 35);
            world.MainCamera.SetRotation(0, -180);

            const string directory = "Assets/models/Cerberus_by_Andrew_Maximov";
            var model = AssimpLoadHelper.LoadModel(directory+"/Cerberus_LP.FBX", true);
            var material = model.Meshes[0].AsPBR();
            material.Textures.Clear();
            material.Textures.Add(TextureHelper.LoadFromImage(directory + "/Textures/Cerberus_A.tga", ShadeType.TEXTURE_TYPE_ALBEDO));
            material.Textures.Add(TextureHelper.LoadFromImage(directory + "/Textures/Cerberus_M.tga", ShadeType.TEXTURE_TYPE_METALLIC));
            material.Textures.Add(TextureHelper.LoadFromImage(directory + "/Textures/Cerberus_N.tga", ShadeType.TEXTURE_TYPE_NORMAL));
            material.Textures.Add(TextureHelper.LoadFromImage(directory+ "/Textures/Cerberus_R.tga", ShadeType.TEXTURE_TYPE_ROUGHNESS));
            material.Albedo = Vector3.One;
            material.Metallic = 1;
            material.Roughness = 1;
            model.Scale = Vector3.One * 0.2f;
            model.Rotation = Quaternion.FromAxisAngle(Vector3.UnitX, MathHelper.DegreesToRadians(-90));
            model.Rotation *= Quaternion.FromAxisAngle(Vector3.UnitZ, MathHelper.DegreesToRadians(-90));
            world.AddObject(model);
            
            var lights = new List<Vector3>(){
                new Vector3(-20.0f, 20.0f, 45.0f),
                new Vector3(20.0f, 20.0f, 45.0f),
                new Vector3(-20.0f, -20.0f, 45.0f),
                new Vector3(20.0f, -20.0f, 45.0f),
            };

            foreach (var lightPos in lights) {
                world.AddLight(new PointLight(lightPos, Vector3.One, 15000));
            }
        }
    }
}