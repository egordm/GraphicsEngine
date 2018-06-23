using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using FruckEngine.Objects;

namespace FruckEngine.Game {
    public abstract class Scene {
        public World World { get; private set; } = null;

        public bool IsLoaded => World != null;

        public void Load() {
            World = new World();
            Init(World);
        }

        public void Destroy() {
            World = null;
            // TODO: moet meer dan dat. Delete buffers enz
        }
        
        protected abstract void Init(World world);

        public virtual void Update(World world, double dt) {  }
    }

    public enum LoadAction {
        SWITCH,
        SWITCH_UNLOAD
    }

    public class SceneManager {
        public Dictionary<string, Scene> Scenes;
        public Scene CurrentScene { get; private set; } = null;
        public World CurrentWorld => CurrentScene != null ? CurrentScene.World : null;

        public SceneManager() {
            Scenes = new Dictionary<string, Scene>();
        }

        public void Update(double dt) {
            if (CurrentScene == null || CurrentWorld == null) return;
            CurrentScene.Update(CurrentWorld, dt);
            CurrentWorld.Update(dt);
        }

        public void Load(string name, LoadAction action) {
            if (!Scenes.ContainsKey(name) || CurrentScene == Scenes[name]) return;

            var scene = Scenes[name];
            if (!scene.IsLoaded) {
                try {
                    scene.Load();
                } catch (Exception e) {
                    Console.WriteLine("Scene was broken. Loading previous scene.");
                    Console.WriteLine(e);
                    return;
                }
            }

            if (action == LoadAction.SWITCH_UNLOAD) Destroy(CurrentScene);
            CurrentScene = scene;
        }

        public void Destroy(Scene s) {
            s.Destroy();
            GC.Collect();
        }
    }
}