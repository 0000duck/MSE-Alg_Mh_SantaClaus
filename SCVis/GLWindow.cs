using OpenTK;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using OpenTK.Graphics;
using ButtonState = OpenTK.Input.ButtonState;
using SCLib;

namespace SCVis
{
    public class GLWindow
    {
        private readonly GameWindow _window;
        private Program _programPoints, _programPath;
        private Vector4 _colorPoints;
        private Vector4 _colorPath;
        private Mesh _mesh;
        private Matrix4 _transformation;
        private MouseState _state;

        public event EventHandler Loaded;

        public GLWindow(int width, int height, string title)
        {
            _state = Mouse.GetState();
            _window = new GameWindow(width, height, GraphicsMode.Default, title);
            _window.Load += OnLoad;
            _window.UpdateFrame += OnUpdate;
            _window.RenderFrame += OnRender;
            _window.Resize += (sender, args) => { GL.Viewport(_window.ClientRectangle); };
        }

        public void Run() => _window.Run(60);

        public void RunBackground(Action action)
        {
            Task.Run(() =>
            {
                var nw = new NativeWindow();
                var context = new GraphicsContext(GraphicsMode.Default, nw.WindowInfo);
                context.MakeCurrent(nw.WindowInfo);
                action.Invoke();
                context.Dispose();
                nw.Dispose();
            });
        }

        private void OnLoad(object sender, EventArgs args)
        {
            //Init application
            var vertexShader = new Shader("Res/VertexShader.glsl", ShaderType.VertexShader);
            var fragmentShader = new Shader("Res/FragmentShader.glsl", ShaderType.FragmentShader);
            var geometryShader = new Shader("Res/GeometryShader.glsl", ShaderType.GeometryShader);
            _programPoints = new Program(new[] {vertexShader, fragmentShader, geometryShader});
            _programPath = new Program(new[] {vertexShader, fragmentShader});
            vertexShader.Delete();
            fragmentShader.Delete();
            geometryShader.Delete();

            _mesh = new Mesh();
            GL.ClearColor(0.9f, 0.9f, 0.9f, 1f);
            _colorPoints = new Vector4(0f, 0f, 1f, 1f);
            _colorPath = new Vector4(1f, 0f, 0f, 1f);
            _transformation = Matrix4.Identity;

            var error = GL.GetError();
            if (error != ErrorCode.NoError)
                throw new Exception(error.ToString());

            Loaded?.Invoke(this, null);
        }

        public void SetVertices(float[] vertices, int n)
        {
            _mesh.SetVertices(vertices, n);
        }

        public void SetTour(List<uint[]> tours)
        {
            var indices = new List<uint>(20000);
            foreach (var tour in tours)
            {
                var p = tour[0];
                for (var i = 1; i < tour.Length; i++)
                {
                    var n = tour[i];
                    indices.Add(p);
                    indices.Add(n);
                    p = n;
                }
            }

            _mesh.SetPaths(indices.ToArray());
        }

        private void OnUpdate(object sender, FrameEventArgs args)
        {
            var current = Mouse.GetState();
            // zoom
            var diff = Mouse.GetState().WheelPrecise - _state.WheelPrecise;
            var scale = diff > 0 ? 1.1f : diff < 0 ? 0.9f : 1f;
            _transformation *= Matrix4.CreateScale(scale);
            // translation
            if (current.LeftButton == ButtonState.Pressed)
            {
                var x = (current.X - _state.X) / 1200f;
                var y = (current.Y - _state.Y) / -800f;
                _transformation *= Matrix4.CreateTranslation(x, y, 0);
            }

            //reset
            if (Keyboard.GetState().IsKeyDown(Key.R))
            {
                _transformation = Matrix4.Identity;
            }

            _state = current;
        }

        private void OnRender(object sender, FrameEventArgs args)
        {
            //clear screen and z-buffer
            GL.Clear(ClearBufferMask.ColorBufferBit);

            //render path
            if (_mesh.ActivePath)
            {
                _programPath.Use();
                _programPath.WriteMat4("T", _transformation);
                _programPath.WriteVec4("C", _colorPath);
                _mesh.RenderPath();
            }

            //render points
            if (_mesh.ActivePoints)
            {
                _programPoints.Use();
                _programPoints.WriteMat4("T", _transformation);
                _programPoints.WriteVec4("C", _colorPoints);
                _mesh.RenderPoints();
            }

            //display
            _window.SwapBuffers();
            var error = GL.GetError();
            if (error != ErrorCode.NoError)
                throw new Exception(error.ToString());
        }
    }
}