using System.Collections.Concurrent;
using Microsoft.Extensions.Options;
using NorviaBackoffice.Models;
using NorviaBackoffice.Services.Interfaces;
namespace NorviaBackoffice.Services.Implementations;
public sealed class MemoryLoginThrottle(IOptions<LockoutSettings> settings) : ILoginThrottle
{
    private sealed record Entry(int Attempts, DateTimeOffset? LockedUntil);
    private readonly ConcurrentDictionary<string, Entry> _entries = new(StringComparer.OrdinalIgnoreCase);
    private readonly LockoutSettings _settings = settings.Value;
    public bool IsLocked(string key) { if (!_entries.TryGetValue(key, out var e) || e.LockedUntil is null) return false; if (e.LockedUntil > DateTimeOffset.UtcNow) return true; _entries.TryRemove(key, out _); return false; }
    public void RecordFailure(string key) => _entries.AddOrUpdate(key, _ => new(1, null), (_, e) => { var count = e.Attempts + 1; return count >= _settings.MaxAttempts ? new(count, DateTimeOffset.UtcNow.AddMinutes(_settings.DurationMinutes)) : new(count, null); });
    public void Reset(string key) => _entries.TryRemove(key, out _);
}
