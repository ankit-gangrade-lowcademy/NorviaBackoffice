using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using NorviaBackoffice.Models;
using NorviaBackoffice.Services.Interfaces;
namespace NorviaBackoffice.Services.Implementations;
public sealed class ProtectedFilePasswordResetTokenStore : IPasswordResetTokenStore
{
    private sealed record TokenRecord(long UserId, string Hash, DateTimeOffset ExpiresOn, bool Consumed);
    private readonly IDataProtector _protector;
    private readonly string _path;
    private readonly SemaphoreSlim _gate = new(1, 1);
    public ProtectedFilePasswordResetTokenStore(IDataProtectionProvider provider, IOptions<ApplicationSettings> options, IWebHostEnvironment env) { _protector = provider.CreateProtector("Norvia.PasswordResetTokens.v1"); _path = Path.IsPathRooted(options.Value.PasswordResetStorePath) ? options.Value.PasswordResetStorePath : Path.Combine(env.ContentRootPath, options.Value.PasswordResetStorePath); }
    public async Task<string> CreateAsync(long userId, TimeSpan lifetime, CancellationToken ct = default)
    {
        var token = WebEncoders.Base64UrlEncode(RandomNumberGenerator.GetBytes(32));
        await _gate.WaitAsync(ct); try { var records = await ReadAsync(ct); records.RemoveAll(x => x.ExpiresOn <= DateTimeOffset.UtcNow || (x.UserId == userId && !x.Consumed)); records.Add(new(userId, Hash(token), DateTimeOffset.UtcNow.Add(lifetime), false)); await WriteAsync(records, ct); } finally { _gate.Release(); }
        return token;
    }
    public async Task<long?> ConsumeAsync(string token, CancellationToken ct = default)
    {
        await _gate.WaitAsync(ct); try { var records = await ReadAsync(ct); var hash = Hash(token); var item = records.FirstOrDefault(x => !x.Consumed && x.ExpiresOn > DateTimeOffset.UtcNow && CryptographicOperations.FixedTimeEquals(Convert.FromHexString(x.Hash), Convert.FromHexString(hash))); if (item is null) return null; records[records.IndexOf(item)] = item with { Consumed = true }; await WriteAsync(records, ct); return item.UserId; } finally { _gate.Release(); }
    }
    private static string Hash(string token) => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(token)));
    private async Task<List<TokenRecord>> ReadAsync(CancellationToken ct) { if (!File.Exists(_path)) return []; var encrypted = await File.ReadAllBytesAsync(_path, ct); try { return JsonSerializer.Deserialize<List<TokenRecord>>(_protector.Unprotect(encrypted)) ?? []; } catch (CryptographicException) { return []; } }
    private async Task WriteAsync(List<TokenRecord> records, CancellationToken ct) { Directory.CreateDirectory(Path.GetDirectoryName(_path)!); var temp = _path + ".tmp"; await File.WriteAllBytesAsync(temp, _protector.Protect(JsonSerializer.SerializeToUtf8Bytes(records)), ct); File.Move(temp, _path, true); }
}
