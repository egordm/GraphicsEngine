using System.Collections.Generic;
using System.Collections.ObjectModel;
using FruckEngine.Graphics;
using FruckEngine.Objects;
using FruckEngine.Structs;
using OpenTK;
using FruckEngine.Game;

namespace FruckEngine.Objects
{
    public class World
    {
        public Camera MainCamera { get; set; }
        private List<Light> _lights = new List<Light>();
        private Object Root;

        public ReadOnlyCollection<Light> Lights => _lights.AsReadOnly();

        public Environment Environment = new Environment();

        public World()
        {
            MainCamera = new Camera(Vector3.Zero, 0, 0, Vector3.UnitY);
            MainCamera.SetFOV(130);
            Root = new Object();
        }

        public void AddObject(Object obj)
        {
            Root.Children.Add(obj);
            obj.Init();
        }
        
        public void AddLight(Light light)
        {
            _lights.Add(light);
        }

        public void Update(double dt)
        {
            Root.Update(dt);
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