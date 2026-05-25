using Microsoft.EntityFrameworkCore;

namespace ef_interactions.login
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<User> Users => Set<User>();
        public DbSet<BiometricEvaluation> BiometricEvaluations => Set<BiometricEvaluation>();
        public DbSet<BiometricMeasure> BiometricMeasures => Set<BiometricMeasure>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User configuration
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // BiometricEvaluation configuration
            modelBuilder.Entity<BiometricEvaluation>()
                .HasOne(be => be.User)
                .WithMany()
                .HasForeignKey(be => be.UserId)
                .OnDelete(DeleteBehavior.Cascade);   // If user is deleted, remove their evaluations

            modelBuilder.Entity<BiometricEvaluation>()
                .HasKey(be => be.EvaluationId);

            modelBuilder.Entity<BiometricEvaluation>()
                .HasMany(be => be.Measures)
                .WithOne(m => m.Evaluation)
                .HasForeignKey(m => m.EvaluationId)
                .OnDelete(DeleteBehavior.Cascade);

            // BiometricMeasure configuration
            modelBuilder.Entity<BiometricMeasure>()
                .Property(m => m.MeasureCode)
                .HasColumnType("CHAR(4)")
                .IsRequired();

            modelBuilder.Entity<BiometricMeasure>()
                .HasKey(m => m.MeasureId);

            modelBuilder.Entity<BiometricMeasure>()
                .Property(m => m.MeasureValue)
                .HasColumnType("DECIMAL(8,3)")
                .IsRequired();

            // Optional: Index for faster lookups by evaluation
            modelBuilder.Entity<BiometricMeasure>()
                .HasIndex(m => m.EvaluationId);
        }
    }
}
