using FruckEngine.Helpers;
using FruckEngine.Objects;
using OpenTK;
using FruckEngine.Game;
using FruckEngine.Graphics;
using OpenTK.Graphics.OpenGL;

namespace FruckEngineDemo.Scenes {
    public class Tiki : Scene {
        protected override void Init(World world) {
            world.Environment.AmbientLight = Vector3.One*3;
            world.Environment.Sun.Position = Vector3.UnitY;
            world.Environment.Sun.Intensity = 1;
            
            var env = TextureHelper.LoadCubemapFromDir("Assets/cubemaps/Mountains", 20000);
            world.Environment.SetTexture(env, true);
            
            world.Environment.ColorLUT = new Texture();
            world.Environment.ColorLUT.SetFilters(TextureMinFilter.Linear, TextureMagFilter.Linear);
            TextureHelper.LoadFromImage(ref world.Environment.ColorLUT, "Assets/lut/pirate_lut.png");
            
            world.MainCamera.Position = new Vector3(2.28f, 0.89f, 7.20f);
            world.MainCamera.SetRotation(9, -158);
            world.MainCamera.FStop = world.MainCamera.FocalLength / 2;
            
            const string directory = "Assets/models/Pirate";
            var model = AssimpLoadHelper.LoadModel(directory + "/ship.obj", true);

            foreach (var mesh in model.Meshes) {
                var material = mesh.AsPBR();
                material.Metallic = 0.4f;
                material.Roughness = 0.7f;
            }
            
            //model.Rotation = Quaternion.FromAxisAngle(Vector3.UnitX, MathHelper.DegreesToRadians(-90));
            //model.Rotation *= Quaternion.FromAxisAngle(Vector3.UnitZ, MathHelper.DegreesToRadians(-90));
            model.Scale = Vector3.One * 0.5f;
            world.AddObject(model);
            
            //world.AddLight(new PointLight(  new Vector3(-20.0f, 50.0f, -45.0f), Vector3.One, 100000));
            //world.AddLight(new PointLight(  new Vector3(-20.0f, 50.0f, 45.0f), Vector3.One, 10000));

        }
    }
}