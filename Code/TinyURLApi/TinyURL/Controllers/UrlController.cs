using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TinyURL.Data;
using TinyURL.Models;
using TinyURL.Services;

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

        public class AddTagsRequest
        {
            public List<string> Tags { get; set; }
        }


        // CREATE SHORT URL
        [HttpPost("api/url/shorten")]
        public async Task<IActionResult> ShortenUrl([FromBody] ShortenRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.LongUrl))
                return BadRequest("URL is required.");

            if (!Uri.IsWellFormedUriString(request.LongUrl, UriKind.Absolute))
                return BadRequest("Invalid URL format.");

            var existing = await _context.UrlMappings
                .FirstOrDefaultAsync(x => x.LongUrl == request.LongUrl);

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
                ClickCount = 0
            };

            _context.UrlMappings.Add(mapping);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                shortUrl = $"{Request.Scheme}://{Request.Host}/{shortCode}"
            });
        }

        // REDIRECT SHORT URL (ROOT LEVEL)
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

        // GET ALL URLS
        [HttpGet("api/url/all")]
        public async Task<IActionResult> GetAllUrls()
        {
            var urls = await _context.UrlMappings
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


        [HttpPost("api/url/{id}/tags")]
        public async Task<IActionResult> AddTags(int id, [FromBody] AddTagsRequest request)
        {
            // 1️⃣ Find URL
            var url = await _context.UrlMappings
                .Include(u => u.UrlTags)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (url == null)
                return NotFound("URL not found.");

            foreach (var tagName in request.Tags)
            {
                var normalized = tagName.Trim().ToLower();

                // 2️⃣ Check if tag already exists
                var tag = await _context.Tags
                    .FirstOrDefaultAsync(t => t.Name == normalized);

                if (tag == null)
                {
                    tag = new Tag { Name = normalized };
                    _context.Tags.Add(tag);
                    await _context.SaveChangesAsync();
                }

                // 3️⃣ Prevent duplicate tag mapping
                if (!url.UrlTags.Any(ut => ut.TagId == tag.Id))
                {
                    url.UrlTags.Add(new UrlTag
                    {
                        UrlMappingId = url.Id,
                        TagId = tag.Id
                    });
                }
            }

            await _context.SaveChangesAsync();

            return Ok("Tags added successfully.");
        }

        [HttpGet("api/url/search")]
        public async Task<IActionResult> SearchByTag(string tag)
        {
            var result = await _context.UrlTags
                .Where(ut => ut.Tag.Name == tag.ToLower())
                .Select(ut => ut.UrlMapping)
                .ToListAsync();

            return Ok(result);
        }

        [HttpDelete("api/url/{id}/tags/{tagName}")]
        public async Task<IActionResult> RemoveTag(int id, string tagName)
        {
            var tag = await _context.Tags
                .FirstOrDefaultAsync(t => t.Name == tagName.ToLower());

            if (tag == null)
                return NotFound("Tag not found.");

            var urlTag = await _context.UrlTags
                .FirstOrDefaultAsync(ut => ut.UrlMappingId == id && ut.TagId == tag.Id);

            if (urlTag == null)
                return NotFound("Tag not attached to this URL.");

            _context.UrlTags.Remove(urlTag);
            await _context.SaveChangesAsync();

            return Ok("Tag removed successfully.");
        }


        [HttpPut("api/url/{id}/tags")]
        public async Task<IActionResult> UpdateTags(int id, [FromBody] AddTagsRequest request)
        {
            var url = await _context.UrlMappings
                .Include(u => u.UrlTags)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (url == null)
                return NotFound("URL not found.");

            // 1️⃣ Remove existing tags
            _context.UrlTags.RemoveRange(url.UrlTags);

            foreach (var tagName in request.Tags)
            {
                var normalized = tagName.Trim().ToLower();

                var tag = await _context.Tags
                    .FirstOrDefaultAsync(t => t.Name == normalized);

                if (tag == null)
                {
                    tag = new Tag { Name = normalized };
                    _context.Tags.Add(tag);
                    await _context.SaveChangesAsync();
                }

                url.UrlTags.Add(new UrlTag
                {
                    UrlMappingId = url.Id,
                    TagId = tag.Id
                });
            }

            await _context.SaveChangesAsync();

            return Ok("Tags updated successfully.");
        }


    }
}