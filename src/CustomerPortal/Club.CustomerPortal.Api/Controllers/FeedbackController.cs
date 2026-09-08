using Hyper.CustomerPortal.Application.Features.Feedback.Commands;
using Hyper.CustomerPortal.Application.Features.Feedback.Queries;
using Microsoft.AspNetCore.Authorization;

namespace Hyper.CustomerPortal.Api.Controllers;

[AppRoute("Hyper", "customer/feedback")]
[Tags("customer/feedback")]
[ApiController]
[Authorize]
public class FeedbackController : AppControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(GetFeedbacksQueryResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<GetFeedbacksQueryResponse>> GetFeedbacks([FromQuery] GetFeedbacksQuery query)
    {
        var response = await Sender.Send(query);
        return Ok(response);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(GetFeedbackByIdQueryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GetFeedbackByIdQueryResponse>> GetFeedbackById(int id)
    {
        var query = new GetFeedbackByIdQuery { Id = id };
        var response = await Sender.Send(query);
        
        if (response.Feedback == null)
        {
            return NotFound(new { message = "بازخورد یافت نشد" });
        }
        
        return Ok(response);
    }

    [HttpPost]
    [ProducesResponseType(typeof(CreateFeedbackCommandResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CreateFeedbackCommandResponse>> CreateFeedback([FromBody] CreateFeedbackCommand command)
    {
        var response = await Sender.Send(command);
        
        if (!response.Success)
        {
            return BadRequest(new { message = response.Message });
        }
        
        return Ok(response);
    }
}

