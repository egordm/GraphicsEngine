using System;
using System.Collections.Generic;
using FruckEngine.Helpers;
using FruckEngine.Objects;
using OpenTK;
using FruckEngine.Game;
using FruckEngine.Graphics;
using Object = FruckEngine.Objects.Object;

namespace FruckEngineDemo.Scenes {
    public class DiscoDragon : Scene {
        public Random Random = new Random();
        public List<Tuple<Vector3, float>> NextColor = new List<Tuple<Vector3, float>>();
        public float Area = 40;
        public double Time = 0;
        public float Speed = 0.3f;
        const int lightCount = 12;
        
        protected override void Init(World world) {
            world.Environment.AmbientLight = Vector3.One * 0.3f;
            world.Environment.Sun.Intensity = 0;
            world.Environment.Sun.HasGodRays = false;

            var env = TextureHelper.LoadCubemapFromDir("Assets/cubemaps/Shanhai", 100000);
            world.Environment.SetTexture(env, true);

            world.MainCamera.Position = new Vector3(0, 8, 15);
            world.MainCamera.SetRotation(0, -180);
            world.Velocity = 0.3f;
            
            var model = new Object(new List<Mesh>(){DefaultModels.GetPlane(false)});
            model.Scale = new Vector3(50, 1, 50);
            model.Meshes[0].AsPBR().Roughness = 0.5f;
            model.Meshes[0].AsPBR().Metallic = 0.5f;
            world.AddObject(model);

            const string directory = "Assets/models/dragon";
            model = AssimpLoadHelper.LoadModel(directory + "/stanford.obj", true);
            var material = model.Meshes[0].AsPBR(); 
            material.Albedo = new Vector3(1, 1f, 1f);
            material.Metallic = 0.3f;
            material.Roughness = 0.6f;

            model.Scale = Vector3.One * 2f;
            //model.Rotation = Quaternion.FromAxisAngle(Vector3.UnitY, MathHelper.DegreesToRadians(-90));
            world.AddObject(model);

            
            for (int i = 0; i < lightCount; i++) {
                var pos = new Vector3((float) Math.Sin(Math.PI*2 / lightCount * i) * Area, 10.0f, (float) Math.Cos(Math.PI*2/lightCount * i) * Area);
                var color = new Vector3((float)Random.Next(1000)/1000, (float)Random.Next(1000)/1000, (float)Random.Next(1000)/1000).Normalized();
                var intensity = (float)Random.Next(1000) / 1000 * 3000 + 800;
                NextColor.Add(new Tuple<Vector3, float>(color, intensity));
                world.AddLight(new PointLight(pos, color, intensity));
            }

            
            //world.AddLight(new PointLight(new Vector3(-20.0f, 50.0f, 45.0f), Vector3.One, 1000));
        }

        public override void Update(World world, double dt) {
            base.Update(world, dt);
            Time += dt;

            for (int i = 0; i < lightCount; i++) {
                var pos = new Vector3((float) Math.Sin(Math.PI * 2 / lightCount * i +  Time * Speed) * Area,
                (float) Math.Sin(Math.PI * 2 / lightCount * i * 20 +  Time * Speed) * 5 + 16,
                    (float) Math.Cos(Math.PI * 2 / lightCount * i + Time * Speed) * Area);
                world.Lights[i].Position = pos;
            }
        }
    }
}