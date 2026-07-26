"""
Telegram Bot — Single entry point for LinkBT Prompt Generation Pipeline.

This bot:
  1. Runs daily scheduled job or handles /run command
  2. Analyzes live codebase & Git history
  3. Generates hyper-detailed AI Image Prompt + LinkedIn Caption
  4. Sends the marketing-prompt.md artifact to Telegram
"""

import asyncio
import json
import logging
from datetime import datetime, time
from pathlib import Path

from telegram import Update, InlineKeyboardButton, InlineKeyboardMarkup
from telegram.ext import (
    Application,
    CommandHandler,
    CallbackQueryHandler,
    ContextTypes,
)

from config import (
    TELEGRAM_BOT_TOKEN,
    TELEGRAM_CHAT_ID,
    DAILY_POST_TIME,
    OUTPUT_DIR,
)

# ── Logging ──────────────────────────────────────────
logging.basicConfig(
    format="%(asctime)s - %(name)s - %(levelname)s - %(message)s",
    level=logging.INFO,
)
logger = logging.getLogger("MarketingBot")

# Store latest result in memory
latest_result: dict | None = None


async def execute_pipeline(context: ContextTypes.DEFAULT_TYPE) -> dict | None:
    """Run marketing pipeline in a thread executor."""
    loop = asyncio.get_event_loop()
    try:
        from pipeline import run_pipeline
        result = await loop.run_in_executor(None, run_pipeline)
        return result
    except Exception as e:
        logger.error(f"Pipeline failed: {e}")
        await context.bot.send_message(
            chat_id=TELEGRAM_CHAT_ID,
            text=f"❌ *Pipeline Error*\n\n```\n{str(e)[:500]}\n```",
            parse_mode="Markdown",
        )
        return None


async def send_for_approval(result: dict, context: ContextTypes.DEFAULT_TYPE):
    """Send generated campaign summary and marketing-prompt.md artifact to Telegram."""
    global latest_result
    latest_result = result
    chat_id = TELEGRAM_CHAT_ID

    idea = result.get("idea", {})
    theme = idea.get("theme", "General")
    headline = idea.get("headline", "N/A")

    # Header message
    await context.bot.send_message(
        chat_id=chat_id,
        text=(
            f"🚀 *New LinkBT Marketing Campaign Ready*\n"
            f"📅 *Date:* {result.get('date', 'Today')}\n"
            f"🎯 *Theme:* {theme}\n"
            f"💡 *Concept:* {idea.get('idea', 'N/A')}\n"
            f"📝 *Headline:* {headline}\n"
            f"✨ *Feature Focus:* {idea.get('feature_focus', 'N/A')}"
        ),
        parse_mode="Markdown",
    )

    # Upload output/marketing-prompt.md artifact document
    artifact_path = result.get("artifact_path") or (OUTPUT_DIR / "marketing-prompt.md")
    if Path(artifact_path).exists():
        with open(artifact_path, "rb") as doc:
            await context.bot.send_document(
                chat_id=chat_id,
                document=doc,
                caption="📄 *marketing-prompt.md* (Copy AI prompt into ChatGPT/Gemini + attach app screenshots)",
                parse_mode="Markdown",
            )

    # Caption Preview
    full_post = result.get("full_post", "")
    keyboard = InlineKeyboardMarkup([
        [
            InlineKeyboardButton("✅ Approve Campaign", callback_data="approve"),
            InlineKeyboardButton("🔄 Regenerate Strategy", callback_data="reject"),
        ],
        [
            InlineKeyboardButton("📝 New Prompt Only", callback_data="new_prompt"),
            InlineKeyboardButton("✍️ New Caption Only", callback_data="new_caption"),
        ],
    ])

    await context.bot.send_message(
        chat_id=chat_id,
        text=(
            f"📱 *LinkedIn Caption Preview:*\n\n"
            f"─────────────────────\n"
            f"{full_post}\n"
            f"─────────────────────\n\n"
            f"Choose an action below:"
        ),
        reply_markup=keyboard,
        parse_mode="Markdown",
    )


# ── Command Handlers ─────────────────────────────────
async def cmd_start(update: Update, context: ContextTypes.DEFAULT_TYPE):
    """Welcome message."""
    await update.message.reply_text(
        "👋 *Welcome to LinkBT AI Marketing Bot!*\n\n"
        f"I automatically inspect your codebase daily at {DAILY_POST_TIME} "
        "and generate ultra-detailed AI Image Prompts + LinkedIn captions.\n\n"
        "*Commands:*\n"
        "/run — Generate campaign prompt now\n"
        "/status — Check system status\n"
        "/help — Show help message",
        parse_mode="Markdown",
    )


