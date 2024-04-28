using System;
using OpenTK.Graphics.OpenGL;

namespace BasicTK_2D_Renderer.Src.Buffers
{
    public sealed class IndexBuffer : IDisposable
    {
        public static readonly int MinIndexCount = 1;
        public static readonly int MaxIndexCount = 250_000;

        private bool isDisposed;

        public readonly int IndexBufferHandle;
        public readonly int IndexCount;
        public readonly bool IsStatic;

        public IndexBuffer(int indexCount, bool isStatic = true)
        {
            if (indexCount < MinIndexCount ||
            indexCount > MaxIndexCount)
            {
                throw new ArgumentOutOfRangeException(nameof(indexCount));
            }

            IndexCount = indexCount;
            IsStatic = isStatic;

            BufferUsageHint hint = BufferUsageHint.StaticDraw;
            if (!IsStatic)
            {
                hint = BufferUsageHint.StreamDraw;
            }

            IndexBufferHandle = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, IndexBufferHandle);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indexCount * sizeof(int), IntPtr.Zero, hint);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
        }

        ~IndexBuffer()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (isDisposed)
            {
                return;
            }

            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
            GL.DeleteBuffer(IndexBufferHandle);

            isDisposed = true;
            GC.SuppressFinalize(this);
        }

        public void SetData(int[] data, int count)
        {
            if (data is null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            if (data.Length <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(data));
            }

            if (count <= 0 ||
            count > IndexCount ||
            count > data.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, IndexBufferHandle);
            GL.BufferSubData(BufferTarget.ElementArrayBuffer, IntPtr.Zero, count * sizeof(int), data);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, 0);
        }
    }
}