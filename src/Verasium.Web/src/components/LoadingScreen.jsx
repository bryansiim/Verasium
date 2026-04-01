import logo from "../assets/logo.png";

const LOADING_PHRASES = {
  text: {
    title: "Analisando texto",
    subtitles: [
      "Detectando padrões linguísticos",
      "Verificando coerência semântica",
      "Analisando estrutura textual",
    ],
  },
  image: {
    title: "Analisando imagem",
    subtitles: [
      "Detectando artefatos visuais",
      "Verificando consistência de pixels",
      "Analisando metadados da imagem",
    ],
  },
  video: {
    title: "Analisando vídeo",
    subtitles: [
      "Detectando inconsistências visuais",
      "Verificando artefatos temporais",
      "Analisando quadros-chave",
    ],
  },
  audio: {
    title: "Analisando áudio",
    subtitles: [
      "Detectando padrões de voz sintética",
      "Verificando espectro de frequência",
      "Analisando artefatos sonoros",
    ],
  },
  pdf: {
    title: "Analisando documento",
    subtitles: [
      "Detectando padrões linguísticos",
      "Verificando estrutura do documento",
      "Analisando metadados e formatação",
    ],
  },
};

export default function LoadingScreen({ contentType = "text" }) {
  const phrases = LOADING_PHRASES[contentType] || LOADING_PHRASES.text;
  return (
    <div className="screen-page">
      <div className="screen-centered">
        <div className="loading-wrap">
          {/* Logo */}
          <div className="loading-logo">
            <img src={logo} alt="Verasium" className="loading-logo-img" draggable={false} />
          </div>

          {/* Scan card with magnifier */}
          <div className="scan-card">
            <div className="magnifier" />
            <div className="txt-row" style={{ width: "88%" }} />
            <div className="txt-row" style={{ width: "72%" }} />
            <div className="txt-row" style={{ width: "94%" }} />
            <div className="txt-row" style={{ width: "58%" }} />
            <div className="txt-row" style={{ width: "80%" }} />
            <div className="txt-row" style={{ width: "66%" }} />
          </div>

          {/* Label + cycling subtitle + dots */}
          <div className="label-area">
            <div className="loading-label">{phrases.title}</div>

            <div className="loading-sub">
              {phrases.subtitles.map((text, i) => (
                <span key={i} className="sub-item">{text}</span>
              ))}
            </div>

            <div className="bouncing-dots">
              <span />
              <span />
              <span />
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}
