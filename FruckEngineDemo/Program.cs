using System;
using System.Globalization;
using System.Threading;
using FruckEngine;
using FruckEngine.Graphics;
using FruckEngine.Helpers;
using FruckEngine.Objects;
using OpenTK;
using OpenTK.Input;

namespace FruckEngineDemo
{
    internal class Program : Game
    {
        
        public static void Main(string[] args)
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo( "en-US" );
            using (var win = new Window(1280, 720, "Fruckenstein" ,new Program())) { win.Run(30.0, 60.0); }
        }

        public override void Init()
        {
            base.Init();
            World = new World(new Camera(new Vector3(0, 0, 0), Quaternion.Identity));

            var tex = TextureHelper.GetNormalNull();
            
            Console.WriteLine("dd");
        }

        public override void Update(double dt)
        {
            base.Update(dt);
            

        }
    }
}