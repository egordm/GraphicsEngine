namespace FruckEngine
{
    public class World
    {
        public Camera MainCamera { get; set; }

        public World(Camera mainCamera)
        {
            MainCamera = mainCamera;
        }
    }
}