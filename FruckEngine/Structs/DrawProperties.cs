namespace FruckEngine.Structs {
    /// <summary>
    /// A comtainer the has information about current render pass setting to avoid rendering things that are
    /// incompatable with current render pass
    /// </summary>
    public struct DrawProperties {
        public MaterialType MaterialType;
        public bool Shade;

        public DrawProperties(MaterialType materialType = MaterialType.Any, bool shade = false) {
            MaterialType = materialType;
            Shade = shade;
        }
    }
}