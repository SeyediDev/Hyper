namespace Hyper.AdminPanel.Domain.UiDefinitions.Tenants;

public class TenantAttributeAllowedValueUiDefinitions : CRUDDefinition<TenantAttributeAllowedValue>
{
    public override string? Icon => "fa fa-cog";
    protected override void IndexFormViewModel()
    {
        AddColumns(
            nameof(TenantAttributeAllowedValue.TenantAttribute),
            nameof(TenantAttributeAllowedValue.Value),
            nameof(TenantAttributeAllowedValue.Title),
            nameof(TenantAttributeAllowedValue.DisplayOrder),
            nameof(TenantAttributeAllowedValue.IsActive)
        );
        form.AddOrderBy(nameof(TenantAttributeAllowedValue.TenantAttribute));
        form.AddOrderBy(nameof(TenantAttributeAllowedValue.DisplayOrder));
    }
    protected override void CUDFormsViewModel()
    {
        AddFields(
            nameof(TenantAttributeAllowedValue.TenantAttribute),
            nameof(TenantAttributeAllowedValue.Value),
            nameof(TenantAttributeAllowedValue.Title),
            nameof(TenantAttributeAllowedValue.DisplayOrder),
            nameof(TenantAttributeAllowedValue.IsActive)
            );
    }
}