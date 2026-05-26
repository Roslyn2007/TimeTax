namespace TimeTax.Model.Entities
{
    public class Checkpoint : Entity
    {
        public bool Activated { get; set; }

        public override float Width { get; set; } = 24f;
        public override float Height { get; set; } = 32f;
    }
}