namespace TimeTax.Model.Entities
{
    public class ExitDoor : Entity
    {
        public bool IsOpen { get; set; }

        public override float Width { get; set; } = 24f;
        public override float Height { get; set; } = 32f;
    }
}