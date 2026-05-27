namespace ef_interactions.benchmarker
{
    using BenchmarkDotNet.Attributes;
    using BenchmarkDotNet.Order;
    using ef_interactions.nplusone;
    using Microsoft.EntityFrameworkCore;

    [MemoryDiagnoser]
    [RankColumn]
    [Orderer(SummaryOrderPolicy.FastestToSlowest)]
    public class EfLoadingBenchmarks
    {
        private AppDbContext _context = null!;

        [GlobalSetup]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlServer("Server=localhost,1433;Database=CompanyDb;User Id=sa;Password=YourStrongPassword123!;TrustServerCertificate=True;")
                .Options;

            _context = new AppDbContext(options);
        }

        [Benchmark(Description = "1. Lazy Loading (N+1)")]
        public void LazyLoading()
        {
            var customers = _context.Customers.ToList();

            foreach (var c in customers)
            {
                var orders = c.Orders.ToList();   // Triggers N+1
                _ = orders.Count;
            }
        }

        [Benchmark(Description = "2. Eager Loading (Single Query)")]
        public void EagerLoading_SingleQuery()
        {
            var customers = _context.Customers
                .Include(c => c.Orders)
                .ToList();
        }

        [Benchmark(Description = "3. Eager Loading + AsSplitQuery")]
        public void EagerLoading_SplitQuery()
        {
            var customers = _context.Customers
                .Include(c => c.Orders)
                .AsSplitQuery()
                .ToList();
        }

        [Benchmark(Description = "4. Projection")]
        public void Projection()
        {
            var dtos = _context.Customers
                .Select(c => new CustomerSummaryDto
                {
                    Id = c.Id,
                    FullName = $"{c.FirstName} {c.LastName}",
                    City = c.City,
                    OrderCount = c.Orders.Count(),
                    TotalSpent = c.Orders.Sum(o => o.Total),
                    LatestOrderDate = c.Orders.Max(o => o.CreatedAt)
                })
                .ToList();
        }
    }
}
