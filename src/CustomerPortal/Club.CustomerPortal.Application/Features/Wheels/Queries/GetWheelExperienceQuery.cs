using Hyper.CustomerPortal.Application.Interfaces;

namespace Hyper.CustomerPortal.Application.Features.Wheels.Queries;

public record GetWheelExperienceQuery : IRequest<GetWheelExperienceQueryResponse>;

public record GetWheelExperienceQueryResponse
{
    public WheelExperienceDto? Wheel { get; init; }
}

public class GetWheelExperienceQueryHandler(
    IWheelService wheelService,
    ICustomerRequesterUser requesterUser)
    : IRequestHandler<GetWheelExperienceQuery, GetWheelExperienceQueryResponse>
{
    public async Task<GetWheelExperienceQueryResponse> Handle(GetWheelExperienceQuery request, CancellationToken cancellationToken)
    {
        var customerId = requesterUser.CustomerId;
        var wheel = await wheelService.GetActiveWheelAsync(customerId, cancellationToken);

        return new GetWheelExperienceQueryResponse
        {
            Wheel = wheel
        };
    }
}


































