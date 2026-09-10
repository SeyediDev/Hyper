namespace Hyper.AdminPanel.Domain.UiDefinitions.Tenants;

public class TenantAttributeUiDefinitions : CRUDDefinition<TenantAttribute>
{
    public override string? Icon => "fa fa-cog";

    protected override void IndexFormViewModel()
    {
        AddColumns(nameof(TenantAttribute.Tenant)
                 , nameof(TenantAttribute.Title)
                 , nameof(TenantAttribute.Area)
                 , nameof(TenantAttribute.CustomerUsage)
                 , nameof(TenantAttribute.Key)
                 , nameof(TenantAttribute.FormulaTerm)
                 , nameof(TenantAttribute.CustomerFormulaTerm)
                 , nameof(TenantAttribute.ValueType)
                 , nameof(TenantAttribute.AreaParameter)
                 , nameof(TenantAttribute.IsActive)
                   );
        AddOrderBy(nameof(TenantAttribute.Tenant)
                 , nameof(TenantAttribute.Area)
                 , nameof(TenantAttribute.CustomerUsage)
                 , nameof(TenantAttribute.Key));
    }

    enum Groups { BasicInfo, AreaParams }

    protected override void CUDFormsViewModel()
    {
        AddGroup(nameof(Groups.AreaParams), "ناحیه");
        {
            AddFields(nameof(TenantAttribute.Tenant)
                    , nameof(TenantAttribute.Title)
                    , nameof(TenantAttribute.Key)
                    , nameof(TenantAttribute.ValueStorageType)
                    , nameof(TenantAttribute.Area)
                    , nameof(TenantAttribute.CustomerUsage)
                    , nameof(TenantAttribute.Segment)
                    , nameof(TenantAttribute.ProductCategory)
                    , nameof(TenantAttribute.Product)
                    , nameof(TenantAttribute.Channel)
                    , nameof(TenantAttribute.EventType)
                );
            EndGroup();
        }
        AddGroup(nameof(Groups.BasicInfo), "اطلاعات تکمیلی");
        {
            AddFields(nameof(TenantAttribute.DefaultValue)
                    , nameof(TenantAttribute.ValueType)
                    , nameof(TenantAttribute.IsActive)
                    , nameof(TenantAttribute.Description)
                    , nameof(TenantAttribute.ValidateByType)
                    , nameof(TenantAttribute.ValidateByList)
                    , nameof(TenantAttribute.IsOptional)
                    , nameof(TenantAttribute.CreatedBySystem)
                );
            EndGroup();
        }
        AddSubTable<TenantAttributeAllowedValue>(nameof(TenantAttributeAllowedValue.TenantAttribute),
            "مقادیر مجاز");
    }

    protected override void UIRules(FormDefinition form)
    {
        base.UIRules(form);

        const string areaField = nameof(TenantAttribute.Area);
        ShowHide(areaField,
            $"q[{areaField}]=={(int)AttributeArea.Segment}",
            nameof(TenantAttribute.Segment));
        ShowHide(areaField,
            $"q[{areaField}]=={(int)AttributeArea.Product}",
            nameof(TenantAttribute.ProductCategory), nameof(TenantAttribute.Product));
        ShowHide(areaField,
            $"q[{areaField}]=={(int)AttributeArea.Channel}",
            nameof(TenantAttribute.Channel));
        ShowHide(areaField,
            $"q[{areaField}]=={(int)AttributeArea.Event}",
            nameof(TenantAttribute.EventType));
        var tenantFilter = $"TenantId=q[{nameof(TenantAttribute.Tenant)}]";
        foreach (var field in new List<string>([
            nameof(TenantAttribute.Segment)
            , nameof(TenantAttribute.ProductCategory)
            //, nameof(TenantAttribute.Product)
            , nameof(TenantAttribute.Channel)
            , nameof(TenantAttribute.EventType)
            ]))
        {
            FilterFormula(nameof(TenantAttribute.Tenant), field!, tenantFilter);
        }
        SetFilterFormula(new([nameof(TenantAttribute.Tenant), nameof(TenantAttribute.ProductCategory)]),
            new([nameof(TenantAttribute.Product)]),
            tenantFilter +
            $"And ((q[{nameof(TenantAttribute.ProductCategory)}]<=0) || " +
            $"({nameof(Product.ProductCategoryId)}=q[{nameof(TenantAttribute.ProductCategory)}]))");
    }
}