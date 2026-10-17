namespace Application.Abstractions.Services
{
    public interface IEmailService
    {
        Task SendAsync(string receiver, string subject, string htmlBody, CancellationToken ct = default);
    }
}
