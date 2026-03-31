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

        //Classifica a exceção e retorna uma mensagem amigável
        private static string ClassifyError(Exception ex)
        {
            string msg = ex.Message?.ToLowerInvariant() ?? "";
            string inner = ex.InnerException?.Message?.ToLowerInvariant() ?? "";
            string full = msg + " " + inner;

            // Rate limit / quota exceeded
            if (full.Contains("429") || full.Contains("rate limit") || full.Contains("quota")
                || full.Contains("resource exhausted") || full.Contains("too many requests"))
                return "Estamos recebendo muitas solicitações no momento. Por favor, aguarde alguns instantes e tente novamente.";

            // Conteúdo bloqueado por safety filters
            if (full.Contains("safety") || full.Contains("blocked") || full.Contains("harm")
                || full.Contains("recitation"))
                return "O conteúdo enviado foi bloqueado pelos filtros de segurança. Tente com outro conteúdo.";

            // Timeout
            if (full.Contains("timeout") || full.Contains("deadline") || full.Contains("timed out"))
                return "A análise demorou mais do que o esperado. Tente novamente ou envie um conteúdo menor.";

            // Arquivo muito grande para a API
            if (full.Contains("payload too large") || full.Contains("request entity too large")
                || full.Contains("content too large") || full.Contains("too long"))
                return "O conteúdo enviado é grande demais para ser processado. Tente com um arquivo menor.";

            // API key / auth
            if (full.Contains("401") || full.Contains("403") || full.Contains("api key")
                || full.Contains("permission") || full.Contains("unauthorized"))
                return "Estamos com um problema temporário no serviço. Tente novamente em alguns minutos.";

            // Erro genérico
            return "Estamos com um problema no momento. Por favor, tente novamente.";
        }

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
                    ErrorMessage = ClassifyError(ex)
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
                    ErrorMessage = ClassifyError(ex)
                };
            }
        }

        //Analisa o conteudo extraido de um PDF (texto + imagens)
        public async Task<AIAnalysisResult> AnalyzePdfAsync(PdfContent pdfContent)
        {
            try
            {
                bool hasText = !string.IsNullOrWhiteSpace(pdfContent.ExtractedText);
                bool hasImages = pdfContent.Images.Count > 0;

                // Se tem so texto, analisa como texto com prompt de PDF
                if (hasText && !hasImages)
                {
                    return await AnalyzePdfTextAsync(pdfContent.ExtractedText);
                }

                // Se tem so imagens, analisa cada imagem e agrega
                if (!hasText && hasImages)
                {
                    return await AnalyzePdfImagesAsync(pdfContent.Images);
                }

                // Se tem ambos, analisa texto e imagens separadamente e agrega
                if (hasText && hasImages)
                {
                    var textTask = AnalyzePdfTextAsync(pdfContent.ExtractedText);
                    var imageTask = AnalyzePdfImagesAsync(pdfContent.Images);
                    await Task.WhenAll(textTask, imageTask);

                    var textResult = textTask.Result;
                    var imageResult = imageTask.Result;

                    return AggregateResults(textResult, imageResult, "pdf");
                }

                return new AIAnalysisResult
                {
                    IsSuccessful = false,
                    ErrorMessage = "O PDF nao contem texto nem imagens extraiveis."
                };
            }
            catch (Exception ex)
            {
                return new AIAnalysisResult
                {
                    IsSuccessful = false,
                    ErrorMessage = ClassifyError(ex)
                };
            }
        }

        //Analisa o texto extraido de um PDF
        private async Task<AIAnalysisResult> AnalyzePdfTextAsync(string text)
        {
            try
            {
                var config = new GenerateContentConfig
                {
                    SystemInstruction = new Content
                    {
                        Parts = new List<Part>
                        {
                            new Part { Text = Prompts.PdfTextAnalysisSystemPrompt }
                        }
                    },
                    Temperature = 0.2,
                    MaxOutputTokens = 8192,
                    ResponseMimeType = "application/json"
                };

                var response = await client.Models.GenerateContentAsync(
                    model: "gemini-2.5-flash",
                    contents: text,
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
                    ErrorMessage = ClassifyError(ex)
                };
            }
        }

        //Analisa imagens extraidas de um PDF
        private async Task<AIAnalysisResult> AnalyzePdfImagesAsync(List<PdfImage> images)
        {
            var tasks = images.Select(img =>
                AnalyzeImageAsync("Imagem extraida de um documento PDF. Sem metadados disponiveis.", img.Bytes, img.MimeType)
            ).ToList();

            var results = await Task.WhenAll(tasks);
            var successful = results.Where(r => r.IsSuccessful).ToList();

            if (successful.Count == 0)
            {
                return new AIAnalysisResult
                {
                    IsSuccessful = false,
                    ErrorMessage = "Nenhuma imagem do PDF foi analisada com sucesso."
                };
            }

            if (successful.Count == 1)
                return successful[0];

            // Agrega resultados de multiplas imagens
            var aggregated = successful[0];
            for (int i = 1; i < successful.Count; i++)
            {
                aggregated = AggregateResults(aggregated, successful[i], "pdf");
            }
            return aggregated;
        }

        //Agrega dois resultados de analise em um unico resultado
        private static AIAnalysisResult AggregateResults(AIAnalysisResult a, AIAnalysisResult b, string contentType)
        {
            if (!a.IsSuccessful) return b;
            if (!b.IsSuccessful) return a;

            var allIndicators = new List<AnalysisIndicator>();
            allIndicators.AddRange(a.Indicators);
            allIndicators.AddRange(b.Indicators);

            string justification = $"{a.Justification ?? ""} {b.Justification ?? ""}".Trim();

            var result = new AIAnalysisResult
            {
                IsSuccessful = true,
                Justification = justification,
                ContentType = contentType,
                Indicators = allIndicators
            };

            ComputeScoreAndConclusion(result);
            return result;
        }

        //Analisa um video enviando-o diretamente ao Gemini
        public async Task<AIAnalysisResult> AnalyzeVideoAsync(byte[] videoBytes, string mimeType)
        {
            try
            {
                var config = new GenerateContentConfig
                {
                    SystemInstruction = new Content
                    {
                        Parts = new List<Part>
                        {
                            new Part { Text = Prompts.VideoAnalysisSystemPrompt }
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
                        new Part { Text = "Analise este video e determine se foi gerado por inteligencia artificial." },
                        new Part
                        {
                            InlineData = new Blob
                            {
                                MimeType = mimeType,
                                Data = videoBytes
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
                    ErrorMessage = ClassifyError(ex)
                };
            }
        }

        //Analisa um audio enviando-o diretamente ao Gemini
        public async Task<AIAnalysisResult> AnalyzeAudioAsync(byte[] audioBytes, string mimeType)
        {
            try
            {
                var config = new GenerateContentConfig
                {
                    SystemInstruction = new Content
                    {
                        Parts = new List<Part>
                        {
                            new Part { Text = Prompts.AudioAnalysisSystemPrompt }
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
                        new Part { Text = "Analise este audio e determine se foi gerado por inteligencia artificial." },
                        new Part
                        {
                            InlineData = new Blob
                            {
                                MimeType = mimeType,
                                Data = audioBytes
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
                    ErrorMessage = ClassifyError(ex)
                };
            }
        }

        //Calcula o score de IA (0-100) a partir dos indicadores
        private static int ComputeScoreFromIndicators(List<AnalysisIndicator> indicators)
        {
            if (indicators == null || indicators.Count == 0) return 50;

            var weights = new Dictionary<string, int>
            {
                { "strong_ai", 2 },
                { "weak_ai", 1 },
                { "neutral", 0 },
                { "weak_human", -1 },
                { "strong_human", -2 },
            };

            int sum = 0;
            foreach (var ind in indicators)
                sum += weights.GetValueOrDefault(ind.Significance, 0);

            int n = indicators.Count;
            int min = -2 * n;
            int max = 2 * n;

            double normalized = (double)(sum - min) / (max - min) * 100;
            return Math.Clamp((int)Math.Round(normalized), 0, 100);
        }

        //Calcula score e conclusion a partir dos indicadores
        private static void ComputeScoreAndConclusion(AIAnalysisResult result)
        {
            if (!result.IsSuccessful) return;

            result.ConfidenceScore = ComputeScoreFromIndicators(result.Indicators);
            result.Conclusion = result.ConfidenceScore >= 65 ? "AI-Generated"
                              : result.ConfidenceScore <= 35 ? "Human-Made"
                              : "Inconclusive";
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
                    ComputeScoreAndConclusion(parsed);
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
                ComputeScoreAndConclusion(result);
                return result;
            }

            throw new InvalidOperationException("Nao foi possivel extrair campos do JSON.");
        }
    }
}
