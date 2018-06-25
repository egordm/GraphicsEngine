using System;
using FruckEngine.Graphics;
using FruckEngine.Helpers;
using FruckEngine.Structs;
using OpenTK;

namespace FruckEngine.Objects {
    public enum LightType {
        PointLight,
        DirectionalLight
    }

    /// <summary>
    /// Basic light object with properties for different effects
    /// </summary>
    public abstract class Light {
        public LightType Type;
        public Vector3 Color;
        public float Intensity;
        public Vector3 Position;

        public bool HasGodRays = false;
        public float Density = 0.1f;
        public float BlurWidth = 0.6f;

        public bool Drawable = false;

        protected Light(LightType type, Vector3 color, Vector3 position, float intensity) {
            Type = type;
            Color = color;
            Intensity = intensity;
            Position = position;
        }
        
        /// <summary>
        /// Put light into a shader
        /// </summary>
        /// <param name="shader"></param>
        /// <param name="index"></param>
        public abstract void Apply(Shader shader, int index);
    }
    
    /// <summary>
    /// Point light
    /// </summary>
    public class PointLight : Light, IDrawable {
       
        public float DrawRadius = 3f;

        public PointLight(Vector3 position, Vector3 color, float intensity) 
            : base(LightType.PointLight, color, position, intensity) {
            Drawable = true;
        }

        public override void Apply(Shader shader, int index) {
            string name = $"uPointLights[{index}].";
            shader.SetVec3(name + "position", Position);
            shader.SetVec3(name + "color", Color);
            shader.SetFloat(name + "intensity", Intensity);
        }

        /// <summary>
        /// Draw light as a bright sphere
        /// </summary>
        /// <param name="coordSys"></param>
        /// <param name="shader"></param>
        /// <param name="properties"></param>
        public void Draw(CoordSystem coordSys, Shader shader, DrawProperties properties) {
            var matrix = Matrix4.CreateScale(DrawRadius);
            matrix *= Matrix4.CreateTranslation(Position);
            coordSys.Model = matrix * coordSys.Model;
            coordSys.Apply(shader);
            
            ((PBRMaterial)DisplaySphere.Material).Albedo = Color * 10;
            DisplaySphere.Draw(shader, properties);
        }
        
        protected static Mesh DisplaySphere = DefaultModels.GetSphere();
    }

    /// <summary>
    /// Directional light
    /// </summary>
    public class DirectionalLight : Light {

        public DirectionalLight(Vector3 position, Vector3 color, float intensity) 
            : base(LightType.DirectionalLight, color, position, intensity) {
        }

        public override void Apply(Shader shader, int index) {
            const string name = "uDirectionalLight.";
            shader.SetVec3(name + "direction", -Position.Normalized());
            shader.SetVec3(name + "color", Color);
            shader.SetFloat(name + "intensity", Intensity);
        }
    }

    /// <summary>
    /// Sun = A special directional light
    /// </summary>
    public class Sun : DirectionalLight {
        public float Radius = 0.3f;

        public Sun(Vector3 direction, Vector3 color, float intensity) : base(direction, color, intensity) {
            HasGodRays = true;
            Density = 0.3f;
            BlurWidth = 0.9f;
        }
        
        
    }
}