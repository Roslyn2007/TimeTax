using TimeTax.Model;

namespace TimeTax.Model.Interfaces
{
    public interface ICollidable
    {
        Vector2 Position { get; }
        float Width { get; }
        float Height { get; }
        (float left, float right, float top, float bottom) GetBounds();
    }
}