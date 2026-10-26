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
  private readonly IUrlSafetyOrchestrator _safetyOrchestrator;

  public UpdateDestinationHandler(
      IUrlRepository repo,
      IUrlSecurityValidator urlSecurityValidator,
      IUrlSafetyOrchestrator safetyOrchestrator)
  {
    _repo = repo;
    _urlSecurityValidator = urlSecurityValidator;
    _safetyOrchestrator = safetyOrchestrator;
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

    // Validate full pipeline (max 2 redirects + concurrent threat scans)
    var pipelineResult = await _safetyOrchestrator.ValidateFullPipelineAsync(normalizedUrl, ct);
    if (!pipelineResult.IsValid) return false;

    url.LongUrl = normalizedUrl;
    await _repo.UpdateAsync(url);

    return true;
  }
}
