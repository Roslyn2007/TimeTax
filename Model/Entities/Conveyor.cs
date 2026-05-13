using TimeTax.Model.Interfaces;

namespace TimeTax.Model.Entities
{
    public enum ConveyorDirection { Left, Right }

    public class Conveyor : ICollidable
    {
        public Vector2 Position { get; set; }
        public float Width { get; set; }
        public float Height { get; set; } = 10f;
        public ConveyorDirection Direction { get; set; } = ConveyorDirection.Right;
        public float Speed { get; set; } = 80f;

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