namespace Hyper.Domain.Features.Rewards;

internal interface IRewardAssetInternalService
{
    [Telemetry]
    Task<List<RewardAsset>> Create(
        Reward reward, CustomerTenant customerTenant, long? eventLogId, int? ruleId, int? actionId, int count, CancellationToken cancellationToken);
}
internal class RewardAssetInternalService(
    ISerialGenerator serialGenerator,
    ICommandRepository<RewardAsset, int> assetCommand
    ) : IRewardAssetInternalService
{
    public async Task<List<RewardAsset>> Create(Reward reward, CustomerTenant customerTenant,
        long? eventLogId, int? ruleId, int? actionId, int count, CancellationToken cancellationToken)
    {
        List<RewardAsset> assets = [];
        List<string>? serials = null;
        if (reward.SerialFormat != null && (reward.ControlAsset==null || reward.ControlAsset==false))
        {
            serials = serialGenerator.GenerateSerials(reward.SerialFormat, count);
        }
        for (int i = 1; i <= count; i++)
        {
            if (reward.ControlAsset is true)
            {
                RewardAsset? asset = await assetCommand.FirstOrDefaultAsync(x => x.RewardId == reward.Id && x.CustomerTenantId == null, cancellationToken);
                if (asset != null)
                {
                    asset.CustomerTenantId = customerTenant.Id;
                    asset.EventLogId = eventLogId;
                    asset.PromotionId = ruleId;
                    asset.PromotionActionId = actionId;
                    asset.Quantity = reward.Quantity;
                    assetCommand.Update(asset);
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
                    RewardId = reward.Id,
                    CustomerTenantId = customerTenant.Id,
                    EventLogId = eventLogId,
                    PromotionId = ruleId,
                    PromotionActionId = actionId,
                    Serial = serials?[i - 1],
                    Quantity = reward.Quantity,
                };
                assetCommand.Add(asset);
                assets.Add(asset);
            }
        }
        _ = await assetCommand.UnitOfWork.SaveChangesAsync(cancellationToken);
        return assets;
    }
}
