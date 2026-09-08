using Hyper.CustomerPortal.Application.Features.Plans.Commands;
using Hyper.CustomerPortal.Application.Features.Plans.Queries;
using Microsoft.AspNetCore.Authorization;

namespace Hyper.CustomerPortal.Api.Controllers;

[AppRoute("Hyper", "plans")]
[Authorize]
public class PlansController : AppControllerBase
{

    /// <summary>
    /// دریافت لیست طرح‌های اشتراک
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(GetPlansQueryResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPlans([FromQuery] GetPlansQuery query, CancellationToken cancellationToken)
    {
        var result = await Sender.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// دریافت جزئیات یک طرح اشتراک
    /// </summary>
    [HttpGet("{planId}")]
    [ProducesResponseType(typeof(GetPlanByIdQueryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPlanById(string planId, CancellationToken cancellationToken)
    {
        var query = new GetPlanByIdQuery { PlanId = planId };
        var result = await Sender.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// دریافت طرح فعال من
    /// </summary>
    [HttpGet("my-active-plan")]
    [ProducesResponseType(typeof(GetMyActivePlanQueryResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyActivePlan(CancellationToken cancellationToken)
    {
        var query = new GetMyActivePlanQuery();
        var result = await Sender.Send(query, cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// خرید طرح اشتراک
    /// </summary>
    [HttpPost("purchase")]
    [ProducesResponseType(typeof(PurchasePlanCommandResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> PurchasePlan([FromBody] PurchasePlanCommand command, CancellationToken cancellationToken)
    {
        var result = await Sender.Send(command, cancellationToken);
        return Ok(result);
    }
}

