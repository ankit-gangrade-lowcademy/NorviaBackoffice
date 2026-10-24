using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using NorviaBackoffice.Models;
using NorviaBackoffice.Models.Entities;
using NorviaBackoffice.Models.ViewModels;
using NorviaBackoffice.Services.Interfaces;
using NorviaBackoffice.Services;
namespace NorviaBackoffice.Controllers;
public sealed class AccountController(IUserService users, IPasswordHasher<NorviaUser> hasher, ILoginThrottle throttle, IPasswordResetTokenStore tokens, IEmailSender emailSender, IOptions<ApplicationSettings> appSettings, ILogger<AccountController> logger) : Controller
{
    private const string GenericLoginError = "The email or password is incorrect.";
    [AllowAnonymous, HttpGet] public IActionResult Login(string? returnUrl = null) { if (User.Identity?.IsAuthenticated == true) return RedirectToAction("Index", "Dashboard"); return View(new LoginViewModel { ReturnUrl = returnUrl }); }
    [AllowAnonymous, HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, CancellationToken ct)
    {
        if (!ModelState.IsValid) return View(model);
        var key = $"{model.Email.Trim().ToUpperInvariant()}:{HttpContext.Connection.RemoteIpAddress}";
        if (throttle.IsLocked(key)) { ModelState.AddModelError("", GenericLoginError); return View(model); }
        var user = await users.FindActiveByEmailAsync(model.Email.Trim(), ct);
        if (user is null || hasher.VerifyHashedPassword(user, user.PasswordHash, model.Password) == PasswordVerificationResult.Failed) { throttle.RecordFailure(key); if (user is not null) await users.RecordFailedLoginAsync(user.Id, ct); ModelState.AddModelError("", GenericLoginError); return View(model); }
        throttle.Reset(key); await users.RecordSuccessfulLoginAsync(user.Id, ct); await SignInAsync(user, model.RememberMe);
        if (user.MustChangePassword) return RedirectToAction(nameof(ChangePassword));
        return Url.IsLocalUrl(model.ReturnUrl) ? LocalRedirect(model.ReturnUrl!) : RedirectToAction("Index", "Dashboard");
    }
    [AllowAnonymous, HttpGet] public IActionResult ForgotPassword() => View(new ForgotPasswordViewModel());
    [AllowAnonymous, HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model, CancellationToken ct)
    {
        if (!ModelState.IsValid) return View(model);
        var user = await users.FindActiveByEmailAsync(model.Email.Trim(), ct);
        if (user is not null) try { var token = await tokens.CreateAsync(user.Id, TimeSpan.FromMinutes(30), ct); var configuredBase = appSettings.Value.BaseUrl.TrimEnd('/'); var relative = Url.Action(nameof(ResetPassword), "Account", new { token })!; var url = string.IsNullOrWhiteSpace(configuredBase) ? Url.Action(nameof(ResetPassword), "Account", new { token }, Request.Scheme)! : configuredBase + relative; await emailSender.SendPasswordResetAsync(user.Email, url, ct); } catch (Exception ex) { logger.LogError(ex, "Password reset delivery failed."); }
        TempData["StatusMessage"] = "If an account exists with this email address, you will receive password reset instructions shortly."; return RedirectToAction(nameof(ForgotPassword));
    }
    [AllowAnonymous, HttpGet] public IActionResult ResetPassword(string token) => View(new ResetPasswordViewModel { Token = token });
    [AllowAnonymous, HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model, CancellationToken ct)
    {
        ValidateNewPassword(model.NewPassword); if (!ModelState.IsValid) return View(model);
        var userId = await tokens.ConsumeAsync(model.Token, ct); var user = userId is null ? null : await users.FindByIdAsync(userId.Value, cancellationToken: ct);
        if (user is null || !user.IsActive || hasher.VerifyHashedPassword(user, user.PasswordHash, model.NewPassword) != PasswordVerificationResult.Failed) { ModelState.AddModelError("", "This reset link is invalid or has expired."); return View(model); }
        await users.UpdatePasswordAsync(user.Id, hasher.HashPassword(user, model.NewPassword), ct); await HttpContext.SignOutAsync(); TempData["StatusMessage"] = "Your password has been reset. Please sign in."; return RedirectToAction(nameof(Login));
    }
    [Authorize, HttpGet] public IActionResult ChangePassword() => View(new ChangePasswordViewModel());
    [Authorize, HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model, CancellationToken ct)
    {
        ValidateNewPassword(model.NewPassword); if (!ModelState.IsValid) return View(model);
        var user = await users.FindByIdAsync(CurrentUserId(), cancellationToken: ct);
        if (user is null || hasher.VerifyHashedPassword(user, user.PasswordHash, model.CurrentPassword) == PasswordVerificationResult.Failed) { ModelState.AddModelError(nameof(model.CurrentPassword), "The current password is incorrect."); return View(model); }
        if (hasher.VerifyHashedPassword(user, user.PasswordHash, model.NewPassword) != PasswordVerificationResult.Failed) { ModelState.AddModelError(nameof(model.NewPassword), "The new password must differ from the current password."); return View(model); }
        await users.UpdatePasswordAsync(user.Id, hasher.HashPassword(user, model.NewPassword), ct); await HttpContext.SignOutAsync(); TempData["StatusMessage"] = "Password changed. Please sign in again."; return RedirectToAction(nameof(Login));
    }
    [Authorize, HttpPost, ValidateAntiForgeryToken] public async Task<IActionResult> Logout() { await HttpContext.SignOutAsync(); return RedirectToAction(nameof(Login)); }
    [AllowAnonymous] public IActionResult AccessDenied() => View();
    private long CurrentUserId() => long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    private void ValidateNewPassword(string value) { var error = PasswordPolicy.Validate(value); if (error is not null) ModelState.AddModelError("NewPassword", error); }
    private async Task SignInAsync(NorviaUser user, bool persistent)
    {
        Claim[] claims = [new(ClaimTypes.NameIdentifier, user.Id.ToString()), new(ClaimTypes.Name, user.Name), new(ClaimTypes.Email, user.Email), new(ClaimTypes.Role, user.Role.Name), new("role_id", user.RoleId.ToString()), new("must_change_password", user.MustChangePassword ? "true" : "false"), new("security_stamp", (user.UpdatedOn?.Ticks ?? 0).ToString())];
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme)), new AuthenticationProperties { IsPersistent = persistent, AllowRefresh = true });
    }
}
