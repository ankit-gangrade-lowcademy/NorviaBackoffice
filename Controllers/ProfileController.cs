using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NorviaBackoffice.Models.ViewModels;
using NorviaBackoffice.Services.Interfaces;
namespace NorviaBackoffice.Controllers;
[Authorize]
public sealed class ProfileController(IUserService users) : Controller
{
    [HttpGet] public async Task<IActionResult> Index(CancellationToken ct) { var user = await users.FindByIdAsync(UserId(), cancellationToken: ct); if (user is null) return Challenge(); return View(new ProfileViewModel { Name = user.Name, Email = user.Email, Role = user.Role.Name, IsActive = user.IsActive, LastLoginOn = user.LastLoginOn }); }
    [HttpPost, ValidateAntiForgeryToken] public async Task<IActionResult> Index(ProfileViewModel model, CancellationToken ct)
    {
        var user = await users.FindByIdAsync(UserId(), cancellationToken: ct); if (user is null) return Challenge(); model.Email = user.Email; model.Role = user.Role.Name; model.IsActive = user.IsActive; model.LastLoginOn = user.LastLoginOn; if (!ModelState.IsValid) return View(model);
        await users.UpdateNameAsync(user.Id, model.Name.Trim(), ct); var updated = await users.FindByIdAsync(user.Id, cancellationToken: ct); var claims = User.Claims.Where(x => x.Type != ClaimTypes.Name && x.Type != "security_stamp").Append(new Claim(ClaimTypes.Name, model.Name.Trim())).Append(new Claim("security_stamp", (updated?.UpdatedOn?.Ticks ?? 0).ToString())); await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme))); TempData["StatusMessage"] = "Profile updated successfully."; return RedirectToAction(nameof(Index));
    }
    private long UserId() => long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
}
