import logoLight from "../../../assets/Verasium-logo.png";
import logoDark from "../../../assets/Verasium-Dark.png";

export default function BrandSection() {
  return (
    <section className="ds-section" id="ds-brand">
      <h2 className="ds-section-title">01. Marca & Conceito</h2>
      <p className="ds-section-desc">
        <strong>Verasium</strong> — do latim <em>"veritas"</em> (verdade). Plataforma de busca
        e verificacao inteligente com IA. Tom: confiante, preciso, tecnologico, acessivel e moderno.
      </p>

      <div className="ds-subsection">
        <h3 className="ds-subsection-title">Logo</h3>
        <p className="ds-body">
          Icone: lupa com estrela de 4 pontas (spark) no interior. A logo e um asset fixo — nao
          redesenhar, recriar ou substituir.
        </p>

        <div className="ds-logo-showcase">
          <div className="ds-logo-card ds-logo-light">
            <img src={logoLight} alt="Verasium logo clara" draggable={false} />
            <span className="ds-logo-label">Logo Clara</span>
            <span className="ds-logo-detail">
              Gradiente azul-marinho (#1A1A3E) → roxo-indigo (#4040CC), fundo branco
            </span>
          </div>
          <div className="ds-logo-card ds-logo-dark">
            <img src={logoDark} alt="Verasium logo escura" draggable={false} />
            <span className="ds-logo-label">Logo Escura / Icone</span>
            <span className="ds-logo-detail">
              Gradiente menta → lavanda, fundo escuro. Este gradiente e um asset de marca — nao replicar em componentes de UI.
            </span>
          </div>
        </div>
      </div>

      <div className="ds-subsection">
        <h3 className="ds-subsection-title">Regras de Uso</h3>
        <div className="ds-rules-grid">
          <div className="ds-rule-card ds-rule-do">
            <span className="ds-rule-icon">&#10003;</span>
            <span>Usar o asset fornecido sem alteracoes</span>
          </div>
          <div className="ds-rule-card ds-rule-do">
            <span className="ds-rule-icon">&#10003;</span>
            <span>Manter area de respiro ao redor da logo</span>
          </div>
          <div className="ds-rule-card ds-rule-dont">
            <span className="ds-rule-icon">&#10007;</span>
            <span>Nao redesenhar ou recriar a logo</span>
          </div>
          <div className="ds-rule-card ds-rule-dont">
            <span className="ds-rule-icon">&#10007;</span>
            <span>Nao usar o gradiente da logo escura em UI</span>
          </div>
        </div>
      </div>
    </section>
  );
}
