using System.Linq.Expressions;

namespace Hyper.Application.Features.Feedback.Queries;

/// <summary>
/// دریافت لیست بازخوردها
/// </summary>
public record GetFeedbacksQuery : IRequest<List<FeedbackDto>>
{
    [Required]
    public int TenantId { get; set; }
    
    public FeedbackType? FeedbackType { get; set; }
    public FeedbackStatus? Status { get; set; }
    public int? CustomerTenantId { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

public record FeedbackDto
{
    public int Id { get; set; }
    public int CustomerTenantId { get; set; }
    public string CustomerName { get; set; } = null!;
    public FeedbackType FeedbackType { get; set; }
    public string Title { get; set; } = null!;
    public string Content { get; set; } = null!;
    public FeedbackStatus Status { get; set; }
    public FeedbackPriority Priority { get; set; }
    public int LikesCount { get; set; }
    public int CommentsCount { get; set; }
    public DateTime CreatedOnUtc { get; set; }
}

public class GetFeedbacksQueryHandler(
    IHyperUnitOfWorkQuery unitOfWork
) : IRequestHandler<GetFeedbacksQuery, List<FeedbackDto>>
{
    public async Task<List<FeedbackDto>> Handle(GetFeedbacksQuery request, CancellationToken cancellationToken)
    {
        var feedbackRepo = unitOfWork.Repository<CustomerFeedback, int>();
        
        List<Expression<Func<CustomerFeedback, object?>>> includes =
        [
            f => f.CustomerTenant.Customer!,
            f => f.Product!
        ];
        var feedbacks = await feedbackRepo.GetAllWithIncludeAsync(
            includes: includes,
            cancellationToken: cancellationToken,
            predicate: f => f.TenantId == request.TenantId &&
                (!request.FeedbackType.HasValue || f.FeedbackType == request.FeedbackType.Value) &&
                (!request.Status.HasValue || f.Status == request.Status.Value) &&
                (!request.CustomerTenantId.HasValue || f.CustomerTenantId == request.CustomerTenantId.Value),
            orderBy: q => q.OrderByDescending(f => f.CreateDate),
            skip: (request.PageNumber - 1) * request.PageSize,
            take: request.PageSize
        );

        var result = feedbacks.Select(f => new FeedbackDto
        {
            Id = f.Id,
            CustomerTenantId = f.CustomerTenantId,
            CustomerName = $"{f.CustomerTenant?.Customer?.FirstName} {f.CustomerTenant?.Customer?.LastName}",
            FeedbackType = f.FeedbackType,
            Title = f.Title,
            Content = f.Content,
            Status = f.Status,
            Priority = f.Priority,
            LikesCount = f.LikesCount,
            CommentsCount = f.CommentsCount,
            CreatedOnUtc = f.CreateDate
        }).ToList();

        return result ?? [];
    }
}

