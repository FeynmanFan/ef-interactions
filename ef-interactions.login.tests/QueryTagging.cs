namespace ef_interactions.login.tests
{
    using ef_interactions.nplusone;
    using Microsoft.EntityFrameworkCore;

    public class QueryTaggingTests: IDisposable
    {
        private readonly AppDbContext _context;
        private readonly ITestOutputHelper _output;

        public QueryTaggingTests(ITestOutputHelper output)
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlServer("Server=localhost,1433;Database=CompanyDb;User Id=sa;Password=YourStrongPassword123!;TrustServerCertificate=True;")
                .Options;

            _context = new AppDbContext(options);
            _output = output;
        }

        [Fact]
        public void TagWith_Should_Add_Custom_Tag_To_SQL()
        {
            var query = _context.Customers
                .TagWith("Unit Test - Get all customers")
                .AsNoTracking();

            var sql = query.ToQueryString();

            _output.WriteLine("=== Query with TagWith ===");
            _output.WriteLine(sql);
            _output.WriteLine("=====================================");

            // Execute the query so it shows up in SQL Profiler
            var result = query.ToList();
            Assert.NotEmpty(result);
        }

        [Fact]
        public void TagWithCallSite_Should_Add_File_And_Line_Number()
        {
            var query = _context.Customers
                .TagWithCallSite()
                .AsNoTracking();

            var sql = query.ToQueryString();

            _output.WriteLine("=== Query with TagWithCallSite ===");
            _output.WriteLine(sql);
            _output.WriteLine("=====================================");

            var result = query.ToList();
            Assert.NotEmpty(result);
        }

        [Fact]
        public void Can_Combine_Both_Tags()
        {
            var query = _context.Customers
                .TagWith("Dashboard - Recent customers")
                .TagWithCallSite()
                .AsNoTracking();

            var sql = query.ToQueryString();

            _output.WriteLine("=== Query with Both Tags ===");
            _output.WriteLine(sql);
            _output.WriteLine("=====================================");

            var result = query.ToList();
            Assert.NotEmpty(result);
        }
        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
