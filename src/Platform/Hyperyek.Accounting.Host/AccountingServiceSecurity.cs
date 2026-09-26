using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Neo.Infrastructure.Features.Client;

namespace Hyperyek.Accounting.Host;

public static class AccountingServiceSecurity
{
    public static IServiceCollection AddAccountingServiceSecurity(this IServiceCollection services, IConfiguration configuration)
    {
        var authority = configuration["IdpSetting:Authority"];
        var audience = configuration["IdpSetting:ClientId"];
        var clients = configuration.GetSection("AccountingApiSecurity:AllowedClientIds").Get<string[]>() ?? [];
        var scope = configuration["AccountingApiSecurity:RequiredScope"] ?? "hyperyek.accounting";
        if (!string.Equals(configuration["IdpSetting:TokenProvider"], "Keycloak", StringComparison.OrdinalIgnoreCase)
            || !Uri.TryCreate(authority, UriKind.Absolute, out var issuer) || issuer.Scheme != Uri.UriSchemeHttps
            || !string.IsNullOrEmpty(issuer.UserInfo) || !string.IsNullOrEmpty(issuer.Query) || !string.IsNullOrEmpty(issuer.Fragment)
            || string.IsNullOrWhiteSpace(audience) || clients.Length == 0 || clients.Any(string.IsNullOrWhiteSpace)
            || string.IsNullOrWhiteSpace(scope) || scope.Any(char.IsWhiteSpace))
            throw new InvalidOperationException("AccountingAuthenticationConfigurationInvalid");

        services.AddNeoAuthentication(configuration);
        services.PostConfigure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, options =>
        {
            options.RequireHttpsMetadata = true;
            options.MapInboundClaims = false;
            options.IncludeErrorDetails = false;
            options.TokenValidationParameters.ClockSkew = TimeSpan.FromSeconds(30);
            options.TokenValidationParameters.ValidAlgorithms = ["RS256", "PS256", "ES256"];
            // Service endpoints do not need Neo's user/realm role transformation,
            // query-string tokens or verbose authentication exception logging.
            options.Events = new JwtBearerEvents();
        });
        services.AddAuthorization(options =>
        {
            options.DefaultPolicy = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme)
                .RequireAuthenticatedUser().RequireAssertion(context =>
                {
                    var ids = context.User.FindAll("azp").Concat(context.User.FindAll("client_id")).Select(x => x.Value).ToArray();
                    return ids.Length > 0 && ids.Distinct(StringComparer.Ordinal).Count() == 1
                        && clients.Contains(ids[0], StringComparer.Ordinal)
                        && context.User.FindAll("scope").Any(x => x.Value.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                            .Contains(scope, StringComparer.Ordinal));
                }).Build();
            options.FallbackPolicy = options.DefaultPolicy;
        });
        return services;
    }
}
