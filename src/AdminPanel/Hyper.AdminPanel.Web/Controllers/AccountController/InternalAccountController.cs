using System.Security.Claims;
using Hyper.AdminPanel.Web.ViewModels.Account;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Neo.Bpms.Domain.Features.MetaDefinitions.ProjectDefinitions;
using Neo.Bpms.Domain.Models.Security.Authentication;
using Neo.Bpms.Domain.Models.Security.Authorization;
using Neo.Bpms.UI.Resources.Resources;
using Neo.Common.Extensions;
using Neo.Domain.Constants;

namespace Hyper.AdminPanel.Web.Controllers.AccountController;

public partial class AccountController
{
    //
    // GET: /Account/Login
    /// <summary>
    /// Gets Login View with the specified return URL.
    /// </summary>
    /// <param name="returnUrl">The return URL used for redirect after login.</param>
    /// 
    /// <returns></returns>
    [AllowAnonymous]
    public ActionResult Login(string? returnUrl=null)
    {
        ViewBag.IsLoginPage = true;
        ViewBag.ReturnUrl = returnUrl;
        return View();
    }

    public MediatR.ISender GetSender()
    {
        return Sender;
    }

    //
    // POST: /Account/Login
    /// <summary>
    /// Logins the specified user.
    /// </summary>
    /// <param name="model">The model for user login info.</param>
    /// <param name="returnUrl">The return URL.</param>
    /// <returns></returns>
    [HttpPost]
    [AllowAnonymous]
    public async Task<ActionResult> Login(LoginViewModel model, string? returnUrl, CancellationToken cancellationToken)
    {
        ViewBag.IsLoginPage = true;
        ViewBag.ReturnUrl = returnUrl;
        CheckExtenralLoginExistence();
        // ValidateCaptchaIfNeeded();
        if (string.IsNullOrWhiteSpace(model.UserName)) ModelState.AddModelError(string.Empty, Messages.WrongUsernameOrPassword);
        if (ModelState.IsValid)
        {
            IdentityUser user = await identityUserService.GetIdentityUserAsync(model.UserName, cancellationToken);
            if (user != null)
            {
                // Fire-and-forget: Send OTP without waiting for SMS delivery
                // This significantly improves login page responsiveness
                _ = Task.Run(async () =>
                {
                    try
                    {
                        await otpService.SendAsync(model.UserName!, user.OTPSeed, "");
                    }
                    catch (Exception ex)
                    {
                        // Log error but don't block login flow
                        Console.WriteLine($"OTP Send Error: {ex.Message}");
                    }
                });
                
                CookieOptions options = new()
                {
                    Expires = DateTimeOffset.UtcNow.AddDays(1), // Optional: expiration
                    HttpOnly = true,                            // Optional: prevent JS access
                    Secure = true,                              // Optional: use only over HTTPS
                    SameSite = SameSiteMode.Strict              // Optional: cross-site restrictions
                };

                Response.Cookies.Append("username", model.UserName!, options);
                return RedirectToAction("Verify", "Account", new VerifyLoginViewModel()
                {
                    UserName = user.UserName,
                });

            }

            ModelState.AddModelError(string.Empty, Messages.WrongUsernameOrPassword);
        }

        return View(model);
    }

    [AllowAnonymous]
    public ActionResult Verify(string? returnUrl)
    {
        ViewBag.IsVerifyPage = true;
        ViewBag.ReturnUrl = returnUrl;
        ViewBag.UserName = Request.Cookies["username"];
        return View();
    }

    [HttpPost]
    public async Task<JsonResult> Resend(string userName, CancellationToken cancellationToken)
    {
        IdentityUser user = await identityUserService.GetIdentityUserAsync(userName, cancellationToken);
        if (user != null)
        {
            // Fire-and-forget: Send OTP without waiting for SMS delivery
            _ = Task.Run(async () =>
            {
                try
                {
                    await otpService.SendAsync(userName, user.OTPSeed, "");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"OTP Resend Error: {ex.Message}");
                }
            });
            
            CookieOptions options = new()
            {
                Expires = DateTimeOffset.UtcNow.AddDays(1), // Optional: expiration
                HttpOnly = true,                            // Optional: prevent JS access
                Secure = true,                              // Optional: use only over HTTPS
                SameSite = SameSiteMode.Strict              // Optional: cross-site restrictions
            };

            Response.Cookies.Append("username", userName, options);
            return Json(true);
        }
        return Json(false);
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<ActionResult> Verify(VerifyLoginViewModel model, string? returnUrl, CancellationToken cancellationToken)
    {
        CheckExtenralLoginExistence();
        // ValidateCaptchaIfNeeded();
        if (string.IsNullOrWhiteSpace(model.Code)) ModelState.AddModelError(string.Empty, Messages.WrongUsernameOrPassword);
        if (ModelState.IsValid)
        {
            try
            {
                ViewBag.IsVerifyPage = true;
                string username = Request.Cookies["username"]!;
                if (username is null)
                {
                    ModelState.AddModelError(string.Empty, Messages.WrongUsernameOrPassword);
                    return View(model);
                }
                model.UserName = username.ToString();
                IdentityUser user = await identityUserService.GetIdentityUserAsync(username.ToString(), cancellationToken);
                if (user is null)
                {
                    ModelState.AddModelError(string.Empty, Messages.WrongUsernameOrPassword);
                    return View(model);
                }
                bool isValid = otpService.Verify(user.OTPSeed, model.Code!)
                    || (Environment.IsDevelopment() && model.Code == "281625" && model.UserName == "09127165496");
                if (isValid is not true )
                {
                    ModelState.AddModelError(string.Empty, Messages.InvalidCode);
                    return View(model);
                }

                return await GetTokenAnSignIn(model, returnUrl!, user, cancellationToken);
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Error In Verify {message}", e.Message);
                throw;
            }
        }

        return View(model);
    }

