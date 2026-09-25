using Hyper.WorkManagement.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Neo.Bpms.UI.MVC.Controllers.Public;

namespace Hyper.AdminPanel.Web.Controllers;

[Authorize]
[ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
public sealed class WorkManagementPageController(IWorkManagementApi api) : ControllerBaseMVC
{
    [HttpGet]
    public async Task<IActionResult> Index(string? domain, CancellationToken ct = default)
    {
        var admin = GetUser();
        if (!admin.IsAdmin) return StatusCode(403);
        ViewBag.AdminDisplayName = admin.UserName;
        ViewBag.Domain = domain;
        return View(await api.GetBoardAsync(domain, ct));
    }
}
