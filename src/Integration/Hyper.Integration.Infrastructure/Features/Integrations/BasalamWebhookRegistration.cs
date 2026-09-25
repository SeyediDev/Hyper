using Hyper.Integration.Domain.Entities.Integrations;
using Hyper.SDK;
using Hyper.SDK.Auth;
using Microsoft.Extensions.Options;
using System.Text.Json;
using Hyper.Infrastructure.Data.Repository.Hyper;
using Microsoft.EntityFrameworkCore;

namespace Hyper.Infrastructure.Features.Integrations;

public interface IBasalamWebhookRegistration
{
    Task RegisterForConnectionAsync(long connectionId, int shopId, string tenantId,
        string vendorId, string callbackBaseUri, CancellationToken ct);
}

public sealed class BasalamWebhookRegistration(HyperIntegrationContext db, BasalamOAuthStore tokens, IBasalamClient client)
    : IBasalamWebhookRegistration
{
    // Official Basalam event ids: vendor order, vendor parcel and product changes.
    private static readonly int[] EventIds = [1, 2, 3, 4, 5, 6, 7, 8, 9];

    public async Task RegisterForConnectionAsync(long connectionId, int shopId, string tenantId,
        string vendorId, string callbackBaseUri, CancellationToken ct)
    {
        if (!Uri.TryCreate(callbackBaseUri, UriKind.Absolute, out var baseUri)
            || baseUri.Scheme != Uri.UriSchemeHttps)
            throw new InvalidOperationException("Basalam webhook callback must be HTTPS.");
        if (!long.TryParse(vendorId, out var parsedVendor) || parsedVendor <= 0)
            throw new InvalidOperationException("Basalam vendor identifier is invalid.");

        var connection = new ExternalIntegrationConnection
        {
            Id = connectionId, ShopId = shopId, TenantId = tenantId,
            Provider = IntegrationProvider.Basalam, AccountIdentifier = vendorId,
            CredentialType = IntegrationCredentialType.OAuth2, IsEnabled = true
        };
        var token = await tokens.GetTokenAsync(connection, ct)
            ?? throw new InvalidOperationException("Basalam token is unavailable after OAuth.");
        client.SetToken(token);
        var callback = new Uri(baseUri, $"/api/integrations/v1/webhooks/basalam/{parsedVendor}");
        var stored = await db.ExternalIntegrationConnections.AsNoTracking()
            .SingleAsync(x => x.Id == connectionId && x.ShopId == shopId && x.TenantId == tenantId, ct);
        using var credentials = JsonDocument.Parse(stored.CredentialsJson);
        if (!credentials.RootElement.TryGetProperty("webhookSecret", out var secret)
            || secret.ValueKind != JsonValueKind.String || string.IsNullOrWhiteSpace(secret.GetString()))
            throw new InvalidOperationException("Basalam webhook authorization is not configured for connection.");
        await client.Webhooks.CreateOfficialWebhookAsync(callback.AbsoluteUri, EventIds,
            $"Bearer {secret.GetString()}", ct);
    }
}
