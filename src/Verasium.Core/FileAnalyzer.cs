using System.Linq;
using System.Text;

namespace Verasium.Core
{
    //Essa classe é responsável por orquestrar a análise e devolver o resultado
    public class FileAnalyzer
    {
        private readonly string inputContent;
        private readonly GeminiAnalyzer geminiAnalyzer;
        private readonly MetadataExtractorService metadataExtractor;

        private static readonly Dictionary<string, string> ImageMimeTypes = new(StringComparer.OrdinalIgnoreCase)
        {
            { ".jpg", "image/jpeg" },
            { ".jpeg", "image/jpeg" },
            { ".png", "image/png" },
            { ".gif", "image/gif" },
            { ".webp", "image/webp" },
            { ".bmp", "image/bmp" }
        };

        private static readonly Dictionary<string, string> VideoMimeTypes = new(StringComparer.OrdinalIgnoreCase)
        {
            { ".mp4", "video/mp4" },
            { ".mpeg", "video/mpeg" },
            { ".mov", "video/quicktime" },
            { ".avi", "video/x-msvideo" },
            { ".webm", "video/webm" },
            { ".mkv", "video/x-matroska" }
        };

        private static readonly Dictionary<string, string> AudioMimeTypes = new(StringComparer.OrdinalIgnoreCase)
        {
            { ".mp3", "audio/mpeg" },
            { ".wav", "audio/wav" },
            { ".aac", "audio/aac" },
            { ".ogg", "audio/ogg" },
            { ".flac", "audio/flac" },
            { ".m4a", "audio/mp4" }
        };

        private static readonly HashSet<string> PdfExtensions = new(StringComparer.OrdinalIgnoreCase) { ".pdf" };

        public FileAnalyzer(string inputContent, GeminiAnalyzer geminiAnalyzer)
        {
            this.inputContent = inputContent
                ?? throw new ArgumentNullException(nameof(inputContent));
            this.geminiAnalyzer = geminiAnalyzer
                ?? throw new ArgumentNullException(nameof(geminiAnalyzer));
            this.metadataExtractor = new MetadataExtractorService();
        }

        //Roda a análise completa e retorna o resultado
        public async Task<AIAnalysisResult> RunAnalysis()
        {
            AIAnalysisResult result;

            // Se é um arquivo que existe no disco
            if (File.Exists(inputContent))
            {
                string extension = Path.GetExtension(inputContent);

                // Se é uma imagem, faz análise multimodal (visual + metadados)
                if (ImageMimeTypes.TryGetValue(extension, out string? mimeType))
                {
                    result = await AnalyzeImage(mimeType);
                    return await ApplyTwoPassIfNeeded(result);
                }

                // Se é um PDF, extrai texto e imagens e analisa
                if (PdfExtensions.Contains(extension))
                {
                    result = await AnalyzePdf();
                    return await ApplyTwoPassIfNeeded(result);
                }

                // Se é um video, envia diretamente ao Gemini
                if (VideoMimeTypes.TryGetValue(extension, out string? videoMime))
                {
                    result = await AnalyzeVideo(videoMime);
                    return await ApplyTwoPassIfNeeded(result);
                }

                // Se é um audio, envia diretamente ao Gemini
                if (AudioMimeTypes.TryGetValue(extension, out string? audioMime))
                {
                    result = await AnalyzeAudio(audioMime);
                    return await ApplyTwoPassIfNeeded(result);
                }

                // Se é um arquivo de texto, lê o conteúdo e faz análise textual
                string fileContent = await File.ReadAllTextAsync(inputContent);
                result = await AnalyzeText(fileContent);
                return await ApplyTwoPassIfNeeded(result);
            }

            // Se não é arquivo, trata como texto direto
            result = await AnalyzeText(inputContent);
            return await ApplyTwoPassIfNeeded(result);
        }

        //Aplica segunda passada de revisao critica se o resultado for borderline
        private async Task<AIAnalysisResult> ApplyTwoPassIfNeeded(AIAnalysisResult firstPass)
        {
            if (!firstPass.IsSuccessful) return firstPass;
            if (firstPass.Conclusion != "Inconclusive") return firstPass;
            if (firstPass.ConfidenceScore < 40 || firstPass.ConfidenceScore > 60) return firstPass;

            var secondPass = await geminiAnalyzer.CriticalReviewAsync(firstPass, firstPass.ContentType ?? "text");

            if (secondPass.IsSuccessful && secondPass.Conclusion != "Inconclusive")
                return secondPass;

            return firstPass;
        }

