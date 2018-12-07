using System;
using System.IO;
using OpenTK.Graphics.OpenGL4;

namespace SCVis
{
    class Shader
    {
        public readonly int Id;
        public readonly ShaderType Type;

        public bool IsDeleted { get; private set; }

        public Shader(string path, ShaderType type)
        {
            Type = type;
            Id = GL.CreateShader(type);
            var source = File.ReadAllText(path);
            GL.ShaderSource(Id, source);
            GL.CompileShader(Id);
            GL.GetShader(Id, ShaderParameter.CompileStatus, out var status);
            if (status != 1) throw new Exception(GL.GetShaderInfoLog(Id));
        }

        public void Delete()
        {
            IsDeleted = true;
            GL.DeleteShader(Id);
        }
    }
}