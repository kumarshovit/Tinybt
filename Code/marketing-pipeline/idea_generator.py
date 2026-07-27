"""
Marketing Idea & Creative Concept Generator — Creative Director Architecture.
Selects Marketing Style first, Layout second, Single Marketing Angle third,
and aligns selected features with dynamic headlines, color palettes, and creativity scoring.
"""

import random
from brand import get_brand_context, load_past_posts
from diversity_engine import (
    MARKETING_STYLES,
    LAYOUTS,
    SINGLE_MARKETING_ANGLES,
    CAMERA_LIGHTING_MODES,
    COLOR_PALETTES,
    get_seasonal_intelligence,
    calculate_creativity_score,
)
from llm import generate_json

# Feature-specific fallback headlines & color themes
FEATURE_THEMATIC_MAP = {
    "security": {
        "headlines": [
            "Zero Spam Clicks. Pure Traffic Stats.",
            "Shield Every Link with Google Safe Browsing.",
            "Enterprise Security for Every Campaign Link.",
            "Block Malware, Phishing & Bot Clicks Instantly."
        ],
        "palette": {"name": "Obsidian & Emerald Shield", "base": "#06120E", "accent": "#10B981", "trust": "#059669", "card": "#0F291E"},
        "hero_icon": "3D Emerald Security Shield & Captcha Lock"
    },
    "analytics": {
        "headlines": [
            "Track Every Click in Real Time.",
            "Uncover Geo, Device & IP Demographics.",
            "Measure Campaign ROI with Precision Analytics.",
            "Data-Driven Marketing Powered by LinkBT."
        ],
        "palette": {"name": "Electric Cyan Telemetry", "base": "#030B14", "accent": "#00F0FF", "trust": "#3B82F6", "card": "#0D1F33"},
        "hero_icon": "3D Translucent Glowing Trend Chart & Geo Globe"
    },
    "brand": {
        "headlines": [
            "Branded Short Links That Drive Clicks.",
            "Build Trust with Custom Domain Aliases.",
            "Turn Messy URLs into High-Converting Short Links.",
            "Custom Alias Shortening Built for Growth."
        ],
        "palette": {"name": "Royal Indigo Luxury", "base": "#090714", "accent": "#8B5CF6", "trust": "#6366F1", "card": "#16122C"},
        "hero_icon": "3D Floating Metallic Chrome Link Chain"
    },
    "developer": {
        "headlines": [
            "Developer-First REST API & Webhooks.",
            "Automate Short Links in 3 Lines of Code.",
            "Programmatic Link Shortening Infrastructure.",
            "High-Speed JWT REST API for Developers."
        ],
        "palette": {"name": "Cyber Matrix Terminal", "base": "#050B07", "accent": "#39FF14", "trust": "#10B981", "card": "#0C1C11"},
        "hero_icon": "3D Glowing Code Syntax Card & Webhook Lightning"
    }
}


