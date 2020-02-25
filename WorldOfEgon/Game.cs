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
using WorldOfEgon.Helper;
using WorldOfEgon.RenderEntities;
using PrimitiveType = OpenTK.Graphics.OpenGL4.PrimitiveType;
using Vector2 = OpenTK.Vector2;
using Vector3 = OpenTK.Vector3;

namespace WorldOfEgon
{
    internal class Game : GameWindow
    {
        private static DebugProc _openGlDebugDelegate;
        private Vector3 _eye = new Vector3(0, 0, 2);
        private readonly Vector3 _target = Vector3.Zero;
        private readonly List<IRenderable> _renderObjects = new List<IRenderable>();
        private Shader _shader;

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

        
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            #region OpenGL
            // var egon = new Egon();
            // _renderObjects.Add(egon);
            // var bubbles = new Bubble[5];
            // for (var i = 0; i < bubbles.Length; i++)
            // {
            //     var position = new Vector3(Utils.GenerateRandom(), Utils.GenerateRandom(), Utils.GenerateRandom(0.5f, -0.5f));
            //     var rotation = Vector3.Zero;
            //     var scale = Utils.GenerateRandom(0.4f, 0.0f);
            //     bubbles[i] = new Bubble(position, rotation, scale);
            //     _renderObjects.Add(bubbles[i]);
            // }
            // foreach (var renderObject in _renderObjects)
            // {
            //     renderObject.Init();
            // }
            
            _shader = new Shader
            (
                @"Shader\simple.vert", @"Shader\simple.frag"    
            );

            SetGlobalSettings();

            #endregion
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);
            foreach (var renderObject in _renderObjects)
            {
                renderObject.Update(e.Time);
            }
            if (Keyboard.GetState().IsKeyDown(Key.Escape))
                Exit();
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            _shader.Use();
            foreach (var renderObject in _renderObjects)
            {
                renderObject.Render(new Vector2(Width, Height));
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
            foreach (var renderObject in _renderObjects)
            {
                renderObject.CleanUp();
            }
            _shader.Dispose();
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