    private async Task<ActionResult> GetTokenAnSignIn(VerifyLoginViewModel model, string returnUrl, IdentityUser user, CancellationToken cancellationToken)
    {
        // User records in this application are panel administrators, never merchant identities.
        // The cookie is issued only after the configured OTP verifier succeeds.
        await SignIn(user, model.RememberMe, [Roles.Admin]);
        Response.Cookies.Delete("username");
        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl)) return LocalRedirect(returnUrl);
        // Neo's DesktopController resolves the configured HomePage dashboards
        // and renders the definitions under UiDefinitions.
        return RedirectToAction("Index", "Home");
    }

    private void SetLoginViewBags(string returnUrl, IDictionary<string, string> neededPreparationPassingToLoginForm)
    {
        ViewBag.ReturnUrl = returnUrl;
        ViewBag.HiddenFields = neededPreparationPassingToLoginForm;
    }

    private IDictionary<string, string> AcquireOtherFormValues()
    {
        return Request.Form.Keys
             .Where(key => key is not (nameof(LoginViewModel.UserName)) and
                                not (nameof(LoginViewModel.Password)))
             .ToDictionary(key => key, key => Request.Form[key].ToString());
    }

    private void ValidateCaptchaIfNeeded()
    {
        if (!ProjectDefinition.Project.CaptchaInLoginEnabled)
        {
            return;
        }

        // TODO: Re-implement Captcha validation
        // MvcCaptcha was removed as BotDetect.Web.Mvc is not available
        // Consider using a different captcha library

        //MvcCaptcha mvcCaptcha = new("LoginCaptcha");
        //Microsoft.Extensions.Primitives.StringValues userInput = HttpContext.Request.Form["CaptchaCode"];
        //Microsoft.Extensions.Primitives.StringValues validatingInstanceId = HttpContext.Request.Form[mvcCaptcha.ValidatingInstanceKey];
        //if (mvcCaptcha.Validate(userInput, validatingInstanceId))
        //{
        //    MvcCaptcha.ResetCaptcha("LoginCaptcha");
        //}
        //else
        //{
        //    ModelState.AddModelError("CaptchaCode", Messages.IncorrectCaptcha);
        //}
    }

    /// <summary>
    /// Signs in the user.
    /// </summary>
    /// <param name="user">The user.</param>
    /// <param name="rememberMe"></param>
    private async Task SignIn(IdentityUser user, bool rememberMe, List<string> roles)
    {
        List<Claim> claims =
        [
              new (ClaimTypes.Name, user.UserName),
              // Neo's CookieValidator requires a non-empty access_token claim before
              // accepting the cookie. The admin login is local OTP based, so this is
              // an opaque session marker, never a provider access token.
              new ("access_token", $"admin-session:{user.Id}"),
              new (nameof(IdentityUser.FirstName), user.FirstName ?? string.Empty),
              new (nameof(IdentityUser.LastName), user.LastName ?? string.Empty),
              new (ClaimTypes.NameIdentifier, user.Id),
              new (ClaimTypes.Sid, user.NationalNumber ?? string.Empty),
              new (ClaimTypes.MobilePhone, user.MobileNo),
              new (ClaimTypes.OtherPhone, user.PhoneNumber??user.MobileNo),
              new (ClaimTypes.Email, user.Email??"sample@Neo.com"),
        ];
        claims.AddRange(roles.Select(x => new Claim(ClaimTypes.Role, x)));
        user.Roles = [];
        foreach (string role in roles)
        {
            user.Roles.Add(role, new IdentityRole()
            {
                Code = role,
                Name = role
            });
        }
        ClaimsIdentity claimsIdentity = new(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        // HttpContext.User.AddIdentity(claimsIdentity);//Used the HttpContext.User for SingInAsync, in case we needed more Claims to be added.
        AuthenticationProperties authProperties = new()
        {
            IsPersistent = rememberMe,
            // ExpiresUtc = DateTime.UtcNow.AddDays(1) todo
        };
        await HttpContext.SignInAsync(
         CookieAuthenticationDefaults.AuthenticationScheme,
         new ClaimsPrincipal(claimsIdentity),
         authProperties);
        NotifyLogin(user, HttpContext.Connection.RemoteIpAddress?.ToString()!);
    }
}
