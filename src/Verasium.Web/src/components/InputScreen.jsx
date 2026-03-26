import { useState, useRef } from "react";
import logo from "../assets/logo.png";
import Navbar from "./Navbar";

const FORMAT_CARDS = [
  {
    icon: (
      <svg width="28" height="28" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="1.5" strokeLinecap="round" strokeLinejoin="round">
        <path d="M14 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V8z" />
        <polyline points="14 2 14 8 20 8" />
        <line x1="16" y1="13" x2="8" y2="13" />
        <line x1="16" y1="17" x2="8" y2="17" />
        <polyline points="10 9 9 9 8 9" />
      </svg>
    ),
    title: "Texto",
    desc: "Artigos, redacoes, e-mails, posts",
    formats: "TXT, DOC, DOCX",
  },
  {
    icon: (
      <svg width="28" height="28" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="1.5" strokeLinecap="round" strokeLinejoin="round">
        <rect x="3" y="3" width="18" height="18" rx="2" ry="2" />
        <circle cx="8.5" cy="8.5" r="1.5" />
        <polyline points="21 15 16 10 5 21" />
      </svg>
    ),
    title: "Imagens",
    desc: "Fotos, ilustracoes, arte digital",
    formats: "JPG, PNG, WEBP, GIF, BMP",
  },
  {
    icon: (
      <svg width="28" height="28" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="1.5" strokeLinecap="round" strokeLinejoin="round">
        <path d="M14 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V8z" />
        <polyline points="14 2 14 8 20 8" />
        <path d="M9 15h6" />
        <path d="M9 11h6" />
      </svg>
    ),
    title: "PDFs",
    desc: "Documentos, relatorios, artigos",
    formats: "PDF (ate 20MB)",
  },
  {
    icon: (
      <svg width="28" height="28" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="1.5" strokeLinecap="round" strokeLinejoin="round">
        <polygon points="23 7 16 12 23 17 23 7" />
        <rect x="1" y="5" width="15" height="14" rx="2" ry="2" />
      </svg>
    ),
    title: "Videos",
    desc: "Clips, deepfakes, gerados por IA",
    formats: "MP4, MOV, AVI, WEBM, MKV",
  },
  {
    icon: (
      <svg width="28" height="28" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="1.5" strokeLinecap="round" strokeLinejoin="round">
        <path d="M9 18V5l12-2v13" />
        <circle cx="6" cy="18" r="3" />
        <circle cx="18" cy="16" r="3" />
      </svg>
    ),
    title: "Audios",
    desc: "Vozes sinteticas, musicas, podcasts",
    formats: "MP3, WAV, AAC, OGG, FLAC",
  },
];

const STEPS = [
  {
    number: "1",
    title: "Envie o conteudo",
    desc: "Cole um texto ou faca upload de qualquer arquivo suportado — imagem, PDF, video ou audio.",
  },
  {
    number: "2",
    title: "IA analisa em profundidade",
    desc: "Nosso motor de analise examina padroes linguisticos, visuais, sonoros e estruturais do conteudo.",
  },
  {
    number: "3",
    title: "Receba o veredito",
    desc: "Obtenha uma conclusao clara com score de confianca e indicadores detalhados da analise.",
  },
];

