using MassTransit.Initializers;
using System.Linq.Expressions;

namespace Hyper.Application.Features.Common.Queries;

public record NotificationCountQuery : IRequest<NotificationCountResponse>;

public record NotificationCountResponse(int Count)
{
}

public class NotificationCountQueryHandler(IRequesterUser user, IQueryRepository<Domain.Entities.Common.Notification, int> queryRepository)
    : IRequestHandler<NotificationCountQuery, NotificationCountResponse>
{
    public async Task<NotificationCountResponse> Handle(NotificationCountQuery request, CancellationToken cancellationToken)
    {
        Expression<Func<Domain.Entities.Common.Notification, bool>> filter = x => x.UserId == user.Id!.Value
                                                                                && !x.IsDeleted
                                                                                && x.IsRead == false;

        var result = await queryRepository.GetAllAsync(cancellationToken, filter)
             .Select(g => new
             {
                 Count = g.Count(),
             });
        return new NotificationCountResponse(result.Count);
    }
}