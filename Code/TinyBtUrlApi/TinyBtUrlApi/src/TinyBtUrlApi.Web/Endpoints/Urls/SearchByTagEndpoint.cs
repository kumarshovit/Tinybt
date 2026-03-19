using FastEndpoints;
using Mediator;
using TinyBtUrlApi.Core.DTOs;
using TinyBtUrlApi.UseCases.Urls.SearchByTag;
using TinyBtUrlApi.Web.Endpoints.Urls.Responses;

namespace TinyBtUrlApi.Web.Endpoints.Urls;

public class SearchByTagEndpoint : EndpointWithoutRequest<List<UrlResponse>>
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
    Description(x => x.WithTags("Url Management"));
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

    var baseUrl = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}";

    var response = result?.Select(m => new UrlResponse
    {
      Id = m.Id,
      LongUrl = m.LongUrl,
      ShortCode = m.ShortCode,
      ShortUrl = $"{baseUrl}/{m.ShortCode}",
      ExpirationDate = m.ExpirationDate,
      Tags = m.Tags,
      ClickCount = m.ClickCount
    }).ToList() ?? new List<UrlResponse>();

    await Send.OkAsync(response, ct);
  }
}
