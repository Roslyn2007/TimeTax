using TimeTax.Model.Interfaces;

namespace TimeTax.Model.Entities
{
    public class Enemy : ICollidable
    {
        public Vector2 Position { get; set; }
        public bool Active { get; set; } = true;

        public float PatrolStartX { get; set; }
        public float PatrolEndX { get; set; }
        public float PatrolSpeed { get; set; } = 60f;
        public bool MovingRight { get; set; } = true;

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

        public void Update(float deltaTime)
        {
            if (!Active) return;

            float move = PatrolSpeed * deltaTime;
            if (MovingRight)
            {
                Position = new Vector2(Position.X + move, Position.Y);
                if (Position.X >= PatrolEndX)
                {
                    Position = new Vector2(PatrolEndX, Position.Y);
                    MovingRight = false;
                }
            }
            else
            {
                Position = new Vector2(Position.X - move, Position.Y);
                if (Position.X <= PatrolStartX)
                {
                    Position = new Vector2(PatrolStartX, Position.Y);
                    MovingRight = true;
                }
            }
        }
    }
}