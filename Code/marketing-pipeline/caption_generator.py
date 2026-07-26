"""
LinkedIn Caption Generator — creates professional post copy, hashtags, and CTA.
"""

from brand import get_brand_context
from llm import generate_json


def generate_caption(idea: dict) -> dict:
    """
    Generate a high-converting LinkedIn caption based on today's campaign strategy.

    Args:
        idea: dict containing theme, idea, headline, subtitle, feature_focus, etc.

    Returns:
        dict with keys: caption, hashtags, cta, full_post
    """
    brand_ctx = get_brand_context()
    summary_text = brand_ctx["summary_text"]

    prompt = f"""
{summary_text}

=== TODAY'S CAMPAIGN STRATEGY ===
Theme: {idea.get('theme', '')}
Idea Concept: {idea.get('idea', '')}
Headline: {idea.get('headline', '')}
Subtitle: {idea.get('subtitle', '')}
Feature Focus: {idea.get('feature_focus', '')}
Angle: {idea.get('angle', '')}
Target Audience: {idea.get('target_audience', '')}

=== YOUR TASK ===
Write a high-converting, professional LinkedIn post for LinkBT based on the campaign above.

=== RESPOND WITH THIS EXACT JSON FORMAT ONLY ===
{{
    "caption": "The main body of the LinkedIn post (see requirements below)",
    "hashtags": "#URLShortener #LinkManagement #DigitalMarketing #SaaS #Analytics #GrowthHacking (5-8 relevant hashtags)",
    "cta": "Try LinkBT free today: https://link.bt"
}}

=== REQUIREMENTS ===
1. First line MUST be an irresistible HOOK that stops scrolling (bold statement, intriguing question, or surprising stat).
2. Write 3-4 concise, scannable paragraphs with clear spacing between lines.
3. Highlight the primary value proposition of LinkBT (100% Free, Enterprise Analytics, Custom Branded Links, Security).
4. Sound authentic, authoritative, and helpful — like a thought leader or senior product strategist.
5. Keep length under 180 words total.
6. Use emojis sparingly (max 3 total).
7. End with a strong call-to-action pointing to https://link.bt.
"""

    system = (
        "You are a top-tier LinkedIn Content Creator and Copywriter who writes viral, high-engagement tech posts. "
        "Always respond with valid JSON only."
    )

    result = generate_json(prompt, system=system)

    # Defaults fallback
    defaults = {
        "caption": (
            f"Stop sharing long, messy links that hurt your click-through rate.\n\n"
            f"With LinkBT, you can turn any long URL into a custom, branded short link with real-time click analytics — completely free.\n\n"
            f"Track device types, geo-locations, and traffic sources in one clean dashboard. Plus, every link is protected by Google Safe Browsing.\n\n"
            f"Upgrade your link infrastructure today at https://link.bt"
        ),
        "hashtags": "#URLShortener #LinkManagement #DigitalMarketing #Analytics #FreeTool #GrowthHacking",
        "cta": "Try LinkBT free → https://link.bt",
    }
    for k, v in defaults.items():
        if k not in result or not result[k]:
            result[k] = v

    full_post = f"{result['caption']}\n\n{result['hashtags']}"
    result["full_post"] = full_post

    return result


if __name__ == "__main__":
    test_idea = {
        "theme": "Analytics",
        "idea": "Track clicks in real time",
        "headline": "Smart Links. Real Insights.",
        "subtitle": "Free URL Shortener with Analytics",
        "feature_focus": "Real-Time Click Analytics",
        "angle": "Problem -> Solution",
    }
    cap = generate_caption(test_idea)
    print("=== GENERATED LINKEDIN CAPTION ===")
    print(cap["full_post"])
