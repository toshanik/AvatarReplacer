const API_URL = "http://localhost:12001/api/"; // укажи адрес ASP.NET микросервиса

document.getElementById("generateBtn").addEventListener("click", async () => {
  const displayName = document.getElementById("displayName").value;
  const prompt = document.getElementById("prompt").value;

  const res = await fetch(`${API_URL}/generation/by-displayname`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ displayName, prompt, size: "512x512" })
  });

  const data = await res.json();
  alert("✅ Сгенерировано: " + data.generatedImageUrl);
});

document.getElementById("batchBtn").addEventListener("click", async () => {
  const prompt = document.getElementById("batchPrompt").value;

  const res = await fetch(`${API_URL}/generation/batch`, {
    method: "POST",
    headers: { "Content-Type": "application/json" },
    body: JSON.stringify({ prompt, applyToAll: true })
  });

  const data = await res.json();
  alert("✅ Массовая генерация запущена (" + data.requestId + ")");
});

document.getElementById("loadHistoryBtn").addEventListener("click", async () => {
  const res = await fetch(`${API_URL}/images/history`);
  const history = await res.json();

  const container = document.getElementById("historyContainer");
  container.innerHTML = "";
  history.forEach(item => {
    const img = document.createElement("img");
    img.src = item.generatedImageUrl;
    container.appendChild(img);
  });
});
