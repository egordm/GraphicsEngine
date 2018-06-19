using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using FruckEngine;
using FruckEngine.Game;
using FruckEngine.Graphics;
using FruckEngine.Helpers;
using FruckEngine.Objects;
using FruckEngine.Structs;
using FruckEngineDemo.Scenes;
using OpenTK;
using OpenTK.Input;

namespace FruckEngineDemo
{
    internal class Program : DeferredShadingGame {
        
        public List<Scene> Scenes = new List<Scene>() {
            new Cody(),
            new Cerberus()
        };
        
        public List<Key> SceneButtons = new List<Key>() {
            Key.Keypad1,
            Key.Keypad2,
            Key.Keypad3,
            Key.Keypad4,
            Key.Keypad5,
            Key.Keypad6,
            Key.Keypad7,
            Key.Keypad8,
            Key.Keypad9,
            Key.Keypad0,
        };

        public int CurrentScene = -1;
        
        public static void Main(string[] args)
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo( "en-US" );
            using (var win = new Window(1280, 720, "Fruck Engine Demo" ,new Program())) { win.Run(30.0, 60.0); }
        }

        public override void Init()
        {
            base.Init();
            
            SetScene(0);
        }

        public override void Update(double dt) {
            base.Update(dt);
            
            Scenes[CurrentScene].Update(World, dt);
        }

        public void SetScene(int i) {
            // TODO: WARNING!!!!!! Dont swicth between scenes that use different shading. unexpected results may occur
            if(i == CurrentScene || i >= Scenes.Count) return;
            CurrentScene = 0;
            World = new World();
            Scenes[CurrentScene].Init(World);
        }

        public override void OnKeyboardUpdate(KeyboardState state) {
            base.OnKeyboardUpdate(state);

            for (int i = 0; i < SceneButtons.Count; i++) {
                if (state[SceneButtons[i]]) {
                    SetScene(i);
                    break;
                }
            }
        }
    }
}