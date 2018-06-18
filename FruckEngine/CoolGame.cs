using System;
using OpenTK.Input;

namespace FruckEngine {
    public class CoolGame : Game {
        protected float Velocity = 0.1f;
        protected float Sensitivity = 0.1f;
        
        public override void Render() {}

        public override void OnKeyboardUpdate(KeyboardState state) {
            base.OnKeyboardUpdate(state);
            if (state[Key.W]) World.MainCamera.Position += World.MainCamera.Direction * Velocity;
            if (state[Key.S]) World.MainCamera.Position -= World.MainCamera.Direction * Velocity;
            if (state[Key.A]) World.MainCamera.Position -= World.MainCamera.Right * Velocity;
            if (state[Key.D]) World.MainCamera.Position += World.MainCamera.Right * Velocity;
            if (state[Key.Q]) World.MainCamera.Position -= World.MainCamera.Up * Velocity;
            if (state[Key.E]) World.MainCamera.Position += World.MainCamera.Up * Velocity;
        }

        public override void OnMouseMove(double dx, double dy) {
            base.OnMouseMove(dx, dy);
            
            dx *= Sensitivity;
            dy *= Sensitivity;

            float yaw = (float) (World.MainCamera.Yaw - dx);
            float pitch = (float) (World.MainCamera.Pitch - dy);
            
            World.MainCamera.SetRotation(pitch, yaw);
            //Console.WriteLine($"Pos: {World.MainCamera.Position} Rot: {World.MainCamera.Pitch}, {World.MainCamera.Yaw}");
        }
    }
}