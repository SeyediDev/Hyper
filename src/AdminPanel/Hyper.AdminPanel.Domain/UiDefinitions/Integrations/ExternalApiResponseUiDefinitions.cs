using Neo.Domain.Entities.Integrations;

namespace Hyper.AdminPanel.Domain.UiDefinitions.Integrations;

public class ExternalApiResponseUiDefinitions : CRUDDefinition<ExternalApiResponse>
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
            nameof(ExternalApiResponse.ExternalApi),
            nameof(ExternalApiResponse.StatusCode),
            nameof(ExternalApiResponse.MediaType),
            nameof(ExternalApiResponse.DataType),
            nameof(ExternalApiResponse.Description)
        );

        form.AddOrderBy(nameof(ExternalApiResponse.ExternalApi));
        form.AddOrderBy(nameof(ExternalApiResponse.StatusCode));
    }

    protected override void CUDFormsViewModel()
    {
        AddField(nameof(ExternalApiResponse.ExternalApi));
        AddField(nameof(ExternalApiResponse.StatusCode));
        AddField(nameof(ExternalApiResponse.Description), eControlTypeId.MultilineTextInput);
        AddField(nameof(ExternalApiResponse.MediaType));
        AddField(nameof(ExternalApiResponse.DataType));
        AddField(nameof(ExternalApiResponse.Format));
        AddField(nameof(ExternalApiResponse.ExampleJson), eControlTypeId.MultilineTextInput);
        AddField(nameof(ExternalApiResponse.SchemaDefinitionJson), eControlTypeId.MultilineTextInput);
        AddField(nameof(ExternalApiResponse.HeadersJson), eControlTypeId.MultilineTextInput);
    }
}

