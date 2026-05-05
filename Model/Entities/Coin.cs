using TimeTax.Model.Interfaces;

namespace TimeTax.Model.Entities
{
    public enum CoinType { Normal, Gold }

    public class Coin : ICollidable
    {
        public Vector2 Position { get; set; }
        public CoinType Type { get; set; }
        public bool Collected { get; set; }

        public float Width => 15f;
        public float Height => 15f;

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