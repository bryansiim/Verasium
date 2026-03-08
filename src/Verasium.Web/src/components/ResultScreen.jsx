export default function ResultScreen({ result, onReset }) {
  const isSuccess = result.isSuccessful;

  return (
    <div className="screen result-screen">
      <div className="result-content">
        <div className={`result-card ${isSuccess ? "success" : "error"}`}>
          <div className="result-header">
            <div className="result-status-icon">
              {isSuccess ? "\u2713" : "\u2717"}
            </div>
            <h2>{isSuccess ? "Resultado da Analise" : "Erro"}</h2>
          </div>
          <div className="result-body">
            {isSuccess ? (
              <p className="ai-response">{result.aiResponse}</p>
            ) : (
              <p className="error-message">{result.errorMessage}</p>
            )}
          </div>
        </div>

        <button className="reset-btn" onClick={onReset}>
          Nova analise
        </button>
      </div>
    </div>
  );
}
