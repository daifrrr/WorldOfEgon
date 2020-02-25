using Assimp;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using PrimitiveType = OpenTK.Graphics.OpenGL4.PrimitiveType;
using Vector2 = OpenTK.Vector2;
using Vector3 = OpenTK.Vector3;

namespace WorldOfEgon
{
    internal class Game : GameWindow
    {
        private static DebugProc _openGlDebugDelegate;
        private int _egonVao;
        private int _egonVertexCount;
        private Shader _shader;
        private Shader _bubbleShader;
        private Matrix4 _camera;
        private Vector3 _eye = new Vector3(0, 0, 2);
        private readonly Vector3 _target = Vector3.Zero;
        private readonly Vector3 _up = new Vector3(0, 1, 0);
        private Matrix4 _modelMatrix = Matrix4.Identity;
        private Matrix4 _viewMatrix = Matrix4.Identity;
        private Matrix4 _projectionMatrix = Matrix4.Identity;
        private int _texture;
        private readonly Bubble[] _bubbles = new Bubble[5];

        public Game() : base(1280, 900,
            new GraphicsMode(new ColorFormat(8, 8, 8, 0),
                24, // Depth bits
                8, // Stencil bits
                4 // FSAA samples
            ),
            string.Empty,
            GameWindowFlags.Default,
            DisplayDevice.Default,
            4, 0,
            GraphicsContextFlags.ForwardCompatible)
        {
            Console.WriteLine($"Renderer: {GL.GetString(StringName.Renderer)}");
            Console.WriteLine($"Version: {GL.GetString(StringName.Version)}");
            _openGlDebugDelegate += OpenGlDebugCallback;
            
        }

        private readonly Random _random = new Random();
        private double _uTime;

        private float BubbleRandom(float minValue = -1.0f, float maxValue = 1.0f)
        {
            return Convert.ToSingle(_random.NextDouble() * (maxValue - minValue) + minValue);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            #region OpenGL

            _shader = new Shader(@"Shader\simple.vert", @"Shader\simple.frag");
            _bubbleShader = new Shader(@"Shader\bubble.vert", @"Shader\bubble.frag");
            _texture = Texture.InitTexture(@"Resources\egon.png");
            var mesh = new Mesh().LoadMesh(@"Resources\egon.obj", out _egonVao, out _egonVertexCount);
            for (var i = 0; i < _bubbles.Length; i++)
            {
                var position = new Vector3(BubbleRandom(), BubbleRandom(), BubbleRandom(0.5f, -0.5f));
                var rotation = Vector3.Zero;
                var scale = BubbleRandom(0.4f, 0.0f);
                _bubbles[i] = new Bubble(position, rotation, scale);
                _bubbles[i].Init();
            }

            SetGlobalSettings();

            #endregion
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);
            _camera = Matrix4.LookAt(_eye, _target, _up);
            _projectionMatrix =
                Matrix4.CreatePerspectiveFieldOfView((float) (60.0 * Math.PI / 180.0), (4f / 3f), 0.1f, 100.0f);
            _viewMatrix = _camera;
            _modelMatrix = Matrix4.CreateScale(1.0f)
                           * Matrix4.CreateRotationX(0.0f)
                           * Matrix4.CreateRotationY(0.0f)
                           * Matrix4.CreateRotationZ(0.0f)
                           * Matrix4.CreateTranslation(new Vector3(0, 0, -10));
            foreach (var bubble in _bubbles)
            {
                bubble.Update(e.Time);
            }
            if (Keyboard.GetState().IsKeyDown(Key.Escape))
                Exit();
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            _shader.Use();
            GL.BindTexture(TextureTarget.Texture2D, _texture);
            GL.UniformMatrix4(10, false, ref _modelMatrix);
            GL.UniformMatrix4(11, false, ref _viewMatrix);
            GL.UniformMatrix4(12, false, ref _projectionMatrix);
            GL.Uniform1(50, _uTime);
            GL.BindVertexArray(_egonVao);
            GL.DrawArrays(PrimitiveType.Triangles, 0, _egonVertexCount);
            foreach (var bubble in _bubbles)
            {
                bubble.Render(new Vector2(Width, Height));
            }
            SwapBuffers();
        }

        protected override void OnMouseMove(MouseMoveEventArgs e)
        {
            var speed = 0.1f;
            speed *= (e.XDelta > 0) ? -1f : 1f;
            if (e.Mouse.RightButton == ButtonState.Pressed)
            {
                var vVector = _eye - _target;
                _eye.Z = (float) (_target.Z + Math.Sin(speed) * vVector.X + Math.Cos(speed) * vVector.Z);
                _eye.X = (float) (_target.X + Math.Cos(speed) * vVector.X - Math.Sin(speed) * vVector.Z);
            }

            base.OnMouseMove(e);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            GL.Viewport(0, 0, Width, Height);
        }

        protected override void OnUnload(EventArgs e)
        {
            base.OnUnload(e);
            _shader.Dispose();
            _bubbleShader.Dispose();
            foreach (var bubble in _bubbles)
            {
                bubble.Dispose();
            }
        }

        private static void OpenGlDebugCallback(DebugSource source, DebugType type, int id, DebugSeverity severity,
            int length, IntPtr message, IntPtr userParam)
        {
            var temp = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("ERROR");
            Console.ForegroundColor = temp;
            Debug.WriteLine(source == DebugSource.DebugSourceApplication
                ? $"openGL - {Marshal.PtrToStringAnsi(message, length)}"
                : $"openGL - {Marshal.PtrToStringAnsi(message, length)}\n\tid:{id} severity:{severity} type:{type} source:{source}\n");
        }

        private static void SetGlobalSettings()
        {
            GL.Enable(EnableCap.DepthTest);
            GL.DepthFunc(DepthFunction.Less);
            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Back);
            GL.FrontFace(FrontFaceDirection.Ccw);
            GL.ClearColor(0.1f, 0.5f, 0.7f, 1.0f);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
        }
    }
}