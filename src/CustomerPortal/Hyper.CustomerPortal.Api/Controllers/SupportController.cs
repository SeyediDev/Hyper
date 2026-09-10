using Hyper.CustomerPortal.Application.Features.Support.Commands;
using Hyper.CustomerPortal.Application.Features.Support.Queries;
using Microsoft.AspNetCore.Authorization;

namespace Hyper.CustomerPortal.Api.Controllers;

[AppRoute("Hyper", "customer/support")]
[Tags("customer/support")]
[ApiController]
[Authorize]
public class SupportController : AppControllerBase
{
    [HttpGet("tickets")]
    [ProducesResponseType(typeof(GetSupportTicketsQueryResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<GetSupportTicketsQueryResponse>> GetTickets([FromQuery] GetSupportTicketsQuery query)
    {
        var response = await Sender.Send(query);
        return Ok(response);
    }

    [HttpGet("tickets/{id}")]
    [ProducesResponseType(typeof(GetSupportTicketByIdQueryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GetSupportTicketByIdQueryResponse>> GetTicketById(long id)
    {
        var query = new GetSupportTicketByIdQuery { Id = id };
        var response = await Sender.Send(query);
        
        if (response.Ticket == null)
        {
            return NotFound(new { message = "تیکت یافت نشد" });
        }
        
        return Ok(response);
    }

    [HttpPost("tickets")]
    [ProducesResponseType(typeof(CreateSupportTicketCommandResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CreateSupportTicketCommandResponse>> CreateTicket([FromBody] CreateSupportTicketCommand command)
    {
        var response = await Sender.Send(command);
        
        if (!response.Success)
        {
            return BadRequest(new { message = response.Message });
        }
        
        return Ok(response);
    }
}

