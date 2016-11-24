using System;
using System.Collections.Generic;
using System.Linq;

// ReSharper disable CompareOfFloatsByEqualityOperator
// ReSharper disable PossibleLossOfFraction

namespace engenious.Graphics
{
    internal class SpriteBatcher : IDisposable
    {
        public class BatchItem
        {
            internal Vector2 TexTopLeft;
            internal Vector2 TexBottomRight;
            internal Vector3[] Positions;
            internal Color Color;
            internal Texture2D Texture;
            internal float SortingKey;

            private void InitBatchItem(Vector2 position, Color color, float rotation, Vector2 origin, Vector2 size,
                SpriteBatch.SpriteEffects effects, float layerDepth, SpriteBatch.SpriteSortMode sortMode,
                Vector4 tempText)
            {
                if ((effects & SpriteBatch.SpriteEffects.FlipVertically) != 0)
                {
                    TexTopLeft.X = tempText.X + tempText.Z;
                    TexBottomRight.X = tempText.X;
                }
                else
                {
                    TexTopLeft.X = tempText.X;
                    TexBottomRight.X = tempText.X + tempText.Z;
                }
                if ((effects & SpriteBatch.SpriteEffects.FlipHorizontally) != 0)
                {
                    TexTopLeft.Y = tempText.Y + tempText.W;
                    TexBottomRight.Y = tempText.Y;
                }
                else
                {
                    TexTopLeft.Y = tempText.Y;
                    TexBottomRight.Y = tempText.Y + tempText.W;
                }

                Positions = new Vector3[4];
                Positions[0] = new Vector3(-origin.X, -origin.Y, layerDepth);
                Positions[1] = new Vector3(-origin.X + size.X, -origin.Y, layerDepth);
                Positions[2] = new Vector3(-origin.X, -origin.Y + size.Y, layerDepth);
                Positions[3] = new Vector3(-origin.X + size.X, -origin.Y + size.Y, layerDepth);

                if (rotation != 0.0f || ((rotation = rotation % (float) (Math.PI * 2)) != 0.0f))
                {
                    float cosR = (float) Math.Cos(rotation); //TODO: correct rotation
                    float sinR = (float) Math.Sin(rotation);
                    for (int i = 0; i < Positions.Length; i++)
                    {
                        Positions[i] = new Vector3(Positions[i].X * cosR - Positions[i].Y * sinR,
                            Positions[i].Y * cosR + Positions[i].X * sinR, Positions[i].Z);
                    }
                }
                for (int i = 0; i < Positions.Length; i++)
                {
                    Positions[i] += new Vector3(origin.X + position.X, origin.Y + position.Y, 0.0f);
                }
                Color = color;

                switch (sortMode)
                {
                    case SpriteBatch.SpriteSortMode.BackToFront:
                        SortingKey = -layerDepth;
                        break;
                    case SpriteBatch.SpriteSortMode.FrontToBack:
                        SortingKey = layerDepth;
                        break;
                    case SpriteBatch.SpriteSortMode.Texture:
                        SortingKey = Texture.GetHashCode();
                        break;
                }
            }

            public BatchItem(Texture2D texture, Vector2 position, RectangleF? sourceRectangle, Color color,
                float rotation, Vector2 origin, Vector2 size, SpriteBatch.SpriteEffects effects, float layerDepth,
                SpriteBatch.SpriteSortMode sortMode)
            {
                Texture = texture;
                Vector4 tempText;
                if (sourceRectangle.HasValue)
                    tempText = new Vector4(sourceRectangle.Value.X, sourceRectangle.Value.Y, sourceRectangle.Value.Width,
                        sourceRectangle.Value.Height);
                else
                    tempText = new Vector4(0, 0, 1, 1);

                InitBatchItem(position, color, rotation, origin, new Vector2(size.X * tempText.Z, size.Y * tempText.W),
                    effects, layerDepth, sortMode, tempText);
            }

            public BatchItem(Texture2D texture, Vector2 position, Rectangle? sourceRectangle, Color color,
                float rotation, Vector2 origin, Vector2 size, SpriteBatch.SpriteEffects effects, float layerDepth,
                SpriteBatch.SpriteSortMode sortMode)
            {
                Texture = texture;
                Vector4 tempText;
                if (sourceRectangle.HasValue)
                    tempText = new Vector4((float) sourceRectangle.Value.X / (float) texture.Width,
                        sourceRectangle.Value.Y / texture.Height, (float) sourceRectangle.Value.Width / texture.Width,
                        (float) sourceRectangle.Value.Height / texture.Height);
                else
                    tempText = new Vector4(0, 0, 1, 1);
                InitBatchItem(position, color, rotation, origin, new Vector2(size.X, size.Y), effects, layerDepth,
                    sortMode, tempText);
            }
        }

