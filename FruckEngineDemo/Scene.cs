using FruckEngine.Objects;

namespace FruckEngineDemo {
    public abstract class Scene {
        public abstract void Init(World world);
        
        public virtual void Update(World world, double dt) {}
    }
}