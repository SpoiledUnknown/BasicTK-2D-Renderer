using System;
using OpenTK.Graphics.OpenGL;

namespace BasicTK_2D_Renderer.Src.Buffers
{
    public sealed class VertexBuffer : IDisposable
    {
        public static readonly int MinVertexCount = 1;
        public static readonly int MaxVertexCount = 100_000;

        private bool isDisposed;

        public readonly int VertexBufferHandle;
        public readonly VertextInfo VertexInfo;
        public readonly int VertexCount;
        public readonly bool IsStatic;

        public VertexBuffer(VertextInfo vertextInfo, int vertexCount, bool isStatic = true)
        {
            isDisposed = false;

            if (vertexCount < MinVertexCount ||
            vertexCount > MaxVertexCount)
            {
                throw new ArgumentOutOfRangeException(nameof(vertexCount));
            }

            VertexInfo = vertextInfo;
            VertexCount = vertexCount;
            IsStatic = isStatic;

            BufferUsageHint hint = BufferUsageHint.StaticDraw;
            if (!IsStatic)
            {
                hint = BufferUsageHint.StreamDraw;
            }

            int vertexSizeInBytes = VertexPositionColor.VertextInfo.SizeInBytes;

            VertexBufferHandle = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferHandle);
            GL.BufferData(BufferTarget.ArrayBuffer, vertexCount * vertextInfo.SizeInBytes, IntPtr.Zero, hint);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        ~VertexBuffer()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (isDisposed) return;

            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.DeleteBuffer(VertexBufferHandle);

            isDisposed = true;
            GC.SuppressFinalize(this);
        }

        public void SetData<T>(T[] data, int count) where T : struct
        {
            if (typeof(T) != VertexInfo.Type)
            {
                throw new ArgumentException("Generic type 'T' does not match the vertex type of vertex buffer");
            }

            if (data is null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            if (data.Length <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(data));
            }

            if (count <= 0 ||
            count > VertexCount ||
            count > data.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }

            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferHandle);
            GL.BufferSubData(BufferTarget.ArrayBuffer, IntPtr.Zero, count * VertexInfo.SizeInBytes, data);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }
    }
}