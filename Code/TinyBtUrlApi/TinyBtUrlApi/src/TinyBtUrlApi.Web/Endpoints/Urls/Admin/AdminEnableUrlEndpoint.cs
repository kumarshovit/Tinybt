using FastEndpoints;
using Microsoft.Extensions.Logging;
using TinyBtUrlApi.Core.Interfaces;

namespace TinyBtUrlApi.Web.Endpoints.Urls.Admin;

public class AdminEnableUrlResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
}

public class AdminEnableUrlEndpoint : EndpointWithoutRequest<AdminEnableUrlResponse>
{
    private readonly IUrlRepository _urlRepository;
    private readonly ILogger<AdminEnableUrlEndpoint> _logger;

    public AdminEnableUrlEndpoint(
        IUrlRepository urlRepository,
        ILogger<AdminEnableUrlEndpoint> logger)
    {
        _urlRepository = urlRepository;
        _logger = logger;
    }

    public override void Configure()
    {
        Put("/api/admin/urls/{shortCode}/enable");
        Roles("Admin");
        Description(x => x.WithTags("Admin Safety Management"));
    }

    public override async Task HandleAsync(CancellationToken ct)
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

        await _urlRepository.UnflagUrlAsync(url.Id, ct);

        _logger.LogInformation("Admin re-enabled short link: {ShortCode}", shortCode);

        await Send.OkAsync(new AdminEnableUrlResponse
        {
            Success = true,
            Message = $"Link '{shortCode}' has been re-enabled."
        }, ct);
    }
}
