using Hyper.Domain.Features.Channels;

namespace Hyper.Domain.Features.Rewards;

public interface IRewardAssetService
{
    [Telemetry]
    Task<PurchaseRewardResponse> PurchaseReward(PurchaseRewardRequest request, CancellationToken cancellationToken);
    [Telemetry]
    Task<ConsumeAccetResponse> ConsumeReward(ConsumeRewardRequest request, CancellationToken cancellationToken);
}

public record PurchaseRewardRequest
{
    public string Customer { get; set; } = null!;
    public int RewardId { get; set; }
}

public record PurchaseRewardResponse
{
    public List<string?> Assets { get; set; } = null!;
}

public record ConsumeRewardRequest
{
    public int ChannelId { get; set; }
    public string Customer { get; set; } = null!;
    public string Serial { get; set; } = null!;
}

public record ConsumeAccetResponse
{
    public int RemainingQuantity { get; internal set; }
}

internal class RewardAssetService(ILogger<RewardAssetService> logger,
    ICustomerTenantService customerTenantService,
    IRewardService rewardService, IRewardAssetInternalService rewardAssetInternalService,
    IEventService eventService,
    ICommandRepository<CustomerTransaction, long> customerTransactionCmdRepo,
    ICommandRepository<RewardAsset, int> assetCmdRepo
    ) : IRewardAssetService
{

    /// <summary>
    /// مشتری درخواست خرید یک پاداش را می کند
    /// در واقع ما آن پاداش را به مشتری می فروشیم
    /// </summary>
    public async Task<PurchaseRewardResponse> PurchaseReward(PurchaseRewardRequest request, CancellationToken cancellationToken)
    {
        Reward? reward = (await rewardService.GetReward(request.RewardId, cancellationToken))
            ?? throw new NullReferenceException(nameof(reward));//TODO 404
        CustomerTenant? customerTenant = await customerTenantService.GetOrCreateAndSetReservedAttributesAsync(
            reward.TenantId, request.Customer, null/*TODO*/, cancellationToken)
            ?? throw new NullReferenceException(nameof(customerTenant));//TODO 404
        RewardDto? rewardDto = (await rewardService.GetRewardDto(request.RewardId, cancellationToken))
            ?? throw new NullReferenceException(nameof(reward));//TODO 404
        IEnumerable<CustomerPointLevel> customerPointLevels =
            await customerTenantService.GetPointLevelsAsync(customerTenant.Id, false, cancellationToken);

        if (rewardDto.PointLevelId is not null and > 0)
        {
            IEnumerable<CustomerPointLevel> pointCustomerPointLevels = customerPointLevels.Where(x => x.PointLevel?.PointId == rewardDto.PointPointId);
            if (!pointCustomerPointLevels.Any(x => x.PointLevel.Level >= rewardDto.PointLevel))
            {
                throw new Exception("مشتری سطح مورد نیاز پاداش را دارا نیست");//TODO 400
            }
        }
        List<CustomerTransaction> customerTransactions = [];
        if (rewardDto.Costs != null && rewardDto.Costs.Count != 0)
        {
            //کنترل اینکه مشتری امتیاز لازمه را دارد
            IEnumerable<CustomerTransaction> customerBalances =
                await customerTenantService.GetPointBalancesAsync(customerTenant.Id, false, cancellationToken);
            foreach (IGrouping<int, RewardCostDto> costs in rewardDto.Costs.GroupBy(x => x.PointId))
            {
                int costAmount = costs.Where(x => x.PointLevelId == null ||
                    x.PointLevel <= customerPointLevels.Where(l => l.PointLevel.PointId == x.PointLevel).Max(l => l.PointLevel.Level)).Min(x => x.Amount);
                CustomerTransaction? poitBalance = customerBalances.FirstOrDefault(x => x.PointId == costs.Key);
                if (poitBalance == null || poitBalance.Balance < costAmount)
                {
                    throw new Exception("مشتری امتیاز مورد نیاز پاداش را دارا نیست");//TODO 400
                }
            }
            foreach (IGrouping<int, RewardCostDto> costs in rewardDto.Costs.GroupBy(x => x.PointId))
            {
                int costAmount = costs.Where(x => x.PointLevelId == null ||
                    x.PointLevel <= customerPointLevels.Where(l => l.PointLevel.PointId == x.PointLevel).Max(l => l.PointLevel.Level)).Min(x => x.Amount);
                CustomerTransaction? poitBalance = customerBalances.FirstOrDefault(x => x.PointId == costs.Key);
                if (poitBalance != null )
                {
                    logger.LogInformation("Pay Cost of Reward");//TODO
                    CustomerTransaction customerTransaction = new()
                    {
                        TransactionType = CustomerTransactionType.Debit,
                        Debit = costAmount,
                        Balance = costAmount - poitBalance.Balance,
                        CustomerTenantId = poitBalance.CustomerTenantId,
                        PointId = costs.Key
                    };
                    customerTransactions.Add(customerTransaction);
                    poitBalance.ExpireDate = new DateTime();
                    customerTransactionCmdRepo.Update(poitBalance);
                }
            }
        }

        EventResponse eventResponse = (await eventService.ReceiveEventAsync(
            new EventRequest
            {
                ReceiveEventType = ReceiveEventType.PurchaseReward,
                CustomerMobile = request.Customer,
                TenantId = reward.TenantId,
                RewardId = request.RewardId
            }, cancellationToken))!;
        foreach (var customerTransaction in customerTransactions)
        {
            customerTransaction.EventLogId = eventResponse.EventLogId;
            customerTransactionCmdRepo.Add(customerTransaction);
        }
        await customerTransactionCmdRepo.UnitOfWork.SaveChangesAsync(cancellationToken);


        List<RewardAsset> assets = await rewardAssetInternalService.Create(reward, customerTenant, eventResponse.EventLogId, null, null, 1, cancellationToken);
        return new PurchaseRewardResponse { Assets = [.. assets.Select(x => x.Serial)] };
    }

    public async Task<ConsumeAccetResponse> ConsumeReward(ConsumeRewardRequest request, CancellationToken cancellationToken)
    {
        CustomerTenant? customerTenant = await customerTenantService.GetOrCreateAndSetReservedAttributesAsync(
            1/*TODO*/, request.Customer, null/*TODO*/, cancellationToken)
            ?? throw new NullReferenceException(nameof(customerTenant));//TODO 404
        var asset = (await assetCmdRepo.FirstOrDefaultAsync(
            x => x.CustomerTenantId == customerTenant.Id && x.Serial== request.Serial, cancellationToken))
            ?? throw new NullReferenceException(nameof(request.Serial));//TODO 404
        //asset.ExpireDate = DateTime.UtcNow; // ExpireDate property doesn't exist in RewardAsset
        asset.ConsumedQuantity++;
        assetCmdRepo.Update(asset);
        await assetCmdRepo.UnitOfWork.SaveChangesAsync(cancellationToken);
        
        Reward? reward = await rewardService.GetReward(asset.RewardId, cancellationToken)
            ?? throw new NullReferenceException(nameof(asset.RewardId));//TODO 404

        EventResponse eventResponse = await eventService.ReceiveEventAsync(
            new EventRequest
            {
                ReceiveEventType = ReceiveEventType.ConsumeReward,
                CustomerMobile = request.Customer,
                TenantId = reward.TenantId,
                RewardId = asset.RewardId,
                AssetId = asset.Id
            }, cancellationToken);
        return new ConsumeAccetResponse 
        {
            RemainingQuantity = asset.RemainingQuantity,
        };
    }
}