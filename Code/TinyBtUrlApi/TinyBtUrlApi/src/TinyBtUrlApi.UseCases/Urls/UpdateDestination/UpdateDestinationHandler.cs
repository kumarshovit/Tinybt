using System;
using System.Collections.Generic;
using System.Text;
using Mediator;
using TinyBtUrlApi.Core.Interfaces;

namespace TinyBtUrlApi.UseCases.Urls.UpdateDestination;

public class UpdateDestinationHandler : IRequestHandler<UpdateDestinationCommand, bool>
{
  private readonly IUrlRepository _repo;
  private readonly IUrlSecurityValidator _urlSecurityValidator;
  private readonly IUrlRedirectResolver _urlRedirectResolver;
  private readonly IGoogleSafeBrowsingService _safeBrowsingService;
  private readonly IUrlhausService _urlhausService;
  private readonly ICloudflareUrlScannerService _cloudflareScanner;

  public UpdateDestinationHandler(
      IUrlRepository repo,
      IUrlSecurityValidator urlSecurityValidator,
      IUrlRedirectResolver urlRedirectResolver,
      IGoogleSafeBrowsingService safeBrowsingService,
      IUrlhausService urlhausService,
      ICloudflareUrlScannerService cloudflareScanner)
  {
    _repo = repo;
    _urlSecurityValidator = urlSecurityValidator;
    _urlRedirectResolver = urlRedirectResolver;
    _safeBrowsingService = safeBrowsingService;
    _urlhausService = urlhausService;
    _cloudflareScanner = cloudflareScanner;
  }

  public async ValueTask<bool> Handle(UpdateDestinationCommand request, CancellationToken ct)
  {
    var url = await _repo.GetByIdAsync(request.Id);
    if (url == null) return false;

    if (url.UserId != request.UserId && !request.IsAdmin) return false;

    // Validate format & security
    var securityResult = await _urlSecurityValidator.ValidateUrlAsync(request.NewLongUrl, ct);
    if (!securityResult.Success) return false;

    var normalizedUrl = securityResult.NormalizedUrl ?? request.NewLongUrl;

    // Resolve redirect chain
    var redirectResult = await _urlRedirectResolver.ResolveChainAsync(normalizedUrl, ct);
    if (!redirectResult.Success) return false;

    // Check Safe Browsing on all hops
    var urlsToCheck = redirectResult.RedirectChain.Count > 0
        ? redirectResult.RedirectChain
        : new List<string> { normalizedUrl };

    var safeBrowsingResult = await _safeBrowsingService.CheckUrlsAsync(urlsToCheck, ct);
    if (!safeBrowsingResult.IsSafe) return false;

    // Check URLhaus on all hops
    var urlhausResult = await _urlhausService.CheckUrlsAsync(urlsToCheck, ct);
    if (!urlhausResult.IsSafe) return false;

    // Check Cloudflare on all hops
    foreach (var urlToCheck in urlsToCheck)
    {
      var cloudflareResult = await _cloudflareScanner.QuickCheckUrlAsync(urlToCheck, ct);
      if (cloudflareResult.IsMalicious) return false;
    }

    url.LongUrl = normalizedUrl;
    await _repo.UpdateAsync(url);

    return true;
  }
}
