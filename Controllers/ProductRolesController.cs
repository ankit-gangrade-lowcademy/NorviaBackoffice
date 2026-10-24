using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NorviaBackoffice.Data;
using NorviaBackoffice.Models.Entities;
using NorviaBackoffice.Models.ViewModels;
namespace NorviaBackoffice.Controllers;
[Authorize(Policy="SuperAdmin")]
public sealed class ProductRolesController(NorviaDbContext db):Controller
{
    public async Task<IActionResult> Index(CancellationToken ct)=>View(await db.ProductUserRoles.AsNoTracking().Include(x=>x.Product).OrderBy(x=>x.Product.Name).ThenBy(x=>x.Order).ToListAsync(ct));
    [HttpGet] public async Task<IActionResult> Create(CancellationToken ct)=>View("Form",await Prepare(new ProductRoleFormViewModel{Order=await db.ProductUserRoles.CountAsync(ct)+1},ct));
    [HttpPost,ValidateAntiForgeryToken] public async Task<IActionResult> Create(ProductRoleFormViewModel model,CancellationToken ct){await Validate(model,ct);if(!ModelState.IsValid)return View("Form",await Prepare(model,ct));db.ProductUserRoles.Add(new ProductUserRole{ProductId=model.ProductId,ProductRoleId=model.ProductRoleId,Name=model.Name.Trim(),Order=model.Order});await db.SaveChangesAsync(ct);TempData["StatusMessage"]="Product role added.";return RedirectToAction(nameof(Index));}
    [HttpGet] public async Task<IActionResult> Edit(int id,CancellationToken ct){var x=await db.ProductUserRoles.FindAsync([id],ct);return x is null?NotFound():View("Form",await Prepare(new ProductRoleFormViewModel{Id=x.Id,ProductId=x.ProductId,ProductRoleId=x.ProductRoleId,Name=x.Name,Order=x.Order},ct));}
    [HttpPost,ValidateAntiForgeryToken] public async Task<IActionResult> Edit(ProductRoleFormViewModel model,CancellationToken ct){await Validate(model,ct);if(!ModelState.IsValid)return View("Form",await Prepare(model,ct));var x=await db.ProductUserRoles.FindAsync([model.Id],ct);if(x is null)return NotFound();x.ProductId=model.ProductId;x.ProductRoleId=model.ProductRoleId;x.Name=model.Name.Trim();x.Order=model.Order;await db.SaveChangesAsync(ct);TempData["StatusMessage"]="Product role updated.";return RedirectToAction(nameof(Index));}
    private async Task<ProductRoleFormViewModel> Prepare(ProductRoleFormViewModel m,CancellationToken ct){m.Products=await db.NorviaProducts.AsNoTracking().OrderBy(x=>x.Name).Select(x=>new SelectListItem(x.Name,x.Id.ToString())).ToListAsync(ct);return m;}
    private async Task Validate(ProductRoleFormViewModel m,CancellationToken ct){if(!await db.NorviaProducts.AnyAsync(x=>x.Id==m.ProductId,ct))ModelState.AddModelError(nameof(m.ProductId),"Select a valid product.");if(await db.ProductUserRoles.AnyAsync(x=>x.Id!=m.Id&&x.ProductId==m.ProductId&&x.ProductRoleId==m.ProductRoleId,ct))ModelState.AddModelError(nameof(m.ProductRoleId),"This role ID is already mapped for the selected product.");}
}
