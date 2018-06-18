using FruckEngine.Graphics;
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

        protected Light(LightType type, Vector3 color, float intensity) {
            Type = type;
            Color = color;
            Intensity = intensity;
        }

        public abstract void Apply(Shader shader, int index);
    }

    public class PointLight : Light {
        public Vector3 Position;

        public PointLight(Vector3 position, Vector3 color, float intensity) 
            : base(LightType.PointLight, color, intensity) {
            Position = position;
        }

        public override void Apply(Shader shader, int index) {
            string name = $"uPointLights[{index}].";
            shader.SetVec3(name + "position", Position);
            shader.SetVec3(name + "color", Color);
            shader.SetFloat(name + "intensity", Intensity);
        }
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