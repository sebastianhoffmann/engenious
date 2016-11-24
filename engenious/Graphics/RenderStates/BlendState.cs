namespace engenious.Graphics
{
    public class BlendState
    {
        public static readonly BlendState AlphaBlend;
        public static readonly BlendState Additive;
        public static readonly BlendState NonPremultiplied;
        public static readonly BlendState Opaque;

        static BlendState()
        {
            AlphaBlend = new BlendState();

            Additive = new BlendState
            {
                ColorSourceBlend = BlendingFactorSrc.One,
                AlphaSourceBlend = BlendingFactorSrc.One,
                ColorDestinationBlend = BlendingFactorDest.OneMinusSrcColor,
                AlphaDestinationBlend = BlendingFactorDest.OneMinusSrcColor
            };
            //TODO: verify?

            NonPremultiplied = new BlendState
            {
                ColorSourceBlend = BlendingFactorSrc.SrcAlpha,
                AlphaSourceBlend = BlendingFactorSrc.SrcAlpha,
                ColorDestinationBlend = BlendingFactorDest.OneMinusSrcAlpha,
                AlphaDestinationBlend = BlendingFactorDest.OneMinusSrcAlpha
            };

            Opaque = new BlendState
            {
                ColorSourceBlend = BlendingFactorSrc.One,
                AlphaSourceBlend = BlendingFactorSrc.One,
                ColorDestinationBlend = BlendingFactorDest.Zero,
                AlphaDestinationBlend = BlendingFactorDest.Zero
            };

            //GL.BlendEquationSeparate(
        }

        public BlendState()
        {
            //GL.BlendFunc(BlendingFactorSrc.One,#
            ColorSourceBlend = BlendingFactorSrc.SrcAlpha;
            AlphaSourceBlend = BlendingFactorSrc.SrcAlpha;
            ColorDestinationBlend = BlendingFactorDest.OneMinusSrcAlpha;
            AlphaDestinationBlend = BlendingFactorDest.OneMinusSrcAlpha;

            ColorBlendFunction = BlendEquationMode.FuncAdd;
            AlphaBlendFunction = BlendEquationMode.FuncAdd;
        }

        public BlendingFactorSrc ColorSourceBlend { get; set; }

        public BlendingFactorSrc AlphaSourceBlend { get; set; }

        public BlendingFactorDest ColorDestinationBlend { get; set; }

        public BlendingFactorDest AlphaDestinationBlend { get; set; }

        public BlendEquationMode AlphaBlendFunction { get; set; }

        public BlendEquationMode ColorBlendFunction { get; set; }

        public Color BlendFactor { get; set; }
    }
}