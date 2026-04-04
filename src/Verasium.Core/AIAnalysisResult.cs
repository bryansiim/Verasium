using System.Text.Json.Serialization;

namespace Verasium.Core
{
    //Guarda o resultado da análise da IA
    public class AIAnalysisResult
    {
        public bool IsSuccessful { get; set; }
        public string? ErrorMessage { get; set; }

        [JsonPropertyName("conclusion")]
        public string? Conclusion { get; set; }

        [JsonPropertyName("confidenceScore")]
        public int ConfidenceScore { get; set; }

        [JsonPropertyName("justification")]
        public string? Justification { get; set; }

        [JsonPropertyName("contentType")]
        public string? ContentType { get; set; }

        [JsonPropertyName("indicators")]
        public List<AnalysisIndicator> Indicators { get; set; } = new();

        [JsonPropertyName("lowConfidence")]
        public bool LowConfidence { get; set; }
    }
}
