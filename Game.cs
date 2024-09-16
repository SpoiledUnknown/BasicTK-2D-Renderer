using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL;
using BasicTK_2D_Renderer.Src.Buffers;
using BasicTK_2D_Renderer.Src;

namespace BasicTK_2D_Renderer
{
    public class Game : GameWindow
    {
        #nullable disable
        private VertexBuffer vertexBuffer;
        private IndexBuffer indexBuffer;
        private VertexArray vertexArray;
        private ShaderProgram shaderProgram;

        private int vertexCount;
        private int indexCount;
        private List<Box> boxCount;

        private float colorFactor = 1f;
        private float deltaColorFactor = 1f / 480f;

        public Game(string title, int height, int width, List<Box> boxCount, VSyncMode haveVsync, WindowState isFullscreen, WindowBorder windowBorder)
            : base(
                  GameWindowSettings.Default,
                  new NativeWindowSettings()
                  {
                      Title = title,
                      Vsync = haveVsync,
                      WindowState = isFullscreen,
                      WindowBorder = windowBorder,
                      StartVisible = false,
                      StartFocused = true,
                      API = ContextAPI.OpenGL,
                      MinimumSize = new Vector2i(width, height),
                      Profile = ContextProfile.Core
                  })
        {
            CenterWindow();
            this.boxCount = boxCount;
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

            GL.ClearColor(Color4.LightSkyBlue);


            VertexPositionColor[] vertices = new VertexPositionColor[boxCount.Count * 4];
            vertexCount = 0;

            foreach (Box box in boxCount)
            {
                vertices[vertexCount++] = new VertexPositionColor(new Vector2(box.x, box.y + box.height), box.color);
                vertices[vertexCount++] = new VertexPositionColor(new Vector2(box.x + box.width, box.y + box.height), box.color);
                vertices[vertexCount++] = new VertexPositionColor(new Vector2(box.x + box.width, box.y), box.color);
                vertices[vertexCount++] = new VertexPositionColor(new Vector2(box.x, box.y), box.color);
            }

            int[] indices = new int[boxCount.Count * 6];
            indexCount = 0;
            vertexCount = 0;

            for (int i = 0; i < boxCount.Count; i++)
            {
                indices[indexCount++] = 0 + vertexCount;
                indices[indexCount++] = 1 + vertexCount;
                indices[indexCount++] = 2 + vertexCount;
                indices[indexCount++] = 0 + vertexCount;
                indices[indexCount++] = 2 + vertexCount;
                indices[indexCount++] = 3 + vertexCount;

                vertexCount += 4;
            }

            vertexBuffer = new VertexBuffer(VertexPositionColor.VertextInfo, vertices.Length, true);
            vertexBuffer.SetData(vertices, vertices.Length);

            indexBuffer = new IndexBuffer(indices.Length, true);
            indexBuffer.SetData(indices, indices.Length);

            vertexArray = new VertexArray(vertexBuffer);

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

            shaderProgram.SetUniform("ViewportSize", viewport[2], viewport[3]);
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
            this.colorFactor += this.deltaColorFactor;

            if (this.colorFactor >= 1f)
            {
                this.colorFactor = 1f;
                this.deltaColorFactor *= -1f;
            }

            if (this.colorFactor <= 0f)
            {
                this.colorFactor = 0f;
                this.deltaColorFactor *= -1f;
            }

            this.shaderProgram.SetUniform("ColorFactor", this.colorFactor);

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
