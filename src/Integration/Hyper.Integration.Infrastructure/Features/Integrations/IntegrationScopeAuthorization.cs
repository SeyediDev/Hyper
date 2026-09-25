using Hyper.Integration.Contracts;
using System.Security.Claims;

namespace Hyper.Infrastructure.Features.Integrations;

/// <summary>
/// Accepts only claims issued for Integration scope. User-supplied shop/tenant
/// headers are never treated as authorization by themselves.
/// </summary>
public sealed class IntegrationScopeAuthorization : IIntegrationScopeAuthorization
{
    public Task<bool> CanAccessAsync(ClaimsPrincipal principal, int shopId, string tenantId,
        CancellationToken cancellationToken = default)
    {
        if (shopId <= 0 || string.IsNullOrWhiteSpace(tenantId) || tenantId.Length > 128
            || principal.Identity?.IsAuthenticated != true)
            return Task.FromResult(false);
        var expected = $"shop:{shopId};tenant:{tenantId}";
        var allowed = principal.FindAll("integration_scope").Concat(principal.FindAll("scope"))
            .Any(x => string.Equals(x.Value, expected, StringComparison.Ordinal));
        return Task.FromResult(allowed);
    }
}
