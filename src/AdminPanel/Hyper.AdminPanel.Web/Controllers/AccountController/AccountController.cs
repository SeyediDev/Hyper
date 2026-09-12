using Neo.Bpms.Domain.Features.MetaDefinitions.ProjectDefinitions;
using Neo.Bpms.UI.MVC.Controllers.Public;
using Neo.Bpms.UI.MVC.Exceptions;
using Neo.Domain.Features.Sms;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Neo.Bpms.Domain.Models.Security.Authentication;

namespace Hyper.AdminPanel.Web.Controllers.AccountController;

public partial class AccountController(
    ISmsService smsService, IIdentityUserService identityUserService,
    //IIdpService idpService, IJwtDecode jwtDecode,
    IOtpService otpService, IWebHostEnvironment environment)
    : ControllerBaseMVC
{
    protected IWebHostEnvironment Environment { get; } = environment;
    [HttpPost]
    [AllowAnonymous]
    [IgnoreAntiforgeryToken]
    public JsonResult ChangeCulture(string culture)
    {
        try
        {
            culture = CultureHelper.GetImplementedCulture(culture);
            Response.Cookies.Append("_culture", culture, new CookieOptions()
            {
                Expires = DateTime.UtcNow.AddYears(1)
            });
            return Json(new { success = true, culture = culture });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpGet]
    public async Task<ActionResult> LogOff()
    {
        await SignOutAsync();
        return RedirectToAction("Login", "Account");
    }

    #region Helpers

    private void NotifyLogin(IdentityUser user, string ip)
    {
        try
        {
            string receiver = $"{user.FirstName} {user.LastName}";
            string systemName = ProjectDefinition.Project.ProjectName;
            DateTime date = DateTime.UtcNow;
            string dateTime = date.ToString("g");
            string smsText = $"{receiver} عزیز\n\rورود به {systemName}\n\r در تاریخ \n\r{dateTime}";
            string emailText =
                 $"{receiver} عزیز<br>ورود به {systemName}<br> در تاریخ <br>{dateTime}<br>از آدرس : {ip}";
            Notify(smsText, receiver);
        }
        catch
        {
            // ignored
        }
    }

    private void NotifyFailedLogin(IdentityUser user, string ip)
    {
        try
        {
            string receiver = $"{user.FirstName} {user.LastName}";
            string systemName = ProjectDefinition.Project.ProjectName;
            DateTime date = DateTime.UtcNow;
            string dateTime = date.ToString("g");
            string smsText = $"{receiver} عزیز\n\rورود ناموفق به {systemName}\n\r در تاریخ \n\r{dateTime}";
            string emailText =
                 $"{receiver} عزیز<br>ورود ناموفق به {systemName}<br> در تاریخ <br>{dateTime}<br>از آدرس : {ip}";
            Notify(smsText, receiver);
        }
        catch
        {
            // ignored
        }
    }

    private void Notify(string smsText, string receiver)
    {
        _ = smsService.SendAsync(new Neo.Domain.Features.Sms.Dto.SmsDto(receiver, smsText));
    }

    /// <summary>
    /// Signs out the user.
    /// </summary>
    private async Task SignOutAsync()
    {
        IdentityUser user = GetUser(User);
        if (user != null)
        {
            //TODO MRSH LogOff in idp
        }
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        if (ExternalLoginIntegrator != null)
        {
            if (user != null)
            {
                Dictionary<string, string> parameters = new()
                {
                    { "userId", user?.Id! },
                };
                await ExternalLoginIntegrator.Signout(parameters);
            }
            throw new UnauthenticatedUserException();
        }
        _ = RedirectToAction("Login", "Account");
    }
    #endregion
}
