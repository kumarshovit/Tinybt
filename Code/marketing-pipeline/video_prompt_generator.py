"""
AI Video Generation Prompt Generator — Creative Video Agency Engine.
Constructs hyper-detailed, publication-ready AI Video Generation Prompts tailored
for Gemini Veo, Runway Gen-4, Sora, Kling AI, Pika, and Hailuo AI with scene-by-scene clip prompts.
Enforces MINIMUM 30 seconds video duration.
"""

import sys
if hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8")
if hasattr(sys.stderr, "reconfigure"):
    sys.stderr.reconfigure(encoding="utf-8")


def generate_video_prompt(idea: dict, story: dict, codebase_ctx: dict, duration_info: dict) -> str:
    """
    Synthesize a 1000-2500 word production-ready AI Video Generation Prompt with Scene-by-Scene Clip Prompts.
    """
    video_style = idea.get("video_style", "Product Launch Commercial")
    story_structure = idea.get("story_structure", "Problem -> Solution")
    camera_direction = idea.get("camera_direction", "FPV Drone Fly-through")
    motion_style = idea.get("motion_style", "3D Floating Glassmorphic UI Cards")
    headline = idea.get("headline", "Enterprise Security for Every Link.")
    angle = idea.get("angle", "Enterprise-Grade Link Security Powered by Google Safe Browsing")
    feature_focus = idea.get("feature_focus", "Bot & Spam Captcha Shield")
    color_palette_name = idea.get("color_palette_name", "Obsidian & Emerald Shield")
    hero_icon = idea.get("hero_icon", "3D Emerald Security Shield")

    music_info = idea.get("music_info", {})
    music_genre = music_info.get("genre", "Future Bass & Upbeat Synth")
    music_mood = music_info.get("mood", "Energetic, High Growth, Confident")
    sfx_cues = story.get("sound_effects_cues", "Digital riser, glass UI chime, low bass thud")
    voiceover_tone = idea.get("voiceover_tone", "Confident, articulate SaaS Thought Leader")

    total_seconds = duration_info.get("seconds", 30)
    duration_label = duration_info.get("label", "30 Seconds (Standard Commercial)")

    pages_list = ", ".join(codebase_ctx.get("pages", ["Dashboard", "Analytics", "Security"]))

    negative_prompts = (
        "DO NOT use generic stock video footage, static unmoving images, jittery AI artifacts, "
        "distorted UI text, blurry resolution, low frame rate lag, unbranded template transitions, "
        "robotic monotonous voiceover, or noisy background artifacts."
    )

    scenes = story.get("scenes", [])

    # Detailed scene breakdowns
    scenes_md_lines = []
    # Scene-by-scene copyable prompts for AI Video models
    model_clip_prompts = []

    for sc in scenes:
        s_num = sc.get('scene_number', 1)
        s_title = sc.get('title', 'Scene')
        s_dur = sc.get('duration', '7.5s')
        s_cam = sc.get('camera', '')
        s_anim = sc.get('animation', '')
        s_vo = sc.get('voiceover', '')
        s_text = sc.get('onscreen_text', '')

        scenes_md_lines.append(f"""### 🎬 Scene {s_num}: {s_title} ({s_dur})
- **Strategic Purpose**: {sc.get('purpose', '')}
- **Camera Direction**: {s_cam}
- **Lighting & Environment**: {sc.get('lighting', '')}
- **UI Animation & Screenshots**: {s_anim}
- **Voiceover Script**: `"{s_vo}"`
- **On-Screen Text Overlay**: `"{s_text}"`
""")

        # Generate individual 10-second clip prompt for Veo / Runway / Kling
        clip_prompt = (
            f"**Scene {s_num} Clip Prompt ({s_dur})**:\n"
            f"```text\n"
            f"Cinematic {video_style} SaaS commercial, Scene {s_num}: {s_title}. {s_cam}. "
            f"Featuring LinkBT UI card with {feature_focus} and {hero_icon}. {s_anim}. "
            f"Color palette: {color_palette_name}. 4K 60fps, photorealistic raytracing, smooth motion.\n"
            f"```"
        )
        model_clip_prompts.append(clip_prompt)

    scenes_breakdown_text = "\n".join(scenes_md_lines)
    clip_prompts_text = "\n\n".join(model_clip_prompts)

    prompt_document = f"""# AI VIDEO GENERATION PROMPT (For Gemini Veo / Runway Gen-4 / Sora / Kling AI)

> **ROLE & CREATIVE DIRECTION**: You are a Lead Commercial Director, Principal VFX Supervisor, and Senior Advertising Strategist for world-class SaaS leaders (Apple, Stripe, Linear, Vercel, Notion). Your goal is to produce a publication-ready, cinematic {total_seconds}-second video advertisement for **LinkBT** (https://link.bt).

---

## 💡 WHY AI VIDEO MODELS GENERATE 10s CLIPS & HOW TO ASSEMBLE YOUR {total_seconds}s COMMERCIAL
AI video models (Gemini Veo, Runway Gen-4, Sora, Kling AI, Pika) generate video in **5 to 10-second single scene clips**. To create your full **{total_seconds}-second commercial**:
1. Copy each **Scene Clip Prompt** from **Section 6** below into your AI Video Generator to generate {len(scenes)} individual 10-second scene clips.
2. Alternatively, use Runway's or Kling AI's **"Extend Video"** feature to stitch Scene 1 → Scene 2 → Scene 3 into a seamless continuous video!

---

## 🎬 1. VIDEO CAMPAIGN STRATEGY & DIRECTORY OVERVIEW
- **Brand Name**: LinkBT (URL Shortener & Analytics Infrastructure)
- **Website URL**: https://link.bt
- **Commercial Category**: **{video_style}**
- **Storytelling Structure**: **{story_structure}**
- **Single Marketing Angle**: **{angle}**
- **Primary Feature Focus**: **{feature_focus}**
- **Hero Graphic Symbol**: **{hero_icon}**
- **Total Video Duration**: **{duration_label} (Minimum 30s Enforced across {len(scenes)} scenes)**
- **Video Aspect Ratio**: 16:9 Horizontal (YouTube/LinkedIn Web) or 9:16 Vertical (LinkedIn Mobile/Reels)
- **Frame Rate**: 60 FPS Ultra-Smooth Motion
- **Resolution Output**: 4K UHD Render

---

## 📸 2. SCREENSHOT INTEGRATION & ANIMATION DIRECTIVES
- **Attached Input**: Real high-resolution screenshots of LinkBT ({pages_list}).
- **MANDATORY AI VIDEO RULE**: **DO NOT GENERATE FAKE OR ILLOGICAL UI INTERFACES**.
- **Screenshot Animation**: Natural 3D perspective rotation, smooth cursor clicks, active hover states, and dynamic chart growth using real LinkBT UI screenshots.
- **Card Container**: Sleek 3D glassmorphic acrylic frames adapted to the **{color_palette_name}** theme.

---

## 📽️ 3. SCENE-BY-SCENE SHOT LIST BREAKDOWN ({total_seconds} SECONDS TOTAL)

{scenes_breakdown_text}

---

## 🎵 4. AUDIO & SOUND DESIGN DIRECTION
- **Soundtrack Genre**: **{music_genre}**
- **Audio Mood & Tempo**: **{music_mood}**
- **Voiceover Style & Tone**: **{voiceover_tone}**
- **Sound Effects (SFX) Cues**: {sfx_cues}
- **Complete Voiceover Script**:
```text
{story.get('full_voiceover_script', '')}
```

---

## 🎨 5. CINEMATOGRAPHY & MOTION GRAPHICS
- **Primary Camera System**: {camera_direction}
- **Motion Graphics Style**: {motion_style}
- **Color Grading & Atmosphere**: {color_palette_name}
- **Lighting Setup**: Soft volumetric studio key lights with specular raytraced reflections.

---

## 🤖 6. INDIVIDUAL SCENE CLIP PROMPTS FOR AI VIDEO GENERATORS
Paste these prompts individually into Gemini Veo, Runway, Kling AI, or Sora to generate each {total_seconds // len(scenes)}-second scene clip:

{clip_prompts_text}

---

## 🚫 7. STRICT NEGATIVE PROMPTS
{negative_prompts}

---

## 📋 8. FINAL PRODUCTION CHECKLIST
1. Verify video duration is at least 30 seconds ({total_seconds}s total across {len(scenes)} scenes).
2. Generate all {len(scenes)} scene clips or use the "Extend Video" feature.
3. Overlay the provided voiceover script over the assembled commercial.
4. Render high-converting CTA: `"{story.get('cta_line', 'Try LinkBT Free → link.bt')}"`.
""".strip()

    return prompt_document
