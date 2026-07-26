"""
Marketing Idea Generator — uses Ollama and live codebase context
to create unique campaign strategies and angles.
"""

import random
from brand import get_brand_context, load_past_posts, CAMPAIGN_THEMES, MARKETING_ANGLES
from llm import generate_json


def generate_idea() -> dict:
    """
    Generate a unique marketing idea & campaign strategy based on live codebase & past history.

    Returns:
        dict with campaign strategy details:
        theme, idea, headline, subtitle, feature_focus, cta, angle, target_audience, emotional_trigger, pain_points
    """
    brand_ctx = get_brand_context()
    summary_text = brand_ctx["summary_text"]
    codebase = brand_ctx["codebase"]

    # History tracking: filter out themes recently used in past posts
    past_posts = load_past_posts()
    recent_themes = [p.get("theme", "") for p in past_posts[-5:] if p.get("theme")]
    available_themes = [t for t in CAMPAIGN_THEMES if t not in recent_themes]
    if not available_themes:
        available_themes = CAMPAIGN_THEMES

    selected_theme = random.choice(available_themes)
    selected_angle = random.choice(MARKETING_ANGLES)

    # Core or newly detected feature focus
    features = codebase.get("core_features", [])
    new_features = codebase.get("newly_detected_features", [])
    all_features = features + new_features
    selected_feature = random.choice(all_features) if all_features else {"name": "Real-Time Click Analytics", "description": "Track clicks"}

    recent_history = "\n".join([
        f"- Theme: {p.get('theme', 'N/A')} | Headline: {p.get('headline', 'N/A')}"
        for p in past_posts[-7:]
    ]) if past_posts else "No previous posts recorded."

    prompt = f"""
{summary_text}

=== RECENT CAMPAIGNS (DO NOT REPEAT THESE HEADLINES OR CONCEPTS) ===
{recent_history}

=== TODAY'S CAMPAIGN ASSIGNMENT ===
Target Theme: {selected_theme}
Marketing Angle: {selected_angle}
Feature Focus: {selected_feature.get('name', 'Link Analytics')} — {selected_feature.get('description', '')}

=== YOUR TASK ===
Generate a unique, high-converting marketing campaign strategy for LinkBT.

=== RESPOND WITH THIS EXACT JSON FORMAT ONLY ===
{{
    "theme": "{selected_theme}",
    "idea": "A 1-2 sentence description of the marketing concept",
    "headline": "A punchy 5-8 word headline for the poster (scroll-stopping)",
    "subtitle": "A 10-15 word supporting line explaining the value proposition",
    "feature_focus": "{selected_feature.get('name', 'Real-Time Click Analytics')}",
    "cta": "Try LinkBT Free → link.bt",
    "angle": "{selected_angle}",
    "target_audience": "Digital Marketers, Content Creators, Developers, SaaS Founders",
    "emotional_trigger": "Confidence, Professional Authority, Data Clarity",
    "pain_points": "Messy long URLs, blind marketing campaigns, overpriced link management tools"
}}
"""

    system = (
        "You are a World-Class Marketing Strategist and Creative Director for top SaaS companies. "
        "You create scroll-stopping LinkedIn campaigns that drive high engagement and user signups. "
        "Always respond with valid JSON only."
    )

    result = generate_json(prompt, system=system)

    # Defaults fallback
    defaults = {
        "theme": selected_theme,
        "idea": "Highlight LinkBT's real-time click analytics and custom link management.",
        "headline": "Smart Links. Real Insights.",
        "subtitle": "Track every click with LinkBT's enterprise analytics dashboard — 100% free.",
        "feature_focus": selected_feature.get("name", "Real-Time Analytics"),
        "cta": "Try LinkBT Free → link.bt",
        "angle": selected_angle,
        "target_audience": "Digital Marketers, Developers, Content Creators, SaaS Teams",
        "emotional_trigger": "Confidence, Data Clarity, Professional Authority",
        "pain_points": "Messy long links, unmeasured campaign clicks, expensive tool subscriptions",
    }
    for k, v in defaults.items():
        if k not in result or not result[k]:
            result[k] = v

    return result


if __name__ == "__main__":
    idea = generate_idea()
    print("=== GENERATED CAMPAIGN IDEA ===")
    for k, v in idea.items():
        print(f"{k}: {v}")
