namespace ef_interactions.nplusone
{
    public class Order
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public decimal Total { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation property
        public virtual Customer Customer { get; set; } = null!;
    }
}
