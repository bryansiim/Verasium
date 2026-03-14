using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using Google.GenAI;
using Google.GenAI.Types;

namespace Verasium.Core
{
    //Classe responsável por fazer a análise da IA usando o Gemini
    public class GeminiAnalyzer
    {
        private readonly Client client;
        private static readonly JsonSerializerOptions jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public GeminiAnalyzer()
        {
            //O cliente já le automaticamente a variavel GOOGLE_API_KEY setada no ambiente
            client = new Client();
        }

        //Analisa uma imagem enviando-a visualmente junto com os metadados
        public async Task<AIAnalysisResult> AnalyzeImageAsync(string metadataSummary, byte[] imageBytes, string mimeType)
        {
            try
            {
                var config = new GenerateContentConfig
                {
                    SystemInstruction = new Content
                    {
                        Parts = new List<Part>
                        {
                            new Part { Text = Prompts.ImageAnalysisSystemPrompt }
                        }
                    },
                    Temperature = 0.2,
                    MaxOutputTokens = 8192,
                    ResponseMimeType = "application/json"
                };

                var content = new Content
                {
                    Parts = new List<Part>
                    {
                        new Part { Text = metadataSummary },
                        new Part
                        {
                            InlineData = new Blob
                            {
                                MimeType = mimeType,
                                Data = imageBytes
                            }
                        }
                    }
                };

                var response = await client.Models.GenerateContentAsync(
                    model: "gemini-2.5-flash",
                    contents: content,
                    config: config
                );

                string rawResponse = response?.Candidates?.FirstOrDefault()
                    ?.Content?.Parts?.FirstOrDefault()?.Text
                    ?? "";

                return ParseGeminiJsonResponse(rawResponse);
            }
            catch (Exception ex)
            {
                return new AIAnalysisResult
                {
                    IsSuccessful = false,
                    ErrorMessage = $"Erro na análise de imagem: {ex.Message}"
                };
            }
        }

        //Analisa um texto buscando padrões de geração por IA
        public async Task<AIAnalysisResult> AnalyzeTextAsync(string textContent)
        {
            try
            {
                var config = new GenerateContentConfig
                {
                    SystemInstruction = new Content
                    {
                        Parts = new List<Part>
                        {
                            new Part { Text = Prompts.TextAnalysisSystemPrompt }
                        }
                    },
                    Temperature = 0.2,
                    MaxOutputTokens = 8192,
                    ResponseMimeType = "application/json"
                };

                var response = await client.Models.GenerateContentAsync(
                    model: "gemini-2.5-flash",
                    contents: textContent,
                    config: config
                );

                string rawResponse = response?.Candidates?.FirstOrDefault()
                    ?.Content?.Parts?.FirstOrDefault()?.Text
                    ?? "";

                return ParseGeminiJsonResponse(rawResponse);
            }
            catch (Exception ex)
            {
                return new AIAnalysisResult
                {
                    IsSuccessful = false,
                    ErrorMessage = $"Erro na análise de texto: {ex.Message}"
                };
            }
        }

        //Faz o parse do JSON retornado pelo Gemini em AIAnalysisResult
        private static AIAnalysisResult ParseGeminiJsonResponse(string rawResponse)
        {
            if (string.IsNullOrWhiteSpace(rawResponse))
            {
                return new AIAnalysisResult
                {
                    IsSuccessful = false,
                    ErrorMessage = "Nao foi possivel obter uma resposta do modelo. Tente novamente."
                };
            }

            string jsonText = rawResponse;

            // Se veio dentro de markdown fences, extrai
            var fenceMatch = Regex.Match(rawResponse, @"```(?:json)?\s*([\s\S]*?)```");
            if (fenceMatch.Success)
                jsonText = fenceMatch.Groups[1].Value.Trim();

            // Tenta deserializar o JSON completo
            try
            {
                var parsed = JsonSerializer.Deserialize<AIAnalysisResult>(jsonText, jsonOptions);
                if (parsed != null)
                {
                    parsed.IsSuccessful = true;
                    return parsed;
                }
            }
            catch { }

            // Se falhou (provavelmente JSON truncado), tenta extrair os campos principais via regex
            try
            {
                return ExtractPartialResult(jsonText);
            }
            catch { }

            return new AIAnalysisResult
            {
                IsSuccessful = false,
                ErrorMessage = "Nao foi possivel interpretar a resposta da IA. Tente novamente."
            };
        }

        //Extrai campos principais de um JSON truncado/malformado
        private static AIAnalysisResult ExtractPartialResult(string json)
        {
            var result = new AIAnalysisResult();

            var conclusionMatch = Regex.Match(json, @"""conclusion""\s*:\s*""([^""]+)""");
            if (conclusionMatch.Success)
                result.Conclusion = conclusionMatch.Groups[1].Value;

            var scoreMatch = Regex.Match(json, @"""confidenceScore""\s*:\s*(\d+)");
            if (scoreMatch.Success)
                result.ConfidenceScore = int.Parse(scoreMatch.Groups[1].Value);

            var justificationMatch = Regex.Match(json, @"""justification""\s*:\s*""((?:[^""\\]|\\.)*)""");
            if (justificationMatch.Success)
                result.Justification = Regex.Unescape(justificationMatch.Groups[1].Value);

            var contentTypeMatch = Regex.Match(json, @"""contentType""\s*:\s*""([^""]+)""");
            if (contentTypeMatch.Success)
                result.ContentType = contentTypeMatch.Groups[1].Value;

            // Tenta extrair indicadores completos (apenas os que tem JSON valido)
            var indicatorMatches = Regex.Matches(json,
                @"\{\s*""name""\s*:\s*""((?:[^""\\]|\\.)*)""\s*,\s*""finding""\s*:\s*""((?:[^""\\]|\\.)*)""\s*,\s*""significance""\s*:\s*""([^""]+)""\s*\}");
            foreach (Match m in indicatorMatches)
            {
                result.Indicators.Add(new AnalysisIndicator
                {
                    Name = Regex.Unescape(m.Groups[1].Value),
                    Finding = Regex.Unescape(m.Groups[2].Value),
                    Significance = m.Groups[3].Value
                });
            }

            // Se conseguiu ao menos a conclusao, considera sucesso
            if (!string.IsNullOrEmpty(result.Conclusion))
            {
                result.IsSuccessful = true;
                return result;
            }

            throw new InvalidOperationException("Nao foi possivel extrair campos do JSON.");
        }
    }
}
