"""
AI Image Generation Prompt Generator.
Constructs a world-class, hyper-detailed (1000-3000 words) AI Image Generation Prompt
tailored for ChatGPT / Gemini to create agency-grade SaaS marketing posters using real app screenshots.
"""

import sys
if hasattr(sys.stdout, "reconfigure"):
    sys.stdout.reconfigure(encoding="utf-8")
if hasattr(sys.stderr, "reconfigure"):
    sys.stderr.reconfigure(encoding="utf-8")

from brand import get_brand_context
from llm import generate


def generate_image_prompt(idea: dict, codebase_ctx: dict) -> str:
    """
    Generate an extremely detailed AI Image Generation Prompt (1000-3000 words)
    instructing ChatGPT or Gemini to produce a world-class SaaS advertisement poster.
    """
    headline = idea.get("headline", "Smart Links. Real Insights.")
    subtitle = idea.get("subtitle", "Track every click with LinkBT's free analytics dashboard.")
    cta = idea.get("cta", "Try LinkBT Free → link.bt")
    theme = idea.get("theme", "Analytics & Real-Time Insights")
    angle = idea.get("angle", "Problem -> Solution")
    target_audience = idea.get("target_audience", "Digital Marketers, Developers, Content Creators, SaaS Teams")
    emotional_trigger = idea.get("emotional_trigger", "Confidence, Data Clarity, Professional Authority")
    pain_points = idea.get("pain_points", "Messy long links, unmeasured marketing campaigns, overpriced shortener subscriptions")
    
    # Extract newly detected features text
    new_features = codebase_ctx.get("newly_detected_features", [])
    new_features_text = "\n".join([f"  • {nf['name']}: {nf['description']} (Source: {nf['source']})" for nf in new_features]) if new_features else "  • Continuous deployment performance & security upgrades"

    # Core features text
    core_features_text = "\n".join([f"  • {cf['name']}: {cf['description']} (Benefit: {cf['benefit']})" for cf in codebase_ctx.get("core_features", [])[:6]])

    pages_list = ", ".join(codebase_ctx.get("pages", ["Dashboard", "Analytics", "Security", "Tag Management"]))

    # Ask LLM to generate rich visual concepts if available
    visual_concept_prompt = f"""
    You are an elite Creative Director for world-class SaaS brands (Stripe, Linear, Vercel, Notion, Figma).
    Theme: {theme}
    Headline: "{headline}"
    Angle: {angle}
    Write a rich 200-word visual story, 3D graphic elements description, and lighting composition for a LinkedIn poster.
    """
    
    llm_visuals = generate(
        visual_concept_prompt,
        system="You are an award-winning Creative Director specializing in 3D SaaS visual design and brand advertising.",
        timeout=10
    )

    visual_details = llm_visuals if llm_visuals else (
        f"The visual composition features a high-impact, dark-mode 3D SaaS product environment. "
        f"At the focal center, an isometric floating desktop browser card tilted at a 15-degree angle showcases "
        f"the real LinkBT UI. Surrounding the main interface card are floating 3D glassmorphic element chips: "
        f"a glowing cyan click-telemetry chart, a metallic chrome link-chain icon, a green security badge with "
        f"Google Safe Browsing verification mark, and translucent campaign tag pills. Soft ambient volumetric lighting "
        f"in Royal Blue (#2563EB) and Sky Blue (#06B6D4) creates a premium studio atmosphere with raytraced specular reflections."
    )

    # Build the full 1000-3000 word AI Image Generation Prompt
    prompt_document = f"""# AI IMAGE GENERATION PROMPT (For ChatGPT / Gemini)

> **ROLE & MISSION**: Act as a World-Class Senior Creative Director, Principal Brand Architect, and Lead Visual Designer for top-tier SaaS companies (Stripe, Linear, Vercel, Notion, Figma, Framer, Canva, Adobe). Your goal is to design a high-converting, publication-ready LinkedIn marketing poster for **LinkBT** (https://link.bt).

---

## 📸 1. CRITICAL SCREENSHOT PLACEMENT INSTRUCTIONS
**MANDATORY INPUT INSTRUCTION**:
I will attach 2–5 real screenshots of the LinkBT web application (including the {pages_list} pages).
- **DO NOT GENERATE FAKE OR RECREATED WIREFRAME UI**.
- **USE THE REAL ATTACHED SCREENSHOTS** as the primary hero UI elements inside the poster.
- Crop, perspective-tilt (15° isometric floating angle), and embed the actual LinkBT UI screenshots inside sleek, dark-mode glassmorphic browser cards with rounded corners (16px radius), subtle 1px border glows (`rgba(255, 255, 255, 0.1)`), and soft multi-layered 3D drop shadows.
- Blend the real UI screenshots seamlessly into the studio environment described in the art direction sections below.

---

## 🎯 2. MARKETING STRATEGY & CAMPAIGN GOALS
- **Brand Name**: LinkBT (URL Shortener & Analytics Infrastructure)
- **Website URL**: https://link.bt
- **Campaign Theme**: {theme}
- **Marketing Objective**: Position LinkBT as the premier enterprise-grade, 100% FREE alternative to paid URL shorteners on LinkedIn, driving user acquisition and brand authority.
- **Campaign Goal**: Capture digital marketer and developer attention with scroll-stopping SaaS visual design while communicating instant product value.
- **Target Audience**: {target_audience}
- **Customer Pain Points**: {pain_points}
- **Emotional Trigger**: {emotional_trigger}
- **Marketing Angle**: {angle}
- **Primary Value Proposition**: 100% Free URL Shortener with real-time click analytics, custom branded aliases, link tagging, expiration controls, and Google Safe Browsing threat protection — zero credit card required, no hidden paywalls.

---

## 🚀 3. PRODUCT FEATURES HIGHLIGHTS
### Core Platform Features:
{core_features_text}

### Newly Released Features (Automatically Detected from Codebase & Git Commits):
{new_features_text}

---

## ✍️ 4. ON-POSTER TYPOGRAPHY & COPY HIERARCHY

### Headline (Display Typography):
- **Text**: `"{headline}"`
- **Styling**: Extra Bold Display Sans-Serif (Inter / SF Pro / Outfit), crisp pure white (`#FFFFFF`), top-left aligned, large scale (optical size ~48pt), crisp kerning, zero blur.

### Supporting Copy (Subtitle):
- **Text**: `"{subtitle}"`
- **Styling**: Medium Weight Sans-Serif, slate silver (`#94A3B8`), positioned directly below headline with 16px line spacing, highly legible on dark background.

### Call-to-Action (Primary CTA Button):
- **Text**: `"{cta}"`
- **Styling**: Pill-shaped button (`border-radius: 9999px`), filled with Royal Blue (`#2563EB`) gradient, featuring a subtle white directional arrow icon (`→`) and glowing outer hover border.

### Trust Badges & Micro-Copy:
- `✓ 100% FREE — No Credit Card Required` (Styled inside an Emerald Green `#10B981` pill badge at bottom-left).
- `⚡ Powered by LinkBT Platform (link.bt)` (Styled as subtle silver footer micro-copy).

---

## 🎨 5. COMPREHENSIVE ART DIRECTION & VISUAL SPECIFICATIONS

### Poster Style & Mood:
- **Style Genre**: Studio-Quality Premium Dark Mode SaaS Advertisement (Stripe / Linear / Vercel design language).
- **Dimensions & Aspect Ratio**: 1200 x 628 pixels (LinkedIn Landscape Feed Standard, 1.91:1 aspect ratio) or 1080 x 1350 pixels (LinkedIn Vertical Carousel).
- **Overall Tone**: Sleek, authoritative, high-tech, trustworthy, modern, hyper-polished, zero clutter.

### Visual Story & Scene Composition:
{visual_details}

### Background Design & Atmosphere:
- **Base Gradient**: Deep dark navy to pitch slate transition (`#0F172A` top-left to `#020617` bottom-right).
- **Isometric Grid**: Ultra-subtle background vector grid lines (`rgba(255, 255, 255, 0.03)`) providing structural depth.
- **Radial Lighting Orbs**: Soft background radial glows in Royal Blue (`#2563EB`) and Electric Cyan (`#06B6D4`) positioned behind the floating screenshot browser card to create a dramatic backlight halo effect.
- **Depth-of-Field Particles**: Floating translucent 3D geometric shapes (glass spheres, glowing data bars, floating link icons, security shield badges) with subtle fore/background bokeh defocus.

### Curated Brand Color Palette:
- **Primary Brand Accent**: Royal Blue (`#2563EB`)
- **Background Base**: Dark Slate Navy (`#0F172A` / `#020617`)
- **Secondary Accent**: Electric Sky Cyan (`#06B6D4`)
- **Trust Accent**: Emerald Green (`#10B981`)
- **Text Primary**: Pure White (`#FFFFFF`)
- **Text Muted**: Slate Silver (`#94A3B8`)
- **Card Background**: Semi-translucent Glassmorphism (`rgba(30, 41, 59, 0.7)` with `backdrop-filter: blur(20px)`).

### Typography System:
- **Font Family**: Geometric Sans-Serif (Inter, SF Pro Display, Outfit, or Roboto).
- **Scale**: Display Headline (48px relative), Subtitle (20px relative), Card Headings (16px relative), Badges (14px relative).
- **Alignment**: Asymmetric layout with headline & copy on the left column, floating UI screenshots on the right center, and CTA/trust badges across the footer.

### 3D Iconography & Micro-Details:
- High-fidelity 3D rendered graphic elements:
  1. 🔗 Metallic chrome short link chain element floating near the UI card.
  2. 📊 Glowing 3D bar chart with upward trend arrow symbolizing real-time click tracking.
  3. 🛡️ 3D emerald security shield badge indicating Google Safe Browsing protection.
  4. 🏷️ Floating translucent campaign tag pill representing link organization.

### Rendering Quality & Lighting:
- **Lighting setup**: Dual-tone studio lighting (Primary Royal Blue key light from top-left, Electric Cyan rim light from bottom-right).
- **Shading & Reflections**: Raytraced specular highlights on glass edges, subsurface scattering on acrylic cards, ambient occlusion under floating UI components.
- **Output Standard**: 8K ultra-sharp vector-crisp typography, zero digital noise, zero pixelation, publication-ready for LinkedIn.

---

## 📋 6. FINAL PRODUCTION CHECKLIST
1. Verify exact headline rendering: `"{headline}"`.
2. Seamlessly embed attached real LinkBT screenshots as the main central browser UI.
3. Display CTA button: `"{cta}"`.
4. Ensure the poster looks like an official advertisement from a multi-billion-dollar SaaS leader.
5. Render clean, professional, and ready for immediate sharing on LinkedIn.
""".strip()

    return prompt_document
