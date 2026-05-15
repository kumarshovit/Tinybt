using System;
using System.Collections.Generic;
using System.Text;
using MailKit.Net.Smtp;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using TinyBtUrlApi.Core.Interfaces;
using TinyBtUrlApi.Infrastructure.Data;

namespace TinyBtUrlApi.Infrastructure.Services;

public class ContactEmailService : IContactEmailService
{
  private readonly IConfiguration _configuration;
  private readonly AppDbContext _context;

  public ContactEmailService(
      IConfiguration configuration,
      AppDbContext context
  )
  {
    _configuration = configuration;
    _context = context;
  }

  public async Task SendContactEmailToAdminsAsync(
      string name,
      string email,
      string subject,
      string message
  )
  {
    var adminEmails = await _context.Users
        .Where(x => x.Role == "Admin")
        .Select(x => x.Email)
        .ToListAsync();

    if (!adminEmails.Any())
      return;

    var mail = new MimeMessage();

    mail.From.Add(
        new MailboxAddress(
            "LINKBT Contact",
            _configuration["Smtp:From"]
        )
    );

    foreach (var adminEmail in adminEmails)
    {
      mail.To.Add(
          MailboxAddress.Parse(adminEmail)
      );
    }

    mail.Subject = $"Contact Message: {subject}";

    mail.Body = new TextPart("html")
    {
      Text = $@"
                <h2>New Contact Message</h2>

                <p>
                    <strong>Name:</strong> {name}
                </p>

                <p>
                    <strong>Email:</strong> {email}
                </p>

                <p>
                    <strong>Subject:</strong> {subject}
                </p>

                <p>
                    <strong>Message:</strong>
                </p>

                <p>{message}</p>
            "
    };

    using var smtp = new MailKit.Net.Smtp.SmtpClient();
    await smtp.ConnectAsync(
        _configuration["Smtp:Host"],
        int.Parse(_configuration["Smtp:Port"]!),
        false
    );

    await smtp.AuthenticateAsync(
        _configuration["Smtp:Username"],
        _configuration["Smtp:Password"]
    );

    await smtp.SendAsync(mail);

    await smtp.DisconnectAsync(true);
  }
}
