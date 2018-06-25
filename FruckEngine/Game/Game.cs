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
        public World World => Scenes.CurrentWorld;
        public double Time;

        public int Width { get; private set; }
        public int Height { get; private set; }

        public virtual void Init() { }

        /// <summary>
        /// Update function. ALl the input and scheduled movement should happen here
        /// </summary>
        /// <param name="dt"></param>
        public virtual void Update(double dt) {
            Time += dt;
            Scenes.Update(dt);
        }

        /// <summary>
        /// Render function. All the drawing should happen here.
        /// </summary>
        public abstract void Render();

        /// <summary>
        /// Resize everything 
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public virtual void Resize(int width, int height) {
            Width = width;
            Height = height;
            if (Scenes.CurrentScene != null && Scenes.CurrentScene.IsLoaded) {
                Scenes.CurrentWorld.MainCamera.Aspect = width / (float)height;
            }
        }

        /// <summary>
        /// Deletion of buffers etc should happen here
        /// </summary>
        public virtual void Destroy() { }

        /// <summary>
        /// Clear function called before the render call. 
        /// </summary>
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