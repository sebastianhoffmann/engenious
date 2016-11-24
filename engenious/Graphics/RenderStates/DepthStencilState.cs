﻿namespace engenious.Graphics
{
    public class DepthStencilState : GraphicsResource
    {
        public static readonly DepthStencilState Default;
        public static readonly DepthStencilState DepthRead;
        public static readonly DepthStencilState None;

        static DepthStencilState()
        {
            Default = new DepthStencilState
            {
                DepthBufferEnable = true,
                DepthBufferWriteEnable = true
            };

            DepthRead = new DepthStencilState
            {
                DepthBufferEnable = true,
                DepthBufferWriteEnable = false
            };

            None = new DepthStencilState
            {
                DepthBufferEnable = false,
                DepthBufferWriteEnable = false
            };
        }

        public DepthStencilState()
        {
            DepthBufferEnable = true;
            DepthBufferWriteEnable = true;
        }

        public bool DepthBufferEnable { get; set; }

        public bool DepthBufferWriteEnable { get; set; }

        public int ReferenceStencil { get; set; }

        public DepthFunction DepthBufferFunction { get; set; }

        public bool StencilEnable { get; set; }

        public StencilFunction StencilFunction { get; set; }

        public int StencilMask { get; set; }

        public StencilOp StencilDepthBufferFail { get; set; }

        public StencilOp StencilFail { get; set; }

        public StencilOp StencilPass { get; set; }
    }
}