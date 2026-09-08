namespace Hyper.AdminPanel.Domain.UiDefinitions.Promotions.PromotionsData;

public class PromotionMetricsUiDefinitions : CRUDDefinition<PromotionMetrics>
{
    public override string? Icon => "fa fa-bar-chart";
    
    protected override void IndexFormViewModel()
    {
        AddColumns(
            nameof(PromotionMetrics.Promotion),
            nameof(PromotionMetrics.ConversionRate),
            nameof(PromotionMetrics.ReturnOnInvestment),
            nameof(PromotionMetrics.CustomerAcquisitionCost),
            nameof(PromotionMetrics.EffectivenessScore),
            nameof(PromotionMetrics.LastMetricsCalculationDate)
        );
    }
    
    protected override void CUDFormsViewModel()
    {
        AddField(nameof(PromotionMetrics.Promotion), eControlPropertyId.ReadOnly);
        
        // Campaign Performance
        AddField(nameof(PromotionMetrics.TargetAudienceCount), eControlPropertyId.ReadOnly);
        AddField(nameof(PromotionMetrics.MessagesSent), eControlPropertyId.ReadOnly);
        AddField(nameof(PromotionMetrics.MessagesDelivered), eControlPropertyId.ReadOnly);
        AddField(nameof(PromotionMetrics.DeliveryRate), eControlPropertyId.ReadOnly);
        
        // Engagement
        AddField(nameof(PromotionMetrics.OpenCount), eControlPropertyId.ReadOnly);
        AddField(nameof(PromotionMetrics.OpenRate), eControlPropertyId.ReadOnly);
        AddField(nameof(PromotionMetrics.ClickCount), eControlPropertyId.ReadOnly);
        AddField(nameof(PromotionMetrics.ClickThroughRate), eControlPropertyId.ReadOnly);
        
        // Conversion
        AddField(nameof(PromotionMetrics.ConversionCount), eControlPropertyId.ReadOnly);
        AddField(nameof(PromotionMetrics.ConversionRate), eControlPropertyId.ReadOnly);
        AddField(nameof(PromotionMetrics.NewCustomersAcquired), eControlPropertyId.ReadOnly);
        
        // Financial
        AddField(nameof(PromotionMetrics.CampaignCost), eControlPropertyId.ReadOnly);
        AddField(nameof(PromotionMetrics.CampaignRevenue), eControlPropertyId.ReadOnly);
        AddField(nameof(PromotionMetrics.ReturnOnInvestment), eControlPropertyId.ReadOnly);
        AddField(nameof(PromotionMetrics.CustomerAcquisitionCost), eControlPropertyId.ReadOnly);
        AddField(nameof(PromotionMetrics.CostPerConversion), eControlPropertyId.ReadOnly);
        
        // Summary
        AddField(nameof(PromotionMetrics.EffectivenessScore), eControlPropertyId.ReadOnly);
        AddField(nameof(PromotionMetrics.LastMetricsCalculationDate), eControlPropertyId.ReadOnly);
    }
}

