import { useState, useRef } from "react";
import logo from "../assets/logo.png";

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
    <div className="screen input-screen">
      <div className="input-branding">
        <img
          src={logo}
          alt="Verasium"
          className="logo"
          draggable={false}
          onDragStart={(e) => e.preventDefault()}
        />
        <p className="subtitle">
          Identifique conteudo gerado por IA com precisao
        </p>
      </div>

      <div className="input-content">
        <form onSubmit={handleSubmit} className="input-form">
          <div className="input-card">
            <div className="input-card-glow" />
            <div
              className={`input-card-inner${dragging ? " dragging" : ""}`}
              onDrop={handleDrop}
              onDragOver={handleDragOver}
              onDragLeave={handleDragLeave}
            >
              <div className="input-card-header">
                <span className="input-card-label">Conteúdo para análise</span>
              </div>

              {fileName ? (
                <div className="file-preview">
                  <svg
                    width="18"
                    height="18"
                    viewBox="0 0 24 24"
                    fill="none"
                    stroke="currentColor"
                    strokeWidth="2"
                    strokeLinecap="round"
                    strokeLinejoin="round"
                  >
                    <path d="M14 2H6a2 2 0 0 0-2 2v16a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2V8z" />
                    <polyline points="14 2 14 8 20 8" />
                  </svg>
                  <span className="file-name">{fileName}</span>
                  <button
                    type="button"
                    className="file-remove"
                    onClick={clearFile}
                  >
                    <svg
                      width="14"
                      height="14"
                      viewBox="0 0 24 24"
                      fill="none"
                      stroke="currentColor"
                      strokeWidth="2.5"
                      strokeLinecap="round"
                      strokeLinejoin="round"
                    >
                      <path d="M18 6 6 18" />
                      <path d="m6 6 12 12" />
                    </svg>
                  </button>
                </div>
              ) : (
                <textarea
                  value={content}
                  onChange={(e) => setContent(e.target.value)}
                  onKeyDown={handleKeyDown}
                  placeholder="Cole aqui o conteúdo que deseja analisar (textos, fotos, PDFs...)"
                  autoFocus
                />
              )}

              <div className="input-card-footer">
                <input
                  ref={fileInputRef}
                  type="file"
                  accept="image/*,.pdf,.txt,.doc,.docx"
                  onChange={handleFileChange}
                  hidden
                />
                <button
                  type="button"
                  className="upload-link"
                  onClick={() => {
                    clearFile();
                    fileInputRef.current.click();
                  }}
                >
                  <svg
                    width="15"
                    height="15"
                    viewBox="0 0 24 24"
                    fill="none"
                    stroke="currentColor"
                    strokeWidth="2"
                    strokeLinecap="round"
                    strokeLinejoin="round"
                  >
                    <path d="M21 15v4a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2v-4" />
                    <polyline points="17 8 12 3 7 8" />
                    <line x1="12" y1="3" x2="12" y2="15" />
                  </svg>
                  Enviar arquivo
                </button>
                <button
                  type="submit"
                  className="submit-btn"
                  disabled={!hasContent}
                >
                  <span>Analisar</span>
                  <svg
                    width="16"
                    height="16"
                    viewBox="0 0 24 24"
                    fill="none"
                    stroke="currentColor"
                    strokeWidth="2.5"
                    strokeLinecap="round"
                    strokeLinejoin="round"
                  >
                    <path d="M5 12h14" />
                    <path d="m12 5 7 7-7 7" />
                  </svg>
                </button>
              </div>
            </div>
          </div>
        </form>
      </div>
    </div>
  );
}
