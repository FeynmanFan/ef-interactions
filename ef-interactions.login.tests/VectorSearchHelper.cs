using ef_interactions.search;
using Microsoft.EntityFrameworkCore;

namespace ef_interactions.login.tests
{
    public class VectorSearchHelper(ITestOutputHelper output)
    {
        private readonly ITestOutputHelper _output = output;

        private static void LoadDotEnvIfPresent()
        {
            // Search for a .env file starting from the test run directory and walking up to repo root
            var dir = AppContext.BaseDirectory;
            for (int i = 0; i < 8 && !string.IsNullOrEmpty(dir); i++)
            {
                var candidate = Path.Combine(dir, ".env");
                if (File.Exists(candidate))
                {
                    foreach (var line in File.ReadAllLines(candidate))
                    {
                        var trimmed = line.Trim();
                        if (string.IsNullOrEmpty(trimmed) || trimmed.StartsWith("#"))
                            continue;

                        var idx = trimmed.IndexOf('=');
                        if (idx <= 0)
                            continue;

                        var key = trimmed.Substring(0, idx).Trim();
                        var value = trimmed.Substring(idx + 1).Trim();
                        // Remove optional surrounding quotes
                        if (value.Length >= 2 && ((value.StartsWith("\"") && value.EndsWith("\"")) || (value.StartsWith("'") && value.EndsWith("'"))))
                        {
                            value = value.Substring(1, value.Length - 2);
                        }

                        Environment.SetEnvironmentVariable(key, value);
                    }

                    return;
                }

                dir = Path.GetDirectoryName(dir);
            }
        }
        
        [Fact]
        public async Task SeedDB()
        {
            LoadDotEnvIfPresent();
            var options = new DbContextOptionsBuilder<search.AppDbContext>()
                .UseSqlServer("Server=localhost,1433;Database=CompanyDb;User Id=sa;Password=YourStrongPassword123!;TrustServerCertificate=True;")
                .Options;

            var context = new search.AppDbContext(options);

            await Document.CreateAsync(context, 
                "Progressive Overload Explained",
                "Progressive overload is the gradual increase of stress placed on the body during exercise. This is the most fundamental principle for muscle growth and strength gains.",
                "Hypertrophy");

            await Document.CreateAsync(context,
                "The Importance of Sleep for Recovery",
                "During deep sleep, your body releases growth hormone and repairs muscle tissue. Most muscle recovery and adaptation occurs while you sleep, not in the gym.",
                "Recovery");

            await Document.CreateAsync(context,
                "Why Protein Timing Matters",
                "Consuming 20-40g of high-quality protein within 2 hours after training maximizes muscle protein synthesis. This is known as the anabolic window.",
                "Nutrition");

            await Document.CreateAsync(context,
                "Mobility vs Flexibility Training",
                "Flexibility is the ability of a muscle to lengthen. Mobility is the ability to control movement through a range of motion. Both are important but serve different purposes.",
                "Training");

            await Document.CreateAsync(context,
                "How to Fix Lower Back Pain from Squats",
                "Most lower back pain during squats comes from poor bracing, excessive spinal flexion, or weak core muscles. Focus on core stability and neutral spine position.",
                "Injury Prevention");
        }

        [Fact]
        public async Task SearchTest()
        {
            LoadDotEnvIfPresent();
            var options = new DbContextOptionsBuilder<search.AppDbContext>()
                .UseSqlServer("Server=localhost,1433;Database=CompanyDb;User Id=sa;Password=YourStrongPassword123!;TrustServerCertificate=True;")
                .Options;

            var context = new search.AppDbContext(options);

            var question = "How best to consume protein for hypertrophy";

            // Act
            var results = await Document.SemanticSearchAsync(context, question, top: 5);

            // Output - This will appear clearly in the test runner
            _output.WriteLine($"=== Semantic Search Results for: '{question}' ===");
            _output.WriteLine($"Found {results.Count} documents\n");

            foreach (var doc in results)
            {
                _output.WriteLine($"Title: {doc.Title}");
                _output.WriteLine($"Category: {doc.Category}");
                _output.WriteLine($"Content Preview: {doc.Content.Substring(0, Math.Min(150, doc.Content.Length))}...");
                _output.WriteLine("".PadRight(80, '-'));
            }
        }
    }
}
