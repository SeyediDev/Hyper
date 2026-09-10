namespace Hyper.AdminPanel.Domain.UiDefinitions.Points;

public class PromotionBudgetUiDefinitions : CRUDDefinition<PromotionBudget>
{
    protected override void IndexFormViewModel()
    {
        AddColumns(
              nameof(PromotionBudget.Promotion)
            , nameof(PromotionBudget.Kind)
            , nameof(PromotionBudget.Amount)
            , nameof(PromotionBudget.ResetType)
            );
        AddOrderBy(nameof(PromotionBudget.Promotion));
        AddOrderBy(nameof(PromotionBudget.Kind));
    }

    protected override void CUDFormsViewModel()
    {
        AddFields(
              nameof(PromotionBudget.Promotion)
            , nameof(PromotionBudget.Kind)
            , nameof(PromotionBudget.Point)
            , nameof(PromotionBudget.Reward)
            , nameof(PromotionBudget.Lottery)
            , nameof(PromotionBudget.CustomerSegment)
            , nameof(PromotionBudget.ExternalApi)
            , nameof(PromotionBudget.NotificationSubType)

            , nameof(PromotionBudget.Amount)

            , nameof(PromotionBudget.ResetType)
            , nameof(PromotionBudget.ResetDayOfWeek)
            , nameof(PromotionBudget.ResetDayOfMonth)
            , nameof(PromotionBudget.ResetMonth)
            , nameof(PromotionBudget.ResetDayOfYear)
            , nameof(PromotionBudget.ResetHour)
            , nameof(PromotionBudget.ResetMinute)
            );
    }
    
    protected override void UIRules(FormDefinition form)
    {
        base.UIRules(form);
    }
}