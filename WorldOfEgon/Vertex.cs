using OpenTK;
using OpenTK.Graphics;

namespace WorldOfEgon
{
    public struct Vertex
    {
        public const int SizeInBytes = (4 + 4 + 4) * 4;
        private readonly Vector4 _position;
        private readonly Vector4 _texCoord;
        private readonly Vector4 _normal;

        public Vertex(Vector4 position, Vector4 texCoord, Vector4 normal)
        {
            _position = position;
            _texCoord = texCoord;
            _normal = normal;
        }
    }
}