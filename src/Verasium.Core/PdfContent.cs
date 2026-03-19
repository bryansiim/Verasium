namespace Verasium.Core
{
    public class PdfContent
    {
        public string ExtractedText { get; set; } = "";
        public List<PdfImage> Images { get; set; } = new();
        public int PageCount { get; set; }
    }

    public class PdfImage
    {
        public byte[] Bytes { get; set; } = Array.Empty<byte>();
        public string MimeType { get; set; } = "image/png";
    }
}
