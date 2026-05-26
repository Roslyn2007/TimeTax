using System.Collections.Generic;
using System.Diagnostics;
using TimeTax.Model;

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

        protected Vector2 velocity = Vector2.Zero;
        protected bool isGrounded = false;
        protected const float Gravity = 600f;

        private List<Platform> platforms = new List<Platform>();
        private List<FadingPlatform> fadingPlatforms = new List<FadingPlatform>();

        public void SetPlatforms(List<Platform> plats, List<FadingPlatform> fading)
        {
            platforms = plats ?? new List<Platform>();
            fadingPlatforms = fading ?? new List<FadingPlatform>();
        }

        public virtual void Respawn()
        {
            Position = SpawnPosition;
            velocity = Vector2.Zero;
            MovingRight = true;
            isGrounded = false;
        }

        public virtual void Update(float deltaTime)
        {
            if (!Active) return;

            bool fellOff = Position.Y + Height >= 480f || Position.Y > 500f || Position.Y < -100 || Position.X < -50 || Position.X > 850;

            if (fellOff)
            {
                Debug.WriteLine($"[Enemy] Respawning at {Position.X:F1},{Position.Y:F1} -> {SpawnPosition.X:F1},{SpawnPosition.Y:F1}");
                Respawn();
                return;
            }

            velocity.Y += Gravity * deltaTime;
            isGrounded = false;

            float move = PatrolSpeed * deltaTime;
            if (MovingRight)
            {
                velocity.X = PatrolSpeed;
                if (Position.X >= PatrolEndX)
                {
                    Position = new Vector2(PatrolEndX, Position.Y);
                    MovingRight = false;
                }
            }
            else
            {
                velocity.X = -PatrolSpeed;
                if (Position.X <= PatrolStartX)
                {
                    Position = new Vector2(PatrolStartX, Position.Y);
                    MovingRight = true;
                }
            }

            Position += velocity * deltaTime;
            ResolveCollisions();
        }

        protected void ResolveCollisions()
        {
            bool grounded = false;
            Vector2 pos = Position;

            foreach (var platform in platforms)
            {
                if (Physics.ResolveFloorCollision(this, ref velocity, platform, out Vector2 newPos))
                {
                    pos = newPos;
                    grounded = true;
                }
            }

            foreach (var fp in fadingPlatforms)
            {
                if (!fp.IsVisible) continue;
                if (Physics.ResolveFloorCollision(this, ref velocity, fp, out Vector2 newPos))
                {
                    pos = newPos;
                    grounded = true;
                }
            }

            if (pos.Y + Height > 480)
            {
                pos.Y = 480 - Height;
                velocity.Y = 0;
                grounded = true;
            }

            Position = pos;
            isGrounded = grounded;
        }
    }
}