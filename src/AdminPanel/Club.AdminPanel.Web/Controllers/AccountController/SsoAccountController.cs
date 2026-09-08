using Neo.Bpms.UI.MVC.Exceptions;
using Neo.Bpms.UI.Resources.Resources;
using Neo.Domain.Features.Client.Dto;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Neo.Bpms.Domain.Models.Security.Authentication;

namespace Hyper.AdminPanel.Web.Controllers.AccountController;

public partial class AccountController
{
    private ISsoIntegrator? _externalLoginIntegrator;
    protected ISsoIntegrator ExternalLoginIntegrator =>
        _externalLoginIntegrator ??= null!;// HttpContext.RequestServices.GetRequiredService<IExternalLoginIntegrator>();

    [AllowAnonymous]
    [Route("{controller}/{action}")]
    public async Task<ActionResult> LoginCallback()
    {
        Dictionary<string, string> parameters = Request.Query.Keys
             .ToDictionary(k => k ?? "", v => Request.Query[v].ToString());


        if (ExternalLoginIntegrator == null)
        {
            throw new UnauthenticatedUserException();
        }

        Neo.Bpms.Domain.Features.Security.Dto.VerificationResult result = await ExternalLoginIntegrator.VerifyRedirectionAsync(parameters, HttpContext);
        if (result.Verified)
        {
            IdentityUser user = await identityUserService.GetIdentityUserAsync(GetUserName(result.Token));
            if (user != null)
            {
                //await SignIn(user, true, result.Token.access_token!/*TODO*/);
                return RedirectToAction("Index", "Home");
            }
            throw new Exception(Messages.AuthenticationFailed);
        }

        throw new Exception(result.Message ?? Messages.AuthenticationFailed);
    }

    private string GetUserName(TokenResponseDto token)
    {
        //TODO fetch user name from token
        return "09127165496";
    }
    private void CheckExtenralLoginExistence()
    {
        if (ExternalLoginIntegrator != null)
        {
            throw new Exception("You should login from SSO!");
        }
    }
}
