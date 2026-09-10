using Hyper.Domain.Features.Rewards;

namespace Hyper.Application.Features.Hyper.Queries;

public record RewardsQuery() : IRequest<RewardsQueryResponse?>
{
    public int TenantId {  get; set; }
}

public record RewardsQueryResponse
{
    public List<RewardDto> Rewards { get; set; } = null!;
}

public class RewardsQueryHandler(
    IRewardService rewardService
    //, IQueryRepository<Customer, int> customerRepository
    ) : IRequestHandler<RewardsQuery, RewardsQueryResponse?>
{
    public async Task<RewardsQueryResponse?> Handle(RewardsQuery request, CancellationToken cancellationToken)
    {
        List<RewardDto> rewards = await rewardService.GetRewardDtos(request.TenantId, cancellationToken);
        //TODO
        return new RewardsQueryResponse
        {
            Rewards = rewards,
        };
    }
}
