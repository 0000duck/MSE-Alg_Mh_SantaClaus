using System;
using System.Collections.Generic;
using System.Linq;
using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace SCVis
{
    class Program
    {
        public readonly int Id;
        public readonly IReadOnlyCollection<Shader> Shaders;

        /// <summary>
        /// Constructor
        /// Creates a new OpenGL program and attach shaders
        /// </summary>
        /// <param name="shaders">shaders to compile and attach</param>
        public Program(IEnumerable<Shader> shaders)
        {
            Id = GL.CreateProgram();
            var list = shaders.ToList();
            foreach (var shader in list)
            {
                GL.AttachShader(Id, shader.Id);
            }
            GL.LinkProgram(Id);
            GL.GetProgram(Id, GetProgramParameterName.LinkStatus, out var status);
            if (status != 1) throw new Exception(GL.GetProgramInfoLog(Id));
            Shaders = list;
        }

        /// <summary>
        /// Use this program's shaders to render
        /// </summary>
        public void Use() => GL.UseProgram(Id);

        public int GetAttributeLocation(string name)
        {
            var location = GL.GetAttribLocation(Id, name);
            if (location >= 0) return location;
            throw new Exception($"No attribute \"{name}\" found in program {Id}");
        }

        public int GetUniformLocation(string name)
        {
            var location = GL.GetUniformLocation(Id, name);
            if (location >= 0) return location;
            throw new Exception($"No uniform \"{name}\" found in program {Id}");
        }

        public void WriteInt(string name, int value) => GL.Uniform1(GetUniformLocation(name), value);
        public void WriteFloat(string name, float value) => GL.Uniform1(GetUniformLocation(name), value);
        public void WriteVec3(string name, Vector3 value) => GL.Uniform3(GetUniformLocation(name), value);
        public void WriteVec4(string name, Vector4 value) => GL.Uniform4(GetUniformLocation(name), value);
        public void WriteMat4(string name, Matrix4 value) => GL.UniformMatrix4(GetUniformLocation(name), false, ref value);
    }
}
