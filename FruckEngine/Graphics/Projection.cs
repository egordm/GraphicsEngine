using FruckEngine.Helpers;
using FruckEngine.Structs;

namespace FruckEngine.Graphics {
    /// <summary>
    /// Primitives that are used as render object. For example plane for post effects
    /// </summary>
    public static class Projection {
        private static Mesh ProjectionPlane = null;
        private static Mesh ProjectionCube = null;
        private static Mesh ProjectionSphere = null;

        public static void ProjectPlane() {
            if (ProjectionPlane == null) {
                ProjectionPlane = DefaultModels.GetPlane(true);
                ProjectionPlane.Indestructible = true;
            }
            ProjectionPlane.Draw(null, new DrawProperties(MaterialType.Any, false));
        }
        
        public static void ProjectCube() {
            if (ProjectionCube == null) {
                ProjectionCube = DefaultModels.GetCube();
                ProjectionPlane.Indestructible = true;
            }
            ProjectionCube.Draw(null, new DrawProperties(MaterialType.Any, false));
        }
        
        public static void ProjectSphere() {
            if (ProjectionSphere == null) {
                ProjectionSphere = DefaultModels.GetSphere();
                ProjectionPlane.Indestructible = true;
            }
            ProjectionSphere.Draw(null, new DrawProperties(MaterialType.Any, false));
        }
    }
}