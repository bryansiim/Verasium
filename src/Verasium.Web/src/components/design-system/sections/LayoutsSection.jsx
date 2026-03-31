export default function LayoutsSection() {
  return (
    <section className="ds-section" id="ds-layouts">
      <h2 className="ds-section-title">06. Layouts Principais</h2>

      <div className="ds-subsection">
        <h3 className="ds-subsection-title">Hero / Onboarding</h3>
        <div className="ds-wireframe">
          <div className="ds-wire-topbar">Header 56px</div>
          <div className="ds-wire-body">
            <div className="ds-wire-logo">Logo</div>
            <div className="ds-wire-title">Título Museo Moderno</div>
            <div className="ds-wire-search">Campo de busca (max-width 600px)</div>
            <div className="ds-wire-chips">
              <span>Chip 1</span><span>Chip 2</span><span>Chip 3</span>
            </div>
          </div>
        </div>
        <div className="ds-component-specs">
          <p>Fundo: Neutral-50 (light) / bg-page #0E0E1A (dark)</p>
          <p>Logo centralizada no topo · Título em Museo Moderno · Input centralizado</p>
        </div>
      </div>

      <div className="ds-subsection">
        <h3 className="ds-subsection-title">App / Dashboard</h3>
        <div className="ds-wireframe ds-wire-dashboard">
          <div className="ds-wire-sidebar">
            <div className="ds-wire-sidebar-item active">Item ativo</div>
            <div className="ds-wire-sidebar-item">Item</div>
            <div className="ds-wire-sidebar-item">Item</div>
            <div className="ds-wire-sidebar-item">Item</div>
          </div>
          <div className="ds-wire-main">
            <div className="ds-wire-topbar">Header 56px + Breadcrumb</div>
            <div className="ds-wire-content">
              <div className="ds-wire-card-grid">
                <div className="ds-wire-card-sm">Card</div>
                <div className="ds-wire-card-sm">Card</div>
                <div className="ds-wire-card-sm">Card</div>
                <div className="ds-wire-card-sm">Card</div>
              </div>
            </div>
          </div>
        </div>
        <div className="ds-component-specs">
          <p>Sidebar fixa 240px + área principal · Header 56px · Padding 24–32px</p>
          <p>Grade de cards: 2–4 colunas desktop, 1 mobile</p>
        </div>
      </div>

      <div className="ds-subsection">
        <h3 className="ds-subsection-title">Resultados / Verificação</h3>
        <div className="ds-wireframe">
          <div className="ds-wire-topbar">Header + Filtros</div>
          <div className="ds-wire-body">
            <div className="ds-wire-result-card">
              <span className="ds-wire-status">&#9679;</span>
              <div className="ds-wire-result-info">
                <div>Título do resultado</div>
                <div className="ds-wire-bar" />
              </div>
            </div>
            <div className="ds-wire-result-card">
              <span className="ds-wire-status">&#9679;</span>
              <div className="ds-wire-result-info">
                <div>Título do resultado</div>
                <div className="ds-wire-bar" />
              </div>
            </div>
          </div>
        </div>
        <div className="ds-component-specs">
          <p>Card de resultado: ícone de status · título · score de confiança · fonte · data</p>
          <p>Barra de confiança com cor semântica · Expansível para detalhes</p>
        </div>
      </div>
    </section>
  );
}
