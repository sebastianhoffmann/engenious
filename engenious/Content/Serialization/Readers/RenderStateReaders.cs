using engenious.Graphics;

namespace engenious.Content.Serialization
{
    [ContentTypeReaderAttribute(typeof(RasterizerState))]
    public class RasterizerStateTypeReader : ContentTypeReader<RasterizerState>
    {
        #region implemented abstract members of ContentTypeReader

        public override RasterizerState Read(ContentManager manager, ContentReader reader)
        {
            if (reader.ReadBoolean())
                return null;
            var state = new RasterizerState
            {
                CullMode = (CullMode) reader.ReadUInt16(),
                FillMode = (PolygonMode) reader.ReadUInt16(),
                MultiSampleAntiAlias = reader.ReadBoolean(),
                ScissorTestEnable = reader.ReadBoolean()
            };
            return state;
        }

        #endregion
    }

    [ContentTypeReaderAttribute(typeof(DepthStencilState))]
    public class DepthStencilStateTypeReader : ContentTypeReader<DepthStencilState>
    {
        #region implemented abstract members of ContentTypeReader

        public override DepthStencilState Read(ContentManager manager, ContentReader reader)
        {
            if (reader.ReadBoolean())
                return null;
            var state = new DepthStencilState
            {
                DepthBufferEnable = reader.ReadBoolean(),
                DepthBufferWriteEnable = reader.ReadBoolean(),
                StencilEnable = reader.ReadBoolean(),
                ReferenceStencil = reader.ReadInt32(),
                StencilMask = reader.ReadInt32(),
                DepthBufferFunction = (DepthFunction) reader.ReadUInt16(),
                StencilFunction = (StencilFunction) reader.ReadUInt16(),
                StencilDepthBufferFail = (StencilOp) reader.ReadUInt16(),
                StencilFail = (StencilOp) reader.ReadUInt16(),
                StencilPass = (StencilOp) reader.ReadUInt16()
            };


            return state;
        }

        #endregion
    }

    [ContentTypeReaderAttribute(typeof(BlendState))]
    public class BlendStateTypeReader : ContentTypeReader<BlendState>
    {
        #region implemented abstract members of ContentTypeReader

        public override BlendState Read(ContentManager manager, ContentReader reader)
        {
            if (reader.ReadBoolean())
                return null;
            var state = new BlendState
            {
                AlphaBlendFunction = (BlendEquationMode) reader.ReadUInt16(),
                AlphaDestinationBlend = (BlendingFactorDest) reader.ReadUInt16(),
                AlphaSourceBlend = (BlendingFactorSrc) reader.ReadUInt16(),
                ColorBlendFunction = (BlendEquationMode) reader.ReadUInt16(),
                ColorDestinationBlend = (BlendingFactorDest) reader.ReadUInt16(),
                ColorSourceBlend = (BlendingFactorSrc) reader.ReadUInt16(),
                BlendFactor = reader.ReadColor()
            };


            return state;
        }

        #endregion
    }
}