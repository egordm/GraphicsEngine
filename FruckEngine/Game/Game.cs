using System.Drawing;
using FruckEngine.Graphics;
using FruckEngine.Objects;
using FruckEngine.Structs;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;

namespace FruckEngine.Game {
    public abstract class Game {
        public SceneManager Scenes = new SceneManager();
        public double Time;

        public int Width { get; private set; }
        public int Height { get; private set; }

        public virtual void Init() { }

        public virtual void Update(double dt) {
            Time += dt;
            Scenes.Update(dt);
        }

        public abstract void Render();

        public virtual void Resize(int width, int height) {
            Width = width;
            Height = height;
            if(Scenes.currentWorld != null)
                Scenes.currentWorld.MainCamera.Aspect = width / (float)height;
        }

        public virtual void Destroy() { }

        public virtual void Clear() {
            GL.ClearColor(Color.Black);
            GL.Enable(EnableCap.Texture2D);
            GL.Enable(EnableCap.DepthTest);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
        }
        
        public virtual void OnMouseMove(double dx, double dy) { }

        public virtual void OnMouseScroll(double ds) { }

        public virtual void OnKeyboardUpdate(KeyboardState state) {}
    }
}