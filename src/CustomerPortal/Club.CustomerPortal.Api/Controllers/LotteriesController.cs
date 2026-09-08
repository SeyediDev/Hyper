using Hyper.CustomerPortal.Application.Features.Lotteries.Commands;
using Hyper.CustomerPortal.Application.Features.Lotteries.Queries;
using Microsoft.AspNetCore.Authorization;
using GetCustomerLotteryHistoryQuery = Hyper.CustomerPortal.Application.Features.Lotteries.Queries.GetCustomerLotteryHistoryQuery;
using GetCustomerLotteryHistoryQueryResponse = Hyper.CustomerPortal.Application.Features.Lotteries.Queries.GetCustomerLotteryHistoryQueryResponse;
using GetCustomerLotteryPositionQuery = Hyper.CustomerPortal.Application.Features.Lotteries.Queries.GetCustomerLotteryPositionQuery;
using GetCustomerLotteryPositionQueryResponse = Hyper.CustomerPortal.Application.Features.Lotteries.Queries.GetCustomerLotteryPositionQueryResponse;

namespace Hyper.CustomerPortal.Api.Controllers;

[AppRoute("Hyper", "customer/lotteries")]
[Tags("customer/lotteries")]
[ApiController]
[Authorize]
public class LotteriesController : AppControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(GetLotteriesQueryResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<GetLotteriesQueryResponse>> GetLotteries(
        [FromQuery] GetLotteriesQuery query)
    {
        var response = await Sender.Send(query);
        return Ok(response);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(GetLotteryByIdQueryResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<GetLotteryByIdQueryResponse>> GetById(string id)
    {
        var query = new GetLotteryByIdQuery { Id = id };
        var response = await Sender.Send(query);
        return Ok(response);
    }

    [HttpPost("{id}/participate")]
    [ProducesResponseType(typeof(ParticipateInLotteryCommandResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ParticipateInLotteryCommandResponse>> Participate(
        string id,
        [FromBody] ParticipateInLotteryCommand command)
    {
        command.LotteryId = id;
        var response = await Sender.Send(command);
        return Ok(response);
    }

    [HttpGet("my-participations")]
    [ProducesResponseType(typeof(GetMyLotteryParticipationsQueryResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<GetMyLotteryParticipationsQueryResponse>> GetMyParticipations(
        [FromQuery] GetMyLotteryParticipationsQuery query)
    {
        var response = await Sender.Send(query);
        return Ok(response);
    }

    [HttpGet("{id}/results")]
    [ProducesResponseType(typeof(GetLotteryResultsQueryResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<GetLotteryResultsQueryResponse>> GetResults(string id)
    {
        var query = new GetLotteryResultsQuery { LotteryId = id };
        var response = await Sender.Send(query);
        return Ok(response);
    }

    [HttpGet("my-history")]
    [ProducesResponseType(typeof(GetCustomerLotteryHistoryQueryResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<GetCustomerLotteryHistoryQueryResponse>> GetMyHistory(
        [FromQuery] GetCustomerLotteryHistoryQuery query)
    {
        var response = await Sender.Send(query);
        return Ok(response);
    }

    [HttpGet("{id}/my-position")]
    [ProducesResponseType(typeof(GetCustomerLotteryPositionQueryResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<GetCustomerLotteryPositionQueryResponse>> GetMyPosition(string id)
    {
        var query = new GetCustomerLotteryPositionQuery { LotteryId = id };
        var response = await Sender.Send(query);
        return Ok(response);
    }
}

