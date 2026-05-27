namespace ef_interactions.nplusone
{
    public class CustomerSummaryDto
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string City { get; set; }
        public int OrderCount { get; set; }
        public decimal TotalSpent { get; set; }
        public DateTime LatestOrderDate { get; set; }
    }
}