using System.Linq;

namespace Verasium.Core
{
    //Essa classe é responsável por orquestrar a análise e devolver o resultado
    public class FileAnalyzer
    {
        private readonly string inputContent;
        private readonly GeminiAnalyzer geminiAnalyzer;
        private readonly MetadataExtractorService metadataExtractor;

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
            var metadata = metadataExtractor.Extract(inputContent);
            string contentSummary = FormatSummary(metadata);
            return await geminiAnalyzer.AnalyzeAsync(contentSummary);
        }

        //Formata os metadados/inputContent para enviar para a IA
        private static string FormatSummary(MetadataInfo metadata)
        {
            string tagsFormatted = string.Join(", ",
                metadata.Tags.Select(t => $"{t.Key}: {t.Value}"));

            return $"\nInputContent = {metadata.InputContent}" +
                   $"\nTags = {tagsFormatted}" +
                   $"\nMetadataError = {metadata.MetadataError}" +
                   $"\nMetadataAvailable = {metadata.MetadataAvailable}";
        }
    }
}
