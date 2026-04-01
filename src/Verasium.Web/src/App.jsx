import { useState } from "react";
import InputScreen from "./components/InputScreen";
import LoadingScreen from "./components/LoadingScreen";
import ResultScreen from "./components/ResultScreen";
import { analyzeContent, analyzeFile } from "./services/api";
import "./App.css";

const FILE_TYPE_MAP = {
  jpg: "image", jpeg: "image", png: "image", gif: "image", webp: "image", bmp: "image", svg: "image",
  pdf: "pdf",
  mp4: "video", mov: "video", avi: "video", mkv: "video", webm: "video",
  mp3: "audio", wav: "audio", ogg: "audio", flac: "audio", m4a: "audio",
};

function getFileType(name) {
  if (!name) return "text";
  const ext = name.split(".").pop().toLowerCase();
  return FILE_TYPE_MAP[ext] || "text";
}

function App() {
  const [screen, setScreen] = useState("input");
  const [result, setResult] = useState(null);
  const [contentType, setContentType] = useState("text");

  const handleSubmit = async (content) => {
    setContentType("text");
    setScreen("loading");

    try {
      const data = await analyzeContent(content);
      setResult(data);
      setScreen("result");
    } catch (err) {
      const msg = err.message === "Failed to fetch"
        ? "Não foi possível conectar ao servidor. Verifique sua conexão e tente novamente."
        : err.message || "Estamos com um problema no momento. Por favor, tente novamente.";
      setResult({ isSuccessful: false, errorMessage: msg });
      setScreen("result");
    }
  };

  const handleFileUpload = async (file) => {
    setContentType(getFileType(file.name));
    setScreen("loading");

    try {
      const data = await analyzeFile(file);
      setResult(data);
      setScreen("result");
    } catch (err) {
      const msg = err.message === "Failed to fetch"
        ? "Não foi possível conectar ao servidor. Verifique sua conexão e tente novamente."
        : err.message || "Estamos com um problema no momento. Por favor, tente novamente.";
      setResult({ isSuccessful: false, errorMessage: msg });
      setScreen("result");
    }
  };

  const handleReset = () => {
    setResult(null);
    setScreen("input");
    window.scrollTo(0, 0);
  };

  return (
    <div className="app">
      {screen === "input" && (
        <InputScreen
          onSubmit={handleSubmit}
          onFileUpload={handleFileUpload}
        />
      )}
{screen === "loading" && <LoadingScreen contentType={contentType} />}
      {screen === "result" && (
        <ResultScreen result={result} onReset={handleReset} />
      )}
    </div>
  );
}

export default App;
