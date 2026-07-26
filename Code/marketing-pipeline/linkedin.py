"""
LinkedIn Publisher — publishes posts with image attachments via the official REST API.

Supports:
  - Text-only posts
  - Posts with image attachments (posters)
  - Personal profiles and organization pages
"""

import requests
from pathlib import Path
from config import LINKEDIN_ACCESS_TOKEN, LINKEDIN_AUTHOR_URN

API_BASE = "https://api.linkedin.com"
API_VERSION = "202401"  # LinkedIn API versioning


def _headers() -> dict:
    """Common headers for LinkedIn API requests."""
    return {
        "Authorization": f"Bearer {LINKEDIN_ACCESS_TOKEN}",
        "Content-Type": "application/json",
        "LinkedIn-Version": API_VERSION,
        "X-Restli-Protocol-Version": "2.0.0",
    }


def _upload_image(image_path: str) -> str | None:
    """
    Upload an image to LinkedIn and return the image URN.

    Steps:
      1. Initialize upload → get upload URL + image URN
      2. PUT the binary image to the upload URL
      3. Return the image URN for use in the post
    """
    # Step 1: Initialize upload
    init_payload = {
        "initializeUploadRequest": {
            "owner": LINKEDIN_AUTHOR_URN,
        }
    }
    resp = requests.post(
        f"{API_BASE}/rest/images?action=initializeUpload",
        headers=_headers(),
        json=init_payload,
    )

    if resp.status_code != 200:
        print(f"❌ Image upload init failed: {resp.status_code} — {resp.text}")
        return None

    data = resp.json()["value"]
    upload_url = data["uploadUrl"]
    image_urn = data["image"]

    # Step 2: Upload the binary image
    with open(image_path, "rb") as f:
        upload_resp = requests.put(
            upload_url,
            headers={
                "Authorization": f"Bearer {LINKEDIN_ACCESS_TOKEN}",
                "Content-Type": "application/octet-stream",
            },
            data=f.read(),
        )

    if upload_resp.status_code not in (200, 201):
        print(f"❌ Image upload failed: {upload_resp.status_code} — {upload_resp.text}")
        return None

    return image_urn


def publish_post(text: str, image_path: str = None) -> dict:
    """
    Publish a post to LinkedIn.

    Args:
        text: The full post text (caption + hashtags).
        image_path: Optional path to an image file to attach.

    Returns:
        dict with status and post URL or error message.
    """
    if not LINKEDIN_ACCESS_TOKEN:
        return {"status": "error", "message": "LinkedIn access token not configured. See .env.example"}

    if not LINKEDIN_AUTHOR_URN:
        return {"status": "error", "message": "LinkedIn author URN not configured. See .env.example"}

    # Build post payload
    post_payload = {
        "author": LINKEDIN_AUTHOR_URN,
        "commentary": text,
        "visibility": "PUBLIC",
        "distribution": {
            "feedDistribution": "MAIN_FEED",
            "targetEntities": [],
            "thirdPartyDistributionChannels": [],
        },
        "lifecycleState": "PUBLISHED",
    }

    # Attach image if provided
    if image_path and Path(image_path).exists():
        image_urn = _upload_image(image_path)
        if image_urn:
            post_payload["content"] = {
                "media": {
                    "altText": "LinkBT — Free URL Shortener with Analytics",
                    "id": image_urn,
                }
            }

    # Publish
    resp = requests.post(
        f"{API_BASE}/rest/posts",
        headers=_headers(),
        json=post_payload,
    )

    if resp.status_code in (200, 201):
        post_id = resp.headers.get("x-restli-id", "unknown")
        return {
            "status": "success",
            "message": f"✅ Published to LinkedIn!",
            "post_id": post_id,
        }
    else:
        return {
            "status": "error",
            "message": f"❌ LinkedIn publish failed: {resp.status_code} — {resp.text}",
        }


def verify_token() -> bool:
    """Check if the LinkedIn access token is valid."""
    if not LINKEDIN_ACCESS_TOKEN:
        return False
    resp = requests.get(
        f"{API_BASE}/rest/me",
        headers=_headers(),
    )
    return resp.status_code == 200
