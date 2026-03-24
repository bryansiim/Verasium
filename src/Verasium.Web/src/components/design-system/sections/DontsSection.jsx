export default function DontsSection() {
  const donts = [
    "Nao redesenhar ou recriar a logo Verasium",
    "Nao usar o gradiente menta/lavanda da logo escura como cor de UI",
    "Nao introduzir novas cores de marca alem do ramp Primary",
    "Nao usar cinzas quentes ou neutros sem matiz azul-roxo",
    "Nao usar cores semanticas (verde, ambar, vermelho) como decoracao",
    "Nao misturar Museo Moderno com Plus Jakarta Sans no mesmo nivel",
    "Nao usar Primary-500 em texto corrido, icones inativos ou fundos grandes",
    "Nao criar um dark mode com identidade visual diferente do light mode",
    "Nao usar sombras em dark mode — usar bordas sutis",
    "Nao misturar sets de icones diferentes",
  ];

  return (
    <section className="ds-section" id="ds-donts">
      <h2 className="ds-section-title">09. O que NAO fazer</h2>
      <p className="ds-section-desc">
        Regras absolutas para manter a consistencia visual do Verasium.
      </p>

      <div className="ds-donts-grid">
        {donts.map((text, i) => (
          <div className="ds-dont-card" key={i}>
            <span className="ds-dont-icon">&#10007;</span>
            <span className="ds-dont-text">{text}</span>
          </div>
        ))}
      </div>
    </section>
  );
}
