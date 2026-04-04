namespace Verasium.Core
{
    public static class ImageMetadataAnalyzer
    {
        private static readonly HashSet<(int Width, int Height)> KnownAIResolutions = new()
        {
            (512, 512), (768, 768), (1024, 1024),
            (1024, 1792), (1792, 1024),
            (896, 1152), (1152, 896),
            (768, 1344), (1344, 768),
            (1536, 1024), (1024, 1536),
            (1024, 1820), (1820, 1024),
            (2048, 2048),
        };

        private static readonly string[] AIToolNames =
        {
            "dall-e", "dall·e", "midjourney", "stable diffusion",
            "adobe firefly", "bing image creator", "leonardo.ai", "leonardo ai",
            "comfyui", "craiyon", "novelai", "flux",
            "kandinsky", "automatic1111", "invokeai", "invoke ai",
            "dreamstudio", "nightcafe", "playground ai", "ideogram",
            "copilot designer", "image creator from designer",
        };

        public static List<AnalysisIndicator> Analyze(MetadataInfo metadata)
        {
            var indicators = new List<AnalysisIndicator>();
            if (!metadata.MetadataAvailable) return indicators;

            var resolution = CheckResolution(metadata.Tags);
            if (resolution != null) indicators.Add(resolution);

            var noExif = CheckMissingExif(metadata.Tags);
            if (noExif != null) indicators.Add(noExif);

            var fullCamera = CheckFullCameraData(metadata.Tags);
            if (fullCamera != null) indicators.Add(fullCamera);

            var aiSoftware = CheckAISoftware(metadata.Tags);
            if (aiSoftware != null) indicators.Add(aiSoftware);

            return indicators;
        }

        private static bool TagsContainAny(Dictionary<string, string> tags, params string[] keywords)
        {
            return tags.Any(t => keywords.Any(k =>
                t.Key.Contains(k, StringComparison.OrdinalIgnoreCase)));
        }

        private static (int Width, int Height)? ParseResolution(Dictionary<string, string> tags)
        {
            int width = 0, height = 0;

            foreach (var tag in tags)
            {
                string keyLower = tag.Key.ToLowerInvariant();
                if (keyLower.Contains("image width") || keyLower.Contains("pixel x"))
                {
                    // Extract first number from value like "1024 pixels" or "1024"
                    var match = System.Text.RegularExpressions.Regex.Match(tag.Value, @"\d+");
                    if (match.Success) width = int.Parse(match.Value);
                }
                else if (keyLower.Contains("image height") || keyLower.Contains("pixel y"))
                {
                    var match = System.Text.RegularExpressions.Regex.Match(tag.Value, @"\d+");
                    if (match.Success) height = int.Parse(match.Value);
                }
            }

            if (width > 0 && height > 0) return (width, height);
            return null;
        }

        private static AnalysisIndicator? CheckResolution(Dictionary<string, string> tags)
        {
            var res = ParseResolution(tags);
            if (res == null) return null;

            var (width, height) = res.Value;

            if (KnownAIResolutions.Contains((width, height)))
            {
                return new AnalysisIndicator
                {
                    Name = "Resolucao conhecida de IA",
                    Finding = $"Resolucao {width}x{height} e uma resolucao tipica de geradores de imagem por IA",
                    Significance = "strong_ai"
                };
            }

            if (width % 64 == 0 && height % 64 == 0 && width != height)
            {
                // Square images with common sizes (e.g. 1000x1000) can be human crops
                return new AnalysisIndicator
                {
                    Name = "Resolucao multipla de 64",
                    Finding = $"Resolucao {width}x{height} — ambas dimensoes sao multiplas de 64, comum em modelos de difusao",
                    Significance = "weak_ai"
                };
            }

            if (width % 64 == 0 && height % 64 == 0 && width == height)
            {
                return new AnalysisIndicator
                {
                    Name = "Resolucao quadrada multipla de 64",
                    Finding = $"Resolucao {width}x{height} — quadrada e multipla de 64, padrao frequente em geradores de IA",
                    Significance = "weak_ai"
                };
            }

            return null;
        }

        private static AnalysisIndicator? CheckMissingExif(Dictionary<string, string> tags)
        {
            if (tags.Count < 5)
            {
                return new AnalysisIndicator
                {
                    Name = "Metadados EXIF ausentes ou minimos",
                    Finding = $"Apenas {tags.Count} tags de metadados encontradas — imagens de cameras reais possuem dezenas de tags",
                    Significance = "weak_ai"
                };
            }

            return null;
        }

        private static AnalysisIndicator? CheckFullCameraData(Dictionary<string, string> tags)
        {
            bool hasCamera = TagsContainAny(tags, "Make", "Model", "Camera");
            bool hasGPS = TagsContainAny(tags, "GPS", "Latitude", "Longitude");
            bool hasExposure = TagsContainAny(tags, "Exposure", "ISO", "Shutter");

            if (hasCamera && hasGPS && hasExposure)
            {
                return new AnalysisIndicator
                {
                    Name = "Metadados completos de camera real",
                    Finding = "Imagem possui dados de camera, GPS e exposicao — consistente com fotografia real",
                    Significance = "strong_human"
                };
            }

            if (hasCamera && hasExposure)
            {
                return new AnalysisIndicator
                {
                    Name = "Metadados de camera presentes",
                    Finding = "Imagem possui dados de camera e exposicao — consistente com fotografia real (sem GPS)",
                    Significance = "weak_human"
                };
            }

            return null;
        }

        private static AnalysisIndicator? CheckAISoftware(Dictionary<string, string> tags)
        {
            var softwareTags = tags
                .Where(t => t.Key.Contains("Software", StringComparison.OrdinalIgnoreCase)
                         || t.Key.Contains("Creator", StringComparison.OrdinalIgnoreCase)
                         || t.Key.Contains("CreatorTool", StringComparison.OrdinalIgnoreCase))
                .ToList();

            foreach (var tag in softwareTags)
            {
                string valueLower = tag.Value.ToLowerInvariant();
                foreach (var aiTool in AIToolNames)
                {
                    if (valueLower.Contains(aiTool))
                    {
                        return new AnalysisIndicator
                        {
                            Name = "Software de IA detectado nos metadados",
                            Finding = $"Campo \"{tag.Key}\" contem \"{tag.Value}\" — ferramenta de geracao de imagem por IA",
                            Significance = "strong_ai"
                        };
                    }
                }
            }

            return null;
        }
    }
}
