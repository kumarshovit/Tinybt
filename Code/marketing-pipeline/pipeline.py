"""
Main Pipeline Orchestrator for LinkBT Marketing Engine.
Supports interactive prompt type selection (Image, Video, or Both) and feature selection.

Flow:
  1. Codebase Analysis (Git history, React components, .NET backend)
  2. Interactive Feature Selection (Ask user at runtime or auto-select if skipped)
  3. Interactive Prompt Type Selection (Ask user: Image, Video, or Both)
  4. Creative Director Strategy (Style -> Layout / Story -> Single Angle -> Uniqueness Score)
  5. Prompt Synthesis:
     - Image Prompt -> output/marketing-prompt.md
     - Video Prompt -> output/video-prompt.md (Gemini Veo, Runway Gen-4, Sora, Kling AI - Min 30s)
"""

import sys
if hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8")
if hasattr(sys.stderr, "reconfigure"):
    sys.stderr.reconfigure(encoding="utf-8")

import json
import random
import argparse
from datetime import datetime
from pathlib import Path
from config import OUTPUT_DIR, PAST_POSTS_FILE
from codebase_analyzer import get_complete_codebase_context
from brand import save_post
from idea_generator import generate_idea
from prompt_generator import generate_image_prompt
from caption_generator import generate_caption

from video_diversity_engine import (
    VIDEO_DURATIONS,
    VIDEO_STYLES,
    STORY_STRUCTURES,
    CAMERA_DIRECTIONS,
    MOTION_GRAPHICS_STYLES,
    MUSIC_DIRECTIONS,
    VOICEOVER_TONES,
    calculate_video_creativity_score,
)
from video_story_generator import generate_video_story
from video_prompt_generator import generate_video_prompt


def prompt_user_for_features(codebase_ctx: dict, preselected: list[str] = None) -> list[str]:
    """Interactively ask user which features to highlight."""
    if preselected:
        print(f"\n🎯 Using pre-selected features: {', '.join(preselected)}")
        return preselected

    core_features = codebase_ctx.get("core_features", [])
    newly_detected = codebase_ctx.get("newly_detected_features", [])
    
    feature_options = []

    for nd in newly_detected:
        feature_options.append(f"✨ {nd['name']} (New: {nd['description']})")
    
    for cf in core_features:
        feature_options.append(f"{cf.get('emoji', '🔗')} {cf['name']} ({cf['description']})")

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

    if not sys.stdin.isatty():
        print("\n⚡ Non-interactive shell detected: Intelligently auto-selecting features...")
        return [feature_options[0].split(" (")[0].replace("✨ ", "").replace("🔗 ", "").replace("⚡ ", "")]

    print("\n" + "="*60)
    print("  🎨 LINKBT CREATIVE DIRECTOR — STEP 1: FEATURE SELECTION")
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
                raw_title = feature_options[idx]
                clean_title = raw_title.split(" (")[0].replace("✨ ", "").replace("🔗 ", "").replace("⚡ ", "").strip()
                selected_names.append(clean_title)

    if selected_names:
        print(f"\n✅ Selected Features for Campaign: {', '.join(selected_names)}")
    else:
        print("\n✨ Auto-selecting optimal features...")

    return selected_names


def prompt_user_for_type(preselected_type: str = None) -> str:
    """Interactively ask user if they want Image, Video, or Both prompts."""
    if preselected_type and preselected_type in ["image", "video", "all"]:
        return preselected_type

    if not sys.stdin.isatty():
        return "all"

    print("\n" + "="*60)
    print("  🎬 LINKBT CREATIVE DIRECTOR — STEP 2: PROMPT TYPE SELECTION")
    print("="*60)
    print("Which type of marketing campaign prompt would you like to generate?\n")
    print("  [1] 🖼️  AI Image Generation Prompt (for ChatGPT / Gemini / DALL-E)")
    print("  [2] 🎬  AI Video Commercial Prompt (for Gemini Veo / Runway / Sora / Kling AI - Min 30s)")
    print("  [3] 🚀  BOTH (Image Poster & Video Commercial Prompts)")

    try:
        user_input = input("\nEnter option [1, 2, or 3] (Default: 3): ").strip()
    except (EOFError, KeyboardInterrupt):
        user_input = ""

    if user_input == "1":
        print("\n🖼️ Selected: AI Image Generation Prompt Only")
        return "image"
    elif user_input == "2":
        print("\n🎬 Selected: AI Video Commercial Prompt Only (Min 30s)")
        return "video"
    else:
        print("\n🚀 Selected: BOTH (AI Image & Video Commercial Prompts)")
        return "all"


