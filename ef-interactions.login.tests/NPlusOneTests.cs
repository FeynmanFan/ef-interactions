namespace ef_interactions.tests;

using Microsoft.EntityFrameworkCore;
using Xunit;

public class NPlusOneTests(ITestOutputHelper output)
{
    private readonly ITestOutputHelper _output = output;

    [Fact]
    public void NPlusOneProblem()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<nplusone.AppDbContext>()
                .UseSqlServer("Server=localhost,1433;Database=CompanyDb;User Id=sa;Password=YourStrongPassword123!;TrustServerCertificate=True;")
                .Options;

        using var context = new nplusone.AppDbContext(options);

        var customers = context.Customers.Where(x => x.City == "Fort Worth").ToList(); // 1 query

        _output.WriteLine($"{customers.Count} customers in the query");

        foreach (var customer in customers)
        {
            var orders = customer.Orders.ToList(); // ← N queries!
            _output.WriteLine("Customer {0} has {1} orders.", customer.Id, orders.Count);
        }

        Assert.Fail("N+1 problem demonstrated. Check the logs to see the number of queries executed.");
    }
}
