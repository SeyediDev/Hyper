using Hyper.CustomerPortal.Application.Interfaces;
using Hyper.Domain.Features.Customers;

namespace Hyper.CustomerPortal.Application.Features.Communities.Queries;

/// <summary>
/// دریافت جامعه‌هایی که مشتری عضو آن‌ها است
/// </summary>
public record GetMyCommunitiesQuery : IRequest<GetMyCommunitiesQueryResponse>
{
}

public record GetMyCommunitiesQueryResponse
{
    public List<MyCommunityDto> Communities { get; set; } = null!;
}

public record MyCommunityDto
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public int MemberCount { get; set; }
    public string? Benefits { get; set; }
    public DateTime JoinedDate { get; set; }
}

public class GetMyCommunitiesQueryHandler(
    ICustomerSegmentService customerSegmentService,
    ICustomerRequesterUser requesterUser
) : IRequestHandler<GetMyCommunitiesQuery, GetMyCommunitiesQueryResponse>
{
    public async Task<GetMyCommunitiesQueryResponse> Handle(
        GetMyCommunitiesQuery request, 
        CancellationToken cancellationToken)
    {
        var customerId = requesterUser.CustomerId;

        // دریافت همه جامعه‌ها (شامل قابل نمایش و غیر قابل نمایش)
        var segments = await customerSegmentService.GetEligibleSegmentsForCustomerAsync(
            1/*TODO*/,
            customerId, 
            onlyVisibleInPortal: false, 
            cancellationToken);

        // فیلتر کردن فقط جامعه‌هایی که عضوشان هستیم
        var myCommunities = segments
            .Where(s => s.IsMember)
            .Select(s => new MyCommunityDto
            {
                Id = s.SegmentId.ToString(),
                Name = s.Title,
                Description = s.Description,
                ImageUrl = s.ImageUrl,
                MemberCount = s.MemberCount,
                Benefits = s.Benefits,
                JoinedDate = DateTime.UtcNow // TODO: باید از Membership.CreatedDate استفاده کنیم
            })
            .ToList();

        return new GetMyCommunitiesQueryResponse
        {
            Communities = myCommunities
        };
    }
}
