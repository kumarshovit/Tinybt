"""
AI Image Generation Prompt Generator — Creative Director Engine.
Constructs hyper-detailed, publication-ready AI Image Generation Prompts tailored
for ChatGPT / Gemini based on the selected Marketing Style, Layout, Camera Angle, Color Palette,
and Feature-Aligned Art Direction.
"""

import sys
if hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8")
if hasattr(sys.stderr, "reconfigure"):
    sys.stderr.reconfigure(encoding="utf-8")

from llm import generate


def generate_image_prompt(idea: dict, codebase_ctx: dict) -> str:
    """
    Generate a production-ready AI Image Generation Prompt (for ChatGPT / Gemini)
    customized strictly to the selected Creative Director parameters and feature focus.
    """
    marketing_style = idea.get("marketing_style", "Feature Spotlight")
    layout = idea.get("layout", "Hero screenshot central showcase with radial halo")
    headline = idea.get("headline", "Zero Spam Clicks. Pure Traffic Stats.")
    subtitle = idea.get("subtitle", "Protect and track your campaign short links with LinkBT — 100% free.")
    cta = idea.get("cta", "Try LinkBT Free → link.bt")
    angle = idea.get("angle", "Enterprise-Grade Link Security Powered by Google Safe Browsing")
    feature_focus = idea.get("feature_focus", "Bot & Spam Captcha Shield")
    color_palette_name = idea.get("color_palette_name", "Obsidian & Emerald Shield")
    camera_mode = idea.get("camera_mode", "Frontal eye-level studio shot")
    lighting_mode = idea.get("lighting_mode", "Soft studio ambient key light")
    hero_icon = idea.get("hero_icon", "3D Emerald Security Shield & Captcha Lock")

    pages_list = ", ".join(codebase_ctx.get("pages", ["Dashboard", "Analytics", "Security"]))

    # Negative prompts to prevent visual monotony & template repeating
    negative_prompts = (
        "DO NOT generate a 5-item left sidebar feature list. "
        "DO NOT generate a bottom horizontal 3-icon badge bar. "
        "DO NOT repeat the standard blue tilted dashboard layout. "
        "DO NOT use generic stock photos, cluttered text, low-resolution wireframes, "
        "blurry text, distorted UI elements, fake generic charts, watermarks, or noisy backgrounds."
    )

    visual_concept_prompt = f"""
    You are an award-winning Creative Director for SaaS brand advertising (Apple, Linear, Stripe, Vercel).
    Marketing Style: {marketing_style}
    Layout Composition: {layout}
    Headline: "{headline}"
    Single Core Angle: {angle}
    Feature Focus: {feature_focus}
    Hero 3D Icon: {hero_icon}
    Color Palette: {color_palette_name}
    Camera Angle: {camera_mode}
    Lighting: {lighting_mode}
    Write a 150-word detailed visual storytelling description for a high-converting LinkedIn poster.
    """

    llm_visuals = generate(
        visual_concept_prompt,
        system="You are a Lead Creative Director specializing in SaaS visual direction and ad campaigns.",
        timeout=10
    )

    visual_details = llm_visuals if llm_visuals else (
        f"The visual composition is crafted in a {marketing_style} aesthetic with a custom {layout} layout. "
        f"The focal hero center features {feature_focus} symbolized by a pristine {hero_icon}. "
        f"The scene utilizes {color_palette_name} tones with {camera_mode} "
        f"and {lighting_mode}."
    )

    prompt_document = f"""# AI IMAGE GENERATION PROMPT (For ChatGPT / Gemini)

> **ROLE & CREATIVE DIRECTION**: You are an elite Senior Creative Director & Lead Visual Designer for top-tier SaaS brands (Apple, Stripe, Linear, Vercel, Notion). Produce a publication-ready LinkedIn ad poster for **LinkBT** (https://link.bt).

---

## 🎨 1. CAMPAIGN STRATEGY & CREATIVE CONCEPT
- **Brand Name**: LinkBT (URL Shortener & Analytics Infrastructure)
- **Website URL**: https://link.bt
- **Marketing Style Category**: **{marketing_style}**
- **Poster Layout Structure**: **{layout}**
- **Single Core Marketing Angle**: **{angle}**
- **Feature Focus**: **{feature_focus}**
- **Primary Hero Visual Element**: **{hero_icon}**
- **Primary Value Proposition**: 100% Free URL Shortener with enterprise analytics, custom aliases, link tagging, expiration, and Google Safe Browsing protection.

---

## 📸 2. SCREENSHOT PLACEMENT & LAYOUT RULES
- **Attached Input**: Real screenshots of LinkBT ({pages_list}).
- **LAYOUT DIRECTIVE**: **DO NOT RECREATE FAKE OR WIREFRAME UI**. Embed real LinkBT screenshots matching the **{layout}** layout.
- **CARD STYLING**: Sleek glassmorphic card container adapted to the **{color_palette_name}** color theme.
- **CRITICAL STRUCTURAL RULE**: **DO NOT include a left-column 5-icon feature list or bottom 3-badge row**. Keep the layout clean, focused strictly on **{feature_focus}** and **{headline}**.

---

## ✍️ 3. ON-POSTER COPY & TYPOGRAPHY HIERARCHY
- **Headline**: `"{headline}"`
  - *Style*: Display Sans-Serif (Inter / SF Pro / Outfit), bold, high visual contrast, styled strictly according to **{layout}**.
- **Supporting Subtitle**: `"{subtitle}"`
  - *Style*: Medium weight, slate/silver tone, high legibility.
- **Call-to-Action (CTA Button)**: `"{cta}"`
  - *Style*: Pill-shaped button with subtle directional arrow icon and glowing accent hover border.
- **Trust Badge**: `✓ 100% FREE — No Credit Card Required`

---

## 🎭 4. ART DIRECTION & VISUAL ENVIRONMENT
- **Visual Style**: {marketing_style}
- **Layout Specification**: {layout}
- **Hero Graphic**: {hero_icon}
- **Camera Perspective**: {camera_mode}
- **Lighting Setup**: {lighting_mode}
- **Color Palette Mode**: {color_palette_name}
- **Visual Scene Description**:
{visual_details}

---

## 🚫 5. STRICT NEGATIVE PROMPTS
{negative_prompts}

---

## 📋 6. FINAL PRODUCTION CHECKLIST
1. Render exact headline: `"{headline}"`.
2. Seamlessly integrate real LinkBT screenshots into layout: `{layout}`.
3. Feature the primary hero graphic: `{hero_icon}`.
4. Render CTA: `"{cta}"`.
5. Ensure the poster looks completely unique and distinct from previous campaigns.
""".strip()

    return prompt_document
