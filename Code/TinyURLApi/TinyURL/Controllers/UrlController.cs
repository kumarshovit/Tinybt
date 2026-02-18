//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using TinyURL.Data;
//using TinyURL.Models;
//using TinyURL.Services;

//namespace TinyURL.Controllers
//{
//    [ApiController]
//    [Route("")]
//    public class UrlController : ControllerBase
//    {
//        private readonly AppDbContext _context;
//        private readonly ShortCodeService _shortCodeService;

//        public UrlController(AppDbContext context, ShortCodeService shortCodeService)
//        {
//            _context = context;
//            _shortCodeService = shortCodeService;
//        }

//        // DTO
//        public class ShortenRequest
//        {
//            public string LongUrl { get; set; }
//        }

//        // CREATE SHORT URL
//        [HttpPost("api/url/shorten")]
//        public async Task<IActionResult> ShortenUrl([FromBody] ShortenRequest request)
//        {
//            if (request == null || string.IsNullOrWhiteSpace(request.LongUrl))
//                return BadRequest("URL is required.");

//            if (!Uri.IsWellFormedUriString(request.LongUrl, UriKind.Absolute))
//                return BadRequest("Invalid URL format.");

//            var existing = await _context.UrlMappings
//                .FirstOrDefaultAsync(x => x.LongUrl == request.LongUrl);

//            if (existing != null)
//            {
//                return Ok(new
//                {
//                    shortUrl = $"{Request.Scheme}://{Request.Host}/{existing.ShortCode}"
//                });
//            }

//            var shortCode = _shortCodeService.GenerateShortCode();

//            var mapping = new UrlMapping
//            {
//                LongUrl = request.LongUrl,
//                ShortCode = shortCode,
//                CreatedAt = DateTime.UtcNow,
//                ClickCount = 0
//            };

//            _context.UrlMappings.Add(mapping);
//            await _context.SaveChangesAsync();

//            return Ok(new
//            {
//                shortUrl = $"{Request.Scheme}://{Request.Host}/{shortCode}"
//            });
//        }

//        // REDIRECT SHORT URL (ROOT LEVEL)
//        [HttpGet("{shortCode}")]
//        public async Task<IActionResult> RedirectToLongUrl(string shortCode)
//        {
//            var mapping = await _context.UrlMappings
//                .FirstOrDefaultAsync(x => x.ShortCode == shortCode);

//            if (mapping == null)
//                return NotFound("Invalid short URL.");

//            mapping.ClickCount++;
//            await _context.SaveChangesAsync();

//            return Redirect(mapping.LongUrl);
//        }

//        // GET ALL URLS
//        [HttpGet("api/url/all")]
//        public async Task<IActionResult> GetAllUrls()
//        {
//            var urls = await _context.UrlMappings
//                .OrderByDescending(x => x.CreatedAt)
//                .Select(x => new
//                {
//                    x.Id,
//                    x.LongUrl,
//                    x.ShortCode,
//                    x.ClickCount,
//                    x.CreatedAt,
//                    ShortUrl = $"{Request.Scheme}://{Request.Host}/{x.ShortCode}"
//                })
//                .ToListAsync();

//            return Ok(urls);
//        }
//    }
//}

//18/02/2026

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

        // ===============================
        // REQUEST DTO
        // ===============================
        public class ShortenRequest
        {
            public string LongUrl { get; set; }
        }

        // ===============================
        // CREATE SHORT URL
        // ===============================
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
                    ShortUrl = $"{Request.Scheme}://{Request.Host}/{existing.ShortCode}",
                    ClickCount = existing.ClickCount
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
                ShortUrl = $"{Request.Scheme}://{Request.Host}/{shortCode}",
                ClickCount = mapping.ClickCount
            });
        }

        // ===============================
        // REDIRECT SHORT URL
        // ===============================
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

        // ===============================
        // GET ALL URLS
        // ===============================
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

        // ===============================
        // GET TOTAL CLICKS FOR ONE URL
        // ===============================
        [HttpGet("api/url/total-clicks")]
        public async Task<IActionResult> GetTotalClicks()
        {
            var totalClicks = await _context.UrlMappings
                .SumAsync(x => x.ClickCount);

            return Ok(new
            {
                TotalClicks = totalClicks
            });
        }
    }
}