async def cmd_run(update: Update, context: ContextTypes.DEFAULT_TYPE):
    """Manually trigger pipeline."""
    await update.message.reply_text("⏳ Analyzing codebase and generating marketing campaign prompt... Please wait 1-2 minutes.")
    result = await execute_pipeline(context)
    if result:
        await send_for_approval(result, context)


async def cmd_status(update: Update, context: ContextTypes.DEFAULT_TYPE):
    """Check system status."""
    from llm import check_connection
    ollama_ok = check_connection()

    from brand import load_past_posts
    posts = load_past_posts()

    status_text = (
        "📊 *LinkBT Marketing Engine Status*\n\n"
        f"{'✅' if ollama_ok else '❌'} Ollama LLM: {'Connected' if ollama_ok else 'Not running'}\n"
        f"{'✅' if TELEGRAM_BOT_TOKEN else '❌'} Telegram Bot: {'Configured' if TELEGRAM_BOT_TOKEN else 'Not configured'}\n\n"
        f"📈 Total campaigns recorded: {len(posts)}\n"
        f"⏰ Daily schedule time: {DAILY_POST_TIME}"
    )
    await update.message.reply_text(status_text, parse_mode="Markdown")


async def cmd_help(update: Update, context: ContextTypes.DEFAULT_TYPE):
    """Show help."""
    await cmd_start(update, context)


# ── Callback Handlers ────────────────────────────────
async def handle_callback(update: Update, context: ContextTypes.DEFAULT_TYPE):
    """Handle inline button presses."""
    global latest_result
    query = update.callback_query
    await query.answer()

    if not latest_result:
        await query.edit_message_text("⚠️ No active campaign. Run /run first.")
        return

    action = query.data

    if action == "approve":
        await query.edit_message_text("✅ Campaign approved and recorded in history! Simply open marketing-prompt.md, copy the AI prompt to ChatGPT/Gemini, and attach LinkBT screenshots.")
        latest_result = None

    elif action == "reject":
        await query.edit_message_text("🔄 Regenerating campaign strategy from scratch...")
        latest_result = None
        result = await execute_pipeline(context)
        if result:
            await send_for_approval(result, context)

    elif action == "new_prompt":
        await query.edit_message_text("📝 Regenerating AI Image Generation Prompt...")
        from prompt_generator import generate_image_prompt
        from codebase_analyzer import get_complete_codebase_context

        loop = asyncio.get_event_loop()
        codebase_ctx = await loop.run_in_executor(None, get_complete_codebase_context)
        new_prompt = await loop.run_in_executor(None, generate_image_prompt, latest_result["idea"], codebase_ctx)
        latest_result["image_prompt"] = new_prompt
        
        # Rewrite artifact file
        from pipeline import run_pipeline
        result = await execute_pipeline(context)
        if result:
            await send_for_approval(result, context)

    elif action == "new_caption":
        await query.edit_message_text("✍️ Regenerating LinkedIn caption...")
        from caption_generator import generate_caption

        loop = asyncio.get_event_loop()
        caption_data = await loop.run_in_executor(None, generate_caption, latest_result["idea"])
        latest_result["caption"] = caption_data.get("caption", "")
        latest_result["hashtags"] = caption_data.get("hashtags", "")
        latest_result["full_post"] = caption_data.get("full_post", "")
        await send_for_approval(latest_result, context)


# ── Main Entry Point ─────────────────────────────────
def main():
    if not TELEGRAM_BOT_TOKEN or TELEGRAM_BOT_TOKEN == "your_telegram_bot_token_here":
        print("❌ Telegram bot token not configured in .env!")
        return

    print("🤖 Starting LinkBT Marketing Prompt Engine Bot...")
    print(f"   📅 Daily schedule: {DAILY_POST_TIME}")
    print(f"   📁 Output dir: {OUTPUT_DIR}")

    app = Application.builder().token(TELEGRAM_BOT_TOKEN).build()
    app.add_handler(CommandHandler("start", cmd_start))
    app.add_handler(CommandHandler("run", cmd_run))
    app.add_handler(CommandHandler("status", cmd_status))
    app.add_handler(CommandHandler("help", cmd_help))
    app.add_handler(CallbackQueryHandler(handle_callback))

    hour, minute = DAILY_POST_TIME.split(":")
    job_time = time(hour=int(hour), minute=int(minute), second=0)
    app.job_queue.run_daily(lambda ctx: execute_pipeline(ctx), time=job_time)

    print("   ✅ Bot is running! Send /start on Telegram.\n")
    app.run_polling(allowed_updates=Update.ALL_TYPES)


if __name__ == "__main__":
    main()
