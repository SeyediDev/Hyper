using Hyper.Integration.Domain.Entities.Integrations;
using Basalam.SDK;
using Hyper.Infrastructure.Data.Repository.Hyper;
using Microsoft.EntityFrameworkCore;

namespace Hyper.Infrastructure.Features.Integrations;

public interface IBasalamWebhookRegistration
{
    Task RegisterForConnectionAsync(long connectionId, int shopId, string tenantId,
        string vendorId, string callbackBaseUri, CancellationToken ct);
}

public sealed class BasalamWebhookRegistration(HyperIntegrationContext db, BasalamOAuthStore tokens,
    IBasalamClient client, IntegrationCredentialVault credentials)
    : IBasalamWebhookRegistration
{
    // Keep registration and ingress routing on the same provider catalog.
    private static readonly int[] EventIds = BasalamWebhookEvents.All.Select(x => x.Id).ToArray();

    public async Task RegisterForConnectionAsync(long connectionId, int shopId, string tenantId,
        string vendorId, string callbackBaseUri, CancellationToken ct)
    {
        if (!Uri.TryCreate(callbackBaseUri, UriKind.Absolute, out var baseUri)
            || baseUri.Scheme != Uri.UriSchemeHttps)
            throw new InvalidOperationException("Basalam webhook callback must be HTTPS.");
        if (!long.TryParse(vendorId, out var parsedVendor) || parsedVendor <= 0)
            throw new InvalidOperationException("Basalam vendor identifier is invalid.");

        // Validate persisted ordinal scope and credentials before GetTokenAsync,
        // because token retrieval may initiate a remote refresh.
        var stored = await db.ExternalIntegrationConnections.AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == connectionId, ct);
        if (stored is null || stored.ShopId != shopId || stored.TenantId != tenantId
            || stored.Provider != IntegrationProvider.Basalam || !stored.IsEnabled
            || stored.CredentialType != IntegrationCredentialType.OAuth2 || stored.AccountIdentifier != vendorId)
            throw new IntegrationCredentialException("IntegrationCredentialsScopeInvalid");
        var document = credentials.Read(stored);
        if (document.WebhookSecret is not { } secret)
            throw new IntegrationCredentialException("IntegrationWebhookSecretMissing");
        var token = await tokens.GetTokenAsync(stored, ct)
            ?? throw new InvalidOperationException("Basalam token is unavailable after OAuth.");
        client.SetToken(token);
        var callback = new Uri(baseUri, $"/api/integrations/v1/webhooks/basalam/{parsedVendor}");
        await client.Webhooks.CreateOfficialWebhookAsync(callback.AbsoluteUri, EventIds,
            $"Bearer {secret}", ct);
    }
}
