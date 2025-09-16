using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Clipo.Application.Services.EmailSender
{

    public sealed class EmailSenderService : IEmailSenderService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailSenderService> _logger;

        public EmailSenderService(IConfiguration configuration, ILogger<EmailSenderService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SendEmailAsync(string recipient, string subject, string body, CancellationToken ct = default)
        {
            try
            {
                string? smtpServer = _configuration["EmailSettings:SmtpServer"];
                int port = int.Parse(_configuration["EmailSettings:Port"]);
                string? user = _configuration["EmailSettings:User"];
                string? password = _configuration["EmailSettings:Password"];

                using SmtpClient smtp = new SmtpClient(smtpServer, port)
                {
                    Credentials = new NetworkCredential(user, password),
                    EnableSsl = true
                };

                MailMessage mail = new MailMessage(user, recipient, subject, body);

                await smtp.SendMailAsync(mail, ct);

                _logger.LogInformation("Email successfully sent to {Recipient}", recipient);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error while sending email to {Recipient}", recipient);
                throw;
            }
        }
    }
}
