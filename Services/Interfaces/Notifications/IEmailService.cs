namespace OpsFlow.Services.Interfaces.Notifications;

public interface IEmailService
{
    Task SendEmailAsync(
        string to,
        string subject,
        string body);
}