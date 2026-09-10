using System.Security.Cryptography;
using Hyper.CustomerPortal.Application.Interfaces;
using Hyper.Domain.Entities.Lotteries;
using Hyper.Domain.Entities.Lotteries.Enums;
using Hyper.Domain.Repository;
using Hyper.Infrastructure.Data.Repository.Hyper;
using Microsoft.EntityFrameworkCore;

namespace Hyper.CustomerPortal.Application.Services;

/// <summary>
/// پیاده‌سازی واقعی تجربه چرخونه مشتریان
/// </summary>
public class WheelService(
    IQueryRepository<Lottery, int> lotteryQueryRepository,
    IQueryRepository<LotteryParticipant, int> participantQueryRepository,
    IQueryRepository<CustomerTenant, int> customerTenantQueryRepository,
    IHyperUnitOfWorkCommand commandUnitOfWork,
    ILogger<WheelService> logger) : IWheelService
{
    public async Task<WheelExperienceDto?> GetActiveWheelAsync(int customerId, CancellationToken cancellationToken = default)
    {
        var wheel = await GetActiveWheelEntityAsync(cancellationToken);
        if (wheel is null)
        {
            return null;
        }

        var customerTenant = await EnsureCustomerTenantAsync(customerId, wheel.Promotion.TenantId, cancellationToken);
        var (spinsToday, totalSpins) = await GetParticipationStatsAsync(wheel.Id, customerTenant.Id, cancellationToken);

        var segments = BuildSegmentDtos(wheel);

        return new WheelExperienceDto
        {
            Id = wheel.Id,
            Title = wheel.Title,
            Description = wheel.Description,
            Subtitle = wheel.WheelSubtitle,
            Theme = wheel.WheelTheme,
            ButtonLabel = wheel.WheelButtonLabel,
            BackgroundColor = wheel.WheelBackgroundColor,
            CenterIcon = wheel.WheelCenterIcon,
            CelebrationMessage = wheel.WheelCelebrationMessage,
            CallToAction = wheel.WheelCallToAction,
            SpinDurationSeconds = wheel.SpinDurationSeconds,
            MaxDailySpins = wheel.MaxDailySpins,
            MaxTotalSpins = wheel.MaxTotalSpins,
            SpinsUsedToday = spinsToday,
            TotalSpins = totalSpins,
            Segments = segments
        };
    }

    public async Task<WheelSpinResultDto> SpinAsync(int customerId, CancellationToken cancellationToken = default)
    {
        var wheel = await GetActiveWheelEntityAsync(cancellationToken)
            ?? throw new InvalidOperationException("چرخونه فعالی برای شرکت در دسترس نیست");

        var customerTenant = await EnsureCustomerTenantAsync(customerId, wheel.Promotion.TenantId, cancellationToken);
        var (spinsToday, totalSpins) = await GetParticipationStatsAsync(wheel.Id, customerTenant.Id, cancellationToken);

        if (spinsToday >= wheel.MaxDailySpins)
        {
            throw new InvalidOperationException("تعداد چرخش‌های امروز شما به سقف مجاز رسیده است");
        }

        if (wheel.MaxTotalSpins.HasValue && totalSpins >= wheel.MaxTotalSpins.Value)
        {
            throw new InvalidOperationException("به سقف کل چرخش‌های مجاز خود رسیده‌اید");
        }

        var activeSegments = wheel.LotteryRewards
            .Where(r => r.IsActive && (!r.MaxDistributionCount.HasValue || r.DistributedCount < r.MaxDistributionCount.Value))
            .OrderBy(r => r.DisplayOrder)
            .ThenBy(r => r.Id)
            .ToList();

        if (!activeSegments.Any())
        {
            throw new InvalidOperationException("برای این چرخونه هیچ قطعه فعالی تعریف نشده است");
        }

        var selectedSegment = SelectSegmentByWeight(activeSegments);

        var participantRepo = commandUnitOfWork.Repository<LotteryParticipant, int>();
        var rewardRepo = commandUnitOfWork.Repository<LotteryReward, int>();

        var trackedSegment = await rewardRepo.GetAsync(selectedSegment.Id, cancellationToken);
        if (trackedSegment is not null)
        {
            trackedSegment.DistributedCount += 1;
            rewardRepo.Update(trackedSegment);
        }

        var now = DateTime.UtcNow;

        var participant = new LotteryParticipant
        {
            LotteryId = wheel.Id,
            LotteryRewardId = selectedSegment.Id,
            CustomerTenantId = customerTenant.Id,
            ParticipatedAt = now,
            IsWinner = selectedSegment.RewardId > 0,
            RewardId = selectedSegment.RewardId > 0 ? selectedSegment.RewardId : null,
            Reward = selectedSegment.Reward,
            RewardAmount = selectedSegment.Amount,
            AnnouncedAt = now,
            IsRewardDistributed = false
        };

        participantRepo.Add(participant);
        await commandUnitOfWork.SaveChangesAsync(cancellationToken);

        var ticketNumber = $"WHL-{participant.Id:D6}";

        logger.LogInformation("Customer {CustomerId} spun wheel {WheelId} and landed on segment {SegmentId}",
            customerTenant.CustomerId, wheel.Id, selectedSegment.Id);

        var segments = BuildSegmentDtos(wheel);
        var selectedSegmentDto = segments.First(s => s.Id == selectedSegment.Id);

        return new WheelSpinResultDto
        {
            Segment = selectedSegmentDto,
            OccurredAt = now,
            SpinsUsedToday = spinsToday + 1,
            RemainingDailySpins = Math.Max(0, wheel.MaxDailySpins - (spinsToday + 1)),
            TotalSpins = totalSpins + 1,
            TicketNumber = ticketNumber
        };
    }

    public async Task<IReadOnlyList<WheelSpinHistoryDto>> GetRecentSpinsAsync(int customerId, int take = 10, CancellationToken cancellationToken = default)
    {
        var wheel = await GetActiveWheelEntityAsync(cancellationToken);
        if (wheel is null)
        {
            return Array.Empty<WheelSpinHistoryDto>();
        }

        var customerTenant = await EnsureCustomerTenantAsync(customerId, wheel.Promotion.TenantId, cancellationToken);

        var query = participantQueryRepository
            .Query()
            .Include(p => p.LotteryReward)
            .Include(p => p.Reward)
            .Where(p => p.CustomerTenantId == customerTenant.Id && p.LotteryId == wheel.Id)
            .OrderByDescending(p => p.ParticipatedAt)
            .Take(take);

        var history = await query
            .Select(p => new WheelSpinHistoryDto
            {
                SegmentLabel = p.LotteryReward != null
                    ? p.LotteryReward.SegmentLabel
                    : (p.Reward != null ? p.Reward.Title : "بدون جایزه"),
                SegmentColor = p.LotteryReward != null ? p.LotteryReward.SegmentColor : "#f97316",
                SegmentIcon = p.LotteryReward != null ? p.LotteryReward.SegmentIcon : null,
                IsWinner = p.IsWinner,
                RewardTitle = p.Reward != null ? p.Reward.Title : null,
                SpunAt = p.ParticipatedAt,
                TicketNumber = $"WHL-{p.Id:D6}"
            })
            .ToListAsync(cancellationToken);

        return history;
    }

    private async Task<Lottery?> GetActiveWheelEntityAsync(CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;

        return await lotteryQueryRepository
            .Query()
            .Include(l => l.Promotion)
            .Include(l => l.LotteryRewards.Where(r => r.IsActive))
                .ThenInclude(r => r.Reward)
            .Where(l => (int)l.LotteryType == (int)LotteryType.Wheel &&
                        l.IsActive &&
                        (!l.FromDate.HasValue || l.FromDate <= now) &&
                        (!l.ToDate.HasValue || l.ToDate >= now))
            .OrderBy(l => l.CreateDate)
            .FirstOrDefaultAsync(cancellationToken);
    }

    private static IReadOnlyList<WheelSegmentDto> BuildSegmentDtos(Lottery wheel)
    {
        var activeRewards = wheel.LotteryRewards
            .Where(r => r.IsActive && (!r.MaxDistributionCount.HasValue || r.DistributedCount < r.MaxDistributionCount.Value))
            .OrderBy(r => r.DisplayOrder)
            .ThenBy(r => r.Id)
            .ToList();

        if (!activeRewards.Any())
        {
            return Array.Empty<WheelSegmentDto>();
        }

        var totalWeight = activeRewards.Sum(r => Math.Max(r.WinRate, 1));
        if (totalWeight == 0)
        {
            totalWeight = activeRewards.Count;
        }

        return activeRewards.Select(r => new WheelSegmentDto
        {
            Id = r.Id,
            Label = string.IsNullOrWhiteSpace(r.SegmentLabel) ? (r.Reward?.Title ?? "شانس دوباره") : r.SegmentLabel,
            Color = r.SegmentColor,
            TextColor = r.SegmentTextColor,
            Icon = r.SegmentIcon,
            Message = r.SegmentMessage ?? (r.Reward != null ? r.Reward.Title : null),
            IsJackpot = r.IsJackpot,
            WinRate = Math.Max(r.WinRate, 1),
            Probability = decimal.Round(Math.Max(r.WinRate, 1) / (decimal)totalWeight, 4),
            Reward = r.Reward != null
                ? new RewardSummaryDto
                {
                    Id = r.RewardId,
                    Title = r.Reward.Title,
                    PictureId = r.Reward.PictureId
                }
                : null
        }).ToList();
    }

    private static LotteryReward SelectSegmentByWeight(IReadOnlyList<LotteryReward> segments)
    {
        var totalWeight = segments.Sum(r => Math.Max(r.WinRate, 1));
        var ticket = RandomNumberGenerator.GetInt32(totalWeight);

        var cumulative = 0;
        foreach (var segment in segments)
        {
            cumulative += Math.Max(segment.WinRate, 1);
            if (ticket < cumulative)
            {
                return segment;
            }
        }

        return segments.Last();
    }

    private async Task<CustomerTenant> EnsureCustomerTenantAsync(int customerId, int tenantId, CancellationToken cancellationToken)
    {
        var customerTenant = await customerTenantQueryRepository
            .FirstOrDefaultAsync(ct => ct.CustomerId == customerId && ct.TenantId == tenantId, cancellationToken);

        if (customerTenant != null)
        {
            return customerTenant;
        }

        var commandRepo = commandUnitOfWork.Repository<CustomerTenant, int>();

        customerTenant = new CustomerTenant
        {
            CustomerId = customerId,
            TenantId = tenantId,
            JoinDate = DateTime.UtcNow,
            IsActive = true
        };

        commandRepo.Add(customerTenant);
        await commandUnitOfWork.SaveChangesAsync(cancellationToken);

        return customerTenant;
    }

    private async Task<(int Today, int Total)> GetParticipationStatsAsync(int lotteryId, int customerTenantId, CancellationToken cancellationToken)
    {
        var participationQuery = participantQueryRepository
            .Query()
            .Where(p => p.LotteryId == lotteryId && p.CustomerTenantId == customerTenantId);

        var totalCount = await participationQuery.CountAsync(cancellationToken);

        var todayStart = DateTime.UtcNow.Date;
        var todayCount = await participationQuery
            .Where(p => p.ParticipatedAt >= todayStart)
            .CountAsync(cancellationToken);

        return (todayCount, totalCount);
    }
}

