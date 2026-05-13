using TimeTax.Model.Interfaces;

namespace TimeTax.Model.Entities
{
    public class Player : ICollidable
    {
        public Vector2 Position { get; set; }
        public Vector2 Velocity { get; set; }
        public bool IsGrounded { get; set; }

        public float Width => 20f;
        public float Height => 20f;

        public const float MoveSpeed = 150f;
        public const float JumpVelocity = -420f;  // Было -320, стало -420
        public const float Gravity = 600f;

        public (float left, float right, float top, float bottom) GetBounds()
        {
            float left = Position.X;
            float right = Position.X + Width;
            float top = Position.Y;
            float bottom = Position.Y + Height;
            return (left, right, top, bottom);
        }

        public void Update(float deltaTime, float gravity)
        {
            Velocity = new Vector2(Velocity.X, Velocity.Y + gravity * deltaTime);
            Position += Velocity * deltaTime;
            IsGrounded = false;
        }
    }
}