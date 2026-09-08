using Hyper.CustomerPortal.Application.Interfaces;

namespace Hyper.CustomerPortal.Application.Features.Lotteries.Queries;

public record GetCustomerLotteryPositionQuery : IRequest<GetCustomerLotteryPositionQueryResponse>
{
    public string LotteryId { get; set; } = null!;
}

public record GetCustomerLotteryPositionQueryResponse
{
    public CustomerLotteryPositionDto? Position { get; set; }
}

public record CustomerLotteryPositionDto
{
    public int ParticipationId { get; set; }
    public int LotteryId { get; set; }
    public DateTime ParticipatedAt { get; set; }
    public bool IsWinner { get; set; }
    public int Rank { get; set; }
    public int TotalParticipants { get; set; }
    public int? WinnerRank { get; set; }
    public int TotalWinners { get; set; }
    public int Chance { get; set; }
    public int? RewardId { get; set; }
    public string? RewardTitle { get; set; }
    public int RewardAmount { get; set; }
    public DateTime? AnnouncedAt { get; set; }
    public bool IsRewardDistributed { get; set; }
}

public class GetCustomerLotteryPositionQueryHandler(
    Domain.Features.Promotions.ILotteryService lotteryService,
    ICustomerRequesterUser requesterUser,
    IQueryRepository<CustomerTenant, int> customerTenantRepo) : IRequestHandler<GetCustomerLotteryPositionQuery, GetCustomerLotteryPositionQueryResponse>
{
    public async Task<GetCustomerLotteryPositionQueryResponse> Handle(GetCustomerLotteryPositionQuery request, CancellationToken cancellationToken)
    {
        var customerId = requesterUser.CustomerId;
        if (customerId <= 0)
        {
            throw new UnauthorizedAccessException("مشتری شناسایی نشده است");
        }

        // Get TenantId from requesterUser
        if (requesterUser.TenantId<=0)
        {
            throw new UnauthorizedAccessException("اکوسیستم شناسایی نشده است");
        }

        // Get CustomerTenantId from CustomerId and TenantId
        var customerTenant = await customerTenantRepo.FirstOrDefaultAsync(
            ct => ct.CustomerId == customerId && ct.TenantId == requesterUser.TenantId,
            cancellationToken);

        if (customerTenant == null)
        {
            throw new UnauthorizedAccessException("رابطه مشتری-اکوسیستم یافت نشد");
        }

        int customerTenantId = customerTenant.Id;

        var position = await lotteryService.GetCustomerPositionInLotteryAsync(
            int.Parse(request.LotteryId),
            customerTenantId,
            cancellationToken);

        if (position == null)
        {
            return new GetCustomerLotteryPositionQueryResponse { Position = null };
        }

        return new GetCustomerLotteryPositionQueryResponse
        {
            Position = new CustomerLotteryPositionDto
            {
                ParticipationId = position.ParticipationId,
                LotteryId = position.LotteryId,
                ParticipatedAt = position.ParticipatedAt,
                IsWinner = position.IsWinner,
                Rank = position.Rank,
                TotalParticipants = position.TotalParticipants,
                WinnerRank = position.WinnerRank,
                TotalWinners = position.TotalWinners,
                Chance = position.Chance,
                RewardId = position.RewardId,
                RewardTitle = position.RewardTitle,
                RewardAmount = position.RewardAmount,
                AnnouncedAt = position.AnnouncedAt,
                IsRewardDistributed = position.IsRewardDistributed
            }
        };
    }
}

