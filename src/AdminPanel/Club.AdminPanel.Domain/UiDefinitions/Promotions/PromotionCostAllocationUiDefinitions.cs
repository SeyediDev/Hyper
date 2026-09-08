namespace Hyper.AdminPanel.Domain.UiDefinitions.Promotions;

public class PromotionCostAllocationUiDefinitions : CRUDDefinition<PromotionCostAllocation>
{
    public override string? Icon => "fa fa-pie-chart";
    
    protected override void IndexFormViewModel()
    {
        AddColumns(
            nameof(PromotionCostAllocation.Promotion),
            nameof(PromotionCostAllocation.Title),
            nameof(PromotionCostAllocation.FromDate),
            nameof(PromotionCostAllocation.ToDate),
            nameof(PromotionCostAllocation.CostPercentage),
            nameof(PromotionCostAllocation.AllocatedCost),
            nameof(PromotionCostAllocation.Order)
        );
    }
    
    protected override void CUDFormsViewModel()
    {
        AddField(nameof(PromotionCostAllocation.Promotion), eControlPropertyId.ReadOnly);
        AddField(nameof(PromotionCostAllocation.Title));
        AddField(nameof(PromotionCostAllocation.FromDate));
        AddField(nameof(PromotionCostAllocation.ToDate));
        AddField(nameof(PromotionCostAllocation.CostPercentage));
        AddField(nameof(PromotionCostAllocation.AllocatedCost));
        AddField(nameof(PromotionCostAllocation.Order));
        AddField(nameof(PromotionCostAllocation.Description));
    }
}