using OpenTK;

namespace FruckEngine.Objects {
    public class Light {
        public Vector3 Position;
        public Vector3 Color;
        public float intensity;

        public Light(Vector3 position, Vector3 color, float intensity) {
            Position = position;
            Color = color;
            this.intensity = intensity;
        }
    }

    public class DirectionalLight : Light {
        public Vector3 Direction;

        public DirectionalLight(Vector3 direction, Vector3 color, float intensity) 
            : base(Vector3.Zero, color, intensity) {
            Direction = direction;
        }
    }
}