        private static AIAnalysisResult? TryShortCircuitTrivialText(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return null;

            int wordCount = text.Split(new[] { ' ', '\t', '\n', '\r' },
                StringSplitOptions.RemoveEmptyEntries).Length;

            if (text.Length < 50 || wordCount < 3)
            {
                return new AIAnalysisResult
                {
                    IsSuccessful = true,
                    Conclusion = "Human-Made",
                    ConfidenceScore = 15,
                    ContentType = "text",
                    Justification = "Texto extremamente curto. Textos com menos de 50 caracteres ou 3 palavras sao consistentes com digitacao humana casual, mensagens rapidas ou testes.",
                    Indicators = new List<AnalysisIndicator>
                    {
                        new() { Name = "Brevidade extrema", Finding = $"Texto com {text.Length} caracteres e {wordCount} palavras", Significance = "strong_human" }
                    }
                };
            }

            return null;
        }

        private async Task<AIAnalysisResult> AnalyzeText(string text)
        {
            var shortCircuit = TryShortCircuitTrivialText(text);
            if (shortCircuit != null) return shortCircuit;

            var statisticalIndicators = TextStatisticsAnalyzer.Analyze(text);
            var result = await geminiAnalyzer.AnalyzeTextAsync(text);

            if (result.IsSuccessful && statisticalIndicators.Count > 0)
            {
                result.Indicators.AddRange(statisticalIndicators);
                ScoringEngine.ComputeScoreAndConclusion(result);
            }

            return result;
        }

        //Analisa uma imagem: extrai metadados categorizados e envia imagem + metadados ao Gemini
        private async Task<AIAnalysisResult> AnalyzeImage(string mimeType)
        {
            var metadata = metadataExtractor.Extract(inputContent);
            var metadataIndicators = ImageMetadataAnalyzer.Analyze(metadata);

            string categorizedSummary = FormatCategorizedMetadata(metadata);
            byte[] imageBytes = await File.ReadAllBytesAsync(inputContent);

            var result = await geminiAnalyzer.AnalyzeImageAsync(categorizedSummary, imageBytes, mimeType);

            if (result.IsSuccessful && metadataIndicators.Count > 0)
            {
                result.Indicators.AddRange(metadataIndicators);
                ScoringEngine.ComputeScoreAndConclusion(result);
            }

            return result;
        }

        private static readonly string[] AIPdfProducers =
        {
            "chatgpt", "claude", "gpt-4", "gpt-3", "gpt4", "gemini",
            "jasper", "copy.ai", "writesonic", "openai",
        };

        //Analisa um PDF: extrai texto e imagens e envia ao Gemini
        private async Task<AIAnalysisResult> AnalyzePdf()
        {
            var pdfExtractor = new PdfExtractorService();
            var pdfContent = pdfExtractor.Extract(inputContent);

            // Deterministic PDF metadata check
            var pdfMetaIndicators = CheckPdfMetadataForAI(pdfContent);

            // Also run text statistics on extracted text
            var textStats = !string.IsNullOrWhiteSpace(pdfContent.ExtractedText)
                ? TextStatisticsAnalyzer.Analyze(pdfContent.ExtractedText)
                : new List<AnalysisIndicator>();

            var result = await geminiAnalyzer.AnalyzePdfAsync(pdfContent);

            if (result.IsSuccessful)
            {
                if (pdfMetaIndicators.Count > 0)
                    result.Indicators.AddRange(pdfMetaIndicators);
                if (textStats.Count > 0)
                    result.Indicators.AddRange(textStats);
                if (pdfMetaIndicators.Count > 0 || textStats.Count > 0)
                    ScoringEngine.ComputeScoreAndConclusion(result);
            }

            return result;
        }

        private static List<AnalysisIndicator> CheckPdfMetadataForAI(PdfContent pdfContent)
        {
            var indicators = new List<AnalysisIndicator>();

            string producerLower = pdfContent.Producer.ToLowerInvariant();
            string creatorLower = pdfContent.Creator.ToLowerInvariant();

            foreach (var aiTool in AIPdfProducers)
            {
                if (producerLower.Contains(aiTool) || creatorLower.Contains(aiTool))
                {
                    indicators.Add(new AnalysisIndicator
                    {
                        Name = "Ferramenta de IA detectada nos metadados do PDF",
                        Finding = $"Producer: \"{pdfContent.Producer}\", Creator: \"{pdfContent.Creator}\" — contem referencia a ferramenta de IA",
                        Significance = "strong_ai"
                    });
                    break;
                }
            }

            return indicators;
        }

