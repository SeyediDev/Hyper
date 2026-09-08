using Hyper.CustomerPortal.Application.Interfaces;
using Hyper.Domain.Entities.Common;
using Microsoft.EntityFrameworkCore;

namespace Hyper.CustomerPortal.Application.Features.Notifications.Queries;

public record GetNotificationsQuery : IRequest<GetNotificationsQueryResponse>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public bool? IsRead { get; set; }
}

public record GetNotificationsQueryResponse
{
    public PaginatedList<NotificationDto> Notifications { get; set; } = null!;
    public int UnreadCount { get; set; }
}

public record NotificationDto
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public DateTime Date { get; set; }
    public bool IsRead { get; set; }
}

public class GetNotificationsQueryHandler(
    ICustomerRequesterUser requesterUser,
    IQueryRepository<Notification> notificationRepository)
    : IRequestHandler<GetNotificationsQuery, GetNotificationsQueryResponse>
{
    public async Task<GetNotificationsQueryResponse> Handle(GetNotificationsQuery request, CancellationToken cancellationToken)
    {
        var userId = requesterUser.Id;
        var tenantId = requesterUser.TenantId;

        var query = notificationRepository.Query()
            .Where(n => n.UserId == userId && !n.IsDeleted);

        if (request.IsRead.HasValue)
        {
            query = query.Where(n => n.IsRead == request.IsRead.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var notifications = await query
            .OrderByDescending(n => n.Date)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        var unreadCount = await notificationRepository.Query()
            .Where(n => n.UserId == userId && !n.IsDeleted && !n.IsRead)
            .CountAsync(cancellationToken);

        var notificationDtos = notifications.Select(n => new NotificationDto
        {
            Id = n.Id,
            Title = n.Title,
            Description = n.Description,
            Date = n.Date,
            IsRead = n.IsRead
        }).ToList();

        return new GetNotificationsQueryResponse
        {
            Notifications = new PaginatedList<NotificationDto>(notificationDtos, totalCount, request.PageNumber, request.PageSize),
            UnreadCount = unreadCount
        };
    }
}