def run_pipeline(selected_features: list[str] = None, prompt_type: str = None) -> dict:
    """
    Execute the marketing pipeline for Image and/or Video prompt generation.
    Interactively prompts for feature and prompt type selection.
    """
    today = datetime.now().strftime("%Y-%m-%d")
    print(f"\n{'='*60}")
    print(f"  🚀 LinkBT Codebase-Driven Creative Director Engine — {today}")
    print(f"{'='*60}\n")

    # 1. Codebase Analysis
    print("🔍 [1/6] Analyzing workspace codebase & Git history...")
    codebase_ctx = get_complete_codebase_context()
    print(f"   📂 Pages detected: {len(codebase_ctx['pages'])}")
    print(f"   ✨ Newly detected features: {len(codebase_ctx['newly_detected_features'])}")

    # 2. Interactive Feature Selection
    if selected_features is None:
        selected_features = prompt_user_for_features(codebase_ctx)

    # 3. Interactive Prompt Type Selection (Image, Video, or Both)
    if prompt_type is None:
        prompt_type = prompt_user_for_type()

    # 4. Creative Strategy Generation
    print("\n🧠 [2/6] Generating Creative Strategy & Brand Parameters...")
    image_idea = generate_idea(selected_features)
    print(f"   🎯 Headline: \"{image_idea.get('headline')}\"")
    print(f"   ✨ Highlighted Feature: {image_idea.get('feature_focus')}")

    results = {}

    # ── Image Prompt Generation ──────────────────────
    if prompt_type in ["image", "all"]:
        print("\n🎨 [3/6] Synthesizing AI Image Generation Prompt...")
        image_prompt = generate_image_prompt(image_idea, codebase_ctx)
        caption_data = generate_caption(image_idea)

        campaign_name = f"LinkBT Campaign — {image_idea.get('marketing_style')} ({today})"
        
        image_prompt_content = f"""# {campaign_name}

## 📊 Campaign Strategy & Creative Direction
- **Campaign Name**: {campaign_name}
- **Marketing Style Category**: **{image_idea.get('marketing_style')}**
- **Poster Layout Structure**: **{image_idea.get('layout')}**
- **Single Core Angle**: **{image_idea.get('angle')}**
- **Highlighted Features**: **{image_idea.get('feature_focus')}**
- **Color Palette**: {image_idea.get('color_palette_name')}

---

## 📱 LinkedIn Post Copy
```text
{caption_data.get('full_post', '')}
```

---

## 🎨 Complete AI Image Generation Prompt (For ChatGPT / Gemini)

{image_prompt}
""".strip()

        img_output_file = OUTPUT_DIR / "marketing-prompt.md"
        with open(img_output_file, "w", encoding="utf-8") as f:
            f.write(image_prompt_content)

        print(f"   ✅ AI Image Prompt generated -> {img_output_file}")
        results["image_artifact"] = str(img_output_file)

    # ── Video Prompt Generation ──────────────────────
    if prompt_type in ["video", "all"]:
        print("\n🎬 [4/6] Generating AI Video Commercial Strategy (Min 30s Enforced)...")

        duration_info = random.choice(VIDEO_DURATIONS)
        video_style = random.choice(VIDEO_STYLES)
        story_structure = random.choice(STORY_STRUCTURES)
        camera_dir = random.choice(CAMERA_DIRECTIONS)
        motion_style = random.choice(MOTION_GRAPHICS_STYLES)
        music_info = random.choice(MUSIC_DIRECTIONS)
        voiceover_tone = random.choice(VOICEOVER_TONES)

        video_concept = {
            "video_style": video_style,
            "story_structure": story_structure,
            "camera_direction": camera_dir,
            "motion_style": motion_style,
            "music_info": music_info,
            "voiceover_tone": voiceover_tone,
            "headline": image_idea.get("headline"),
            "angle": image_idea.get("angle"),
            "feature_focus": image_idea.get("feature_focus"),
            "color_palette_name": image_idea.get("color_palette_name"),
            "hero_icon": image_idea.get("hero_icon", "3D Emerald Security Shield"),
        }

        video_story = generate_video_story(video_concept, image_idea.get("feature_focus"), duration_info)

        print("\n📊 [5/6] Evaluating Video Uniqueness & Anti-Repetition Score...")
        v_score = calculate_video_creativity_score(video_concept, [])
        print(f"   ⭐ Video Novelty Score: {v_score.get('novelty')}/100")
        print(f"   ⭐ Duration: {duration_info['label']}")

        print("\n🎥 [6/6] Synthesizing Cinematic AI Video Prompt (Gemini Veo / Runway / Sora / Kling)...")
        video_prompt = generate_video_prompt(video_concept, video_story, codebase_ctx, duration_info)
        video_caption = generate_caption(image_idea)

        video_campaign_name = f"LinkBT Commercial — {video_style} ({duration_info['seconds']}s) ({today})"

        video_prompt_content = f"""# {video_campaign_name}

## 📊 Video Campaign Strategy
- **Commercial Category**: **{video_style}**
- **Story Structure**: **{story_structure}**
- **Duration**: **{duration_info['label']} (Min 30s Enforced)**
- **Highlighted Feature**: **{image_idea.get('feature_focus')}**
- **Hero Graphic**: {image_idea.get('hero_icon')}

---

## 📱 LinkedIn Video Post Copy
```text
{video_caption.get('full_post', '')}
```

---

## 🎬 Complete AI Video Generation Prompt (For Gemini Veo / Runway Gen-4 / Sora / Kling AI)

{video_prompt}
""".strip()

        vid_output_file = OUTPUT_DIR / "video-prompt.md"
        with open(vid_output_file, "w", encoding="utf-8") as f:
            f.write(video_prompt_content)

        print(f"   ✅ AI Video Commercial Prompt generated -> {vid_output_file}")
        results["video_artifact"] = str(vid_output_file)

    print(f"\n{'='*60}")
    print(f"  ✅ [SUCCESS] Creative Director Pipeline Complete!")
    print(f"{'='*60}")
    if "image_artifact" in results:
        print(f"🖼️  Image Prompt: {results['image_artifact']}")
    if "video_artifact" in results:
        print(f"🎬 Video Prompt: {results['video_artifact']}")

    save_post({
        "date": today,
        "type": prompt_type,
        "headline": image_idea.get("headline"),
        "feature_focus": image_idea.get("feature_focus"),
        "status": "pending_approval",
    })

    return results


if __name__ == "__main__":
    parser = argparse.ArgumentParser(description="LinkBT Marketing Pipeline")
    parser.add_argument("--features", type=str, help="Comma-separated feature names")
    parser.add_argument("--type", type=str, choices=["image", "video", "all"], help="Prompt type (image, video, all)")
    args = parser.parse_args()

    feats = [f.strip() for f in args.features.split(",")] if args.features else None
    run_pipeline(feats, args.type)
