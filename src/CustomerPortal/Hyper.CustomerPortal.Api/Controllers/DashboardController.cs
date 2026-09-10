using Hyper.CustomerPortal.Application.Features.Dashboard.Queries;
using Microsoft.AspNetCore.Authorization;

namespace Hyper.CustomerPortal.Api.Controllers;

[AppRoute("Hyper", "customer/dashboard")]
[Tags("customer/dashboard")]
[ApiController]
[Authorize]
public class DashboardController : AppControllerBase
{
    [HttpGet("stats")]
    [ProducesResponseType(typeof(GetDashboardStatsQueryResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<GetDashboardStatsQueryResponse>> GetStats()
    {
        var query = new GetDashboardStatsQuery();
        var response = await Sender.Send(query);
        return Ok(response);
    }
}

