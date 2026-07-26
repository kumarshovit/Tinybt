"""
Main Pipeline Orchestrator for LinkBT Marketing Engine.

Flow:
  1. Analyze codebase (Git history, React components, .NET backend)
  2. Generate unique campaign strategy (Ollama LLM)
  3. Synthesize hyper-detailed AI Image Generation Prompt (1000-3000 words)
  4. Generate high-converting LinkedIn caption, CTA & hashtags
  5. Write single output artifact: output/marketing-prompt.md
"""

import sys
if hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8")
if hasattr(sys.stderr, "reconfigure"):
    sys.stderr.reconfigure(encoding="utf-8")

import json
from datetime import datetime
from pathlib import Path
from config import OUTPUT_DIR, PAST_POSTS_FILE
from codebase_analyzer import get_complete_codebase_context
from brand import save_post
from idea_generator import generate_idea
from prompt_generator import generate_image_prompt
from caption_generator import generate_caption


def run_pipeline() -> dict:
    """
    Execute the upgraded marketing pipeline.

    Returns:
        dict with all generated marketing campaign artifacts.
    """
    today = datetime.now().strftime("%Y-%m-%d")
    print(f"\n{'='*60}")
    print(f"  🚀 LinkBT Codebase-Driven Marketing Engine — {today}")
    print(f"{'='*60}\n")

    # ── Step 1: Codebase Analysis ────────────────────
    print("🔍 [1/4] Analyzing workspace codebase & Git history...")
    codebase_ctx = get_complete_codebase_context()
    print(f"   📂 Pages detected: {len(codebase_ctx['pages'])} ({', '.join(codebase_ctx['pages'][:5])}...)")
    print(f"   ✨ Newly detected features: {len(codebase_ctx['newly_detected_features'])}")
    for nd in codebase_ctx['newly_detected_features']:
        print(f"      - {nd['name']}")

    # ── Step 2: Generate Campaign Strategy ───────────
    print("\n🧠 [2/4] Generating unique campaign concept...")
    idea = generate_idea()
    print(f"   🎯 Theme: {idea.get('theme', 'N/A')}")
    print(f"   💡 Idea: {idea.get('idea', 'N/A')}")
    print(f"   📝 Headline: {idea.get('headline', 'N/A')}")
    print(f"   ✨ Feature Focus: {idea.get('feature_focus', 'N/A')}")

    # ── Step 3: Generate AI Image Generation Prompt ──
    print("\n🎨 [3/4] Synthesizing 1000-3000 word AI Image Generation Prompt...")
    image_prompt = generate_image_prompt(idea, codebase_ctx)
    word_count = len(image_prompt.split())
    print(f"   📄 AI Image Prompt generated ({word_count} words)")

    # ── Step 4: Generate LinkedIn Caption ────────────
    print("\n✍️  [4/4] Generating LinkedIn caption & post copy...")
    caption_data = generate_caption(idea)
    print(f"   📄 Caption length: {len(caption_data.get('caption', ''))} chars")
    print(f"   #️⃣  Hashtags: {caption_data.get('hashtags', 'N/A')}")

    # ── Step 5: Format Final Single Artifact: marketing-prompt.md ──
    campaign_name = f"LinkBT Campaign — {idea.get('theme', 'General')} ({today})"

    new_features_md = "\n".join([
        f"- **{nf['name']}**: {nf['description']} *(Source: {nf['source']})*"
        for nf in codebase_ctx.get("newly_detected_features", [])
    ]) if codebase_ctx.get("newly_detected_features") else "- Continuous deployment performance & security upgrades"

    core_features_md = "\n".join([
        f"- **{cf['name']}**: {cf['description']}"
        for cf in codebase_ctx.get("core_features", [])
    ])

    marketing_prompt_content = f"""# {campaign_name}

## 📊 Campaign Strategy & Overview
- **Campaign Name**: {campaign_name}
- **Campaign Strategy Theme**: {idea.get('theme', 'N/A')}
- **Marketing Goal**: Drive signups and brand awareness on LinkedIn by showcasing LinkBT's enterprise feature suite.
- **Target Audience**: {idea.get('target_audience', 'Digital Marketers, Content Creators, Developers')}
- **Customer Pain Points**: {idea.get('pain_points', 'N/A')}
- **Emotional Trigger**: {idea.get('emotional_trigger', 'N/A')}
- **Marketing Angle**: {idea.get('angle', 'N/A')}
- **Value Proposition**: 100% Free, custom short URLs, real-time analytics, Google Safe Browsing, and link tagging.

---

## 🚀 Product Features
### Newly Released Features (Automatically Detected from Local Codebase & Git)
{new_features_md}

### Core Product Features
{core_features_md}

---

## 📱 Marketing Copy & LinkedIn Caption

### Headline & Supporting Copy
- **Headline**: {idea.get('headline', '')}
- **Supporting Copy**: {idea.get('subtitle', '')}
- **CTA**: {caption_data.get('cta', '')}

### Full LinkedIn Post
```text
{caption_data.get('full_post', '')}
```

- **Hashtags**: {caption_data.get('hashtags', '')}

---

## 🎨 Complete AI Image Generation Prompt (For ChatGPT / Gemini)

{image_prompt}
""".strip()

    # Save to output/marketing-prompt.md
    output_file = OUTPUT_DIR / "marketing-prompt.md"
    with open(output_file, "w", encoding="utf-8") as f:
        f.write(marketing_prompt_content)

    print(f"\n{'='*60}")
    print(f"  ✅ [SUCCESS] Pipeline Complete! Output artifact generated.")
    print(f"{'='*60}")
    print(f"📁 Output Artifact: {output_file}")
    print(f"📝 Total Artifact Word Count: {len(marketing_prompt_content.split())} words")

    result = {
        "date": today,
        "campaign_name": campaign_name,
        "theme": idea.get("theme", ""),
        "idea": idea,
        "caption": caption_data.get("caption", ""),
        "hashtags": caption_data.get("hashtags", ""),
        "cta": caption_data.get("cta", ""),
        "full_post": caption_data.get("full_post", ""),
        "image_prompt": image_prompt,
        "artifact_path": str(output_file),
        "status": "pending_approval",
    }

    # Record to history
    save_post({
        "date": today,
        "campaign_name": campaign_name,
        "theme": idea.get("theme", ""),
        "idea": idea.get("idea", ""),
        "headline": idea.get("headline", ""),
        "angle": idea.get("angle", ""),
        "status": "pending_approval",
    })

    return result


def run_standalone():
    """Run pipeline standalone without Telegram bot for testing."""
    from llm import check_connection

    print("🔍 Checking Ollama connection...")
    if not check_connection():
        print("\n❌ Ollama is not running or model not found!")
        print("   1. Install Ollama:  https://ollama.com")
        print("   2. Start it:        ollama serve")
        print("   3. Pull model:      ollama pull llama3.2:3b")
        return None

    print("✅ Ollama is ready!\n")
    return run_pipeline()


if __name__ == "__main__":
    run_standalone()
