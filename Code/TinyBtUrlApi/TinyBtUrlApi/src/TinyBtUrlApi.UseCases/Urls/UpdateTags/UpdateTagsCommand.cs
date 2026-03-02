using Mediator;

namespace TinyBtUrlApi.UseCases.Urls.UpdateTags;

public record UpdateTagsCommand(int UrlId, List<string> Tags) : IRequest;
