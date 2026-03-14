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
            // Se é um arquivo que existe no disco
            if (File.Exists(inputContent))
            {
                string extension = Path.GetExtension(inputContent);

                // Se é uma imagem, faz análise multimodal (visual + metadados)
                if (ImageMimeTypes.TryGetValue(extension, out string? mimeType))
                {
                    return await AnalyzeImage(mimeType);
                }

                // Se é um arquivo de texto, lê o conteúdo e faz análise textual
                string fileContent = await File.ReadAllTextAsync(inputContent);
                return await geminiAnalyzer.AnalyzeTextAsync(fileContent);
            }

            // Se não é arquivo, trata como texto direto
            return await geminiAnalyzer.AnalyzeTextAsync(inputContent);
        }

        //Analisa uma imagem: extrai metadados categorizados e envia imagem + metadados ao Gemini
        private async Task<AIAnalysisResult> AnalyzeImage(string mimeType)
        {
            var metadata = metadataExtractor.Extract(inputContent);
            string categorizedSummary = FormatCategorizedMetadata(metadata);
            byte[] imageBytes = await File.ReadAllBytesAsync(inputContent);

            return await geminiAnalyzer.AnalyzeImageAsync(categorizedSummary, imageBytes, mimeType);
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
