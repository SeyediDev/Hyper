using Hyper.AdminPanel.Web.ViewModels;
using Hyper.Domain.Entities.Integrations;
using Hyper.Domain.Features.Integrations;
using Microsoft.AspNetCore.Mvc;
using Neo.Bpms.UI.MVC.Controllers.Public;

namespace Hyper.AdminPanel.Web.Controllers;

[ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
public sealed class MerchantSimulationController(IAdminMerchantSimulationService simulations,
    AdminSimulationTickets tickets, IIntegrationDashboardQuery dashboard) : ControllerBaseMVC
{
    private const string CookieName = "Hyper.AdminMerchantSimulation";

    [HttpGet]
    public async Task<IActionResult> Dashboard(CancellationToken ct)
    {
        var admin = GetUser();
        if (!admin.IsAdmin) return StatusCode(403);
        var selected = ReadTicket(admin.Id, Request.Cookies[CookieName]) is { } id
            ? await simulations.GetAsync(admin.Id, id, ct) : null;
        var stats = selected is null ? null : await dashboard.GetAsync(selected.ShopId, selected.TenantId, ct);
        return View(new IntegrationDashboardViewModel(selected, stats));
    }

    [HttpGet]
    public async Task<IActionResult> Index(string? search, CancellationToken ct)
    {
        var admin = GetUser();
        if (!admin.IsAdmin) return StatusCode(403);
        if (search?.Length > 200) return BadRequest();
        var selected = ReadTicket(admin.Id, Request.Cookies[CookieName]) is { } id
            ? await simulations.GetAsync(admin.Id, id, ct) : null;
        var shops = await simulations.SearchShopsAsync(search, ct);
        return View(new MerchantSimulationViewModel(admin.UserName, search, shops, selected,
            selected is null ? null : Protect(admin.Id, selected.Id)));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Select(int shopId, CancellationToken ct)
    {
        var admin = GetUser();
        if (!admin.IsAdmin) return StatusCode(403);
        var selected = await simulations.SelectAsync(admin.Id, shopId, ct);
        if (selected is null) return NotFound();
        if (ReadTicket(admin.Id, Request.Cookies[CookieName]) is { } previous)
            await simulations.EndAsync(admin.Id, previous, ct);
        Response.Cookies.Append(CookieName, Protect(admin.Id, selected.Id), new CookieOptions
        {
            HttpOnly = true, Secure = Request.IsHttps, SameSite = SameSiteMode.Strict,
            Path = "/", MaxAge = TimeSpan.FromMinutes(30), IsEssential = true
        });
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Clear(CancellationToken ct)
    {
        var admin = GetUser();
        if (!admin.IsAdmin) return StatusCode(403);
        if (ReadTicket(admin.Id, Request.Cookies[CookieName]) is { } id)
            await simulations.EndAsync(admin.Id, id, ct);
        Response.Cookies.Delete(CookieName, new CookieOptions { Path = "/" });
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Current(CancellationToken ct)
    {
        var admin = GetUser();
        if (!admin.IsAdmin) return StatusCode(403);
        var selected = ReadTicket(admin.Id, Request.Cookies[CookieName]) is { } id
            ? await simulations.GetAsync(admin.Id, id, ct) : null;
        return selected is null ? Json(new { selected = false }) : Json(new
        {
            selected = true, shopName = selected.ShopName, shopId = selected.ShopId,
            merchantIdentifier = selected.MerchantIdentifier,
            expiresAtUtc = DateTime.SpecifyKind(selected.ExpiresAtUtc, DateTimeKind.Utc)
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RequestToken(string? contextTicket, int displayedShopId,
        IntegrationProvider provider, IntegrationCredentialType credentialType, CancellationToken ct)
    {
        var admin = GetUser();
        if (!admin.IsAdmin) return StatusCode(403);
        var id = ReadTicket(admin.Id, contextTicket);
        if (id is null) return BadRequest("ابتدا مغازه‌دار و مغازه را انتخاب کنید.");
        var request = await simulations.RequestTokenAsync(admin.Id, id.Value, displayedShopId, provider, credentialType, ct);
        if (request is null) return Conflict("زمینه شبیه‌سازی تغییر کرده یا منقضی شده است؛ دوباره مغازه را انتخاب کنید.");
        TempData["SimulationNotice"] = $"درخواست شبیه‌سازی با شناسه {request.Id} ثبت شد. هنوز توکنی از پلتفرم دریافت نشده است.";
        return RedirectToAction(nameof(Index));
    }

    private string Protect(string adminId, Guid id) =>
        tickets.Protect(adminId, id);

    private Guid? ReadTicket(string adminId, string? ticket) => tickets.Read(adminId, ticket);
}
