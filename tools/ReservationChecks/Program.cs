using Hyper.Infrastructure.Data.Repository.Hyper;
using Hyper.Infrastructure.Features.Integrations;
using Hyper.Integration.Domain.Entities.Integrations;
using Hyper.Integration.Domain.Features.Integrations;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Options;

var input = Environment.GetEnvironmentVariable("RESERVATION_TEST_SQL") ?? throw new Exception("RESERVATION_TEST_SQL required");
var database = "HyperReservationChecks_" + Guid.NewGuid().ToString("N");
var cs = new SqlConnectionStringBuilder(input) { InitialCatalog = database };
var options = new DbContextOptionsBuilder<HyperIntegrationContext>().UseSqlServer(cs.ConnectionString)
    .ReplaceService<IModelCustomizer, TestSchemaCustomizer>().Options;
var shop = new OwnedIntegrationShop(7, "tenant-a");
var checks = 0;
void Check(bool ok, string name) { if (!ok) throw new Exception(name); Console.WriteLine("PASS " + name); checks++; }
async Task Reject(Func<Task> action, string code)
{
    try { await action(); }
    catch (Exception ex) when (ex.Message == code) { Check(true, code); return; }
    throw new Exception("Expected " + code);
}
IntegrationInventoryReservation Service(HyperIntegrationContext db, bool verified = true) =>
    new(db, new Catalog(), Options.Create(new IntegrationInventoryCaptureOptions { AccountingStockSourceVerified = verified }));
