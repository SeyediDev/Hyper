namespace Hyper.CustomerPortal.Application.Features.Points.Queries;

public record GetPointTypesQuery : IRequest<GetPointTypesQueryResponse>;

public record GetPointTypesQueryResponse
{
    public List<PointTypeDto> PointTypes { get; set; } = [];
}

public record PointTypeDto
{
    public string Id { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public string Color { get; set; } = null!;
    public string? Icon { get; set; }
    public bool IsConvertible { get; set; }
    public bool IsTransferable { get; set; }
    public int? ExpirationDays { get; set; }
    public bool ShowInLeaderboard { get; set; }
}

public class GetPointTypesQueryHandler : IRequestHandler<GetPointTypesQuery, GetPointTypesQueryResponse>
{
    // TODO: باید از database لیست نوع امتیازات را بگیریم
    public Task<GetPointTypesQueryResponse> Handle(GetPointTypesQuery request, CancellationToken cancellationToken)
    {
        // فعلاً یک لیست نمونه برمی‌گردانیم
        var pointTypes = new List<PointTypeDto>
        {
            new()
            {
                Id = "1",
                Name = "امتیاز طلایی",
                Description = "امتیاز اصلی برنامه وفاداری",
                Color = "#FFD700",
                Icon = "star",
                IsConvertible = true,
                IsTransferable = true,
                ExpirationDays = 365,
                ShowInLeaderboard = true
            },
            new()
            {
                Id = "2",
                Name = "امتیاز نقره‌ای",
                Description = "امتیاز ثانویه",
                Color = "#C0C0C0",
                Icon = "gift",
                IsConvertible = true,
                IsTransferable = false,
                ExpirationDays = 180,
                ShowInLeaderboard = false
            }
        };
        
        return Task.FromResult(new GetPointTypesQueryResponse
        {
            PointTypes = pointTypes
        });
    }
}

