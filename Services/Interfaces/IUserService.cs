using NorviaBackoffice.Models.Entities;
namespace NorviaBackoffice.Services.Interfaces;
public interface IUserService
{
    Task<NorviaUser?> FindActiveByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<NorviaUser?> FindByIdAsync(long id, bool tracking = false, CancellationToken cancellationToken = default);
    Task RecordFailedLoginAsync(long id, CancellationToken cancellationToken = default);
    Task RecordSuccessfulLoginAsync(long id, CancellationToken cancellationToken = default);
    Task UpdatePasswordAsync(long id, string passwordHash, CancellationToken cancellationToken = default);
    Task UpdateNameAsync(long id, string name, CancellationToken cancellationToken = default);
}
