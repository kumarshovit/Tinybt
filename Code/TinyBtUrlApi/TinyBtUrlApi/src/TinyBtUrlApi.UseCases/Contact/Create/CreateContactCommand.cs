using System;
using System.Collections.Generic;
using System.Text;
using Mediator;

namespace TinyBtUrlApi.UseCases.Contact.Create;

public class CreateContactCommand : IRequest<bool>
{
  public string Name { get; set; } = default!;

  public string Email { get; set; } = default!;

  public string Subject { get; set; } = default!;

  public string Message { get; set; } = default!;
}
