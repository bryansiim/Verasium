function Swatch({ name, hex, desc }) {
  return (
    <div className="ds-swatch">
      <div className="ds-swatch-color" style={{ backgroundColor: hex }} />
      <div className="ds-swatch-info">
        <span className="ds-swatch-name">{name}</span>
        <span className="ds-swatch-hex">{hex}</span>
        {desc && <span className="ds-swatch-desc">{desc}</span>}
      </div>
    </div>
  );
}

function SwatchGroup({ title, swatches }) {
  return (
    <div className="ds-swatch-group">
      <h4 className="ds-swatch-group-title">{title}</h4>
      <div className="ds-swatch-grid">
        {swatches.map((s) => (
          <Swatch key={s.name} {...s} />
        ))}
      </div>
    </div>
  );
}

export default function ColorsSection() {
  const primary = [
    { name: "Primary 50", hex: "#ECEEFF", desc: "tints, fundos de badge" },
    { name: "Primary 100", hex: "#C7CBFF", desc: "focus rings, estados passivos" },
    { name: "Primary 200", hex: "#9EA5F9", desc: "accent dark mode, bordas" },
    { name: "Primary 400", hex: "#6B72F0", desc: "icones ativos, links" },
    { name: "Primary 500", hex: "#4B50D6", desc: "COR PRINCIPAL — botoes, foco" },
    { name: "Primary 700", hex: "#3530A8", desc: "hover botao primario" },
    { name: "Primary 900", hex: "#1E1A6E", desc: "texto forte light mode" },
  ];

  const neutrals = [
    { name: "Neutral 50", hex: "#F5F5FA", desc: "fundo pagina (light)" },
    { name: "Neutral 100", hex: "#E4E4EE", desc: "cards secundarios, bordas" },
    { name: "Neutral 200", hex: "#C0C0D4", desc: "bordas, separadores" },
    { name: "Neutral 400", hex: "#8080A0", desc: "placeholder, icones inativos" },
    { name: "Neutral 600", hex: "#4A4A6A", desc: "texto secundario (light)" },
    { name: "Neutral 900", hex: "#1C1C2E", desc: "fundo cards (dark)" },
    { name: "Neutral 950", hex: "#0E0E1A", desc: "fundo pagina (dark)" },
  ];

  const semantic = [
    { name: "Success Base", hex: "#2B7A4B" },
    { name: "Success Tint", hex: "#DCFAE8" },
    { name: "Success Border", hex: "#A3D9B8" },
    { name: "Success Text", hex: "#1A5C35" },
    { name: "Warning Base", hex: "#A36A00" },
    { name: "Warning Tint", hex: "#FFF3CC" },
    { name: "Warning Border", hex: "#E8C87A" },
    { name: "Warning Text", hex: "#7A4F00" },
    { name: "Danger Base", hex: "#B03030" },
    { name: "Danger Tint", hex: "#FFE0E0" },
    { name: "Danger Border", hex: "#FFAAAA" },
    { name: "Danger Text", hex: "#7A1A1A" },
    { name: "Info Base", hex: "#2257B0" },
    { name: "Info Tint", hex: "#E8F0FC" },
    { name: "Info Border", hex: "#A3BCF0" },
    { name: "Info Text", hex: "#1A3D8A" },
  ];

  const darkTokens = [
    { name: "bg-page", hex: "#0E0E1A" },
    { name: "bg-panel", hex: "#161624" },
    { name: "bg-card", hex: "#1E1E30" },
    { name: "text-primary", hex: "#EEEEF8" },
    { name: "text-secondary", hex: "#9090B8" },
    { name: "text-tertiary", hex: "#5C5C80" },
    { name: "primary-accent", hex: "#9EA5F9", desc: "Primary-200" },
  ];

  return (
    <section className="ds-section" id="ds-colors">
      <h2 className="ds-section-title">03. Paleta de Cores</h2>
      <p className="ds-section-desc">
        A identidade cromatica e exclusivamente azul-indigo/roxo (ramp Primary).
        Todas as demais cores servem funcao — nunca decoracao.
      </p>

      <SwatchGroup title="Primary — Identidade da Marca" swatches={primary} />
      <SwatchGroup title="Neutros — Escala Fria (matiz azul-roxo)" swatches={neutrals} />
      <SwatchGroup title="Semanticas — Feedback de Sistema" swatches={semantic} />
      <SwatchGroup title="Dark Mode — Tokens" swatches={darkTokens} />

      <div className="ds-subsection">
        <h3 className="ds-subsection-title">Regras</h3>
        <ul className="ds-rules-list">
          <li>Semanticas usadas SOMENTE para estados funcionais (validacoes, alertas, status)</li>
          <li>Nunca usar cores semanticas como decoracao ou identidade visual</li>
          <li>Dark mode e variacao da mesma identidade — nao introduzir novas cores</li>
          <li>Neutros tem leve matiz azul-roxo — nao usar cinzas quentes ou puros</li>
        </ul>
      </div>
    </section>
  );
}
