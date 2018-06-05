using System.Collections.Generic;
using System.Collections.ObjectModel;
using FruckEngine.Objects;

namespace FruckEngine
{
    public class World
    {
        public Camera MainCamera { get; set; }

        private List<Object> _objects = new List<Object>();
        
        public ReadOnlyCollection<Object> Objects => _objects.AsReadOnly();

        public World(Camera mainCamera)
        {
            MainCamera = mainCamera;
        }

        public void AddObject(Object obj)
        {
            _objects.Add(obj);
            obj.Init();
        }
    }
}