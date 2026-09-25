using Hyper.Domain.Entities.Database;
using Hyper.Infrastructure.Data.Repository.Hyper;
using Hyperyek.Accounting.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Hyperyek.Accounting.Infrastructure;

public static class AccountingInfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddHyperyekSqlAccounting(this IServiceCollection services, string connectionString)
    {
        services.AddDbContext<HyperSqlServerContext>(options => options.UseSqlServer(connectionString));
        services.AddScoped<IAccountingCommandHandler, SqlAccountingCommandHandler>();
        return services;
    }
}

/// <summary>
/// Platform-owned implementation of the accounting command boundary.
/// Integration supplies only contract identifiers; this layer owns accounting tables.
/// </summary>
public sealed class SqlAccountingCommandHandler(HyperSqlServerContext db) : IAccountingCommandHandler
{
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

    public async Task<AccountingCommandResult> ApplyVendorOrderAsync(VendorOrderCommand command, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(command);
        var existing = await FindOrderAsync(command.Scope, command.ExternalOrderId, ct);
        if (existing is not null) return new(AccountingCommandStatus.Duplicate, existing.Saleorderid.ToString());
        var customer = await db.TblPersons.SingleOrDefaultAsync(x => x.Id == command.AccountingCustomerId
            && x.Shopid == command.Scope.ShopId && x.TenantId == command.Scope.TenantId && x.Isenabled, ct);
        if (customer is null) return new(AccountingCommandStatus.PendingDependency, ErrorCode: "AccountingCustomerNotFound");
        var products = await db.TblProducts.Where(x => command.Lines.Select(l => l.HyperProductId).Contains(x.Id)
            && x.Shopid == command.Scope.ShopId && x.TenantId == command.Scope.TenantId).ToDictionaryAsync(x => x.Id, ct);
        if (products.Count != command.Lines.Select(x => x.HyperProductId).Distinct().Count())
            return new(AccountingCommandStatus.PendingDependency, ErrorCode: "AccountingProductNotFound");
        var period = await OpenFiscalPeriodAsync(command.Scope, ct);
        if (period is null) return new(AccountingCommandStatus.PendingDependency, ErrorCode: "OpenFiscalPeriodNotFound");
        await using var transaction = await db.Database.BeginTransactionAsync(ct);
        var total = command.Lines.Sum(x => x.Quantity * x.UnitPrice);
        var now = DateTime.UtcNow;
        var order = new SqlTblSaleorder
        {
            Totalamountbeforediscount = total, Totalinvoiceamount = total, Totalamount = total,
            Totalamountafterdiscount = total, Paidamount = command.PaymentStatus == 2 ? total : 0,
            Paymentstatus = command.PaymentStatus, Status = 1, Shopid = command.Scope.ShopId,
            TenantId = command.Scope.TenantId, Customerid = customer.Id, Fiscalperiodid = period.Fiscalperiodid,
            Creationdatetime = now, Issuedatetime = now, Description = Marker(command.EventId, command.ExternalOrderId),
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
                Unitconversionfactor = 1
            });
        }
        await db.SaveChangesAsync(ct);
        await transaction.CommitAsync(ct);
        return new(AccountingCommandStatus.Applied, order.Saleorderid.ToString());
    }

    public async Task<AccountingCommandResult> ApplyCustomerOrderAsync(CustomerOrderCommand command, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(command);
        return new(AccountingCommandStatus.PendingDependency, ErrorCode: "CustomerOrderAccountingPolicyNotConfigured");
    }

    public async Task<AccountingCommandResult> CancelOrderAsync(CancelOrderCommand command, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(command);
        var order = await FindOrderAsync(command.Scope, command.ExternalOrderId, ct);
        if (order is null) return new(AccountingCommandStatus.PendingDependency, ErrorCode: "AccountingOrderNotFound");
        if (order.Status == 9) return new(AccountingCommandStatus.Duplicate, order.Saleorderid.ToString());
        order.Status = 9;
        order.Description = $"{order.Description};cancel:{command.Reason}"[..Math.Min(4000, ($"{order.Description};cancel:{command.Reason}").Length)];
        await db.SaveChangesAsync(ct);
        return new(AccountingCommandStatus.Applied, order.Saleorderid.ToString());
    }

    public async Task<AccountingCommandResult> ApplyParcelStatusAsync(ParcelStatusCommand command, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(command);
        var order = await FindOrderAsync(command.Scope, command.ExternalOrderId, ct);
        if (order is null) return new(AccountingCommandStatus.PendingDependency, ErrorCode: "AccountingOrderNotFound");
        order.Waybillnumber = command.TrackingCode;
        order.Deliverystatus = ParcelState(command.Status);
        await db.SaveChangesAsync(ct);
        return new(AccountingCommandStatus.Applied, order.Saleorderid.ToString());
    }

    public async Task<AccountingCommandResult> ApplyExternalProductChangedAsync(ExternalProductChangedCommand command, CancellationToken ct)
    {
        ArgumentNullException.ThrowIfNull(command);
        var product = await db.TblProducts.SingleOrDefaultAsync(x => x.Id == command.HyperProductId
            && x.Shopid == command.Scope.ShopId && x.TenantId == command.Scope.TenantId, ct);
        if (product is null) return new(AccountingCommandStatus.PendingDependency, ErrorCode: "AccountingProductNotFound");
        product.Name = command.Title; product.Taxcode = command.Sku; if (command.Price is not null) product.Saleprice = command.Price.Value;
        if (command.Inventory is not null) product.Accountingstock = command.Inventory.Value;
        await db.SaveChangesAsync(ct);
        return new(AccountingCommandStatus.Applied, product.Id.ToString());
    }

    private Task<SqlTblSaleorder?> FindOrderAsync(AccountingScope scope, string externalOrderId, CancellationToken ct) =>
        db.TblSaleorders.SingleOrDefaultAsync(x => x.Shopid == scope.ShopId && x.TenantId == scope.TenantId
            && x.Description != null && x.Description.Contains($"external-order:{externalOrderId};"), ct);

    private Task<SqlTblShopfiscalperiod?> OpenFiscalPeriodAsync(AccountingScope scope, CancellationToken ct) =>
        db.TblShopfiscalperiods.Where(x => x.Shopid == scope.ShopId && x.TenantId == scope.TenantId
            && x.Fiscalperiodstatusid == 1 && x.Startdate <= DateOnly.FromDateTime(DateTime.UtcNow)
            && x.Enddate >= DateOnly.FromDateTime(DateTime.UtcNow)).OrderByDescending(x => x.Startdate).FirstOrDefaultAsync(ct);

    private static string Marker(string eventId, string externalOrderId) => $"integration-event:{eventId};external-order:{externalOrderId};";
    private static byte ParcelState(string status) => status.Contains("deliv", StringComparison.OrdinalIgnoreCase) ? (byte)2 : (byte)1;
}
