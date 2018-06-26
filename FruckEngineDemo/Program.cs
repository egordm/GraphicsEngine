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
using System;

namespace FruckEngineDemo
{
    internal class Program : DeferredShadingGame {
        
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
            using (var win = new Window(1280, 720, "Fruck Engine Demo", new Program())) { win.Run(30.0, 60.0); }
        }

        public override void Init()
        {
            base.Init();
            Scenes.Scenes.Add("0", new Cody());
            Scenes.Scenes.Add("1", new StormTrooper());
            Scenes.Scenes.Add("2", new Cerberus());
            Scenes.Scenes.Add("3", new Yoda());
            Scenes.Scenes.Add("4", new MarioCart());
            Scenes.Scenes.Add("5", new Tiki());
            Scenes.Scenes.Add("6", new Car());
            Scenes.Scenes.Add("7", new Spheres());
            Scenes.Load("0", LoadAction.SWITCH);
        }

        public override void Update(double dt) {
            base.Update(dt);
        }

        public override void OnKeyboardUpdate(KeyboardState state) {
            base.OnKeyboardUpdate(state);

            for (int i = 0; i < SceneButtons.Count; i++) {
                if (state[SceneButtons[i]]) {
                    Scenes.Load("" + i, LoadAction.SWITCH);
                    break;
                }
            }
        }
    }
}