using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;
using OpsFlow.Configuration;
using OpsFlow.Services.Interfaces.Notifications;

namespace OpsFlow.Services.Notifications;

public class EmailService : IEmailService
{
    private readonly SmtpOptions _smtpOptions;

    public EmailService(IOptions<SmtpOptions> smtpOptions)
    {
        _smtpOptions = smtpOptions.Value;
    }

    public async Task SendEmailAsync(
        string to,
        string subject,
        string body)
    {
        using var message = new MailMessage();

        message.From = new MailAddress(
            _smtpOptions.FromEmail,
            _smtpOptions.FromName);

        message.To.Add(to);

        message.Subject = subject;

        message.Body = body;

        message.IsBodyHtml = true;


        using var smtpClient = new SmtpClient(
            _smtpOptions.Host,
            _smtpOptions.Port);

        smtpClient.EnableSsl = true;

        smtpClient.Credentials = new NetworkCredential(
            _smtpOptions.Username,
            _smtpOptions.Password);

        await smtpClient.SendMailAsync(message);
    }
}