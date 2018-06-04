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
        public Scene Scene;
        
        protected int Width { get; set; }
        protected int Height { get; set; }

        public abstract void Init();
        public abstract void Update();
        public abstract void Render();

        public virtual void Resize(int width, int height)
        {
            Width = width;
            Height = height;
        }

        public virtual void Destroy() { }
    }
}