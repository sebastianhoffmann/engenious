namespace engenious.Graphics
{
    public class Mesh : GraphicsResource
    {
        public Mesh(GraphicsDevice graphicsDevice)
            : base(graphicsDevice)
        {
        }

        public int PrimitiveCount { get; set; }

        public VertexBuffer Vb { get; set; }

        public BoundingBox BoundingBox { get; internal set; }

        public void Draw()
        {
            GraphicsDevice.VertexBuffer = Vb;
            GraphicsDevice.DrawPrimitives(PrimitiveType.Triangles, 0, PrimitiveCount);
        }
    }
}