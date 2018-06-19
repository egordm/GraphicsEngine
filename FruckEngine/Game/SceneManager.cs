using System;
using System.Collections.Generic;
using FruckEngine.Objects;

namespace FruckEngine.Game
{
    public abstract class Scene
    {
        public abstract void Init(World world);

        public virtual void Update(World world, double dt) { }
    }

    public enum LoadAction
    {
        SWITCH, SWITCH_UNLOAD
    }

    public class SceneManager
    {
        public Dictionary<string, Scene> Scenes;
        private Dictionary<Scene, bool> loaded;
        private Dictionary<Scene, World> world;
        private Scene currentScene = null;
        public World CurrentWorld { get; private set; }

        public SceneManager()
        {
            Scenes = new Dictionary<string, Scene>();
            loaded = new Dictionary<Scene, bool>();
            world = new Dictionary<Scene, World>();
            CurrentWorld = null;
        }

        public void Update(double dt)
        {
            if (currentScene == null || CurrentWorld == null) return;
            currentScene.Update(CurrentWorld, dt);
            CurrentWorld.Update(dt);
        }

        public void Load(string name, LoadAction action)
        {
            if (!Scenes.ContainsKey(name)) return;
            Scene s = Scenes[name];
            if (currentScene == s) return;
            if (action == LoadAction.SWITCH_UNLOAD)
                Destroy(s);
            currentScene = s;
            if(world.ContainsKey(s))
                if(world[s] != null)
                {
                    CurrentWorld = world[s];
                    return;
                }
            world[s] = new World();
            s.Init(world[s]);
            CurrentWorld = world[s];
            loaded[s] = true;
        }

        public void Destroy(Scene s)
        {
            if (!loaded.ContainsKey(s)) return;
            if (loaded[s] == false) return;
            world[s] = null;
            CurrentWorld = null;
            loaded[s] = false;
            GC.Collect();
        }
    }
}