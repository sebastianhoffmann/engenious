using System;
using OpenTK.Graphics.OpenGL4;
using System.Runtime.InteropServices;

namespace engenious.Graphics
{
    public class VertexBuffer : GraphicsResource
    {
        internal int Vbo = -1;
        internal int TempVbo = -1;
        internal VertexAttributes Vao = null;

        private VertexBuffer(GraphicsDevice graphicsDevice, int vertexCount,
            BufferUsageHint usage = BufferUsageHint.StaticDraw)
            : base(graphicsDevice)
        {
            VertexCount = vertexCount;
            BufferUsage = usage;
        }

        public VertexBuffer(GraphicsDevice graphicsDevice, Type vertexType, int vertexCount,
            BufferUsageHint usage = BufferUsageHint.StaticDraw)
            : this(graphicsDevice, vertexCount, usage)
        {
            var tp = Activator.CreateInstance(vertexType) as IVertexType;
            if (tp == null)
                throw new ArgumentException("must be a vertexType");

            VertexDeclaration = tp.VertexDeclaration;
            ThreadingHelper.BlockOnUIThread(() =>
            {
                Vbo = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ArrayBuffer, Vbo);
                GL.BufferData(BufferTarget.ArrayBuffer, new IntPtr(vertexCount * VertexDeclaration.VertexStride),
                    IntPtr.Zero, (OpenTK.Graphics.OpenGL4.BufferUsageHint) BufferUsage);
            });
            ThreadingHelper.BlockOnUIThread(() =>
            {
                Vao = new VertexAttributes();
                Vao.Vbo = Vbo;
                Vao.Bind();
                GL.BindBuffer(BufferTarget.ArrayBuffer, Vbo);
                VertexAttributes.ApplyAttributes(Vao, VertexDeclaration);

                GL.BindVertexArray(0);
            }, true);
            GraphicsDevice.CheckError();
        }

        internal bool Bind()
        {
            if (Vao == null)
                return false;
            Vao.Bind();
            GraphicsDevice.CheckError();
            return true;
        }

        public VertexBuffer(GraphicsDevice graphicsDevice, VertexDeclaration vertexDeclaration, int vertexCount,
            BufferUsageHint usage = BufferUsageHint.StaticDraw)
            : this(graphicsDevice, vertexCount, usage)
        {
            VertexDeclaration = vertexDeclaration;
            ThreadingHelper.BlockOnUIThread(() =>
            {
                Vbo = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ArrayBuffer, Vbo);
                GL.BufferData(BufferTarget.ArrayBuffer, new IntPtr(vertexCount * VertexDeclaration.VertexStride),
                    IntPtr.Zero, (OpenTK.Graphics.OpenGL4.BufferUsageHint) BufferUsage);
            });
            ThreadingHelper.BlockOnUIThread(() =>
            {
                Vao = new VertexAttributes();
                Vao.Vbo = Vbo;
                Vao.Bind();
                GL.BindBuffer(BufferTarget.ArrayBuffer, Vbo);
                VertexAttributes.ApplyAttributes(Vao, VertexDeclaration);
                GL.BindVertexArray(0);
            }, true);
            GraphicsDevice.CheckError();
        }

        public void Resize(int vertexCount, bool keepData = false)
        {
            int tempVbo = 0;
            ThreadingHelper.BlockOnUIThread(() =>
            {
                GL.BindVertexArray(0);
                tempVbo = GL.GenBuffer();
                GL.BindBuffer(BufferTarget.ArrayBuffer, tempVbo);
                GL.BufferData(BufferTarget.ArrayBuffer, new IntPtr(vertexCount * VertexDeclaration.VertexStride),
                    IntPtr.Zero, (OpenTK.Graphics.OpenGL4.BufferUsageHint) BufferUsage);
                GraphicsDevice.CheckError();
            });

            ThreadingHelper.BlockOnUIThread(() =>
            {
                VertexCount = vertexCount;
                GL.DeleteBuffer(Vbo);
                Vbo = tempVbo;
                GraphicsDevice.CheckError();
            }); /*
            ThreadingHelper.BlockOnUIThread(() =>
                {

                }, true);*/
            if (keepData)
            {
                //TODO:
                throw new NotImplementedException();
            }
        }

        internal void EnsureVao()
        {
            if (Vao != null && Vao.Vbo != Vbo)
            {
                Vao.Vbo = Vbo;
                Vao.Bind();
                GL.BindBuffer(BufferTarget.ArrayBuffer, Vbo);
                VertexAttributes.ApplyAttributes(Vao, VertexDeclaration);

                GL.BindVertexArray(0);
            }
        }

        public BufferUsageHint BufferUsage { get; private set; }

        public int VertexCount { get; private set; }

        public VertexDeclaration VertexDeclaration { get; private set; }

        public void SetData<T>(T[] data) where T : struct
        {
            ThreadingHelper.BlockOnUIThread(() =>
            {
                GL.BindBuffer(BufferTarget.ArrayBuffer, Vbo);

                GCHandle buffer = GCHandle.Alloc(data, GCHandleType.Pinned);
                GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero,
                        new IntPtr(data.Length * VertexDeclaration.VertexStride), buffer.AddrOfPinnedObject());
                    //TODO use bufferusage
                buffer.Free();
            });
            GraphicsDevice.CheckError();
        }

        public void SetData<T>(T[] data, int startIndex, int elementCount) where T : struct
        {
            ThreadingHelper.BlockOnUIThread(() =>
            {
                GL.BindBuffer(BufferTarget.ArrayBuffer, Vbo);

                GCHandle buffer = GCHandle.Alloc(data, GCHandleType.Pinned);
                GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero,
                    new IntPtr(elementCount * VertexDeclaration.VertexStride),
                    buffer.AddrOfPinnedObject() + startIndex * VertexDeclaration.VertexStride); //TODO use bufferusage

                buffer.Free();
            });
            GraphicsDevice.CheckError();
        }

        public void SetData<T>(int offsetInBytes, T[] data, int startIndex, int elementCount, int vertexStride)
            where T : struct
        {
            ThreadingHelper.BlockOnUIThread(() =>
            {
                //vao.Bind();//TODO: verify
                GL.BindBuffer(BufferTarget.ArrayBuffer, Vbo);

                GCHandle buffer = GCHandle.Alloc(data, GCHandleType.Pinned);
                GL.BufferSubData(BufferTarget.ArrayBuffer, new IntPtr(offsetInBytes),
                    new IntPtr(elementCount * vertexStride), buffer.AddrOfPinnedObject() + startIndex * vertexStride);

                buffer.Free();
            });
            GraphicsDevice.CheckError();
        }

        public override void Dispose()
        {
            ThreadingHelper.BlockOnUIThread(() => { GL.DeleteBuffer(Vbo); });
            Vao.Dispose();
            base.Dispose();
        }
    }
}