using Microsoft.EntityFrameworkCore;

namespace ef_interactions.login.tests
{
    public class PasswordUpdater
    {
        [Fact]
        public void UpdatePasswords()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlServer("Server=localhost,1433;Database=CompanyDb;User Id=sa;Password=YourStrongPassword123!;TrustServerCertificate=True;")
                .Options;

            using var context = new AppDbContext(options);

            int recordsAffected = context.Users.ExecuteUpdate<User>(u => u.SetProperty(user => user.PasswordHash, user => UnbreakableEncryption(user.PasswordHash)));

            Console.WriteLine($"{recordsAffected} user passwords updated.");    
        }

        public string UnbreakableEncryption(string password)
        {
            // Placeholder for a real hashing algorithm
            return new string(password.Reverse().ToArray());
        }
    }
}
