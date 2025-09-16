namespace Clipo.Application.Services.EmailSender
{
    public interface IEmailSenderService
    {
        Task SendEmailAsync(string recipient, string subject, string body, CancellationToken ct = default);
    }
}
