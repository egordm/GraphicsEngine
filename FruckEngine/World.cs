using System.Collections.Generic;
using FruckEngine.Objects;

namespace FruckEngine
{
    public class World
    {
        public Camera MainCamera { get; set; }
        
        public List<Object> Objects { get; private set; } = new List<Object>();

        public World(Camera mainCamera)
        {
            MainCamera = mainCamera;
        }

        public void AddObject(Object obj)
        {
            Objects.Add(obj);
        }
    }
}