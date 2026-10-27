using System;
using System.Threading;
using System.Threading.Tasks;
using Mediator;
using TinyBtUrlApi.Core.Interfaces;

namespace TinyBtUrlApi.UseCases.Urls.PreviewUrl;

public class GetUrlPreviewHandler : IRequestHandler<GetUrlPreviewQuery, GetUrlPreviewResult>
{
    private readonly IUrlRepository _repo;
    private readonly IUrlAccessTokenService _tokenService;
    private readonly IUrlSafetyOrchestrator _safetyOrchestrator;

    public GetUrlPreviewHandler(
        IUrlRepository repo,
        IUrlAccessTokenService tokenService,
        IUrlSafetyOrchestrator safetyOrchestrator)
    {
        _repo = repo;
        _tokenService = tokenService;
        _safetyOrchestrator = safetyOrchestrator;
    }

    public async ValueTask<GetUrlPreviewResult> Handle(
        GetUrlPreviewQuery request,
        CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(request.ShortCode) || string.IsNullOrWhiteSpace(request.Token))
        {
            return GetUrlPreviewResult.Invalid("Short code and preview token are required.");
        }

        // 1. Validate preview access token
        var payload = _tokenService.TryUnprotect(request.Token, request.ShortCode);
        if (payload is null)
        {
            return GetUrlPreviewResult.Invalid("Invalid or expired preview token. Please visit the original short link.");
        }

        // 2. Fetch URL mapping
        var url = await _repo.GetByShortCodeAsync(request.ShortCode);
        if (url is null || url.IsDeleted)
        {
            return GetUrlPreviewResult.NotFound("The requested short link was not found.");
        }

        // 3. Expiration check
        if (url.ExpirationDate.HasValue && url.ExpirationDate.Value < DateTime.UtcNow)
        {
            return GetUrlPreviewResult.Expired("This short link has expired.");
        }

        // 4. Safety checks — pre-flagged in database
        if (url.IsFlagged)
        {
            return GetUrlPreviewResult.Blocked(url.FlaggedReason ?? "This link has been blocked because it leads to a malicious or harmful website.");
        }

        // 5. Redirect-time safety recheck (with caching & auto-flagging handled by orchestrator)
        var safetyResult = await _safetyOrchestrator.CheckRedirectSafetyAsync(url, ct);
        if (!safetyResult.IsSafe)
        {
            return GetUrlPreviewResult.Blocked(safetyResult.Description ?? "This link has been blocked because it leads to a malicious or harmful website.");
        }

        // 6. Extract domain for clean preview presentation
        string domain = url.LongUrl;
        if (Uri.TryCreate(url.LongUrl, UriKind.Absolute, out var uri))
        {
            domain = uri.Host;
        }

        return GetUrlPreviewResult.Ok(
            url.ShortCode,
            domain,
            url.LongUrl,
            url.ExpirationDate,
            url.ScreenshotPath,
            url.ScreenshotStatus
        );
    }
}
