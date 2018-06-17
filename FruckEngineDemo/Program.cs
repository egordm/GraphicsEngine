using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using FruckEngine;
using FruckEngine.Graphics;
using FruckEngine.Helpers;
using FruckEngine.Objects;
using FruckEngine.Structs;

namespace FruckEngineDemo
{
    internal class Program : CoolGame {
        private Shader MainShader;
        
        public static void Main(string[] args)
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo( "en-US" );
            using (var win = new Window(1280, 720, "Fruckenstein" ,new Program())) { win.Run(30.0, 60.0); }
        }

        public override void Init()
        {
            base.Init();

            /*var model = AssimpLoadHelper.LoadModel("Assets/models/cyborg/cyborg.obj", false);*/
            var model = new Object(new List<Mesh>(){DefaultModels.GetPlane(true)});
            World.AddObject(model);
            
            MainShader = DefaultShaders.CreateDebugDraw(true);
        }

        public override void Render() {
            base.Render();
            World.Draw(MainShader, new DrawProperties());
        }
    }
}