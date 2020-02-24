using System;
using System.IO;
using OpenTK.Graphics.OpenGL4;

namespace OpenGLRefactorLater
{
    public sealed class Shader
    {
        private readonly int _shaderProgram;
        private bool _disposedValue = false;

        public int Attribute(string name)
        {
            return GL.GetAttribLocation(_shaderProgram, name);
        }

        public int Uniform(string name)
        {
            return GL.GetUniformLocation(_shaderProgram, name);
        }

        public Shader(string vertexShaderSource, string fragmentShaderSource)
        {
            _shaderProgram = GL.CreateProgram();
            LoadShader(vertexShaderSource, ShaderType.VertexShader, out var vertexShaderId);
            LoadShader(fragmentShaderSource, ShaderType.FragmentShader, out var fragmentShaderId);
            var info = GL.GetProgramInfoLog(_shaderProgram);
            if(!string.IsNullOrWhiteSpace(info))
                Console.Error.WriteLine($"Error with shader program:_ {info}");
            GL.BindAttribLocation(_shaderProgram, 0, "aPositions");
            GL.BindAttribLocation(_shaderProgram, 2, "aTexCoords");

            GL.LinkProgram(_shaderProgram);
            
            GL.DetachShader(_shaderProgram, vertexShaderId);
            GL.DetachShader(_shaderProgram, fragmentShaderId);
            GL.DeleteShader(fragmentShaderId);
            GL.DeleteShader(vertexShaderId);
        }

        private void LoadShader(string source, ShaderType s, out int shaderId)
        {
            var sr = File.ReadAllText(source);
            shaderId = GL.CreateShader(s);
            GL.ShaderSource(shaderId, sr);
            GL.CompileShader(shaderId);
            GL.AttachShader(_shaderProgram, shaderId);
            var info = GL.GetShaderInfoLog(shaderId);
            if(!string.IsNullOrWhiteSpace(info))
                Console.Error.WriteLine($"Error with shader [{s}]: {info}");
        }
        
        public void Use()
        {
            GL.UseProgram(_shaderProgram);
        }

        private void Dispose(bool disposing)
        {
            if (_disposedValue) return;
            GL.DeleteProgram(_shaderProgram);

            _disposedValue = true;
        }
        
        ~Shader()
        {
            GL.DeleteProgram(_shaderProgram);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}