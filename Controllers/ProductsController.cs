using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NorviaBackoffice.Data;
using NorviaBackoffice.Models.Entities;
using NorviaBackoffice.Models.ViewModels;
namespace NorviaBackoffice.Controllers;
[Authorize(Policy="SuperAdmin")]
public sealed class ProductsController(NorviaDbContext db):Controller
{
    public async Task<IActionResult> Index(CancellationToken ct)=>View(await db.NorviaProducts.AsNoTracking().OrderBy(x=>x.Name).ToListAsync(ct));
    [HttpGet] public IActionResult Create()=>View("Form",new ProductFormViewModel());
    [HttpPost,ValidateAntiForgeryToken] public async Task<IActionResult> Create(ProductFormViewModel model,CancellationToken ct){await ValidateName(model,ct);if(!ModelState.IsValid)return View("Form",model);db.NorviaProducts.Add(new NorviaProduct{Name=model.Name.Trim(),Description=model.Description?.Trim()});await db.SaveChangesAsync(ct);TempData["StatusMessage"]="Product added.";return RedirectToAction(nameof(Index));}
    [HttpGet] public async Task<IActionResult> Edit(int id,CancellationToken ct){var x=await db.NorviaProducts.FindAsync([id],ct);return x is null?NotFound():View("Form",new ProductFormViewModel{Id=x.Id,Name=x.Name,Description=x.Description});}
    [HttpPost,ValidateAntiForgeryToken] public async Task<IActionResult> Edit(ProductFormViewModel model,CancellationToken ct){await ValidateName(model,ct);if(!ModelState.IsValid)return View("Form",model);var x=await db.NorviaProducts.FindAsync([model.Id],ct);if(x is null)return NotFound();x.Name=model.Name.Trim();x.Description=model.Description?.Trim();await db.SaveChangesAsync(ct);TempData["StatusMessage"]="Product updated.";return RedirectToAction(nameof(Index));}
    private async Task ValidateName(ProductFormViewModel model,CancellationToken ct){var name=model.Name.Trim();if(await db.NorviaProducts.AnyAsync(x=>x.Id!=model.Id&&x.Name==name,ct))ModelState.AddModelError(nameof(model.Name),"A product with this name already exists.");}
}
