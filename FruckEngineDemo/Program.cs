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
        private MeshObject teapot;
        
        public static void Main(string[] args)
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo( "en-US" );
            using (var win = new Window(1280, 720, "Fruckenstein" ,new Program())) { win.Run(30.0, 60.0); }
        }

        public override void Init()
        {
            base.Init();
            World = new World(new Camera(new Vector3(0, 0, 0), Quaternion.Identity));

            /*teapot = new MeshObject(MeshCreator.FromWavefront("Assets/models/teapot.obj"), new MaterialShader());
            teapot.Material.Texture = Texture.LoadTexture("Assets/textures/wood.jpg");
            teapot.Position = new Vector3(0, -4, -15);*/
            
            teapot = new MeshObject(MeshCreator.FromWavefront("Assets/models/frog.obj"), new MaterialShader());
            teapot.Material.Texture = Texture.LoadTexture("Assets/textures/frog.jpg");
            teapot.Position = new Vector3(0, -4, -15);
            teapot.Scale = Vector3.One * 0.2f;
            World.AddObject(teapot);
        }

        public override void Update(double dt)
        {
            base.Update(dt);
            
            var keyState = Keyboard.GetState();
            var delta = 0.02f;
            if (keyState.IsKeyDown(Key.Down)) World.MainCamera.Rotation *= new Quaternion(delta, 0, 0);
            if (keyState.IsKeyDown(Key.Up)) World.MainCamera.Rotation *= new Quaternion(-delta, 0, 0);
            if (keyState.IsKeyDown(Key.Right)) World.MainCamera.Rotation *= new Quaternion(0, delta, 0);
            if (keyState.IsKeyDown(Key.Left)) World.MainCamera.Rotation *= new Quaternion(0, -delta, 0);
            if (keyState.IsKeyDown(Key.W)) World.MainCamera.Rotation *= new Quaternion(0, 0, delta);
            if (keyState.IsKeyDown(Key.S)) World.MainCamera.Rotation *= new Quaternion(0, 0, -delta);

            var rotationSpeed = 0.5f;
            teapot.Rotation *= new Quaternion(0, rotationSpeed * (float)dt, 0);
        }
    }
}