using Hyper.CustomerPortal.Application.Features.Notifications.Commands;
using Hyper.CustomerPortal.Application.Features.Notifications.Queries;
using Microsoft.AspNetCore.Authorization;

namespace Hyper.CustomerPortal.Api.Controllers;

[AppRoute("Hyper", "customer/notifications")]
[Tags("customer/notifications")]
[ApiController]
[Authorize]
public class NotificationsController : AppControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(GetNotificationsQueryResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<GetNotificationsQueryResponse>> GetNotifications([FromQuery] GetNotificationsQuery query)
    {
        var response = await Sender.Send(query);
        return Ok(response);
    }

    [HttpPost("{id}/read")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> MarkAsRead(int id)
    {
        var command = new MarkNotificationAsReadCommand { NotificationId = id };
        await Sender.Send(command);
        return Ok();
    }

    [HttpPost("read-all")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> MarkAllAsRead()
    {
        var command = new MarkAllNotificationsAsReadCommand();
        await Sender.Send(command);
        return Ok();
    }
}




