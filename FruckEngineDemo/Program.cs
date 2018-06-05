using System;
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
            using (var win = new Window(1280, 720, "Fruckenstein" ,new Program())) { win.Run(30.0, 60.0); }
        }

        public override void Init()
        {
            World = new World(new Camera(new Vector3(0, 0, 0), Quaternion.Identity));
            
            var teapotShader = new Shader("Assets/shaders/vs.glsl", "Assets/shaders/fs_test.glsl");
            teapotShader.AddAttributeVar("vPosition");
            teapotShader.AddAttributeVar("vUV");
            teapotShader.AddAttributeVar("vNormal");
            teapotShader.AddUniformVar("transform");
            var teapot = new MeshObject(MeshCreator.FromWavefront("Assets/models/teapot.obj"), teapotShader);
            teapot.Position = new Vector3(0, 0, -50);
            World.Objects.Add(teapot);
            
            base.Init();
        }

        public override void Update()
        {
            base.Update();
            var keyState = Keyboard.GetState();
            var delta = 0.2f;
            if (keyState.IsKeyDown(Key.Left)) World.MainCamera.Rotation += new Quaternion(1, 0, 0, delta);
            if (keyState.IsKeyDown(Key.Right)) World.MainCamera.Rotation += new Quaternion(1, 0, 0, -delta);
            if (keyState.IsKeyDown(Key.Up)) World.MainCamera.Rotation += new Quaternion(0, 1, 0, delta);
            if (keyState.IsKeyDown(Key.Down)) World.MainCamera.Rotation += new Quaternion(0, 1, 0, -delta);
            if (keyState.IsKeyDown(Key.W)) World.MainCamera.Rotation += new Quaternion(0, 0, 1, delta);
            if (keyState.IsKeyDown(Key.S)) World.MainCamera.Rotation += new Quaternion(0, 0, 1, -delta);
        }
    }
}