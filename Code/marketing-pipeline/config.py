"""
Configuration loader for the LinkBT Marketing Pipeline.
Loads settings from .env file and provides defaults.
"""

import os
from pathlib import Path
from dotenv import load_dotenv

# ── Load .env ────────────────────────────────────────
BASE_DIR = Path(__file__).resolve().parent
load_dotenv(BASE_DIR / ".env")

# ── LLM Provider & Keys ──────────────────────────────
LLM_PROVIDER = os.getenv("LLM_PROVIDER", "auto")  # "gemini", "ollama", or "auto"
GEMINI_API_KEY = os.getenv("GEMINI_API_KEY", "")
OPENAI_API_KEY = os.getenv("OPENAI_API_KEY", "")

# ── Ollama Settings ──────────────────────────────────
OLLAMA_MODEL = os.getenv("OLLAMA_MODEL", "llama3.2:3b")
OLLAMA_URL = os.getenv("OLLAMA_URL", "http://localhost:11434")
OLLAMA_GENERATE_URL = f"{OLLAMA_URL}/api/generate"

# ── Telegram ─────────────────────────────────────────
TELEGRAM_BOT_TOKEN = os.getenv("TELEGRAM_BOT_TOKEN", "")
TELEGRAM_CHAT_ID = os.getenv("TELEGRAM_CHAT_ID", "")

# ── LinkedIn ─────────────────────────────────────────
LINKEDIN_ACCESS_TOKEN = os.getenv("LINKEDIN_ACCESS_TOKEN", "")
LINKEDIN_AUTHOR_URN = os.getenv("LINKEDIN_AUTHOR_URN", "")

# ── Schedule ─────────────────────────────────────────
DAILY_POST_TIME = os.getenv("DAILY_POST_TIME", "09:00")

# ── Paths ────────────────────────────────────────────
OUTPUT_DIR = BASE_DIR / os.getenv("OUTPUT_DIR", "output")
ASSETS_DIR = BASE_DIR / "assets"
FONTS_DIR = ASSETS_DIR / "fonts"
PAST_POSTS_FILE = BASE_DIR / "past_posts.json"

# ── Ensure directories exist ────────────────────────
OUTPUT_DIR.mkdir(parents=True, exist_ok=True)
FONTS_DIR.mkdir(parents=True, exist_ok=True)
