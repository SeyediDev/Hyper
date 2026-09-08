using Hyper.CustomerPortal.Application.Interfaces;

namespace Hyper.CustomerPortal.Application.Features.Points.Queries;

public record GetPointsSummaryQuery : IRequest<GetPointsSummaryQueryResponse>;

public record GetPointsSummaryQueryResponse
{
    public List<CustomerPointDto> Points { get; set; } = [];
}

public record CustomerPointDto
{
    public string PointTypeId { get; set; } = null!;
    public string PointTypeName { get; set; } = null!;
    public string PointTypeColor { get; set; } = null!;
    public int TotalPoints { get; set; }
    public int AvailablePoints { get; set; }
    public int PendingPoints { get; set; }
    public int UsedPoints { get; set; }
    public int ExpiredPoints { get; set; }
    public int? ExpiringPoints { get; set; }
    public DateTime? ExpirationDate { get; set; }
}

public class GetPointsSummaryQueryHandler(
    IPointService pointService,
    ICustomerRequesterUser requesterUser) : IRequestHandler<GetPointsSummaryQuery, GetPointsSummaryQueryResponse>
{
    public async Task<GetPointsSummaryQueryResponse> Handle(GetPointsSummaryQuery request, CancellationToken cancellationToken)
    {
        var customerId = requesterUser.CustomerId;
        
        var summary = await pointService.GetPointsSummaryAsync(customerId, cancellationToken);
        
        // TODO: باید از دیتابیس لیست تمام نوع‌های امتیاز مشتری را بگیریم
        // فعلاً یک پاسخ sample برمی‌گردانیم
        
        return new GetPointsSummaryQueryResponse
        {
            Points =
            [
                new CustomerPointDto
                {
                    PointTypeId = "1",
                    PointTypeName = "امتیاز طلایی",
                    PointTypeColor = "#FFD700",
                    TotalPoints = (int)summary.TotalPoints,
                    AvailablePoints = (int)summary.AvailablePoints,
                    PendingPoints = (int)summary.PendingPoints,
                    UsedPoints = 0,
                    ExpiredPoints = (int)summary.ExpiredPoints,
                    ExpiringPoints = null,
                    ExpirationDate = null
                }
            ]
        };
    }
}

