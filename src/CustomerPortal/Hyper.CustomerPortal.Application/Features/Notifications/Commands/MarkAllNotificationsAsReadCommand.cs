using Hyper.Domain.Entities.Common;
using Microsoft.EntityFrameworkCore;

namespace Hyper.CustomerPortal.Application.Features.Notifications.Commands;

public record MarkAllNotificationsAsReadCommand : IRequest;

public class MarkAllNotificationsAsReadCommandHandler(
    IRequesterUser requesterUser,
        ICommandRepository<Notification> notificationRepository) : IRequestHandler<MarkAllNotificationsAsReadCommand>
{
    public async Task Handle(MarkAllNotificationsAsReadCommand request, CancellationToken cancellationToken)
    {
        var userId = requesterUser.GetUserId();

        var notifications = await notificationRepository.Query()
            .Where(n => n.UserId == userId && !n.IsDeleted && !n.IsRead)
            .ToListAsync(cancellationToken);

        foreach (var notification in notifications)
        {
            notification.IsRead = true;
        }

        await notificationRepository.SaveChangesAsync(cancellationToken);
    }
}

