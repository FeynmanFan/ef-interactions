namespace ef_interactions.nplusone
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int CategoryId { get; set; }
        public virtual ProductCategory Category { get; set; } = null!;
        public decimal Price { get; set; }
    }
}