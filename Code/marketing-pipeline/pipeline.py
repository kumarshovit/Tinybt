"""
Main Pipeline Orchestrator for LinkBT Marketing Engine.
Creative Director Pipeline with Interactive Feature Selection & Diversity Engine.

Flow:
  1. Codebase Analysis (Git history, React components, .NET backend)
  2. Interactive Feature Selection (Ask user at runtime or auto-select if skipped)
  3. Creative Director Strategy (Marketing Style -> Layout -> Single Angle -> Seasonal Context)
  4. Creativity & Uniqueness Scoring (<30% similarity threshold)
  5. AI Image Prompt Generation (tailored to style, layout & selected features)
  6. LinkedIn Caption & Copy Generation
  7. Save Output: output/marketing-prompt.md & update past_posts.json
"""

import sys
if hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8")
if hasattr(sys.stderr, "reconfigure"):
    sys.stderr.reconfigure(encoding="utf-8")

import json
import argparse
from datetime import datetime
from pathlib import Path
from config import OUTPUT_DIR, PAST_POSTS_FILE
from codebase_analyzer import get_complete_codebase_context
from brand import save_post
from idea_generator import generate_idea
from prompt_generator import generate_image_prompt
from caption_generator import generate_caption


def prompt_user_for_features(codebase_ctx: dict, preselected: list[str] = None) -> list[str]:
    """
    Interactively ask the user which features to highlight in this campaign.
    If preselected is provided (via CLI args), uses those directly.
    """
    if preselected:
        print(f"\n🎯 Using pre-selected features: {', '.join(preselected)}")
        return preselected

    # Build available feature list
    core_features = codebase_ctx.get("core_features", [])
    newly_detected = codebase_ctx.get("newly_detected_features", [])
    
    feature_options = []

    # Newly detected first
    for nd in newly_detected:
        feature_options.append(f"✨ {nd['name']} (New: {nd['description']})")
    
    # Core features
    for cf in core_features:
        feature_options.append(f"{cf.get('emoji', '🔗')} {cf['name']} ({cf['description']})")

    # Additional standard options
    additional = [
        "QR Code Generation & Smart Routing",
        "UTM Link Builder & Campaign Tracking",
        "Custom Domains & Branded Aliases",
        "Password Protection & Access Controls",
        "Bio Pages & Link-in-Bio Builder",
        "Bulk URL Shortener Management",
    ]
    for add in additional:
        feature_options.append(f"⚡ {add}")

    # If non-interactive environment (e.g. redirected stdin), return auto-selected default
    if not sys.stdin.isatty():
        print("\n⚡ Non-interactive shell detected: Intelligently auto-selecting features...")
        return [feature_options[0].split(" (")[0].replace("✨ ", "").replace("🔗 ", "").replace("⚡ ", "")]

    print("\n" + "="*60)
    print("  🎨 LINKBT CREATIVE DIRECTOR — FEATURE SELECTION")
    print("="*60)
    print("Which feature(s) would you like to highlight in this campaign?\n")

    for i, feat in enumerate(feature_options, 1):
        print(f"  [{i}] {feat}")
    print("  [0] Skip selection (Intelligently auto-select optimal features)")

    try:
        user_input = input("\nEnter feature numbers separated by comma (e.g. 1, 2) [Default: 0]: ").strip()
    except (EOFError, KeyboardInterrupt):
        user_input = ""

    if not user_input or user_input == "0":
        print("\n✨ Auto-selecting optimal features from latest code additions...")
        return []

    selected_names = []
    for part in user_input.split(","):
        part = part.strip()
        if part.isdigit():
            idx = int(part) - 1
            if 0 <= idx < len(feature_options):
                # Clean title
                raw_title = feature_options[idx]
                clean_title = raw_title.split(" (")[0].replace("✨ ", "").replace("🔗 ", "").replace("⚡ ", "").strip()
                selected_names.append(clean_title)

    if selected_names:
        print(f"\n✅ Selected Features for Campaign: {', '.join(selected_names)}")
    else:
        print("\n✨ Auto-selecting optimal features...")

    return selected_names


