using System.ComponentModel.DataAnnotations;

namespace ef_interactions.login
{
    public class BiometricEvaluation
    {
        public int EvaluationId { get; set; }
        public int UserId { get; set; }
        public DateTime EvaluationDate { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation properties
        public User? User { get; set; }
        public ICollection<BiometricMeasure> Measures { get; set; } = [];
    }
}
