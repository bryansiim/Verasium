export default function MotionSection() {
  const timings = [
    { name: "Ultra-rápido", duration: "100–120ms", use: "hover, toggle", demo: "0.11s" },
    { name: "Rápido", duration: "150ms", use: "tooltip, badge", demo: "0.15s" },
    { name: "Médio", duration: "220–280ms", use: "modal, drawer, transição", demo: "0.25s" },
    { name: "Lento", duration: "400–600ms", use: "loading, intro, animação", demo: "0.5s" },
  ];

  const easings = [
    { name: "Entrada (ease-out)", value: "cubic-bezier(0.0, 0.0, 0.2, 1.0)" },
    { name: "Saída (ease-in)", value: "cubic-bezier(0.4, 0.0, 1.0, 1.0)" },
    { name: "Geral", value: "cubic-bezier(0.4, 0.0, 0.2, 1.0)" },
  ];

  return (
    <section className="ds-section" id="ds-motion">
      <h2 className="ds-section-title">07. Motion</h2>

      <div className="ds-subsection">
        <h3 className="ds-subsection-title">Timings</h3>
        <div className="ds-motion-demos">
          {timings.map((t) => (
            <div className="ds-motion-item" key={t.name}>
              <div className="ds-motion-info">
                <span className="ds-motion-name">{t.name}</span>
                <span className="ds-motion-duration">{t.duration}</span>
                <span className="ds-motion-use">{t.use}</span>
              </div>
              <div
                className="ds-motion-box"
                style={{ transition: `all ${t.demo} cubic-bezier(0.4, 0.0, 0.2, 1.0)` }}
              />
            </div>
          ))}
        </div>
      </div>

      <div className="ds-subsection">
        <h3 className="ds-subsection-title">Easing</h3>
        <div className="ds-table-wrapper">
          <table className="ds-table">
            <thead>
              <tr><th>Tipo</th><th>Valor</th></tr>
            </thead>
            <tbody>
              {easings.map((e) => (
                <tr key={e.name}><td>{e.name}</td><td><code>{e.value}</code></td></tr>
              ))}
            </tbody>
          </table>
        </div>
      </div>

      <div className="ds-subsection">
        <h3 className="ds-subsection-title">Microinterações</h3>
        <ul className="ds-rules-list">
          <li><strong>Input focus:</strong> ring fade-in + scale(1.004) no container</li>
          <li><strong>Botão press:</strong> scale(0.98)</li>
          <li><strong>Card hover:</strong> translateY(-2px) + shadow mais intensa (light)</li>
          <li><strong>Resultado entrada:</strong> fade-in + translateY(12px → 0)</li>
          <li><strong>Score de confiança:</strong> barra preenche com ease-out</li>
          <li><strong>Loading global:</strong> spark do ícone pulsa suavemente (800ms loop)</li>
        </ul>
      </div>

      <div className="ds-subsection">
        <h3 className="ds-subsection-title">Loading States</h3>
        <p className="ds-body">Usar skeleton screens, nunca spinners isolados.</p>
        <div className="ds-skeleton-demo">
          <div className="ds-skeleton ds-skeleton-circle" />
          <div className="ds-skeleton-lines">
            <div className="ds-skeleton ds-skeleton-line" style={{ width: "70%" }} />
            <div className="ds-skeleton ds-skeleton-line" style={{ width: "50%" }} />
            <div className="ds-skeleton ds-skeleton-line" style={{ width: "85%" }} />
          </div>
        </div>
      </div>
    </section>
  );
}
