using Hyper.Domain.Entities.Lotteries;

namespace Hyper.AdminPanel.Domain.UiDefinitions.Promotions;

public partial class PromotionUiDefinitions : CRUDDefinition<Promotion>
{
    private static readonly List<string> DefaultRoles =
    [
        Neo.Domain.Constants.Roles.Admin,
        HyperRoles.MarketingManager,
        HyperRoles.Analyst,
        HyperRoles.Manager
    ];

    public override List<string>? Roles => DefaultRoles;
    public override string? Icon => "fa fa-bullhorn";

    protected override void IndexFormViewModel()
    {
        AddColumns(nameof(Promotion.Title)
                 , nameof(Promotion.Category)
                 , nameof(Promotion.Status)
                 , nameof(Promotion.FromDate)
                 , nameof(Promotion.ToDate)
                 , nameof(Promotion.FromHour)
                 , nameof(Promotion.ToHour)
                   );
        AddSubjectColumn<Setting>();
    }

    protected override void CUDFormsViewModel()
    {
        AddFields(nameof(Promotion.Title)
                , nameof(Promotion.Category)
                , nameof(Promotion.Status)
                , nameof(Promotion.FromDate)
                , nameof(Promotion.ToDate)
                , nameof(Promotion.FromHour)
                , nameof(Promotion.ToHour)
                );
    }

    public class Setting() : SubjectEditForm<Setting>("تنظیمات")
    {
        protected override void ViewModel()
        {
            AddTable<PromotionTrigger>("محرک‌های عملیات پویش");
            AddTable<PromotionAction>("عملیات پویش");
            AddTable<PromotionCustomerSegment>("جوامع/بازارهای هدف");
            AddTable<PromotionBudget>("بودجه");
            AddTable<PromotionCostAllocation>("تسهیم‌های زمانی هزینه");
            AddTable<Plan>("طرح‌ها");
            AddTable<Survey>("مسابقات/نظرسنجی");
            AddTable<Lottery>("قرعه‌کشی/چرخونه");
            AddTable<PromotionMessage>("پیام‌های ارسالی");

            void AddTable<TTableEntity>(string labelName)
                where TTableEntity : IEntity, ISubOfPromotion
            {
                AddSubTable<TTableEntity>(
                    nameof(ISubOfPromotion.Promotion), labelName, ContainerControl.MultiTab, null, null, null, null, false, null);
            }
        }
    }
}