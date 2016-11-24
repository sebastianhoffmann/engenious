using OpenTK.Graphics.OpenGL4;
using System;
using System.Runtime.InteropServices;

namespace engenious.Graphics
{
    public class Texture2DArray : Texture
    {
        private int _texture;

        public Texture2DArray(GraphicsDevice graphicsDevice, int levels, int width, int height, int layers)
            : base(graphicsDevice)
        {
            ThreadingHelper.BlockOnUIThread(() =>
            {
                _texture = GL.GenTexture();

                GL.BindTexture(TextureTarget.Texture2DArray, _texture);

                GL.TexStorage3D(TextureTarget3d.Texture2DArray, levels, SizedInternalFormat.Rgba8, width, height,
                    Math.Max(layers, 1));
            });
            Width = width;
            Height = height;
            LayerCount = layers;
        }

        public Texture2DArray(GraphicsDevice graphicsDevice, int levels, int width, int height, Texture2D[] textures)
            : this(graphicsDevice, levels, width, height, textures.Length)
        {
            ThreadingHelper.BlockOnUIThread(() =>
            {
                int layer = 0;

                bool createMipMaps = false;
                foreach (var text in textures)
                {
                    if (text.LevelCount < levels)
                        createMipMaps = true;
                    int mipWidth = text.Width, mipHeight = text.Height;
                    for (int i = 0; i < 1 && createMipMaps || !createMipMaps && i < levels; i++)
                    {
                        GL.CopyImageSubData(text.Texture, ImageTarget.Texture2D, i, 0, 0, 0, _texture,
                            ImageTarget.Texture2DArray, i, 0, 0, layer, mipWidth, mipHeight, 1);
                        mipWidth /= 2;
                        mipHeight /= 2;
                    }
                    layer++;
                }
                if (createMipMaps)
                {
                    GL.GenerateMipmap(GenerateMipmapTarget.Texture2DArray);
                }
                SetDefaultTextureParameters();
            });
        }

        private static void SetDefaultTextureParameters()
        {
            GL.TexParameter(TextureTarget.Texture2DArray, TextureParameterName.TextureMinFilter, (int) All.Linear);
            GL.TexParameter(TextureTarget.Texture2DArray, TextureParameterName.TextureMagFilter, (int) All.Linear);
            GL.TexParameter(TextureTarget.Texture2DArray, TextureParameterName.TextureMagFilter, (int) All.Linear);
            GL.TexParameter(TextureTarget.Texture2DArray, TextureParameterName.TextureMinFilter,
                (int) All.LinearMipmapLinear);
        }

        public int Width { get; private set; }
        public int Height { get; private set; }
        public int LayerCount { get; private set; }

        internal override void Bind()
        {
            GL.BindTexture(TextureTarget.Texture2DArray, _texture);
        }

        internal override void SetSampler(SamplerState state)
        {
            //TODO:throw new NotImplementedException();
        }

        public void SetData<T>(T[] data, int layer, int level = 0) where T : struct
        {
            var handle = GCHandle.Alloc(data, GCHandleType.Pinned);
            ThreadingHelper.BlockOnUIThread(() =>
            {
                Bind();
                var pxType = PixelType.UnsignedByte;
                if (typeof(T) == typeof(Color))
                    pxType = PixelType.Float;

                GL.TexSubImage3D(TextureTarget.Texture2DArray, level, 0, 0, layer, Width, Height, 1,
                    OpenTK.Graphics.OpenGL4.PixelFormat.Bgra, pxType, handle.AddrOfPinnedObject());
            });
            handle.Free();
        }
    }
}