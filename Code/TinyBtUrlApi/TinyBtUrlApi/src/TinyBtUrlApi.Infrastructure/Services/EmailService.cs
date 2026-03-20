using System.Net;
using System.Net.Mail;
using TinyBtUrlApi.Core.Interfaces;

namespace TinyBtUrlApi.Infrastructure.Services;

public class EmailService : IEmailSender
{
  private readonly IConfiguration _configuration;

  public EmailService(IConfiguration configuration)
  {
    _configuration = configuration;
  }

  public async Task SendEmailAsync(
      string to,
      string from,
      string subject,
      string body)
  {
    var host = _configuration["Smtp:Host"];
    var port = int.Parse(_configuration["Smtp:Port"]!);
    var username = _configuration["Smtp:Username"];
    var password = _configuration["Smtp:Password"];

    using var client = new System.Net.Mail.SmtpClient(host);
    client.Port = port;
    client.Credentials = new NetworkCredential(username, password);
    client.EnableSsl = true;

    var mailMessage = new MailMessage(from, to, subject, body);
    mailMessage.IsBodyHtml = true;
    await client.SendMailAsync(mailMessage);
  }
}
