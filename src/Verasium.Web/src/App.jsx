import { useState } from "react";
import InputScreen from "./components/InputScreen";
import LoadingScreen from "./components/LoadingScreen";
import ResultScreen from "./components/ResultScreen";
import { analyzeContent, analyzeFile } from "./services/api";
import "./App.css";

function App() {
  const [screen, setScreen] = useState("input");
  const [result, setResult] = useState(null);

  const handleSubmit = async (content) => {
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
{screen === "loading" && <LoadingScreen />}
      {screen === "result" && (
        <ResultScreen result={result} onReset={handleReset} />
      )}
    </div>
  );
}

export default App;
