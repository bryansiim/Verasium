using System.Text;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;

namespace Verasium.Core
{
    public class PdfExtractorService
    {
        public PdfContent Extract(string filePath)
        {
            var result = new PdfContent();

            try
            {
                using var document = PdfDocument.Open(filePath);
                result.PageCount = document.NumberOfPages;

                // Extract PDF document metadata
                if (document.Information != null)
                {
                    result.Producer = document.Information.Producer ?? "";
                    result.Creator = document.Information.Creator ?? "";
                    result.Author = document.Information.Author ?? "";
                }

                var textBuilder = new StringBuilder();

                foreach (var page in document.GetPages())
                {
                    // Extrair texto
                    string pageText = page.Text;
                    if (!string.IsNullOrWhiteSpace(pageText))
                    {
                        textBuilder.AppendLine($"--- Pagina {page.Number} ---");
                        textBuilder.AppendLine(pageText);
                        textBuilder.AppendLine();
                    }

                    // Extrair imagens (ate 5 no total para limitar custo)
                    if (result.Images.Count < 5)
                    {
                        foreach (var image in page.GetImages())
                        {
                            if (result.Images.Count >= 5) break;

                            if (image.TryGetPng(out var pngBytes))
                            {
                                result.Images.Add(new PdfImage
                                {
                                    Bytes = pngBytes,
                                    MimeType = "image/png"
                                });
                            }
                            else if (image.RawBytes.Count > 0)
                            {
                                result.Images.Add(new PdfImage
                                {
                                    Bytes = image.RawBytes.ToArray(),
                                    MimeType = "image/jpeg"
                                });
                            }
                        }
                    }
                }

                result.ExtractedText = textBuilder.ToString().Trim();
            }
            catch (Exception ex)
            {
                result.ExtractedText = $"[Erro ao extrair conteudo do PDF: {ex.Message}]";
            }

            return result;
        }
    }
}
