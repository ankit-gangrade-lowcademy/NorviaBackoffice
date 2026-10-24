namespace NorviaBackoffice.Services.Interfaces;
public interface IPasswordResetTokenStore
{
    Task<string> CreateAsync(long userId, TimeSpan lifetime, CancellationToken cancellationToken = default);
    Task<long?> ConsumeAsync(string token, CancellationToken cancellationToken = default);
}
