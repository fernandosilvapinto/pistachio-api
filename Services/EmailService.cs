using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace Pistachio.Api.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _config;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration config, ILogger<EmailService> logger)
    {
        _config = config;
        _logger = logger;
    }

    public async Task SendEmailAsync(string to, string subject, string htmlBody)
    {
        var host = _config["Smtp:Host"] ?? "localhost";
        var port = int.Parse(_config["Smtp:Port"] ?? "1025");
        var from = _config["Smtp:From"] ?? "no-reply@pistachio.local";

        var message = new MimeMessage();
        message.From.Add(MailboxAddress.Parse(from));
        message.To.Add(MailboxAddress.Parse(to));
        message.Subject = subject;
        message.Body = new TextPart("html") { Text = htmlBody };

        using var client = new SmtpClient();

        // O Mailpit não exige TLS nem autenticação em dev — ligação simples.
        await client.ConnectAsync(host, port, SecureSocketOptions.None);
        await client.SendAsync(message);
        await client.DisconnectAsync(true);

        _logger.LogInformation("Email enviado para {To} — assunto: {Subject}", to, subject);
    }
}
