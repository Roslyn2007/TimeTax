namespace TimeTax.Model.Entities
{
    public class Player : Entity
    {
        public Vector2 Velocity { get; set; }
        public bool IsGrounded { get; set; }

        public override float Width { get; set; } = 20f;
        public override float Height { get; set; } = 20f;

        public const float MoveSpeed = 150f;
        public const float JumpVelocity = -420f; 
        public const float Gravity = 600f;

        public void Update(float deltaTime, float gravity)
        {
            Velocity = new Vector2(Velocity.X, Velocity.Y + gravity * deltaTime);
            Position += Velocity * deltaTime;
            IsGrounded = false;
        }
    }
}