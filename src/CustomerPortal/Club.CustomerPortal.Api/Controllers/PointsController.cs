using Hyper.CustomerPortal.Application.Features.Points.Commands;
using Hyper.CustomerPortal.Application.Features.Points.Queries;
using Microsoft.AspNetCore.Authorization;

namespace Hyper.CustomerPortal.Api.Controllers;

[AppRoute("Hyper", "customer/points")]
[Tags("customer/points")]
[ApiController]
[Authorize]
public class PointsController : AppControllerBase
{
    [HttpGet("summary")]
    [ProducesResponseType(typeof(GetPointsSummaryQueryResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<GetPointsSummaryQueryResponse>> GetSummary()
    {
        var query = new GetPointsSummaryQuery();
        var response = await Sender.Send(query);
        return Ok(response);
    }

    [HttpGet("types")]
    [ProducesResponseType(typeof(GetPointTypesQueryResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<GetPointTypesQueryResponse>> GetTypes()
    {
        var query = new GetPointTypesQuery();
        var response = await Sender.Send(query);
        return Ok(response);
    }

    [HttpGet("transactions")]
    [ProducesResponseType(typeof(GetPointTransactionsQueryResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<GetPointTransactionsQueryResponse>> GetTransactions(
        [FromQuery] GetPointTransactionsQuery query)
    {
        var response = await Sender.Send(query);
        return Ok(response);
    }

    [HttpGet("referral-points")]
    [ProducesResponseType(typeof(GetReferralPointsQueryResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<GetReferralPointsQueryResponse>> GetReferralPoints()
    {
        var query = new GetReferralPointsQuery();
        var response = await Sender.Send(query);
        return Ok(response);
    }

    [HttpPost("convert")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ConvertPoints([FromBody] ConvertPointsCommand command)
    {
        await Sender.Send(command);
        return Ok();
    }

    [HttpPost("transfer")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> TransferPoints([FromBody] TransferPointsCommand command)
    {
        await Sender.Send(command);
        return Ok();
    }

    [HttpGet("conversion-rates/{fromPointTypeId}")]
    [ProducesResponseType(typeof(GetConversionRatesQueryResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<GetConversionRatesQueryResponse>> GetConversionRates(string fromPointTypeId)
    {
        var query = new GetConversionRatesQuery { FromPointTypeId = fromPointTypeId };
        var response = await Sender.Send(query);
        return Ok(response);
    }

    [HttpPost("mark-viewed")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> MarkViewed([FromBody] MarkPointsViewedCommand command)
    {
        await Sender.Send(command);
        return Ok();
    }

    [HttpGet("expiring")]
    [ProducesResponseType(typeof(GetExpiringPointsQueryResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<GetExpiringPointsQueryResponse>> GetExpiringPoints()
    {
        var query = new GetExpiringPointsQuery();
        var response = await Sender.Send(query);
        return Ok(response);
    }
}

