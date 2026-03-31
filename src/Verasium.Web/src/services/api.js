const API_URL = import.meta.env.VITE_API_URL || "http://localhost:5029/api/analysis";

const MAX_FILE_SIZES = {
  ".mp4": 25, ".mpeg": 25, ".mov": 25, ".avi": 25, ".webm": 25, ".mkv": 25,
  ".mp3": 10, ".wav": 10, ".aac": 10, ".ogg": 10, ".flac": 10, ".m4a": 10,
  ".pdf": 20,
};
const DEFAULT_MAX_MB = 30;

export async function analyzeContent(content) {
  const response = await fetch(API_URL, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ content }),
  });

  if (!response.ok) {
    const error = await response.json().catch(() => null);
    throw new Error(error?.error || "Estamos com um problema no momento. Por favor, tente novamente.");
  }

  return response.json();
}

export async function analyzeFile(file) {
  const ext = "." + file.name.split(".").pop().toLowerCase();
  const maxMb = MAX_FILE_SIZES[ext] || DEFAULT_MAX_MB;
  const maxBytes = maxMb * 1_000_000;

  if (file.size > maxBytes) {
    throw new Error(`Arquivo muito grande. O limite para este tipo é ${maxMb}MB.`);
  }

  const formData = new FormData();
  formData.append("file", file);

  const response = await fetch(`${API_URL}/upload`, {
    method: "POST",
    body: formData,
  });

  if (!response.ok) {
    const error = await response.json().catch(() => null);
    throw new Error(error?.error || "Estamos com um problema no momento. Por favor, tente novamente.");
  }

  return response.json();
}
