"""
Video Story & Shot List Generator for LinkBT AI Video Prompts.
Generates detailed scene breakdowns, camera directions, voiceover scripts,
and UI screenshot animation directives for minimum 30s videos.
"""

import random
from llm import generate_json


def generate_video_story(concept: dict, feature_focus: str, duration_info: dict) -> dict:
    """
    Generate scene-by-scene Shot List, voiceover script, and scene breakdown.
    
    Args:
        concept: dict with video_style, story_structure, camera_direction, motion_style, music_info.
        feature_focus: str name of highlighted features.
        duration_info: dict with label, seconds (min 30s), scenes_count.

    Returns:
        dict containing scenes list, full voiceover text, audio cues, and prompt breakdown.
    """
    total_seconds = duration_info.get("seconds", 30)
    scenes_count = duration_info.get("scenes_count", 4)
    scene_dur = round(total_seconds / scenes_count, 1)

    video_style = concept.get("video_style", "Product Launch Commercial")
    story_structure = concept.get("story_structure", "Problem -> Conflict -> Solution -> Reveal")
    camera_dir = concept.get("camera_direction", "FPV Drone Fly-through")
    motion_style = concept.get("motion_style", "3D Floating Glassmorphic UI Cards")
    headline = concept.get("headline", "Enterprise Security for Every Link.")
    hero_icon = concept.get("hero_icon", "3D Emerald Security Shield")

    prompt = f"""
You are a Lead Video Commercial Director for top-tier SaaS brands (Apple, Stripe, Notion, Linear, Vercel).
Design a high-converting {total_seconds}-second commercial for LinkBT (https://link.bt).

=== CAMPAIGN SPECIFICATIONS ===
• Commercial Category: {video_style}
• Story Structure: {story_structure}
• Primary Feature Focus: {feature_focus}
• Hero 3D Graphic Element: {hero_icon}
• Main Headline: "{headline}"
• Primary Camera Movement: {camera_dir}
• Motion Graphic Style: {motion_style}
• Total Video Duration: {total_seconds} seconds ({scenes_count} scenes, ~{scene_dur}s per scene)

=== SCENE BREAKDOWN REQUIREMENT ===
Generate exactly {scenes_count} sequential cinematic scenes (Scene 1 to Scene {scenes_count}):
- Scene 1: Opening Hook (Stop scrolling, introduce problem/curiosity)
- Scene 2: Conflict & Pain Point (Current frustrations with messy/unsecure/untracked links)
- Scene 3: Feature Reveal & Screenshot Motion (Hero reveal of LinkBT UI screenshot highlighting {feature_focus})
- Scene 4: Benefit & Value Proof (Real-time data/security proof, zero cost, branded trust)
{"- Scene 5: Call to Action & Brand End Screen (Final logo, CTA button, URL: link.bt)" if scenes_count >= 5 else ""}
{"- Scene 6: Outro & Tagline" if scenes_count >= 6 else ""}

=== RESPOND WITH THIS EXACT JSON FORMAT ONLY ===
{{
    "scenes": [
        {{
            "scene_number": 1,
            "title": "Opening Hook",
            "duration": "{scene_dur} seconds",
            "purpose": "Capture viewer attention instantly within 3 seconds",
            "camera": "Cinematic camera movement details for Scene 1",
            "lighting": "Lighting setup and color grading for Scene 1",
            "animation": "Motion graphic & screenshot animation details for Scene 1",
            "voiceover": "Voiceover line for Scene 1",
            "onscreen_text": "TEXT ON SCREEN"
        }}
    ],
    "full_voiceover_script": "Complete continuous voiceover script text for the commercial",
    "sound_effects_cues": "Detailed SFX cues synchronized with scene transitions",
    "cta_line": "Try LinkBT 100% Free → link.bt"
}}
"""

    system = (
        "You are an award-winning Video Commercial Director and Advertising Screenwriter. "
        "Create engaging, high-production commercial scripts. Respond with valid JSON only."
    )

    result = generate_json(prompt, system=system)

    # Defaults fallback if LLM response is incomplete
    if "scenes" not in result or len(result["scenes"]) < 3:
        result["scenes"] = [
            {
                "scene_number": 1,
                "title": "Opening Hook",
                "duration": f"{scene_dur} seconds",
                "purpose": "Capture immediate attention with high visual energy.",
                "camera": f"{camera_dir} zooming into a dark dramatic studio environment.",
                "lighting": "Dramatic dual-tone key light with volumetric cyan beams.",
                "animation": f"Abstract 3D particles morphing into a glowing {hero_icon}.",
                "voiceover": f"What if your campaign links did more than just redirect traffic?",
                "onscreen_text": headline,
            },
            {
                "scene_number": 2,
                "title": "Conflict & Pain Point",
                "duration": f"{scene_dur} seconds",
                "purpose": "Highlight the cost of unmeasured or unsecure links.",
                "camera": "Fast push-in macro shot on cluttered, unbranded URLs.",
                "lighting": "Moody slate lighting with high-contrast shadows.",
                "animation": "Red warning glow around unverified links fading away.",
                "voiceover": "Unbranded short links hurt trust, expose users to spam, and hide analytics.",
                "onscreen_text": "UNRELIABLE LINKS COST CLICKS",
            },
            {
                "scene_number": 3,
                "title": "Feature Reveal",
                "duration": f"{scene_dur} seconds",
                "purpose": f"Reveal LinkBT UI with focus on {feature_focus}.",
                "camera": "Smooth 360-degree orbit around a floating glass browser card.",
                "lighting": "Bright studio key light highlighting pristine UI screenshot.",
                "animation": f"LinkBT {feature_focus} dashboard screenshot smoothly expands into 3D space.",
                "voiceover": f"Meet LinkBT. Built with {feature_focus} for complete peace of mind.",
                "onscreen_text": f"POWERED BY {feature_focus.upper()}",
            },
            {
                "scene_number": 4,
                "title": "Call To Action",
                "duration": f"{scene_dur} seconds",
                "purpose": "Drive immediate conversion and website visit.",
                "camera": "Frontal lock-on shot focusing on CTA button and LinkBT logo.",
                "lighting": "Vibrant emerald green and royal blue rim lighting.",
                "animation": "Pill button pulses with glowing outer halo. Website URL link.bt shines.",
                "voiceover": "Start shortening, tracking, and securing your links today. 100% free at link.bt.",
                "onscreen_text": "TRY LINKBT FREE → LINK.BT",
            },
        ]
        result["full_voiceover_script"] = (
            "What if your campaign links did more than just redirect traffic? "
            "Unbranded short links hurt trust, expose users to spam, and hide analytics. "
            f"Meet LinkBT. Built with {feature_focus} for complete peace of mind. "
            "Start shortening, tracking, and securing your links today. 100% free at link.bt."
        )
        result["sound_effects_cues"] = "Scene 1: Digital riser. Scene 2: Low bass thud. Scene 3: Glass UI chime. Scene 4: Upbeat synth finish."
        result["cta_line"] = "Try LinkBT Free → link.bt"

    return result
