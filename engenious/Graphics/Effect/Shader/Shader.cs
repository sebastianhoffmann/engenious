﻿using System;
using OpenTK.Graphics.OpenGL4;


namespace engenious.Graphics
{
    public enum ShaderType
    {
        FragmentShader = 35632,
        VertexShader,
        GeometryShader = 36313,
        TessEvaluationShader = 36487,
        TessControlShader,
        ComputeShader = 37305
    }

    internal class Shader : IDisposable
    {
        internal int BaseShader;

        public Shader(ShaderType type, string source)
        {
            ThreadingHelper.BlockOnUIThread(() =>
            {
                BaseShader = GL.CreateShader((OpenTK.Graphics.OpenGL4.ShaderType) type);
                GL.ShaderSource(BaseShader, source);
            });
        }

        internal void Compile()
        {
            ThreadingHelper.BlockOnUIThread(() =>
            {
                GL.CompileShader(BaseShader);

                int compiled;
                GL.GetShader(BaseShader, ShaderParameter.CompileStatus, out compiled);
                if (compiled != 1)
                {
                    string error = GL.GetShaderInfoLog(BaseShader);
                    throw new Exception(error);
                }
            });
        }

        public void Dispose()
        {
            GL.DeleteProgram(BaseShader);
        }
    }
}