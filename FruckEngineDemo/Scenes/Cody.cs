using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FruckEngine.Objects;
using OpenTK;
using FruckEngine.Helpers;
using FruckEngine.Graphics;

namespace FruckEngineDemo.Scenes
{
    public class Cody : Scene
    {
        public override void Init(World world)
        {
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
            FruckEngine.Objects.Object sphere, child, grandchild;
            {
                const string directory = "Assets/models/frog";
                sphere = AssimpLoadHelper.LoadModel(directory + "/frog.obj", true);
                var material = sphere.Meshes[0].AsPBR();
                material.Textures.Clear();
                material.Textures.Add(TextureHelper.LoadFromImage(directory + "/diffuse.png", ShadeType.TEXTURE_TYPE_ALBEDO));
                material.Albedo = Vector3.One;
                material.Metallic = 0.99f;
                material.Roughness = 0.01f;
                sphere.Scale = Vector3.One * 0.1f;
                sphere.Rotation = Quaternion.FromAxisAngle(Vector3.UnitZ, 45f);
                world.AddObject(sphere);
            }
            {
                var sm = DefaultModels.GetSphere();
                child = new FruckEngine.Objects.Object();
                child.Meshes.Add(sm);
                var mat = sm.AsPBR();
                mat.Metallic = 0.9f;
                mat.Roughness = 0.5f;
                child.Position = new Vector3(0, 1, 0);
                child.Rotation = Quaternion.Identity;
                child.Scale = Vector3.One;
                world.AddObject(child, sphere);
            }
            {
                var sm = DefaultModels.GetSphere();
                grandchild = new FruckEngine.Objects.Object();
                grandchild.Meshes.Add(sm);
                var mat = sm.AsPBR();
                mat.Metallic = 0.9f;
                mat.Roughness = 0.5f;
                grandchild.Position = new Vector3(0, 1, 0);
                grandchild.Rotation = Quaternion.Identity;
                grandchild.Scale = Vector3.One;
                world.AddObject(grandchild, child);
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
    }
}