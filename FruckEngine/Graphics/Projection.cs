using FruckEngine.Helpers;
using FruckEngine.Structs;

namespace FruckEngine.Graphics {
    public static class Projection {
        private static Mesh ProjectionPlane = null;
        private static Mesh ProjectionCube = null;

        public static void ProjectPlane() {
            if (ProjectionPlane == null) ProjectionPlane = DefaultModels.GetPlane(true);
            ProjectionPlane.Draw(null, MaterialType.Any);
        }
        
        public static void ProjectCube() {
            if (ProjectionCube == null) ProjectionCube = DefaultModels.GetCube();
            ProjectionCube.Draw(null, MaterialType.Any);
        }
    }
}