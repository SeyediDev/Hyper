using System.Data;
using System.Globalization;
using Hyper.Infrastructure.Data.Repository.Hyper;
using Microsoft.EntityFrameworkCore;

namespace Hyper.Infrastructure.Features.Integrations;

/// <summary>The local receipt boundary used by sale orchestration.</summary>
public interface IIntegrationOrderMappingReceipt
{
    Task PreflightAsync(ExternalIntegrationConnection connection, IntegrationVendorOrderCommand command, CancellationToken ct);
    Task RecordAsync(ExternalIntegrationConnection connection, IntegrationVendorOrderCommand command,
        BusinessCommandResult result, CancellationToken ct);
}

/// <summary>Persists the accounting owner's acknowledged order identity in Integration.</summary>
public sealed class IntegrationOrderMappingReceipt(HyperIntegrationContext db) : IIntegrationOrderMappingReceipt
{
    public async Task PreflightAsync(
        ExternalIntegrationConnection connection, IntegrationVendorOrderCommand command, CancellationToken ct)
    {
        ValidateIdentity(connection, command);
        var stored = await db.ExternalIntegrationConnections.AsNoTracking()
            .SingleOrDefaultAsync(x => x.Id == command.ConnectionId, ct);
        ValidateScope(stored, connection, command);
        ValidateExisting(await FindAsync(db, command, ct), command, null);
    }

    public async Task RecordAsync(
        ExternalIntegrationConnection connection, IntegrationVendorOrderCommand command,
        BusinessCommandResult result, CancellationToken ct)
    {
        if (result.Status is not (BusinessCommandStatus.Applied or BusinessCommandStatus.Duplicate)) return;
        ValidateIdentity(connection, command);
        if (result.InternalReference is not { } reference
            || !long.TryParse(reference, NumberStyles.None, CultureInfo.InvariantCulture, out var invoiceId)
            || invoiceId <= 0 || reference != invoiceId.ToString(CultureInfo.InvariantCulture))
            throw Failure("AccountingInvoiceReferenceInvalid");

        // Accounting already committed independently. Never keep this transaction
        // open across HTTP or nest the reservation service's own transaction.
        await using var transaction = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable, ct);
        var stored = await db.ExternalIntegrationConnections.FromSqlInterpolated($"""
            SELECT * FROM dbo.ExternalIntegrationConnections WITH (UPDLOCK, HOLDLOCK)
            WHERE Id = {command.ConnectionId}
            """).AsNoTracking().SingleOrDefaultAsync(ct);
        ValidateScope(stored, connection, command);
        var existing = await FindAsync(db, command, ct);
        ValidateExisting(existing, command, invoiceId);
        var now = DateTime.UtcNow;
        if (existing is null)
        {
            // ExternalParcelId is deliberately null: a webhook's parcel ID does
            // not prove that the parcel represents this complete invoice.
            await db.Database.ExecuteSqlInterpolatedAsync($"""
                INSERT INTO dbo.ExternalOrderMappings
                    (ConnectionId, ShopId, ExternalOrderId, HyperSaleOrderId, ExternalParcelId, Status, LastSyncAtUtc)
                VALUES ({command.ConnectionId}, {command.ShopId}, {command.ExternalOrderId}, {invoiceId}, NULL, 0, {now})
                """, ct);
        }
        else
        {
            // Only receipt fields change. Preserve trusted parcel bindings and
            // status, and never flush unrelated edits tracked by this DbContext.
            await db.ExternalOrderMappings.Where(x => x.Id == existing.Id)
                .ExecuteUpdateAsync(s => s.SetProperty(x => x.HyperSaleOrderId, invoiceId)
                    .SetProperty(x => x.LastSyncAtUtc, now), ct);
        }
        await transaction.CommitAsync(ct);
    }

    private static Task<ExternalOrderMapping?> FindAsync(HyperIntegrationContext db,
        IntegrationVendorOrderCommand command, CancellationToken ct) =>
        db.ExternalOrderMappings.AsNoTracking().SingleOrDefaultAsync(x =>
            x.ConnectionId == command.ConnectionId && x.ExternalOrderId == command.ExternalOrderId, ct);

    private static void ValidateIdentity(ExternalIntegrationConnection connection, IntegrationVendorOrderCommand command)
    {
        if (command.ConnectionId <= 0 || command.ShopId <= 0
            || !Identity(command.TenantId, 30) || !Identity(command.ExternalOrderId, 128)
            || connection.Id != command.ConnectionId || connection.ShopId != command.ShopId
            || !string.Equals(connection.TenantId, command.TenantId, StringComparison.Ordinal))
            throw Failure("OrderMappingScopeMismatch");
    }

    private static void ValidateScope(ExternalIntegrationConnection? stored,
        ExternalIntegrationConnection connection, IntegrationVendorOrderCommand command)
    {
        // Query by numeric ID first: SQL's tenant/order collation is not the
        // ordinal identity used by accounting markers and reservation keys.
        if (stored is null || !stored.IsEnabled || !connection.IsEnabled
            || stored.ShopId != command.ShopId || stored.Provider != connection.Provider
            || stored.CredentialType != connection.CredentialType
            || !string.Equals(stored.TenantId, command.TenantId, StringComparison.Ordinal)
            || !string.Equals(stored.AccountIdentifier, connection.AccountIdentifier, StringComparison.Ordinal))
            throw Failure("OrderMappingScopeMismatch");
    }

    private static void ValidateExisting(ExternalOrderMapping? existing,
        IntegrationVendorOrderCommand command, long? invoiceId)
    {
        if (existing is null) return;
        if (existing.ShopId != command.ShopId
            || !string.Equals(existing.ExternalOrderId, command.ExternalOrderId, StringComparison.Ordinal))
            throw Failure("OrderMappingIdentityConflict");
        if (existing.HyperSaleOrderId is <= 0
            || invoiceId.HasValue && existing.HyperSaleOrderId.HasValue && existing.HyperSaleOrderId != invoiceId)
            throw Failure("OrderMappingInvoiceConflict");
        if (existing.ExternalParcelId is not null && command.ExternalParcelId is not null
            && !string.Equals(existing.ExternalParcelId, command.ExternalParcelId, StringComparison.Ordinal))
            throw Failure("OrderMappingParcelConflict");
    }

    private static bool Identity(string? value, int limit) => !string.IsNullOrWhiteSpace(value)
        && value.Length <= limit && value == value.Trim() && !value.Any(char.IsControl);

    private static IntegrationProviderException Failure(string code) => new(code, false);
}
