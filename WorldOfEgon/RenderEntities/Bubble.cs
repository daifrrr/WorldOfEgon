using System;
using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace WorldOfEgon.RenderEntities
{
    public class Bubble : IRenderable, IDisposable
    {
        private static readonly float[] Points =
        {
            -0.25f, 0.25f, 0,
            -0.25f, -0.25f, 0,
            0.25f, -0.25f, 0,
            0.25f, 0.25f, 0,
        };

        private static readonly float[] TexCoords =
        {
            1f, 1f,
            0f, 0f,
            1f, 0f,
            0f, 0f,
            1f, 1f,
            0f, 1f,
        };

        private static readonly int[] Elements =
        {
            0, 1, 2,
            2, 3, 0
        };
        
        private readonly int _vao;
        private readonly int _texture;
        private readonly Shader _shader;
        private Matrix4 _bubbleModel;
        private float _timer;

        private Vector3 Position { get; set; }
        private Vector3 Rotation { get; set; }
        private float Scale { get; set; }

        public Bubble(Vector3? position, Vector3? rotation, float? scale)
        {
            _shader = new Shader
            (
                @"Shader\bubble.vert", @"Shader\bubble.frag"
            );
            _vao = GL.GenVertexArray();
            Position = position ?? Vector3.Zero;
            Rotation = rotation ?? Vector3.One;
            Scale = scale ?? 1.0f;
            _texture = Texture.InitTexture(@"Resources\bubble.png");
            _bubbleModel = Matrix4.CreateScale(Scale)
                           * Matrix4.CreateRotationX(Rotation.X)
                           * Matrix4.CreateRotationX(Rotation.Y)
                           * Matrix4.CreateRotationX(Rotation.Z)
                           * Matrix4.CreateTranslation(Position);
        }

        public void Init()
        {
            GL.BindVertexArray(0);
            var pointsVbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, pointsVbo);
            GL.BufferData
            (
                BufferTarget.ArrayBuffer,
                Points.Length * sizeof(float),
                Points,
                BufferUsageHint.StaticDraw
            );

            var texcoordsVbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, texcoordsVbo);
            GL.BufferData
            (
                BufferTarget.ArrayBuffer,
                TexCoords.Length * sizeof(float),
                TexCoords,
                BufferUsageHint.StaticDraw
            );

            var elementsVbo = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, elementsVbo);
            GL.BufferData
            (
                BufferTarget.ElementArrayBuffer,
                Elements.Length * sizeof(int),
                Elements,
                BufferUsageHint.StaticDraw
            );

            GL.BindVertexArray(_vao);
            // GL.BindBuffer(BufferTarget.ArrayBuffer, pointsVbo);
            GL.BindVertexBuffer(0, pointsVbo, IntPtr.Zero, 3 * sizeof(float));
            GL.EnableVertexAttribArray(_shader.Attribute("aPositions"));
            // GL.VertexAttribPointer(_shader.Attribute("aPositions"), 3, VertexAttribPointerType.Float, false, 0, 0);
            GL.VertexAttribFormat(_shader.Attribute("aPositions"), 3, VertexAttribType.Float, false, 0);
            GL.VertexAttribBinding(0, 0);

            // GL.BindBuffer(BufferTarget.ArrayBuffer, texcoordsVbo);
            GL.BindVertexBuffer(0, texcoordsVbo, IntPtr.Zero, 3 * sizeof(float));
            GL.EnableVertexAttribArray(_shader.Attribute("aTexCoords"));
            // GL.VertexAttribPointer(_shader.Attribute("aTexCoords"), 3, VertexAttribPointerType.Float, false, 0, 0);
            GL.VertexAttribFormat(_shader.Attribute("aTexCoords"), 3, VertexAttribType.Float, false, sizeof(float) * 3);
            GL.VertexAttribBinding(2, 0);
        }
        
        public void Update(double timer)
        {
            _bubbleModel = Matrix4.CreateScale(Scale)
                           * Matrix4.CreateRotationX(Rotation.X)
                           * Matrix4.CreateRotationX(Rotation.Y)
                           * Matrix4.CreateRotationX(Rotation.Z)
                           * Matrix4.CreateTranslation(Position);
            _timer += (float) timer;
        }

        public void Render(Vector2 resolution)
        {
            _shader.Use();
            GL.Enable(EnableCap.Blend);
            GL.UniformMatrix4(_shader.Uniform("uModel"), false, ref _bubbleModel);
            GL.Uniform2(_shader.Uniform("aspectRatio"), resolution);
            GL.Uniform1(_shader.Uniform("uTime"), _timer);
            GL.BindTexture(TextureTarget.Texture2D, _texture);
            GL.BindVertexArray(_vao);
            GL.DrawElements(PrimitiveType.Triangles, Elements.Length, DrawElementsType.UnsignedInt, Elements);
            GL.Disable(EnableCap.Blend);
        }

        public void CleanUp()
        {
            Dispose();
        }
        public void Dispose()
        {
            GL.BindVertexArray(0);
            _shader.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}