namespace Hyper.AdminPanel.Domain.UiDefinitions.Promotions.Base;

using Promotion = Hyper.Domain.Entities.Promotions.Promotion;
using PromotionUiDefinitions = Hyper.AdminPanel.Domain.UiDefinitions.Promotions.PromotionUiDefinitions;

public abstract class KpiWidgetPromotionBase<TReportConfig>
    : KpiWidgetBase<Promotion, PromotionUiDefinitions.PublicReport, TReportConfig>
    where TReportConfig : ReportConfigDefinition
{
}









