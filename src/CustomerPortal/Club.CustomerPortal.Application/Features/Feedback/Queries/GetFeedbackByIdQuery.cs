using Hyper.CustomerPortal.Application.Interfaces;
using Hyper.Domain.Entities.Feedback;
using Microsoft.EntityFrameworkCore;

namespace Hyper.CustomerPortal.Application.Features.Feedback.Queries;

public record GetFeedbackByIdQuery : IRequest<GetFeedbackByIdQueryResponse>
{
    public int Id { get; set; }
}

public record GetFeedbackByIdQueryResponse
{
    public FeedbackDetailDto? Feedback { get; set; }
}

public record FeedbackDetailDto
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string Content { get; set; } = null!;
    public FeedbackType FeedbackType { get; set; }
    public string FeedbackTypeName { get; set; } = null!;
    public FeedbackStatus Status { get; set; }
    public string StatusName { get; set; } = null!;
    public FeedbackPriority Priority { get; set; }
    public string PriorityName { get; set; } = null!;
    public int? SatisfactionScore { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? ResponseDate { get; set; }
    public string? Response { get; set; }
    public string? ProductName { get; set; }
    public string? Category { get; set; }
    public string? Tags { get; set; }
    public int LikesCount { get; set; }
    public int CommentsCount { get; set; }
    public List<FeedbackCommentDto> Comments { get; set; } = [];
}

public record FeedbackCommentDto
{
    public int Id { get; set; }
    public string Content { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public string? AuthorName { get; set; }
}

public class GetFeedbackByIdQueryHandler(
    ICustomerRequesterUser requesterUser,
    IQueryRepository<CustomerFeedback> feedbackRepository,
    IQueryRepository<CustomerTenant> customerTenantRepository) : IRequestHandler<GetFeedbackByIdQuery, GetFeedbackByIdQueryResponse>
{
    public async Task<GetFeedbackByIdQueryResponse> Handle(GetFeedbackByIdQuery request, CancellationToken cancellationToken)
    {
        var customerId = requesterUser.CustomerId;
        var tenantId = requesterUser.TenantId??0;

        var customerTenant = await customerTenantRepository
            .FirstOrDefaultAsync(ct => ct.CustomerId == customerId && ct.TenantId == tenantId && !ct.IsDeleted, cancellationToken);

        if (customerTenant == null)
        {
            return new GetFeedbackByIdQueryResponse { Feedback = null };
        }

		CustomerFeedback? feedback = await feedbackRepository.Query()
            .Include(f => f.Product)
            .Include(f => f.Comments)
            .FirstOrDefaultAsync( f => f.Id == request.Id && f.CustomerTenantId == customerTenant.Id && !f.IsDeleted, cancellationToken);

        if (feedback == null)
        {
            return new GetFeedbackByIdQueryResponse { Feedback = null };
        }

        var feedbackDto = new FeedbackDetailDto
        {
            Id = feedback.Id,
            Title = feedback.Title,
            Content = feedback.Content,
            FeedbackType = feedback.FeedbackType,
            FeedbackTypeName = GetFeedbackTypeName(feedback.FeedbackType),
            Status = feedback.Status,
            StatusName = GetStatusName(feedback.Status),
            Priority = feedback.Priority,
            PriorityName = GetPriorityName(feedback.Priority),
            SatisfactionScore = feedback.SatisfactionScore,
            CreatedAt = feedback.CreateDate,
            ResponseDate = feedback.ResponseDate,
            Response = feedback.Response,
            ProductName = feedback.Product?.Title,
            Category = feedback.Category,
            Tags = feedback.Tags,
            LikesCount = feedback.LikesCount,
            CommentsCount = feedback.CommentsCount,
            Comments = feedback.Comments.Select(c => new FeedbackCommentDto
            {
                Id = c.Id,
                Content = c.Content,
                CreatedAt = c.CreateDate,
                AuthorName = null // می‌توانید از User entity استفاده کنید
            }).ToList()
        };

        return new GetFeedbackByIdQueryResponse { Feedback = feedbackDto };
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

    private static string GetPriorityName(FeedbackPriority priority)
    {
        return priority switch
        {
            FeedbackPriority.Low => "پایین",
            FeedbackPriority.Medium => "متوسط",
            FeedbackPriority.High => "بالا",
            FeedbackPriority.Critical => "بحرانی",
            _ => priority.ToString()
        };
    }
}

