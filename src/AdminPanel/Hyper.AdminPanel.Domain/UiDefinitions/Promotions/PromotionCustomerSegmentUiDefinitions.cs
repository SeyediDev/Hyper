namespace Hyper.AdminPanel.Domain.UiDefinitions.Promotions;

public class PromotionCustomerSegmentUiDefinitions : CRUDDefinition<PromotionCustomerSegment>
{
    public override string? Icon => "fa fa-users";
    
    protected override void IndexFormViewModel()
    {
        AddColumns(
            nameof(PromotionCustomerSegment.Promotion)
            , nameof(PromotionCustomerSegment.CustomerSegment)
            , nameof(PromotionCustomerSegment.Order)
            , nameof(PromotionCustomerSegment.Chance)
        );
    }
    
    protected override void CUDFormsViewModel()
    {
        AddFields(nameof(PromotionCustomerSegment.Promotion)
                , nameof(PromotionCustomerSegment.CustomerSegment)
                , nameof(PromotionCustomerSegment.Order)
                , nameof(PromotionCustomerSegment.Chance)
            );
    }
}