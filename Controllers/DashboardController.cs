using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace NorviaBackoffice.Controllers;
[Authorize]
public sealed class DashboardController : Controller { public IActionResult Index() => View(); }
