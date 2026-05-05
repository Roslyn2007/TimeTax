using TimeTax.Model.Interfaces;

namespace TimeTax.Model.Entities
{
    public class Checkpoint : ICollidable
    {
        public Vector2 Position { get; set; }
        public bool Activated { get; set; }

        public float Width => 24f;
        public float Height => 32f;

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