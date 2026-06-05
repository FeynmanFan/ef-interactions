using ef_interactions.nplusone;
using ef_interactions.transactions;
using Microsoft.EntityFrameworkCore;
using System.Transactions;
using System.Data;

namespace ef_interactions.tests;

public class TransactionTests
{
    private static AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
        .UseSqlServer("Server=localhost,1433;Database=CompanyDb;User Id=sa;Password=YourStrongPassword123!;TrustServerCertificate=True;")
        .Options;

        var context = new AppDbContext(options);
        return context;
    }

    [Fact]
    public async Task CommitsTransaction()
    {
        // Arrange
        int customerId = 1; // Chris Behrens

        var context = CreateContext();
        var service = new OrderService(context);

        // Act
        var result = await service.CreateOrderWithPaymentAsync(customerId, 199.99m);

        // Assert
        Assert.True(result.Success);

        var order = result.Result;

        Assert.NotNull(order);
        Assert.Equal("Completed", order.Status);
        Assert.NotNull(order.TransactionId);
        Assert.Equal(199.99m, order.Total);
    }

    [Fact]
    public async Task RollsBackTransaction()
    {
        // Arrange
        using var context = CreateContext();
        var service = new OrderService(context);
        int customerId = 1; // Chris Behrens

        var initialOrderCount = await context.Orders.CountAsync(cancellationToken: TestContext.Current.CancellationToken);

        // Act - Force payment failure by using a special amount (you can adjust logic if needed)
        var result = await service.CreateOrderWithPaymentAsync(customerId, 999999m); // High amount to trigger failure in demo

        // Assert
        Assert.False(result.Success);

        // Verify no order was saved (transaction was rolled back)
        var finalOrderCount = await context.Orders.CountAsync(cancellationToken: TestContext.Current.CancellationToken);
        Assert.Equal(initialOrderCount, finalOrderCount);
    }

    [Fact]
    public async Task DetectsConcurrencyConflict()
    {
        using var context1 = CreateContext();
        using var context2 = CreateContext();

        const int CustomerId = 1; // Chris Behrens

        // Arrange: Both "clients" load the same customer
        var customer1 = await context1.Customers.FindAsync([CustomerId], TestContext.Current.CancellationToken);
        var customer2 = await context2.Customers.FindAsync([CustomerId], TestContext.Current.CancellationToken);

        var customer1RowVersion = customer1.RowVersion;
        var customer2RowVersion = customer2.RowVersion;

        Assert.Equal(customer1RowVersion, customer2RowVersion);

        // Act: Client 1 updates first
        customer1.Balance += 100;
        await context1.SaveChangesAsync(TestContext.Current.CancellationToken);

        customer1RowVersion = customer1.RowVersion;
        Assert.NotEqual(customer1RowVersion, customer2RowVersion); // RowVersion should have changed

        // Client 2 tries to update the same record (based on old data)
        customer2.Balance += 50;

        customer2RowVersion = customer2.RowVersion;
        Assert.NotEqual(customer1RowVersion, customer2RowVersion); // RowVersion should have changed

        // Assert: Should throw concurrency exception
        await Assert.ThrowsAsync<DbUpdateConcurrencyException>(
            async () => await context2.SaveChangesAsync(TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task Explicit_Locking()
    {
        using var context = CreateContext();

        var customer = new Customer { FirstName = "LockTest", LastName = "User", Address1 = "1234 Testarossa Blvd", City = "Centerville", State = "TX", ZipCode = "70210" };
        context.Customers.Add(customer);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);

        await using var transaction = await context.Database
            .BeginTransactionAsync(System.Data.IsolationLevel.RepeatableRead, TestContext.Current.CancellationToken);

        try
        {
            // Lock Customer row early
           await context.Customers
                .FromSql($"SELECT Id, FirstName, LastName, Address1, City, State, ZipCode, Balance, RowVersion FROM Customers WITH (UPDLOCK, HOLDLOCK) WHERE Id = {customer.Id}")
                .ToListAsync(cancellationToken: TestContext.Current.CancellationToken);

            // Lock Orders with UPDLOCKUPD
            var orders = await context.Orders
                .FromSql($"SELECT Id, CustomerId, Total, CreatedAt, Status, TransactionId FROM Orders WITH (UPDLOCK) WHERE CustomerId = {customer.Id}")
                .ToListAsync(cancellationToken: TestContext.Current.CancellationToken);

            // do stuff that requires the transactions to be held until the end of the transaction scope

            await transaction.CommitAsync(TestContext.Current.CancellationToken);
        }
        catch
        {
            await transaction.RollbackAsync(TestContext.Current.CancellationToken);
            throw;
        }
    }
}