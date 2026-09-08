using Hyper.CustomerPortal.Application.Features.Wheels.Commands;
using Hyper.CustomerPortal.Application.Features.Wheels.Queries;
using Microsoft.AspNetCore.Authorization;

namespace Hyper.CustomerPortal.Api.Controllers;

[AppRoute("Hyper", "customer/wheel")]
[Tags("customer/wheel")]
[ApiController]
[Authorize]
public class WheelController : AppControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(GetWheelExperienceQueryResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<GetWheelExperienceQueryResponse>> GetExperience()
    {
        var response = await Sender.Send(new GetWheelExperienceQuery());
        return Ok(response);
    }

    [HttpPost("spin")]
    [ProducesResponseType(typeof(SpinWheelCommandResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<SpinWheelCommandResponse>> Spin()
    {
        var response = await Sender.Send(new SpinWheelCommand());
        return Ok(response);
    }

    [HttpGet("history")]
    [ProducesResponseType(typeof(GetWheelHistoryQueryResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<GetWheelHistoryQueryResponse>> GetHistory([FromQuery] GetWheelHistoryQuery query)
    {
        var response = await Sender.Send(query);
        return Ok(response);
    }
}





