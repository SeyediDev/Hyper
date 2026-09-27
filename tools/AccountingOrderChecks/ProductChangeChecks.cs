using Hyper.Domain.Entities.Database;
using Hyper.Infrastructure.Data.Repository.Hyper;
using Hyperyek.Accounting.Contracts;
using Hyperyek.Accounting.Infrastructure;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

internal static class ProductChangeChecks
{
    public static async Task Run(DbContextOptions<HyperSqlServerContext> options, AccountingScope scope,
        int productId, int otherProductId, int legacyProductId, Action<bool, string> check)
    {
        await using var inspect = new HyperSqlServerContext(options);
        var database = inspect.Database.GetDbConnection().Database;
        if (!database.StartsWith("HyperAccountingChecks_", StringComparison.Ordinal)
            || !Guid.TryParseExact(database["HyperAccountingChecks_".Length..], "N", out _))
            throw new InvalidOperationException("Disposable accounting fixture required");
        async Task<AccountingCommandResult> Apply(ExternalProductChangedCommand command)
        {
            await using var db = new HyperSqlServerContext(options);
            return await new SqlAccountingCommandHandler(db, Options.Create(new AccountingCustomerOptions()),
                Options.Create(new AccountingInventoryOptions())).ApplyExternalProductChangedAsync(command, default);
        }
        Task<SqlTblProduct> Product() => inspect.TblProducts.AsNoTracking().SingleAsync(x => x.Id == productId);
        Task<int> Receipts() => inspect.Database.SqlQuery<int>($"SELECT COUNT(*) AS Value FROM dbo.AccountingProductChangeReceipts").SingleAsync();
        var initial = await Product();
        var command = new ExternalProductChangedCommand("product-1", scope, 111, productId, "external", null, "123", "new title", 2m, 999m, 10);
        check((await Apply(command)).ErrorCode == "AccountingProductReceiptSchemaMissing" && (await Product()).Name == initial.Name,
            "missing receipt schema fails closed before product mutation");
        var schema = await File.ReadAllTextAsync(Path.Combine(AppContext.BaseDirectory, "ensure-accounting-product-receipts.sql"));
        await inspect.Database.ExecuteSqlRawAsync(schema);
        await inspect.Database.ExecuteSqlRawAsync(schema);
        check(await Receipts() == 0, "accounting-owned receipt schema installs repeatably without seeded history");
        check((await Apply(command)).Status == AccountingCommandStatus.Applied && await Receipts() == 1,
            "product and command receipt commit together");
        var changed = await Product();
        check(changed.Name == "new title" && changed.Taxcode == "123" && changed.Saleprice == 2 && changed.Accountingstock == initial.Accountingstock,
            "name SKU price apply but marketplace inventory never overwrites accounting stock");
        check((await Apply(command with { Price = 2.000m, Inventory = 999.000m })).Status == AccountingCommandStatus.Duplicate && await Receipts() == 1,
            "fresh-context lost-ACK replay with equal decimal values has one durable effect");
        check((await Apply(command with { Title = "conflict" })).ErrorCode == "AccountingProductEventConflict",
            "event identity cannot be reused for changed content");
        check((await Apply(command with { HyperProductId = otherProductId })).ErrorCode == "AccountingProductEventConflict",
            "connection event identity cannot be reused for another accounting product");
        check((await Apply(command with { EventId = "other-id" })).ErrorCode == "AccountingProductVersionConflict",
            "a different event cannot reuse the source version");
        check((await Apply(command with { EventId = "older", SourceVersion = 9 })).ErrorCode == "StaleAccountingProductVersion",
            "previously unseen stale version cannot overwrite current product");
        var absent = command with { EventId = "product-2", SourceVersion = 11, Title = "title only", Sku = null, Price = null, Inventory = null };
        check((await Apply(absent)).Status == AccountingCommandStatus.Applied && (await Product()) is { Taxcode: "123", Saleprice: 2 },
            "absent SKU and price preserve accounting values");
        check((await Apply(command)).Status == AccountingCommandStatus.Duplicate && (await Product()).Name == "title only",
            "old exact replay after a newer update acknowledges without reverting the product");
        check((await Apply(absent with { EventId = "product-3", SourceVersion = 12, Price = 0 })).Status == AccountingCommandStatus.Applied
            && (await Product()).Saleprice == 0, "explicit zero price is retained");
        var count = await Receipts();
        foreach (var invalid in new[]
        {
            (command with { Title = " " }, "InvalidAccountingProductTitle"),
            (command with { Title = new string('x',201) }, "InvalidAccountingProductTitle"),
            (command with { Price = -1 }, "InvalidAccountingProductPrice"),
            (command with { Price = 0.000000001m }, "InvalidAccountingProductPrice"),
            (command with { Price = 1_000_000_000_000_000_000m }, "InvalidAccountingProductPrice"),
            (command with { Sku = new string('x',14) }, "AccountingProductSkuNeedsReview"),
            (command with { Sku = "کالا" }, "AccountingProductSkuNeedsReview"),
            (command with { Sku = "" }, "AccountingProductSkuNeedsReview"),
            (command with { ExternalVariantId = "variant" }, "AccountingProductVariantUnsupported"),
            (command with { SourceVersion = 0 }, "InvalidAccountingProductIdentity"),
            (command with { Scope = null! }, "InvalidAccountingProductIdentity"),
            (command with { Inventory = -1 }, "InvalidMarketplaceInventoryObservation")
        })
            check((await Apply(invalid.Item1)).ErrorCode == invalid.Item2, invalid.Item2);
        check(await Receipts() == count && (await Product()).Name == "title only", "rejected input changes neither receipts nor product");
        check((await Apply(command with { Scope = scope with { TenantId = "tenant-b" } })).ErrorCode == "AccountingShopScopeMismatch",
            "wrong tenant cannot mutate accounting product");
        check((await Apply(command with { Scope = scope with { ShopId = 99999 } })).ErrorCode == "AccountingShopScopeMismatch",
            "missing shop cannot accept product changes");
        check((await Apply(command with { HyperProductId = legacyProductId })).ErrorCode == "AccountingProductNotFound",
            "foreign-shop product remains inaccessible");
        check((await Apply(command with { Scope = new(8,"shop:8"), HyperProductId = legacyProductId })).Status == AccountingCommandStatus.Applied,
            "tenantless legacy product uses canonical shop scope consistently with reads");
        var race = command with { EventId = "race", SourceVersion = 20, Title = "race" };
        var raced = await Task.WhenAll(Apply(race), Apply(race));
        check(raced.Count(x => x.Status == AccountingCommandStatus.Applied) == 1 && raced.Count(x => x.Status == AccountingCommandStatus.Duplicate) == 1,
            "concurrent identical deliveries produce one product receipt");
        var versions = await Task.WhenAll(Apply(race with { EventId = "v21", SourceVersion = 21, Title = "v21" }),
            Apply(race with { EventId = "v22", SourceVersion = 22, Title = "v22" }));
        check(versions[1].Status == AccountingCommandStatus.Applied && (await Product()).Name == "v22",
            "concurrent source versions finish with the newest product state");
        check((await Apply(command with { ConnectionId = 112, SourceVersion = 1, Title = "other connection" })).Status == AccountingCommandStatus.Applied,
            "independent connection has its own event and version namespace");
        var beforeFailure = await Product();
        var receiptsBeforeFailure = await Receipts();
        // Failure injection exists only inside the verified disposable database.
        await inspect.Database.ExecuteSqlRawAsync("ALTER TABLE dbo.AccountingProductChangeReceipts ADD CONSTRAINT CK_Fixture_Reject900 CHECK(SourceVersion <> 900)");
        try
        {
            try { await Apply(command with { EventId = "rollback", SourceVersion = 900, Title = "must rollback" }); throw new Exception("Expected receipt insert failure"); }
            catch (SqlException ex) when (ex.Number == 547) { }
            check((await Product()).Name == beforeFailure.Name && await Receipts() == receiptsBeforeFailure,
                "receipt insert failure rolls back preceding product SQL update atomically");
        }
        finally
        {
            await inspect.Database.ExecuteSqlRawAsync("ALTER TABLE dbo.AccountingProductChangeReceipts DROP CONSTRAINT CK_Fixture_Reject900");
        }
        check((await Apply(command with { EventId = "rollback", SourceVersion = 900, Title = "recovered" })).Status == AccountingCommandStatus.Applied,
            "failed transaction leaves no phantom receipt and can be retried");
    }
}
