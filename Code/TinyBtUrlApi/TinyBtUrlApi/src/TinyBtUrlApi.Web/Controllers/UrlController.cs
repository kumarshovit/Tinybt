using Mediator;
using FastEndpoints;

using Microsoft.AspNetCore.Mvc;
using TinyBtUrlApi.UseCases.Urls.AddTags;
using TinyBtUrlApi.UseCases.Urls.CreateShortUrl;
using TinyBtUrlApi.UseCases.Urls.GetAllUrls;
using TinyBtUrlApi.UseCases.Urls.RedirectUrl;
using TinyBtUrlApi.UseCases.Urls.RemoveTag;
using TinyBtUrlApi.UseCases.Urls.RenameTag;
using TinyBtUrlApi.UseCases.Urls.SearchByTag;
using TinyBtUrlApi.UseCases.Urls.UpdateTags;
using static TinyBtUrlApi.UseCases.Urls.UpdateTagss.UpdareTagsCommand;

namespace TinyBtUrlApi.Web.Controllers;

[ApiController]
[Route("api/url")]
public class UrlController : ControllerBase
{
  private readonly IMediator _mediator;

  public UrlController(IMediator mediator)
  {
    _mediator = mediator;
  }

  [Microsoft.AspNetCore.Mvc.HttpPost("shorten")]
  public async Task<IActionResult> Shorten(CreateShortUrlCommand cmd)
      => Ok(await _mediator.Send(cmd));

  [Microsoft.AspNetCore.Mvc.HttpGet("all")]
  public async Task<IActionResult> GetAll()
      => Ok(await _mediator.Send(new GetAllUrlsQuery()));

  [Microsoft.AspNetCore.Mvc.HttpGet("{shortCode}")]
  public async Task<IActionResult> RedirectUrl(string shortCode)
  {
    var result = await _mediator.Send(new RedirectUrlQuery(shortCode));
    if (result == null) return NotFound();
    return Redirect(result);
    return Redirect(result.LongUrl);
  }

  [Microsoft.AspNetCore.Mvc.HttpPost("{id}/tags")]
  public async Task<IActionResult> AddTags(int id, AddTagsCommand cmd)
  {
    await _mediator.Send(cmd with { UrlId = id });
    return Ok();
  }

  [Microsoft.AspNetCore.Mvc.HttpPut("{id}/tags")]
  public async Task<IActionResult> UpdateTags(int id, UpdateTagsCommand cmd)
  {
    await _mediator.Send(cmd with { UrlId = id });
    return Ok();
  }

  [Microsoft.AspNetCore.Mvc.HttpDelete("{id}/tags/{tag}")]
  public async Task<IActionResult> RemoveTag(int id, string tag)
  {
    await _mediator.Send(new RemoveTagCommand(id, tag));
    return Ok();
  }

  [Microsoft.AspNetCore.Mvc.HttpPut("{id}/tags/{oldTag}")]
  public async Task<IActionResult> RenameTag(int id, string oldTag, [FromBody] string newTag)
  {
    await _mediator.Send(new RenameTagCommand(id, oldTag, newTag));
    return Ok();
  }

  [Microsoft.AspNetCore.Mvc.HttpGet("search")]
  public async Task<IActionResult> Search(string tag)
      => Ok(await _mediator.Send(new SearchByTagQuery(tag)));
}
