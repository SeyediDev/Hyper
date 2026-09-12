using Hyper.Domain.Features.Integrations;

namespace Hyper.CustomerPortal.Api;

public sealed class IntegrationActor(IHttpContextAccessor httpContext, IConfiguration configuration) : IIntegrationActor
{
    public IntegrationMerchantIdentity? Identity => httpContext.HttpContext is { } context
        ? IntegrationMerchantClaims.Read(context.User,
            configuration["IntegrationAccess:TrustedIssuer"] ?? configuration["IdpSetting:Authority"],
            configuration["IntegrationAccess:SubjectClaimType"] ?? "sub")
        : null;
}
