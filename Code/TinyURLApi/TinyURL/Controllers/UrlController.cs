using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TinyURL.Data;
using TinyURL.Models;
using TinyURL.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace TinyURL.Controllers
{
    [ApiController]
    [Route("")]
    public class UrlController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ShortCodeService _shortCodeService;

        public UrlController(AppDbContext context, ShortCodeService shortCodeService)
        {
            _context = context;
            _shortCodeService = shortCodeService;
        }

        // DTO
        public class ShortenRequest
        {
            public string LongUrl { get; set; }
        }

        // 🔐 CREATE SHORT URL (Protected)
        [Authorize]
        [HttpPost("api/url/shorten")]
        public async Task<IActionResult> ShortenUrl([FromBody] ShortenRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.LongUrl))
                return BadRequest("URL is required.");

            if (!Uri.IsWellFormedUriString(request.LongUrl, UriKind.Absolute))
                return BadRequest("Invalid URL format.");

            // Get logged-in user ID
            var userId = int.Parse(
                User.FindFirst(ClaimTypes.NameIdentifier)!.Value
            );

            var existing = await _context.UrlMappings
                .FirstOrDefaultAsync(x => x.LongUrl == request.LongUrl && x.UserId == userId);

            if (existing != null)
            {
                return Ok(new
                {
                    shortUrl = $"{Request.Scheme}://{Request.Host}/{existing.ShortCode}"
                });
            }

            var shortCode = _shortCodeService.GenerateShortCode();

            var mapping = new UrlMapping
            {
                LongUrl = request.LongUrl,
                ShortCode = shortCode,
                CreatedAt = DateTime.UtcNow,
                ClickCount = 0,
                UserId = userId   // 🔥 Link to logged-in user
            };

            _context.UrlMappings.Add(mapping);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                shortUrl = $"{Request.Scheme}://{Request.Host}/{shortCode}"
            });
        }

        // 🌍 PUBLIC REDIRECT (No Authorize)
        [HttpGet("{shortCode}")]
        public async Task<IActionResult> RedirectToLongUrl(string shortCode)
        {
            var mapping = await _context.UrlMappings
                .FirstOrDefaultAsync(x => x.ShortCode == shortCode);

            if (mapping == null)
                return NotFound("Invalid short URL.");

            mapping.ClickCount++;
            await _context.SaveChangesAsync();

            return Redirect(mapping.LongUrl);
        }

        // 🔐 GET USER'S URLS ONLY
        [Authorize]
        [HttpGet("api/url/all")]
        public async Task<IActionResult> GetAllUrls()
        {
            var userId = int.Parse(
                User.FindFirst(ClaimTypes.NameIdentifier)!.Value
            );

            var urls = await _context.UrlMappings
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.CreatedAt)
                .Select(x => new
                {
                    x.Id,
                    x.LongUrl,
                    x.ShortCode,
                    x.ClickCount,
                    x.CreatedAt,
                    ShortUrl = $"{Request.Scheme}://{Request.Host}/{x.ShortCode}"
                })
                .ToListAsync();

            return Ok(urls);
        }
    }
}
