export default function ComponentsSection() {
  return (
    <section className="ds-section" id="ds-components">
      <h2 className="ds-section-title">05. Componentes</h2>

      {/* Border Radius */}
      <div className="ds-subsection">
        <h3 className="ds-subsection-title">Border Radius</h3>
        <div className="ds-radius-showcase">
          <div className="ds-radius-item">
            <div className="ds-radius-box" style={{ borderRadius: "6px" }} />
            <span>6px</span>
            <span className="ds-radius-use">Badges, chips, tags</span>
          </div>
          <div className="ds-radius-item">
            <div className="ds-radius-box" style={{ borderRadius: "10px" }} />
            <span>10px</span>
            <span className="ds-radius-use">Inputs, botoes, items de lista</span>
          </div>
          <div className="ds-radius-item">
            <div className="ds-radius-box" style={{ borderRadius: "14px" }} />
            <span>14px</span>
            <span className="ds-radius-use">Cards, paineis, modais</span>
          </div>
          <div className="ds-radius-item">
            <div className="ds-radius-box" style={{ borderRadius: "9999px" }} />
            <span>9999px</span>
            <span className="ds-radius-use">Avatar, icone circular</span>
          </div>
        </div>
      </div>

      {/* Botoes */}
      <div className="ds-subsection">
        <h3 className="ds-subsection-title">Botoes</h3>
        <div className="ds-component-group">
          <div className="ds-component-row">
            <button className="ds-btn ds-btn-primary ds-btn-sm">Small</button>
            <button className="ds-btn ds-btn-primary ds-btn-md">Primary</button>
            <button className="ds-btn ds-btn-primary ds-btn-lg">Large</button>
          </div>
          <div className="ds-component-row">
            <button className="ds-btn ds-btn-secondary ds-btn-md">Secondary</button>
            <button className="ds-btn ds-btn-ghost ds-btn-md">Ghost</button>
            <button className="ds-btn ds-btn-danger ds-btn-md">Danger</button>
          </div>
          <div className="ds-component-specs">
            <p><strong>Primary:</strong> bg Primary-500 · texto branco · hover Primary-700 · radius 10px</p>
            <p><strong>Secondary:</strong> border 1px Primary-500 · texto Primary-500 · bg transparente</p>
            <p><strong>Ghost:</strong> sem borda · texto Neutral-600 · hover bg Neutral-100</p>
            <p><strong>Danger:</strong> bg #B03030 · texto branco</p>
            <p><strong>Tamanhos:</strong> sm 32px · md 40px · lg 48px (altura)</p>
          </div>
        </div>
      </div>

      {/* Inputs */}
      <div className="ds-subsection">
        <h3 className="ds-subsection-title">Inputs</h3>
        <div className="ds-component-group">
          <div className="ds-input-examples">
            <div className="ds-input-item">
              <label className="ds-label">Input padrao</label>
              <input className="ds-input" type="text" placeholder="Placeholder text..." readOnly />
            </div>
            <div className="ds-input-item">
              <label className="ds-label">Input com foco</label>
              <input className="ds-input ds-input-focused" type="text" defaultValue="Texto digitado" readOnly />
            </div>
            <div className="ds-input-item">
              <label className="ds-label">Input com erro</label>
              <input className="ds-input ds-input-error" type="text" defaultValue="Valor invalido" readOnly />
              <span className="ds-input-error-text">Este campo e obrigatorio</span>
            </div>
          </div>
          <div className="ds-component-specs">
            <p><strong>Altura:</strong> 40–44px · radius 10px</p>
            <p><strong>Border:</strong> 1px Neutral-200 (light) / rgba(155,160,255,0.18) (dark)</p>
            <p><strong>Focus ring:</strong> 2px solid Primary-500 + box-shadow 0 0 0 3px Primary-100</p>
          </div>
        </div>
      </div>

      {/* Cards */}
      <div className="ds-subsection">
        <h3 className="ds-subsection-title">Cards</h3>
        <div className="ds-cards-showcase">
          <div className="ds-card-example">
            <h4>Titulo do Card</h4>
            <p>Conteudo do card com texto de exemplo. Background branco em light mode, bg-card em dark mode.</p>
            <span className="ds-card-meta">Radius 14px · Padding 20–24px</span>
          </div>
        </div>
      </div>

      {/* Badges */}
      <div className="ds-subsection">
        <h3 className="ds-subsection-title">Badges de Status</h3>
        <div className="ds-badges-showcase">
          <span className="ds-badge ds-badge-success">
            <span className="ds-badge-dot" />Verificado
          </span>
          <span className="ds-badge ds-badge-warning">
            <span className="ds-badge-dot" />Pendente
          </span>
          <span className="ds-badge ds-badge-danger">
            <span className="ds-badge-dot" />Erro
          </span>
          <span className="ds-badge ds-badge-neutral">
            <span className="ds-badge-dot" />Neutro
          </span>
        </div>
      </div>

      {/* Icones */}
      <div className="ds-subsection">
        <h3 className="ds-subsection-title">Icones</h3>
        <div className="ds-component-specs">
          <p><strong>Biblioteca:</strong> Lucide Icons — manter consistencia, nao misturar sets</p>
          <p><strong>Tamanho UI:</strong> 18px · <strong>Tamanho hero/feature:</strong> 20–24px</p>
          <p><strong>Cor padrao:</strong> Neutral-400 (inativo) · Primary-500 (ativo)</p>
        </div>
      </div>
    </section>
  );
}
