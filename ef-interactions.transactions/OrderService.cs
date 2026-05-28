using ef_interactions.nplusone;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace ef_interactions.transactions;

public class OrderService(AppDbContext context)
{
    private readonly AppDbContext _context = context;

    public async Task<OrderCreationResult> CreateOrderWithPaymentAsync(int customerId, decimal amount)
    {
        await using var transaction = await _context.Database
            .BeginTransactionAsync(IsolationLevel.Snapshot);

        try
        {
            // Step 1: Create the Order
            var order = new Order
            {
                CustomerId = customerId,
                Total = amount,
                CreatedAt = DateTime.UtcNow,
                Status = "Processing"
            };

            // presume some useful database stuff is done here

            // Step 2: Create Savepoint ===
            await transaction.CreateSavepointAsync("BeforeOrderCreated");

            _context.Orders.Add(order);

            await _context.SaveChangesAsync(); // Save to get Order ID for payment simulation

            // Step 3: Simulate Payment
            var paymentResult = await SimulatePaymentApiAsync(order.Id, amount);

            if (!paymentResult.Success)
            {
                // Partial rollback to savepoint
                await transaction.RollbackToSavepointAsync("BeforeOrderCreated");

                await transaction.CommitAsync();

                Console.WriteLine($"Payment failed. Rolled back to savepoint.");
                return OrderCreationResult.Failure;
            }

            // Step 4: Success path
            order.Status = "Completed";
            order.TransactionId = paymentResult.TransactionId;

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            Console.WriteLine($"Order completed successfully. TxId: {order.TransactionId}");
            return new OrderCreationResult
            {
                Success = true,
                Result = order
            };
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();

            await transaction.CommitAsync();
            Console.WriteLine($"Error: Transaction fully rolled back. {ex.Message}");
            return OrderCreationResult.Failure;
        }
    }

    // Simulated external payment API
    private static async Task<PaymentResult> SimulatePaymentApiAsync(int orderId, decimal amount)
    {
        await Task.Delay(600); // Simulate network/API latency

        // For demo purposes: fail on very large amounts
        if (amount > 5000)
        {
            return new PaymentResult
            {
                Success = false,
                Message = "Insufficient funds or payment declined"
            };
        }

        return new PaymentResult
        {
            Success = true,
            TransactionId = $"TX-{Guid.NewGuid().ToString()[..12].ToUpper()}"
        };
    }
}