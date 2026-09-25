using Hyper.Integration.Domain.Entities.Integrations;
using Hyper.Integration.Domain.Features.Integrations;
using Hyper.Infrastructure.Features.Integrations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Neo.Bpms.UI.MVC.Controllers.Public;

namespace Hyper.AdminPanel.Web.Controllers;

[ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
[Route("api/auth/basalam")]
public sealed class OAuthCallbackController(BasalamOAuthService oauth, BasalamOAuthStore store,
    IAdminMerchantSimulationService simulations, AdminSimulationTickets tickets,
    IBasalamWebhookRegistration webhookRegistration,
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
    // The simulator form already carries a signed, short-lived context ticket. Do not
    // reject the OAuth start with ASP.NET's antiforgery cookie check: the external
    // redirect can cross browser cookie policies before the callback, while the
    // signed ticket still binds this request to the authenticated admin/shop.
    [IgnoreAntiforgeryToken]
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
        // Never log the full URL: it contains the protected state ticket.
        logger.LogInformation("Basalam OAuth authorization redirect prepared for request {RequestId}; endpoint={Endpoint}; callback={Callback}",
            request.Id, oauth.AuthorizationEndpoint, oauth.RedirectUri);
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
        catch (InvalidOperationException ex) { return CallbackNotice(ex.Message); }
        var selected = await simulations.GetAsync(data.AdminId, data.SimulationId, ct);
        if (selected is null) return CallbackNotice("زمینه مغازه پایان یافته است؛ اتصال را دوباره شروع کنید.");
        if (!await store.TryClaimAsync(data, ct)) return CallbackNotice("این درخواست قبلاً مصرف شده یا دیگر معتبر نیست.");
        Response.Cookies.Delete(CorrelationCookie, new CookieOptions { Path = "/api/auth/basalam" });
        if (!string.IsNullOrEmpty(error))
        {
            await store.SetOutcomeAsync(data.RequestId, 3, ct);
            return CallbackNotice("اجازه دسترسی در باسلام صادر نشد؛ توکنی ذخیره نشد.");
        }
        if (string.IsNullOrWhiteSpace(code) || code.Length > 8192)
        {
            await store.SetOutcomeAsync(data.RequestId, 4, ct);
            return CallbackNotice("باسلام کد مجوز معتبر برنگرداند؛ اتصال را دوباره شروع کنید.");
        }
        try
        {
            var token = await oauth.ExchangeCodeForTokenAsync(code, data, ct);
            var vendor = await oauth.GetVendorAsync(token, ct);
            var connectionId = await store.SaveAsync(data, selected, vendor, token, ct);
            await webhookRegistration.RegisterForConnectionAsync(connectionId, selected.ShopId,
                selected.TenantId, vendor.Id, oauth.RedirectUri, ct);
            return CallbackNotice($"اتصال غرفه «{vendor.Title}» برقرار شد و وب‌هوک‌های باسلام ثبت شدند.");
        }
        catch (Exception ex) when (ex is not OperationCanceledException || !ct.IsCancellationRequested)
        {
            // Do not log response bodies, authorization codes, state or SQL parameter values.
            logger.LogWarning("Basalam callback failed for request {RequestId}; category {Category}", data.RequestId, ex.GetType().Name);
            await store.SetOutcomeAsync(data.RequestId, 4, ct);
            return CallbackNotice(ex is InvalidOperationException ? ex.Message : "دریافت یا ذخیره توکن ناموفق بود؛ تنظیمات و گزارش سرور را بررسی کنید و دوباره شروع کنید.");
        }
    }

    private IActionResult Notice(string message)
    {
        TempData["SimulationNotice"] = message;
        return RedirectToAction("Index", "MerchantSimulation");
    }

    private ContentResult CallbackNotice(string message) =>
        Content($"<!doctype html><meta charset=\"utf-8\"><title>نتیجه اتصال باسلام</title>" +
                $"<main dir=\"rtl\" style=\"font-family:sans-serif;max-width:640px;margin:4rem auto\"><h2>{System.Net.WebUtility.HtmlEncode(message)}</h2>" +
                "<p>می‌توانید این صفحه را ببندید و به پنل مدیریت برگردید.</p></main>", "text/html; charset=utf-8");
}

