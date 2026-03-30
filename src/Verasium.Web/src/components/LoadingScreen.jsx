import logo from "../assets/logo.png";

export default function LoadingScreen() {
  return (
    <div className="screen-page">
      <div className="screen-centered">
        <div className="loading-wrap">
          {/* Logo */}
          <div className="loading-logo">
            <img src={logo} alt="Verasium" className="loading-logo-img" draggable={false} />
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
