namespace NorviaBackoffice.Services.Interfaces;
public interface ILoginThrottle { bool IsLocked(string key); void RecordFailure(string key); void Reset(string key); }
