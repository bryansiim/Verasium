export default function DontsSection() {
  const donts = [
    "Não redesenhar ou recriar a logo Verasium",
    "Não usar o gradiente menta/lavanda da logo escura como cor de UI",
    "Não introduzir novas cores de marca além do ramp Primary",
    "Não usar cinzas quentes ou neutros sem matiz azul-roxo",
    "Não usar cores semânticas (verde, âmbar, vermelho) como decoração",
    "Não misturar Museo Moderno com Plus Jakarta Sans no mesmo nível",
    "Não usar Primary-500 em texto corrido, ícones inativos ou fundos grandes",
    "Não criar um dark mode com identidade visual diferente do light mode",
    "Não usar sombras em dark mode, usar bordas sutis",
    "Não misturar sets de ícones diferentes",
  ];

  return (
    <section className="ds-section" id="ds-donts">
      <h2 className="ds-section-title">09. O que NÃO fazer</h2>
      <p className="ds-section-desc">
        Regras absolutas para manter a consistência visual do Verasium.
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
