using System;
using OpenTK;

namespace FruckEngine {
    public class Camera {
        public Vector3 Direction { get; private set; }
        public Vector3 Up { get; private set; }
        public Vector3 Right { get; private set; }
        public Vector3 WorldUp { get; private set; }
        public float Pitch { get; private set; }
        public float Yaw { get; private set; }
        public float Fovy { get; set; }
        public float Aspect { get; set; } = 16f / 9f;
        public float ZNear { get; set; } = 0.1f;
        public float ZFar { get; set; } = 100f;

        public Vector3 Position { get; set; }

        public Camera(Vector3 position, float pitch, float yaw, Vector3 worldUp) {
            WorldUp = worldUp;
            Pitch = pitch;
            Yaw = yaw;
            Position = position;
            UpdateDirection();
        }

        public Matrix4 GetViewMatrix() {
            return Matrix4.LookAt(Position, Position + Direction, Up);
        }

        public Matrix4 GetProjectionMatrix() {
            var ret = Matrix4.CreatePerspectiveFieldOfView(Fovy, Aspect, ZNear, ZFar);
            ret *= Matrix4.CreateScale(1, -1, 1);
            return ret;
        }

        public void SetRotation(float pitch = 0, float yaw = 0) {
            Pitch = pitch;
            Yaw = yaw;
            var direction = new Vector3 {
                X = (float) Math.Sin(MathHelper.DegreesToRadians(Yaw)) *
                    (float) Math.Cos(MathHelper.DegreesToRadians(Pitch)),
                Y = (float) Math.Sin(MathHelper.DegreesToRadians(Pitch)),
                Z = (float) Math.Cos(MathHelper.DegreesToRadians(Yaw)) *
                    (float) Math.Cos(MathHelper.DegreesToRadians(Pitch))
            };
            Direction = direction;
            UpdateDirection();
        }

        public void SetDirection(Vector3 direction) {
            Direction = direction.Normalized();
            Pitch = (float) Math.Asin(-Direction.Y);
            Yaw = (float) Math.Atan2(Direction.X, Direction.Z);
            UpdateDirection();
        }

        public void SetFOV(float angle) {
            Fovy = MathHelper.DegreesToRadians(angle) / Aspect;
        }

        private void UpdateDirection() {
            Right = Vector3.Cross(Direction, WorldUp).Normalized();
            Up = Vector3.Cross(Right, Direction).Normalized();
        }
    }
}