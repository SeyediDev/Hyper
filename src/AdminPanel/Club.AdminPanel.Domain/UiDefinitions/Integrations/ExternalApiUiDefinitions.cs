using Neo.Domain.Entities.Integrations;

namespace Hyper.AdminPanel.Domain.UiDefinitions.Integrations;

public class ExternalApiUiDefinitions : CRUDDefinition<ExternalApi>
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
            nameof(ExternalApi.Name),
            nameof(ExternalApi.Category),
            nameof(ExternalApi.Method),
            nameof(ExternalApi.BaseUrl),
            nameof(ExternalApi.RelativePath),
            nameof(ExternalApi.RequiresAuthentication),
            nameof(ExternalApi.UseOAuth2),
            nameof(ExternalApi.OAuthGrantType)
        );

        form.AddOrderBy(nameof(ExternalApi.Name));
    }

    protected override void CUDFormsViewModel()
    {
        AddField(nameof(ExternalApi.Name));
        AddField(nameof(ExternalApi.Description), eControlTypeId.MultilineTextInput);
        AddField(nameof(ExternalApi.Category));
        AddField(nameof(ExternalApi.Version));
        AddField(nameof(ExternalApi.Method));
        AddField(nameof(ExternalApi.BaseUrl));
        AddField(nameof(ExternalApi.RelativePath));
        AddField(nameof(ExternalApi.RequiresAuthentication));
        AddField(nameof(ExternalApi.UseOAuth2));
        AddField(nameof(ExternalApi.OAuthGrantType));
        AddField(nameof(ExternalApi.TokenEndpoint));
        AddField(nameof(ExternalApi.ClientAuthenticationMethod));
        AddField(nameof(ExternalApi.ClientId));
        AddField(nameof(ExternalApi.ClientSecret));
        AddField(nameof(ExternalApi.Scope));
        AddField(nameof(ExternalApi.Resource));
        AddField(nameof(ExternalApi.Audience));
        AddField(nameof(ExternalApi.Username));
        AddField(nameof(ExternalApi.Password));
        AddField(nameof(ExternalApi.AdditionalAuthParametersJson), eControlTypeId.MultilineTextInput);
        AddField(nameof(ExternalApi.TokenLifetimeSeconds));
        AddField(nameof(ExternalApi.TokenClockSkewSeconds));
        AddField(nameof(ExternalApi.DefaultHeadersJson), eControlTypeId.MultilineTextInput);
        AddField(nameof(ExternalApi.OpenApiDocumentJson), eControlTypeId.MultilineTextInput);
    }
}

