import { useState, useEffect } from "react";
import InputScreen from "./components/InputScreen";
import LoadingScreen from "./components/LoadingScreen";
import ResultScreen from "./components/ResultScreen";
import DesignSystemScreen from "./components/design-system/DesignSystemScreen";
import { analyzeContent, analyzeFile } from "./services/api";
import "./App.css";

function App() {
  const [screen, setScreen] = useState("input");
  const [result, setResult] = useState(null);
  const [darkMode, setDarkMode] = useState(() => {
    const saved = localStorage.getItem("verasium-theme");
    if (saved) return saved === "dark";
    return window.matchMedia("(prefers-color-scheme: dark)").matches;
  });

  useEffect(() => {
    document.documentElement.setAttribute("data-theme", darkMode ? "dark" : "light");
    localStorage.setItem("verasium-theme", darkMode ? "dark" : "light");
  }, [darkMode]);

  const handleSubmit = async (content) => {
    setScreen("loading");

    try {
      const data = await analyzeContent(content);
      setResult(data);
      setScreen("result");
    } catch (err) {
      setResult({
        isSuccessful: false,
        errorMessage: err.message,
      });
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
      setResult({
        isSuccessful: false,
        errorMessage: err.message,
      });
      setScreen("result");
    }
  };

  const handleReset = () => {
    setResult(null);
    setScreen("input");
  };

  return (
    <div className="app">
      {screen === "input" && (
        <InputScreen
          onSubmit={handleSubmit}
          onFileUpload={handleFileUpload}
          darkMode={darkMode}
          onToggleTheme={() => setDarkMode(!darkMode)}
          onNavigateDesignSystem={() => setScreen("design-system")}
        />
      )}
      {screen === "design-system" && (
        <DesignSystemScreen
          darkMode={darkMode}
          onToggleTheme={() => setDarkMode(!darkMode)}
          onBack={() => setScreen("input")}
        />
      )}
      {screen === "loading" && <LoadingScreen darkMode={darkMode} onToggleTheme={() => setDarkMode(!darkMode)} />}
      {screen === "result" && (
        <ResultScreen result={result} onReset={handleReset} darkMode={darkMode} onToggleTheme={() => setDarkMode(!darkMode)} />
      )}
    </div>
  );
}

export default App;
