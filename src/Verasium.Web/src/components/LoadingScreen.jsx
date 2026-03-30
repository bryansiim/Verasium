export default function LoadingScreen() {
  return (
    <div className="screen-page">
      <div className="screen-centered">
        <div className="loading-wrap">
          {/* Logo */}
          <div className="loading-logo">
            <svg width="20" height="20" viewBox="0 0 24 24" fill="none" xmlns="http://www.w3.org/2000/svg">
              <circle cx="11" cy="11" r="7" stroke="#4A5FD4" strokeWidth="1.8" />
              <line x1="16" y1="16" x2="21" y2="21" stroke="#4A5FD4" strokeWidth="1.8" strokeLinecap="round" />
              <circle cx="11" cy="11" r="2.5" fill="#4A5FD4" fillOpacity="0.35" />
            </svg>
            Verasium
          </div>

          {/* Scan card with magnifier */}
          <div className="scan-card">
            <div className="magnifier" />
            <div className="txt-row" style={{ width: "88%" }} />
            <div className="txt-row" style={{ width: "72%" }} />
            <div className="txt-row" style={{ width: "94%" }} />
            <div className="txt-row" style={{ width: "58%" }} />
            <div className="txt-row" style={{ width: "80%" }} />
            <div className="txt-row" style={{ width: "66%" }} />
          </div>

          {/* Label + cycling subtitle + dots */}
          <div className="label-area">
            <div className="loading-label">Analisando conteúdo</div>

            <div className="loading-sub">
              <span className="sub-item">Detectando padrões linguísticos</span>
              <span className="sub-item">Verificando coerência semântica</span>
              <span className="sub-item">Analisando metadados</span>
            </div>

            <div className="bouncing-dots">
              <span />
              <span />
              <span />
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}
