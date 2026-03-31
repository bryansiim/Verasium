import { useState } from "react";

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
  video: "Vídeo",
  audio: "Áudio",
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

export default function ResultScreen({ result, onReset }) {
  const [showIndicators, setShowIndicators] = useState(false);
  const isSuccess = result.isSuccessful;

  if (!isSuccess) {
    return (
      <div className="screen-page">
        <div className="screen-centered">
          <div className="result-content">
            <div className="result-card error">
              <div className="result-header">
                <div className="result-status-icon">{"\u2717"}</div>
                <h2>Erro</h2>
              </div>
              <div className="result-body">
                <p className="error-message">
                  {result.errorMessage || "Ocorreu um erro ao processar a análise. Por favor, tente novamente."}
                </p>
              </div>
            </div>
            <button className="reset-btn" onClick={onReset}>Nova análise</button>
          </div>
        </div>
      </div>
    );
  }

  const conclusionStyle = getConclusionStyle(result.conclusion);
  const aiPercent = result.confidenceScore;
  const humanPercent = 100 - aiPercent;
  const significanceWeight = {
    strong_ai: 0,
    strong_human: 1,
    weak_ai: 2,
    weak_human: 3,
    neutral: 4,
  };
  const indicators = [...(result.indicators || [])].sort(
    (a, b) => (significanceWeight[a.significance] ?? 4) - (significanceWeight[b.significance] ?? 4)
  );

  return (
    <div className="screen-page">
      <div className="screen-centered">
        <div className="result-content">
          <div className="result-card success">
            <div className="result-header">
              <div className="result-status-icon">{"\u2713"}</div>
              <h2>Resultado da Análise</h2>
              {result.contentType && (
                <span className="content-type-badge">
                  {contentTypeLabels[result.contentType] || result.contentType}
                </span>
              )}
            </div>

            <div className="result-body">
              <h2 className="conclusion-title" style={{ color: conclusionStyle.color }}>
                {conclusionStyle.label}
              </h2>

              <div className="confidence-section">
                <div className="spectrum-scores">
                  <div className="score-block">
                    <span className="score-pct human">
                      {humanPercent}%
                    </span>
                  </div>
                  <div className="score-block right">
                    <span className="score-pct ai">
                      {aiPercent}%
                    </span>
                  </div>
                </div>

                <div className="spectrum-track">
                  <div
                    className="spectrum-needle"
                    style={{
                      left: `${100 - humanPercent}%`,
                      border: `2.5px solid ${humanPercent > 60 ? "#1D9E75" : aiPercent > 60 ? "#E24B4A" : "#aaa"}`,
                    }}
                  />
                </div>

                <div className="spectrum-foot">
                  <span>Humano</span>
                  <span className="mid">50 / 50</span>
                  <span>IA</span>
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

          <button className="reset-btn" onClick={onReset}>Nova análise</button>
        </div>
      </div>
    </div>
  );
}
