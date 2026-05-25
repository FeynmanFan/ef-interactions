namespace ef_interactions.search
{
    using Microsoft.Data.SqlTypes;
    using Microsoft.EntityFrameworkCore;
    using OpenAI.Embeddings;
    using System.ClientModel;

    public class Document
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;

        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

        public SqlVector<float> Embedding { get; set; }

        // ======================
        // Static Service Methods
        // ======================

        // The OpenAI API key should not be stored in source control. Read it from an
        // environment variable (or user-secrets / configuration) at runtime.
        private static EmbeddingClient? _embeddingClient;

        private static EmbeddingClient EmbeddingClientInstance
        {
            get
            {
                if (_embeddingClient != null)
                    return _embeddingClient;

                var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY");
                if (string.IsNullOrWhiteSpace(apiKey))
                    throw new InvalidOperationException("OpenAI API key not configured. Set the OPENAI_API_KEY environment variable or use user-secrets.");

                _embeddingClient = new EmbeddingClient("text-embedding-3-small", apiKey);
                return _embeddingClient;
            }
        }

        /// <summary>
        /// Creates and saves a new document with its embedding
        /// </summary>
        public static async Task<Document> CreateAsync(
            AppDbContext context,
            string title,
            string content,
            string category)
        {
            var embeddingResult = await EmbeddingClientInstance.GenerateEmbeddingAsync(content);

            // call extension method with parentheses
            var vector = embeddingResult.ToArray();
            var document = new Document
            {
                Title = title,
                Content = content,
                Category = category,
                LastUpdated = DateTime.UtcNow,
                Embedding = new SqlVector<float>(vector)
            };

            context.Documents.Add(document);
            await context.SaveChangesAsync();

            return document;
        }

        /// <summary>
        /// Performs semantic search across all documents
        /// </summary>
        public static async Task<List<Document>> SemanticSearchAsync(
            AppDbContext context,
            string question,
            int top = 5)
        {
            var queryEmbeddingResult = await EmbeddingClientInstance.GenerateEmbeddingAsync(question);
            float[] vector = queryEmbeddingResult.ToArray();

            return await context.Documents
                .OrderBy(d => EF.Functions.VectorDistance<float>("cosine", d.Embedding, new SqlVector<float>(vector)))
                .Take(top)
                .ToListAsync();
        }
    }

    /// <summary>
    /// Helper class for extension method
    /// </summary>
    public static class OpenAIEmbeddingExtensions
    {
        public static float[] ToArray(this ClientResult<OpenAIEmbedding> result)
        {
            if (result == null || result.Value == null)
                throw new ArgumentNullException(nameof(result), "Embedding result cannot be null.");

            ReadOnlyMemory<float> vectorMemory = result.Value.ToFloats();
            return vectorMemory.ToArray();
        }
    }
}
