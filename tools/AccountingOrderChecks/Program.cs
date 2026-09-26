using Hyper.Domain.Entities.Database;
using Hyper.Infrastructure.Data.Repository.Hyper;
using Hyperyek.Accounting.Contracts;
using Hyperyek.Accounting.Infrastructure;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var input = Environment.GetEnvironmentVariable("ACCOUNTING_TEST_SQL") ?? throw new Exception("ACCOUNTING_TEST_SQL required");
var database = "HyperAccountingChecks_" + Guid.NewGuid().ToString("N");
var cs = new SqlConnectionStringBuilder(input) { InitialCatalog = database };
var options = new DbContextOptionsBuilder<HyperSqlServerContext>().UseSqlServer(cs.ConnectionString)
    .ReplaceService<IModelCustomizer, FixtureModel>().Options;
var scope = new AccountingScope(7, "tenant-a");
var passed = 0;
void Check(bool condition, string name) { if (!condition) throw new Exception(name); Console.WriteLine("PASS " + name); passed++; }
SqlAccountingCommandHandler Handler(HyperSqlServerContext db, bool verified = true) => new(db,
    Options.Create(new AccountingCustomerOptions()), Options.Create(new AccountingInventoryOptions { AccountingStockSourceVerified = verified }));
await using var setup = new HyperSqlServerContext(options);
var created = false;
try
{
    await setup.Database.EnsureCreatedAsync(); created = true;
    var services = new ServiceCollection();
    services.AddSingleton<IConfiguration>(new ConfigurationBuilder().Build());
    services.AddHyperyekSqlAccounting(cs.ConnectionString);
    using (var provider = services.BuildServiceProvider())
    using (var serviceScope = provider.CreateScope())
        Check(ReferenceEquals(serviceScope.ServiceProvider.GetRequiredService<IAccountingCommandHandler>(),
            serviceScope.ServiceProvider.GetRequiredService<IAccountingPlatformReadHandler>()), "accounting command and read handlers resolve in DI");
    var person = new SqlTblPerson { Shopid = 7, TenantId = scope.TenantId, Nickname = "fixture", Isenabled = true };
    var products = Enumerable.Range(1, 8).Select(n => new SqlTblProduct { Shopid = 7, TenantId = scope.TenantId,
        Name = "fixture-" + n, Accountingstock = 10, Isenabled = true, Issellable = true,
        Isonlinesellable = true, Isstockable = true }).ToArray();
    setup.Add(person); setup.AddRange(products);
    setup.TblShopfiscalperiods.Add(new() { Shopid = 7, TenantId = scope.TenantId, Fiscalperiodname = "fixture",
        Fiscalperiodstatusid = 1, Startdate = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(-2),
        Enddate = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(2) });
    await setup.SaveChangesAsync();
    VendorOrderCommand Order(string id, int product, decimal qty, long connection = 10) =>
        new("event-" + id, scope, connection, id, null, "buyer", [new(product, qty, 2)], qty * 2, 1, person.Id);
    async Task<AccountingCommandResult> Apply(VendorOrderCommand order, bool verified = true)
    { await using var db = new HyperSqlServerContext(options); return await Handler(db, verified).ApplyVendorOrderAsync(order, default); }
    async Task<decimal> Stock(int id)
    { await using var db = new HyperSqlServerContext(options); return await db.TblProducts.Where(x => x.Id == id).Select(x => x.Accountingstock).SingleAsync(); }
    async Task<AccountingCommandResult> Cancel(string id, long connection = 10)
    { await using var db = new HyperSqlServerContext(options); return await Handler(db).CancelOrderAsync(new("cancel-" + id, scope, connection, id, new string('x', 400)), default); }

    var first = Order("first", products[0].Id, 3);
    Check((await Apply(first, false)).ErrorCode == "AccountingStockSourceUnverified", "unverified stock cannot be written");
    var applied = await Apply(first);
    Check(applied.Status == AccountingCommandStatus.Applied && applied.StockCommitted && await Stock(products[0].Id) == 7,
        "invoice and stock debit commit together");
    await using (var read = new HyperSqlServerContext(options))
    {
        var invoice = await read.TblSaleorders.SingleAsync();
        Check(invoice.Paidamount == 6 && invoice.Totalamount == 6, "Paid=1 maps to full paid amount");
    }
    var duplicate = await Apply(first with { EventId = "different-event" });
    Check(duplicate.Status == AccountingCommandStatus.Duplicate && duplicate.StockCommitted && await Stock(products[0].Id) == 7,
        "different event replay cannot debit twice");
    Check((await Apply(first with { TotalAmount = 6.00m, Lines = [new(products[0].Id, 3.000m, 2.00m)] })).Status == AccountingCommandStatus.Duplicate,
        "decimal formatting does not change order identity");
    Check((await Apply(first with { TotalAmount = 8, Lines = [new(products[0].Id, 4, 2)] })).ErrorCode == "AccountingOrderContentChanged",
        "changed replay is rejected");
    Check((await Apply(Order("first", products[0].Id, 1, 11))).Status == AccountingCommandStatus.Applied && await Stock(products[0].Id) == 6,
        "same external id on another connection is independent");
    Check((await Cancel("first")).Status == AccountingCommandStatus.Applied && await Stock(products[0].Id) == 9,
        "cancellation restores only its connection quantity");
    Check((await Cancel("first")).Status == AccountingCommandStatus.Duplicate && await Stock(products[0].Id) == 9,
        "duplicate cancellation cannot inflate stock");
    Check((await Apply(first)).ErrorCode == "AccountingOrderCancelled", "cancelled order cannot be recreated");
    Check((await Apply(Order("unpaid", products[1].Id, 2) with { PaymentStatus = 0 })).ErrorCode == "BoothOrderPaymentNotConfirmed"
        && await Stock(products[1].Id) == 10, "unpaid order cannot debit stock");
    var race = await Task.WhenAll(Apply(Order("race-a", products[2].Id, 6)), Apply(Order("race-b", products[2].Id, 6)));
    Check(race.Count(x => x.Status == AccountingCommandStatus.Applied) == 1 && await Stock(products[2].Id) == 4,
        "concurrent orders cannot sell twelve from ten");
    var same = Order("same-order", products[3].Id, 3);
    var repeat = await Task.WhenAll(Apply(same), Apply(same));
    Check(repeat.Count(x => x.Status == AccountingCommandStatus.Applied) == 1
        && repeat.Count(x => x.Status == AccountingCommandStatus.Duplicate) == 1 && await Stock(products[3].Id) == 7,
        "concurrent replay creates one invoice and one debit");
    var multi = Order("rollback", products[4].Id, 1) with { Lines = [new(products[4].Id, 1, 2), new(products[5].Id, 11, 2)], TotalAmount = 24 };
    Check((await Apply(multi)).ErrorCode == "InsufficientSellableStock" && await Stock(products[4].Id) == 10,
        "later line failure rolls back prior stock debit");
    var shipped = Order("shipped", products[6].Id, 2); await Apply(shipped);
    await using (var db = new HyperSqlServerContext(options))
        await Handler(db).ApplyParcelStatusAsync(new("parcel", scope, 10, "shipped", "p", "delivered", "track"), default);
    Check((await Cancel("shipped")).ErrorCode == "PhysicalReturnConfirmationRequired" && await Stock(products[6].Id) == 8,
        "shipped goods require physical return confirmation");
    await using (var db = new HyperSqlServerContext(options))
        await Handler(db).ApplyExternalProductChangedAsync(new("product", scope, 10, products[7].Id, "ext", null, null, "title", 2, 999, 1), default);
    Check(await Stock(products[7].Id) == 10, "marketplace snapshot cannot overwrite accounting stock");
    // Competing native-style conditional writer uses an independent SQL connection.
    async Task<int> NativeSale()
    {
        await using var db = new HyperSqlServerContext(options);
        return await db.TblProducts.Where(x => x.Id == products[7].Id && x.Accountingstock >= 6)
            .ExecuteUpdateAsync(u => u.SetProperty(x => x.Accountingstock, x => x.Accountingstock - 6));
    }
    var native = NativeSale(); var online = Apply(Order("native-race", products[7].Id, 6));
    await Task.WhenAll(native, online);
    Check(native.Result + (online.Result.Status == AccountingCommandStatus.Applied ? 1 : 0) == 1
        && await Stock(products[7].Id) == 4, "conditional POS-style writer and online order share row concurrency");
    Console.WriteLine($"{passed} accounting SQL checks passed.");
}
finally
{
    if (created && setup.Database.GetDbConnection().Database == database && database.StartsWith("HyperAccountingChecks_", StringComparison.Ordinal))
        await setup.Database.EnsureDeletedAsync();
}

sealed class FixtureModel(ModelCustomizerDependencies dependencies) : ModelCustomizer(dependencies)
{
    public override void Customize(ModelBuilder builder, DbContext context)
    {
        base.Customize(builder, context);
        Type[] keep = [typeof(SqlTblPerson), typeof(SqlTblProduct), typeof(SqlTblShopfiscalperiod), typeof(SqlTblSaleorder), typeof(SqlTblSaleorderitem)];
        // Keep real column/key/default mappings; only omit unrelated legacy tables
        // from the disposable fixture, not from the production accounting model.
        foreach (var entity in builder.Model.GetEntityTypes().ToArray())
            if (!keep.Contains(entity.ClrType)) builder.Ignore(entity.ClrType);
        foreach (var entity in builder.Model.GetEntityTypes()) entity.SetIsTableExcludedFromMigrations(false);
    }
}
