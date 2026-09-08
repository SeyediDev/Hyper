using Hyper.CustomerPortal.Application.Interfaces;

namespace Hyper.CustomerPortal.Application.Features.Points.Queries;

public record GetExpiringPointsQuery : IRequest<GetExpiringPointsQueryResponse>;

public record GetExpiringPointsQueryResponse
{
    public List<ExpiringPointDto> ExpiringPoints { get; set; } = [];
}

public record ExpiringPointDto
{
    public PointTypeDto PointType { get; set; } = null!;
    public int Amount { get; set; }
    public DateTime ExpirationDate { get; set; }
}

public class GetExpiringPointsQueryHandler(
    IPointService pointService,
    ICustomerRequesterUser requesterUser) : IRequestHandler<GetExpiringPointsQuery, GetExpiringPointsQueryResponse>
{
    public async Task<GetExpiringPointsQueryResponse> Handle(GetExpiringPointsQuery request, CancellationToken cancellationToken)
    {
        var customerId = requesterUser.CustomerId;
        
        var expiringPoints = await pointService.GetExpiringPointsAsync(customerId, 30, cancellationToken);
        
        var result = expiringPoints.Select(ep => new ExpiringPointDto
        {
            PointType = new PointTypeDto
            {
                Id = "1",
                Name = "امتیاز طلایی",
                Description = "امتیاز اصلی",
                Color = "#FFD700",
                Icon = "star",
                IsConvertible = true,
                IsTransferable = true,
                ExpirationDays = 365,
                ShowInLeaderboard = true
            },
            Amount = (int)ep.Amount,
            ExpirationDate = ep.ExpiryDate
        }).ToList();
        
        return new GetExpiringPointsQueryResponse
        {
            ExpiringPoints = result
        };
    }
}