string Key(string order, long connection = 10) => IntegrationReservationIdentity.ForOrder(shop, connection, order);
IntegrationOrderLineCommand Line(int product, decimal quantity) => new(product, product.ToString(), null, quantity, 1);
await using var setup = new HyperIntegrationContext(options);
try
{
    await setup.Database.EnsureCreatedAsync();
    var key = Key("multi");
    var lines = new[] { Line(1, 2), Line(2, 3), Line(1, 1) };
    await Service(setup).ReserveAsync(shop, key, lines, default);
    Check(await setup.InventoryReservationLogs.CountAsync() == 2, "multi-product order with repeated product");
    await Service(setup).ReserveAsync(shop, key, lines, default);
    Check(await setup.InventoryReservationLogs.SumAsync(x => x.Quantity) == 6, "exact replay does not reserve twice");
    await Reject(() => Service(setup).ReserveAsync(shop, key, [Line(1, 4)], default), "ReservationContentChanged");
    await Reject(() => Service(setup, false).ReserveAsync(shop, Key("unverified"), lines, default), "AccountingStockSourceUnverified");
    await Service(setup).ReserveAsync(shop, Key("multi", 11), [Line(1, 1)], default);
    await Service(setup).ReleaseAsync(shop, key, default);
    await Service(setup).ReleaseAsync(shop, key, default);
    Check(await setup.InventoryReservationLogs.Where(x => x.Status == 0).SumAsync(x => x.Quantity) == 1,
        "release all lines once, preserving another connection");
    await Reject(() => Service(setup).ReserveAsync(shop, key, lines, default), "ReservationAlreadyReleased");
    Check(Key("case") != Key("CASE") && Key("case") != Key("case", 11)
        && Key("case") != IntegrationReservationIdentity.ForOrder(shop with { TenantId = "tenant-b" }, 10, "case"), "scoped case-sensitive identity");
    async Task<bool> Race(string order)
    {
        await using var db = new HyperIntegrationContext(options);
        try { await Service(db).ReserveAsync(shop, Key(order), [Line(3, 6)], default); return true; }
        catch (IntegrationProviderException ex) when (ex.Message == "InsufficientAvailableInventory") { return false; }
    }
    var race = await Task.WhenAll(Race("race-a"), Race("race-b"));
    Check(race.Count(x => x) == 1, "concurrent orders cannot reserve 12 from 10");
    Check(await setup.InventoryReservationLogs.Where(x => x.HyperProductId == 3 && x.Status == 0).SumAsync(x => x.Quantity) == 6,
        "failed reservation leaves no partial rows");
    var connection = new ExternalIntegrationConnection { ShopId = 7, TenantId = shop.TenantId,
        DisplayName = "test", AccountIdentifier = "test", CredentialsJson = "{}", Provider = IntegrationProvider.Custom };
    setup.ExternalIntegrationConnections.Add(connection);
    await setup.SaveChangesAsync();
    setup.ExternalProductMappings.Add(new() { ConnectionId = connection.Id, ShopId = 7, HyperProductId = 4, ExternalProductId = "external-4" });
    setup.IntegrationCustomerMappings.Add(new() { ShopId = 7, TenantId = shop.TenantId, ExternalCustomerId = "buyer", PersonId = 9 });
    await setup.SaveChangesAsync();
    var accounting = new Commands();
    async Task Dispatch(string eventId, string order, string payment)
    {
        setup.IntegrationWebhookInbox.Add(new() { ConnectionId = connection.Id, ExternalEventId = eventId, EventType = "order.vendor.created",
            PayloadJson = System.Text.Json.JsonSerializer.Serialize(new { externalOrderId = order, externalCustomerId = "buyer",
                paymentStatus = payment, totalAmount = 2, lines = new[] { new { externalProductId = "external-4", hyperProductId = 999, quantity = 2, unitPrice = 1 } } }) });
        await setup.SaveChangesAsync();
        var dispatcher = new IntegrationBusinessEventDispatcher(accounting, new UnregisteredIntegrationEngagementPort(), Service(setup), setup);
        await dispatcher.DispatchAsync(new() { ConnectionId = connection.Id, ShopId = 7, TenantId = shop.TenantId, EventId = eventId, Item = IntegrationSyncItem.Sale }, connection, default);
    }
    await Dispatch("unpaid", "unpaid", "Pending");
    Check(accounting.Calls == 0 && !await setup.InventoryReservationLogs.AnyAsync(x => x.HyperProductId == 4), "unpaid sale neither reserves nor writes accounting");
    accounting.Timeout = true;
    try { await Dispatch("timeout", "paid", "Paid"); throw new Exception("Expected timeout"); } catch (HttpRequestException) { }
    Check(await setup.InventoryReservationLogs.Where(x => x.HyperProductId == 4 && x.Status == 0).SumAsync(x => x.Quantity) == 2, "timeout retains hold");
    accounting.Timeout = false;
    await Dispatch("retry", "paid", "Paid");
    Check(accounting.Calls == 2 && accounting.LastProduct == 4 && await setup.InventoryReservationLogs.CountAsync(x => x.HyperProductId == 4) == 1,
        "replay resolves mapping and reuses hold");
    Console.WriteLine($"{checks} SQL/dispatcher checks passed.");
}
finally
{
    if (setup.Database.GetDbConnection().Database == database && database.StartsWith("HyperReservationChecks_", StringComparison.Ordinal))
        await setup.Database.EnsureDeletedAsync();
}
sealed class Catalog : IIntegrationPlatformCatalogPort
{
    public Task<IReadOnlyList<IntegrationPlatformProduct>> GetProductsAsync(int shopId, string tenantId, CancellationToken ct) =>
        Task.FromResult<IReadOnlyList<IntegrationPlatformProduct>>(Enumerable.Range(1, 4).Select(id => new IntegrationPlatformProduct(id, "product", null, 1, 10, true, true, null)).ToArray());
}
sealed class TestSchemaCustomizer(ModelCustomizerDependencies dependencies) : ModelCustomizer(dependencies)
{
    public override void Customize(ModelBuilder builder, DbContext context)
    {
        base.Customize(builder, context);
        // Production provisions tables with SQL scripts. Enable only fixture DDL
        // generation here; keys, precision and relationship mappings stay intact.
        foreach (var entity in builder.Model.GetEntityTypes()) entity.SetIsTableExcludedFromMigrations(false);
    }
}
sealed class Commands : IIntegrationBusinessCommandPort
{
    public bool Timeout; public int Calls; public int LastProduct;
    public Task<BusinessCommandResult> ApplyVendorOrderAsync(IntegrationVendorOrderCommand c, CancellationToken ct)
    { Calls++; LastProduct = c.Lines.Single().HyperProductId; if (Timeout) throw new HttpRequestException(); return Task.FromResult(new BusinessCommandResult(BusinessCommandStatus.Duplicate)); }
    public Task<BusinessCommandResult> ApplyCounterpartyAsync(IntegrationCounterpartyCommand c, CancellationToken ct) => throw new NotSupportedException();
    public Task<BusinessCommandResult> ApplyCustomerOrderAsync(IntegrationCustomerOrderCommand c, CancellationToken ct) => throw new NotSupportedException();
    public Task<BusinessCommandResult> CancelOrderAsync(IntegrationOrderCancellationCommand c, CancellationToken ct) => throw new NotSupportedException();
    public Task<BusinessCommandResult> ApplyParcelStatusAsync(IntegrationParcelStatusCommand c, CancellationToken ct) => throw new NotSupportedException();
    public Task<BusinessCommandResult> ApplyExternalProductChangedAsync(IntegrationExternalProductChangedCommand c, CancellationToken ct) => throw new NotSupportedException();
}
