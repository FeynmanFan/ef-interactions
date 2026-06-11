namespace ef_interactions.benchmarker
{
    using BenchmarkDotNet.Attributes;
    using BenchmarkDotNet.Order;
    using ef_interactions.nplusone;
    using Microsoft.EntityFrameworkCore;

    [MemoryDiagnoser]
    [RankColumn]
    [Orderer(SummaryOrderPolicy.FastestToSlowest)]
    public class ChangeTrackingBenchmarks
    {
        public AppDbContext CreateFreshContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlServer("Server=localhost,1433;Database=CompanyDb;User Id=sa;Password=YourStrongPassword123!;TrustServerCertificate=True;")
                .Options;

            return new AppDbContext(options);
        }

        [Benchmark(Baseline = true)]
        public int WithTracking()
        {
            using var ctx = CreateFreshContext();
            return ctx.Customers
                .Include(c => c.Orders)
                .ThenInclude(o => o.Items)
                    .ThenInclude(i => i.Product)
                        .ThenInclude(p => p.Category)
                .ToList().Count;
        }

        [Benchmark]
        public int WithAsNoTracking()
        {
            using var ctx = CreateFreshContext();
            return ctx.Customers
                .Include(c => c.Orders)
                .ThenInclude(o => o.Items)
                    .ThenInclude(i => i.Product)
                        .ThenInclude(p => p.Category)
                .AsNoTracking()
                .ToList().Count;
        }

        [Benchmark]
        public int WithIdentityResolution()
        {
            using var ctx = CreateFreshContext();
            return ctx.Customers
                .Include(c => c.Orders)
                .ThenInclude(o => o.Items)
                    .ThenInclude(i => i.Product)
                        .ThenInclude(p => p.Category)
                .AsNoTrackingWithIdentityResolution()
                .ToList().Count;
        }
    }
}