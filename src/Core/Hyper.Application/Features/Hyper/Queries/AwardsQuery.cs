namespace Hyper.Application.Features.Hyper.Queries;

public record AwardsQuery() : IRequest<AwardsQueryResponse?>
{
    public int TenantId {  get; set; }
}

public record AwardsQueryResponse
{
    public List<AwardDto> Awards { get; set; } = null!;
}

public class AwardsQueryHandler(
    IAwardService awardService
    //, IQueryRepository<Customer, int> customerRepository
    ) : IRequestHandler<AwardsQuery, AwardsQueryResponse?>
{
    public async Task<AwardsQueryResponse?> Handle(AwardsQuery request, CancellationToken cancellationToken)
    {
        List<AwardDto> awards = await awardService.GetAwardDtos(request.TenantId, cancellationToken);
        //TODO
        return new AwardsQueryResponse
        {
            Awards = awards,
        };
    }
}
