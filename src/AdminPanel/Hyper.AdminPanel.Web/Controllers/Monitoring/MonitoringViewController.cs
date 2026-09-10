using System.Globalization;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Hyper.AdminPanel.Web.Controllers.Monitoring;

/// <summary>
/// Controller for System Monitoring dashboard views
/// این Controller فقط برای سرو View ها استفاده می‌شود
/// منطق API در Neo.Endpoint.Controller.Api.MonitoringController است
/// </summary>
[Authorize]
public class MonitoringViewController : Controller
{
    private readonly ILogger<MonitoringViewController> _logger;

    public MonitoringViewController(ILogger<MonitoringViewController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Main monitoring dashboard page (MVC version)
    /// استفاده مستقیم از MonitoringController از Neo.Endpoint که View مستقل دارد
    /// </summary>
    public IActionResult Index()
    {
        _logger.LogInformation("Monitoring dashboard accessed by user {User}", User.Identity?.Name);
        
        // تنظیم ViewBag و Culture مانند MonitoringController در Neo
        ViewBag.PagePackId = "/Monitoring/Index";
        ViewBag.user = GetUser(User);
        SetPersianCulture();
        
        // تنظیم نام API برای View
        var apiName = "پنل ادمین";
        ViewBag.ApiName = apiName;
        ViewData["Title"] = $"مانیتورینگ {apiName}";
        
        // برگرداندن View مستقیماً از مسیر Views/Monitoring/Index.cshtml
        return View("Index");
    }

    /// <summary>
    /// Advanced monitoring dashboard (React micro-frontend)
    /// استفاده مستقیم از MonitoringController از Neo.Endpoint که View مستقل دارد
    /// </summary>
    public IActionResult Advanced()
    {
        _logger.LogInformation("Advanced monitoring dashboard accessed by user {User}", User.Identity?.Name);
        
        // تنظیم ViewBag و Culture مانند MonitoringController در Neo
        ViewBag.PagePackId = "/Monitoring/Advanced";
        ViewBag.user = GetUser(User);
        SetPersianCulture();
        
        // برگرداندن View مستقیماً از مسیر Views/Monitoring/Advanced.cshtml
        return View("Advanced");
    }
    
    /// <summary>
    /// تنظیم Culture به فارسی برای صفحه مانیتورینگ
    /// </summary>
    private void SetPersianCulture()
    {
        try
        {
            var persianCulture = new CultureInfo("fa-IR");
            CultureInfo.CurrentCulture = persianCulture;
            CultureInfo.CurrentUICulture = persianCulture;
            
            // تنظیم Culture برای Thread جاری
            Thread.CurrentThread.CurrentCulture = persianCulture;
            Thread.CurrentThread.CurrentUICulture = persianCulture;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to set Persian culture, using default culture");
        }
        
        // تنظیم CSP header برای جلوگیری از خطاهای BrowserLink
        Response.Headers.Append("Content-Security-Policy",
            "default-src 'self'; " +
            "script-src 'self' 'unsafe-inline' 'unsafe-eval'; " +
            "style-src 'self' 'unsafe-inline' https://cdn.jsdelivr.net; " +
            "connect-src 'self' ws://localhost:* http://localhost:* ws://127.0.0.1:* http://127.0.0.1:* ws://* http://* https://*; " +
            "img-src 'self' data: https:; " +
            "font-src 'self' data: https://cdn.jsdelivr.net https://fonts.googleapis.com https://fonts.gstatic.com;");
    }
    
    private object GetUser(ClaimsPrincipal claimsPrincipal)
    {
        if (claimsPrincipal.Identity is null || !claimsPrincipal.Identity.IsAuthenticated)
        {
            return GenerateUser(claimsPrincipal);
        }
        
        var requesterUser = HttpContext.RequestServices.GetRequiredService<Neo.Domain.Features.Client.IRequesterUser>();
        var userProperty = requesterUser.GetProperty("IdentityUser");
        if (userProperty != null)
        {
            return userProperty;
        }
        return GenerateUser(claimsPrincipal);
    }

    private static object GenerateUser(ClaimsPrincipal? claimsPrincipal)
    {
        var claims = claimsPrincipal?.Claims ?? Enumerable.Empty<Claim>();
        var idClaim = claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier) 
            ?? claims.FirstOrDefault(c => c.Type == "sub");
        var nameClaim = claims.FirstOrDefault(c => c.Type == ClaimTypes.Name) 
            ?? claims.FirstOrDefault(c => c.Type == "name");
        
        string userName = (claimsPrincipal != null ? GetPrincipalName(claimsPrincipal) : null)
            ?? nameClaim?.Value 
            ?? claimsPrincipal?.Identity?.Name 
            ?? "Anonymous User";
        string userId = idClaim?.Value 
            ?? claimsPrincipal?.Identity?.Name 
            ?? Guid.NewGuid().ToString();
        
        // ایجاد یک anonymous object که فیلدهای مورد نیاز View را دارد
        return new
        {
            Id = userId,
            UserName = userName,
            FirstName = claims.FirstOrDefault(c => c.Type == "FirstName")?.Value,
            LastName = claims.FirstOrDefault(c => c.Type == "LastName")?.Value,
            NationalNumber = claims.FirstOrDefault(c => c.Type == ClaimTypes.Sid)?.Value,
            MobileNo = claims.FirstOrDefault(c => c.Type == ClaimTypes.MobilePhone)?.Value,
            PhoneNumber = claims.FirstOrDefault(c => c.Type == ClaimTypes.OtherPhone)?.Value,
            Email = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value
                ?? claims.FirstOrDefault(c => c.Type == "email")?.Value,
        };
    }

    private static string? GetPrincipalName(ClaimsPrincipal? principal)
    {
        string? name = principal?.Identity?.Name;
        if (string.IsNullOrEmpty(name))
            return null;
        string[] nameArray = name.Split('\\');
        return nameArray.Length < 1 ? null : nameArray[^1];
    }
}

