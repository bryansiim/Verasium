namespace Verasium.Core
{
    //Guarda o resultado da análise da IA
    public class AIAnalysisResult
    {
        public string? AiResponse { get; set; }
        public bool IsSuccessful { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
