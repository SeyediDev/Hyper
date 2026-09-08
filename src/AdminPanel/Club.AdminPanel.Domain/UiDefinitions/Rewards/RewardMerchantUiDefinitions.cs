namespace Hyper.AdminPanel.Domain.UiDefinitions.Rewards;

public class RewardMerchantUiDefinitions : CRUDDefinition<RewardMerchant>
{
    protected override void IndexFormViewModel()
    {
        AddColumns(nameof(RewardMerchant.Title),
                        nameof(RewardMerchant.CreateDate)
                        );
        AddSubjectColumn<MerchantRewards>();
    }
    protected override void CUDFormsViewModel()
    {
        AddField(nameof(RewardMerchant.Title));
    }

    public class MerchantRewards : EditForm, ISubjectFormDefinition
    {
        public override string SubjectId => nameof(MerchantRewards);
        public override string Name => "پاداش‌های ارائه شده توسط ";

        protected override void ViewModel()
        {
            AddSubTable(nameof(Reward), nameof(Reward.Merchant), null, "پاداش‌ها", null, false, ContainerControl.None);
        }
    }
}