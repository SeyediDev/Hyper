using Hyper.CustomerPortal.Application.Features.Promotions.Commands;
using Hyper.CustomerPortal.Application.Features.Promotions.Queries;
using Microsoft.AspNetCore.Authorization;

namespace Hyper.CustomerPortal.Api.Controllers;

[AppRoute("Hyper", "customer/promotions")]
[Tags("customer/promotions")]
[ApiController]
[Authorize]
public class PromotionsController : AppControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(GetPromotionsQueryResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<GetPromotionsQueryResponse>> GetPromotions(
        [FromQuery] GetPromotionsQuery query)
    {
        var response = await Sender.Send(query);
        return Ok(response);
    }

    /// <summary>
    /// لیست کمپین‌های فعال (فیلتر IsActive=true, IsVisibleToCustomer در سرویس اعمال می‌شود)
    /// </summary>
    [HttpGet("active")]
    [ProducesResponseType(typeof(GetPromotionsQueryResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<GetPromotionsQueryResponse>> GetActivePromotions(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20)
    {
        var query = new GetPromotionsQuery
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            IsActive = true
        };
        var response = await Sender.Send(query);
        return Ok(response);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(GetPromotionByIdQueryResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<GetPromotionByIdQueryResponse>> GetById(string id)
    {
        var query = new GetPromotionByIdQuery { Id = id };
        var response = await Sender.Send(query);
        return Ok(response);
    }

    [HttpPost("{id}/participate")]
    [ProducesResponseType(typeof(ParticipateInPromotionCommandResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ParticipateInPromotionCommandResponse>> Participate(string id)
    {
        var command = new ParticipateInPromotionCommand { PromotionId = id };
        var response = await Sender.Send(command);
        return Ok(response);
    }

    [HttpGet("my-participations")]
    [ProducesResponseType(typeof(GetMyParticipationsQueryResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<GetMyParticipationsQueryResponse>> GetMyParticipations(
        [FromQuery] GetMyParticipationsQuery query)
    {
        var response = await Sender.Send(query);
        return Ok(response);
    }
}

