using System.Globalization;
using System.Linq.Expressions;
using System.Security.Claims;

namespace Hyper.Integration.Domain.Features.Integrations;

public interface IIntegrationActor
{
    IntegrationMerchantIdentity? Identity { get; }
}

public sealed record IntegrationMerchantIdentity(string Issuer, string SubjectId);

public sealed record OwnedIntegrationShop(int ShopId, string TenantId);

public interface IIntegrationShopAccess
{
    Task<IReadOnlyList<OwnedIntegrationShop>> GetOwnedShopsAsync(IntegrationMerchantIdentity identity, CancellationToken cancellationToken);
}

public static class IntegrationMerchantClaims
{
    // This is an external account identity, never the administrator User table or its userid claim.
    public static IntegrationMerchantIdentity? Read(ClaimsPrincipal principal, string? trustedIssuer, string claimType = "sub")
    {
        if (!Uri.TryCreate(trustedIssuer, UriKind.Absolute, out var issuer) || issuer.Scheme != Uri.UriSchemeHttps
            || string.IsNullOrWhiteSpace(claimType)) return null;
        var authenticated = principal.Identities.Where(i => i.IsAuthenticated).SelectMany(i => i.Claims).ToArray();
        var claims = authenticated.Where(c => c.Type == claimType && c.Issuer == trustedIssuer).ToArray();
        // JWT middleware may map sub to NameIdentifier. Neo's additional userid claim has LOCAL AUTHORITY issuer.
        if (claims.Length == 0 && claimType == "sub")
            claims = authenticated.Where(c => c.Type == ClaimTypes.NameIdentifier && c.Issuer == trustedIssuer).ToArray();
        if (claims.Length != 1) return null;
        var value = claims[0].Value;
        return string.IsNullOrWhiteSpace(value) || value.Length > 128 || value != value.Trim() || value.Any(char.IsControl)
            || trustedIssuer!.Length > 300 ? null : new IntegrationMerchantIdentity(trustedIssuer, value);
    }
}

public static class IntegrationConnectionScope
{
    public static string CanonicalTenant(int shopId, string? databaseTenant) =>
        string.IsNullOrWhiteSpace(databaseTenant) ? "shop:" + shopId.ToString(CultureInfo.InvariantCulture) : databaseTenant;

    public static Expression<Func<ExternalIntegrationConnection, bool>> For(IReadOnlyList<OwnedIntegrationShop> shops)
    {
        var connection = Expression.Parameter(typeof(ExternalIntegrationConnection), "connection");
        Expression allowed = Expression.Constant(false);
        foreach (var shop in shops)
        {
            if (shop.ShopId <= 0 || string.IsNullOrWhiteSpace(shop.TenantId)) continue;
            var shopMatch = Expression.Equal(Expression.Property(connection, nameof(ExternalIntegrationConnection.ShopId)), Expression.Constant(shop.ShopId));
            var tenantMatch = Expression.Equal(Expression.Property(connection, nameof(ExternalIntegrationConnection.TenantId)), Expression.Constant(shop.TenantId));
            allowed = Expression.OrElse(allowed, Expression.AndAlso(shopMatch, tenantMatch));
        }
        return Expression.Lambda<Func<ExternalIntegrationConnection, bool>>(allowed, connection);
    }
}

