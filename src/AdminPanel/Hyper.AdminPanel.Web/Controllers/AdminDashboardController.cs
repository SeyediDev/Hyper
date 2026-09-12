using Hyper.AdminPanel.Web.ViewModels;
using Hyper.Domain.Features.Integrations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Neo.Bpms.UI.MVC.Controllers.Public;

namespace Hyper.AdminPanel.Web.Controllers;

[Authorize]
[ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
public sealed class AdminDashboardController(IAdminOverviewQuery overview, IAdminMerchantSimulationService simulations,
    AdminSimulationTickets tickets, IWebHostEnvironment environment) : ControllerBaseMVC
{
    public async Task<IActionResult> Index(int days = 7, CancellationToken ct = default)
    {
        if (days is not (7 or 30)) return BadRequest();
        var admin = GetUser();
        if (!admin.IsAdmin) return StatusCode(403);
        var selected = tickets.Read(admin.Id, Request.Cookies["Hyper.AdminMerchantSimulation"]) is { } id
            ? await simulations.GetAsync(admin.Id, id, ct) : null;
        ViewBag.AdminDisplayName = admin.UserName;
        var stats = await overview.GetAsync(days, selected?.ShopId, selected?.TenantId, ct);
        return View(new AdminOverviewViewModel(stats, selected?.ShopName ?? "همهٔ مغازه‌ها", false, selected is not null));
    }

    [AllowAnonymous]
    public IActionResult Preview(int days = 7)
    {
        if (!environment.IsDevelopment()) return NotFound();
        if (days is not (7 or 30)) return BadRequest();
        ViewBag.DashboardPreview = true;
        return View("Index", AdminOverviewViewModel.Preview(days));
    }
}
