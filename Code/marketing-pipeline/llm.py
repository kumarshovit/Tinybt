"""
Multi-Provider LLM Wrapper (Google Gemini API + Ollama + Local Synthesis Engine).
Provides world-class marketing strategy and image generation prompt synthesis.
"""

import sys
if hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8")
if hasattr(sys.stderr, "reconfigure"):
    sys.stderr.reconfigure(encoding="utf-8")

import json
import requests
from config import GEMINI_API_KEY, OLLAMA_GENERATE_URL, OLLAMA_MODEL, LLM_PROVIDER


def _generate_gemini(prompt: str, system: str = "") -> str:
    """Generate text using Google Gemini API (Free tier: tries 2.0-flash then 1.5-flash)."""
    if not GEMINI_API_KEY:
        return ""

    headers = {"Content-Type": "application/json"}
    contents = []
    if system:
        contents.append({"role": "user", "parts": [{"text": f"SYSTEM INSTRUCTION: {system}"}]})
        contents.append({"role": "model", "parts": [{"text": "Understood. Ready."}]})
    contents.append({"role": "user", "parts": [{"text": prompt}]})

    payload = {
        "contents": contents,
        "generationConfig": {
            "temperature": 0.8,
            "topP": 0.95,
            "maxOutputTokens": 2048,
        }
    }

    # Try gemini-1.5-flash (highest free quota) then gemini-2.0-flash
    for model_name in ["gemini-1.5-flash", "gemini-2.0-flash"]:
        url = f"https://generativelanguage.googleapis.com/v1beta/models/{model_name}:generateContent?key={GEMINI_API_KEY}"
        try:
            resp = requests.post(url, headers=headers, json=payload, timeout=20)
            if resp.status_code == 200:
                data = resp.json()
                candidates = data.get("candidates", [])
                if candidates:
                    parts = candidates[0].get("content", {}).get("parts", [])
                    if parts:
                        return parts[0].get("text", "").strip()
        except Exception:
            pass

    return ""


def check_connection() -> bool:
    """Verify available LLM provider."""
    if GEMINI_API_KEY:
        return True
    try:
        resp = requests.get(f"{OLLAMA_GENERATE_URL.rsplit('/api', 1)[0]}/api/tags", timeout=1)
        return resp.status_code == 200
    except Exception:
        return False


def generate(prompt: str, system: str = "", as_json: bool = False, timeout: int = 15) -> str:
    """
    Generate text using Gemini API if configured, otherwise Ollama or dynamic fallback.
    """
    # Provider 1: Gemini API
    if GEMINI_API_KEY and (LLM_PROVIDER in ("gemini", "auto")):
        res = _generate_gemini(prompt, system=system)
        if res:
            return res

    # Provider 2: Local Ollama
    payload = {
        "model": OLLAMA_MODEL,
        "prompt": prompt,
        "stream": False,
        "options": {
            "temperature": 0.8,
            "top_p": 0.9,
            "num_predict": 1024,
        },
    }
    if system:
        payload["system"] = system
    if as_json:
        payload["format"] = "json"

    try:
        resp = requests.post(OLLAMA_GENERATE_URL, json=payload, timeout=timeout)
        if resp.status_code == 200:
            return resp.json().get("response", "")
    except Exception:
        pass

    return ""


def generate_json(prompt: str, system: str = "") -> dict:
    """
    Generate a response and parse it as JSON with fallback safety.
    """
    raw = generate(prompt, system=system, as_json=True, timeout=15)
    if not raw:
        return {}

    try:
        return json.loads(raw)
    except json.JSONDecodeError:
        pass

    if "```json" in raw:
        try:
            start = raw.index("```json") + 7
            end = raw.index("```", start)
            return json.loads(raw[start:end].strip())
        except Exception:
            pass

    brace_start = raw.find("{")
    brace_end = raw.rfind("}") + 1
    if brace_start != -1 and brace_end > brace_start:
        try:
            return json.loads(raw[brace_start:brace_end])
        except Exception:
            pass

    return {}
