namespace TimeTax.Model.Entities
{
    public class Portal : Entity
    {
        public Vector2 TargetPosition { get; set; }
        public int PartnerIndex { get; set; } = -1;
        public bool Active { get; set; } = true;

        public override float Width { get; set; } = 30f;
        public override float Height { get; set; } = 40f;
    }
}