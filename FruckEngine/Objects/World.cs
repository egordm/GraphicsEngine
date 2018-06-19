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
        private List<Light> _lights = new List<Light>();
        public Object Root;

        public ReadOnlyCollection<Light> Lights => _lights.AsReadOnly();

        public Environment Environment = new Environment();

        public World()
        {
            MainCamera = new Camera(Vector3.Zero, 0, 0, Vector3.UnitY);
            MainCamera.SetFOV(130);
            Root = new Object();
        }

        public void AddObject(Object obj, Object parent = null)
        {
            if (parent != null)
                parent.AddChild(obj);
            else Root.AddChild(obj);
            obj.Init();
        }
        
        public void AddLight(Light light)
        {
            _lights.Add(light);
        }

        public void Draw(Shader shader, DrawProperties properties) {
            var coordSystem = InitialCoordSystem();
            Root.Draw(coordSystem, shader, properties);
        }
        
        public CoordSystem InitialCoordSystem() {
            return new CoordSystem(MainCamera.GetProjectionMatrix(), MainCamera.GetViewMatrix(), Matrix4.Identity);
        }
    }
}