namespace Verasium.Core
{
    public static class ScoringEngine
    {
        private static readonly Dictionary<string, int> Weights = new()
        {
            { "strong_ai", 4 },
            { "weak_ai", 1 },
            { "neutral", 0 },
            { "weak_human", -1 },
            { "strong_human", -4 },
        };

        public static int ComputeScoreFromIndicators(List<AnalysisIndicator> indicators)
        {
            if (indicators == null || indicators.Count == 0) return 50;

            int sum = 0;
            foreach (var ind in indicators)
                sum += Weights.GetValueOrDefault(ind.Significance, 0);

            int n = indicators.Count;
            int min = -4 * n;
            int max = 4 * n;

            double normalized = (double)(sum - min) / (max - min) * 100;
            return Math.Clamp((int)Math.Round(normalized), 0, 100);
        }

        public static void ComputeScoreAndConclusion(AIAnalysisResult result)
        {
            if (!result.IsSuccessful) return;

            result.ConfidenceScore = ComputeScoreFromIndicators(result.Indicators);

            int strongAiCount = result.Indicators.Count(i => i.Significance == "strong_ai");
            int strongHumanCount = result.Indicators.Count(i => i.Significance == "strong_human");
            int nonNeutralCount = result.Indicators.Count(i => i.Significance != "neutral");

            // Override rules: strong signals force conclusion
            if (strongAiCount >= 2 && strongHumanCount == 0)
            {
                result.Conclusion = "AI-Generated";
                result.ConfidenceScore = Math.Max(result.ConfidenceScore, 75);
            }
            else if (strongHumanCount >= 2 && strongAiCount == 0)
            {
                result.Conclusion = "Human-Made";
                result.ConfidenceScore = Math.Min(result.ConfidenceScore, 25);
            }
            else
            {
                result.Conclusion = result.ConfidenceScore >= 60 ? "AI-Generated"
                                  : result.ConfidenceScore <= 40 ? "Human-Made"
                                  : "Inconclusive";
            }

            result.LowConfidence = nonNeutralCount < 3;
        }
    }
}
