using FastEndpoints;
using Microsoft.Extensions.Logging;
using TinyBtUrlApi.Core.Entities;
using TinyBtUrlApi.Core.Interfaces;
using TinyBtUrlApi.Infrastructure.Data;

namespace TinyBtUrlApi.Web.Endpoints.Urls;

public class ReportUrlRequest
{
    public string Reason { get; set; } = string.Empty;       // "phishing", "malware", "scam", "other"
    public string? Details { get; set; }
}

public class ReportUrlResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
}

public class ReportUrlEndpoint : Endpoint<ReportUrlRequest, ReportUrlResponse>
{
    private readonly AppDbContext _context;
    private readonly IUrlRepository _urlRepository;
    private readonly IUrlSafetyOrchestrator _safetyOrchestrator;
    private readonly ILogger<ReportUrlEndpoint> _logger;

    public ReportUrlEndpoint(
        AppDbContext context,
        IUrlRepository urlRepository,
        IUrlSafetyOrchestrator safetyOrchestrator,
        ILogger<ReportUrlEndpoint> logger)
    {
        _context = context;
        _urlRepository = urlRepository;
        _safetyOrchestrator = safetyOrchestrator;
        _logger = logger;
    }

    public override void Configure()
    {
        Post("/api/urls/{shortCode}/report");
        AllowAnonymous();
        Description(x => x.WithTags("Url Safety"));
    }

    public override async Task HandleAsync(ReportUrlRequest req, CancellationToken ct)
    {
        var shortCode = Route<string>("shortCode");
        if (string.IsNullOrWhiteSpace(shortCode))
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        var url = await _urlRepository.GetByShortCodeAsync(shortCode);
        if (url == null || url.IsDeleted)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        var clientIp = HttpContext.Connection.RemoteIpAddress?.ToString();

        var report = new UrlReport
        {
            ShortCode = shortCode,
            Reason = string.IsNullOrWhiteSpace(req.Reason) ? "other" : req.Reason,
            Details = req.Details,
            ReporterIp = clientIp,
            ReportedAt = DateTime.UtcNow,
            IsReviewed = false
        };

        _context.UrlReports.Add(report);
        await _context.SaveChangesAsync(ct);

        _logger.LogWarning("User reported link {ShortCode}. Reason: {Reason}. Running automated safety verification...",
            shortCode, report.Reason);

        // Immediate automated check verification
        try
        {
            var safetyCheck = await _safetyOrchestrator.QuickCheckAsync(url.LongUrl, ct);
            if (!safetyCheck.IsSafe)
            {
                await _urlRepository.FlagUrlAsync(url.Id, $"report_verified:{safetyCheck.ThreatSource}:{safetyCheck.ThreatType}", ct);
                _logger.LogWarning("Reported URL {ShortCode} automatically flagged following threat intelligence verification.", shortCode);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error verifying reported URL {ShortCode}.", shortCode);
        }

        await Send.OkAsync(new ReportUrlResponse
        {
            Success = true,
            Message = "Thank you. Your report has been submitted and our security systems are investigating."
        }, ct);
    }
}
