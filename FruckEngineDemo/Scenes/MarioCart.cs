using FruckEngine.Helpers;
using FruckEngine.Objects;
using OpenTK;
using FruckEngine.Game;

namespace FruckEngineDemo.Scenes {
    public class MarioCart : Scene {
        protected override void Init(World world) {
            world.Environment.AmbientLight = Vector3.One;
            world.Environment.Sun.Position = new Vector3(0.842257f, 0.5299188f, 0.0989397f);
            world.Environment.Sun.Density = 0.3f;
            world.Environment.Sun.Intensity = 1;
            
            var env = TextureHelper.LoadCubemapFromDir("Assets/cubemaps/Mountains", 15000);
            world.Environment.SetTexture(env, true);
            
            world.MainCamera.Position = new Vector3(-1.533677f, 7.852062f, 2.867414f);
            world.MainCamera.SetRotation(-13, -180);
            
            const string directory = "Assets/models/mario_circuit";
            var model = AssimpLoadHelper.LoadModel(directory + "/Mario Circuit.obj", true);

            foreach (var mesh in model.Meshes) {
                var material = mesh.AsPBR();
                material.Albedo = Vector3.One;
                material.Metallic = 0.4f;
                material.Roughness = 0.7f;
            }
            
            model.Scale = Vector3.One * 0.1f;
            world.AddObject(model);
            
            /*world.AddLight(new PointLight(  new Vector3(-20.0f, 50.0f, -45.0f), new Vector3(1.0342f, 1.0759f, 0.3f), 100000));
            world.AddLight(new PointLight(  new Vector3(-20.0f, 50.0f, 45.0f), Vector3.One, 10000));*/

        }
    }
}