        //Analisa um video: extrai metadados e envia video + resumo de metadados ao Gemini
        private async Task<AIAnalysisResult> AnalyzeVideo(string mimeType)
        {
            var metadata = metadataExtractor.Extract(inputContent);
            var metadataIndicators = VideoMetadataAnalyzer.Analyze(metadata);
            string metadataSummary = FormatMediaMetadata(metadata, "video");

            byte[] videoBytes = await File.ReadAllBytesAsync(inputContent);
            var result = await geminiAnalyzer.AnalyzeVideoAsync(metadataSummary, videoBytes, mimeType);

            if (result.IsSuccessful && metadataIndicators.Count > 0)
            {
                result.Indicators.AddRange(metadataIndicators);
                ScoringEngine.ComputeScoreAndConclusion(result);
            }

            return result;
        }

        //Analisa um audio: extrai metadados e envia audio + resumo de metadados ao Gemini
        private async Task<AIAnalysisResult> AnalyzeAudio(string mimeType)
        {
            var metadata = metadataExtractor.Extract(inputContent);
            var metadataIndicators = AudioMetadataAnalyzer.Analyze(metadata);
            string metadataSummary = FormatMediaMetadata(metadata, "audio");

            byte[] audioBytes = await File.ReadAllBytesAsync(inputContent);
            var result = await geminiAnalyzer.AnalyzeAudioAsync(metadataSummary, audioBytes, mimeType);

            if (result.IsSuccessful && metadataIndicators.Count > 0)
            {
                result.Indicators.AddRange(metadataIndicators);
                ScoringEngine.ComputeScoreAndConclusion(result);
            }

            return result;
        }

        //Formata metadados de video/audio para envio ao Gemini
        private static string FormatMediaMetadata(MetadataInfo metadata, string mediaType)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"== METADADOS DO {mediaType.ToUpperInvariant()} ==");

            if (!metadata.MetadataAvailable)
            {
                sb.AppendLine("Nenhum metadado disponivel.");
                return sb.ToString();
            }

            var tags = metadata.Tags;

            // Software/Encoder
            var softwareTags = FindTags(tags, "Software", "Creator", "Encoder", "Handler", "Compressor", "Tool");
            sb.AppendLine(softwareTags.Count > 0
                ? $"Software/Encoder: [Presente] - {FormatTagValues(softwareTags)}"
                : "Software/Encoder: [Ausente]");

            // Camera/Device
            var cameraTags = FindTags(tags, "Make", "Model", "Camera", "Device");
            sb.AppendLine(cameraTags.Count > 0
                ? $"Dispositivo: [Presente] - {FormatTagValues(cameraTags)}"
                : "Dispositivo: [Ausente]");

            // GPS
            var gpsTags = FindTags(tags, "GPS", "Latitude", "Longitude");
            sb.AppendLine(gpsTags.Count > 0
                ? $"GPS: [Presente] - {FormatTagValues(gpsTags)}"
                : "GPS: [Ausente]");

            // Resolution/Dimensions (video)
            var resTags = FindTags(tags, "Width", "Height", "Resolution", "Frame");
            if (resTags.Count > 0)
                sb.AppendLine($"Resolucao/Frames: [Presente] - {FormatTagValues(resTags)}");

            // Duration
            var durationTags = FindTags(tags, "Duration");
            if (durationTags.Count > 0)
                sb.AppendLine($"Duracao: {FormatTagValues(durationTags)}");

            // Audio info
            var audioTags = FindTags(tags, "Audio", "Sample Rate", "Channels", "Bitrate", "Bit Rate", "Codec");
            if (audioTags.Count > 0)
                sb.AppendLine($"Audio: [Presente] - {FormatTagValues(audioTags)}");

            // Dates
            var dateTags = FindTags(tags, "Date", "DateTime", "Creation Time");
            if (dateTags.Count > 0)
                sb.AppendLine($"Datas: {FormatTagValues(dateTags)}");

            // ID3/Music tags (audio)
            var musicTags = FindTags(tags, "Artist", "Album", "Title", "Genre", "Year", "Performer", "Comment");
            if (musicTags.Count > 0)
                sb.AppendLine($"Tags de musica: {FormatTagValues(musicTags)}");

            sb.AppendLine($"Total de tags encontradas: {tags.Count}");

