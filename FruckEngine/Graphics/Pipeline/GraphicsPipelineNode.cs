namespace FruckEngine.Graphics.Pipeline {
    public abstract class GraphicsPipelineNode {
        protected int Width, Height;

        protected GraphicsPipelineNode(int width, int height) {
            Width = width;
            Height = height;
        }

        public virtual void Resize(int width, int height) {
            Width = width;
            Height = height;
        }
    }
}