using FastEndpoints;
using Microsoft.Extensions.Logging;
using TinyBtUrlApi.Core.Interfaces;

namespace TinyBtUrlApi.Web.Endpoints.Urls.Admin;

public class AdminDisableUrlRequest
{
    public string? Reason { get; set; }
}

public class AdminDisableUrlResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
}

public class AdminDisableUrlEndpoint : Endpoint<AdminDisableUrlRequest, AdminDisableUrlResponse>
{
    private readonly IUrlRepository _urlRepository;
    private readonly ILogger<AdminDisableUrlEndpoint> _logger;

    public AdminDisableUrlEndpoint(
        IUrlRepository urlRepository,
        ILogger<AdminDisableUrlEndpoint> logger)
    {
        _urlRepository = urlRepository;
        _logger = logger;
    }

    public override void Configure()
    {
        Put("/api/admin/urls/{shortCode}/disable");
        Roles("Admin");
        Description(x => x.WithTags("Admin Safety Management"));
    }

    public override async Task HandleAsync(AdminDisableUrlRequest req, CancellationToken ct)
    {
        var shortCode = Route<string>("shortCode");
        if (string.IsNullOrWhiteSpace(shortCode))
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        var url = await _urlRepository.GetByShortCodeAsync(shortCode);

        if (url == null)
        {
            await Send.NotFoundAsync(ct);
            return;
        }

        var reason = string.IsNullOrWhiteSpace(req.Reason) ? "admin_manual_block" : $"admin_manual:{req.Reason}";
        await _urlRepository.FlagUrlAsync(url.Id, reason, ct);

        _logger.LogInformation("Admin disabled short link: {ShortCode} with reason: {Reason}", shortCode, reason);

        await Send.OkAsync(new AdminDisableUrlResponse
        {
            Success = true,
            Message = $"Link '{shortCode}' has been disabled."
        }, ct);
    }
}
