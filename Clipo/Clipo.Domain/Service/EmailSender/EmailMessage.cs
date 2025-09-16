namespace Clipo.Domain.Service.EmailSender
{
    public sealed record EmailAddress(string Address, string? Name = null);

    public sealed class EmailMessage
    {
        public required EmailAddress To { get; init; }
        public required string Subject { get; init; }
        public string? HtmlBody { get; init; }
        public string? TextBody { get; init; }
    }
}
