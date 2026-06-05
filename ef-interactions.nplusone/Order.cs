using System.ComponentModel.DataAnnotations;

namespace ef_interactions.nplusone
{
    public class Order
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public decimal Total { get; set; }
        public DateTime CreatedAt { get; set; }

        public string Status { get; set; } = "Pending";
        public string? TransactionId { get; set; }

        // Navigation property
        public virtual Customer Customer { get; set; } = null!;
    }
}
