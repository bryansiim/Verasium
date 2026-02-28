using System.Linq;
using Google.GenAI;
using Google.GenAI.Types;

namespace Verasium.Core
{
    //Classe responsável por fazer a análise da IA usando o Gemini
    public class GeminiAnalyzer
    {
        private readonly Client client;

        public GeminiAnalyzer()
        {
            //O cliente já le automaticamente a variavel GOOGLE_API_KEY setada no ambiente
            client = new Client();
        }

        public async Task<AIAnalysisResult> AnalyzeAsync(string contentSummary)
        {
            var result = new AIAnalysisResult();

            try
            {
                var config = new GenerateContentConfig
                {
                    SystemInstruction = new Content
                    {
                        Parts = new List<Part>
                        {
                            new Part { Text = Prompts.AiDetectionSystemPrompt }
                        }
                    },
                    Temperature = 0.2,
                    MaxOutputTokens = 1024
                };

                var response = await client.Models.GenerateContentAsync(
                    model: "gemini-2.5-flash",
                    contents: contentSummary,
                    config: config
                );

                result.AiResponse = response?.Candidates?.FirstOrDefault()
                    ?.Content?.Parts?.FirstOrDefault()?.Text
                    ?? "Sem resposta do modelo.";
                result.IsSuccessful = true;
            }
            catch (Exception ex)
            {
                result.IsSuccessful = false;
                result.ErrorMessage = $"Erro na análise de IA: {ex.Message}";
            }

            return result;
        }
    }
}
