using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Neo.Bpms.UI.MVC.Controllers.Public;

namespace Hyper.AdminPanel.Web.Controllers;

[Authorize]
[ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
public sealed class WorkManagementPageController(IConfiguration configuration) : ControllerBaseMVC
{
    [HttpGet]
    public async Task<IActionResult> Index(string? domain, string? project, string? role, bool includeArchived = false, CancellationToken ct = default)
    {
        var admin = GetUser();
        if (!admin.IsAdmin) return StatusCode(403);
        var neoUrl = configuration["AgentOrchestration:WebUrl"];
        if (Uri.TryCreate(neoUrl, UriKind.Absolute, out var target) &&
            (target.Scheme == Uri.UriSchemeHttp || target.Scheme == Uri.UriSchemeHttps))
            return Redirect(target.ToString());
        ViewBag.AdminDisplayName = admin.UserName;
        ViewBag.Domain = domain;
        ViewBag.Project = project;
        ViewBag.Role = role;
        ViewBag.IncludeArchived = includeArchived;
        return View(await api.GetBoardAsync(domain, project, role, includeArchived, ct));
    }
}
