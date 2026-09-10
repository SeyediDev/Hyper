using Hyper.Domain.Entities.Rewards;

namespace Hyper.Domain.Features;

public interface IAwardAssetService
{
    [Telemetry]
    Task<PurchaseAwardResponse> PurchaseAward(PurchaseAwardRequest request, CancellationToken cancellationToken);
    [Telemetry]
    Task<ConsumeAccetResponse> ConsumeAward(ConsumeAwardRequest request, CancellationToken cancellationToken);
}

[OldDbMap("PurchaseProductRequest")]
public record PurchaseAwardRequest
{
    public string Customer { get; set; } = null!;
    [OldDbMap("ProductId")]
    public int AwardId { get; set; }
}

[OldDbMap("PurchaseProductResponse")]
public record PurchaseAwardResponse
{
    public List<string?> Assets { get; set; } = null!;
}

public record ConsumeAwardRequest
{
    public string Customer { get; set; } = null!;
    public string Serial { get; set; } = null!;
}

public record ConsumeAccetResponse
{
    public int RemainingQuantity { get; internal set; }
}

internal class AwardAssetService(ILogger<AwardAssetService> logger,
    ICustomerService customerService,
    IAwardService awardService, IAwardAssetInternalService awardAssetInternalService,
    IEventService eventService, IPromotionService promotionService,
    ICommandRepository<CustomerTransaction, long> customerTransactionCmdRepo,
    ICommandRepository<RewardAsset, int> assetCmdRepo
    ) : IAwardAssetService
{

    /// <summary>
    /// مشتری درخواست خرید یک پاداش را می کند
    /// در واقع ما آن پاداش را به مشتری می فروشیم
    /// </summary>
    public async Task<PurchaseAwardResponse> PurchaseAward(PurchaseAwardRequest request, CancellationToken cancellationToken)
    {
        Reward? award = (await awardService.GetAward(request.AwardId, cancellationToken))
            ?? throw new NullReferenceException(nameof(award));//TODO 404
        Customer? customer = await customerService.GetCustomer(request.Customer, false, null, cancellationToken)
            ?? throw new NullReferenceException(nameof(customer));//TODO 404
        AwardDto? awardDto = (await awardService.GetAwardDto(request.AwardId, cancellationToken))
            ?? throw new NullReferenceException(nameof(award));//TODO 404
        IEnumerable<CustomerPointLevel> customerPointLevels = await customerService.GetCustomerPointLevels(customer.Id, award.TenantId, cancellationToken);
        EventResponse eventResponse = (await eventService.RecordEventAsync(
            new(TriggerType.PurchaseAward, request.Customer, null)
            {
                TenantId = award.TenantId,
                AwardId = request.AwardId
            }, cancellationToken))!;

        if (awardDto.PointLevelId is not null and > 0)
        {
            IEnumerable<CustomerPointLevel> pointCustomerPointLevels = customerPointLevels.Where(x => x.PointLevel?.PointId == awardDto.PointPointId);
            if (!pointCustomerPointLevels.Any(x => x.PointLevel.Level >= awardDto.PointLevel))
            {
                throw new Exception("مشتری سطح مورد نیاز پاداش را دارا نیست");//TODO 400
            }
        }
        if (awardDto.Costs != null && awardDto.Costs.Any())
        {
            //کنترل اینکه مشتری امتیاز لازمه را دارد
            IEnumerable<CustomerTransaction> customerBalances = await customerService.GetCustomerPointBalances(customer.Id, award.TenantId, cancellationToken);
            foreach (IGrouping<int, AwardCostDto> costs in awardDto.Costs.GroupBy(x => x.PointId))
            {
                int costAmount = costs.Where(x => x.PointLevelId == null ||
                    x.PointLevel <= customerPointLevels.Where(l => l.PointLevel.PointId == x.PointLevel).Max(l => l.PointLevel.Level)).Min(x => x.Amount);
                CustomerTransaction? poitBalance = customerBalances.FirstOrDefault(x => x.PointId == costs.Key);
                if (poitBalance == null || poitBalance.Balance < costAmount)
                {
                    throw new Exception("مشتری امتیاز مورد نیاز پاداش را دارا نیست");//TODO 400
                }
            }
            foreach (IGrouping<int, AwardCostDto> costs in awardDto.Costs.GroupBy(x => x.PointId))
            {
                int costAmount = costs.Where(x => x.PointLevelId == null ||
                    x.PointLevel <= customerPointLevels.Where(l => l.PointLevel.PointId == x.PointLevel).Max(l => l.PointLevel.Level)).Min(x => x.Amount);
                CustomerTransaction? poitBalance = customerBalances.FirstOrDefault(x => x.PointId == costs.Key);
                if (poitBalance != null )
                {
                    logger.LogInformation("Pay Cost of Award");//TODO
                    CustomerTransaction customerTransaction = new()
                    {
                        TransactionType = CustomerTransactionType.Debit,
                        Debit = costAmount,
                        Balance = costAmount - poitBalance.Balance,
                        TenantId = award.TenantId,
                        CustomerTenantId = poitBalance.CustomerTenantId,
                        PointId = costs.Key,
                        EventLogId = eventResponse.EventLogId,
                    };
                    customerTransactionCmdRepo.Add(customerTransaction);
                    poitBalance.ExpireDate = new DateTime();
                    customerTransactionCmdRepo.Update(poitBalance);
                }
            }
        }

        List<RewardAsset> assets = await awardAssetInternalService.Create(award, customer, eventResponse.EventLogId, null, null, 1, cancellationToken);
        await promotionService.ProcessEventAsync(
            new PromotionProcessingRequest(
                award.TenantId,
                (int)eventResponse.EventLogId,
                0,
                0,
                eventResponse.Customer,
                null)
            {
                TriggerType = TriggerType.PurchaseAward,
                AwardId = award.Id
            }, cancellationToken);
        return new PurchaseAwardResponse { Assets = assets.Select(x => x.Serial).ToList() };
    }

    public async Task<ConsumeAccetResponse> ConsumeAward(ConsumeAwardRequest request, CancellationToken cancellationToken)
    {
        Customer? customer = (await customerService.GetCustomer(request.Customer, false, null, cancellationToken))
            ?? throw new NullReferenceException(nameof(customer));//TODO 404
        var asset = (await assetCmdRepo.FirstOrDefaultAsync(
            x => x.CustomerId == customer.Id && x.Serial== request.Serial, cancellationToken))
            ?? throw new NullReferenceException(nameof(request.Serial));//TODO 404
        //asset.ExpireDate = DateTime.Now; // ExpireDate property doesn't exist in AwardAsset
        asset.ConsumedQuantity++;
        assetCmdRepo.Update(asset);
        await assetCmdRepo.UnitOfWork.SaveChangesAsync(cancellationToken);
        
        Reward? reward = await awardService.GetAward(asset.RewardId, cancellationToken)
            ?? throw new NullReferenceException(nameof(asset.RewardId));//TODO 404

        EventResponse eventResponse = await eventService.RecordEventAsync(
            new(TriggerType.ConsumeAward, request.Customer, null)
            {
                TenantId = reward.TenantId,
                AwardId = asset.RewardId,
                AssetId = asset.Id
            }, cancellationToken);
        if (eventResponse != null)
        {
            await promotionService.ProcessEventAsync(
                new PromotionProcessingRequest(
                    reward.TenantId,
                    (int)eventResponse.EventLogId,
                    0,
                    0,
                    eventResponse.Customer,
                    null)
                {
                    TriggerType = TriggerType.ConsumeAward,
                    AwardId = asset.RewardId
                }, cancellationToken);
        }
        return new ConsumeAccetResponse 
        {
            RemainingQuantity = asset.RemainingQuantity,
        };
    }
}
