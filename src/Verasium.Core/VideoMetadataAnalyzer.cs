namespace Verasium.Core
{
    public static class VideoMetadataAnalyzer
    {
        private static readonly string[] AIToolNames =
        {
            "sora", "runway", "pika", "kling", "luma",
            "stable video", "stable diffusion", "hailuoai", "minimax",
            "veo", "synthesia", "d-id", "heygen", "colossyan",
            "deepbrain", "elai", "invideo ai", "fliki",
            "genmo", "kaiber", "morph studio", "pixverse",
            "dreamina",
        };

        // Known AI video resolutions (common defaults from generators)
        private static readonly HashSet<(int Width, int Height)> KnownAIResolutions = new()
        {
            (512, 512), (768, 768), (1024, 1024),
            (1024, 576), (576, 1024),   // 16:9 and 9:16 at low res
            (1280, 720),                 // While also common for real video, suspicious without other metadata
            (768, 1344), (1344, 768),
            (896, 1152), (1152, 896),
        };

        public static List<AnalysisIndicator> Analyze(MetadataInfo metadata)
        {
            var indicators = new List<AnalysisIndicator>();
            if (!metadata.MetadataAvailable) return indicators;

            var aiSoftware = CheckAISoftware(metadata.Tags);
            if (aiSoftware != null) indicators.Add(aiSoftware);

            var encoder = CheckEncoder(metadata.Tags);
            if (encoder != null) indicators.Add(encoder);

            var resolution = CheckResolution(metadata.Tags);
            if (resolution != null) indicators.Add(resolution);

            var cameraData = CheckCameraData(metadata.Tags);
            if (cameraData != null) indicators.Add(cameraData);

            var duration = CheckDuration(metadata.Tags);
            if (duration != null) indicators.Add(duration);

            var audioTracks = CheckAudioTracks(metadata.Tags);
            if (audioTracks != null) indicators.Add(audioTracks);

            return indicators;
        }

        private static AnalysisIndicator? CheckAISoftware(Dictionary<string, string> tags)
        {
            var softwareTags = tags
                .Where(t => t.Key.Contains("Software", StringComparison.OrdinalIgnoreCase)
                         || t.Key.Contains("Creator", StringComparison.OrdinalIgnoreCase)
                         || t.Key.Contains("Encoder", StringComparison.OrdinalIgnoreCase)
                         || t.Key.Contains("Handler", StringComparison.OrdinalIgnoreCase)
                         || t.Key.Contains("Comment", StringComparison.OrdinalIgnoreCase)
                         || t.Key.Contains("Description", StringComparison.OrdinalIgnoreCase))
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
                            Name = "Software de IA detectado nos metadados do video",
                            Finding = $"Campo \"{tag.Key}\" contem \"{tag.Value}\" — ferramenta de geracao de video por IA",
                            Significance = "strong_ai"
                        };
                    }
                }
            }

            return null;
        }

        private static AnalysisIndicator? CheckEncoder(Dictionary<string, string> tags)
        {
            // Look for real camera/editing software encoders
            var encoderTags = tags
                .Where(t => t.Key.Contains("Encoder", StringComparison.OrdinalIgnoreCase)
                         || t.Key.Contains("Software", StringComparison.OrdinalIgnoreCase)
                         || t.Key.Contains("Compressor", StringComparison.OrdinalIgnoreCase))
                .ToList();

            string[] realEncoders = {
                "ffmpeg", "handbrake", "adobe premiere", "davinci resolve",
                "final cut", "avid", "sony vegas", "filmora",
                "obs", "x264", "x265", "libx264", "libx265",
                "apple", "canon", "nikon", "sony", "gopro", "dji",
            };

            foreach (var tag in encoderTags)
            {
                string valueLower = tag.Value.ToLowerInvariant();
                foreach (var encoder in realEncoders)
                {
                    if (valueLower.Contains(encoder))
                    {
                        return new AnalysisIndicator
                        {
                            Name = "Encoder/software de edicao real detectado",
                            Finding = $"Campo \"{tag.Key}\" contem \"{tag.Value}\" — software de captura ou edicao de video real",
                            Significance = "weak_human"
                        };
                    }
                }
            }

            return null;
        }

        private static AnalysisIndicator? CheckResolution(Dictionary<string, string> tags)
        {
            int width = 0, height = 0;

            foreach (var tag in tags)
            {
                string keyLower = tag.Key.ToLowerInvariant();
                if (keyLower.Contains("width") && !keyLower.Contains("display"))
                {
                    var match = System.Text.RegularExpressions.Regex.Match(tag.Value, @"\d+");
                    if (match.Success) width = int.Parse(match.Value);
                }
                else if (keyLower.Contains("height") && !keyLower.Contains("display"))
                {
                    var match = System.Text.RegularExpressions.Regex.Match(tag.Value, @"\d+");
                    if (match.Success) height = int.Parse(match.Value);
                }
            }

            if (width <= 0 || height <= 0) return null;

            if (KnownAIResolutions.Contains((width, height)))
            {
                return new AnalysisIndicator
                {
                    Name = "Resolucao tipica de video IA",
                    Finding = $"Resolucao {width}x{height} e comum em geradores de video por IA",
                    Significance = "weak_ai"
                };
            }

            return null;
        }

        private static AnalysisIndicator? CheckCameraData(Dictionary<string, string> tags)
        {
            bool hasCamera = tags.Any(t =>
                t.Key.Contains("Make", StringComparison.OrdinalIgnoreCase)
                || t.Key.Contains("Camera", StringComparison.OrdinalIgnoreCase));

            bool hasGPS = tags.Any(t =>
                t.Key.Contains("GPS", StringComparison.OrdinalIgnoreCase)
                || t.Key.Contains("Latitude", StringComparison.OrdinalIgnoreCase)
                || t.Key.Contains("Longitude", StringComparison.OrdinalIgnoreCase));

            if (hasCamera && hasGPS)
            {
                return new AnalysisIndicator
                {
                    Name = "Metadados de camera e GPS presentes no video",
                    Finding = "Video possui dados de camera e localizacao — consistente com gravacao real",
                    Significance = "strong_human"
                };
            }

            if (hasCamera)
            {
                return new AnalysisIndicator
                {
                    Name = "Metadados de camera presentes no video",
                    Finding = "Video possui dados de dispositivo de captura — consistente com gravacao real",
                    Significance = "weak_human"
                };
            }

            return null;
        }

        private static AnalysisIndicator? CheckDuration(Dictionary<string, string> tags)
        {
            // AI-generated videos are typically very short (< 10-15 seconds)
            var durationTag = tags.FirstOrDefault(t =>
                t.Key.Contains("Duration", StringComparison.OrdinalIgnoreCase));

            if (durationTag.Value == null) return null;

            // Try to parse duration - formats vary: "00:00:05", "5.2s", "5200 ms", etc.
            string val = durationTag.Value.ToLowerInvariant();
            double seconds = 0;

            var msMatch = System.Text.RegularExpressions.Regex.Match(val, @"(\d+)\s*ms");
            if (msMatch.Success)
            {
                seconds = double.Parse(msMatch.Groups[1].Value) / 1000.0;
            }
            else
            {
                // Try HH:MM:SS or MM:SS format
                var timeMatch = System.Text.RegularExpressions.Regex.Match(val, @"(\d+):(\d+):(\d+)");
                if (timeMatch.Success)
                {
                    seconds = int.Parse(timeMatch.Groups[1].Value) * 3600
                            + int.Parse(timeMatch.Groups[2].Value) * 60
                            + int.Parse(timeMatch.Groups[3].Value);
                }
                else
                {
                    var secMatch = System.Text.RegularExpressions.Regex.Match(val, @"([\d.]+)\s*s");
                    if (secMatch.Success)
                        seconds = double.Parse(secMatch.Groups[1].Value, System.Globalization.CultureInfo.InvariantCulture);
                }
            }

            if (seconds > 0 && seconds <= 10)
            {
                return new AnalysisIndicator
                {
                    Name = "Duracao muito curta do video",
                    Finding = $"Video com {seconds:F1} segundos — geradores de video IA tipicamente produzem clips de 3 a 10 segundos",
                    Significance = "weak_ai"
                };
            }

            return null;
        }

        private static AnalysisIndicator? CheckAudioTracks(Dictionary<string, string> tags)
        {
            // Many AI-generated videos have no audio track
            bool hasAudioInfo = tags.Any(t =>
                t.Key.Contains("Audio", StringComparison.OrdinalIgnoreCase)
                || t.Key.Contains("Sound", StringComparison.OrdinalIgnoreCase));

            // Only flag if we have video metadata but no audio metadata at all
            bool hasVideoInfo = tags.Any(t =>
                t.Key.Contains("Video", StringComparison.OrdinalIgnoreCase)
                || t.Key.Contains("Width", StringComparison.OrdinalIgnoreCase));

            if (hasVideoInfo && !hasAudioInfo)
            {
                return new AnalysisIndicator
                {
                    Name = "Video sem trilha de audio detectada",
                    Finding = "Nenhum metadado de audio encontrado — videos gerados por IA frequentemente nao possuem audio",
                    Significance = "weak_ai"
                };
            }

            return null;
        }
    }
}
