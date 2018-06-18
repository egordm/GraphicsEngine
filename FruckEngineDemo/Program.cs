using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using FruckEngine;
using FruckEngine.Game;
using FruckEngine.Graphics;
using FruckEngine.Helpers;
using FruckEngine.Objects;
using FruckEngine.Structs;
using OpenTK;

namespace FruckEngineDemo
{
    internal class Program : DeferredShadingGame {
        
        public static void Main(string[] args)
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo( "en-US" );
            using (var win = new Window(1280, 720, "Fruckenstein" ,new Program())) { win.Run(30.0, 60.0); }
        }

        public override void Init()
        {
            base.Init();
            
            var env = TextureHelper.LoadFromCubemap(new List<string> {
                "Assets/cubemaps/Home/_posx.hdr",
                "Assets/cubemaps/Home/_negx.hdr",
                "Assets/cubemaps/Home/_posy.hdr",
                "Assets/cubemaps/Home/_negy.hdr",
                "Assets/cubemaps/Home/_posz.hdr",
                "Assets/cubemaps/Home/_negz.hdr"
            });
            World.Environment.SetTexture(env, true);

            //var model = AssimpLoadHelper.LoadModel("Assets/models/cyborg/cyborg.obj", false);
            var model = new Object(new List<Mesh>(){DefaultModels.GetSphere()});
            //model.Meshes[0].AsPBR().Metallic = 0.7f;
            //model.Meshes[0].AsPBR().Roughness = 0.4f;
            World.AddObject(model);
            
            
            World.MainCamera.Position = new Vector3(0,0, -3);
            World.MainCamera.SetDirection(model.Position - World.MainCamera.Position);
        }
    }
}