using OpenTK;

namespace WorldOfEgon.RenderEntities
{
    public interface IRenderable
    {
        void Init();
        void Update(double timer);
        void Render(Vector2 resolution);
        void CleanUp();
    }
}