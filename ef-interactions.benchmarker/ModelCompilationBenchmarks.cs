namespace ef_interactions.benchmarker
{
    using BenchmarkDotNet.Attributes;
    using BenchmarkDotNet.Order;
    using ef_interactions.nplusone;
    using ef_interactions.nplusone.CompiledModels;
    using Microsoft.EntityFrameworkCore;

    [MemoryDiagnoser]
    [Orderer(SummaryOrderPolicy.FastestToSlowest)]
    public class ModelCompilationBenchmarks
    {
        // Normal DbContext (no compiled model)
        private static AppDbContext CreateNormalContext() {

            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlServer("Server=localhost,1433;Database=CompanyDb;User Id=sa;Password=YourStrongPassword123!;TrustServerCertificate=True;")
                .Options;
            return new AppDbContext(options);
        }

        // DbContext using Compiled Model
        private static AppDbContext CreateCompiledModelContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlServer("Server=localhost,1433;Database=CompanyDb;User Id=sa;Password=YourStrongPassword123!;TrustServerCertificate=True;")
                .UseModel(AppDbContextModel.Instance)  // ← Compiled Model
                .Options;

            return new AppDbContext(options);
        }

        [GlobalSetup]
        public void Setup()
        {
            // Warm up both contexts
            using var normal = CreateNormalContext();
            using var compiled = CreateCompiledModelContext();
        }

        // ===================================================================
        // Baseline: Normal DbContext + Normal Query
        // ===================================================================
        [Benchmark(Baseline = true)]
        public int Normal_DbContext()
        {
            using var ctx = CreateNormalContext();

            var result = ctx.Customers
                .Include(c => c.Orders)
                    .ThenInclude(o => o.Items)
                        .ThenInclude(i => i.Product)
                            .ThenInclude(p => p.Category)
                .AsNoTracking()
                .ToList();

            return result.Count;
        }

        // ===================================================================
        // Compiled Model + Same Query
        // ===================================================================
        [Benchmark]
        public int CompiledModel_DbContext()
        {
            using var ctx = CreateCompiledModelContext();

            var result = ctx.Customers
                .Include(c => c.Orders)
                    .ThenInclude(o => o.Items)
                        .ThenInclude(i => i.Product)
                            .ThenInclude(p => p.Category)
                .AsNoTracking()
                .ToList();

            return result.Count;
        }

        // ===================================================================
        // Optional: Add this if you want to highlight creation cost specifically
        // ===================================================================
        [Benchmark]
        public int CompiledModel_JustCreation()
        {
            using var ctx = CreateCompiledModelContext();
            return 0; // Just measuring context creation + model loading
        }
    }
}
