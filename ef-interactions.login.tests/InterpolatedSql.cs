namespace ef_interactions.login.tests
{
    using Microsoft.EntityFrameworkCore;
    using Xunit;

    public class InterpolatedSql(ITestOutputHelper output)
    {
        [Fact]
        public void DemonstrateInterpolatedandRawSql()
        {
            var options = new DbContextOptionsBuilder<nplusone.AppDbContext>()
                    .UseSqlServer("Server=localhost,1433;Database=CompanyDb;User Id=sa;Password=YourStrongPassword123!;TrustServerCertificate=True;")
                    .Options;
            using var context = new nplusone.AppDbContext(options);

            var attackCity = "' OR 1=1;--";

            var sql = $"SELECT * FROM Customers WHERE City = '" + attackCity + "'";

            var customers = context.Customers.FromSqlRaw(sql).ToList();

            // this is a sql injection attack, so we expect to get all customers back
            Assert.Equal(2, customers.Count);

            var interpolatedCustomers = context.Customers.FromSqlInterpolated($"SELECT * FROM Customers WHERE City = {attackCity}").ToList();

            // the interpolated query should be parameterized, so we expect to get no customers back
            Assert.Equal(0, interpolatedCustomers.Count);
        }
    }
}
