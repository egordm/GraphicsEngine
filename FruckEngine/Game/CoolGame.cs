using OpenTK.Input;

namespace FruckEngine.Game {
    public class CoolGame : Game {
        protected float Velocity = 0.1f;
        protected float Sensitivity = 0.1f;
        
        public override void Render() {}

        public override void OnKeyboardUpdate(KeyboardState state) {
            base.OnKeyboardUpdate(state);
            if (state[Key.W]) Scenes.currentWorld.MainCamera.Position += Scenes.currentWorld.MainCamera.Direction * Velocity;
            if (state[Key.S]) Scenes.currentWorld.MainCamera.Position -= Scenes.currentWorld.MainCamera.Direction * Velocity;
            if (state[Key.A]) Scenes.currentWorld.MainCamera.Position -= Scenes.currentWorld.MainCamera.Right * Velocity;
            if (state[Key.D]) Scenes.currentWorld.MainCamera.Position += Scenes.currentWorld.MainCamera.Right * Velocity;
            if (state[Key.Q]) Scenes.currentWorld.MainCamera.Position -= Scenes.currentWorld.MainCamera.Up * Velocity;
            if (state[Key.E]) Scenes.currentWorld.MainCamera.Position += Scenes.currentWorld.MainCamera.Up * Velocity;
        }

        public override void OnMouseMove(double dx, double dy) {
            base.OnMouseMove(dx, dy);
            
            dx *= Sensitivity;
            dy *= Sensitivity;

            float yaw = (float) (Scenes.currentWorld.MainCamera.Yaw - dx);
            float pitch = (float) (Scenes.currentWorld.MainCamera.Pitch - dy);

            Scenes.currentWorld.MainCamera.SetRotation(pitch, yaw);
            //Console.WriteLine($"Pos: {World.MainCamera.Position} Rot: {World.MainCamera.Pitch}, {World.MainCamera.Yaw}");
        }
    }
}