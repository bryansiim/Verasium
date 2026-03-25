import { useState } from "react";
import Navbar from "./Navbar";

const significanceLabels = {
  strong_ai: "Forte sinal de IA",
  weak_ai: "Leve sinal de IA",
  neutral: "Neutro",
  weak_human: "Leve sinal humano",
  strong_human: "Forte sinal humano",
};

const significanceColors = {
  strong_ai: "var(--red)",
  weak_ai: "var(--amber)",
  neutral: "var(--gray-400)",
  weak_human: "var(--green-light)",
  strong_human: "var(--green)",
};

const contentTypeLabels = {
  image: "Imagem",
  text: "Texto",
  pdf: "PDF",
  video: "Video",
  audio: "Audio",
};

function getConclusionStyle(conclusion) {
  switch (conclusion) {
    case "AI-Generated":
      return { bg: "var(--red-tint)", color: "var(--red)", label: "Gerado por IA" };
    case "Human-Made":
      return { bg: "var(--green-tint)", color: "var(--green)", label: "Produzido por Humano" };
    default:
      return { bg: "var(--amber-tint)", color: "var(--amber)", label: "Inconclusivo" };
  }
}

function getScoreColor(score) {
  if (score <= 30) return "var(--green)";
  if (score <= 60) return "var(--amber)";
  return "var(--red)";
}

export default function ResultScreen({ result, onReset }) {
  const [showIndicators, setShowIndicators] = useState(false);
  const isSuccess = result.isSuccessful;

  if (!isSuccess) {
    return (
      <div className="screen-page">
        <Navbar />
        <div className="screen-centered">
          <div className="result-content">
            <div className="result-card error">
              <div className="result-header">
                <div className="result-status-icon">{"\u2717"}</div>
                <h2>Erro</h2>
              </div>
              <div className="result-body">
                <p className="error-message">
                  {result.errorMessage || "Ocorreu um erro ao processar a analise. Por favor, tente novamente."}
                </p>
              </div>
            </div>
            <button className="reset-btn" onClick={onReset}>Nova analise</button>
          </div>
        </div>
      </div>
    );
  }

  const conclusionStyle = getConclusionStyle(result.conclusion);
  const scoreColor = getScoreColor(result.confidenceScore);
  const indicators = result.indicators || [];

  return (
    <div className="screen-page">
      <Navbar />
      <div className="screen-centered">
        <div className="result-content">
          <div className="result-card success">
            <div className="result-header">
              <div className="result-status-icon">{"\u2713"}</div>
              <h2>Resultado da Analise</h2>
              {result.contentType && (
                <span className="content-type-badge">
                  {contentTypeLabels[result.contentType] || result.contentType}
                </span>
              )}
            </div>

            <div className="result-body">
              <div
                className="conclusion-badge"
                style={{ background: conclusionStyle.bg, color: conclusionStyle.color }}
              >
                {conclusionStyle.label}
              </div>

              <div className="confidence-section">
                <div className="confidence-label">
                  <span>Probabilidade de ser IA</span>
                  <span style={{ color: scoreColor, fontWeight: 700 }}>
                    {result.confidenceScore}%
                  </span>
                </div>
                <div className="confidence-meter">
                  <div
                    className="confidence-fill"
                    style={{
                      width: `${result.confidenceScore}%`,
                      background: scoreColor,
                    }}
                  />
                </div>
              </div>

              {result.justification && (
                <div className="justification-section">
                  <h3>Justificativa</h3>
                  <p>{result.justification}</p>
                </div>
              )}

              {indicators.length > 0 && (
                <div className="indicators-section">
                  <button
                    className="indicators-toggle"
                    onClick={() => setShowIndicators(!showIndicators)}
                  >
                    <span>Indicadores analisados ({indicators.length})</span>
                    <span className={`indicators-arrow ${showIndicators ? "open" : ""}`}>
                      &#9662;
                    </span>
                  </button>

                  {showIndicators && (
                    <div className="indicators-list">
                      {indicators.map((ind, i) => (
                        <div key={i} className="indicator-row">
                          <span
                            className="indicator-dot"
                            style={{ background: significanceColors[ind.significance] || "var(--gray-400)" }}
                            title={significanceLabels[ind.significance] || ind.significance}
                          />
                          <div className="indicator-info">
                            <strong>{ind.name}</strong>
                            <span>{ind.finding}</span>
                          </div>
                          <span
                            className="indicator-badge"
                            style={{ color: significanceColors[ind.significance] || "var(--gray-400)" }}
                          >
                            {significanceLabels[ind.significance] || ind.significance}
                          </span>
                        </div>
                      ))}
                    </div>
                  )}
                </div>
              )}
            </div>
          </div>

          <button className="reset-btn" onClick={onReset}>Nova analise</button>
        </div>
      </div>
    </div>
  );
}
