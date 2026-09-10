using Hyper.Domain.Entities.Rewards;

namespace Hyper.Domain.Features;

internal interface IAwardAssetInternalService
{
    [Telemetry]
    Task<List<RewardAsset>> Create(
        Reward award, Customer customer, long? eventLogId, int? ruleId, int? actionId, int count, CancellationToken cancellationToken);
}
internal class AwardAssetInternalService(
    ISerialGenerator serialGenerator,
    ICommandRepository<RewardAsset, int> assetCmdRepo
    ) : IAwardAssetInternalService
{
    public async Task<List<RewardAsset>> Create(Reward award, Customer customer,
        long? eventLogId, int? ruleId, int? actionId, int count, CancellationToken cancellationToken)
    {
        List<RewardAsset> assets = [];
        List<string>? serials = null;
        if (award.SerialFormat != null && (award.ControlAsset==null || award.ControlAsset==false))
        {
            serials = serialGenerator.GenerateSerials(award.SerialFormat, count);
        }
        for (int i = 1; i <= count; i++)
        {
            if (award.ControlAsset is true)
            {
                RewardAsset? asset = await assetCmdRepo.FirstOrDefaultAsync(x => x.RewardId == award.Id && x.CustomerId == null, cancellationToken);
                if (asset != null)
                {
                    asset.CustomerId = customer.Id;
                    asset.EventLogId = eventLogId;
                    asset.PromotionId = ruleId;
                    asset.PromotionActionId = actionId;
                    asset.Quantity = award.Quantity;
                    assetCmdRepo.Update(asset);
                    assets.Add(asset);
                }
                else
                {
                    //دارایی نداشتیم
                }
            }
            else
            {
                RewardAsset asset = new()
                {
                    RewardId = award.Id,
                    CustomerId = customer.Id,
                    EventLogId = eventLogId,
                    PromotionId = ruleId,
                    PromotionActionId = actionId,
                    Serial = serials?[i - 1],
                    Quantity = award.Quantity,
                };
                assetCmdRepo.Add(asset);
                assets.Add(asset);
            }
        }
        _ = await assetCmdRepo.UnitOfWork.SaveChangesAsync(cancellationToken);
        return assets;
    }
}
