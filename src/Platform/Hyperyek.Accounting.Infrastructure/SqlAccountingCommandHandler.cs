using Hyper.Domain.Entities.Database;
using Hyper.Infrastructure.Data.Repository.Hyper;
using Hyperyek.Accounting.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Data;

namespace Hyperyek.Accounting.Infrastructure;

public static class AccountingInfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddHyperyekSqlAccounting(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<HyperSqlServerContext>(options => options.UseSqlServer(connectionString));
        services.AddOptions<AccountingCustomerOptions>().BindConfiguration("IntegrationCustomer");
        services.AddOptions<AccountingInventoryOptions>().BindConfiguration("AccountingInventory");
        services.AddScoped<SqlAccountingCommandHandler>();
        services.AddScoped<IAccountingCommandHandler>(sp => sp.GetRequiredService<SqlAccountingCommandHandler>());
        services.AddScoped<IAccountingPlatformReadHandler>(sp => sp.GetRequiredService<SqlAccountingCommandHandler>());
        return services;
    }
}

/// <summary>
/// Platform-owned implementation of the accounting command boundary.
/// Integration supplies only contract identifiers; this layer owns accounting tables.
/// </summary>
public sealed class AccountingCustomerOptions
{
    public byte? PersonType { get; set; }
    public string? CustomerRole { get; set; }
    public string? DetailAccountEntityType { get; set; }
}

