using FastEndpoints;
using Mediator;
using TinyBtUrlApi.Core.DTOs;
using TinyBtUrlApi.UseCases.Urls.SearchByTag;

namespace TinyBtUrlApi.Web.Endpoints.Urls;

public class SearchByTagEndpoint : EndpointWithoutRequest<List<UrlDto>>
{
  private readonly IMediator _mediator;

  public SearchByTagEndpoint(IMediator mediator)
  {
    _mediator = mediator;
  }

  public override void Configure()
  {
    Get("/api/urls/by-tag/{tag}");
    AllowAnonymous();
  }

  public override async Task HandleAsync(CancellationToken ct)
  {
    var tag = Route<string>("tag");

    if (string.IsNullOrWhiteSpace(tag))
    {
      HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
      return;
    }

    var result = await _mediator.Send(new SearchByTagQuery(tag), ct);

    // Map domain entities to DTOs here to avoid serialization cycles and leak of domain model
    var dtos = result?.Select(m => new UrlDto
    {
      Id = m.Id,
      LongUrl = m.LongUrl,
      ShortCode = m.ShortCode,
      ClickCount = m.ClickCount,
      CreatedAt = m.CreatedAt,
      ExpirationDate = m.ExpirationDate,
      Tags = m.UrlTags?.Select(ut => ut.Tag?.Name ?? string.Empty).Where(n => !string.IsNullOrEmpty(n)).ToList() ?? new List<string>()
    }).ToList() ?? new List<UrlDto>();

    await Send.OkAsync(dtos, ct);
  }
}