        private readonly DynamicVertexBuffer _vertexBuffer;
        private readonly VertexPositionColorTexture[] _vertexData = new VertexPositionColorTexture[MaxBatch * 4];
        private readonly DynamicIndexBuffer _indexBuffer;
        private readonly ushort[] _indexData = new ushort[MaxBatch * 6];
        private const int MaxBatch = 256;
        private SpriteBatch.SpriteSortMode _sortMode;
        private readonly GraphicsDevice _graphicsDevice;

        public SpriteBatcher(GraphicsDevice graphicsDevice)
        {
            _graphicsDevice = graphicsDevice;
            Batches = new List<BatchItem>(MaxBatch);
            _vertexBuffer = new DynamicVertexBuffer(graphicsDevice, VertexPositionColorTexture.VertexDeclaration,
                4 * MaxBatch);
            _indexBuffer = new DynamicIndexBuffer(graphicsDevice, DrawElementsType.UnsignedShort, 6 * MaxBatch);
        }

        public void Begin(SpriteBatch.SpriteSortMode sortMode)
        {
            Batches.Clear();
            _sortMode = sortMode;
        }

        internal SamplerState SamplerState;

        public void Flush(Effect effect, Texture texture, int batch, int batchCount)
        {
            if (batchCount == 0)
                return;


            //graphicsDevice.DrawPrimitives(OpenTK.Graphics.OpenGL4.PrimitiveType.Qu
            _vertexBuffer.SetData<VertexPositionColorTexture>(
                VertexPositionColorTexture.VertexDeclaration.VertexStride * batch * 4, _vertexData, 0, batchCount * 4,
                VertexPositionColorTexture.VertexDeclaration.VertexStride);
            _indexBuffer.SetData<ushort>(sizeof(short) * 6 * batch, _indexData, 0, batchCount * 6);


            _graphicsDevice.VertexBuffer = _vertexBuffer;
            _graphicsDevice.IndexBuffer = _indexBuffer;
            _graphicsDevice.Textures[0] = texture;
            //graphicsDevice.SamplerStates[0] = samplerState;
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                _graphicsDevice.DrawIndexedPrimitives(PrimitiveType.Triangles, 0, 0, batchCount * 4, 0, batchCount * 2);
            }

            _graphicsDevice.IndexBuffer = null;
            _graphicsDevice.VertexBuffer = null;
        }

        public void End(Effect effect)
        {
            if (Batches.Count == 0)
                return;


            if (_sortMode != SpriteBatch.SpriteSortMode.Immediate && _sortMode != SpriteBatch.SpriteSortMode.Deffered)
            {
                Batches.Sort((x, y) =>
                {
                    int first = y.SortingKey.CompareTo(x.SortingKey);
                    return first;
                });
            }


            //Array.Sort (Batches, 0, Batches.Count);
            Texture currentTexture = Batches.First().Texture;
            int batchStart = 0, batchCount = 0;

            ushort bufferIndex = 0;
            ushort indicesIndex = 0;
            foreach (var item in Batches)
            {
                if (item.Texture != currentTexture || batchCount == MaxBatch)
                {
                    Flush(effect, currentTexture, batchStart, batchCount);
                    currentTexture = item.Texture;
                    batchStart = batchCount = 0;
                    indicesIndex = bufferIndex = 0;
                }

                _indexData[indicesIndex++] = (ushort) (bufferIndex + 0);
                _indexData[indicesIndex++] = (ushort) (bufferIndex + 1);
                _indexData[indicesIndex++] = (ushort) (bufferIndex + 2);

                _indexData[indicesIndex++] = (ushort) (bufferIndex + 1);
                _indexData[indicesIndex++] = (ushort) (bufferIndex + 3);
                _indexData[indicesIndex++] = (ushort) (bufferIndex + 2);

                _vertexData[bufferIndex++] = new VertexPositionColorTexture(item.Positions[0], item.Color,
                    item.TexTopLeft);
                _vertexData[bufferIndex++] = new VertexPositionColorTexture(item.Positions[1], item.Color,
                    new Vector2(item.TexBottomRight.X, item.TexTopLeft.Y));
                _vertexData[bufferIndex++] = new VertexPositionColorTexture(item.Positions[2], item.Color,
                    new Vector2(item.TexTopLeft.X, item.TexBottomRight.Y));
                _vertexData[bufferIndex++] = new VertexPositionColorTexture(item.Positions[3], item.Color,
                    item.TexBottomRight);
                batchCount++;
            }
            if (batchCount > 0)
                Flush(effect, currentTexture, batchStart, batchCount);
        }

        public List<BatchItem> Batches { get; private set; }

        public void Dispose()
        {
            _vertexBuffer.Dispose();
            _indexBuffer.Dispose();
        }
    }
}