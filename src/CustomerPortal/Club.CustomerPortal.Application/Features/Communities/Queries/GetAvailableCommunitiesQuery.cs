using Hyper.CustomerPortal.Application.Interfaces;
using Hyper.Domain.Entities.CustomerSegments.Enums;
using Hyper.Domain.Features.Customers;

namespace Hyper.CustomerPortal.Application.Features.Communities.Queries;

/// <summary>
/// دریافت جامعه‌های قابل نمایش و عضویت در پرتال
/// </summary>
public record GetAvailableCommunitiesQuery : IRequest<GetAvailableCommunitiesQueryResponse>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

public record GetAvailableCommunitiesQueryResponse
{
    public PaginatedList<CommunityDto> Communities { get; set; } = null!;
}

public record CommunityDto
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
}

public class GetAvailableCommunitiesQueryHandler(
    ICustomerSegmentService customerSegmentService,
    ICustomerRequesterUser requesterUser
) : IRequestHandler<GetAvailableCommunitiesQuery, GetAvailableCommunitiesQueryResponse>
{
    public async Task<GetAvailableCommunitiesQueryResponse> Handle(
        GetAvailableCommunitiesQuery request, 
        CancellationToken cancellationToken)
    {
        var customerId = requesterUser.CustomerId;

        // دریافت جامعه‌های قابل نمایش
        var segments = await customerSegmentService.GetEligibleSegmentsForCustomerAsync(
            1/*TODO*/,
            customerId, 
            true,
            cancellationToken);

        // تبدیل به DTO
        var communities = segments.Select(s => new CommunityDto
        {
            Id = s.SegmentId.ToString(),
            Name = s.Title,
            Description = s.Description,
            ImageUrl = s.ImageUrl,
            MemberCount = s.MemberCount,
            IsMember = s.IsMember,
            JoinMode = s.JoinMode.ToString(),
            Benefits = s.Benefits!,
            CanJoin = !s.IsMember && s.JoinMode != CustomerSegmentJoinMode.SystemOnly
        }).ToList();

        // صفحه‌بندی
        var pagedCommunities = communities
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        return new GetAvailableCommunitiesQueryResponse
        {
            Communities = new PaginatedList<CommunityDto>(
                pagedCommunities, 
                communities.Count, 
                request.PageNumber, 
                request.PageSize)
        };
    }
}
