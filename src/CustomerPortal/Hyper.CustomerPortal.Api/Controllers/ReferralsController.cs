using Hyper.CustomerPortal.Application.Features.Referrals.Commands;
using Hyper.CustomerPortal.Application.Features.Referrals.Queries;
using Microsoft.AspNetCore.Authorization;

namespace Hyper.CustomerPortal.Api.Controllers;

[AppRoute("Hyper", "customer/referrals")]
[Tags("customer/referrals")]
[ApiController]
[Authorize]
public class ReferralsController : AppControllerBase
{
    [HttpGet("stats")]
    [ProducesResponseType(typeof(GetReferralStatsQueryResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<GetReferralStatsQueryResponse>> GetStats()
    {
        var query = new GetReferralStatsQuery();
        var response = await Sender.Send(query);
        return Ok(response);
    }

    [HttpGet("customers")]
    [ProducesResponseType(typeof(GetReferredCustomersQueryResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<GetReferredCustomersQueryResponse>> GetReferredCustomers(
        [FromQuery] GetReferredCustomersQuery query)
    {
        var response = await Sender.Send(query);
        return Ok(response);
    }

    [HttpGet("leaderboard")]
    [ProducesResponseType(typeof(GetLeaderboardQueryResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<GetLeaderboardQueryResponse>> GetLeaderboard(
        [FromQuery] GetLeaderboardQuery query)
    {
        var response = await Sender.Send(query);
        return Ok(response);
    }

    [HttpGet("my-position")]
    [ProducesResponseType(typeof(GetMyPositionQueryResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<GetMyPositionQueryResponse>> GetMyPosition([FromQuery] string? pointTypeId)
    {
        var query = new GetMyPositionQuery { PointTypeId = pointTypeId };
        var response = await Sender.Send(query);
        return Ok(response);
    }

    [HttpPost("set-referrer")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SetReferrer([FromBody] SetReferrerCommand command)
    {
        await Sender.Send(command);
        return Ok();
    }

    [HttpPost("request-code")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> RequestCode()
    {
        var command = new RequestReferrerCodeCommand();
        await Sender.Send(command);
        return Ok();
    }

    [HttpPost("validate-code")]
    [ProducesResponseType(typeof(ValidateReferrerCodeCommandResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<ValidateReferrerCodeCommandResponse>> ValidateCode(
        [FromBody] ValidateReferrerCodeCommand command)
    {
        var response = await Sender.Send(command);
        return Ok(response);
    }
}

