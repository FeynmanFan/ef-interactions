using Microsoft.EntityFrameworkCore;

namespace ef_interactions.search
{
    public class AppDbContext(DbContextOptions<search.AppDbContext> options) : DbContext(options)
    {
        public DbSet<Document> Documents => Set<Document>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Document>(entity =>
            {
                entity.HasKey(d => d.Id);

                // Important: Configure the VECTOR column for SQL Server
                entity.Property(d => d.Embedding)
                      .HasColumnType("VECTOR(1536)");   // Matches text-embedding-3-small

                entity.HasIndex(d => d.Category);
            });
        }
    }
}
