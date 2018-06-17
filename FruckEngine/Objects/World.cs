using System.Collections.Generic;
using System.Collections.ObjectModel;
using FruckEngine.Graphics;
using FruckEngine.Objects;
using FruckEngine.Structs;
using OpenTK;

namespace FruckEngine.Objects
{
    public class World
    {
        public Camera MainCamera { get; set; }

        private List<Object> _objects = new List<Object>();
        private List<Light> _lights = new List<Light>();
        
        public ReadOnlyCollection<Object> Objects => _objects.AsReadOnly();
        public ReadOnlyCollection<Light> Lights => _lights.AsReadOnly();

        public Environment Environment = new Environment();

        public World()
        {
            MainCamera = new Camera(Vector3.Zero, 0, 0, Vector3.UnitY);
            MainCamera.SetFOV(100);
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

        public void Draw(Shader shader, DrawProperties properties) {
            var coordSystem = InitialCoordSystem();
            foreach (var o in Objects) o.Draw(coordSystem, shader, properties);
        }
        
        protected CoordSystem InitialCoordSystem() {
            return new CoordSystem(MainCamera.GetProjectionMatrix(), MainCamera.GetViewMatrix(), Matrix4.Identity);
        }
    }
}