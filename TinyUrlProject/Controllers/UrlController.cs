using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TinyUrlApi.Data;
using TinyUrlApi.Models;
using TinyUrlApi.Services;

namespace TinyUrlApi.Controllers
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

        // CREATE SHORT URL
        [HttpPost("api/url/shorten")]
        public async Task<IActionResult> ShortenUrl([FromBody] string longUrl)
        {
            if (!Uri.IsWellFormedUriString(longUrl, UriKind.Absolute))
                return BadRequest("Invalid URL");

            // Check duplicate
            var existing = await _context.UrlMappings
                .FirstOrDefaultAsync(x => x.LongUrl == longUrl);

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
                LongUrl = longUrl,
                ShortCode = shortCode
            };

            _context.UrlMappings.Add(mapping);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                shortUrl = $"{Request.Scheme}://{Request.Host}/{shortCode}"
            });
        }

        // REDIRECT SHORT URL → LONG URL
        [HttpGet("{shortCode}")]
        public async Task<IActionResult> RedirectToLongUrl(string shortCode)
        {
            var mapping = await _context.UrlMappings
                .FirstOrDefaultAsync(x => x.ShortCode == shortCode);

            if (mapping == null)
                return NotFound("Invalid short URL");

            mapping.ClickCount++;
            await _context.SaveChangesAsync();

            return Redirect(mapping.LongUrl);
        }
        // GET: api/url/all
        [HttpGet("api/url/all")]
        public async Task<IActionResult> GetAllUrls()
        {
            var urls = await _context.UrlMappings
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();

            return Ok(urls);
        }

    }
}
