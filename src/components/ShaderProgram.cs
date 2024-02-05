using System;
using OpenTK.Graphics.OpenGL;

namespace OpenTKProject
{
    public readonly struct ShaderUniform
    {
        public readonly string Name;
        public readonly int Location;
        public readonly ActiveUniformType Type;

        public ShaderUniform(string name, int location, ActiveUniformType type)
        {
            Name = name;
            Location = location;
            Type = type;
        }
    }

    public readonly struct ShaderAttribute
    {
        public readonly string Name;
        public readonly int Location;
        public readonly ActiveAttribType Type;

        public ShaderAttribute(string name, int location, ActiveAttribType type)
        {
            Name = name;
            Location = location;
            Type = type;
        }
    }

    public sealed class ShaderProgram : IDisposable
    {
        private bool isDisposed;

        public readonly int ShaderProgramHandle;
        public readonly int VertexShaderHandle;
        public readonly int PixelShaderHandle;

        private readonly ShaderUniform[] uniforms;
        private readonly ShaderAttribute[] attributes;

        public ShaderProgram(string vertexShaderCode, string pixelShaderCode)
        {
            isDisposed = false;

            if (!ShaderProgram.CompileVertexShader(vertexShaderCode, out VertexShaderHandle, out string vertexShaderCompileError))
            {
                throw new ArgumentException(vertexShaderCompileError);
            }

            if (!ShaderProgram.CompilePixelShader(pixelShaderCode, out PixelShaderHandle, out string pixelShaderCompileError))
            {
                throw new ArgumentException(pixelShaderCompileError);
            }

            ShaderProgramHandle = ShaderProgram.CreateLinkProgram(VertexShaderHandle, PixelShaderHandle);

            uniforms = ShaderProgram.createUniformList(ShaderProgramHandle);
            attributes = ShaderProgram.createAttributeList(ShaderProgramHandle);
        }

        ~ShaderProgram()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (isDisposed)
            {
                return;
            }

            GL.DeleteShader(VertexShaderHandle);
            GL.DeleteShader(PixelShaderHandle);

            GL.UseProgram(0);
            GL.DeleteProgram(ShaderProgramHandle);

            isDisposed = true;
            GC.SuppressFinalize(this);
        }

        public ShaderUniform[] GetUniformList()
        {
            ShaderUniform[] result = new ShaderUniform[uniforms.Length];
            Array.Copy(uniforms, result, uniforms.Length);
            return result;
        }

        public ShaderAttribute[] GetAttributeList()
        {
            ShaderAttribute[] result = new ShaderAttribute[attributes.Length];
            Array.Copy(attributes, result, attributes.Length);
            return result;
        }

        public void SetUniform(string name, float v1)
        {
            if (!GetShaderUniform(name, out ShaderUniform uniform))
            {
                throw new ArgumentException("Name was not found.");
            }

            if (uniform.Type != ActiveUniformType.Float)
            {
                throw new ArgumentException("Uniform type is not float.");
            }

            
            GL.UseProgram(ShaderProgramHandle);
            GL.Uniform1(uniform.Location, v1);
            GL.UseProgram(0);
        }

        public void SetUniform(string name, float v1, float v2)
        {
            if (!GetShaderUniform(name, out ShaderUniform uniform))
            {
                throw new ArgumentException("Name was not found.");
            }

            if (uniform.Type != ActiveUniformType.FloatVec2)
            {
                throw new ArgumentException("Uniform type is not float.");
            }

            
            GL.UseProgram(ShaderProgramHandle);
            GL.Uniform2(uniform.Location, v1, v2);
            GL.UseProgram(0);
        }

        private bool GetShaderUniform(string name, out ShaderUniform uniform)
        {
            uniform = new ShaderUniform();

            for (int i = 0; i < uniforms.Length; i++)
            {
                uniform = uniforms[i];

                if (name == uniform.Name)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool CompileVertexShader(string vertexShaderCode, out int vertexShaderHandle, out string errorMessage)
        {
            errorMessage =  string.Empty;

            vertexShaderHandle = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShaderHandle, vertexShaderCode);
            GL.CompileShader(vertexShaderHandle);

            string vertexShaderInfo = GL.GetShaderInfoLog(vertexShaderHandle);
            if(vertexShaderInfo != string.Empty)
            {
                errorMessage = vertexShaderInfo;
                return false;
            }

            return true;
        }

        public static bool CompilePixelShader(string pixelShaderCode, out int pixelShaderhandle, out string errorMessage)
        {
            errorMessage = string.Empty;

            pixelShaderhandle = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(pixelShaderhandle, pixelShaderCode);
            GL.CompileShader(pixelShaderhandle);

            string pixelShaderInfo = GL.GetShaderInfoLog(pixelShaderhandle);
            if (pixelShaderInfo != string.Empty)
            {
                errorMessage = pixelShaderInfo;
                return false;
            }

            return true;
        }

        public static int CreateLinkProgram(int vertexShaderHandle, int pixelShaderHandle)
        {
            int shaderProgramHandle = GL.CreateProgram();

            GL.AttachShader(shaderProgramHandle, vertexShaderHandle);
            GL.AttachShader(shaderProgramHandle, pixelShaderHandle);

            GL.LinkProgram(shaderProgramHandle);

            GL.DetachShader(shaderProgramHandle, vertexShaderHandle);
            GL.DetachShader(shaderProgramHandle, pixelShaderHandle);

            return shaderProgramHandle;
        }

        public static ShaderUniform[] createUniformList(int shaderProgramHandle)
        {
            GL.GetProgram(shaderProgramHandle, GetProgramParameterName.ActiveUniforms, out int uniformCount);
            ShaderUniform[] uniforms = new ShaderUniform[uniformCount];

            for (int i = 0; i < uniformCount; i++)
            {
                GL.GetActiveUniform(shaderProgramHandle, i, 256, out _, out _, out ActiveUniformType type, out string name);
                int location = GL.GetUniformLocation(shaderProgramHandle, name);
                uniforms[i] = new ShaderUniform(name, location, type);
            }

            return uniforms;
        }

        public static ShaderAttribute[] createAttributeList(int shaderProgramHandle)
        {
            GL.GetProgram(shaderProgramHandle, GetProgramParameterName.ActiveAttributes, out int attributeCount);
            ShaderAttribute[] attributes = new ShaderAttribute[attributeCount];

            for (int i = 0; i < attributeCount; i++)
            {
                GL.GetActiveAttrib(shaderProgramHandle, i, 256, out _, out _, out ActiveAttribType type, out string name);
                int location = GL.GetAttribLocation(shaderProgramHandle, name);
                attributes[i] = new ShaderAttribute(name, location, type);
            }

            return attributes;
        }
    }
}