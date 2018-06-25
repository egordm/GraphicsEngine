using System;
using OpenTK;

namespace FruckEngine {
    public class Camera {
        // Camera parameters
        public Vector3 Direction { get; private set; } = Vector3.UnitZ;
        public Vector3 Up { get; private set; }
        public Vector3 Right { get; private set; }
        public Vector3 WorldUp { get; private set; }
        public float Pitch { get; private set; }
        public float Yaw { get; private set; }
        public float Fovy { get; set; }
        public float Aspect { get; set; } = 16f / 9f;
        public float ZNear { get; set; } = 0.1f;
        public float ZFar { get; set; } = 100f;
        public float FocalLength = 28f;
        public float FStop = 28/4f;

        public Vector3 Position { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="position"></param>
        /// <param name="pitch"></param>
        /// <param name="yaw"></param>
        /// <param name="worldUp"></param>
        public Camera(Vector3 position, float pitch, float yaw, Vector3 worldUp) {
            WorldUp = worldUp;
            Pitch = pitch;
            Yaw = yaw;
            Position = position;
            UpdateDirection();
        }

        /// <summary>
        /// Gets view matrix for the graphics pipline from slides
        /// </summary>
        /// <returns></returns>
        public Matrix4 GetViewMatrix() {
            return Matrix4.LookAt(Position, Position + Direction, Up);
        }

        /// <summary>
        /// Gets projection matrix for the graphics pipline from slides
        /// </summary>
        /// <returns></returns>
        public Matrix4 GetProjectionMatrix() {
            var ret = Matrix4.CreatePerspectiveFieldOfView(Fovy, Aspect, ZNear, ZFar);
            return ret;
        }

        /// <summary>
        /// Creates a direction from a given rotation in degrees.
        /// Only 2 angles since no need for more and to avoid gimbal lock
        /// </summary>
        /// <param name="pitch"></param>
        /// <param name="yaw"></param>
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

        /// <summary>
        /// Updates camera rotation from given direction
        /// </summary>
        /// <param name="direction"></param>
        public void SetDirection(Vector3 direction) {
            Direction = direction.Normalized();
            Pitch = (float) Math.Asin(-Direction.Y);
            Yaw = (float) Math.Atan2(Direction.X, Direction.Z);
            UpdateDirection();
        }

        /// <summary>
        /// Sets field of view from a horizontal angle
        /// </summary>
        /// <param name="angle"></param>
        public void SetFOV(float angle) {
            Fovy = MathHelper.DegreesToRadians(angle) / Aspect;
        }

        /// <summary>
        /// Updates the up and right from given direction.
        /// </summary>
        private void UpdateDirection() {
            Right = Vector3.Cross(Direction, WorldUp).Normalized();
            Up = Vector3.Cross(Right, Direction).Normalized();
        }
    }
}