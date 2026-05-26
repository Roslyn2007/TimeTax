using System;
using TimeTax.Model.Entities;

namespace TimeTax.Model
{
    public static class Physics
    {
        public static (Vector2 newPos, Vector2 newVel, bool grounded)? ResolvePlatformCollision(
            ICollidable entity, Vector2 velocity, ICollidable platform)
        {
            var e = entity.GetBounds();
            var p = platform.GetBounds();

            if (!(e.right > p.left && e.left < p.right && e.bottom > p.top && e.top < p.bottom))
                return null;

            float overlapTop    = e.bottom - p.top;
            float overlapBottom = p.bottom - e.top;
            float overlapLeft   = e.right - p.left;
            float overlapRight  = p.right - e.left;
            float minOverlap    = Math.Min(Math.Min(overlapTop, overlapBottom), Math.Min(overlapLeft, overlapRight));

            Vector2 pos = entity.Position;
            Vector2 vel = velocity;
            bool grounded = false;
            const float epsilon = 0.001f;

            if (Math.Abs(minOverlap - overlapTop) < epsilon && velocity.Y >= -1f)
            {
                pos = new Vector2(pos.X, p.top - entity.Height);
                vel = new Vector2(vel.X, 0);
                grounded = true;
            }
            else if (Math.Abs(minOverlap - overlapBottom) < epsilon && velocity.Y <= 1f)
            {
                pos = new Vector2(pos.X, p.bottom);
                vel = new Vector2(vel.X, 0);
            }
            else if (Math.Abs(minOverlap - overlapLeft) < epsilon && velocity.X >= -1f)
            {
                pos = new Vector2(p.left - entity.Width, pos.Y);
                vel = new Vector2(0, vel.Y);
            }
            else if (Math.Abs(minOverlap - overlapRight) < epsilon && velocity.X <= 1f)
            {
                pos = new Vector2(p.right, pos.Y);
                vel = new Vector2(0, vel.Y);
            }
            else if (velocity.Y >= 0 && overlapTop < entity.Height * 0.5f)
            {
                pos = new Vector2(pos.X, p.top - entity.Height);
                vel = new Vector2(vel.X, 0);
                grounded = true;
            }
            else if (velocity.Y <= 0 && overlapBottom < entity.Height * 0.5f)
            {
                pos = new Vector2(pos.X, p.bottom);
                vel = new Vector2(vel.X, 0);
            }
            else if (velocity.X >= 0 && overlapLeft < entity.Width * 0.5f)
            {
                pos = new Vector2(p.left - entity.Width, pos.Y);
                vel = new Vector2(0, vel.Y);
            }
            else if (velocity.X <= 0 && overlapRight < entity.Width * 0.5f)
            {
                pos = new Vector2(p.right, pos.Y);
                vel = new Vector2(0, vel.Y);
            }

            return (pos, vel, grounded);
        }

        public static bool ResolveFloorCollision(
            ICollidable entity, ref Vector2 velocity, ICollidable platform, out Vector2 newPosition)
        {
            newPosition = entity.Position;
            var e = entity.GetBounds();
            var p = platform.GetBounds();

            if (velocity.Y >= -2f &&
                e.bottom >= p.top - 2f &&
                e.bottom <= p.top + 12f &&
                e.right > p.left + 2f &&
                e.left < p.right - 2f &&
                e.top < p.top)
            {
                newPosition = new Vector2(entity.Position.X, p.top - entity.Height);
                velocity    = new Vector2(velocity.X, 0);
                return true;
            }
            return false;
        }

        public static void ClampToWorldBounds(ICollidable entity, float worldWidth, float worldHeight,
            ref Vector2 position, ref Vector2 velocity)
        {
            if (position.Y + entity.Height > worldHeight)
            {
                position = new Vector2(position.X, worldHeight - entity.Height);
                velocity = new Vector2(velocity.X, Math.Min(velocity.Y, 0));
            }
            if (position.X < 0)
            {
                position = new Vector2(0, position.Y);
                velocity = new Vector2(Math.Max(velocity.X, 0), velocity.Y);
            }
            if (position.X + entity.Width > worldWidth)
            {
                position = new Vector2(worldWidth - entity.Width, position.Y);
                velocity = new Vector2(Math.Min(velocity.X, 0), velocity.Y);
            }
        }
    }
}