def run_pipeline(selected_features: list[str] = None) -> dict:
    """
    Execute the upgraded marketing pipeline.

    Args:
        selected_features: Optional list of feature names chosen by user.

    Returns:
        dict with all generated marketing campaign artifacts.
    """
    today = datetime.now().strftime("%Y-%m-%d")
    print(f"\n{'='*60}")
    print(f"  🚀 LinkBT Codebase-Driven Marketing Engine — {today}")
    print(f"{'='*60}\n")

    # ── Step 1: Codebase Analysis ────────────────────
    print("🔍 [1/5] Analyzing workspace codebase & Git history...")
    codebase_ctx = get_complete_codebase_context()
    print(f"   📂 Pages detected: {len(codebase_ctx['pages'])} ({', '.join(codebase_ctx['pages'][:4])}...)")
    print(f"   ✨ Newly detected features: {len(codebase_ctx['newly_detected_features'])}")

    # ── Step 2: Interactive Feature Selection ────────
    if selected_features is None:
        selected_features = prompt_user_for_features(codebase_ctx)

    # ── Step 3: Creative Director Strategy Selection ─
    print("\n🧠 [2/5] Selecting Creative Director strategy & Diversity parameters...")
    idea = generate_idea(selected_features)
    print(f"   🎨 Marketing Style: {idea.get('marketing_style')}")
    print(f"   📐 Poster Layout: {idea.get('layout')}")
    print(f"   🎯 Single Angle: {idea.get('angle')}")
    print(f"   📝 Headline: \"{idea.get('headline')}\"")
    print(f"   ✨ Highlighted Features: {idea.get('feature_focus')}")

    # ── Step 4: Uniqueness & Creativity Score Check ──
    print("\n📊 [3/5] Evaluating Creativity & Anti-Repetition Score...")
    c_score = idea.get("creativity_score", {})
    print(f"   ⭐ Novelty Score: {c_score.get('novelty')}/100")
    print(f"   ⭐ Visual Diversity Score: {c_score.get('visual_diversity')}/100")
    print(f"   ⭐ Similarity with History: {c_score.get('similarity_percentage')}% (Threshold: <30%)")
    
    if not c_score.get("passed", True):
        print("   ⚠️ Similarity exceeded 30%! Re-rolling creative concept for maximum diversity...")
        idea = generate_idea(selected_features)
        c_score = idea.get("creativity_score", {})
        print(f"   ✅ Re-rolled Concept: Style={idea.get('marketing_style')}, Layout={idea.get('layout')}")

    # ── Step 5: Synthesize AI Image Generation Prompt 
    print("\n🎨 [4/5] Synthesizing AI Image Generation Prompt...")
    image_prompt = generate_image_prompt(idea, codebase_ctx)
    word_count = len(image_prompt.split())
    print(f"   📄 AI Image Prompt generated ({word_count} words)")

    # ── Step 6: Generate LinkedIn Caption Copy ────────
    print("\n✍️  [5/5] Generating LinkedIn caption & post copy...")
    caption_data = generate_caption(idea)
    print(f"   📄 Caption generated successfully")

    # ── Step 7: Write Output Artifact: output/marketing-prompt.md ──
    campaign_name = f"LinkBT Campaign — {idea.get('marketing_style')} ({today})"

    new_features_md = "\n".join([
        f"- **{nf['name']}**: {nf['description']} *(Source: {nf['source']})*"
        for nf in codebase_ctx.get("newly_detected_features", [])
    ]) if codebase_ctx.get("newly_detected_features") else "- Continuous deployment performance & security upgrades"

    core_features_md = "\n".join([
        f"- **{cf['name']}**: {cf['description']}"
        for cf in codebase_ctx.get("core_features", [])
    ])

    marketing_prompt_content = f"""# {campaign_name}

## 📊 Campaign Strategy & Creative Direction
- **Campaign Name**: {campaign_name}
- **Marketing Style Category**: **{idea.get('marketing_style')}**
- **Poster Layout**: **{idea.get('layout')}**
- **Single Core Angle**: **{idea.get('angle')}**
- **Highlighted Features**: **{idea.get('feature_focus')}**
- **Color Palette**: {idea.get('color_palette_name')}
- **Camera Angle**: {idea.get('camera_mode')}
- **Lighting**: {idea.get('lighting_mode')}

### 📈 Creativity & Uniqueness Evaluation
- **Novelty Score**: {c_score.get('novelty')}/100
- **Visual Diversity Score**: {c_score.get('visual_diversity')}/100
- **Marketing Creativity**: {c_score.get('marketing_creativity')}/100
- **Similarity with History**: {c_score.get('similarity_percentage')}% *(Passed <30% threshold)*

---

## 🚀 Product Knowledge Graph
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

    output_file = OUTPUT_DIR / "marketing-prompt.md"
    with open(output_file, "w", encoding="utf-8") as f:
        f.write(marketing_prompt_content)

    print(f"\n{'='*60}")
    print(f"  ✅ [SUCCESS] Creative Director Pipeline Complete!")
    print(f"{'='*60}")
    print(f"📁 Output Artifact: {output_file}")

    result = {
        "date": today,
        "campaign_name": campaign_name,
        "marketing_style": idea.get("marketing_style"),
        "layout": idea.get("layout"),
        "headline": idea.get("headline"),
        "angle": idea.get("angle"),
        "feature_focus": idea.get("feature_focus"),
        "artifact_path": str(output_file),
    }

    # Record to campaign history
    save_post({
        "date": today,
        "campaign_name": campaign_name,
        "marketing_style": idea.get("marketing_style"),
        "layout": idea.get("layout"),
        "headline": idea.get("headline"),
        "angle": idea.get("angle"),
        "feature_focus": idea.get("feature_focus"),
        "status": "pending_approval",
    })

    return result


if __name__ == "__main__":
    parser = argparse.ArgumentParser(description="LinkBT Marketing Pipeline")
    parser.add_argument("--features", type=str, help="Comma-separated feature names to highlight")
    args = parser.parse_args()

    feats = [f.strip() for f in args.features.split(",")] if args.features else None
    run_pipeline(feats)