def generate_idea(selected_feature_names: list[str] = None) -> dict:
    """
    Generate a Creative Director campaign concept with dynamic feature-aligned headlines & themes.
    """
    brand_ctx = get_brand_context()
    summary_text = brand_ctx["summary_text"]
    codebase = brand_ctx["codebase"]

    past_posts = load_past_posts()
    recent_styles = [p.get("marketing_style") for p in past_posts[-5:] if p.get("marketing_style")]
    recent_layouts = [p.get("layout") for p in past_posts[-5:] if p.get("layout")]

    # 1. Select Marketing Style (avoid recent)
    available_styles = [s for s in MARKETING_STYLES if s not in recent_styles]
    selected_style = random.choice(available_styles) if available_styles else random.choice(MARKETING_STYLES)

    # 2. Select Layout (never same consecutively)
    last_layout = recent_layouts[-1] if recent_layouts else None
    available_layouts = [l for l in LAYOUTS if l != last_layout]
    selected_layout = random.choice(available_layouts)

    # 3. Select Single Marketing Angle
    selected_angle = random.choice(SINGLE_MARKETING_ANGLES)

    # 4. Seasonal Context
    seasonal = get_seasonal_intelligence()

    # 5. Camera & Lighting
    cam_lighting = random.choice(CAMERA_LIGHTING_MODES)

    # 6. Resolve Features Highlighted & Dynamic Thematic Alignment
    all_core = codebase.get("core_features", [])
    newly_detected = codebase.get("newly_detected_features", [])

    feature_category = "analytics"

    if selected_feature_names:
        matched_features = []
        stop_words = {"&", "and", "the", "a", "or", "in", "with", "for", "to", "of", "-", "new:"}
        for sf in selected_feature_names:
            clean_sf = sf.replace("(Recommended)", "").strip()
            keywords = [w.lower() for w in clean_sf.split() if w.lower() not in stop_words and len(w) > 2]
            matched = False
            for f in newly_detected + all_core:
                fname = f.get("name", "").lower()
                if any(kw in fname for kw in keywords):
                    matched_features.append(f)
                    matched = True
                    break
            if not matched:
                matched_features.append({"name": clean_sf, "description": clean_sf})
        features_to_use = matched_features
    else:
        features_to_use = newly_detected[:2] if newly_detected else random.sample(all_core, min(2, len(all_core)))

    feature_summary = ", ".join([f["name"] for f in features_to_use])
    feat_text_lower = feature_summary.lower()

    if any(k in feat_text_lower for k in ["captcha", "security", "safe browsing", "spam", "shield"]):
        feature_category = "security"
    elif any(k in feat_text_lower for k in ["alias", "brand", "domain", "custom"]):
        feature_category = "brand"
    elif any(k in feat_text_lower for k in ["api", "webhook", "developer", "code"]):
        feature_category = "developer"
    else:
        feature_category = "analytics"

    theme_data = FEATURE_THEMATIC_MAP[feature_category]
    default_headline = random.choice(theme_data["headlines"])
    color_palette = theme_data["palette"]
    hero_icon = theme_data["hero_icon"]

    recent_history = "\n".join([
        f"- Style: {p.get('marketing_style', 'N/A')} | Layout: {p.get('layout', 'N/A')} | Headline: {p.get('headline', 'N/A')}"
        for p in past_posts[-7:]
    ]) if past_posts else "No previous posts recorded."

    prompt = f"""
{summary_text}

=== RECENT CAMPAIGNS (DO NOT REPEAT THESE STYLES OR LAYOUTS) ===
{recent_history}

=== CREATIVE DIRECTION ASSIGNMENT ===
• Marketing Style: {selected_style}
• Poster Layout: {selected_layout}
• Single Core Angle: {selected_angle}
• Seasonal Context: {seasonal['event']} ({seasonal['angle']})
• Highlighted Features: {feature_summary}
• Color Palette: {color_palette['name']} (Base: {color_palette['base']}, Accent: {color_palette['accent']})
• Camera & Lighting: {cam_lighting['camera']} | {cam_lighting['lighting']}
• Suggested Headline Focus: "{default_headline}"

=== YOUR TASK ===
Act as a Creative Director. Write a unique, single-focus marketing poster concept for LinkBT.
Write a punchy headline specific to {feature_summary} (DO NOT use "Real-Time Insights. Zero Guesswork.").

=== RESPOND WITH THIS EXACT JSON FORMAT ONLY ===
{{
    "marketing_style": "{selected_style}",
    "layout": "{selected_layout}",
    "theme": "{selected_style} — {seasonal['event']}",
    "headline": "{default_headline}",
    "subtitle": "Track, protect, and optimize your campaign short links with LinkBT — 100% free.",
    "idea": "A {selected_style} poster with a {selected_layout} showcasing {feature_summary}.",
    "feature_focus": "{feature_summary}",
    "cta": "Try LinkBT Free → link.bt",
    "angle": "{selected_angle}",
    "target_audience": "Digital Marketers, Content Creators, SaaS Growth Lead, Developers",
    "emotional_trigger": "Confidence, Authority, Curiosity, High Performance",
    "color_palette_name": "{color_palette['name']}",
    "camera_mode": "{cam_lighting['camera']}",
    "lighting_mode": "{cam_lighting['lighting']}",
    "hero_icon": "{hero_icon}"
}}
"""

    system = (
        "You are an award-winning Creative Director specializing in SaaS advertising (Apple, Stripe, Linear, Vercel). "
        "You NEVER repeat visual layouts or headlines. Respond with valid JSON only."
    )

    result = generate_json(prompt, system=system)

    defaults = {
        "marketing_style": selected_style,
        "layout": selected_layout,
        "theme": f"{selected_style} — {seasonal['event']}",
        "headline": default_headline,
        "subtitle": f"Track and secure your campaign short links with LinkBT — 100% free.",
        "idea": f"A {selected_style} poster with a {selected_layout} highlighting {feature_summary}.",
        "feature_focus": feature_summary,
        "cta": "Try LinkBT Free → link.bt",
        "angle": selected_angle,
        "target_audience": "Digital Marketers, Growth Engineers, Developers",
        "emotional_trigger": "Authority, Data Clarity, Confidence",
        "color_palette_name": color_palette["name"],
        "camera_mode": cam_lighting["camera"],
        "lighting_mode": cam_lighting["lighting"],
        "hero_icon": hero_icon,
    }

    for k, v in defaults.items():
        if k not in result or not result[k]:
            result[k] = v

    creativity_score = calculate_creativity_score(result, past_posts)
    result["creativity_score"] = creativity_score

    return result
