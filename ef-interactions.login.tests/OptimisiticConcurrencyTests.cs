namespace ef_interactions.login.tests
{
    using Microsoft.EntityFrameworkCore;

    public class OptimisticConcurrencyTests
    {
        private nplusone.AppDbContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<nplusone.AppDbContext>()
        .UseSqlServer("Server=localhost,1433;Database=CompanyDb;User Id=sa;Password=YourStrongPassword123!;TrustServerCertificate=True;")
        .Options;
            return new nplusone.AppDbContext(options);
        }

    }
}