            return sb.ToString();
        }

        //Formata os metadados em categorias relevantes para a análise de IA
        private static string FormatCategorizedMetadata(MetadataInfo metadata)
        {
            var sb = new StringBuilder();
            sb.AppendLine("== ANALISE DE METADADOS ==");

            if (!metadata.MetadataAvailable)
            {
                sb.AppendLine("Nenhum metadado disponivel.");
                if (!string.IsNullOrEmpty(metadata.MetadataError))
                    sb.AppendLine($"Motivo: {metadata.MetadataError}");
                sb.AppendLine($"Extensao do arquivo: {Path.GetExtension(metadata.InputContent)} (NOTA: nomes/extensoes de arquivo NAO sao indicadores confiaveis)");
                return sb.ToString();
            }

            var tags = metadata.Tags;

            // Camera Make/Model
            var cameraTags = FindTags(tags, "Make", "Model", "Camera");
            sb.AppendLine(cameraTags.Count > 0
                ? $"Camera: [Presente] - {FormatTagValues(cameraTags)}"
                : "Camera: [Ausente]");

            // GPS
            var gpsTags = FindTags(tags, "GPS", "Latitude", "Longitude");
            sb.AppendLine(gpsTags.Count > 0
                ? $"GPS: [Presente] - {FormatTagValues(gpsTags)}"
                : "GPS: [Ausente]");

            // Lens
            var lensTags = FindTags(tags, "Lens", "Focal Length", "Aperture", "F-Number");
            sb.AppendLine(lensTags.Count > 0
                ? $"Lente: [Presente] - {FormatTagValues(lensTags)}"
                : "Lente: [Ausente]");

            // Exposure
            var exposureTags = FindTags(tags, "Exposure", "ISO", "Shutter", "Metering", "White Balance");
            sb.AppendLine(exposureTags.Count > 0
                ? $"Exposicao: [Presente] - {FormatTagValues(exposureTags)}"
                : "Exposicao: [Ausente]");

            // Software
            var softwareTags = FindTags(tags, "Software", "Creator", "CreatorTool", "Processing Software");
            sb.AppendLine(softwareTags.Count > 0
                ? $"Software: [Presente] - {FormatTagValues(softwareTags)}"
                : "Software: [Ausente]");

            // Color Profile
            var colorTags = FindTags(tags, "Color Space", "ICC Profile", "Color Profile", "Color Mode");
            sb.AppendLine(colorTags.Count > 0
                ? $"Perfil de Cor: [Presente] - {FormatTagValues(colorTags)}"
                : "Perfil de Cor: [Ausente]");

            // Resolution
            var resTags = FindTags(tags, "Image Width", "Image Height", "Pixel", "Resolution");
            sb.AppendLine(resTags.Count > 0
                ? $"Resolucao: [Presente] - {FormatTagValues(resTags)}"
                : "Resolucao: [Ausente]");

            // Creation Date
            var dateTags = FindTags(tags, "Date", "DateTime", "Date/Time");
            sb.AppendLine(dateTags.Count > 0
                ? $"Data de Criacao: [Presente] - {FormatTagValues(dateTags)}"
                : "Data de Criacao: [Ausente]");

            // EXIF Thumbnail
            var thumbTags = FindTags(tags, "Thumbnail");
            sb.AppendLine(thumbTags.Count > 0
                ? $"Thumbnail EXIF: [Presente]"
                : "Thumbnail EXIF: [Ausente]");

            // XMP/IPTC
            var xmpTags = FindTags(tags, "XMP", "IPTC", "History");
            sb.AppendLine(xmpTags.Count > 0
                ? $"XMP/IPTC: [Presente] - {FormatTagValues(xmpTags)}"
                : "XMP/IPTC: [Ausente]");

            sb.AppendLine($"Extensao do arquivo: {Path.GetExtension(metadata.InputContent)} (NOTA: nomes/extensoes de arquivo NAO sao indicadores confiaveis)");
            sb.AppendLine($"Total de tags de metadados encontradas: {tags.Count}");

            return sb.ToString();
        }

        //Busca tags que contenham qualquer uma das palavras-chave no nome
        private static Dictionary<string, string> FindTags(Dictionary<string, string> allTags, params string[] keywords)
        {
            return allTags
                .Where(t => keywords.Any(k => t.Key.Contains(k, StringComparison.OrdinalIgnoreCase)))
                .ToDictionary(t => t.Key, t => t.Value);
        }

        //Formata pares chave-valor de tags para exibição
        private static string FormatTagValues(Dictionary<string, string> tags)
        {
            return string.Join(", ", tags.Select(t => $"{t.Key}: {t.Value}"));
        }
    }
}