public sealed class SqlAccountingCommandHandler(HyperSqlServerContext db,
    IOptions<AccountingCustomerOptions> customerOptions,
    IOptions<AccountingInventoryOptions> inventoryOptions) : IAccountingCommandHandler, IAccountingPlatformReadHandler
{
    public async Task<IReadOnlyList<AccountingShopRead>> SearchShopsAsync(string? search, CancellationToken ct)
    {
        var shops = db.TblShops.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(search))
        {
            search = search.Trim();
            shops = shops.Where(x => x.Shopid.ToString().Contains(search) || x.Name.Contains(search)
                || x.Ownerid.Contains(search) || (x.Mobile != null && x.Mobile.Contains(search)));
        }
        return await shops.OrderBy(x => x.Name).ThenBy(x => x.Shopid).Take(100)
            .Select(x => new AccountingShopRead(x.Shopid, x.Name, x.Ownerid,
                CanonicalTenant(x.Shopid, x.TenantId))).ToListAsync(ct);
    }

    public async Task<IReadOnlyList<AccountingShopRead>> GetShopsAsync(IReadOnlyCollection<int> shopIds, CancellationToken ct)
    {
        if (shopIds.Count == 0) return [];
        return await db.TblShops.AsNoTracking().Where(x => shopIds.Contains(x.Shopid))
            .Select(x => new AccountingShopRead(x.Shopid, x.Name, x.Ownerid,
                CanonicalTenant(x.Shopid, x.TenantId))).ToListAsync(ct);
    }

    public async Task<AccountingShopRead?> GetShopAsync(int shopId, CancellationToken ct) =>
        await db.TblShops.AsNoTracking().Where(x => x.Shopid == shopId)
            .Select(x => new AccountingShopRead(x.Shopid, x.Name, x.Ownerid,
                CanonicalTenant(x.Shopid, x.TenantId))).SingleOrDefaultAsync(ct);

    public async Task<IReadOnlyList<AccountingProductRead>> GetProductsAsync(AccountingScope scope, CancellationToken ct)
    {
        if (scope.ShopId <= 0 || string.IsNullOrWhiteSpace(scope.TenantId))
            throw new ArgumentException("Shop requires tenant scope.", nameof(scope));
        var shop = await GetShopAsync(scope.ShopId, ct);
        if (shop is null || !string.Equals(shop.TenantId, scope.TenantId, StringComparison.Ordinal)) return [];
        var tenantless = scope.TenantId == CanonicalTenant(scope.ShopId, null);
        return await db.TblProducts.AsNoTracking().Where(x => x.Shopid == scope.ShopId
                && (x.TenantId != null && x.TenantId.Trim() == scope.TenantId
                    || tenantless && (x.TenantId == null || x.TenantId.Trim() == "")))
            .OrderBy(x => x.Id).Select(x => new AccountingProductRead(x.Id, x.Name, x.Taxcode,
                x.Saleprice, x.Accountingstock, x.Isenabled, x.Isstockable, x.Minimumstock,
                x.Isenabled && x.Issellable && x.Isonlinesellable && x.Isstockable && !x.Isservice)).ToListAsync(ct);
    }

    public async Task<AccountingPlatformOverview> GetOverviewAsync(int days, AccountingScope? scope, CancellationToken ct)
    {
        if (days is not (7 or 30)) throw new ArgumentOutOfRangeException(nameof(days));
        if (scope is not null && (scope.ShopId <= 0 || string.IsNullOrWhiteSpace(scope.TenantId)))
            throw new ArgumentException("Shop requires tenant scope.", nameof(scope));
        var now = DateTime.UtcNow;
        var start = now.Date.AddDays(1 - days);
        var until = now.Date.AddDays(1);
        var shops = db.TblShops.AsNoTracking();
        var products = db.TblProducts.AsNoTracking();
        var people = db.TblPersons.AsNoTracking();
        var invoices = db.TblSaleorders.AsNoTracking();
        if (scope is not null)
        {
            var tenantless = scope.TenantId == CanonicalTenant(scope.ShopId, null);
            shops = shops.Where(x => x.Shopid == scope.ShopId &&
                (x.TenantId == scope.TenantId || tenantless && (x.TenantId == null || x.TenantId == "")));
            products = products.Where(x => x.Shopid == scope.ShopId &&
                (x.TenantId == scope.TenantId || tenantless && (x.TenantId == null || x.TenantId == "")));
            people = people.Where(x => x.Shopid == scope.ShopId &&
                (x.TenantId == scope.TenantId || tenantless && (x.TenantId == null || x.TenantId == "")));
            invoices = invoices.Where(x => x.Shopid == scope.ShopId &&
                (x.TenantId == scope.TenantId || tenantless && (x.TenantId == null || x.TenantId == "")));
        }
        var periodInvoices = invoices.Where(x => x.Issuedatetime >= start && x.Issuedatetime < until);
        var trend = await periodInvoices.GroupBy(x => x.Issuedatetime.Date)
            .Select(x => new AccountingOverviewDay(x.Key, x.Count())).ToListAsync(ct);
        return new(await shops.CountAsync(ct), await products.CountAsync(ct),
            await products.CountAsync(x => x.Isenabled, ct), await people.CountAsync(ct),
            await periodInvoices.CountAsync(ct),
            await products.CountAsync(x => x.Isenabled && x.Isstockable && x.Minimumstock != null
                && x.Accountingstock < x.Minimumstock, ct), trend);
    }

    public async Task<AccountingCommandResult> ValidateCustomerAsync(ValidateCustomerCommand command, CancellationToken ct)
    {
        var policy = Policy();
        var person = await db.TblPersons.SingleOrDefaultAsync(x => x.Id == command.PersonId
            && x.Shopid == command.Scope.ShopId && x.TenantId == command.Scope.TenantId && x.Isenabled, ct);
        if (person is null || person.Type != policy.PersonType || !HasRole(person.Roles, policy.CustomerRole)
            || (command.Customer.Mobile != null && person.Mobilenumber != null && person.Mobilenumber != command.Customer.Mobile)
            || (command.Customer.IdentifierNumber != null && person.Identifiernumber != null
                && person.Identifiernumber != command.Customer.IdentifierNumber))
            return new(AccountingCommandStatus.PendingDependency, ErrorCode: "AccountingCustomerMappingNeedsReview");
        var account = await db.TblDetailaccounts.AnyAsync(a => a.Detailaccountid == person.Detailaccountid
            && a.Shopid == command.Scope.ShopId && a.TenantId == command.Scope.TenantId, ct);
        return account ? new(AccountingCommandStatus.Applied, person.Id.ToString())
            : new(AccountingCommandStatus.PendingDependency, ErrorCode: "AccountingCustomerDetailAccountMismatch");
    }

    public async Task<AccountingCommandResult> ResolveCustomerAsync(ResolveCustomerCommand command, CancellationToken ct)
    {
        var policy = Policy();
        await using var transaction = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable, ct);
        var matches = await db.TblPersons.Where(p => p.Shopid == command.Scope.ShopId
                && (p.TenantId == command.Scope.TenantId || p.TenantId == null || p.TenantId == "")
                && ((command.Customer.Mobile != null && p.Mobilenumber == command.Customer.Mobile)
                    || (command.Customer.IdentifierNumber != null && p.Identifiernumber == command.Customer.IdentifierNumber)))
            .ToListAsync(ct);
        if (matches.Count > 1) return new(AccountingCommandStatus.PendingDependency, ErrorCode: "AmbiguousAccountingCustomer");
        if (matches.Count == 1)
        {
            var person = matches[0];
            if (command.Customer.IdentifierNumber is null || person.Identifiernumber != command.Customer.IdentifierNumber
                || !person.Isenabled || (command.Customer.Mobile != null && person.Mobilenumber != null
                    && person.Mobilenumber != command.Customer.Mobile)
                || person.Type != policy.PersonType || !HasRole(person.Roles, policy.CustomerRole))
                return new(AccountingCommandStatus.PendingDependency, ErrorCode: "AccountingCustomerNeedsReview");
            await transaction.CommitAsync(ct);
            return new(AccountingCommandStatus.Applied, person.Id.ToString());
        }
        var account = new SqlTblDetailaccount { Shopid = command.Scope.ShopId, TenantId = command.Scope.TenantId,
            Entitytype = policy.DetailAccountEntityType, Name = command.Customer.Name.Trim() };
        db.TblDetailaccounts.Add(account);
        await db.SaveChangesAsync(ct);
        var name = command.Customer.Name.Trim();
        var personNew = new SqlTblPerson { Shopid = command.Scope.ShopId, TenantId = command.Scope.TenantId,
            Detailaccountid = account.Detailaccountid, Name = name[..Math.Min(50, name.Length)], Nickname = name,
            Type = policy.PersonType, Roles = policy.CustomerRole, Isenabled = true,
            Mobilenumber = command.Customer.Mobile, Identifiernumber = command.Customer.IdentifierNumber };
        db.TblPersons.Add(personNew);
        await db.SaveChangesAsync(ct);
        account.Referenceid = personNew.Id;
        await db.SaveChangesAsync(ct);
        await transaction.CommitAsync(ct);
        return new(AccountingCommandStatus.Applied, personNew.Id.ToString());
    }

    public async Task<AccountingCommandResult> ApplyCounterpartyAsync(CounterpartyCommand command, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(command);
        var person = command.ExternalCustomerId;
        var match = await db.TblPersons.AsNoTracking().SingleOrDefaultAsync(x =>
            x.Id.ToString() == person && x.Shopid == command.Scope.ShopId && x.TenantId == command.Scope.TenantId, ct);
        return match is null
            ? new(AccountingCommandStatus.PendingDependency, ErrorCode: "AccountingCustomerMustBeRegistered")
            : new(AccountingCommandStatus.Applied, match.Id.ToString());
    }

    private AccountingCustomerPolicy Policy()
    {
        var value = customerOptions.Value;
        if (value.PersonType is null || string.IsNullOrWhiteSpace(value.CustomerRole)
            || value.CustomerRole.Length > 20 || string.IsNullOrWhiteSpace(value.DetailAccountEntityType)
            || value.DetailAccountEntityType.Length > 50)
            throw new InvalidOperationException("AccountingCustomerConventionNotConfigured");
        return new(value.PersonType.Value, value.CustomerRole, value.DetailAccountEntityType);
    }

    private static bool HasRole(string? roles, string role) => roles?.Split(',', StringSplitOptions.TrimEntries)
        .Contains(role, StringComparer.OrdinalIgnoreCase) == true;

    private static string CanonicalTenant(int shopId, string? tenantId) =>
        string.IsNullOrWhiteSpace(tenantId) ? $"shop:{shopId}" : tenantId.Trim();

    public async Task<AccountingCommandResult> ApplyVendorOrderAsync(VendorOrderCommand command, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(command);
        var marker = AccountingOrderIdentity.Marker(command.Scope, command.ConnectionId, command.ExternalOrderId);
        if (AccountingOrderIdentity.Validate(command) is { } error)
            return new(AccountingCommandStatus.Rejected, ErrorCode: error);
        if (command.PaymentStatus != 1)
            return new(AccountingCommandStatus.PendingDependency, ErrorCode: "BoothOrderPaymentNotConfirmed");
        var fingerprint = AccountingOrderIdentity.Fingerprint(command);
        await using var transaction = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable, ct);
        await LockShopAsync(command.Scope.ShopId, ct);
        var existing = await FindOrderAsync(command.Scope, command.ConnectionId, command.ExternalOrderId, ct);
        if (existing is not null)
        {
            if (existing.Status == 9) return new(AccountingCommandStatus.Rejected, ErrorCode: "AccountingOrderCancelled");
            if (!existing.Description!.Contains(fingerprint, StringComparison.Ordinal))
                return new(AccountingCommandStatus.Rejected, ErrorCode: "AccountingOrderContentChanged");
            return new(AccountingCommandStatus.Duplicate, existing.Saleorderid.ToString(),
                StockCommitted: existing.Description.Contains("stock:committed;", StringComparison.Ordinal));
        }
        if (!inventoryOptions.Value.AccountingStockSourceVerified)
            return new(AccountingCommandStatus.PendingDependency, ErrorCode: "AccountingStockSourceUnverified");
        if (await HasLegacyOrderAsync(command.Scope, command.ExternalOrderId, ct))
            return new(AccountingCommandStatus.PendingDependency, ErrorCode: "LegacyAccountingOrderNeedsReconciliation");
        var tenantless = command.Scope.TenantId == CanonicalTenant(command.Scope.ShopId, null);
        var customer = await db.TblPersons.SingleOrDefaultAsync(x => x.Id == command.AccountingCustomerId
            && x.Shopid == command.Scope.ShopId && (x.TenantId == command.Scope.TenantId
                || tenantless && (x.TenantId == null || x.TenantId == "")) && x.Isenabled, ct);
        if (customer is null) return new(AccountingCommandStatus.PendingDependency, ErrorCode: "AccountingCustomerNotFound");
        var products = await db.TblProducts.AsNoTracking().Where(x => command.Lines.Select(l => l.HyperProductId).Contains(x.Id)
            && x.Shopid == command.Scope.ShopId && (x.TenantId == command.Scope.TenantId
                || tenantless && (x.TenantId == null || x.TenantId == ""))).ToDictionaryAsync(x => x.Id, ct);
        if (products.Count != command.Lines.Select(x => x.HyperProductId).Distinct().Count())
            return new(AccountingCommandStatus.PendingDependency, ErrorCode: "AccountingProductNotFound");
        var period = await OpenFiscalPeriodAsync(command.Scope, ct);
        if (period is null) return new(AccountingCommandStatus.PendingDependency, ErrorCode: "OpenFiscalPeriodNotFound");
        foreach (var group in command.Lines.GroupBy(x => x.HyperProductId).OrderBy(x => x.Key))
        {
            var quantity = group.Sum(x => x.Quantity);
            var affected = await db.TblProducts.Where(x => x.Id == group.Key && x.Shopid == command.Scope.ShopId
                && (x.TenantId == command.Scope.TenantId || tenantless && (x.TenantId == null || x.TenantId == ""))
                && x.Isenabled && x.Issellable && x.Isonlinesellable && x.Isstockable && !x.Isservice
                && x.Accountingstock >= quantity)
                .ExecuteUpdateAsync(update => update.SetProperty(x => x.Accountingstock, x => x.Accountingstock - quantity), ct);
            if (affected != 1) return new(AccountingCommandStatus.PendingDependency, ErrorCode: "InsufficientSellableStock");
        }
        var total = command.Lines.Sum(x => x.Quantity * x.UnitPrice);
        var now = DateTime.UtcNow;
        var order = new SqlTblSaleorder
        {
            Totalamountbeforediscount = total, Totalinvoiceamount = total, Totalamount = total,
            Totalamountafterdiscount = total, Paidamount = total,
            Paymentstatus = command.PaymentStatus, Status = 1, Deliverystatus = 0, Shopid = command.Scope.ShopId,
            TenantId = command.Scope.TenantId, Customerid = customer.Id, Fiscalperiodid = period.Fiscalperiodid,
            Creationdatetime = now, Issuedatetime = now, Description = marker + fingerprint + "stock:committed;",
            Currency = "IRR", Exchangerate = 1, Invoicesubject = 1, Invoiceformat = 1
        };
        db.TblSaleorders.Add(order);
        await db.SaveChangesAsync(ct);
        foreach (var line in command.Lines)
        {
            var product = products[line.HyperProductId];
            var amount = line.Quantity * line.UnitPrice;
            db.TblSaleorderitems.Add(new SqlTblSaleorderitem
            {
                Saleorderid = order.Saleorderid, Productid = product.Id, Quantity = line.Quantity,
                Price = line.UnitPrice, Linetotalamount = amount, Lineamountbeforediscount = amount,
                Lineamountafterdiscount = amount, Shopid = command.Scope.ShopId,
                TenantId = command.Scope.TenantId, Fiscalperiodid = period.Fiscalperiodid,
                Productdescription = product.Name, Producttaxcode = product.Taxcode,
                Unit = product.Baseunit, Unitconversionfactor = 1
            });
        }
        await db.SaveChangesAsync(ct);
        await transaction.CommitAsync(ct);
        return new(AccountingCommandStatus.Applied, order.Saleorderid.ToString(), StockCommitted: true);
    }

    public async Task<AccountingCommandResult> ApplyCustomerOrderAsync(CustomerOrderCommand command, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(command);
        return new(AccountingCommandStatus.PendingDependency, ErrorCode: "CustomerOrderAccountingPolicyNotConfigured");
    }

    public async Task<AccountingCommandResult> CancelOrderAsync(CancelOrderCommand command, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(command);
        AccountingOrderIdentity.Marker(command.Scope, command.ConnectionId, command.ExternalOrderId);
        if (string.IsNullOrWhiteSpace(command.Reason) || command.Reason.Length > 500)
            return new(AccountingCommandStatus.Rejected, ErrorCode: "InvalidCancellationReason");
        await using var transaction = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable, ct);
        await LockShopAsync(command.Scope.ShopId, ct);
        var order = await FindOrderAsync(command.Scope, command.ConnectionId, command.ExternalOrderId, ct);
        if (order is null) return new(AccountingCommandStatus.PendingDependency, ErrorCode: "AccountingOrderNotFound");
        if (order.Status == 9) return new(AccountingCommandStatus.Duplicate, order.Saleorderid.ToString());
        if (order.Deliverystatus != 0)
            return new(AccountingCommandStatus.PendingDependency, ErrorCode: "PhysicalReturnConfirmationRequired");
        if (order.Description!.Contains("stock:committed;", StringComparison.Ordinal))
        {
            var quantities = await db.TblSaleorderitems.AsNoTracking().Where(x => x.Saleorderid == order.Saleorderid
                && x.Shopid == command.Scope.ShopId && x.TenantId == command.Scope.TenantId)
                .GroupBy(x => x.Productid).Select(x => new { Id = x.Key, Quantity = x.Sum(l => l.Quantity) })
                .OrderBy(x => x.Id).ToListAsync(ct);
            if (quantities.Count == 0 || quantities.Any(x => x.Quantity <= 0))
                return new(AccountingCommandStatus.PendingDependency, ErrorCode: "AccountingOrderLinesNeedReview");
            var tenantless = command.Scope.TenantId == CanonicalTenant(command.Scope.ShopId, null);
            foreach (var line in quantities)
            {
                var affected = await db.TblProducts.Where(x => x.Id == line.Id && x.Shopid == command.Scope.ShopId
                    && (x.TenantId == command.Scope.TenantId || tenantless && (x.TenantId == null || x.TenantId == "")))
                    .ExecuteUpdateAsync(update => update.SetProperty(x => x.Accountingstock, x => x.Accountingstock + line.Quantity), ct);
                if (affected != 1) return new(AccountingCommandStatus.PendingDependency, ErrorCode: "AccountingProductNotFound");
            }
        }
        order.Status = 9;
        var description = $"{order.Description};cancel:{command.Reason}";
        var maxLength = db.Model.FindEntityType(typeof(SqlTblSaleorder))!
            .FindProperty(nameof(SqlTblSaleorder.Description))!.GetMaxLength() ?? 200;
        order.Description = description[..Math.Min(maxLength, description.Length)];
        await db.SaveChangesAsync(ct);
        await transaction.CommitAsync(ct);
        return new(AccountingCommandStatus.Applied, order.Saleorderid.ToString());
    }

    public async Task<AccountingCommandResult> ApplyParcelStatusAsync(ParcelStatusCommand command, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(command);
        await using var transaction = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable, ct);
        await LockShopAsync(command.Scope.ShopId, ct);
        var order = await FindOrderAsync(command.Scope, command.ConnectionId, command.ExternalOrderId, ct);
        if (order is null) return new(AccountingCommandStatus.PendingDependency, ErrorCode: "AccountingOrderNotFound");
        if (order.Status == 9) return new(AccountingCommandStatus.Rejected, ErrorCode: "AccountingOrderCancelled");
        order.Waybillnumber = command.TrackingCode;
        order.Deliverystatus = ParcelState(command.Status);
        await db.SaveChangesAsync(ct);
        await transaction.CommitAsync(ct);
        return new(AccountingCommandStatus.Applied, order.Saleorderid.ToString());
    }

    public async Task<AccountingCommandResult> ApplyExternalProductChangedAsync(ExternalProductChangedCommand command, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(command);
        var product = await db.TblProducts.SingleOrDefaultAsync(x => x.Id == command.HyperProductId
            && x.Shopid == command.Scope.ShopId && x.TenantId == command.Scope.TenantId, ct);
        if (product is null) return new(AccountingCommandStatus.PendingDependency, ErrorCode: "AccountingProductNotFound");
        product.Name = command.Title; product.Taxcode = command.Sku; if (command.Price is not null) product.Saleprice = command.Price.Value;
        // Marketplace inventory is an observation, never an authoritative
        // replacement for accounting stock or an acknowledged stock movement.
        await db.SaveChangesAsync(ct);
        return new(AccountingCommandStatus.Applied, product.Id.ToString());
    }

    private Task<SqlTblSaleorder?> FindOrderAsync(AccountingScope scope, long connectionId, string externalOrderId, CancellationToken ct)
    {
        var marker = AccountingOrderIdentity.Marker(scope, connectionId, externalOrderId);
        return db.TblSaleorders.SingleOrDefaultAsync(x => x.Shopid == scope.ShopId && x.TenantId == scope.TenantId
            && x.Description != null && x.Description.StartsWith(marker), ct);
    }

    private Task<bool> HasLegacyOrderAsync(AccountingScope scope, string externalOrderId, CancellationToken ct) =>
        db.TblSaleorders.AsNoTracking().AnyAsync(x => x.Shopid == scope.ShopId && x.TenantId == scope.TenantId
            && x.Description != null && x.Description.StartsWith("integration-event:")
            && x.Description.Contains($"external-order:{externalOrderId};"), ct);

    private Task LockShopAsync(int shopId, CancellationToken ct) => db.Database.ExecuteSqlInterpolatedAsync($"""
        DECLARE @result int;
        EXEC @result=sys.sp_getapplock @Resource={"accounting-orders:" + shopId},
            @LockMode='Exclusive', @LockOwner='Transaction', @LockTimeout=15000;
        IF @result < 0 THROW 51000, 'AccountingOrderLockUnavailable', 1;
        """, ct);

    private Task<SqlTblShopfiscalperiod?> OpenFiscalPeriodAsync(AccountingScope scope, CancellationToken ct) =>
        db.TblShopfiscalperiods.Where(x => x.Shopid == scope.ShopId && (x.TenantId == scope.TenantId
            || scope.TenantId == "shop:" + scope.ShopId && (x.TenantId == null || x.TenantId == ""))
            && x.Fiscalperiodstatusid == 1 && x.Startdate <= DateOnly.FromDateTime(DateTime.UtcNow)
            && x.Enddate >= DateOnly.FromDateTime(DateTime.UtcNow)).OrderByDescending(x => x.Startdate).FirstOrDefaultAsync(ct);

    private static byte ParcelState(string status) => status.Contains("deliv", StringComparison.OrdinalIgnoreCase) ? (byte)2 : (byte)1;
}

internal sealed record AccountingCustomerPolicy(byte PersonType, string CustomerRole,
    string DetailAccountEntityType);