export default function InputScreen({ onSubmit, onFileUpload }) {
  const [content, setContent] = useState("");
  const [fileName, setFileName] = useState(null);
  const [dragging, setDragging] = useState(false);
  const fileInputRef = useRef(null);
  const selectedFileRef = useRef(null);
  const handleSubmit = (e) => {
    e.preventDefault();
    if (selectedFileRef.current) {
      onFileUpload(selectedFileRef.current);
    } else if (content.trim()) {
      onSubmit(content.trim());
    }
  };

  const handleKeyDown = (e) => {
    if (e.key === "Enter" && !e.shiftKey) {
      e.preventDefault();
      handleSubmit(e);
    }
  };

  const handleFileChange = (e) => {
    const file = e.target.files[0];
    if (file) {
      selectedFileRef.current = file;
      setFileName(file.name);
      setContent("");
    }
  };

  const clearFile = () => {
    selectedFileRef.current = null;
    setFileName(null);
    fileInputRef.current.value = "";
  };

  const handleDrop = (e) => {
    e.preventDefault();
    setDragging(false);
    const file = e.dataTransfer.files[0];
    if (file) {
      selectedFileRef.current = file;
      setFileName(file.name);
      setContent("");
    }
  };

  const handleDragOver = (e) => {
    e.preventDefault();
    setDragging(true);
  };

  const handleDragLeave = () => {
    setDragging(false);
  };

  const hasContent = content.trim() || fileName;

  return (
    <div className="landing">
      <Navbar />

      {/* ===== HERO ===== */}
      <section className="hero" id="analisar">
        <div className="hero-bg-glow" />
        <div className="hero-logo-wrapper">
          <img
            src={logo}
            alt="Verasium"
            className="hero-logo"
            draggable={false}
            onDragStart={(e) => e.preventDefault()}
          />
        </div>
        <div className="hero-grid">
          <div className="hero-content">
            <h1 className="hero-title">
              Descubra se o conteudo e<br />
              <span className="hero-highlight">real ou gerado por IA</span>
            </h1>
            <p className="hero-subtitle">
              Analise textos, imagens, PDFs, videos e audios com inteligencia artificial avancada.
              Resultados detalhados com indicadores de confianca em segundos.
            </p>
            <div className="hero-stats">
              <div className="hero-stat">
                <span className="hero-stat-number">5+</span>
                <span className="hero-stat-label">Formatos suportados</span>
              </div>
              <div className="hero-stat-divider" />
              <div className="hero-stat">
                <span className="hero-stat-number">10+</span>
                <span className="hero-stat-label">Indicadores analisados</span>
              </div>
              <div className="hero-stat-divider" />
              <div className="hero-stat">
                <span className="hero-stat-number">~30s</span>
                <span className="hero-stat-label">Tempo medio</span>
              </div>
            </div>
          </div>

          <div className="hero-input">
            <form onSubmit={handleSubmit} className="input-form">
              <div
                className={`input-card${dragging ? " dragging" : ""}`}
                onDrop={handleDrop}
                onDragOver={handleDragOver}
                onDragLeave={handleDragLeave}
              >
                <input
                  ref={fileInputRef}
                  type="file"
                  accept="image/*,.pdf,.txt,.doc,.docx,.mp4,.mpeg,.mov,.avi,.webm,.mkv,.mp3,.wav,.aac,.ogg,.flac,.m4a"
                  onChange={handleFileChange}
                  hidden
                />

                <div className="input-area">
                  {fileName ? (
                    <div className="file-preview">
                      <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
                        <path d="M14 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V8z" />
                        <polyline points="14 2 14 8 20 8" />
                      </svg>
                      <span className="file-name">{fileName}</span>
                      <button type="button" className="file-remove" onClick={clearFile}>
                        <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5" strokeLinecap="round" strokeLinejoin="round">
                          <path d="M18 6 6 18" />
                          <path d="m6 6 12 12" />
                        </svg>
                      </button>
                    </div>
                  ) : (
                    <>
                      {!content && (
                        <div className="input-placeholder">
                          <span>Insira seu conteudo ou</span>
                          <button
                            type="button"
                            className="upload-link"
                            onClick={() => {
                              clearFile();
                              fileInputRef.current.click();
                            }}
                          >
                            <svg width="15" height="15" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
                              <path d="M21 15v4a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2v-4" />
                              <polyline points="17 8 12 3 7 8" />
                              <line x1="12" y1="3" x2="12" y2="15" />
                            </svg>
                            Enviar arquivos
                          </button>
                        </div>
                      )}
                      <textarea
                        value={content}
                        onChange={(e) => {
                          if (e.target.value.length <= 10000) setContent(e.target.value);
                        }}
                        onKeyDown={handleKeyDown}
                      />
                    </>
                  )}
                </div>

                <div className="supported-formats">
                  <span className="formats-label">Formatos suportados:</span>
                  <span className="format-tag">Texto</span>
                  <span className="format-tag">Imagem</span>
                  <span className="format-tag">Audio</span>
                  <span className="format-tag">Video</span>
                  <span className="format-tag">PDF</span>
                </div>

                <div className="input-card-footer">
                  <span className="input-char-count">
                    {content.length.toLocaleString("pt-BR")}/10.000 caracteres
                  </span>
                  <button type="submit" className="submit-btn" disabled={!hasContent}>
                    <span>Analisar</span>
                    <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2.5" strokeLinecap="round" strokeLinejoin="round">
                      <path d="M5 12h14" />
                      <path d="m12 5 7 7-7 7" />
                    </svg>
                  </button>
                </div>
              </div>
            </form>
          </div>
        </div>
      </section>

      {/* ===== HOW IT WORKS ===== */}
      <section className="section steps-section" id="como-funciona">
        <div className="section-container">
          <div className="section-header">
            <span className="section-tag">Como funciona</span>
            <h2 className="section-title">Simples e poderoso</h2>
            <p className="section-desc">Tres passos para verificar a autenticidade de qualquer conteudo.</p>
          </div>

          <div className="steps-grid">
            {STEPS.map((step) => (
              <div key={step.number} className="step-card">
                <div className="step-number">{step.number}</div>
                <h3 className="step-title">{step.title}</h3>
                <p className="step-desc">{step.desc}</p>
              </div>
            ))}
          </div>
        </div>
      </section>

      {/* ===== SUPPORTED FORMATS ===== */}
      <section className="section formats-section" id="formatos">
        <div className="section-container">
          <div className="section-header">
            <span className="section-tag">Formatos</span>
            <h2 className="section-title">Multi-formato</h2>
            <p className="section-desc">Suporte completo para os principais tipos de midia digital.</p>
          </div>

          <div className="formats-grid">
            {FORMAT_CARDS.map((f) => (
              <div key={f.title} className="format-card">
                <div className="format-icon">{f.icon}</div>
                <h3 className="format-title">{f.title}</h3>
                <p className="format-desc">{f.desc}</p>
                <span className="format-formats">{f.formats}</span>
              </div>
            ))}
          </div>
        </div>
      </section>

      {/* ===== ABOUT ===== */}
      <section className="section about-section" id="sobre">
        <div className="section-container">
          <div className="about-content">
            <div className="about-text">
              <span className="section-tag">Sobre</span>
              <h2 className="section-title">Por que Verasium?</h2>
              <p className="about-paragraph">
                Com o avanco acelerado da inteligencia artificial generativa, distinguir conteudo autentico
                de conteudo sintetico tornou-se um dos maiores desafios da era digital.
              </p>
              <p className="about-paragraph">
                O Verasium utiliza o modelo <strong>Gemini</strong> do Google para analisar conteudo em multiplas
                dimensoes — padroes linguisticos, consistencia visual, artefatos sonoros e estruturais — entregando
                resultados transparentes com indicadores detalhados.
              </p>
              <div className="about-features">
                <div className="about-feature">
                  <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
                    <path d="M22 11.08V12a10 10 0 1 1-5.93-9.14" />
                    <polyline points="22 4 12 14.01 9 11.01" />
                  </svg>
                  <span>Analise multi-modal (texto, imagem, video, audio, PDF)</span>
                </div>
                <div className="about-feature">
                  <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
                    <path d="M22 11.08V12a10 10 0 1 1-5.93-9.14" />
                    <polyline points="22 4 12 14.01 9 11.01" />
                  </svg>
                  <span>Indicadores detalhados com nivel de significancia</span>
                </div>
                <div className="about-feature">
                  <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
                    <path d="M22 11.08V12a10 10 0 1 1-5.93-9.14" />
                    <polyline points="22 4 12 14.01 9 11.01" />
                  </svg>
                  <span>Score de confianca transparente de 0 a 100%</span>
                </div>
                <div className="about-feature">
                  <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
                    <path d="M22 11.08V12a10 10 0 1 1-5.93-9.14" />
                    <polyline points="22 4 12 14.01 9 11.01" />
                  </svg>
                  <span>Gratuito e open-source</span>
                </div>
              </div>
            </div>
          </div>
        </div>
      </section>

      {/* ===== FOOTER ===== */}
      <footer className="footer">
        <div className="footer-container">
          <div className="footer-brand">
            <img src={logo} alt="Verasium" className="footer-logo" draggable={false} />
          </div>
          <div className="footer-bottom">
            <p>&copy; {new Date().getFullYear()} Verasium. Projeto open-source.</p>
          </div>
        </div>
      </footer>
    </div>
  );
}
