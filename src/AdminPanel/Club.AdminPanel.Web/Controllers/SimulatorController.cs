using Microsoft.AspNetCore.Mvc;

namespace Hyper.AdminPanel.Web.Controllers;

/// <summary>
/// کنترلر شبیه‌ساز کانال برای تست API ارسال رویداد
/// </summary>
public class SimulatorController : Controller
{
    /// <summary>
    /// صفحه اصلی شبیه‌ساز کانال
    /// </summary>
    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }
}
