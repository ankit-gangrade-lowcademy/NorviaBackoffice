using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using NorviaBackoffice.Models;
using NorviaBackoffice.Models.Entities;
using NorviaBackoffice.Services;
using NorviaBackoffice.Services.Implementations;

namespace NorviaBackoffice.Tests;

public sealed class SecurityTests
{
    [Theory]
    [InlineData("short")]
    [InlineData("password1234")]
    [InlineData("123456789012")]
    public void PasswordPolicy_RejectsWeakPasswords(string password) => Assert.NotNull(PasswordPolicy.Validate(password));

    [Fact]
    public void PasswordHasher_DoesNotStoreOrComparePlaintext()
    {
        var user = new NorviaUser();
        var hasher = new PasswordHasher<NorviaUser>();
        var hash = hasher.HashPassword(user, "Correct-Horse-Battery-Staple");
        Assert.NotEqual("Correct-Horse-Battery-Staple", hash);
        Assert.NotEqual(PasswordVerificationResult.Failed, hasher.VerifyHashedPassword(user, hash, "Correct-Horse-Battery-Staple"));
        Assert.Equal(PasswordVerificationResult.Failed, hasher.VerifyHashedPassword(user, hash, "wrong-password"));
    }

    [Fact]
    public void LoginThrottle_LocksAfterConfiguredFailures()
    {
        var throttle = new MemoryLoginThrottle(Options.Create(new LockoutSettings { MaxAttempts = 2, DurationMinutes = 15 }));
        throttle.RecordFailure("account:ip"); Assert.False(throttle.IsLocked("account:ip"));
        throttle.RecordFailure("account:ip"); Assert.True(throttle.IsLocked("account:ip"));
        throttle.Reset("account:ip"); Assert.False(throttle.IsLocked("account:ip"));
    }
}
