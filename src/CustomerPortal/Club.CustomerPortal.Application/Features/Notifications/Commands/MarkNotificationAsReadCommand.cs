using Hyper.Domain.Entities.Common;

namespace Hyper.CustomerPortal.Application.Features.Notifications.Commands;

public record MarkNotificationAsReadCommand : IRequest
{
    public int NotificationId { get; set; }
}

public class MarkNotificationAsReadCommandHandler(
    IRequesterUser requesterUser,
    ICommandRepository<Notification> notificationRepository) : IRequestHandler<MarkNotificationAsReadCommand>
{
    public async Task Handle(MarkNotificationAsReadCommand request, CancellationToken cancellationToken)
    {
        var userId = requesterUser.GetUserId();

        var notification = await notificationRepository
            .FirstOrDefaultAsync(n => n.Id == request.NotificationId && n.UserId == userId && !n.IsDeleted, cancellationToken);

        if (notification != null)
        {
            notification.IsRead = true;
            await notificationRepository.SaveChangesAsync(cancellationToken);
        }
    }
}

