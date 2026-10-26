using Mediator;

namespace TinyBtUrlApi.UseCases.Urls.PreviewUrl;

public record GetUrlPreviewQuery(
    string ShortCode,
    string Token
) : IRequest<GetUrlPreviewResult>;
