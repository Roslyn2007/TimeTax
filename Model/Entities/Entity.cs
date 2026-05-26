using TimeTax.Model;

namespace TimeTax.Model.Entities
{
    public abstract class Entity : ICollidable
    {
        public Vector2 Position { get; set; }
        public virtual float Width { get; set; }
        public virtual float Height { get; set; }

        public virtual (float left, float right, float top, float bottom) GetBounds()
        {
            float left = Position.X;
            float right = Position.X + Width;
            float top = Position.Y;
            float bottom = Position.Y + Height;
            return (left, right, top, bottom);
        }
    }
}