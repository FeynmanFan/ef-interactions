using Microsoft.EntityFrameworkCore;

namespace ef_interactions.login.tests
{
    public class Deleter
    {
        [Fact]
        public void DeleteOldEvaluations()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlServer("Server=localhost,1433;Database=CompanyDb;User Id=sa;Password=YourStrongPassword123!;TrustServerCertificate=True;")
                .Options;
            using var context = new AppDbContext(options);
            DateTime cutoffDate = DateTime.UtcNow.AddMonths(-6); // Delete evaluations older than 6 months

            using var transaction = context.Database.BeginTransaction();

            try
            {
                context.BiometricMeasures.Where(bm => context.BiometricEvaluations
                .Where(be => be.EvaluationDate < cutoffDate)
                .Select(be => be.EvaluationId)
                .Contains(bm.EvaluationId))
                .ExecuteDelete();

                throw new InvalidOperationException("Something broke");

                int recordsAffected = context.BiometricEvaluations
                    .Where(be => be.EvaluationDate < cutoffDate)
                    .ExecuteDelete();
                Console.WriteLine($"{recordsAffected} old biometric evaluations deleted.");

                transaction.Commit();
            }
            catch(Exception ex) // capture a more specific exception type if possible, e.g., DbUpdateException
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                transaction.Rollback();
            }
        }
    }
}
