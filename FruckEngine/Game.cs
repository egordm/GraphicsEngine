using System.Drawing;
using FruckEngine.Structs;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace FruckEngine
{
    public interface IGame
    {
        void Init();
        void Update();
        void Render();
        void Resize(int width, int height);
        void Destroy();
    }
    
    public abstract class Game : IGame
    {
        public World World;
        
        protected int Width { get; set; }
        protected int Height { get; set; }

        public abstract void Init();
        public abstract void Update();

        public virtual void Render()
        {
            var matrix = new TransformMatrix(Matrix4.Identity, Matrix4.Identity, Matrix4.Identity);
            // TODO: render each and very object
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