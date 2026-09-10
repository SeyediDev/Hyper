using Hyper.CustomerPortal.Application.Interfaces;
using Hyper.Domain.Entities.Feedback;
using Microsoft.EntityFrameworkCore;

namespace Hyper.CustomerPortal.Application.Features.Feedback.Queries;

public record GetFeedbacksQuery : IRequest<GetFeedbacksQueryResponse>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public FeedbackType? FeedbackType { get; set; }
    public FeedbackStatus? Status { get; set; }
}

public record GetFeedbacksQueryResponse
{
    public PaginatedList<FeedbackDto> Feedbacks { get; set; } = null!;
}

public record FeedbackDto
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string Content { get; set; } = null!;
    public FeedbackType FeedbackType { get; set; }
    public string FeedbackTypeName { get; set; } = null!;
    public FeedbackStatus Status { get; set; }
    public string StatusName { get; set; } = null!;
    public int? SatisfactionScore { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ResponseDate { get; set; }
    public string? Response { get; set; }
    public string? ProductName { get; set; }
    public int LikesCount { get; set; }
    public int CommentsCount { get; set; }
}

public class GetFeedbacksQueryHandler(
    ICustomerRequesterUser requesterUser,
    IQueryRepository<CustomerFeedback> feedbackRepository,
    IQueryRepository<CustomerTenant> customerTenantRepository) : IRequestHandler<GetFeedbacksQuery, GetFeedbacksQueryResponse>
{
    public async Task<GetFeedbacksQueryResponse> Handle(GetFeedbacksQuery request, CancellationToken cancellationToken)
    {
        var customerId = requesterUser.CustomerId;
        var tenantId = requesterUser.TenantId;

        var customerTenant = await customerTenantRepository
            .FirstOrDefaultAsync(ct => ct.CustomerId == customerId && ct.TenantId == tenantId && !ct.IsDeleted, cancellationToken);

        if (customerTenant == null)
        {
            return new GetFeedbacksQueryResponse
            {
                Feedbacks = new PaginatedList<FeedbackDto>([], 0, request.PageNumber, request.PageSize)
            };
        }

        var query = feedbackRepository.Query()
            .Where(f => f.CustomerTenantId == customerTenant.Id && !f.IsDeleted);

        if (request.FeedbackType.HasValue)
        {
            query = query.Where(f => f.FeedbackType == request.FeedbackType.Value);
        }

        if (request.Status.HasValue)
        {
            query = query.Where(f => f.Status == request.Status.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var feedbacks = await query
            .Include(f => f.Product)
            .OrderByDescending(f => f.CreateDate)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        var feedbackDtos = feedbacks.Select(f => new FeedbackDto
        {
            Id = f.Id,
            Title = f.Title,
            Content = f.Content,
            FeedbackType = f.FeedbackType,
            FeedbackTypeName = GetFeedbackTypeName(f.FeedbackType),
            Status = f.Status,
            StatusName = GetStatusName(f.Status),
            SatisfactionScore = f.SatisfactionScore,
            CreatedAt = f.CreateDate,
            ResponseDate = f.ResponseDate,
            Response = f.Response,
            ProductName = f.Product?.Title,
            LikesCount = f.LikesCount,
            CommentsCount = f.CommentsCount
        }).ToList();

        return new GetFeedbacksQueryResponse
        {
            Feedbacks = new PaginatedList<FeedbackDto>(feedbackDtos, totalCount, request.PageNumber, request.PageSize)
        };
    }

    private static string GetFeedbackTypeName(FeedbackType type)
    {
        return type switch
        {
            FeedbackType.Suggestion => "پیشنهاد",
            FeedbackType.Criticism => "انتقاد",
            FeedbackType.Complaint => "شکایت",
            FeedbackType.Appreciation => "تشکر",
            FeedbackType.Question => "سوال",
            FeedbackType.FeatureRequest => "درخواست ویژگی",
            _ => type.ToString()
        };
    }

    private static string GetStatusName(FeedbackStatus status)
    {
        return status switch
        {
            FeedbackStatus.New => "جدید",
            FeedbackStatus.UnderReview => "در حال بررسی",
            FeedbackStatus.InProgress => "در حال اقدام",
            FeedbackStatus.Resolved => "حل شده",
            FeedbackStatus.Rejected => "رد شده",
            FeedbackStatus.Closed => "بسته شده",
            _ => status.ToString()
        };
    }
}

