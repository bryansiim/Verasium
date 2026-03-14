using System.Text.Json.Serialization;

namespace Verasium.Core
{
    public class AnalysisIndicator
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("finding")]
        public string Finding { get; set; } = string.Empty;

        [JsonPropertyName("significance")]
        public string Significance { get; set; } = string.Empty;
    }
}
