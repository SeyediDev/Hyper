using Hyper.AdminPanel.Web.Infrastructure;
using Hyper.AdminPanel.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Neo.Bpms.UI.MVC.Controllers.Public;

namespace Hyper.AdminPanel.Web.Controllers;

[Authorize]
[ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
public sealed class AccountingFinancialPreviewController(AdminFinancialPreviewWorkflow workflow) : ControllerBaseMVC
{
    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var admin = GetUser();
        if (!admin.IsAdmin) return StatusCode(403);
        var page = await workflow.PrepareAsync(admin.Id, Request.Cookies[AdminFinancialPreviewWorkflow.CookieName], ct);
        Response.StatusCode = page.StatusCode;
        return View(page);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequestSizeLimit(262_144)]
    public async Task<IActionResult> Preview(FinancialPreviewForm form, CancellationToken ct)
    {
        var admin = GetUser();
        if (!admin.IsAdmin) return StatusCode(403);
        if (!ModelState.IsValid) return BadRequest("ورودی پیش‌نمایش معتبر نیست.");
        var page = await workflow.PreviewAsync(admin.Id, Request.Cookies[AdminFinancialPreviewWorkflow.CookieName], form, ct);
        Response.StatusCode = page.StatusCode;
        return View("Index", page);
    }
}
