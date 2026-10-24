namespace NorviaBackoffice.Services.Interfaces;
public interface IEmailSender { Task SendPasswordResetAsync(string recipient, string resetUrl, CancellationToken cancellationToken = default); }
