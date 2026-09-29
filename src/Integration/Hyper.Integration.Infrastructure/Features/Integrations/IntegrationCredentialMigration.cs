using Hyper.Infrastructure.Data.Repository.Hyper;
using Microsoft.EntityFrameworkCore;

namespace Hyper.Infrastructure.Features.Integrations;

public enum IntegrationCredentialMigrationResult { Migrated, AlreadyProtected, Conflict }

// No startup hook, public HTTP endpoint or authentication side effect invokes
// this service. Operators explicitly prepare/apply a single selected row.
public sealed class IntegrationCredentialMigration(HyperIntegrationContext db, IntegrationCredentialVault vault)
{
    public async Task<IntegrationCredentialMigrationPlan> PrepareAsync(OwnedIntegrationShop shop,
        long connectionId, CancellationToken ct)
    {
        var connection = await db.ExternalIntegrationConnections.AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == connectionId, ct);
        if (connection is null || connection.ShopId != shop.ShopId || connection.TenantId != shop.TenantId)
            throw new IntegrationCredentialException("IntegrationCredentialsScopeInvalid");
        if (connection.CredentialsJson.StartsWith(IntegrationCredentialVault.ReservedPrefix, StringComparison.Ordinal))
        {
            vault.Read(connection); // Unknown version or decryption failure is never legacy.
            return new(connection, connection.CredentialsJson, true);
        }
        return new(connection, vault.ProtectLegacy(connection), false);
    }

    public async Task<IntegrationCredentialMigrationResult> ApplyAsync(IntegrationCredentialMigrationPlan plan,
        CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(plan);
        await using var transaction = await db.Database.BeginTransactionAsync(ct);
        if (!plan.IsAlreadyProtected)
        {
            // CredentialsJson and scope strings use CI_AS in deployed SQL. Both
            // binary bytes and byte length are required: SQL text equality alone
            // can ignore case/trailing spaces and overwrite a concurrent edit.
            var changed = await db.Database.ExecuteSqlInterpolatedAsync($"""
                UPDATE dbo.ExternalIntegrationConnections
                SET CredentialsJson = {plan.Replacement}
                WHERE Id = {plan.ConnectionId} AND ShopId = {plan.ShopId} AND Provider = {plan.Provider}
                  AND CredentialType = {plan.CredentialType}
                  AND CONVERT(varbinary(max), TenantId) = CONVERT(varbinary(max), {plan.TenantId})
                  AND DATALENGTH(TenantId) = DATALENGTH({plan.TenantId})
                  AND CONVERT(varbinary(max), AccountIdentifier) = CONVERT(varbinary(max), {plan.AccountIdentifier})
                  AND DATALENGTH(AccountIdentifier) = DATALENGTH({plan.AccountIdentifier})
                  AND CONVERT(varbinary(max), CredentialsJson) = CONVERT(varbinary(max), {plan.Expected})
                  AND DATALENGTH(CredentialsJson) = DATALENGTH({plan.Expected})
                """, ct);
            if (changed == 1)
            {
                await transaction.CommitAsync(ct);
                return IntegrationCredentialMigrationResult.Migrated;
            }
        }
        var current = await db.ExternalIntegrationConnections.AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == plan.ConnectionId, ct);
        var already = current is not null && plan.MatchesScope(current)
            && string.Equals(current.CredentialsJson, plan.Replacement, StringComparison.Ordinal);
        await transaction.CommitAsync(ct);
        return already ? IntegrationCredentialMigrationResult.AlreadyProtected : IntegrationCredentialMigrationResult.Conflict;
    }
}

// Secret-bearing snapshots are internal and immutable; no record-generated
// ToString, public JSON properties or logs may expose original/cipher documents.
public sealed class IntegrationCredentialMigrationPlan
{
    internal IntegrationCredentialMigrationPlan(ExternalIntegrationConnection connection, string replacement, bool already)
    {
        ConnectionId = connection.Id; ShopId = connection.ShopId; TenantId = connection.TenantId;
        Provider = connection.Provider; CredentialType = connection.CredentialType;
        AccountIdentifier = connection.AccountIdentifier; Expected = connection.CredentialsJson;
        Replacement = replacement; IsAlreadyProtected = already;
    }
    public long ConnectionId { get; }
    public bool IsAlreadyProtected { get; }
    internal int ShopId { get; }
    internal string TenantId { get; }
    internal IntegrationProvider Provider { get; }
    internal IntegrationCredentialType CredentialType { get; }
    internal string AccountIdentifier { get; }
    internal string Expected { get; }
    internal string Replacement { get; }
    internal bool MatchesScope(ExternalIntegrationConnection connection) =>
        connection.Id == ConnectionId && connection.ShopId == ShopId && connection.TenantId == TenantId
        && connection.Provider == Provider && connection.CredentialType == CredentialType
        && connection.AccountIdentifier == AccountIdentifier;
    public override string ToString() => nameof(IntegrationCredentialMigrationPlan);
}
