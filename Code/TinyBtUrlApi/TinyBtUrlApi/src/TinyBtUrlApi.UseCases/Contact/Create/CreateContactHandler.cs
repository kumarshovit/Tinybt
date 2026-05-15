using System;
using System.Collections.Generic;
using System.Text;
using Mediator;
using TinyBtUrlApi.Core.Entities;
using TinyBtUrlApi.Core.Interfaces;

namespace TinyBtUrlApi.UseCases.Contact.Create;

public class CreateContactHandler
    : IRequestHandler<CreateContactCommand, bool>
{
  private readonly IContactRepository _contactRepository;
  private readonly IContactEmailService _contactEmailService;
  public CreateContactHandler(
      IContactRepository contactRepository,
        IContactEmailService contactEmailService

  )
  {
    _contactRepository = contactRepository;
    _contactEmailService = contactEmailService;

  }

  public async ValueTask<bool> Handle(
      CreateContactCommand command,
      CancellationToken cancellationToken
  )
  {
    var contactMessage = new ContactMessage
    {
      Id = Guid.NewGuid(),

      Name = command.Name,

      Email = command.Email,

      Subject = command.Subject,

      Message = command.Message,

      CreatedAt = DateTime.UtcNow,

      IsRead = false
    };

    await _contactRepository.AddAsync(contactMessage);

    await _contactEmailService.SendContactEmailToAdminsAsync(
        command.Name,
        command.Email,
        command.Subject,
        command.Message
    );

    return true;
  }
}
