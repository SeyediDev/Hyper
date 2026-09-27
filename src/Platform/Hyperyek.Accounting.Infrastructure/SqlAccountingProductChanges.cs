using System.Data;
using System.Globalization;
using System.Security.Cryptography;
using System.Text.Json;
using Hyper.Infrastructure.Data.Repository.Hyper;
using Hyperyek.Accounting.Contracts;
using Microsoft.EntityFrameworkCore;

namespace Hyperyek.Accounting.Infrastructure;

// This receipt belongs to the accounting owner, not to the Integration inbox.
// Keeping it in the product transaction closes the lost-HTTP-ACK replay window.
internal sealed class SqlAccountingProductChanges(HyperSqlServerContext db)
{
    public async Task<AccountingCommandResult> ApplyAsync(ExternalProductChangedCommand command, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(command);
        if (Validate(command) is { } error) return new(AccountingCommandStatus.Rejected, ErrorCode: error);
        var scope = command.Scope;
        var connectionKey = Hash(scope.ShopId, scope.TenantId, command.ConnectionId);
        var streamKey = Hash(scope.ShopId, scope.TenantId, command.ConnectionId, command.HyperProductId);
        var eventKey = Hash(command.EventId);
        var fingerprint = Hash(command.HyperProductId, command.ExternalProductId, command.ExternalVariantId,
            command.Sku, command.Title, Number(command.Price), Number(command.Inventory), command.SourceVersion);

        await using var transaction = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable, ct);
        // Same shop serialization as accounting order writes; source versions are
        // per connection/product, never compared across independent connections.
        await db.Database.ExecuteSqlInterpolatedAsync($"""
            DECLARE @result int;
            EXEC @result=sys.sp_getapplock @Resource={"accounting-orders:" + scope.ShopId},
                @LockMode='Exclusive', @LockOwner='Transaction', @LockTimeout=15000;
            IF @result < 0 THROW 51000, 'AccountingProductLockUnavailable', 1;
            """, ct);
        var shop = await db.TblShops.AsNoTracking().SingleOrDefaultAsync(x => x.Shopid == scope.ShopId, ct);
        if (shop is null || CanonicalTenant(scope.ShopId, shop.TenantId) != scope.TenantId)
            return new(AccountingCommandStatus.Rejected, ErrorCode: "AccountingShopScopeMismatch");
        var product = await db.TblProducts.AsNoTracking().SingleOrDefaultAsync(x =>
            x.Id == command.HyperProductId && x.Shopid == scope.ShopId, ct);
        if (product is null || CanonicalTenant(scope.ShopId, product.TenantId) != scope.TenantId)
            return new(AccountingCommandStatus.PendingDependency, ErrorCode: "AccountingProductNotFound");
        if (await db.Database.SqlQuery<int>($"SELECT CASE WHEN OBJECT_ID(N'dbo.AccountingProductChangeReceipts', N'U') IS NULL THEN 0 ELSE 1 END AS Value").SingleAsync(ct) == 0)
            return new(AccountingCommandStatus.PendingDependency, ErrorCode: "AccountingProductReceiptSchemaMissing");
        var prior = await db.Database.SqlQuery<ProductChangeReceipt>($"""
            SELECT HyperProductId, SourceVersion, Fingerprint FROM dbo.AccountingProductChangeReceipts
            WHERE ConnectionKey={connectionKey} AND EventKey={eventKey}
            """).SingleOrDefaultAsync(ct);
        if (prior is not null)
            return prior.HyperProductId == command.HyperProductId && prior.SourceVersion == command.SourceVersion
                && prior.Fingerprint.SequenceEqual(fingerprint)
                ? new(AccountingCommandStatus.Duplicate, product.Id.ToString(CultureInfo.InvariantCulture))
                : new(AccountingCommandStatus.Rejected, ErrorCode: "AccountingProductEventConflict");
        var last = await db.Database.SqlQuery<long>($"""
            SELECT COALESCE(MAX(SourceVersion),CONVERT(bigint,0)) AS Value
            FROM dbo.AccountingProductChangeReceipts WHERE StreamKey={streamKey}
            """).SingleAsync(ct);
        if (command.SourceVersion <= last)
            return new(AccountingCommandStatus.Rejected, ErrorCode: command.SourceVersion == last
                ? "AccountingProductVersionConflict" : "StaleAccountingProductVersion");

        // Null optional values mean absent, not clear. No marketplace stock write,
        // SKU truncation, implicit unit conversion or decimal rounding is allowed.
        await db.TblProducts.Where(x => x.Id == product.Id && x.Shopid == scope.ShopId)
            .ExecuteUpdateAsync(s => s.SetProperty(x => x.Name, command.Title)
                .SetProperty(x => x.Taxcode, x => command.Sku ?? x.Taxcode)
                .SetProperty(x => x.Saleprice, x => command.Price ?? x.Saleprice), ct);
        await db.Database.ExecuteSqlInterpolatedAsync($"""
            INSERT INTO dbo.AccountingProductChangeReceipts
                (ConnectionKey,EventKey,StreamKey,ShopId,TenantId,ConnectionId,HyperProductId,SourceVersion,Fingerprint,AppliedAtUtc)
            VALUES ({connectionKey},{eventKey},{streamKey},{scope.ShopId},{scope.TenantId},{command.ConnectionId},
                {product.Id},{command.SourceVersion},{fingerprint},{DateTime.UtcNow})
            """, ct);
        await transaction.CommitAsync(ct);
        return new(AccountingCommandStatus.Applied, product.Id.ToString(CultureInfo.InvariantCulture));
    }

    private static string? Validate(ExternalProductChangedCommand c)
    {
        if (c.Scope is not { ShopId: > 0 } || !Identity(c.Scope.TenantId, 64) || c.ConnectionId <= 0
            || c.HyperProductId <= 0 || c.SourceVersion <= 0 || !Identity(c.EventId, 128)
            || !Identity(c.ExternalProductId, 128)) return "InvalidAccountingProductIdentity";
        // The accounting schema has no variant-level product writer.
        if (c.ExternalVariantId is not null) return "AccountingProductVariantUnsupported";
        if (string.IsNullOrWhiteSpace(c.Title) || c.Title.Length > 200 || c.Title.Any(char.IsControl))
            return "InvalidAccountingProductTitle";
        if (c.Sku is not null && (!Identity(c.Sku, 13) || c.Sku.Any(x => x < 33 || x > 126)))
            return "AccountingProductSkuNeedsReview"; // existing TAXCODE_ is varchar(13)
        if (c.Price is { } price && (price < 0 || price >= 1_000_000_000_000_000_000m || decimal.Round(price, 8) != price))
            return "InvalidAccountingProductPrice";
        if (c.Inventory is < 0) return "InvalidMarketplaceInventoryObservation";
        return null;
    }

    private static bool Identity(string? value, int limit) => !string.IsNullOrWhiteSpace(value)
        && value.Length <= limit && value == value.Trim() && !value.Any(char.IsControl);
    private static string CanonicalTenant(int shopId, string? tenant) => string.IsNullOrWhiteSpace(tenant) ? $"shop:{shopId}" : tenant.Trim();
    private static string? Number(decimal? value) => value?.ToString("G29", CultureInfo.InvariantCulture);
    private static byte[] Hash(params object?[] values) => SHA256.HashData(JsonSerializer.SerializeToUtf8Bytes(values));
}

internal sealed class ProductChangeReceipt
{
    public int HyperProductId { get; set; }
    public long SourceVersion { get; set; }
    public byte[] Fingerprint { get; set; } = [];
}
