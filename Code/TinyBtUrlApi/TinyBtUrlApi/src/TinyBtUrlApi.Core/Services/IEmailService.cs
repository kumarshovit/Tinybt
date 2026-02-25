using System;
using System.Collections.Generic;
using System.Text;

namespace TinyBtUrlApi.Core.Services;

public interface IEmailService
{
  Task SendEmailAsync(string toEmail, string subject, string body);
}
