using System.Collections.Generic;
using System.Collections.ObjectModel;
using FruckEngine.Objects;
using FruckEngine.Structs;

namespace FruckEngine
{
    public class World
    {
        public Camera MainCamera { get; set; }

        private List<Object> _objects = new List<Object>();
        private List<Light> _lights = new List<Light>();
        
        public ReadOnlyCollection<Object> Objects => _objects.AsReadOnly();
        public ReadOnlyCollection<Light> Lights => _lights.AsReadOnly();

        public Environment Environment;

        public World(Camera mainCamera)
        {
            MainCamera = mainCamera;
        }

        public void AddObject(Object obj)
        {
            _objects.Add(obj);
            obj.Init();
        }
        
        public void AddLight(Light light)
        {
            _lights.Add(light);
        }
    }
}