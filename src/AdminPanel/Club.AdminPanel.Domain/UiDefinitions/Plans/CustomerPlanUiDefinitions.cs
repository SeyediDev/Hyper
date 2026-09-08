using Hyper.Domain.Entities.Promotions.Plans.Data;

namespace Hyper.AdminPanel.Domain.UiDefinitions.Plans;

public partial class CustomerPlanUiDefinitions : CRUDDefinition<CustomerPlan>
{
    private static readonly List<string> DefaultRoles =
    [
        Neo.Domain.Constants.Roles.Admin,
        HyperRoles.Manager,
        HyperRoles.FinanceManager,
        HyperRoles.CallCenterManager,
        HyperRoles.CallCenterSupport
    ];

    public override List<string>? Roles => DefaultRoles;

    protected override void IndexFormViewModel()
    {
        AddColumns(
            nameof(CustomerPlan.CustomerTenant),
            nameof(CustomerPlan.Plan),
            nameof(CustomerPlan.Status),
            nameof(CustomerPlan.StartDate),
            nameof(CustomerPlan.ExpiryDate),
            nameof(CustomerPlan.IsActive),
            nameof(CustomerPlan.UsageCount),
            nameof(CustomerPlan.TotalDiscountReceived)
        );

        form.AddOrderBy(nameof(CustomerPlan.StartDate));
    }

    protected override void CUDFormsViewModel()
    {
        AddField(nameof(CustomerPlan.CustomerTenant));
        AddField(nameof(CustomerPlan.Plan));
        AddField(nameof(CustomerPlan.PurchaseDate));
        AddField(nameof(CustomerPlan.StartDate));
        AddField(nameof(CustomerPlan.ExpiryDate));
        AddField(nameof(CustomerPlan.Status));
        AddField(nameof(CustomerPlan.PaidAmount));
        AddField(nameof(CustomerPlan.UsageCount));
        AddField(nameof(CustomerPlan.TotalDiscountReceived));
        AddField(nameof(CustomerPlan.CustomerTransaction));
        AddField(nameof(CustomerPlan.IsActive));
        AddField(nameof(CustomerPlan.Notes));
    }
}

