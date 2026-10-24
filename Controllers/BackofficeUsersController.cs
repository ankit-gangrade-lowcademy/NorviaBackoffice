using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NorviaBackoffice.Data;
using NorviaBackoffice.Models.Entities;
using NorviaBackoffice.Models.ViewModels;
using NorviaBackoffice.Services;
namespace NorviaBackoffice.Controllers;
[Authorize(Policy="SuperAdmin")]
public sealed class BackofficeUsersController(NorviaDbContext db,IPasswordHasher<NorviaUser> hasher):Controller
{
    public async Task<IActionResult> Index(CancellationToken ct)=>View(await db.NorviaUsers.AsNoTracking().Include(x=>x.Role).OrderBy(x=>x.Name).ToListAsync(ct));
    [HttpGet] public async Task<IActionResult> Create(CancellationToken ct)=>View("Form",await Prepare(new BackofficeUserFormViewModel(),ct));
    [HttpPost,ValidateAntiForgeryToken] public async Task<IActionResult> Create(BackofficeUserFormViewModel model,CancellationToken ct){if(string.IsNullOrEmpty(model.Password))ModelState.AddModelError(nameof(model.Password),"Password is required.");await Validate(model,ct);if(!ModelState.IsValid)return View("Form",await Prepare(model,ct));var user=new NorviaUser{Name=model.Name.Trim(),Email=model.Email.Trim(),RoleId=model.RoleId,IsActive=model.IsActive,MustChangePassword=model.MustChangePassword,CreatedOn=DateTime.UtcNow};user.PasswordHash=hasher.HashPassword(user,model.Password!);db.NorviaUsers.Add(user);await db.SaveChangesAsync(ct);TempData["StatusMessage"]="Backoffice user created.";return RedirectToAction(nameof(Index));}
    [HttpGet] public async Task<IActionResult> Edit(long id,CancellationToken ct){var x=await db.NorviaUsers.FindAsync([id],ct);return x is null?NotFound():View("Form",await Prepare(new BackofficeUserFormViewModel{Id=x.Id,Name=x.Name,Email=x.Email,RoleId=x.RoleId,IsActive=x.IsActive,MustChangePassword=x.MustChangePassword},ct));}
    [HttpPost,ValidateAntiForgeryToken] public async Task<IActionResult> Edit(BackofficeUserFormViewModel model,CancellationToken ct){await Validate(model,ct);if(!ModelState.IsValid)return View("Form",await Prepare(model,ct));var x=await db.NorviaUsers.FindAsync([model.Id],ct);if(x is null)return NotFound();x.Name=model.Name.Trim();x.Email=model.Email.Trim();x.RoleId=model.RoleId;x.IsActive=model.IsActive;x.MustChangePassword=model.MustChangePassword;x.UpdatedOn=DateTime.UtcNow;if(!string.IsNullOrWhiteSpace(model.Password)){var error=PasswordPolicy.Validate(model.Password);if(error is not null){ModelState.AddModelError(nameof(model.Password),error);return View("Form",await Prepare(model,ct));}x.PasswordHash=hasher.HashPassword(x,model.Password);}await db.SaveChangesAsync(ct);TempData["StatusMessage"]="Backoffice user updated.";return RedirectToAction(nameof(Index));}
    private async Task Validate(BackofficeUserFormViewModel m,CancellationToken ct){var email=m.Email.Trim();if(await db.NorviaUsers.AnyAsync(x=>x.Id!=m.Id&&x.Email==email,ct))ModelState.AddModelError(nameof(m.Email),"This email is already in use.");if(!await db.NorviaRoles.AnyAsync(x=>x.Id==m.RoleId&&x.IsActive,ct))ModelState.AddModelError(nameof(m.RoleId),"Select a valid active role.");if(!string.IsNullOrWhiteSpace(m.Password)){var error=PasswordPolicy.Validate(m.Password);if(error is not null)ModelState.AddModelError(nameof(m.Password),error);}}
    private async Task<BackofficeUserFormViewModel> Prepare(BackofficeUserFormViewModel m,CancellationToken ct){m.Roles=await db.NorviaRoles.AsNoTracking().Where(x=>x.IsActive).OrderBy(x=>x.Id).Select(x=>new SelectListItem(x.Name,x.Id.ToString())).ToListAsync(ct);return m;}
}
