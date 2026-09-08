using System.Linq.Expressions;

namespace Hyper.Application.Features.Common.Queries;

public record NotificationQuery : PaginationQuery, IRequest<NotificationQueryResponse>;

public record NotificationQueryResponse : PaginationResponse<NotificationItem>
{
}

public record NotificationItem(int Id, string Title, string? Content, DateTime Date, bool IsRead)
{
}

public class NotificationQueryHandler(IRequesterUser user, IQueryRepository<Domain.Entities.Common.Notification, int> queryRepository) : IRequestHandler<NotificationQuery, NotificationQueryResponse>
{
    public async Task<NotificationQueryResponse> Handle(NotificationQuery request, CancellationToken cancellationToken)
    {
        Expression<Func<Domain.Entities.Common.Notification, bool>> filter = x => x.UserId == user.Id!.Value
                                                                            && !x.IsDeleted;

        List<Expression<Func<Domain.Entities.Common.Notification, object>>> includes = [];

        (IEnumerable<Domain.Entities.Common.Notification> Entities, bool HasNext) = await queryRepository.GetPagedWithIncludeAsync
            (includes, request.PageNumber, request.PageSize, cancellationToken, filter);

        NotificationQueryResponse response = new()
        {
            Items = Entities.Select(x => new NotificationItem(x.Id, x.Title, x.Description, x.Date, x.IsRead)).ToList(),
            HasNext = HasNext
        };
        return response;
    }
}

