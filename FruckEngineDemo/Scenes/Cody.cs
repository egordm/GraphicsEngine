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
            {
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
            }
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
                sphere.Position = Vector3.UnitY * 0;
                world.AddObject(sphere);
            }
            {
                const string directory = "Assets/models/frog";
                child = AssimpLoadHelper.LoadModel(directory + "/frog.obj", true);
                var material = sphere.Meshes[0].AsPBR();
                material.Textures.Clear();
                material.Textures.Add(TextureHelper.LoadFromImage(directory + "/diffuse.png", ShadeType.TEXTURE_TYPE_ALBEDO));
                material.Albedo = Vector3.One;
                material.Metallic = 0.99f;
                material.Roughness = 0.01f;
                child.Scale = Vector3.One;
                //child.Rotation = Quaternion.FromAxisAngle(Vector3.UnitZ, 45f);
                child.Position = Vector3.UnitY * 20;
                world.AddObject(child, sphere);
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