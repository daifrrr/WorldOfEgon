using System;
using System.Collections.Generic;
using System.Linq;
using OpenTK;
using OpenTK.Graphics.OpenGL4;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace WorldOfEgon
{
    public class Bubble : IDisposable
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

        private const string Path = @"Resources\bubble.png";

        private float _scale;
        private Vector3 _position;
        private Vector3 _rotation;
        private int _vao;
        private int _texture;
        private readonly Shader _shader;
        private Matrix4 _bubbleModel;
        private float _timer;

        public Vector3 Position
        {
            get => _position;
            set => _position = value;
        }

        public Bubble(Vector3? position, Vector3? rotation, float? scale)
        {
            _shader = new Shader
            (
                @"Shader\bubble.vert", @"Shader\bubble.frag"    
            );
            _vao = GL.GenVertexArray();
            Position = position ?? Vector3.Zero;
            _rotation = rotation ?? Vector3.One;
            _scale = scale ?? 1.0f;
            _texture = InitTexture();
            _bubbleModel = Matrix4.CreateScale(_scale)
                           * Matrix4.CreateRotationX(_rotation.X)
                           * Matrix4.CreateRotationX(_rotation.Y)
                           * Matrix4.CreateRotationX(_rotation.Z)
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

        private static int InitTexture()
        {
            int width, height;
            var data = LoadTexture(out width, out height);
            var texId = GL.GenTexture();
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, texId);
            GL.TexImage2D
            (
                TextureTarget.Texture2D,
                0,
                PixelInternalFormat.Rgba,
                width,
                height,
                0,
                PixelFormat.Rgba,
                PixelType.UnsignedByte,
                data
            );
            GL.TexParameter(
                TextureTarget.Texture2D,
                TextureParameterName.TextureWrapS,
                (int) TextureWrapMode.ClampToBorder
            );
            GL.TexParameter
            (
                TextureTarget.Texture2D,
                TextureParameterName.TextureWrapT,
                (int) TextureWrapMode.ClampToBorder
            );
            GL.TexParameter
            (
                TextureTarget.Texture2D,
                TextureParameterName.TextureMagFilter,
                (int) TextureMagFilter.Linear
            );
            GL.TexParameter
            (
                TextureTarget.Texture2D,
                TextureParameterName.TextureMinFilter,
                (int) TextureMinFilter.Linear
            );
            return texId;
        }

        public void Update(double timer)
        {
            _bubbleModel = Matrix4.CreateScale(_scale)
                           * Matrix4.CreateRotationX(_rotation.X)
                           * Matrix4.CreateRotationX(_rotation.Y)
                           * Matrix4.CreateRotationX(_rotation.Z)
                           * Matrix4.CreateTranslation(Position);
            _timer += (float)timer;
        }
        public void Render(Vector2 aspect)
        {
            _shader.Use();
            GL.Enable(EnableCap.Blend);
            GL.UniformMatrix4(_shader.Uniform("uModel"), false, ref _bubbleModel);
            GL.Uniform1(_shader.Uniform("aspectRatio"), aspect.X / aspect.Y);
            GL.Uniform1(_shader.Uniform("uTime"), _timer);
            GL.BindTexture(TextureTarget.Texture2D, _texture);
            GL.BindVertexArray(_vao);
            GL.DrawElements(PrimitiveType.Triangles, Elements.Length, DrawElementsType.UnsignedInt, Elements);
            GL.Disable(EnableCap.Blend);
        }

        private static byte[] LoadTexture(out int width, out int height)
        {
            var image = Image.Load(Path) as Image<Rgba32>;
            image.Mutate(x => x.Flip(FlipMode.Vertical));
            width = image.Width;
            height = image.Height;
            var tempPixels = image.GetPixelSpan().ToArray();

            var pixels = new List<byte>();
            foreach (var p in tempPixels)
            {
                pixels.Add(p.R);
                pixels.Add(p.G);
                pixels.Add(p.B);
                pixels.Add(p.A);
            }

            return pixels.ToArray();
        }
        
        public void Dispose()
        {
            GL.BindVertexArray(0);
            _shader.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}