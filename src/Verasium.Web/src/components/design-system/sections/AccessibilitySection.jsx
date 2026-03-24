export default function AccessibilitySection() {
  return (
    <section className="ds-section" id="ds-accessibility">
      <h2 className="ds-section-title">08. Acessibilidade</h2>

      <div className="ds-subsection">
        <h3 className="ds-subsection-title">Contraste</h3>
        <div className="ds-a11y-cards">
          <div className="ds-a11y-card">
            <div className="ds-a11y-ratio">7:1</div>
            <span className="ds-a11y-level">WCAG AAA</span>
            <p>Texto primario sobre fundo — minimo obrigatorio</p>
          </div>
          <div className="ds-a11y-card">
            <div className="ds-a11y-ratio">4.5:1</div>
            <span className="ds-a11y-level">WCAG AA</span>
            <p>Texto secundario sobre fundo — minimo aceitavel</p>
          </div>
        </div>
        <p className="ds-body ds-note">
          Verificar Primary-500 (#4B50D6) sobre branco antes de usar em texto.
        </p>
      </div>

      <div className="ds-subsection">
        <h3 className="ds-subsection-title">Focus</h3>
        <div className="ds-a11y-focus-demo">
          <button className="ds-btn ds-btn-primary ds-btn-md ds-focus-demo">
            Elemento com foco
          </button>
        </div>
        <div className="ds-component-specs">
          <p><strong>Ring:</strong> 2px solid Primary-500, offset 2px</p>
          <p><strong>Shadow:</strong> 0 0 0 4px Primary-100</p>
          <p>Todos os elementos interativos com <code>:focus-visible</code> visivel</p>
        </div>
      </div>

      <div className="ds-subsection">
        <h3 className="ds-subsection-title">Diretrizes Gerais</h3>
        <ul className="ds-rules-list">
          <li>Nunca usar cor como unico indicador de estado — sempre acompanhar com icone ou texto</li>
          <li>Todos os icones decorativos com <code>aria-hidden="true"</code></li>
          <li>Inputs sempre com label associado (<code>for/id</code> ou <code>aria-label</code>)</li>
          <li>Animacoes respeitam <code>prefers-reduced-motion</code></li>
        </ul>
      </div>
    </section>
  );
}
