using Hyper.CustomerPortal.Application.Interfaces;

namespace Hyper.CustomerPortal.Application.Features.Points.Queries;

public record GetConversionRatesQuery : IRequest<GetConversionRatesQueryResponse>
{
    public string FromPointTypeId { get; set; } = null!;
}

public record GetConversionRatesQueryResponse
{
    public PointTypeDto FromPointType { get; set; } = null!;
    public List<ConversionRateDto> Conversions { get; set; } = [];
}

public record ConversionRateDto
{
    public PointTypeDto ToPointType { get; set; } = null!;
    public decimal Rate { get; set; }
}

public class GetConversionRatesQueryHandler(IPointService pointService)
    : IRequestHandler<GetConversionRatesQuery, GetConversionRatesQueryResponse>
{
    public async Task<GetConversionRatesQueryResponse> Handle(GetConversionRatesQuery request, CancellationToken cancellationToken)
    {
        var conversionRates = await pointService.GetConversionRatesAsync(cancellationToken);
        
        var conversions = conversionRates.Select(cr => new ConversionRateDto
        {
            ToPointType = new PointTypeDto
            {
                Id = cr.TargetPointTypeId.ToString(),
                Name = cr.TargetPointTypeName,
                Description = null,
                Color = "#FFD700",
                Icon = "star",
                IsConvertible = true,
                IsTransferable = true,
                ExpirationDays = null,
                ShowInLeaderboard = true
            },
            Rate = cr.Rate
        }).ToList();
        
        return new GetConversionRatesQueryResponse
        {
            FromPointType = new PointTypeDto
            {
                Id = request.FromPointTypeId,
                Name = "امتیاز طلایی",
                Description = "امتیاز اصلی",
                Color = "#FFD700",
                Icon = "star",
                IsConvertible = true,
                IsTransferable = true,
                ExpirationDays = 365,
                ShowInLeaderboard = true
            },
            Conversions = conversions
        };
    }
}

