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

        [HttpPost("upload")]
        public async Task<IActionResult> AnalyzeFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { error = "Nenhum arquivo enviado." });

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
