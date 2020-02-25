using System;
using OpenTK;
using OpenTK.Graphics.OpenGL4;

namespace WorldOfEgon.RenderEntities
{
    public class Egon : IRenderable, IDisposable
    {
        private int _vao;
        private int _vertexCount;
        private Shader _shader;
        private int _texture;
        private Matrix4 _modelMatrix = Matrix4.Identity;
        private Matrix4 _viewMatrix = Matrix4.Identity;
        private Matrix4 _projectionMatrix = Matrix4.Identity;
        private Matrix4 _camera;
        private readonly Vector3 _eye = new Vector3(0, 0, 2);
        private readonly Vector3 _target = Vector3.Zero;
        private readonly Vector3 _up = new Vector3(0, 1, 0);

        public void Init()
        {
            _shader = new Shader(@"Shader\egon.vert", @"Shader\egon.frag");
            new Mesh().LoadMesh(@"Resources\egon.obj", out _vao, out _vertexCount);
            _texture = Texture.InitTexture(@"Resources\egon.png");
        }

        public void Update(double timer)
        {
            _camera = Matrix4.LookAt(_eye, _target, _up);
            _projectionMatrix =
                Matrix4.CreatePerspectiveFieldOfView((float) (60.0 * Math.PI / 180.0), (4f / 3f), 0.1f, 100.0f);
            _viewMatrix = _camera;
            _modelMatrix = Matrix4.CreateScale(1.0f)
                           * Matrix4.CreateRotationX(0.0f)
                           * Matrix4.CreateRotationY(0.0f)
                           * Matrix4.CreateRotationZ(0.0f)
                           * Matrix4.CreateTranslation(new Vector3(0, 0, -10));
        }

        public void Render(Vector2 resolution)
        {
            _shader.Use();
            GL.BindTexture(TextureTarget.Texture2D, _texture);
            GL.UniformMatrix4(10, false, ref _modelMatrix);
            GL.UniformMatrix4(11, false, ref _viewMatrix);
            GL.UniformMatrix4(12, false, ref _projectionMatrix);
            GL.BindVertexArray(_vao);
            GL.DrawArrays(PrimitiveType.Triangles, 0, _vertexCount);
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