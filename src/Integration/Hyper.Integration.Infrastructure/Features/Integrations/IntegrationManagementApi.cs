using Hyper.Integration.Contracts;
using Hyper.Integration.Domain.Entities.Integrations;
using Hyper.Infrastructure.Data.Repository.Hyper;
using Microsoft.EntityFrameworkCore;

namespace Hyper.Infrastructure.Features.Integrations;

/// <summary>Application boundary for the versioned Integration management API.</summary>
public sealed class IntegrationManagementApi(HyperIntegrationContext db) : IIntegrationManagementApi
{
    public async Task<IReadOnlyList<IntegrationConnectionSummary>> ListConnectionsAsync(
        IntegrationConnectionListRequest request, CancellationToken cancellationToken = default) =>
        await db.ExternalIntegrationConnections.AsNoTracking()
            .Where(x => x.ShopId == request.ShopId && x.TenantId == request.TenantId)
            .OrderBy(x => x.Provider).ThenBy(x => x.DisplayName)
            .Select(x => new IntegrationConnectionSummary(x.Id, x.ShopId, x.TenantId,
                ToContractProvider(x.Provider), x.DisplayName, x.IsEnabled, x.ExpiresAtUtc, x.LastSyncAtUtc))
            .ToListAsync(cancellationToken);

    public async Task<IntegrationConnectionResponse?> CreateConnectionAsync(
        IntegrationConnectionCreateRequest request, CancellationToken cancellationToken = default)
    {
        if (request.ShopId <= 0 || string.IsNullOrWhiteSpace(request.TenantId) || request.TenantId.Length > 30
            || string.IsNullOrWhiteSpace(request.DisplayName) || request.DisplayName.Length > 200
            || string.IsNullOrWhiteSpace(request.AccountIdentifier) || request.AccountIdentifier.Length > 200
            || !Enum.IsDefined(request.Provider) || !Enum.IsDefined(typeof(IntegrationCredentialType), request.CredentialType))
            return null;
        var provider = ToDomainProvider(request.Provider);
        if (provider is null) return null;
        var exists = await db.ExternalIntegrationConnections.AnyAsync(x =>
            x.ShopId == request.ShopId && x.Provider == provider.Value
            && x.AccountIdentifier == request.AccountIdentifier, cancellationToken);
        if (exists) return null;
        var connection = new ExternalIntegrationConnection
        {
            ShopId = request.ShopId,
            TenantId = request.TenantId.Trim(),
            Provider = provider.Value,
            DisplayName = request.DisplayName.Trim(),
            AccountIdentifier = request.AccountIdentifier.Trim(),
            CredentialType = (IntegrationCredentialType)request.CredentialType,
            // Credentials are provisioned by OAuth/vault flows, never through this DTO.
            CredentialsJson = "{}",
            IsEnabled = false
        };
        db.ExternalIntegrationConnections.Add(connection);
        try { await db.SaveChangesAsync(cancellationToken); }
        catch (DbUpdateException) { return null; }
        return new(ToSummary(connection));
    }

    public async Task<bool> SetConnectionEnabledAsync(IntegrationConnectionCommandRequest request, bool enabled,
        CancellationToken cancellationToken = default)
    {
        var changed = await db.ExternalIntegrationConnections
            .Where(x => x.Id == request.ConnectionId && x.ShopId == request.ShopId && x.TenantId == request.TenantId)
            .ExecuteUpdateAsync(s => s.SetProperty(x => x.IsEnabled, enabled), cancellationToken);
        return changed == 1;
    }

    public async Task<IntegrationReplayResponse?> ReplayWebhookAsync(IntegrationConnectionCommandRequest request,
        long inboxId, CancellationToken cancellationToken = default)
    {
        if (inboxId <= 0) return null;
        var changed = await db.IntegrationWebhookInbox
            .Where(x => x.Id == inboxId && x.ConnectionId == request.ConnectionId
                && db.ExternalIntegrationConnections.Any(c => c.Id == x.ConnectionId
                    && c.ShopId == request.ShopId && c.TenantId == request.TenantId))
            .ExecuteUpdateAsync(s => s.SetProperty(x => x.Status, (byte)0)
                .SetProperty(x => x.Error, (string?)null).SetProperty(x => x.ProcessedAtUtc, (DateTime?)null), cancellationToken);
        return changed == 1 ? new IntegrationReplayResponse(inboxId, "Queued") : null;
    }

    private static IntegrationConnectionSummary ToSummary(ExternalIntegrationConnection x) =>
        new(x.Id, x.ShopId, x.TenantId, ToContractProvider(x.Provider), x.DisplayName,
            x.IsEnabled, x.ExpiresAtUtc, x.LastSyncAtUtc);

    private static Hyper.Integration.Domain.Entities.Integrations.IntegrationProvider? ToDomainProvider(
        Hyper.Integration.Contracts.IntegrationProvider provider) => provider switch
        {
            Hyper.Integration.Contracts.IntegrationProvider.Basalam => Hyper.Integration.Domain.Entities.Integrations.IntegrationProvider.Basalam,
            Hyper.Integration.Contracts.IntegrationProvider.Digikala => Hyper.Integration.Domain.Entities.Integrations.IntegrationProvider.Digikala,
            Hyper.Integration.Contracts.IntegrationProvider.Torob => Hyper.Integration.Domain.Entities.Integrations.IntegrationProvider.Torob,
            Hyper.Integration.Contracts.IntegrationProvider.Custom => Hyper.Integration.Domain.Entities.Integrations.IntegrationProvider.Custom,
            _ => null
        };

    private static Hyper.Integration.Contracts.IntegrationProvider ToContractProvider(
        Hyper.Integration.Domain.Entities.Integrations.IntegrationProvider provider) => provider switch
    {
        Hyper.Integration.Domain.Entities.Integrations.IntegrationProvider.Basalam => Hyper.Integration.Contracts.IntegrationProvider.Basalam,
        Hyper.Integration.Domain.Entities.Integrations.IntegrationProvider.Digikala => Hyper.Integration.Contracts.IntegrationProvider.Digikala,
        Hyper.Integration.Domain.Entities.Integrations.IntegrationProvider.Torob => Hyper.Integration.Contracts.IntegrationProvider.Torob,
        _ => Hyper.Integration.Contracts.IntegrationProvider.Custom
    };
}
