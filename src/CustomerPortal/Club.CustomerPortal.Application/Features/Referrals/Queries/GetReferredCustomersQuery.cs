using Hyper.CustomerPortal.Application.Interfaces;

namespace Hyper.CustomerPortal.Application.Features.Referrals.Queries;

public record GetReferredCustomersQuery : IRequest<GetReferredCustomersQueryResponse>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? Status { get; set; }
}

public record GetReferredCustomersQueryResponse
{
    public PaginatedList<ReferredCustomerDto> Customers { get; set; } = null!;
}

public record ReferredCustomerDto
{
    public string Id { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public DateTime JoinDate { get; set; }
    public bool IsActive { get; set; }
    public int PointsEarned { get; set; }
    public string Status { get; set; } = null!;
}

public class GetReferredCustomersQueryHandler(
    IReferralService referralService,
    ICustomerRequesterUser requesterUser)
    : IRequestHandler<GetReferredCustomersQuery, GetReferredCustomersQueryResponse>
{
    public async Task<GetReferredCustomersQueryResponse> Handle(GetReferredCustomersQuery request, CancellationToken cancellationToken)
    {
        var customerId = requesterUser.CustomerId;
        var result = await referralService.GetReferredCustomersAsync(customerId, request.PageNumber, request.PageSize, cancellationToken);
        
        var customers = result.Items.Select(c => new ReferredCustomerDto
        {
            Id = "1",
            FirstName = c.Name.Split(' ').FirstOrDefault() ?? c.Name,
            LastName = c.Name.Split(' ').Skip(1).FirstOrDefault() ?? string.Empty,
            JoinDate = c.JoinedAt,
            IsActive = c.IsActive,
            PointsEarned = (int)c.PointsEarned,
            Status = c.IsActive ? "Active" : "Inactive"
        }).ToList();
        
        return new GetReferredCustomersQueryResponse
        {
            Customers = new PaginatedList<ReferredCustomerDto>(customers, result.TotalCount, request.PageNumber, request.PageSize)
        };
    }
}

