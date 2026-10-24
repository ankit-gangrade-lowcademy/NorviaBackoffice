using Microsoft.EntityFrameworkCore;
using NorviaBackoffice.Data;
using NorviaBackoffice.Models.Entities;
using NorviaBackoffice.Services.Interfaces;
namespace NorviaBackoffice.Services.Implementations;
public sealed class UserService(NorviaDbContext db) : IUserService
{
    public Task<NorviaUser?> FindActiveByEmailAsync(string email, CancellationToken ct = default) => db.NorviaUsers.AsNoTracking().Include(x => x.Role).SingleOrDefaultAsync(x => x.Email == email && x.IsActive && x.Role.IsActive, ct);
    public Task<NorviaUser?> FindByIdAsync(long id, bool tracking = false, CancellationToken ct = default) { var query = db.NorviaUsers.Include(x => x.Role).AsQueryable(); return (tracking ? query : query.AsNoTracking()).SingleOrDefaultAsync(x => x.Id == id, ct); }
    public async Task RecordFailedLoginAsync(long id, CancellationToken ct = default) { await db.NorviaUsers.Where(x => x.Id == id).ExecuteUpdateAsync(s => s.SetProperty(x => x.FailedLoginAttempts, x => x.FailedLoginAttempts + 1), ct); }
    public async Task RecordSuccessfulLoginAsync(long id, CancellationToken ct = default) { await db.NorviaUsers.Where(x => x.Id == id).ExecuteUpdateAsync(s => s.SetProperty(x => x.FailedLoginAttempts, 0).SetProperty(x => x.LastLoginOn, DateTime.UtcNow), ct); }
    public async Task UpdatePasswordAsync(long id, string hash, CancellationToken ct = default) { await db.NorviaUsers.Where(x => x.Id == id).ExecuteUpdateAsync(s => s.SetProperty(x => x.PasswordHash, hash).SetProperty(x => x.MustChangePassword, false).SetProperty(x => x.UpdatedOn, DateTime.UtcNow), ct); }
    public async Task UpdateNameAsync(long id, string name, CancellationToken ct = default) { await db.NorviaUsers.Where(x => x.Id == id).ExecuteUpdateAsync(s => s.SetProperty(x => x.Name, name).SetProperty(x => x.UpdatedOn, DateTime.UtcNow), ct); }
}
