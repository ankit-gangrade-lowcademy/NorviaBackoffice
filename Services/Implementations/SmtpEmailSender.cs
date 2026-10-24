using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;
using NorviaBackoffice.Models;
using NorviaBackoffice.Services.Interfaces;
namespace NorviaBackoffice.Services.Implementations;
public sealed class SmtpEmailSender(IOptions<EmailSettings> options, ILogger<SmtpEmailSender> logger) : IEmailSender
{
    public async Task SendPasswordResetAsync(string recipient, string resetUrl, CancellationToken ct = default)
    {
        var s = options.Value;
        if (string.IsNullOrWhiteSpace(s.Username) || string.IsNullOrWhiteSpace(s.AppPassword) || string.IsNullOrWhiteSpace(s.FromEmail)) throw new InvalidOperationException("Gmail SMTP settings are not configured.");
        using var message = new MailMessage { From = new MailAddress(s.FromEmail, s.FromName), Subject = "Reset your Norvia Backoffice password", Body = $"A password reset was requested for your account. This single-use link expires in 30 minutes:\n\n{resetUrl}\n\nIf you did not request this, you can ignore this email.", IsBodyHtml = false };
        message.To.Add(recipient);
        using var client = new SmtpClient(s.SmtpHost, s.SmtpPort) { EnableSsl = true, Credentials = new NetworkCredential(s.Username, s.AppPassword) };
        try { await client.SendMailAsync(message, ct); } catch (Exception ex) { logger.LogError(ex, "Unable to send password reset email."); throw; }
    }
}
