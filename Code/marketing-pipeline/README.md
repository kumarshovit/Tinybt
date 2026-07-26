# LinkBT Marketing Pipeline — Prompt Generation Engine

Automated AI Marketing Strategy & AI Image Prompt Generator for [LinkBT](https://link.bt).  
Analyzes local Git history, React components, and .NET backend code on every run to generate **extremely detailed (1000–3000 words) AI Image Generation Prompts** and high-converting LinkedIn captions.

---

## 🚀 Key Paradigm Shift: Prompt Generation Only

The pipeline **does NOT render images directly**.  
Instead, each run produces a single production artifact: **`output/marketing-prompt.md`**.

### Workflow:
1. 🔍 **Analyzes Local Workspace & Git**: Scans React pages, UI components, .NET endpoints, and recent Git commit logs (e.g. Google Safe Browsing, Captcha, IP tracking) to discover live product features automatically.
2. 🧠 **Generates Unique Campaign Strategy**: Rotates weekly campaign themes (Analytics, QR Codes, Developer API, Custom Domains, Security, Growth, etc.) using Ollama LLM, ensuring 100% non-repeating campaigns based on past history (`past_posts.json`).
3. 🎨 **Synthesizes 1000–3000 Word AI Image Prompt**: Constructs a hyper-detailed studio art direction prompt for ChatGPT or Gemini, specifying layout, typography, 3D icons, color palette (Stripe/Linear/Vercel SaaS tier aesthetics), and explicit instructions for embedding 2–5 attached LinkBT app screenshots.
4. ✍️ **Writes LinkedIn Caption & Copy**: Generates scroll-stopping LinkedIn post text, call-to-action, and hashtags.
5. 📬 **Delivers Artifact to Telegram**: Sends `marketing-prompt.md` to Telegram for review.

---

## 📁 File Structure

```
marketing-pipeline/
├── bot.py                 # Telegram bot & daily job scheduler
├── pipeline.py            # Main orchestrator producing marketing-prompt.md
├── codebase_analyzer.py   # Scans Git history, React components & .NET backend
├── prompt_generator.py    # Synthesizes 1000-3000 word AI Image Generation Prompt
├── idea_generator.py      # Generates non-repeating campaign concepts (Ollama)
├── caption_generator.py   # Generates LinkedIn caption & post text (Ollama)
├── brand.py               # Live brand context & past campaign tracker
├── llm.py                 # Ollama API wrapper with retry logic
├── config.py              # Environment configuration loader
├── requirements.txt       # Python dependencies
├── past_posts.json        # History log preventing repeated campaigns
└── output/
    └── marketing-prompt.md # Single generated artifact per run
```

---

## 🛠️ Quick Start

### 1. Requirements
- Python 3.10+
- [Ollama](https://ollama.com) running locally with model `llama3.2:3b` or `llama3.2:1b`.

### 2. Test Standalone Execution
```powershell
cd d:\OneDrive\Desktop\Intellisoft\TinyBt\Code\marketing-pipeline
python pipeline.py
```
This runs codebase analysis and produces `output/marketing-prompt.md`.

### 3. Test Telegram Bot
```powershell
python bot.py
```
Send `/run` to your bot on Telegram to generate a campaign and receive `marketing-prompt.md`.

---

## 🎨 How to Use the Output
1. Open `output/marketing-prompt.md`.
2. Copy the **AI IMAGE GENERATION PROMPT** section into ChatGPT (DALL-E 3 / GPT-4o) or Gemini.
3. Attach 2–5 actual screenshots of LinkBT (Dashboard, Analytics, Link Generator, Security Settings).
4. Let ChatGPT/Gemini synthesize the final agency-level marketing poster using your real screenshots!
