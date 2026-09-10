namespace Hyper.AdminPanel.Domain.UiDefinitions.Plans;

public partial class PlanUiDefinitions : CRUDDefinition<Plan>
{
    private static readonly List<string> DefaultRoles =
    [
        Neo.Domain.Constants.Roles.Admin,
        HyperRoles.Manager,
        HyperRoles.FinanceManager
    ];

    public override List<string>? Roles => DefaultRoles;

    protected override void IndexFormViewModel()
    {
        AddColumns(
            nameof(Plan.Title),
            nameof(Plan.Point),
            nameof(Plan.PriceInPoints),
            nameof(Plan.ValidityDays),
            nameof(Plan.DiscountType),
            nameof(Plan.DiscountValue),
            nameof(Plan.IsGlobalDiscount),
            nameof(Plan.IsActive)
        );

        form.AddOrderBy(nameof(Plan.Title));
    }

    protected override void CUDFormsViewModel()
    {
        AddField(nameof(Plan.Title));
        AddField(nameof(Plan.Description));
        AddField(nameof(Plan.Point));
        AddField(nameof(Plan.PriceInPoints));
        AddField(nameof(Plan.ValidityDays));
        AddField(nameof(Plan.DiscountType));
        AddField(nameof(Plan.DiscountValue));
        AddField(nameof(Plan.IsGlobalDiscount));
        AddField(nameof(Plan.CustomerSegment));
        AddField(nameof(Plan.OrderId));
        AddField(nameof(Plan.Picture), eControlTypeId.File);
        AddField(nameof(Plan.IsActive));
    }
}

