﻿namespace engenious.Content
{
    public enum TextureContentFormat
    {
        Png = 0,
        Jpg = 1,
        DXT1 = OpenTK.Graphics.OpenGL4.PixelInternalFormat.CompressedRgbaS3tcDxt1Ext,
        DXT3 = OpenTK.Graphics.OpenGL4.PixelInternalFormat.CompressedRgbaS3tcDxt3Ext,
        DXT5 = OpenTK.Graphics.OpenGL4.PixelInternalFormat.CompressedRgbaS3tcDxt5Ext,
    }
}