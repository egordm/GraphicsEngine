using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using FruckEngine.Graphics;
using FruckEngine.Objects;
using FruckEngine.Structs;
using OpenTK;
using FruckEngine.Game;
using Environment = FruckEngine.Structs.Environment;

namespace FruckEngine.Objects {
    /// <summary>
    /// World that holds every model that is relavent to the game at the current moment
    /// </summary>
    public class World {
        /// <summary>
        /// World has a main camera which will be the primary render target
        /// </summary>
        public Camera MainCamera { get; set; }
        /// <summary>
        /// We store the lights separately obtain them quickly for render
        /// </summary>
        private List<Light> _lights = new List<Light>();
        /// <summary>
        /// Root Scene graph node
        /// </summary>
        public Object Root { get; private set; }

        /// <summary>
        /// Readonly list with lights
        /// </summary>
        public ReadOnlyCollection<Light> Lights => _lights.AsReadOnly();

        /// <summary>
        /// World environment wihth all the info about environment map and lighting
        /// </summary>
        public Environment Environment = new Environment();
        
        //Temporary
        public float Velocity = 0.1f;


        public World() {
            // Create default settings
            MainCamera = new Camera(Vector3.Zero, 0, 0, Vector3.UnitY);
            MainCamera.SetFOV(130); // Human fov
            Root = new Object();
        }
        
        /// <summary>
        /// Add object to the root node
        /// </summary>
        /// <param name="obj"></param>
        public void AddObject(Object obj) {
            Root.Children.Add(obj);
            obj.Init();
        }

        /// <summary>
        /// Add light to scene
        /// </summary>
        /// <param name="light"></param>
        public void AddLight(Light light) {
            _lights.Add(light);
        }

        /// <summary>
        /// Update the children
        /// </summary>
        /// <param name="dt"></param>
        public void Update(double dt) {
            Root.Update(dt);
        }

        /// <summary>
        /// Draw children and lights
        /// </summary>
        /// <param name="shader"></param>
        /// <param name="properties"></param>
        public void Draw(Shader shader, DrawProperties properties) {
            var coordSystem = InitialCoordSystem();
            Root.Draw(coordSystem, shader, properties);

            foreach (var light in Lights) {
                if (light.Type == LightType.PointLight && light.Drawable) {
                    ((PointLight) light).Draw(coordSystem, shader, properties);
                }
            }
        }

        /// <summary>
        /// Get world coordinate system
        /// </summary>
        /// <returns></returns>
        public CoordSystem InitialCoordSystem() {
            return new CoordSystem(MainCamera.GetProjectionMatrix(), MainCamera.GetViewMatrix(), Matrix4.Identity);
        }
    }
}