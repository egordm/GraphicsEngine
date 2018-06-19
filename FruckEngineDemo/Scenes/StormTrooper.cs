using FruckEngine.Helpers;
using FruckEngine.Objects;
using OpenTK;

namespace FruckEngineDemo.Scenes {
    public class StormTrooper : Scene {
        public override void Init(World world) {
            world.Environment.AmbientLight = Vector3.One;
            world.Environment.DirectionalLight.Intensity = 10000;

            var env = TextureHelper.LoadCubemapFromDir("Assets/cubemaps/Kiara", 30000);
            world.Environment.SetTexture(env, true);

            world.MainCamera.Position = new Vector3(0, 8, 15);
            world.MainCamera.SetRotation(0, -180);

            const string directory = "Assets/models/storm_trooper";
            var model = AssimpLoadHelper.LoadModel(directory + "/storm_trooper.obj", true);

            foreach (var mesh in model.Meshes) {
                var material = mesh.AsPBR();

                if (material.Tags.Contains("ArmNLeg") || material.Tags.Contains("Back_crotch") ||
                    material.Tags.Contains("Belt") || material.Tags.Contains("chest")) {
                    material.Albedo = material.Albedo;
                    /*material.Metallic = 1f;
                    material.Roughness = 0.2f;*/
                    material.Metallic = 0.0f;
                    material.Roughness = 1f;
                } else if(material.Tags.Contains("Helmet")) {
                    material.Albedo = material.Albedo;
                    material.Metallic = 0f;
                    material.Roughness = 0.2f;
                } else if (material.Tags.Contains("HandsNFeet")) {
                    material.Albedo = material.Albedo;
                    material.Metallic = 0f;
                    material.Roughness = 0.9f;
                } else {
                    material.Albedo = material.Albedo;
                    material.Metallic = 0f;
                    material.Roughness = 1f;
                }
            }


            model.Scale = Vector3.One * 0.8f;
            model.Rotation = Quaternion.FromAxisAngle(Vector3.UnitY, MathHelper.DegreesToRadians(-90));
            world.AddObject(model);

            /*world.AddLight(new PointLight(new Vector3(-20.0f, 50.0f, -45.0f), Vector3.One, 100000));
            world.AddLight(new PointLight(new Vector3(-20.0f, 50.0f, 45.0f), Vector3.One, 10000));*/
        }
    }
}