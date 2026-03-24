import { useState, useEffect } from "react";
import Navbar from "./Navbar";
import iconLight from "../assets/icon.png";
import iconDark from "../assets/icon-dark.png";

const messages = [
  "Analisando metadados",
  "Verificando padroes de escrita",
  "Analisando estrutura do conteudo",
  "Comparando padroes linguisticos",
  "Verificando consistencia semantica",
  "Processando resultados",
];

export default function LoadingScreen({ darkMode, onToggleTheme }) {
  const [messageIndex, setMessageIndex] = useState(0);

  useEffect(() => {
    const interval = setInterval(() => {
      setMessageIndex((prev) => (prev + 1) % messages.length);
    }, 2800);
    return () => clearInterval(interval);
  }, []);

  return (
    <div className="screen-page">
      <Navbar darkMode={darkMode} onToggleTheme={onToggleTheme} />
      <div className="screen-centered">
        <div className="loading-content">
          <img src={darkMode ? iconDark : iconLight} alt="" className="loading-icon" draggable={false} />
          <div className="loading-messages">
            <p className="loading-message" key={messageIndex}>
              {messages[messageIndex]}
            </p>
            <div className="loading-dots">
              <span />
              <span />
              <span />
            </div>
          </div>
          <div className="loading-progress">
            <div className="loading-progress-bar" />
          </div>
        </div>
      </div>
    </div>
  );
}
