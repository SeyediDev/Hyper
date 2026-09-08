namespace Hyper.CustomerPortal.Application.Features.Points.Commands;

public record MarkPointsViewedCommand : IRequest
{
    public required string PointTypeId { get; set; }
}

public class MarkPointsViewedCommandHandler(
    IRequesterUser requesterUser,
    ILogger<MarkPointsViewedCommandHandler> logger) : IRequestHandler<MarkPointsViewedCommand>
{
    public Task Handle(MarkPointsViewedCommand request, CancellationToken cancellationToken)
    {
        var customerId = requesterUser.GetUserId();
        
        // TODO: علامت‌گذاری امتیازات به عنوان "مشاهده شده" در database
        // این ممکن است برای نوتیفیکیشن یا badge count استفاده شود
        
        logger.LogInformation("Points marked as viewed for customer {CustomerId}, PointType {PointTypeId}",
            customerId, request.PointTypeId);
        
        return Task.CompletedTask;
    }
}

