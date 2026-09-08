using Hyper.CustomerPortal.Application.Interfaces;
using Hyper.Domain.Entities.CustomerSegments.Enums;
using Hyper.Domain.Features.Customers;

namespace Hyper.CustomerPortal.Application.Features.Communities.Queries;

/// <summary>
/// دریافت جزئیات یک جامعه
/// </summary>
public record GetCommunityByIdQuery : IRequest<GetCommunityByIdQueryResponse>
{
    public int CommunityId { get; set; }
}

public record GetCommunityByIdQueryResponse
{
    public CommunityDetailDto? Community { get; set; }
}

public record CommunityDetailDto
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public int MemberCount { get; set; }
    public bool IsMember { get; set; }
    public string JoinMode { get; set; } = null!;
    public string? Benefits { get; set; }
    public bool CanJoin { get; set; }
    public bool IsEligible { get; set; }
}

public class GetCommunityByIdQueryHandler(
    ICustomerSegmentService customerSegmentService,
    ICustomerRequesterUser requesterUser
) : IRequestHandler<GetCommunityByIdQuery, GetCommunityByIdQueryResponse>
{
    public async Task<GetCommunityByIdQueryResponse> Handle(
        GetCommunityByIdQuery request, 
        CancellationToken cancellationToken)
    {
        var customerId = requesterUser.CustomerId;

        // دریافت همه جامعه‌ها
        var segments = await customerSegmentService.GetEligibleSegmentsForCustomerAsync(
            1/*TODO*/,
            customerId, 
            onlyVisibleInPortal: false,
            cancellationToken);

        var segment = segments.FirstOrDefault(s => s.SegmentId == request.CommunityId);
        if (segment == null)
        {
            return new GetCommunityByIdQueryResponse { Community = null };
        }

        // بررسی واجد شرایط بودن
        var eligibility = await customerSegmentService.CheckCustomerEligibilityAsync(
            customerId, 
            request.CommunityId, []/*TODO*/,
            cancellationToken);

        var communityDetail = new CommunityDetailDto
        {
            Id = segment.SegmentId.ToString(),
            Name = segment.Title,
            Description = segment.Description,
            ImageUrl = segment.ImageUrl,
            MemberCount = segment.MemberCount,
            IsMember = segment.IsMember,
            JoinMode = segment.JoinMode.ToString(),
            Benefits = segment.Benefits,
            CanJoin = !segment.IsMember && segment.JoinMode != CustomerSegmentJoinMode.SystemOnly,
            IsEligible = eligibility.IsEligible
        };

        return new GetCommunityByIdQueryResponse
        {
            Community = communityDetail
        };
    }
}
