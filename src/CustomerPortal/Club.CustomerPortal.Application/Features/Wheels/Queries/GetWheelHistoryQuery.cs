using Hyper.CustomerPortal.Application.Interfaces;

namespace Hyper.CustomerPortal.Application.Features.Wheels.Queries;

public record GetWheelHistoryQuery : IRequest<GetWheelHistoryQueryResponse>
{
    public int Take { get; init; } = 10;
}

public record GetWheelHistoryQueryResponse
{
    public IReadOnlyList<WheelSpinHistoryDto> Items { get; init; } = Array.Empty<WheelSpinHistoryDto>();
}

public class GetWheelHistoryQueryHandler(
    IWheelService wheelService,
    ICustomerRequesterUser requesterUser)
    : IRequestHandler<GetWheelHistoryQuery, GetWheelHistoryQueryResponse>
{
    public async Task<GetWheelHistoryQueryResponse> Handle(GetWheelHistoryQuery request, CancellationToken cancellationToken)
    {
        var customerId = requesterUser.CustomerId;
        var history = await wheelService.GetRecentSpinsAsync(customerId, request.Take, cancellationToken);

        return new GetWheelHistoryQueryResponse
        {
            Items = history
        };
    }
}


































