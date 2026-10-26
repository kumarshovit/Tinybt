using Mediator;
using TinyBtUrlApi.Core.Entities;
using TinyBtUrlApi.Core.Interfaces;
using TinyBtUrlApi.Core.Models;

namespace TinyBtUrlApi.UseCases.Urls.RedirectUrl;

public class RedirectUrlHandler(
    IUrlRepository repo,
    IUrlSafetyOrchestrator safetyOrchestrator)
    : IRequestHandler<RedirectUrlQuery, RedirectResult>
{
  public async ValueTask<RedirectResult> Handle(
      RedirectUrlQuery request,
      CancellationToken ct)
  {
    var url = await repo.GetByShortCodeAsync(request.ShortCode);

    if (url is null || url.IsDeleted)
      return new RedirectResult(RedirectStatus.NotFound);

    if (url.ExpirationDate.HasValue &&
        url.ExpirationDate.Value < DateTime.UtcNow)
      return new RedirectResult(RedirectStatus.Expired);

    // 🛡️ Safety Flag Check — pre-existing flag
    if (url.IsFlagged)
      return new RedirectResult(RedirectStatus.Blocked, url, url.FlaggedReason);

    // 🔐 Password-protected — early exit, no click logged
    if (url.IsPasswordProtected)
      return new RedirectResult(RedirectStatus.PasswordProtected, url);

    // 🛡️ Redirect-time safety recheck (with caching & auto-flagging handled by orchestrator)
    var safetyResult = await safetyOrchestrator.CheckRedirectSafetyAsync(url, ct);
    if (!safetyResult.IsSafe)
      return new RedirectResult(RedirectStatus.Blocked, url, safetyResult.Description);
    // Notice: Click counting and logging have moved to the continuation endpoint (ContinueRedirectHandler),
    // and only occur when the user explicitly clicks "Continue" on the destination preview page.
    return new RedirectResult(RedirectStatus.Found, url);
  }
}
