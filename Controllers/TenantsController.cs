using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NorviaBackoffice.Data;
using NorviaBackoffice.Models.Entities;
using NorviaBackoffice.Models.ViewModels;
namespace NorviaBackoffice.Controllers;
[Authorize(Policy="Administrator")]
public sealed class TenantsController(NorviaDbContext db) : Controller
{
    public async Task<IActionResult> Index(string? search, CancellationToken ct) { var query=db.Tenants.AsNoTracking(); if(!string.IsNullOrWhiteSpace(search)) query=query.Where(x=>x.Name.Contains(search)); ViewBag.Search=search; ViewBag.ActiveCount=await db.Tenants.CountAsync(x=>x.IsActive,ct); ViewBag.AdminCount=await db.UserExtended.CountAsync(x=>!x.IsStudent,ct); ViewBag.BackofficeCount=await db.NorviaUsers.CountAsync(x=>x.IsActive,ct); return View(await query.OrderBy(x=>x.Name).ToListAsync(ct)); }
    [HttpGet] public IActionResult Create()=>View("Form",new TenantFormViewModel());
    [HttpPost,ValidateAntiForgeryToken] public async Task<IActionResult> Create(TenantFormViewModel model,CancellationToken ct){await ValidateName(model,ct);if(!ModelState.IsValid)return View("Form",model);db.Tenants.Add(new Tenant{Name=model.Name.Trim(),IsActive=model.IsActive});await db.SaveChangesAsync(ct);TempData["StatusMessage"]="Tenant created successfully.";return RedirectToAction(nameof(Index));}
    [HttpGet] public async Task<IActionResult> Edit(long id,CancellationToken ct){var x=await db.Tenants.FindAsync([id],ct);return x is null?NotFound():View("Form",new TenantFormViewModel{Id=x.Id,Name=x.Name,IsActive=x.IsActive});}
    [HttpPost,ValidateAntiForgeryToken] public async Task<IActionResult> Edit(TenantFormViewModel model,CancellationToken ct){await ValidateName(model,ct);if(!ModelState.IsValid)return View("Form",model);var x=await db.Tenants.FindAsync([model.Id],ct);if(x is null)return NotFound();x.Name=model.Name.Trim();x.IsActive=model.IsActive;await db.SaveChangesAsync(ct);TempData["StatusMessage"]="Tenant updated successfully.";return RedirectToAction(nameof(Index));}
    private async Task ValidateName(TenantFormViewModel model,CancellationToken ct){var name=model.Name.Trim();if(await db.Tenants.AnyAsync(x=>x.Id!=model.Id&&x.Name==name,ct))ModelState.AddModelError(nameof(model.Name),"A tenant with this name already exists.");}
}
