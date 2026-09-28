using Hyper.Integration.Contracts;
using Hyper.Integration.Domain.Features.Integrations;
using Microsoft.AspNetCore.Mvc;

namespace Hyper.AdminPanel.Web.Controllers;

public sealed partial class MerchantSimulationController
{
    [HttpGet]
    public async Task<IActionResult> Products(long? connectionId, CancellationToken ct,
        ProductTransferDirection direction = ProductTransferDirection.ToPlatform, int skip = 0)
    {
        var admin = GetUser(); if (!admin.IsAdmin) return StatusCode(403);
        if (!Enum.IsDefined(direction) || skip < 0) return BadRequest();
        var selected = ReadTicket(admin.Id, Request.Cookies[CookieName]) is { } id ? await simulations.GetAsync(admin.Id, id, ct) : null;
        if (selected is null) return RedirectToAction(nameof(Index));
        var connections = await scenarios.ConnectionsAsync(new(selected.ShopId, selected.TenantId), ct);
        var chosen = connectionId ?? connections.FirstOrDefault()?.Id;
        var scope = new IntegrationConnectionCommandRequest(selected.ShopId, selected.TenantId, chosen ?? 0);
        var board = chosen is null ? null : await readiness.BoardAsync(scope, skip, 25, ct);
        if (chosen is not null && board is null) return NotFound();
        ViewBag.ProductConnections = connections;
        return View(new ViewModels.ProductReadinessViewModel(chosen, direction, skip, Protect(admin.Id, selected.Id),
            selected.ShopName, chosen is null ? new() : (await readiness.PolicyAsync(scope, direction, ct))!, board));
    }
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> PrepareProduct(string contextTicket, long connectionId,
        ProductPreparationInput input, int expectedRevision, CancellationToken ct)
    {
        var admin = GetUser(); if (!admin.IsAdmin) return StatusCode(403);
        if (!ModelState.IsValid) return BadRequest("اطلاعات معتبر نیست.");
        var id = ReadTicket(admin.Id, contextTicket);
        if (id is null || ReadTicket(admin.Id, Request.Cookies[CookieName]) != id) return Conflict("زمینه مغازه تغییر کرده است.");
        var selected = await simulations.GetAsync(admin.Id, id.Value, ct);
        if (selected is null) return Conflict("زمینه مغازه منقضی شده است.");
        try
        {
            var result = await readiness.PrepareAsync(new(selected.ShopId, selected.TenantId, connectionId), input, expectedRevision, admin.Id, ct);
            if (result is null) return NotFound();
            TempData["SimulationNotice"] = result.JobId is null ? "نقص‌ها ثبت شدند؛ پس از اصلاح دوباره بررسی کنید." : "درخواست در صف قرار گرفت؛ موفقیت نهایی پس از اجرای پردازشگر مشخص می‌شود.";
        }
        catch (ArgumentException) { return BadRequest("شناسه یا مشخصات کالا معتبر نیست."); }
        catch (InvalidOperationException) { return Conflict("نسخه تغییر کرده یا درخواست قبلاً ارسال شده است؛ صفحه را تازه کنید."); }
        return RedirectToAction(nameof(Products), new { connectionId, direction = input.Direction });
    }
    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> ProductPolicy(string contextTicket, long connectionId, ProductTransferDirection direction,
        ProductPreparationPolicy policy, CancellationToken ct)
    {
        var admin = GetUser(); if (!admin.IsAdmin) return StatusCode(403);
        if (!ModelState.IsValid || !Enum.IsDefined(direction) || !Enum.IsDefined(policy.Mode)) return BadRequest();
        var id = ReadTicket(admin.Id, contextTicket);
        if (id is null || ReadTicket(admin.Id, Request.Cookies[CookieName]) != id) return Conflict("زمینه مغازه تغییر کرده است.");
        var selected = await simulations.GetAsync(admin.Id, id.Value, ct);
        if (selected is null) return Conflict("زمینه مغازه منقضی شده است.");
        if (!await readiness.SetPolicyAsync(new(selected.ShopId, selected.TenantId, connectionId), direction, policy, admin.Id, ct)) return NotFound();
        TempData["SimulationNotice"] = "سیاست ذخیره شد؛ برای اعمال آن روی موارد متوقف، بررسی مجدد را بزنید. موارد ارسال‌شده تغییر نمی‌کنند.";
        return RedirectToAction(nameof(Products), new { connectionId, direction });
    }
}
