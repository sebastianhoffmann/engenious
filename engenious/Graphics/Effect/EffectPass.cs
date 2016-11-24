﻿using System;
using OpenTK.Graphics.OpenGL4;
using System.Collections.Generic;

namespace engenious.Graphics
{
    public sealed class EffectPass : IDisposable
    {
        internal int Program;

        internal EffectPass(string name) //TODO: content loading
        {
            Name = name;
            ThreadingHelper.BlockOnUIThread(() => { Program = GL.CreateProgram(); });
        }

        internal void BindAttribute(VertexElementUsage usage, string name)
        {
            ThreadingHelper.BlockOnUIThread(() => { GL.BindAttribLocation(Program, (int) usage, name); });
        }

        internal void CacheParameters()
        {
            int total = -1;
            ThreadingHelper.BlockOnUIThread(() =>
            {
                GL.GetProgram(Program, GetProgramParameterName.ActiveUniforms, out total);
                for (int i = 0; i < total; ++i)
                {
                    int size;
                    ActiveUniformType type;
                    string name = GL.GetActiveUniform(Program, i, out size, out type);
                    int location = GetUniformLocation(name);
                    Parameters.Add(new EffectPassParameter(this, name, location));
                }
                GL.GetProgram(Program, GetProgramParameterName.ActiveUniformBlocks, out total);
                for (int i = 0; i < total; ++i)
                {
                    int size;
                    System.Text.StringBuilder sb = new System.Text.StringBuilder(512);
                    GL.GetActiveUniformBlockName(Program, i, 512, out size, sb);
                    string name = sb.ToString();
                    int location = i; //TODO: is index really the correct location?
                    location = GL.GetUniformBlockIndex(Program, name);
                    Parameters.Add(new EffectPassParameter(this, name, location));
                }
                //TODO: ssbos?
            });
        }

        internal int GetUniformLocation(string name)
        {
            return GL.GetUniformLocation(Program, name);
        }

        internal List<Shader> Attached = new List<Shader>();

        internal void AttachShaders(IEnumerable<Shader> shaders)
        {
            ThreadingHelper.BlockOnUIThread(() =>
            {
                foreach (var shader in shaders)
                {
                    AttachShader(shader);
                }
            });
        }

        internal void AttachShader(Shader shader)
        {
            ThreadingHelper.BlockOnUIThread(() => { GL.AttachShader(Program, shader.BaseShader); });
        }

        internal void Link()
        {
            if (Attached == null)
                throw new Exception("Already linked");
            ThreadingHelper.BlockOnUIThread(() =>
            {
                GL.LinkProgram(Program);
                int linked;
                GL.GetProgram(Program, GetProgramParameterName.LinkStatus, out linked);
                if (linked != 1)
                {
                    string error = GL.GetProgramInfoLog(Program);
                    if (string.IsNullOrEmpty(error))
                        throw new Exception("Unknown error occured");
                    throw new Exception(error);
                }
                foreach (Shader shader in Attached)
                {
                    GL.DetachShader(Program, shader.BaseShader);
                }
            });
            Attached.Clear();
            Attached = null;
            ThreadingHelper.BlockOnUIThread(() => { Parameters = new EffectPassParameterCollection(this); });
        }

        internal EffectPassParameterCollection Parameters { get; private set; }

        public string Name { get; private set; }

        public void Apply()
        {
            ThreadingHelper.BlockOnUIThread(() => { GL.UseProgram(Program); });
        }

        public void Dispose()
        {
            ThreadingHelper.BlockOnUIThread(() => { GL.DeleteProgram(Program); });
        }

        public BlendState BlendState { get; internal set; }
        //TODO: apply states

        public DepthStencilState DepthStencilState { get; internal set; }

        public RasterizerState RasterizerState { get; internal set; }
    }
}