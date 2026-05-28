using ef_interactions.nplusone;
using ef_interactions.transactions;
using Microsoft.EntityFrameworkCore;

namespace ef_interactions.tests;

public class OrderServiceTests
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
}