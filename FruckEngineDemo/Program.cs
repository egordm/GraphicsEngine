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
            new Cerberus(),
            new Yoda(),
            new StormTrooper(),
        };
        
        public List<Key> SceneButtons = new List<Key>() {
            Key.Number1,
            Key.Number2,
            Key.Number3,
            Key.Number4,
            Key.Number5,
            Key.Number6,
            Key.Number7,
            Key.Number8,
            Key.Number9,
            Key.Number0,
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
            
            SetScene(3);
        }

        public override void Update(double dt) {
            base.Update(dt);
            
            Scenes[CurrentScene].Update(World, dt);
        }

        public void SetScene(int i) {
            // TODO: WARNING!!!!!! Dont swicth between scenes that use different shading. unexpected results may occur
            if(i == CurrentScene || i >= Scenes.Count) return;
            CurrentScene = i;
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