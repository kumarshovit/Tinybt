using System.Net;
using System.Net.Mail;

namespace TinyURL.Services
{
    public class EmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            try
            {
                var host = _configuration["Smtp:Host"];
                var port = int.Parse(_configuration["Smtp:Port"]);
                var username = _configuration["Smtp:Username"];
                var password = _configuration["Smtp:Password"];
                var from = _configuration["Smtp:From"];

                using (var client = new SmtpClient(host))
                {
                    client.Port = port;
                    client.Credentials = new NetworkCredential(username, password);
                    client.EnableSsl = true;

                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress(from),
                        Subject = subject,
                        Body = body,
                        IsBodyHtml = false
                    };

                    mailMessage.To.Add(toEmail);

                    await client.SendMailAsync(mailMessage);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Email sending failed: " + ex.Message);
            }
        }
    }
}
