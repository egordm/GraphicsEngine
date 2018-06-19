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
        public World currentWorld { get; private set; }

        public SceneManager()
        {
            Scenes = new Dictionary<string, Scene>();
            loaded = new Dictionary<Scene, bool>();
            world = new Dictionary<Scene, World>();
            currentWorld = null;
        }

        public void Update(double dt)
        {
            if (currentScene == null || currentWorld == null) return;
            currentScene.Update(currentWorld, dt);
            currentWorld.Update(dt);
        }

        public void Load(string name, LoadAction action)
        {
            if (!Scenes.ContainsKey(name)) return;
            Scene s = Scenes[name];
            if (currentScene == s) return;
            if(action == LoadAction.SWITCH_UNLOAD)
            {
                world[s] = null;
                currentWorld = null;
                loaded[s] = false;
                GC.Collect();
            }
            currentScene = s;
            if(world.ContainsKey(s))
                if(world[s] != null)
                {
                    currentWorld = world[s];
                    return;
                }
            world[s] = new World();
            s.Init(world[s]);
            currentWorld = world[s];
            loaded[s] = true;
        }
    }
}