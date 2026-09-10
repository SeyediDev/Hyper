using Hyper.CustomerPortal.Application.Features.Auth.Commands;
using Hyper.CustomerPortal.Application.Features.Auth.Queries;
using Microsoft.AspNetCore.Authorization;

namespace Hyper.CustomerPortal.Api.Controllers;

[AppRoute("Hyper", "customer/auth")]
[Tags("customer/auth")]
[Authorize]
public class CustomerAuthController : AppControllerBase
{
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(LoginCommandResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<LoginCommandResponse>> Login([FromBody] LoginCommand command)
    {
        var response = await Sender.Send(command);
        return Ok(response);
    }

    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(LoginCommandResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<LoginCommandResponse>> Register([FromBody] RegisterCommand command)
    {
        var response = await Sender.Send(command);
        return Ok(response);
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        var command = new LogoutCommand();
        await Sender.Send(command);
        return Ok();
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(RefreshTokenCommandResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<RefreshTokenCommandResponse>> Refresh([FromBody] RefreshTokenCommand command)
    {
        var response = await Sender.Send(command);
        return Ok(response);
    }

    [HttpGet("profile")]
    public async Task<ActionResult<GetProfileQueryResponse>> GetProfile()
    {
        var query = new GetProfileQuery();
        var response = await Sender.Send(query);
        return Ok(response);
    }

    [HttpPut("profile")]
    public async Task<ActionResult<UpdateProfileCommandResponse>> UpdateProfile([FromBody] UpdateProfileCommand command)
    {
        var response = await Sender.Send(command);
        return Ok(response);
    }

    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordCommand command)
    {
        await Sender.Send(command);
        return Ok();
    }

    [HttpPost("forgot-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordCommand command)
    {
        await Sender.Send(command);
        return Ok();
    }

    [HttpPost("reset-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordCommand command)
    {
        await Sender.Send(command);
        return Ok();
    }
}

