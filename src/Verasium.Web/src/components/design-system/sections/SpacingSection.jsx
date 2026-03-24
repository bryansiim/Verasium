export default function SpacingSection() {
  const scale = [4, 8, 12, 16, 20, 24, 32, 40, 48, 64, 80, 96, 128];

  const breakpoints = [
    { name: "Mobile", range: "< 768px", cols: "4 colunas", gutter: "16px", margin: "20px" },
    { name: "Tablet", range: "768 – 1023px", cols: "8 colunas", gutter: "20px", margin: "32px" },
    { name: "Desktop", range: "1024 – 1439px", cols: "12 colunas", gutter: "24px", margin: "40px" },
    { name: "Wide", range: "≥ 1440px", cols: "12 colunas", gutter: "24px", margin: "40px" },
  ];

  return (
    <section className="ds-section" id="ds-spacing">
      <h2 className="ds-section-title">04. Espacamento & Grid</h2>

      <div className="ds-subsection">
        <h3 className="ds-subsection-title">Escala Base (multiplos de 4px)</h3>
        <div className="ds-spacing-scale">
          {scale.map((px) => (
            <div className="ds-spacing-row" key={px}>
              <span className="ds-spacing-label">{px}px</span>
              <div className="ds-spacing-bar" style={{ width: `${Math.min(px, 128)}px` }} />
            </div>
          ))}
        </div>
      </div>

      <div className="ds-subsection">
        <h3 className="ds-subsection-title">Grid</h3>
        <div className="ds-grid-specs">
          <div className="ds-spec-item">
            <span className="ds-spec-label">Max-width</span>
            <span className="ds-spec-value">1200px</span>
          </div>
          <div className="ds-spec-item">
            <span className="ds-spec-label">Colunas Desktop</span>
            <span className="ds-spec-value">12</span>
          </div>
          <div className="ds-spec-item">
            <span className="ds-spec-label">Colunas Mobile</span>
            <span className="ds-spec-value">4</span>
          </div>
          <div className="ds-spec-item">
            <span className="ds-spec-label">Gutter Desktop</span>
            <span className="ds-spec-value">24px</span>
          </div>
          <div className="ds-spec-item">
            <span className="ds-spec-label">Gutter Mobile</span>
            <span className="ds-spec-value">16px</span>
          </div>
        </div>
      </div>

      <div className="ds-subsection">
        <h3 className="ds-subsection-title">Breakpoints</h3>
        <div className="ds-table-wrapper">
          <table className="ds-table">
            <thead>
              <tr>
                <th>Nome</th>
                <th>Faixa</th>
                <th>Colunas</th>
                <th>Gutter</th>
                <th>Margin</th>
              </tr>
            </thead>
            <tbody>
              {breakpoints.map((bp) => (
                <tr key={bp.name}>
                  <td><strong>{bp.name}</strong></td>
                  <td>{bp.range}</td>
                  <td>{bp.cols}</td>
                  <td>{bp.gutter}</td>
                  <td>{bp.margin}</td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </div>
    </section>
  );
}
