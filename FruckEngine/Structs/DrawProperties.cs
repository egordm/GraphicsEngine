namespace FruckEngine.Structs {
    public struct DrawProperties {
        public MaterialType MaterialType;
        public bool Shade;

        public DrawProperties(MaterialType materialType = MaterialType.Any, bool shade = false) {
            MaterialType = materialType;
            Shade = shade;
        }
    }
}