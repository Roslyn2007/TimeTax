namespace TimeTax.Model.Entities
{
    public class Conveyor : Entity
    {
        public ConveyorDirection Direction { get; set; } = ConveyorDirection.Right;
        public float Speed { get; set; } = 80f;

        public override float Height { get; set; } = 10f;
    }
}