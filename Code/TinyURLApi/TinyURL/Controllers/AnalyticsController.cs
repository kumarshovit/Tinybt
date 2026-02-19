using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TinyURL.Data;

namespace TinyURL.Controllers
{
    [ApiController]
    [Route("api/analytics")]
    public class AnalyticsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AnalyticsController(AppDbContext context)
        {
            _context = context;
        }

        // ===========================
        // TOTAL CLICKS
        // ===========================
        [HttpGet("total-clicks")]
        public async Task<IActionResult> GetTotalClicks()
        {
            var total = await _context.ClickLogs.CountAsync();
            return Ok(new { TotalClicks = total });
        }

        // ===========================
        // CLICKS OVER TIME (DAILY)
        // ===========================
        // ===========================
        // CLICKS OVER TIME (DAILY / WEEKLY)
        // ===========================
        [HttpGet("clicks-over-time")]
        public async Task<IActionResult> GetClicksOverTime(
            DateTime startDate,
            DateTime endDate,
            string view = "daily") // daily or weekly
        {
            var query = _context.ClickLogs
                .Where(x => x.ClickedAt >= startDate && x.ClickedAt <= endDate);

            if (view.ToLower() == "weekly")
            {
                var weeklyData = await query
                    .GroupBy(x => EF.Functions.DateDiffWeek(startDate, x.ClickedAt))
                    .Select(g => new
                    {
                        Week = g.Key,
                        Clicks = g.Count()
                    })
                    .OrderBy(x => x.Week)
                    .ToListAsync();

                return Ok(weeklyData);
            }
            else
            {
                var dailyData = await query
                    .GroupBy(x => x.ClickedAt.Date)
                    .Select(g => new
                    {
                        Date = g.Key,
                        Clicks = g.Count()
                    })
                    .OrderBy(x => x.Date)
                    .ToListAsync();

                return Ok(dailyData);
            }
        }


        // ===========================
        // CLICKS BY DEVICE TYPE
        // ===========================
        [HttpGet("device-type")]
        public async Task<IActionResult> GetDeviceType()
        {
            var data = await _context.ClickLogs
                .GroupBy(x => x.DeviceType)
                .Select(g => new
                {
                    DeviceType = g.Key,
                    Clicks = g.Count()
                })
                .ToListAsync();

            return Ok(data);
        }

        // ===========================
        // CLICKS BY BROWSER
        // ===========================
        [HttpGet("browser")]
        public async Task<IActionResult> GetBrowser()
        {
            var data = await _context.ClickLogs
                .GroupBy(x => x.Browser)
                .Select(g => new
                {
                    Browser = g.Key,
                    Clicks = g.Count()
                })
                .ToListAsync();

            return Ok(data);
        }

        // ===========================
        // CLICKS BY OS
        // ===========================
        [HttpGet("os")]
        public async Task<IActionResult> GetOS()
        {
            var data = await _context.ClickLogs
                .GroupBy(x => x.OS)
                .Select(g => new
                {
                    OS = g.Key,
                    Clicks = g.Count()
                })
                .ToListAsync();

            return Ok(data);
        }

        // ===========================
        // POPULAR DAYS
        // ===========================
        [HttpGet("popular-days")]
        public async Task<IActionResult> GetPopularDays()
        {
            var data = await _context.ClickLogs
                .GroupBy(x => x.ClickedAt.DayOfWeek)
                .Select(g => new
                {
                    Day = g.Key.ToString(),
                    Clicks = g.Count()
                })
                .OrderByDescending(x => x.Clicks)
                .ToListAsync();

            return Ok(data);
        }
    }
}
