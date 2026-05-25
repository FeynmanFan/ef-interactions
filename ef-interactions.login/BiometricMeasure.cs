using System.ComponentModel.DataAnnotations;

namespace ef_interactions.login
{
    public class BiometricMeasure
    {
        public int MeasureId { get; set; }
        public int EvaluationId { get; set; }
        public string MeasureCode { get; set; } = string.Empty;   // e.g., "BFPT", "WGTL"
        public decimal MeasureValue { get; set; }
        public string? Unit { get; set; }
        public string? Notes { get; set; }

        // Navigation property
        public BiometricEvaluation? Evaluation { get; set; }
    }
}