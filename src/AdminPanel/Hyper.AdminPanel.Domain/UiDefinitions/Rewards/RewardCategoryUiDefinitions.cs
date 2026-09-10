namespace Hyper.AdminPanel.Domain.UiDefinitions.Rewards;

public class RewardCategoryUiDefinitions : CRUDDefinition<RewardCategory>
{
    protected override void IndexFormViewModel()
    {
        AddColumns(nameof(RewardCategory.Tenant),
                        nameof(RewardCategory.Title),
                        nameof(RewardCategory.ParentRewardCategory)
                        );
        AddSubjectColumn<CategoryRewards>("پاداش‌ها");
    }

    protected override void CUDFormsViewModel()
    {
        AddFields(nameof(RewardCategory.Tenant),
                       nameof(RewardCategory.Title),
                       nameof(RewardCategory.ParentRewardCategory));
    }

    public class CategoryRewards() : SubjectEditForm<CategoryRewards>("پاداش‌های طبقه‌بندی")
    {
        protected override void ViewModel()
        {
            AddSubTable<RewardCategory>(nameof(RewardCategory.ParentRewardCategory), "طبقه‌بندی های زیرمجموعه");
            AddSubTable<Reward>(nameof(Reward.RewardCategory), "پاداش‌ها");
        }
    }
}