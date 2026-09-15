using Hyper.AdminPanel.Web.ViewModels;
using Hyper.Infrastructure.Features.Integrations;
using Microsoft.Data.SqlClient;
using Hyper.Domain.Entities.Integrations;
using Hyper.Domain.Features.Integrations;
using Microsoft.AspNetCore.Mvc;
using Neo.Bpms.UI.MVC.Controllers.Public;

namespace Hyper.AdminPanel.Web.Controllers;

[ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
public sealed class MerchantSimulationController(IAdminMerchantSimulationService simulations,
    AdminSimulationTickets tickets, IIntegrationDashboardQuery dashboard,
    BasalamOAuthService oauth, BasalamOAuthStore oauthStore, IIntegrationScenarioQueue scenarios) : ControllerBaseMVC
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
    public async Task<IActionResult> Index(string? search, CancellationToken ct, IntegrationProvider integrationProvider = IntegrationProvider.Basalam)
    {
        var admin = GetUser();
        if (!admin.IsAdmin) return StatusCode(403);
        if (search?.Length > 200) return BadRequest();
        var selected = ReadTicket(admin.Id, Request.Cookies[CookieName]) is { } id
            ? await simulations.GetAsync(admin.Id, id, ct) : null;
        var authorization = IntegrationAuthorizationCatalog.Find(integrationProvider);
        if (authorization is null) return BadRequest("پلتفرم معتبر نیست.");
        ViewBag.Authorization = authorization;
        var shops = await simulations.SearchShopsAsync(search, ct);
        ViewBag.OAuthConfigurationError = authorization.UnavailableReason ?? oauth.ConfigurationError();
        if (selected is not null)
        {
            var scope = new OwnedIntegrationShop(selected.ShopId, selected.TenantId);
            ViewBag.ScenarioConnections = await scenarios.ConnectionsAsync(scope, ct);
            ViewBag.ScenarioJobs = await scenarios.RecentAsync(scope, ct);
        }
        ViewBag.OAuthScopes = oauth.Scopes;
        ViewBag.OAuthRedirectUri = oauth.RedirectUri;
        if (selected is not null)
        {
            try { ViewBag.OAuthStatus = await oauthStore.GetStatusAsync(selected.ShopId, selected.TenantId, ct); }
            catch (SqlException ex) when (ex.Number == 208)
            {
                ViewBag.OAuthConfigurationError = "جدول توکن آماده نیست؛ اسکریپت docs/schema/ensure-external-oauth-tokens.sql را روی Hyperyek اجرا کنید.";
            }
        }
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

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ReceiveScenario(string? contextTicket, long connectionId, string eventId,
        IntegrationSyncItem item, IntegrationSyncTrigger trigger, CancellationToken ct)
    {
        var admin = GetUser();
        if (!admin.IsAdmin) return StatusCode(403);
        if (!ModelState.IsValid) return BadRequest("رویداد معتبر نیست.");
        var id = ReadTicket(admin.Id, contextTicket);
        if (id is null || ReadTicket(admin.Id, Request.Cookies[CookieName]) != id)
            return Conflict("زمینه مغازه تغییر کرده است؛ صفحه را تازه کنید.");
        var selected = await simulations.GetAsync(admin.Id, id.Value, ct);
        if (selected is null) return Conflict("زمینه مغازه منقضی شده است.");
        if (!Guid.TryParseExact(eventId, "N", out var eventKey)) return BadRequest("شناسه رویداد معتبر نیست.");
        try
        {
            var jobId = await scenarios.EnqueueAsync(new OwnedIntegrationShop(selected.ShopId, selected.TenantId), connectionId,
                new IntegrationScenarioRequest($"simulation:{eventKey:N}", item, trigger), ct);
            TempData["SimulationNotice"] = $"رویداد آزمایشی شماره {jobId} در صف ثبت شد. ثبت در صف به معنی اعمال موفق نیست.";
        }
        catch (ArgumentException) { return BadRequest("نوع آیتم یا محرک معتبر نیست."); }
        catch (InvalidOperationException)
        {
            return Conflict("اتصال متعلق به این زمینه نیست یا شناسه رویداد با درخواست قبلی تعارض دارد.");
        }
        return RedirectToAction(nameof(Index));
    }

    private string Protect(string adminId, Guid id) =>
        tickets.Protect(adminId, id);

    private Guid? ReadTicket(string adminId, string? ticket) => tickets.Read(adminId, ticket);
}
