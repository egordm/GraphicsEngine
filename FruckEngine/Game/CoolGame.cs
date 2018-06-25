using OpenTK.Input;

namespace FruckEngine.Game {
    public class CoolGame : Game {
        protected float Velocity = 0.1f;
        protected float Sensitivity = 0.1f;
        
        public override void Render() {}

        public override void OnKeyboardUpdate(KeyboardState state) {
            base.OnKeyboardUpdate(state);
            // Move the camera with the keyboard. up down right left etc
            if (state[Key.W]) Scenes.CurrentWorld.MainCamera.Position += Scenes.CurrentWorld.MainCamera.Direction * Velocity;
            if (state[Key.S]) Scenes.CurrentWorld.MainCamera.Position -= Scenes.CurrentWorld.MainCamera.Direction * Velocity;
            if (state[Key.A]) Scenes.CurrentWorld.MainCamera.Position -= Scenes.CurrentWorld.MainCamera.Right * Velocity;
            if (state[Key.D]) Scenes.CurrentWorld.MainCamera.Position += Scenes.CurrentWorld.MainCamera.Right * Velocity;
            if (state[Key.Q]) Scenes.CurrentWorld.MainCamera.Position -= Scenes.CurrentWorld.MainCamera.Up * Velocity;
            if (state[Key.E]) Scenes.CurrentWorld.MainCamera.Position += Scenes.CurrentWorld.MainCamera.Up * Velocity;
        }

        public override void OnMouseMove(double dx, double dy) {
            base.OnMouseMove(dx, dy);
            
            dx *= Sensitivity;
            dy *= Sensitivity;
            
            // Rotate camera by delta mouse pos
            float yaw = (float) (Scenes.CurrentWorld.MainCamera.Yaw - dx);
            float pitch = (float) (Scenes.CurrentWorld.MainCamera.Pitch - dy);

            Scenes.CurrentWorld.MainCamera.SetRotation(pitch, yaw);
            //Console.WriteLine($"Pos: {World.MainCamera.Position} Rot: {World.MainCamera.Pitch}, {World.MainCamera.Yaw}");
        }
    }
}