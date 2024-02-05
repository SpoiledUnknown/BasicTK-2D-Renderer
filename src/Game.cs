using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL;

namespace OpenTKProject
{
    public class Game : GameWindow
    {
        private VertexBuffer vertexBuffer;
        private IndexBuffer indexBuffer;
        private VertexArray vertexArray;
        private ShaderProgram shaderProgram;

        private int vertexCount;
        private int indexCount;

        private float colorFactor = 1f;
        private float deltaColorFactor = 1f / 240f;

        public Game(string title, int height, int width, VSyncMode haveVsync, WindowState isFullscreen)
            : base(
                  GameWindowSettings.Default,
                  new NativeWindowSettings()
                  {
                      Title = title,
                      Size = new Vector2i(width, height),
                      Vsync = haveVsync,
                      WindowState = isFullscreen,
                      WindowBorder = WindowBorder.Fixed,
                      StartVisible = false,
                      StartFocused = true,
                      API = ContextAPI.OpenGL,
                      Profile = ContextProfile.Core
                  })
        {
            CenterWindow();
        }

        public static NativeWindowSettings GetNativeWindowSettings()
        {
            NativeWindowSettings settings = NativeWindowSettings.Default;
            settings.Flags = ContextFlags.ForwardCompatible;
            return settings;
        }

        protected override void OnLoad()
        {
            IsVisible = true;

            GL.ClearColor(Color4.DimGray);

            // Drawing Boxes
            Random rand = new Random();

            int windowWidth = ClientSize.X;
            int windowHeight = ClientSize.Y;

            int boxCount = 20000;

            VertexPositionColor[] vertices = new VertexPositionColor[boxCount * 4];
            vertexCount = 0;

            for (int i = 0; i < boxCount; i++)
            {
                int w  = rand.Next(32, 128);
                int h  = rand.Next(32, 128);
                int x  = rand.Next(0, windowWidth - w);
                int y  = rand.Next(0, windowHeight - h);

                float r = (float)rand.NextDouble();
                float g = (float)rand.NextDouble();
                float b = (float)rand.NextDouble();

                vertices[vertexCount++] = new VertexPositionColor(new Vector2(x, y + h), new Color4(r, g, b, 1f));
                vertices[vertexCount++] = new VertexPositionColor(new Vector2(x + w, y + h), new Color4(r, g, b, 1f));
                vertices[vertexCount++] = new VertexPositionColor(new Vector2(x + w, y), new Color4(r, g, b, 1f));
                vertices[vertexCount++] = new VertexPositionColor(new Vector2(x, y), new Color4(r, g, b, 1f));
            }

            int[] indices =  new int[boxCount * 6];
            indexCount = 0;
            vertexCount = 0;

            for (int i = 0; i < boxCount; i++)
            {
                indices[indexCount++] = 0 + vertexCount;
                indices[indexCount++] = 1 + vertexCount;
                indices[indexCount++] = 2 + vertexCount;
                indices[indexCount++] = 0 + vertexCount;
                indices[indexCount++] = 2 + vertexCount;
                indices[indexCount++] = 3 + vertexCount;

                vertexCount += 4;
            }

            vertexBuffer =  new VertexBuffer(VertexPositionColor.VertextInfo, vertices.Length, true);
            vertexBuffer.SetData(vertices, vertices.Length);

            indexBuffer =  new IndexBuffer(indices.Length, true);
            indexBuffer.SetData(indices, indices.Length);

            vertexArray =  new VertexArray(vertexBuffer);

            string vertextShaderCode =
                @"
                #version 330 core

                uniform vec2 ViewportSize;
                uniform float ColorFactor; 

                layout (location = 0) in vec2 aPosition;
                layout (location = 1) in vec4 aColor;

                out vec4 vColor;

                void main()
                {
                    float nx = aPosition.x / ViewportSize.x * 2.0 - 1.0;
                    float ny = aPosition.y / ViewportSize.y * 2.0 - 1.0;
                    gl_Position = vec4(nx, ny, 0.0, 1.0);

                    vColor = aColor * ColorFactor;
                }
                ";

            string pixelShaderCode =
                @"
                #version 330
                    
                    in vec4 vColor;
                    out vec4 outputColor;

                    void main()
                    {
                        outputColor = vColor;
                    }
                ";

            shaderProgram = new ShaderProgram(vertextShaderCode, pixelShaderCode);

            int[] viewport = new int[4];
            GL.GetInteger(GetPName.Viewport, viewport);

            shaderProgram.SetUniform("ViewportSize", (float)viewport[2], (float)viewport[3]);
            shaderProgram.SetUniform("ColorFactor", colorFactor);

            base.OnLoad();
        }

        protected override void OnUnload()
        {
            vertexArray?.Dispose();
            indexBuffer?.Dispose();
            vertexBuffer?.Dispose();



            base.OnUnload();
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            GL.Viewport(0, 0, e.Width, e.Height);
            base.OnResize(e);
        }

        protected override void OnUpdateFrame(FrameEventArgs args)
        {
            colorFactor += deltaColorFactor;

            if (colorFactor >= 1f)
            {
                colorFactor = 1f;
                deltaColorFactor *= -1f;
            }

            if (colorFactor <= 0f)
            {
                colorFactor = 0f;
                deltaColorFactor *= -1f;
            }

            shaderProgram.SetUniform("ColorFactor", colorFactor);

            base.OnUpdateFrame(args);
        }

        protected override void OnRenderFrame(FrameEventArgs args)
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);

            GL.UseProgram(shaderProgram.ShaderProgramHandle);
            GL.BindVertexArray(vertexArray.VertexArrayHandle);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, indexBuffer.IndexBufferHandle);
            GL.DrawElements(PrimitiveType.Triangles, indexCount, DrawElementsType.UnsignedInt, 0);

            Context.SwapBuffers();
            base.OnRenderFrame(args);
        }
    }
}
