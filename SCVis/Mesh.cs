using OpenTK.Graphics.OpenGL4;
using System.Collections.Generic;

namespace SCVis
{
    class Mesh
    {
        public bool ActivePoints { get; private set; }
        public bool ActivePath { get; private set; }

        private readonly int _vao, _vbo;
        private int _length;

        public Mesh()
        {
            // create vertex array object
            GL.GenVertexArrays(1, out _vao);
            GL.BindVertexArray(_vao);
            // create vertex buffer object
            GL.GenBuffers(1, out _vbo);
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
            GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 0, 0);
            GL.EnableVertexAttribArray(0);
        }

        public void SetVertices(float[] vertices, int length)
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices,
                BufferUsageHint.StaticDraw);
            ActivePoints = true;
            ActivePath = false;
            _length = length;
        }

        public int[] GenPaths(int n)
        {
            // create element array buffer object
            int[] ebos = new int[n];
            GL.GenBuffers(n, ebos);
            return ebos;
        }

        public void SetPath(int ebo, uint[] indices)
        {
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
            GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices,
                BufferUsageHint.StaticDraw);
            ActivePath = true;
        }

        public void RenderPoints()
        {
            GL.BindVertexArray(_vao);
            GL.DrawArrays(PrimitiveType.Points, 0, _length);
        }

        public void RenderPath(int ebo)
        {
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, ebo);
            GL.DrawElements(PrimitiveType.LineLoop, _length, DrawElementsType.UnsignedInt, 0);
        }
    }
}