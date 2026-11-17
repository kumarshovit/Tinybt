using FastEndpoints;
using Mediator;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;
using TinyBtUrlApi.UseCases.Urls.UpdateAlias;
using TinyBtUrlApi.Web.Endpoints.Urls.Requests;
using TinyBtUrlApi.Web.Endpoints.Urls.Responses;

namespace TinyBtUrlApi.Web.Endpoints.Urls;

public class UpdateAliasEndpoint
    : Endpoint<UpdateAliasRequest, UrlResponse>
{
  private readonly IMediator _mediator;
  private readonly IConfiguration _config;

  public UpdateAliasEndpoint(IMediator mediator, IConfiguration config)
  {
    _mediator = mediator;
    _config = config;
  }

  public override void Configure()
  {
    Put("/api/urls/{id}/alias");
    Roles("User", "Admin");
    Description(x => x.WithTags("Url Management"));
  }

  public override async Task HandleAsync(UpdateAliasRequest req, CancellationToken ct)
  {
    var id = Route<int>("id");
    var userId = int.Parse(HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    var isAdmin = HttpContext.User.IsInRole("Admin");

    if (string.IsNullOrWhiteSpace(req.NewAlias))
    {
      AddError("Alias is required.");
      await Send.ErrorsAsync();
      return;
    }

    var normalizedAlias = req.NewAlias.Trim().ToLowerInvariant();

    if (TinyBtUrlApi.Core.Constants.ReservedAliases.IsReserved(normalizedAlias))
    {
      AddError("This alias is reserved and cannot be used.");
      await Send.ErrorsAsync();
      return;
    }

    if (!System.Text.RegularExpressions.Regex.IsMatch(normalizedAlias, "^[a-zA-Z0-9-]+$"))
    {
      AddError("Alias can contain only letters, numbers and hyphens.");
      await Send.ErrorsAsync();
      return;
    }

    var command = new UpdateAliasCommand(id, normalizedAlias, userId, isAdmin);

    var result = await _mediator.Send(command, ct);

    if (result is null)
    {
      AddError("Alias already exists or URL not found.");
      await Send.ErrorsAsync();
      return;
    }

    var baseUrl = HttpContext.Request.Host.Host.Contains("localhost")
      ? $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}"
      : (_config["BaseUrl:ShortUrlDomain"] ?? "https://link.bt");

    var response = new UrlResponse
    {
      Id = result.Id,
      LongUrl = result.LongUrl,
      ShortCode = result.ShortCode,
      ShortUrl = $"{baseUrl}/{result.ShortCode}",
      ExpirationDate = result.ExpirationDate,
      ClickCount = result.ClickCount
    };

    await Send.OkAsync(response, ct);
  }
}
