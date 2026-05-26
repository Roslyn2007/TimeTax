namespace TimeTax.Model.Entities
{
    public class Coin : Entity
    {
        public CoinType Type { get; set; }
        public bool Collected { get; set; }

        public override float Width { get; set; } = 15f;
        public override float Height { get; set; } = 15f;
    }
}