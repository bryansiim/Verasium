export default function TypographySection() {
  const hierarchy = [
    { role: "Display / Hero", font: "Museo Moderno", size: "48–64px", weight: "700", lh: "1.2", cls: "ds-typo-display" },
    { role: "H1", font: "Museo Moderno", size: "36–48px", weight: "700", lh: "1.2", cls: "ds-typo-h1" },
    { role: "H2", font: "Plus Jakarta Sans", size: "24–30px", weight: "600", lh: "1.3", cls: "ds-typo-h2" },
    { role: "H3", font: "Plus Jakarta Sans", size: "18–22px", weight: "600", lh: "1.4", cls: "ds-typo-h3" },
    { role: "Body Large", font: "Plus Jakarta Sans", size: "16px", weight: "400", lh: "1.75", cls: "ds-typo-body-lg" },
    { role: "Body", font: "Plus Jakarta Sans", size: "14–15px", weight: "400", lh: "1.7", cls: "ds-typo-body" },
    { role: "Label / UI", font: "Plus Jakarta Sans", size: "12–13px", weight: "500", lh: "1.5", cls: "ds-typo-label" },
    { role: "Caption", font: "Plus Jakarta Sans", size: "11–12px", weight: "400", lh: "1.5", cls: "ds-typo-caption" },
  ];

  return (
    <section className="ds-section" id="ds-typography">
      <h2 className="ds-section-title">02. Tipografia</h2>

      <div className="ds-subsection">
        <h3 className="ds-subsection-title">Fontes</h3>
        <div className="ds-font-specimens">
          <div className="ds-font-card">
            <span className="ds-font-name ds-museo">Museo Moderno</span>
            <span className="ds-font-role">Display & Títulos (H1, Hero, Logotipo)</span>
            <p className="ds-font-sample ds-museo">
              Aa Bb Cc Dd Ee Ff Gg Hh Ii Jj Kk Ll Mm Nn Oo Pp Qq Rr Ss Tt Uu Vv Ww Xx Yy Zz
            </p>
            <p className="ds-font-sample ds-museo">0 1 2 3 4 5 6 7 8 9</p>
            <span className="ds-font-meta">Geométrica, arredondada, moderna. Peso: 600–700</span>
          </div>
          <div className="ds-font-card">
            <span className="ds-font-name ds-jakarta">Plus Jakarta Sans</span>
            <span className="ds-font-role">Corpo & Interface (todo o restante)</span>
            <p className="ds-font-sample ds-jakarta">
              Aa Bb Cc Dd Ee Ff Gg Hh Ii Jj Kk Ll Mm Nn Oo Pp Qq Rr Ss Tt Uu Vv Ww Xx Yy Zz
            </p>
            <p className="ds-font-sample ds-jakarta">0 1 2 3 4 5 6 7 8 9</p>
            <span className="ds-font-meta">Sans-serif geométrica-humanista. Peso: 400–700</span>
          </div>
        </div>
      </div>

      <div className="ds-subsection">
        <h3 className="ds-subsection-title">Hierarquia</h3>
        <div className="ds-typo-hierarchy">
          {hierarchy.map((h) => (
            <div className="ds-typo-row" key={h.role}>
              <div className="ds-typo-preview">
                <span className={h.cls}>Verasium verifica a verdade</span>
              </div>
              <div className="ds-typo-specs">
                <span className="ds-typo-role">{h.role}</span>
                <span className="ds-typo-detail">{h.font} · {h.weight} · {h.size} · lh {h.lh}</span>
              </div>
            </div>
          ))}
        </div>
      </div>

      <div className="ds-subsection">
        <h3 className="ds-subsection-title">Regras</h3>
        <ul className="ds-rules-list">
          <li>Nunca misturar as duas fontes no mesmo nível hierárquico</li>
          <li>Museo Moderno apenas em H1/display, nunca em labels, botões ou body</li>
          <li>Peso mínimo em texto corrido: 400. Nunca 300 ou lighter.</li>
        </ul>
      </div>
    </section>
  );
}
