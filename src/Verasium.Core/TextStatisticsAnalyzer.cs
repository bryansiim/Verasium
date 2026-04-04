namespace Verasium.Core
{
    public static class TextStatisticsAnalyzer
    {
        private static readonly char[] SentenceSeparators = { '.', '!', '?', '\n' };

        private static readonly string[] AiPhrasesPortuguese =
        {
            "espero ter ajudado",
            "fico a disposicao",
            "fico à disposição",
            "com certeza!",
            "aqui esta",
            "aqui está",
            "excelente pergunta",
            "otima pergunta",
            "ótima pergunta",
            "boa pergunta",
            "e importante ressaltar",
            "é importante ressaltar",
            "vale destacar que",
            "vale ressaltar que",
            "nesse sentido",
            "em suma",
            "de modo geral",
            "no mundo de hoje",
            "no cenario atual",
            "no cenário atual",
            "com o avanco da tecnologia",
            "com o avanço da tecnologia",
            "diante disso",
            "sendo assim",
            "nesse contexto",
            "e importante notar",
            "é importante notar",
            "em conclusao",
            "em conclusão",
            "alem disso",
            "além disso",
            "vale a pena mencionar",
            "e fundamental",
            "é fundamental",
            "claro!",
            "vamos la!",
            "vamos lá!",
        };

        private static readonly string[] AiPhrasesEnglish =
        {
            "i hope this helps",
            "feel free to",
            "let me know if",
            "i'd be happy to",
            "here's a",
            "great question",
            "that's a great",
            "in conclusion",
            "it's worth noting",
            "it is worth noting",
            "furthermore",
            "moreover",
            "delve",
            "tapestry",
            "nuanced",
            "in today's world",
            "in the realm of",
            "navigating the",
            "a testament to",
            "plays a crucial role",
            "it's important to note",
        };

        private static readonly char[] UnicodeMarkers =
        {
            '\u2014', // em dash —
            '\u2013', // en dash –
            '\u201C', // left double curly quote "
            '\u201D', // right double curly quote "
            '\u2018', // left single curly quote '
            '\u2019', // right single curly quote '
            '\u2026', // horizontal ellipsis …
            '\u2022', // bullet •
        };

        public static List<AnalysisIndicator> Analyze(string text)
        {
            var indicators = new List<AnalysisIndicator>();
            if (string.IsNullOrWhiteSpace(text) || text.Length < 50) return indicators;

            var burstiness = AnalyzeBurstiness(text);
            if (burstiness != null) indicators.Add(burstiness);

            var unicode = AnalyzeUnicodeMarkers(text);
            if (unicode != null) indicators.Add(unicode);

            var ttr = AnalyzeTypeTokenRatio(text);
            if (ttr != null) indicators.Add(ttr);

            var aiPhrases = AnalyzeAIPhrases(text);
            if (aiPhrases != null) indicators.Add(aiPhrases);

            var cv = AnalyzeSentenceLengthVariation(text);
            if (cv != null) indicators.Add(cv);

            return indicators;
        }

        private static List<string> SplitIntoSentences(string text)
        {
            return text.Split(SentenceSeparators, StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.Trim())
                .Where(s => s.Length > 0)
                .ToList();
        }

        private static int WordCount(string sentence)
        {
            return sentence.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries).Length;
        }

        private static AnalysisIndicator? AnalyzeBurstiness(string text)
        {
            var sentences = SplitIntoSentences(text);
            if (sentences.Count < 4) return null;

            var lengths = sentences.Select(WordCount).ToList();
            double mean = lengths.Average();
            if (mean == 0) return null;

            double variance = lengths.Sum(l => Math.Pow(l - mean, 2)) / lengths.Count;
            double stdDev = Math.Sqrt(variance);

            if (stdDev < 2.0)
            {
                return new AnalysisIndicator
                {
                    Name = "Burstiness (variacao de comprimento de frases)",
                    Finding = $"Desvio padrao de {stdDev:F1} palavras por frase — uniformidade alta, tipica de IA",
                    Significance = "weak_ai"
                };
            }

            if (stdDev > 5.0)
            {
                return new AnalysisIndicator
                {
                    Name = "Burstiness (variacao de comprimento de frases)",
                    Finding = $"Desvio padrao de {stdDev:F1} palavras por frase — variacao alta, tipica de escrita humana",
                    Significance = "weak_human"
                };
            }

            return null;
        }

        private static AnalysisIndicator? AnalyzeUnicodeMarkers(string text)
        {
            int count = 0;
            var found = new List<string>();

            foreach (char marker in UnicodeMarkers)
            {
                int occurrences = text.Count(c => c == marker);
                if (occurrences > 0)
                {
                    count += occurrences;
                    string name = marker switch
                    {
                        '\u2014' => "em dash (—)",
                        '\u2013' => "en dash (–)",
                        '\u201C' or '\u201D' => "aspas curvas duplas",
                        '\u2018' or '\u2019' => "aspas curvas simples",
                        '\u2026' => "reticencias unicode (…)",
                        '\u2022' => "bullet (•)",
                        _ => $"U+{(int)marker:X4}"
                    };
                    if (!found.Contains(name))
                        found.Add(name);
                }
            }

            if (count == 0) return null;

            if (count >= 6)
            {
                return new AnalysisIndicator
                {
                    Name = "Marcadores Unicode tipicos de IA",
                    Finding = $"{count} ocorrencias encontradas: {string.Join(", ", found)}. Esses caracteres sao raros na digitacao humana normal em portugues",
                    Significance = "strong_ai"
                };
            }

            if (count >= 3)
            {
                return new AnalysisIndicator
                {
                    Name = "Marcadores Unicode tipicos de IA",
                    Finding = $"{count} ocorrencias encontradas: {string.Join(", ", found)}",
                    Significance = "weak_ai"
                };
            }

            // 1-2 ocorrencias: leve suspeita
            return new AnalysisIndicator
            {
                Name = "Marcadores Unicode tipicos de IA",
                Finding = $"{count} ocorrencia(s) encontrada(s): {string.Join(", ", found)}",
                Significance = "neutral"
            };
        }

        private static AnalysisIndicator? AnalyzeTypeTokenRatio(string text)
        {
            var words = text.ToLowerInvariant()
                .Split(new[] { ' ', '\t', '\n', '\r', ',', '.', ';', ':', '!', '?', '(', ')', '[', ']', '{', '}', '"', '\'' },
                    StringSplitOptions.RemoveEmptyEntries)
                .Where(w => w.Length > 1)
                .ToList();

            if (words.Count < 20) return null;

            double ttr = (double)words.Distinct().Count() / words.Count;

            if (ttr < 0.40)
            {
                return new AnalysisIndicator
                {
                    Name = "Type-Token Ratio (diversidade lexica)",
                    Finding = $"TTR de {ttr:F2} — vocabulario repetitivo, comum em textos de IA",
                    Significance = "weak_ai"
                };
            }

            if (ttr > 0.75)
            {
                return new AnalysisIndicator
                {
                    Name = "Type-Token Ratio (diversidade lexica)",
                    Finding = $"TTR de {ttr:F2} — vocabulario diverso, comum em escrita humana",
                    Significance = "weak_human"
                };
            }

            return null;
        }

        private static AnalysisIndicator? AnalyzeAIPhrases(string text)
        {
            string lower = text.ToLowerInvariant();
            var matchedPhrases = new List<string>();

            foreach (var phrase in AiPhrasesPortuguese)
            {
                if (lower.Contains(phrase))
                    matchedPhrases.Add(phrase);
            }

            foreach (var phrase in AiPhrasesEnglish)
            {
                if (lower.Contains(phrase))
                    matchedPhrases.Add(phrase);
            }

            if (matchedPhrases.Count == 0) return null;

            if (matchedPhrases.Count >= 3)
            {
                return new AnalysisIndicator
                {
                    Name = "Frases tipicas de IA detectadas",
                    Finding = $"{matchedPhrases.Count} frases encontradas: \"{string.Join("\", \"", matchedPhrases.Take(5))}\"",
                    Significance = "strong_ai"
                };
            }

            return new AnalysisIndicator
            {
                Name = "Frases tipicas de IA detectadas",
                Finding = $"{matchedPhrases.Count} frase(s) encontrada(s): \"{string.Join("\", \"", matchedPhrases)}\"",
                Significance = "weak_ai"
            };
        }

        private static AnalysisIndicator? AnalyzeSentenceLengthVariation(string text)
        {
            var sentences = SplitIntoSentences(text);
            if (sentences.Count < 5) return null;

            var lengths = sentences.Select(WordCount).ToList();
            double mean = lengths.Average();
            if (mean < 1.0) return null;

            double variance = lengths.Sum(l => Math.Pow(l - mean, 2)) / lengths.Count;
            double stdDev = Math.Sqrt(variance);
            double cv = stdDev / mean;

            if (cv < 0.25)
            {
                return new AnalysisIndicator
                {
                    Name = "Coeficiente de variacao de comprimento de frases",
                    Finding = $"CV de {cv:F2} — frases muito uniformes em tamanho, padrao tipico de IA",
                    Significance = "weak_ai"
                };
            }

            if (cv > 0.60)
            {
                return new AnalysisIndicator
                {
                    Name = "Coeficiente de variacao de comprimento de frases",
                    Finding = $"CV de {cv:F2} — frases com tamanhos bem variados, padrao tipico de escrita humana",
                    Significance = "weak_human"
                };
            }

            return null;
        }
    }
}
