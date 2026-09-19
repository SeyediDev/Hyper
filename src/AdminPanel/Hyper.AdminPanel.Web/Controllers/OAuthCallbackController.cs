using Hyper.Domain.Entities.Integrations;
using Hyper.Domain.Features.Integrations;
using Hyper.Infrastructure.Features.Integrations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Neo.Bpms.UI.MVC.Controllers.Public;

namespace Hyper.AdminPanel.Web.Controllers;

[ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
[Route("api/auth/basalam")]
public sealed class OAuthCallbackController(BasalamOAuthService oauth, BasalamOAuthStore store,
    IAdminMerchantSimulationService simulations, AdminSimulationTickets tickets,
    ILogger<OAuthCallbackController> logger) : ControllerBaseMVC
{
    private const string CorrelationCookie = "Hyper.Basalam.Correlation";
    private const string SimulationCookie = "Hyper.AdminMerchantSimulation";

    // This endpoint starts OAuth only through the selected-shop POST form. A direct
    // browser navigation must return to the simulator instead of leaving the user
    // on the internal API URL without the required context ticket and antiforgery token.
    [HttpGet("login")]
    public IActionResult LoginGet() => RedirectToAction("Index", "MerchantSimulation");

    [HttpPost("login")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(string? contextTicket, int displayedShopId, CancellationToken ct)
    {
        var admin = GetUser();
        if (!admin.IsAdmin) return StatusCode(403);
        var id = tickets.Read(admin.Id, contextTicket);
        if (id is null || tickets.Read(admin.Id, Request.Cookies[SimulationCookie]) != id)
            return Notice("ابتدا مغازه را انتخاب کنید؛ فرم انتخاب‌شده معتبر نیست.");
        if (oauth.ConfigurationError() is { } error) return Notice(error);
        var selected = await simulations.GetAsync(admin.Id, id.Value, ct);
        if (selected is null || selected.ShopId != displayedShopId)
            return Notice("زمینه مغازه تغییر کرده یا منقضی شده است.");
        var request = await simulations.RequestTokenAsync(admin.Id, selected.Id, selected.ShopId,
            IntegrationProvider.Basalam, IntegrationCredentialType.OAuth2, ct);
        if (request is null) return Notice("زمینه مغازه منقضی شده است.");
        var nonce = BasalamOAuthService.Nonce();
        var url = oauth.CreateAuthorizationUrl(request.Id, selected.Id, admin.Id, nonce);
        logger.LogInformation("OAuth redirect URL: {Url}", url);
        Response.Cookies.Append(CorrelationCookie, nonce, new CookieOptions
        {
            HttpOnly = true, Secure = Request.IsHttps, SameSite = SameSiteMode.Lax,
            Path = "/api/auth/basalam", MaxAge = TimeSpan.FromMinutes(10), IsEssential = true
        });
        return Redirect(url);
    }

    // Basalam returns code + state. No session and no nonstandard encrypted_state parameter.
    [AllowAnonymous]
    [HttpGet("callback")]
    public async Task<IActionResult> Callback(string? code, string? state, string? error, CancellationToken ct)
    {
        Response.Headers["Referrer-Policy"] = "no-referrer";
        BasalamAuthorizationState data;
        try { data = oauth.ReadState(state, Request.Cookies[CorrelationCookie]); }
        catch (InvalidOperationException ex) { return Notice(ex.Message); }
        var selected = await simulations.GetAsync(data.AdminId, data.SimulationId, ct);
        if (selected is null) return Notice("زمینه مغازه پایان یافته است؛ اتصال را دوباره شروع کنید.");
        if (!await store.TryClaimAsync(data, ct)) return Notice("این درخواست قبلاً مصرف شده یا دیگر معتبر نیست.");
        Response.Cookies.Delete(CorrelationCookie, new CookieOptions { Path = "/api/auth/basalam" });
        if (!string.IsNullOrEmpty(error))
        {
            await store.SetOutcomeAsync(data.RequestId, 3, ct);
            return Notice("اجازه دسترسی در باسلام صادر نشد؛ توکنی ذخیره نشد.");
        }
        if (string.IsNullOrWhiteSpace(code) || code.Length > 8192)
        {
            await store.SetOutcomeAsync(data.RequestId, 4, ct);
            return Notice("باسلام کد مجوز معتبر برنگرداند؛ اتصال را دوباره شروع کنید.");
        }
        try
        {
            var token = await oauth.ExchangeCodeForTokenAsync(code, data, ct);
            var vendor = await oauth.GetVendorAsync(token, ct);
            await store.SaveAsync(data, selected, vendor, token, ct);
            return Notice($"توکن غرفه «{vendor.Title}» برای مغازه «{selected.ShopName}» ذخیره شد. یکسان‌سازی خودکار هنوز فعال نشده است.");
        }
        catch (Exception ex) when (ex is not OperationCanceledException || !ct.IsCancellationRequested)
        {
            // Do not log response bodies, authorization codes, state or SQL parameter values.
            logger.LogWarning("Basalam callback failed for request {RequestId}; category {Category}", data.RequestId, ex.GetType().Name);
            await store.SetOutcomeAsync(data.RequestId, 4, ct);
            return Notice(ex is InvalidOperationException ? ex.Message : "دریافت یا ذخیره توکن ناموفق بود؛ تنظیمات و گزارش سرور را بررسی کنید و دوباره شروع کنید.");
        }
    }

    private IActionResult Notice(string message)
    {
        TempData["SimulationNotice"] = message;
        return RedirectToAction("Index", "MerchantSimulation");
    }
}
