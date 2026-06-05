using System.ComponentModel.DataAnnotations;

namespace ef_interactions.nplusone
{
    public class Customer
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Address1 { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string ZipCode { get; set; } = string.Empty;
        public decimal Balance { get; set; } = decimal.Zero;

        // Navigation property
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

        [Timestamp]                          
        public byte[] RowVersion { get; set; } = [];
    }
}