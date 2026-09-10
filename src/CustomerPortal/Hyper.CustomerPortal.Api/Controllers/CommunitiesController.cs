using Hyper.CustomerPortal.Application.Features.Communities.Commands;
using Hyper.CustomerPortal.Application.Features.Communities.Queries;
using Microsoft.AspNetCore.Authorization;

namespace Hyper.CustomerPortal.Api.Controllers;

/// <summary>
/// کنترلر مدیریت جامعه‌های مشتریان
/// </summary>
[AppRoute("Hyper", "customer/communities")]
[Tags("customer/communities")]
[ApiController]
[Authorize]
public class CommunitiesController : AppControllerBase
{
    /// <summary>
    /// دریافت لیست جامعه‌های قابل نمایش و عضویت
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(GetAvailableCommunitiesQueryResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<GetAvailableCommunitiesQueryResponse>> GetAvailableCommunities(
        [FromQuery] GetAvailableCommunitiesQuery query)
    {
        var response = await Sender.Send(query);
        return Ok(response);
    }

    /// <summary>
    /// دریافت جزئیات یک جامعه
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(GetCommunityByIdQueryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GetCommunityByIdQueryResponse>> GetById(int id)
    {
        var query = new GetCommunityByIdQuery { CommunityId = id };
        var response = await Sender.Send(query);
        
        if (response.Community == null)
        {
            return NotFound(new { message = "جامعه مورد نظر یافت نشد" });
        }
        
        return Ok(response);
    }

    /// <summary>
    /// دریافت جامعه‌های من (جامعه‌هایی که عضوشان هستم)
    /// </summary>
    [HttpGet("my-communities")]
    [ProducesResponseType(typeof(GetMyCommunitiesQueryResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<GetMyCommunitiesQueryResponse>> GetMyCommunities()
    {
        var query = new GetMyCommunitiesQuery();
        var response = await Sender.Send(query);
        return Ok(response);
    }

    /// <summary>
    /// درخواست عضویت در یک جامعه
    /// </summary>
    [HttpPost("{id}/join")]
    [ProducesResponseType(typeof(JoinCommunityCommandResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<JoinCommunityCommandResponse>> JoinCommunity(int id)
    {
        var command = new JoinCommunityCommand { CommunityId = id };
        var response = await Sender.Send(command);
        
        if (!response.Success)
        {
            return BadRequest(new { message = response.Message });
        }
        
        return Ok(response);
    }
}
