using System;
using System.Collections.Generic;
using System.Text;

namespace TinyBtUrlApi.Core.Interfaces;

public interface IContactEmailService
{
  Task SendContactEmailToAdminsAsync(
      string name,
      string email,
      string subject,
      string message
  );
}
