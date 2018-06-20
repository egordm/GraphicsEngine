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
        private bool broken = false;
        private Scene prev;

        public SceneManager()
        {
            Scenes = new Dictionary<string, Scene>();
            loaded = new Dictionary<Scene, bool>();
            world = new Dictionary<Scene, World>();
            CurrentWorld = null;
            prev = null;
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
            Load(s, action);
        }

        public void Load(Scene s, LoadAction action)
        {
            if (s == null) return;
            if (currentScene == s) return;
            if (action == LoadAction.SWITCH_UNLOAD)
                Destroy(currentScene);
            currentScene = s;
            if (world.ContainsKey(s))
                if (world[s] != null)
                {
                    CurrentWorld = world[s];
                    prev = currentScene;
                    return;
                }
            world[s] = new World(this);
            s.Init(world[s]);
            CurrentWorld = world[s];
            loaded[s] = true;
            if (broken)
                Repair();
            else
                prev = currentScene;
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

        public void ImBroken()
        {
            broken = true;
        }

        private void Repair()
        {
            if(prev == null)
            {
                Console.WriteLine("Scene was broken. Nothing loaded");
                world[currentScene] = null;
                loaded[currentScene] = false;
                currentScene = null;
                CurrentWorld = null;
            }
            Console.WriteLine("Scene was broken. Loading previous scene.");
            Load(prev, LoadAction.SWITCH_UNLOAD);
        }
    }
}