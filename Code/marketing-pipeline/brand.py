"""
LinkBT Brand Context & Campaign History Engine.
Integrates live codebase analysis with past campaign tracking to ensure
every generated campaign is completely unique.
"""

import json
from pathlib import Path
from config import PAST_POSTS_FILE
from codebase_analyzer import get_complete_codebase_context

# ── Rotation Campaign Themes ─────────────────────────────
CAMPAIGN_THEMES = [
    "Analytics & Real-Time Insights",
    "QR Code Generation & Smart Routing",
    "Developer API & Webhook Integration",
    "Custom Domains & Branded Short Links",
    "Team Collaboration & Link Organization",
    "Marketing Attribution & Campaign Tracking",
    "Security, Google Safe Browsing & Captcha Shield",
    "Link Expiration & Promotional Flash Sales",
    "Productivity & Workflow Automation",
    "Growth Hacking & Social Media CTR Optimization",
]

MARKETING_ANGLES = [
    "Feature spotlight — deep dive into one feature",
    "Use case story — how a specific role benefits from LinkBT",
    "Problem → Solution — common link management pain point",
    "Tips & tricks — actionable advice for link management",
    "Industry insight — why link analytics matter in digital marketing",
    "Comparison angle — messy long URLs vs. clean LinkBT links",
    "Social proof — why professionals choose branded short links",
    "Behind the scenes — how LinkBT makes link management simple",
    "Quick tip — one actionable insight about short links",
    "Myth busting — debunking URL shortener misconceptions",
]


def load_past_posts() -> list[dict]:
    """Load previously generated campaigns to avoid repetition."""
    if PAST_POSTS_FILE.exists():
        try:
            with open(PAST_POSTS_FILE, "r", encoding="utf-8") as f:
                return json.load(f)
        except Exception as e:
            print(f"[BrandEngine] Warning loading past posts: {e}")
    return []


def save_post(post: dict) -> None:
    """Append a generated campaign to history."""
    posts = load_past_posts()
    posts.append(post)
    # Keep up to 100 past posts
    posts = posts[-100:]
    with open(PAST_POSTS_FILE, "w", encoding="utf-8") as f:
        json.dump(posts, f, indent=2, ensure_ascii=False)


def get_recent_campaign_history_text(n: int = 10) -> str:
    """Return a summary of recent campaigns so Ollama avoids repeating them."""
    posts = load_past_posts()[-n:]
    if not posts:
        return "No previous campaigns recorded yet."
    lines = []
    for p in posts:
        lines.append(
            f"- [{p.get('date', '?')}] Theme: {p.get('theme', 'N/A')} | "
            f"Headline: {p.get('headline', 'N/A')} | Angle: {p.get('angle', 'N/A')}"
        )
    return "\n".join(lines)


def get_brand_context() -> dict:
    """
    Fetch full brand context combined with live codebase analysis.
    """
    codebase_ctx = get_complete_codebase_context()

    features_formatted = "\n".join(
        f"  • {f['name']}: {f['description']} (Benefit: {f['benefit']})"
        for f in codebase_ctx["core_features"]
    )

    new_features_formatted = "\n".join(
        f"  ✨ {nf['name']} — {nf['description']} (Commit: {nf['source']})"
        for nf in codebase_ctx["newly_detected_features"]
    ) if codebase_ctx["newly_detected_features"] else "  (All baseline features up to date)"

    pages_text = ", ".join(codebase_ctx["pages"])

    summary_text = f"""
=== PRODUCT INFORMATION ===
Product Name: {codebase_ctx['brand_name']}
Website URL: {codebase_ctx['url']}
Tagline: {codebase_ctx['tagline']}
Discovered UI Pages: {pages_text}

=== LIVE CORE FEATURES (Extracted from Codebase) ===
{features_formatted}

=== NEWLY DETECTED FEATURES & ENHANCEMENTS (From Git & Recent Code) ===
{new_features_formatted}

=== BRAND VALUE PROPOSITION ===
• 100% Free forever — no paid tiers, no click limits
• Enterprise security with Google Safe Browsing and anti-bot Captcha
• Real-time traffic analytics (Geo, Device, Referral, IP breakdown)
• Branded short URLs to maximize click-through rate & audience trust
""".strip()

    return {
        "summary_text": summary_text,
        "codebase": codebase_ctx,
    }


if __name__ == "__main__":
    brand = get_brand_context()
    print("=== BRAND CONTEXT READY ===")
    print(brand["summary_text"])
