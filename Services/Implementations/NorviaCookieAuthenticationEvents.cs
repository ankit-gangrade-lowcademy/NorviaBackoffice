using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using NorviaBackoffice.Services.Interfaces;
namespace NorviaBackoffice.Services.Implementations;
public sealed class NorviaCookieAuthenticationEvents(IUserService users) : CookieAuthenticationEvents
{
    public override async Task ValidatePrincipal(CookieValidatePrincipalContext context)
    {
        if (!long.TryParse(context.Principal?.FindFirstValue(ClaimTypes.NameIdentifier), out var id)) { context.RejectPrincipal(); return; }
        var user = await users.FindByIdAsync(id, cancellationToken: context.HttpContext.RequestAborted);
        var stamp = context.Principal!.FindFirstValue("security_stamp");
        if (user is null || !user.IsActive || !user.Role.IsActive || stamp != (user.UpdatedOn?.Ticks ?? 0).ToString()) { context.RejectPrincipal(); await context.HttpContext.SignOutAsync(); }
    }
}
