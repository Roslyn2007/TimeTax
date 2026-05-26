namespace TimeTax.Model.Entities
{
    public class Enemy : Entity
    {
        public bool Active { get; set; } = true;

        public float PatrolStartX { get; set; }
        public float PatrolEndX { get; set; }
        public float PatrolSpeed { get; set; } = 60f;
        public bool MovingRight { get; set; } = true;

        public Vector2 SpawnPosition { get; set; }

        public override float Width { get; set; } = 24f;
        public override float Height { get; set; } = 24f;

        public virtual void Respawn()
        {
            Position = SpawnPosition;
            MovingRight = true;
        }

        public virtual void Update(float deltaTime)
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

            // Респавн при падении в бездну
            if (Position.Y > 500 || Position.Y < -100)
            {
                Respawn();
            }
        }
    }
}