namespace engenious.Graphics
{
    public class RasterizerState : GraphicsResource
    {
        public static readonly RasterizerState CullNone;
        public static readonly RasterizerState CullClockwise;
        public static readonly RasterizerState CullCounterClockwise;

        static RasterizerState()
        {
            CullNone = new RasterizerState
            {
                CullMode = CullMode.None,
                FillMode = PolygonMode.Fill
            };

            CullClockwise = new RasterizerState
            {
                CullMode = CullMode.Clockwise,
                FillMode = PolygonMode.Fill
            };

            CullCounterClockwise = new RasterizerState
            {
                CullMode = CullMode.CounterClockwise,
                FillMode = PolygonMode.Fill
            };
        }

        public RasterizerState()
        {
            CullMode = CullMode.None;
            FillMode = PolygonMode.Fill;
        }

        public CullMode CullMode { get; set; }

        public PolygonMode FillMode { get; set; }

        public bool MultiSampleAntiAlias { get; set; }

        public bool ScissorTestEnable { get; set; }
    }
}