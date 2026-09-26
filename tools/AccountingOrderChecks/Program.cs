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
    Options.Create(new AccountingCustomerOptions { PersonType = 1, CustomerRole = "customer", DetailAccountEntityType = "person" }),
    Options.Create(new AccountingInventoryOptions { AccountingStockSourceVerified = verified }));
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
    for (var i = 1; i <= 8; i++)
    {
        setup.TblShops.Add(new() { Ownerid = "fixture-owner", Name = "fixture-shop-" + i,
            TenantId = i == 8 ? null : scope.TenantId });
        await setup.SaveChangesAsync();
    }
    var detail = new SqlTblDetailaccount { Shopid = 7, TenantId = scope.TenantId, Name = "fixture", Entitytype = "person" };
    var stockAccount = new SqlTblDetailaccount { Shopid = 7, TenantId = scope.TenantId, Name = "stock", Entitytype = "product" };
    var legacyStockAccount = new SqlTblDetailaccount { Shopid = 8, Name = "legacy stock", Entitytype = "product" };
    setup.AddRange(detail, stockAccount, legacyStockAccount); await setup.SaveChangesAsync();
    var person = new SqlTblPerson { Shopid = 7, TenantId = scope.TenantId, Nickname = "fixture", Isenabled = true,
        Type = 1, Roles = "customer", Detailaccountid = detail.Detailaccountid };
    var products = Enumerable.Range(1, 8).Select(n => new SqlTblProduct { Shopid = 7, TenantId = scope.TenantId,
        Name = "fixture-" + n, Accountingstock = 10, Isenabled = true, Issellable = true,
        Isonlinesellable = true, Isstockable = true, Detailaccountid = stockAccount.Detailaccountid }).ToArray();
    setup.Add(person); setup.AddRange(products);
    setup.TblShopfiscalperiods.Add(new() { Shopid = 7, TenantId = scope.TenantId, Fiscalperiodname = "fixture",
        Fiscalperiodstatusid = 1, Startdate = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(-2),
        Enddate = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(2) });
    await setup.SaveChangesAsync();
    var catalog = await Handler(setup).GetProductsAsync(scope, default);
    Check(catalog.Count == 8 && catalog.All(x => x.CanSell), "accounting catalog exposes authoritative sellability");
    Check((await Handler(setup).GetProductsAsync(scope with { TenantId = "other-tenant" }, default)).Count == 0,
        "catalog rejects a different shop tenant");
    products[0].Isonlinesellable = false;
    products[1].Issellable = false;
    products[2].Isservice = true;
    products[3].Isstockable = false;
    products[4].Isenabled = false;
    await setup.SaveChangesAsync();
    catalog = await Handler(setup).GetProductsAsync(scope, default);
    Check(catalog.Where(x => products.Take(5).Select(p => p.Id).Contains(x.ProductId)).All(x => !x.CanSell),
        "all five accounting sellability flags are respected");
    products[0].Isonlinesellable = true; products[1].Issellable = true; products[2].Isservice = false;
    products[3].Isstockable = true; products[4].Isenabled = true;
    var legacy = new SqlTblProduct { Shopid = 8, TenantId = "  ", Name = "legacy", Accountingstock = 4,
        Isenabled = true, Issellable = true, Isonlinesellable = true, Isstockable = true,
        Detailaccountid = legacyStockAccount.Detailaccountid };
    setup.Add(legacy); await setup.SaveChangesAsync();
    Check((await Handler(setup).GetProductsAsync(new(8, "shop:8"), default)).Single().ProductId == legacy.Id,
        "tenantless legacy shop uses its canonical accounting scope");
    async Task<AccountingCommandResult> Resolve(AccountingScope customerScope, AccountingCustomerIdentity identity)
    { await using var db = new HyperSqlServerContext(options); return await Handler(db).ResolveCustomerAsync(new(customerScope, identity), default); }
    async Task<AccountingCommandResult> Validate(AccountingScope customerScope, int id, AccountingCustomerIdentity identity)
    { await using var db = new HyperSqlServerContext(options); return await Handler(db).ValidateCustomerAsync(new(customerScope, id, identity), default); }
    var identity = new AccountingCustomerIdentity("Customer fixture", "09120000001", "1234567890", "external-fixture");
    var resolved = await Resolve(scope, identity);
    var customerId = int.Parse(resolved.InternalReference!);
    Check(resolved.Status == AccountingCommandStatus.Applied
        && (await Validate(scope, customerId, identity)).Status == AccountingCommandStatus.Applied,
        "customer and linked detail account commit and validate together");
    var replayCustomer = await Resolve(scope, identity);
    Check(replayCustomer.InternalReference == resolved.InternalReference,
        "recovery after lost Integration mapping reuses national-identity customer");
    var concurrentIdentity = identity with { Mobile = "09120000002", IdentifierNumber = "1234567891" };
    var customerRace = await Task.WhenAll(Resolve(scope, concurrentIdentity), Resolve(scope, concurrentIdentity));
    Check(customerRace.All(x => x.Status == AccountingCommandStatus.Applied)
        && customerRace[0].InternalReference == customerRace[1].InternalReference,
        "concurrent resolution creates one person and detail account");
    Check((await Resolve(scope with { TenantId = "other" }, identity)).ErrorCode == "AccountingShopScopeMismatch"
        && (await Validate(scope with { TenantId = "other" }, customerId, identity)).ErrorCode == "AccountingShopScopeMismatch",
        "resolve and validate reject noncanonical shop tenant");
    Check((await Resolve(new(9999, "missing"), identity)).ErrorCode == "AccountingShopScopeMismatch",
        "missing shop cannot receive customer records");
    Check((await Resolve(scope, identity with { Mobile = null, IdentifierNumber = null })).ErrorCode == "InvalidAccountingCustomer"
        && (await Resolve(scope, identity with { Name = new string('n', 101) })).ErrorCode == "InvalidAccountingCustomer"
        && (await Resolve(scope, identity with { Mobile = " " })).ErrorCode == "InvalidAccountingCustomer",
        "invalid customer fields reject before persistence");
    Check((await Resolve(scope, identity with { IdentifierNumber = null })).ErrorCode == "AccountingCustomerNeedsReview",
        "mobile-only match does not silently merge identities");
    Check((await Resolve(scope, identity with { Mobile = concurrentIdentity.Mobile })).ErrorCode == "AmbiguousAccountingCustomer",
        "conflicting national identity and mobile require review");
    var legacyIdentity = identity with { Mobile = "09120000003", IdentifierNumber = "1234567892" };
    var legacyAccount = new SqlTblDetailaccount { Shopid = 8, TenantId = " ", Entitytype = "person", Name = "legacy customer" };
    setup.Add(legacyAccount); await setup.SaveChangesAsync();
    var legacyPerson = new SqlTblPerson { Shopid = 8, TenantId = null, Nickname = "legacy customer", Type = 1,
        Roles = "customer", Isenabled = true, Detailaccountid = legacyAccount.Detailaccountid,
        Mobilenumber = legacyIdentity.Mobile, Identifiernumber = legacyIdentity.IdentifierNumber };
    setup.Add(legacyPerson); await setup.SaveChangesAsync();
    Check((await Resolve(new(8, "shop:8"), legacyIdentity)).InternalReference == legacyPerson.Id.ToString()
        && (await Validate(new(8, "shop:8"), legacyPerson.Id, legacyIdentity)).Status == AccountingCommandStatus.Applied,
        "legacy null and blank tenant customer and account use canonical shop scope");
    var unscopedIdentity = identity with { Mobile = "09120000004", IdentifierNumber = "1234567893" };
    var unscopedPerson = new SqlTblPerson { Shopid = 7, TenantId = null, Nickname = "unscoped", Type = 1,
        Roles = "customer", Isenabled = true, Detailaccountid = detail.Detailaccountid,
        Mobilenumber = unscopedIdentity.Mobile, Identifiernumber = unscopedIdentity.IdentifierNumber };
    setup.Add(unscopedPerson); await setup.SaveChangesAsync();
    Check((await Validate(scope, unscopedPerson.Id, unscopedIdentity)).ErrorCode == "AccountingCustomerMappingNeedsReview"
        && (await Resolve(scope, unscopedIdentity)).InternalReference != unscopedPerson.Id.ToString(),
        "tenanted shop never adopts an unscoped person");
    await using (var db = new HyperSqlServerContext(options))
    {
        var createdPerson = await db.TblPersons.SingleAsync(x => x.Id == customerId);
        var createdAccount = await db.TblDetailaccounts.SingleAsync(x => x.Detailaccountid == createdPerson.Detailaccountid);
        Check(createdAccount.Referenceid == customerId, "new account points back to its person");
        createdAccount.Shopid = 8; await db.SaveChangesAsync();
        Check((await Resolve(scope, identity)).ErrorCode == "AccountingCustomerDetailAccountMismatch"
            && (await Validate(scope, customerId, identity)).ErrorCode == "AccountingCustomerDetailAccountMismatch",
            "cross-shop detail account rejects both resolution and linked validation");
        createdAccount.Shopid = 7; createdAccount.Referenceid = person.Id; await db.SaveChangesAsync();
        Check((await Resolve(scope, identity)).ErrorCode == "AccountingCustomerDetailAccountMismatch",
            "account reference to another person requires review");
        createdAccount.Referenceid = customerId; createdAccount.Entitytype = "supplier"; await db.SaveChangesAsync();
        Check((await Validate(scope, customerId, identity)).ErrorCode == "AccountingCustomerDetailAccountMismatch",
            "wrong detail account entity type requires review");
        createdAccount.Entitytype = "person"; await db.SaveChangesAsync();
    }
    VendorOrderCommand Order(string id, int product, decimal qty, long connection = 10) =>
        new("event-" + id, scope, connection, id, null, "buyer", [new(product, qty, 2)], qty * 2, 1, person.Id);
    async Task<AccountingCommandResult> Apply(VendorOrderCommand order, bool verified = true)
    { await using var db = new HyperSqlServerContext(options); return await Handler(db, verified).ApplyVendorOrderAsync(order, default); }
    async Task<decimal> Stock(int id)
    { await using var db = new HyperSqlServerContext(options); return await db.TblProducts.Where(x => x.Id == id).Select(x => x.Accountingstock).SingleAsync(); }
    async Task<AccountingCommandResult> Cancel(string id, long connection = 10)
    { await using var db = new HyperSqlServerContext(options); return await Handler(db).CancelOrderAsync(new("cancel-" + id, scope, connection, id, new string('x', 400)), default); }

    var first = Order("first", products[0].Id, 3);
    detail.Referenceid = customerId; await setup.SaveChangesAsync();
    Check((await Apply(first)).ErrorCode == "AccountingCustomerDetailAccountMismatch" && await Stock(products[0].Id) == 10,
        "invoice cannot bypass customer account consistency or debit stock on failure");
    detail.Referenceid = person.Id; person.Roles = "supplier"; await setup.SaveChangesAsync();
    Check((await Apply(first)).ErrorCode == "AccountingCustomerNeedsReview" && await Stock(products[0].Id) == 10,
        "invoice requires configured customer role before stock debit");
    person.Roles = "customer"; await setup.SaveChangesAsync();
    Check((await Apply(first with { Scope = scope with { TenantId = "other" } })).ErrorCode == "AccountingShopScopeMismatch",
        "invoice validates canonical shop tenant");
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
    async Task<AccountingCommandResult> Parcel(string id, string status, string? tracking = null, long connection = 10)
    {
        await using var db = new HyperSqlServerContext(options);
        return await Handler(db).ApplyParcelStatusAsync(new("parcel-" + id, scope, connection, id, "p-" + id, status, tracking), default);
    }
    async Task<SqlTblSaleorder> Invoice(string reference)
    {
        await using var db = new HyperSqlServerContext(options);
        return await db.TblSaleorders.AsNoTracking().SingleAsync(x => x.Saleorderid == long.Parse(reference));
    }
    var parcelInvoice = await Apply(Order("parcel-life", products[4].Id, 1));
    var parcelReference = parcelInvoice.InternalReference!;
    Check((await Parcel("parcel-life", "delivery_failed", "bad")).ErrorCode == "AccountingParcelStatusUnsupported"
        && (await Invoice(parcelReference)).Deliverystatus == 0 && (await Invoice(parcelReference)).Waybillnumber is null,
        "unknown parcel status never guesses shipment or delivery");
    Check((await Parcel("parcel-life", "preparing")).Status == AccountingCommandStatus.Duplicate,
        "preparation is a no-op before dispatch and remains cancellable");
    Check((await Parcel("parcel-life", "shipped", new string('t', 19))).ErrorCode == "AccountingTrackingCodeNeedsReview"
        && (await Invoice(parcelReference)).Deliverystatus == 0,
        "overlength tracking pauses without truncation or partial status write");
    Check((await Parcel("parcel-life", "shipped", "۱۲۳")).ErrorCode == "AccountingTrackingCodeNeedsReview",
        "varchar-incompatible tracking is not silently converted");
    var tracking18 = new string('t', 18);
    Check((await Parcel("parcel-life", " SHIPPED ", tracking18)).Status == AccountingCommandStatus.Applied
        && (await Invoice(parcelReference)).Deliverystatus == 1,
        "normalized shipment and maximum-length tracking are applied");
    Check((await Parcel("parcel-life", "shipped", tracking18)).Status == AccountingCommandStatus.Duplicate,
        "identical parcel replay returns duplicate");
    Check((await Parcel("parcel-life", "delivered", "another")).ErrorCode == "AccountingTrackingCodeConflict"
        && (await Invoice(parcelReference)).Deliverystatus == 1 && (await Invoice(parcelReference)).Waybillnumber == tracking18,
        "conflicting tracking cannot overwrite code or advance state");
    Check((await Parcel("parcel-life", "delivered", " ")).Status == AccountingCommandStatus.Applied
        && (await Invoice(parcelReference)).Waybillnumber == tracking18,
        "delivery without tracking preserves the existing code");
    Check((await Parcel("parcel-life", "shipped", "old-code")).Status == AccountingCommandStatus.Duplicate
        && (await Invoice(parcelReference)).Deliverystatus == 2 && (await Invoice(parcelReference)).Waybillnumber == tracking18,
        "older shipment cannot regress delivery or overwrite tracking");
    Check((await Parcel("parcel-life", "preparing")).Status == AccountingCommandStatus.Duplicate
        && (await Cancel("parcel-life")).ErrorCode == "PhysicalReturnConfirmationRequired"
        && await Stock(products[4].Id) == 9,
        "late preparation cannot reopen stock-restoring cancellation");
    Check((await Parcel("parcel-life", "delivered", connection: 11)).ErrorCode == "AccountingOrderNotFound",
        "parcel lookup remains scoped to its connection");
    await using (var db = new HyperSqlServerContext(options))
    {
        Check((await Handler(db).ApplyParcelStatusAsync(new("event", scope with { TenantId = "other" }, 10,
            "parcel-life", "p", "delivered", null), default)).ErrorCode == "AccountingShopScopeMismatch",
            "parcel verifies canonical shop tenant");
        Check((await Handler(db).CancelOrderAsync(new("event", scope with { TenantId = "other" }, 10,
            "parcel-life", "cancel"), default)).ErrorCode == "AccountingShopScopeMismatch",
            "cancellation verifies canonical shop tenant");
        Check((await Handler(db).ApplyParcelStatusAsync(new("event", scope, 10,
            "parcel-life", "", "delivered", null), default)).ErrorCode == "InvalidAccountingParcel",
            "missing parcel identity cannot update invoice");
    }
    Check((await Parcel("first", "shipped", "track")).ErrorCode == "AccountingOrderCancelled",
        "cancelled order cannot be revived by shipment");
    Check((await Parcel("parcel-before-order", "delivered")).ErrorCode == "AccountingOrderNotFound",
        "parcel arriving before invoice waits for dependency");
    var delayedInvoice = await Apply(Order("parcel-before-order", products[4].Id, 1));
    Check((await Parcel("parcel-before-order", "delivered")).Status == AccountingCommandStatus.Applied
        && (await Parcel("parcel-before-order", "delivered", "late-track")).Status == AccountingCommandStatus.Applied
        && (await Invoice(delayedInvoice.InternalReference!)).Waybillnumber == "late-track",
        "parcel retry after invoice can advance directly and add missing tracking");
    var concurrentInvoice = await Apply(Order("parcel-concurrent", products[4].Id, 1));
    await Task.WhenAll(Parcel("parcel-concurrent", "shipped"), Parcel("parcel-concurrent", "delivered"));
    Check((await Invoice(concurrentInvoice.InternalReference!)).Deliverystatus == 2,
        "concurrent shipment and delivery finish at delivery");
    var cancelRaceInvoice = await Apply(Order("parcel-cancel-race", products[4].Id, 1));
    var stockBeforeRace = await Stock(products[4].Id);
    var cancelRace = await Task.WhenAll(Cancel("parcel-cancel-race"), Parcel("parcel-cancel-race", "shipped"));
    var raceInvoice = await Invoice(cancelRaceInvoice.InternalReference!);
    Check((raceInvoice.Status == 9 && raceInvoice.Deliverystatus == 0
            && cancelRace[0].Status == AccountingCommandStatus.Applied && cancelRace[1].ErrorCode == "AccountingOrderCancelled"
            && await Stock(products[4].Id) == stockBeforeRace + 1)
        || (raceInvoice.Status != 9 && raceInvoice.Deliverystatus == 1
            && cancelRace[0].ErrorCode == "PhysicalReturnConfirmationRequired" && cancelRace[1].Status == AccountingCommandStatus.Applied
            && await Stock(products[4].Id) == stockBeforeRace),
        "concurrent cancellation and shipment choose one consistent outcome");
    await using (var db = new HyperSqlServerContext(options))
    {
        var legacyDelivery = await db.TblSaleorders.SingleAsync(x => x.Saleorderid == long.Parse(concurrentInvoice.InternalReference!));
        legacyDelivery.Deliverystatus = null; await db.SaveChangesAsync();
    }
    Check((await Parcel("parcel-concurrent", "shipped")).ErrorCode == "AccountingDeliveryStateNeedsReview",
        "unknown legacy delivery state is not guessed");
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
    var resolvedOrder = Order("resolved-customer", products[1].Id, 1) with { AccountingCustomerId = customerId };
    var resolvedInvoice = await Apply(resolvedOrder);
    Check(resolvedInvoice.Status == AccountingCommandStatus.Applied && resolvedInvoice.StockCommitted
        && (await Apply(resolvedOrder)).Status == AccountingCommandStatus.Duplicate && await Stock(products[1].Id) == 9,
        "resolved customer proceeds through invoice and replay without another stock debit");
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
        Type[] keep = [typeof(SqlTblShop), typeof(SqlTblPerson), typeof(SqlTblDetailaccount), typeof(SqlTblProduct), typeof(SqlTblShopfiscalperiod), typeof(SqlTblSaleorder), typeof(SqlTblSaleorderitem)];
        // Keep real column/key/default mappings; only omit unrelated legacy tables
        // from the disposable fixture, not from the production accounting model.
        foreach (var entity in builder.Model.GetEntityTypes().ToArray())
            if (!keep.Contains(entity.ClrType)) builder.Ignore(entity.ClrType);
        foreach (var entity in builder.Model.GetEntityTypes()) entity.SetIsTableExcludedFromMigrations(false);
    }
}
