using Microsoft.AspNetCore.Mvc;
using Verasium.Api.Models;
using Verasium.Core;

namespace Verasium.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AnalysisController : ControllerBase
    {
        private readonly GeminiAnalyzer geminiAnalyzer;

        public AnalysisController(GeminiAnalyzer geminiAnalyzer)
        {
            this.geminiAnalyzer = geminiAnalyzer;
        }

        [HttpPost]
        public async Task<IActionResult> Analyze([FromBody] AnalysisRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Content))
                return BadRequest(new { error = "Conteúdo não pode ser vazio." });

            var analyzer = new FileAnalyzer(request.Content, geminiAnalyzer);
            var result = await analyzer.RunAnalysis();

            return Ok(result);
        }

        private static readonly Dictionary<string, long> MaxFileSizes = new(StringComparer.OrdinalIgnoreCase)
        {
            { ".mp4", 25_000_000 },
            { ".mpeg", 25_000_000 },
            { ".mov", 25_000_000 },
            { ".avi", 25_000_000 },
            { ".webm", 25_000_000 },
            { ".mkv", 25_000_000 },
            { ".mp3", 10_000_000 },
            { ".wav", 10_000_000 },
            { ".aac", 10_000_000 },
            { ".ogg", 10_000_000 },
            { ".flac", 10_000_000 },
            { ".m4a", 10_000_000 },
            { ".pdf", 20_000_000 }
        };

        [HttpPost("upload")]
        [RequestSizeLimit(30_000_000)]
        public async Task<IActionResult> AnalyzeFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { error = "Nenhum arquivo enviado." });

            string extension = Path.GetExtension(file.FileName);
            if (MaxFileSizes.TryGetValue(extension, out long maxSize) && file.Length > maxSize)
            {
                string maxMb = (maxSize / 1_000_000).ToString();
                return BadRequest(new { error = $"Arquivo muito grande. Limite para este tipo: {maxMb}MB." });
            }

            var tempDir = Path.Combine(Path.GetTempPath(), "Verasium");
            Directory.CreateDirectory(tempDir);

            var tempPath = Path.Combine(tempDir, file.FileName);

            try
            {
                using (var stream = new FileStream(tempPath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var analyzer = new FileAnalyzer(tempPath, geminiAnalyzer);
                var result = await analyzer.RunAnalysis();

                return Ok(result);
            }
            finally
            {
                if (System.IO.File.Exists(tempPath))
                    System.IO.File.Delete(tempPath);
            }
        }
    }
}
