using TimeTax.Model.Interfaces;

namespace TimeTax.Model.Entities
{
    public class Enemy : ICollidable
    {
        public Vector2 Position { get; set; }
        public bool Active { get; set; } = true;

        public float Width => 24f;
        public float Height => 24f;

        public (float left, float right, float top, float bottom) GetBounds()
        {
            float left = Position.X;
            float right = Position.X + Width;
            float top = Position.Y;
            float bottom = Position.Y + Height;
            return (left, right, top, bottom);
        }
    }
}