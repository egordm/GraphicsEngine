using System.Drawing;
using FruckEngine.Objects;
using FruckEngine.Structs;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace FruckEngine
{
    public interface IGame
    {
        void Init();
        void Update(double dt);
        void Render();
        void Clear();
        void Resize(int width, int height);
        void Destroy();
    }
    
    public abstract class Game : IGame
    {
        public World World;
        
        protected int Width { get; set; }
        protected int Height { get; set; }

        public virtual void Init()
        {
        }

        public virtual void Update(double dt)
        {
            foreach (var o in World.Objects) o.Update(dt);
        }

        public virtual void Render()
        {
            var matrix = new TransformMatrix(Matrix4.Identity, World.MainCamera.GetMatrix(), Matrix4.Identity);

            foreach (var o in World.Objects) (o as IRenderable)?.Render(matrix);
        }

        public virtual void Resize(int width, int height)
        {
            Width = width;
            Height = height;
        }

        public virtual void Destroy() { }

        /// <summary>
        /// Clear the screen
        /// </summary>
        public virtual void Clear()
        {
            GL.ClearColor(Color.Black);
            GL.Enable(EnableCap.Texture2D);
            GL.Enable(EnableCap.DepthTest);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
        }
    }
}