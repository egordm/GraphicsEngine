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

    public abstract class Light {
        public LightType Type;
        public Vector3 Color;
        public float Intensity;
        public bool Drawable = false;

        protected Light(LightType type, Vector3 color, float intensity) {
            Type = type;
            Color = color;
            Intensity = intensity;
        }

        public abstract void Apply(Shader shader, int index);
    }
    
    public class PointLight : Light, IDrawable {
        public Vector3 Position;
        public float DrawRadius = 3f;

        public PointLight(Vector3 position, Vector3 color, float intensity) 
            : base(LightType.PointLight, color, intensity) {
            Position = position;
            Drawable = true;
        }

        public override void Apply(Shader shader, int index) {
            string name = $"uPointLights[{index}].";
            shader.SetVec3(name + "position", Position);
            shader.SetVec3(name + "color", Color);
            shader.SetFloat(name + "intensity", Intensity);
        }

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

    public class DirectionalLight : Light {
        public Vector3 Direction;

        public DirectionalLight(Vector3 direction, Vector3 color, float intensity) 
            : base(LightType.DirectionalLight, color, intensity) {
            Direction = direction;
        }

        public override void Apply(Shader shader, int index) {
            const string name = "uDirectionalLight.";
            shader.SetVec3(name + "direction", Direction);
            shader.SetVec3(name + "color", Color);
            shader.SetFloat(name + "intensity", Intensity);
        }
    }
}