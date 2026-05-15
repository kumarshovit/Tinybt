using FastEndpoints;
using Mediator;
using TinyBtUrlApi.UseCases.Contact.Create;

namespace TinyBtUrlApi.Web.Endpoints.Contact;

public class CreateContactEndpoint
    : Endpoint<CreateContactCommand>
{
  private readonly IMediator _mediator;

  public CreateContactEndpoint(IMediator mediator)
  {
    _mediator = mediator;
  }

  public override void Configure()
  {
    Post("/api/contact");

    AllowAnonymous();

    Description(x => x.WithTags("Contact"));
  }

  public override async Task HandleAsync(
      CreateContactCommand req,
      CancellationToken ct)
  {
    var result = await _mediator.Send(req, ct);

    if (!result)
    {
      AddError("Failed to send message.");

      await Send.ErrorsAsync();

      return;
    }

    await Send.OkAsync(
        new
        {
          message = "Message sent successfully"
        });
  }
}
