using FruckEngine;
using OpenTK;

namespace FruckEngineDemo
{
    internal class Program : Game
    {
        public static void Main(string[] args)
        {
            using (var win = new Window(1280, 720, "Fruckenstein" ,new Program())) { win.Run(30.0, 60.0); }
        }

        public override void Init()
        {
            World = new World(new Camera(new Vector3(0, 1, 0), Quaternion.Identity));
        }

        public override void Update()
        {
            //throw new System.NotImplementedException();
        }
    }
}