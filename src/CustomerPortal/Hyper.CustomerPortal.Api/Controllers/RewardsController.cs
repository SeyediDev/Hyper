using Hyper.CustomerPortal.Application.Features.Rewards.Commands;
using Hyper.CustomerPortal.Application.Features.Rewards.Queries;
using Microsoft.AspNetCore.Authorization;

namespace Hyper.CustomerPortal.Api.Controllers;

[AppRoute("Hyper", "customer/rewards")]
[Tags("customer/rewards")]
[ApiController]
[Authorize]
public class RewardsController : AppControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(GetRewardsQueryResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<GetRewardsQueryResponse>> GetRewards([FromQuery] GetRewardsQuery query)
    {
        var response = await Sender.Send(query);
        return Ok(response);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(GetRewardByIdQueryResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<GetRewardByIdQueryResponse>> GetRewardById(string id)
    {
        var query = new GetRewardByIdQuery { Id = id };
        var response = await Sender.Send(query);
        return Ok(response);
    }

    [HttpGet("categories")]
    [ProducesResponseType(typeof(GetRewardCategoriesQueryResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<GetRewardCategoriesQueryResponse>> GetCategories()
    {
        var query = new GetRewardCategoriesQuery();
        var response = await Sender.Send(query);
        return Ok(response);
    }

    [HttpPost("purchase")]
    [ProducesResponseType(typeof(PurchaseRewardCommandResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PurchaseRewardCommandResponse>> Purchase([FromBody] PurchaseRewardCommand command)
    {
        var response = await Sender.Send(command);
        return Ok(response);
    }

    [HttpGet("purchased")]
    [ProducesResponseType(typeof(GetPurchasedRewardsQueryResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<GetPurchasedRewardsQueryResponse>> GetPurchasedRewards(
        [FromQuery] GetPurchasedRewardsQuery query)
    {
        var response = await Sender.Send(query);
        return Ok(response);
    }

    [HttpGet("purchased/{id}")]
    [ProducesResponseType(typeof(GetPurchasedRewardByIdQueryResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<GetPurchasedRewardByIdQueryResponse>> GetPurchasedById(string id)
    {
        var query = new GetPurchasedRewardByIdQuery { Id = id };
        var response = await Sender.Send(query);
        return Ok(response);
    }
}

