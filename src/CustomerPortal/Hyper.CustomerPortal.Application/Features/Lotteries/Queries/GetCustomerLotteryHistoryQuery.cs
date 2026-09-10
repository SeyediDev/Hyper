using Hyper.CustomerPortal.Application.Interfaces;
using Hyper.Domain.Features.Channels;

namespace Hyper.CustomerPortal.Application.Features.Lotteries.Queries;

public record GetCustomerLotteryHistoryQuery : IRequest<GetCustomerLotteryHistoryQueryResponse>
{
    public int LotteryId { get; set; }
}

public record GetCustomerLotteryHistoryQueryResponse
{
    public PaginatedList<CustomerLotteryHistoryDto> History { get; set; } = null!;
}

public record CustomerLotteryHistoryDto
{
    public int ParticipationId { get; set; }
    public int LotteryId { get; set; }
    public string LotteryTitle { get; set; } = null!;
    public string LotteryType { get; set; } = null!;
    public DateTime ParticipatedAt { get; set; }
    public bool IsWinner { get; set; }
    public int? RewardId { get; set; }
    public string? RewardTitle { get; set; }
    public int RewardAmount { get; set; }
    public DateTime? AnnouncedAt { get; set; }
    public bool IsRewardDistributed { get; set; }
    public int Chance { get; set; }
}

public class GetCustomerLotteryHistoryQueryHandler(
    IChannelService channelService,
    Hyper.Domain.Features.Promotions.ILotteryService lotteryService,
    ICustomerRequesterUser requesterUser,
    IQueryRepository<CustomerTenant, int> customerTenantQuery)
    : IRequestHandler<GetCustomerLotteryHistoryQuery, GetCustomerLotteryHistoryQueryResponse>
{
    public async Task<GetCustomerLotteryHistoryQueryResponse> Handle(GetCustomerLotteryHistoryQuery request, CancellationToken cancellationToken)
    {
        var channelDto = await channelService.GetChannelFromUserAsync(cancellationToken);
        if(channelDto==null)
        {
            throw new UnauthorizedAccessException("کانال شناسایی نشده است");
        }
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
        var customerTenant = await customerTenantQuery.FirstOrDefaultAsync(
            ct => ct.CustomerId == customerId && ct.TenantId == requesterUser.TenantId,
            cancellationToken);

        if (customerTenant == null)
        {
            throw new UnauthorizedAccessException("رابطه مشتری-اکوسیستم یافت نشد");
        }

        int customerTenantId = customerTenant.Id;

        var history = await lotteryService.GetCustomerLotteryHistoryAsync(
            customerTenantId,
            request.LotteryId, cancellationToken);

        var historyDtos = history.Select(h => new CustomerLotteryHistoryDto
        {
            ParticipationId = h.ParticipationId,
            LotteryId = h.LotteryId,
            LotteryTitle = h.LotteryTitle,
            LotteryType = h.LotteryType.ToString(),
            ParticipatedAt = h.ParticipatedAt,
            IsWinner = h.IsWinner,
            RewardId = h.RewardId,
            RewardTitle = h.RewardTitle,
            RewardAmount = h.RewardAmount,
            AnnouncedAt = h.AnnouncedAt,
            IsRewardDistributed = h.IsRewardDistributed,
            Chance = h.Chance
        }).ToList();

        return new GetCustomerLotteryHistoryQueryResponse
        {
            History = new PaginatedList<CustomerLotteryHistoryDto>(
                historyDtos,
                historyDtos.Count,
                1,
                historyDtos.Count)
        };
    }
}

