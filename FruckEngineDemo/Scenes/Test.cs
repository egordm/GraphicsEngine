using FruckEngine.Helpers;
using FruckEngine.Objects;
using OpenTK;

namespace FruckEngineDemo.Scenes {
    public class Test : Scene {
        public override void Init(World world) {
            world.Environment.AmbientLight = Vector3.One;
            world.Environment.DirectionalLight.Intensity = 1;
            
            var env = TextureHelper.LoadCubemapFromDir("Assets/cubemaps/Mountains", 30000);
            world.Environment.SetTexture(env, true);
            
            world.MainCamera.Position = new Vector3(0, 8, 15);
            world.MainCamera.SetRotation(0, -180);
            
            const string directory = "Assets/models/kiz";
            var model = AssimpLoadHelper.LoadModel(directory + "/kiz.fbx", true);

            foreach (var mesh in model.Meshes) {
                var material = mesh.AsPBR();
                material.Albedo = material.Albedo;
                material.Metallic = 0.4f;
                material.Roughness = 0.7f;
            }
            
            model.Rotation = Quaternion.FromAxisAngle(Vector3.UnitX, MathHelper.DegreesToRadians(-90));
            model.Rotation *= Quaternion.FromAxisAngle(Vector3.UnitZ, MathHelper.DegreesToRadians(-90));
            model.Scale = Vector3.One * 0.5f;
            world.AddObject(model);
            
            //world.AddLight(new PointLight(  new Vector3(-20.0f, 50.0f, -45.0f), Vector3.One, 100000));
            //world.AddLight(new PointLight(  new Vector3(-20.0f, 50.0f, 45.0f), Vector3.One, 10000));

        }
    }
}