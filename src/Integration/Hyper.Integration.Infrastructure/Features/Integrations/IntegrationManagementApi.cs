using Hyper.Integration.Contracts;
using Hyper.Integration.Domain.Entities.Integrations;
using Hyper.Infrastructure.Data.Repository.Hyper;
using Microsoft.EntityFrameworkCore;

namespace Hyper.Infrastructure.Features.Integrations;

/// <summary>Application boundary for the versioned Integration management API.</summary>
public sealed class IntegrationManagementApi(HyperIntegrationContext db, IntegrationCredentialVault credentials) : IIntegrationManagementApi
{
    public async Task<IReadOnlyList<IntegrationConnectionSummary>> ListConnectionsAsync(
        IntegrationConnectionListRequest request, CancellationToken cancellationToken = default)
    {
        var tenantId = NormalizeTenant(request.TenantId);
        if (request.ShopId <= 0 || tenantId is null) return [];

        return await db.ExternalIntegrationConnections.AsNoTracking()
            .Where(x => x.ShopId == request.ShopId && x.TenantId == tenantId)
            .OrderBy(x => x.Provider).ThenBy(x => x.DisplayName)
            .Select(x => new IntegrationConnectionSummary(x.Id, x.ShopId, x.TenantId,
                ToContractProvider(x.Provider), x.DisplayName, x.IsEnabled, x.ExpiresAtUtc, x.LastSyncAtUtc))
            .ToListAsync(cancellationToken);
    }

    public async Task<IntegrationConnectionResponse?> CreateConnectionAsync(
        IntegrationConnectionCreateRequest request, CancellationToken cancellationToken = default)
    {
        var tenantId = NormalizeTenant(request.TenantId);
        var displayName = NormalizeText(request.DisplayName, 200);
        var accountIdentifier = NormalizeText(request.AccountIdentifier, 200);
        if (request.ShopId <= 0 || tenantId is null || displayName is null || accountIdentifier is null
            || !Enum.IsDefined(request.Provider) || !Enum.IsDefined(typeof(IntegrationCredentialType), request.CredentialType))
            return null;
        var provider = ToDomainProvider(request.Provider);
        if (provider is null) return null;
        var exists = await db.ExternalIntegrationConnections.AnyAsync(x =>
            x.ShopId == request.ShopId && x.TenantId == tenantId && x.Provider == provider.Value
            && x.AccountIdentifier == accountIdentifier,
            cancellationToken);
        if (exists) return null;
        var connection = new ExternalIntegrationConnection
        {
            ShopId = request.ShopId,
            TenantId = tenantId,
            Provider = provider.Value,
            DisplayName = displayName,
            AccountIdentifier = accountIdentifier,
            CredentialType = (IntegrationCredentialType)request.CredentialType,
            // Credentials are provisioned by OAuth/vault flows, never through this DTO.
            CredentialsJson = "{}",
            IsEnabled = false
        };
        await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);
        db.ExternalIntegrationConnections.Add(connection);
        try
        {
            await db.SaveChangesAsync(cancellationToken);
            connection.CredentialsJson = credentials.Protect(connection, "{}");
            await db.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
        catch (DbUpdateException) { return null; }
        return new(ToSummary(connection));
    }

    public async Task<bool> SetConnectionEnabledAsync(IntegrationConnectionCommandRequest request, bool enabled,
        CancellationToken cancellationToken = default)
    {
        var tenantId = NormalizeTenant(request.TenantId);
        if (request.ShopId <= 0 || request.ConnectionId <= 0 || tenantId is null) return false;
        var changed = await db.ExternalIntegrationConnections
            .Where(x => x.Id == request.ConnectionId && x.ShopId == request.ShopId && x.TenantId == tenantId)
            .ExecuteUpdateAsync(s => s.SetProperty(x => x.IsEnabled, enabled), cancellationToken);
        return changed == 1;
    }

    public async Task<IntegrationReplayResponse?> ReplayWebhookAsync(IntegrationConnectionCommandRequest request,
        long inboxId, CancellationToken cancellationToken = default)
    {
        var tenantId = NormalizeTenant(request.TenantId);
        if (inboxId <= 0 || request.ShopId <= 0 || request.ConnectionId <= 0 || tenantId is null) return null;
        await using var transaction = await db.Database.BeginTransactionAsync(cancellationToken);
        var inbox = await db.IntegrationWebhookInbox.AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == inboxId && x.ConnectionId == request.ConnectionId
                && db.ExternalIntegrationConnections.Any(c => c.Id == x.ConnectionId
                    && c.ShopId == request.ShopId && c.TenantId == tenantId && c.IsEnabled), cancellationToken);
        if (inbox is null) return null;
        var job = await db.IntegrationScenarioJobs.AsNoTracking().SingleOrDefaultAsync(x =>
            x.ConnectionId == request.ConnectionId && x.ShopId == request.ShopId && x.TenantId == tenantId
            && x.EventId == inbox.ExternalEventId, cancellationToken);
        // Echoes/unsupported notifications have no job. Do not leave their Inbox pending forever.
        if (job is null) return new(inboxId, "NotReplayable");
        if (job.Status == IntegrationScenarioStatus.Completed) return new(inboxId, "AlreadyCompleted");
        if (job.Status == IntegrationScenarioStatus.Running) return new(inboxId, "Running");
        if (job.Status == IntegrationScenarioStatus.Pending) return new(inboxId, "AlreadyQueued");
        var changed = await db.IntegrationScenarioJobs.Where(x => x.Id == job.Id
                && (x.Status == IntegrationScenarioStatus.DeadLetter || x.Status == IntegrationScenarioStatus.NeedsAttention))
            .ExecuteUpdateAsync(s => s.SetProperty(x => x.Status, IntegrationScenarioStatus.Pending)
                .SetProperty(x => x.Attempts, 0).SetProperty(x => x.NextAttemptAtUtc, DateTime.UtcNow)
                .SetProperty(x => x.ErrorCode, (string?)null).SetProperty(x => x.ResultJson, (string?)null)
                .SetProperty(x => x.CompletedAtUtc, (DateTime?)null).SetProperty(x => x.LeaseId, (Guid?)null)
                .SetProperty(x => x.LeaseExpiresAtUtc, (DateTime?)null), cancellationToken);
        if (changed != 1) return new(inboxId, "ReplayConflict");
        changed = await db.IntegrationWebhookInbox.Where(x => x.Id == inbox.Id && x.ConnectionId == job.ConnectionId)
            .ExecuteUpdateAsync(s => s.SetProperty(x => x.Status, (byte)0)
                .SetProperty(x => x.Error, (string?)null).SetProperty(x => x.ProcessedAtUtc, (DateTime?)null), cancellationToken);
        if (changed != 1) return null;
        await transaction.CommitAsync(cancellationToken);
        return new(inboxId, "Queued");
    }

    private static string? NormalizeTenant(string? value) => NormalizeText(value, 30);

    private static string? NormalizeText(string? value, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value)) return null;
        var normalized = value.Trim();
        return normalized.Length == 0 || normalized.Length > maxLength || normalized.Any(char.IsControl)
            ? null : normalized;
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
