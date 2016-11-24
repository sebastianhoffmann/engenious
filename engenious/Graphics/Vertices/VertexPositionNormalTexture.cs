﻿using System.Runtime.InteropServices;

namespace engenious.Graphics
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct VertexPositionNormalTexture : IVertexType
    {
        public static readonly VertexDeclaration VertexDeclaration;

        static VertexPositionNormalTexture()
        {
            var elements = new[]
            {
                new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
                new VertexElement(12, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0),
                new VertexElement(24, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0)
            };
            var declaration = new VertexDeclaration(elements);
            VertexDeclaration = declaration;
        }

        VertexDeclaration IVertexType.VertexDeclaration => VertexDeclaration;

        public VertexPositionNormalTexture(Vector3 position, Vector3 normal, Vector2 textureCoord)
        {
            Position = position;
            Normal = normal;
            TextureCoordinate = textureCoord;
        }

        public Vector3 Position { get; private set; }
        public Vector3 Normal { get; private set; }
        public Vector2 TextureCoordinate { get; private set; }
    }
}