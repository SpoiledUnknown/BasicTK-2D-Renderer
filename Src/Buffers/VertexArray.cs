using System;
using OpenTK.Graphics.OpenGL;

namespace BasicTK_2D_Renderer.Src.Buffers
{
    public sealed class VertexArray : IDisposable
    {
        private bool isDisposed;

        public readonly int VertexArrayHandle;
        public readonly VertexBuffer VertexBuffer;

        public VertexArray(VertexBuffer vertexBuffer)
        {
            isDisposed = false;

            if (vertexBuffer is null)
            {
                throw new ArgumentNullException(nameof(vertexBuffer));
            }

            VertexBuffer = vertexBuffer;

            int vertexSizeInBytes = VertexBuffer.VertexInfo.SizeInBytes;
            VertextAttribute[] attributes = VertexBuffer.VertexInfo.VertextAttributes;

            VertexArrayHandle = GL.GenVertexArray();
            GL.BindVertexArray(VertexArrayHandle);

            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBuffer.VertexBufferHandle);


            for (int i = 0; i < attributes.Length; i++)
            {
                VertextAttribute attribute = attributes[i];
                GL.VertexAttribPointer(attribute.Index, attribute.ComponentCount, VertexAttribPointerType.Float, false, vertexSizeInBytes, attribute.Offset);
                GL.EnableVertexAttribArray(attribute.Index);
            }

            GL.BindVertexArray(0);
        }

        ~VertexArray()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (isDisposed)
            {
                return;
            }

            GL.BindVertexArray(0);
            GL.DeleteVertexArray(VertexArrayHandle);

            isDisposed = true;
            GC.SuppressFinalize(this);
        }
    }
}