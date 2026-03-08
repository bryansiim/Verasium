const API_URL = "http://localhost:5029/api/analysis";

export async function analyzeContent(content) {
  const response = await fetch(API_URL, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ content }),
  });

  if (!response.ok) {
    const error = await response.json().catch(() => null);
    throw new Error(error?.error || "Erro ao conectar com o servidor.");
  }

  return response.json();
}

export async function analyzeFile(file) {
  const formData = new FormData();
  formData.append("file", file);

  const response = await fetch(`${API_URL}/upload`, {
    method: "POST",
    body: formData,
  });

  if (!response.ok) {
    const error = await response.json().catch(() => null);
    throw new Error(error?.error || "Erro ao conectar com o servidor.");
  }

  return response.json();
}
