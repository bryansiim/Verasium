namespace Verasium.Core
{
    public static class AudioMetadataAnalyzer
    {
        private static readonly string[] AIToolNames =
        {
            "elevenlabs", "eleven labs", "playht", "play.ht",
            "murf", "vall-e", "bark", "tortoise", "coqui",
            "suno", "udio", "musiclm", "stable audio",
            "aiva", "soundraw", "boomy", "amper",
            "synthesia", "resemble", "descript", "lovo",
            "wellsaid", "speechify", "listnr", "typecast",
            "fakeyou", "uberduck", "voicemod",
        };

        private static readonly string[] RealRecordingSoftware =
        {
            "audacity", "adobe audition", "pro tools", "logic pro",
            "garageband", "ableton", "fl studio", "cubase",
            "reaper", "studio one", "bitwig", "reason",
            "obs", "voice memos", "sound recorder",
            "zoom", "tascam", "rode", "focusrite",
        };

        public static List<AnalysisIndicator> Analyze(MetadataInfo metadata)
        {
            var indicators = new List<AnalysisIndicator>();
            if (!metadata.MetadataAvailable) return indicators;

            var aiSoftware = CheckAISoftware(metadata.Tags);
            if (aiSoftware != null) indicators.Add(aiSoftware);

            var realSoftware = CheckRealSoftware(metadata.Tags);
            if (realSoftware != null) indicators.Add(realSoftware);

            var sampleRate = CheckSampleRate(metadata.Tags);
            if (sampleRate != null) indicators.Add(sampleRate);

            var missingMetadata = CheckMissingMetadata(metadata.Tags);
            if (missingMetadata != null) indicators.Add(missingMetadata);

            var id3Tags = CheckID3Tags(metadata.Tags);
            if (id3Tags != null) indicators.Add(id3Tags);

            return indicators;
        }

        private static AnalysisIndicator? CheckAISoftware(Dictionary<string, string> tags)
        {
            var relevantTags = tags
                .Where(t => t.Key.Contains("Software", StringComparison.OrdinalIgnoreCase)
                         || t.Key.Contains("Encoder", StringComparison.OrdinalIgnoreCase)
                         || t.Key.Contains("Creator", StringComparison.OrdinalIgnoreCase)
                         || t.Key.Contains("Comment", StringComparison.OrdinalIgnoreCase)
                         || t.Key.Contains("Tool", StringComparison.OrdinalIgnoreCase)
                         || t.Key.Contains("Description", StringComparison.OrdinalIgnoreCase)
                         || t.Key.Contains("Handler", StringComparison.OrdinalIgnoreCase))
                .ToList();

            foreach (var tag in relevantTags)
            {
                string valueLower = tag.Value.ToLowerInvariant();
                foreach (var aiTool in AIToolNames)
                {
                    if (valueLower.Contains(aiTool))
                    {
                        return new AnalysisIndicator
                        {
                            Name = "Software de IA detectado nos metadados do audio",
                            Finding = $"Campo \"{tag.Key}\" contem \"{tag.Value}\" — ferramenta de geracao de audio/voz por IA",
                            Significance = "strong_ai"
                        };
                    }
                }
            }

            return null;
        }

        private static AnalysisIndicator? CheckRealSoftware(Dictionary<string, string> tags)
        {
            var relevantTags = tags
                .Where(t => t.Key.Contains("Software", StringComparison.OrdinalIgnoreCase)
                         || t.Key.Contains("Encoder", StringComparison.OrdinalIgnoreCase)
                         || t.Key.Contains("Creator", StringComparison.OrdinalIgnoreCase)
                         || t.Key.Contains("Tool", StringComparison.OrdinalIgnoreCase))
                .ToList();

            foreach (var tag in relevantTags)
            {
                string valueLower = tag.Value.ToLowerInvariant();
                foreach (var sw in RealRecordingSoftware)
                {
                    if (valueLower.Contains(sw))
                    {
                        return new AnalysisIndicator
                        {
                            Name = "Software de gravacao/edicao real detectado",
                            Finding = $"Campo \"{tag.Key}\" contem \"{tag.Value}\" — software de gravacao ou producao de audio real",
                            Significance = "weak_human"
                        };
                    }
                }
            }

            return null;
        }

        private static AnalysisIndicator? CheckSampleRate(Dictionary<string, string> tags)
        {
            var sampleTag = tags.FirstOrDefault(t =>
                t.Key.Contains("Sample Rate", StringComparison.OrdinalIgnoreCase)
                || t.Key.Contains("SampleRate", StringComparison.OrdinalIgnoreCase)
                || t.Key.Contains("Sampling Rate", StringComparison.OrdinalIgnoreCase));

            if (sampleTag.Value == null) return null;

            var match = System.Text.RegularExpressions.Regex.Match(sampleTag.Value, @"(\d+)");
            if (!match.Success) return null;

            int rate = int.Parse(match.Value);

            // AI TTS tools commonly output at 22050 or 24000 Hz
            if (rate == 22050 || rate == 24000)
            {
                return new AnalysisIndicator
                {
                    Name = "Sample rate tipico de TTS/IA",
                    Finding = $"Sample rate de {rate} Hz — valor comum em ferramentas de sintese de voz por IA (22050/24000 Hz)",
                    Significance = "weak_ai"
                };
            }

            return null;
        }

        private static AnalysisIndicator? CheckMissingMetadata(Dictionary<string, string> tags)
        {
            // Audio from real recordings (phones, recorders) typically have rich metadata
            // AI-generated audio typically has minimal metadata
            if (tags.Count < 4)
            {
                return new AnalysisIndicator
                {
                    Name = "Metadados minimos no audio",
                    Finding = $"Apenas {tags.Count} tags de metadados — audios de gravacoes reais geralmente possuem mais metadados",
                    Significance = "weak_ai"
                };
            }

            return null;
        }

        private static AnalysisIndicator? CheckID3Tags(Dictionary<string, string> tags)
        {
            // Check for rich ID3/metadata that indicates real music production
            bool hasArtist = tags.Any(t =>
                t.Key.Contains("Artist", StringComparison.OrdinalIgnoreCase)
                || t.Key.Contains("Performer", StringComparison.OrdinalIgnoreCase));

            bool hasAlbum = tags.Any(t =>
                t.Key.Contains("Album", StringComparison.OrdinalIgnoreCase));

            bool hasYear = tags.Any(t =>
                t.Key.Contains("Year", StringComparison.OrdinalIgnoreCase)
                || t.Key.Contains("Date", StringComparison.OrdinalIgnoreCase));

            bool hasGenre = tags.Any(t =>
                t.Key.Contains("Genre", StringComparison.OrdinalIgnoreCase));

            int richCount = (hasArtist ? 1 : 0) + (hasAlbum ? 1 : 0) + (hasYear ? 1 : 0) + (hasGenre ? 1 : 0);

            if (richCount >= 3)
            {
                return new AnalysisIndicator
                {
                    Name = "Metadados ID3/tags de musica completos",
                    Finding = "Audio possui artista, album, ano e/ou genero — consistente com producao musical real ou gravacao catalogada",
                    Significance = "weak_human"
                };
            }

            return null;
        }
    }
}
