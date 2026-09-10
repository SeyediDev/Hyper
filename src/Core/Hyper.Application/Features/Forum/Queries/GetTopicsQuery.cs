namespace Hyper.Application.Features.Forum.Queries;

/// <summary>
/// دریافت موضوعات انجمن
/// </summary>
public record GetTopicsQuery : IRequest<List<ForumTopicDto>>
{
    [Required]
    public int TenantId { get; set; }
    
    public string? Category { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

public record ForumTopicDto
{
    public int Id { get; set; }
    public int CustomerTenantId { get; set; }
    public string CustomerName { get; set; } = null!;
    public string Title { get; set; } = null!;
    public string? Category { get; set; }
    public bool IsPinned { get; set; }
    public bool IsLocked { get; set; }
    public int ViewsCount { get; set; }
    public int PostsCount { get; set; }
    public int LikesCount { get; set; }
    public DateTime CreatedOnUtc { get; set; }
}

public class GetTopicsQueryHandler(
    IHyperUnitOfWorkQuery unitOfWork
) : IRequestHandler<GetTopicsQuery, List<ForumTopicDto>>
{
    public async Task<List<ForumTopicDto>> Handle(GetTopicsQuery request, CancellationToken cancellationToken)
    {
        var topicRepo = unitOfWork.Repository<ForumTopic, int>();
        
        var topics = await topicRepo.GetAllWithIncludeAsync(
            include: t => t.CreatorCustomerTenant.Customer!,
            cancellationToken: cancellationToken,
            predicate: t => t.TenantId == request.TenantId &&
                (string.IsNullOrEmpty(request.Category) || t.Category == request.Category),
            orderBy: q => q.OrderByDescending(t => t.IsPinned).ThenByDescending(t => t.Id),
            skip: (request.PageNumber - 1) * request.PageSize,
            take: request.PageSize
        );

        var result = topics.Select(t => new ForumTopicDto
        {
            Id = t.Id,
            CustomerTenantId = t.CreatorCustomerTenantId,
            CustomerName = $"{t.CreatorCustomerTenant?.Customer?.FirstName} {t.CreatorCustomerTenant?.Customer?.LastName}",
            Title = t.Title,
            Category = t.Category,
            IsPinned = t.IsPinned,
            IsLocked = t.IsLocked,
            ViewsCount = t.ViewsCount,
            PostsCount = t.PostsCount,
            LikesCount = t.LikesCount,
            CreatedOnUtc = DateTime.UtcNow // TODO: Map from base entity
        }).ToList() ?? [];

        return result;
    }
}

