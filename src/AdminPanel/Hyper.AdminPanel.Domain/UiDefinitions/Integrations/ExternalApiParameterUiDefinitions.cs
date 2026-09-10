using Neo.Domain.Entities.Integrations;

namespace Hyper.AdminPanel.Domain.UiDefinitions.Integrations;

public class ExternalApiParameterUiDefinitions : CRUDDefinition<ExternalApiParameter>
{
    private static readonly List<string> DefaultRoles =
    [
        Neo.Domain.Constants.Roles.Admin,
        HyperRoles.Manager,
        HyperRoles.Analyst
    ];

    public override List<string>? Roles => DefaultRoles;

    protected override void IndexFormViewModel()
    {
        AddColumns(
            nameof(ExternalApiParameter.ExternalApi),
            nameof(ExternalApiParameter.Name),
            nameof(ExternalApiParameter.Location),
            nameof(ExternalApiParameter.DataType),
            nameof(ExternalApiParameter.IsRequired),
            nameof(ExternalApiParameter.BindingExpression),
            nameof(ExternalApiParameter.Order)
        );

        form.AddOrderBy(nameof(ExternalApiParameter.ExternalApi));
        form.AddOrderBy(nameof(ExternalApiParameter.Order));
    }

    protected override void CUDFormsViewModel()
    {
        AddField(nameof(ExternalApiParameter.ExternalApi));
        AddField(nameof(ExternalApiParameter.ParentParameter));
        AddField(nameof(ExternalApiParameter.Name));
        AddField(nameof(ExternalApiParameter.Description), eControlTypeId.MultilineTextInput);
        AddField(nameof(ExternalApiParameter.Location));
        AddField(nameof(ExternalApiParameter.DataType));
        AddField(nameof(ExternalApiParameter.Format));
        AddField(nameof(ExternalApiParameter.IsRequired));
        AddField(nameof(ExternalApiParameter.AllowEmptyValue));
        AddField(nameof(ExternalApiParameter.Explode));
        AddField(nameof(ExternalApiParameter.Order));
        AddField(nameof(ExternalApiParameter.DefaultValue));
        AddField(nameof(ExternalApiParameter.Example));
        AddField(nameof(ExternalApiParameter.EnumValuesJson), eControlTypeId.MultilineTextInput);
        AddField(nameof(ExternalApiParameter.SchemaDefinitionJson), eControlTypeId.MultilineTextInput);
        AddField(nameof(ExternalApiParameter.BindingExpression));
        AddField(nameof(ExternalApiParameter.SourceKey));
    }
}

