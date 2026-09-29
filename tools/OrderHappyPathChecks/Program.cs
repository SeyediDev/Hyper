using System.Net;
using System.Text;
using System.Text.Json;
using Hyper.Infrastructure.Data.Repository.Hyper;
using Hyper.Infrastructure.Features.Integrations;
using Microsoft.EntityFrameworkCore;

// Only application orchestration and HTTP serialization are real here. No SQL
// connection, provider network call, accounting document or stock write occurs.
var passed = 0;
void Check(bool value, string name)
{
    if (!value) throw new InvalidOperationException(name);
    passed++;
    Console.WriteLine("PASS " + name);
}
var steps = new List<string>();
var reservations = new ReservationProbe(steps);
using var transport = new AccountingReply(steps);
using var http = new HttpClient(transport) { BaseAddress = new Uri("https://accounting.fixture.invalid/") };
var commands = new HyperyekAccountingApiClient(http);
await using var db = new HyperIntegrationContext(new DbContextOptionsBuilder<HyperIntegrationContext>()
    .UseInMemoryDatabase("order-happy-" + Guid.NewGuid().ToString("N")).Options);
var shop = new OwnedIntegrationShop(7, "tenant-a");
var connection = new ExternalIntegrationConnection { ShopId = shop.ShopId, TenantId = shop.TenantId,
    Provider = IntegrationProvider.Custom, DisplayName = "fixture", AccountIdentifier = "fixture", CredentialsJson = "{}" };
db.Add(connection);
await db.SaveChangesAsync();
db.AddRange(new ExternalProductMapping { ConnectionId = connection.Id, ShopId = shop.ShopId,
    HyperProductId = 41, ExternalProductId = "product-a", IsActive = true },
    new IntegrationCustomerMapping { ShopId = shop.ShopId, TenantId = shop.TenantId,
        ExternalCustomerId = "buyer-a", PersonId = 91 });
await db.SaveChangesAsync();
var receipts = new ReceiptProbe(steps, connection);
var dispatcher = new IntegrationBusinessEventDispatcher(commands, new UnregisteredIntegrationEngagementPort(), reservations, db,
    orderMappingReceipt: receipts);

async Task Dispatch(string eventId, string eventType, IntegrationSyncItem item, object payload)
{
    db.Add(new IntegrationWebhookInbox { ConnectionId = connection.Id, ExternalEventId = eventId,
        EventType = eventType, PayloadJson = JsonSerializer.Serialize(payload) });
    await db.SaveChangesAsync();
    var result = await dispatcher.DispatchAsync(new() { ConnectionId = connection.Id, ShopId = shop.ShopId,
        TenantId = shop.TenantId, EventId = eventId, Item = item }, connection, default);
    Check(result.Compared == 1 && result.Differences.Count == 0, eventId + " completes without a pending dependency");
}
object Sale(string orderId) => new { externalOrderId = orderId, externalCustomerId = "buyer-a",
    paymentStatus = "Paid", totalAmount = 25m,
    lines = new[] { new { externalProductId = "product-a", quantity = 2.5m, unitPrice = 10m } } };

await Dispatch("sale-held", "order.vendor.created", IntegrationSyncItem.Sale, Sale("order-held"));
var heldKey = IntegrationReservationIdentity.ForOrder(shop, connection.Id, "order-held");
Check(steps.SequenceEqual(["preflight", "reserve", "http:vendor-orders", "receipt"]) && reservations.Held.Contains(heldKey)
    && reservations.LastShop == shop && reservations.LastLines.Single().HyperProductId == 41,
    "paid sale resolves product mapping and reserves before accounting");
using (var body = JsonDocument.Parse(transport.LastBody))
{
    var root = body.RootElement;
    Check(root.GetProperty("accountingCustomerId").GetInt32() == 91
        && root.GetProperty("connectionId").GetInt64() == connection.Id
        && root.GetProperty("scope").GetProperty("shopId").GetInt32() == shop.ShopId
        && root.GetProperty("scope").GetProperty("tenantId").GetString() == shop.TenantId
        && root.GetProperty("lines")[0].GetProperty("hyperProductId").GetInt32() == 41
        && root.GetProperty("lines")[0].GetProperty("quantity").GetDecimal() == 2.5m
        && root.GetProperty("totalAmount").GetDecimal() == 25m,
        "accounting HTTP request preserves customer mapping, scope and amounts");
}
steps.Clear();
await Dispatch("cancel-held", "order.vendor.cancelled", IntegrationSyncItem.Sale,
    new { externalOrderId = "order-held", reason = "customer-request" });
Check(steps.SequenceEqual(["http:orders/cancel", "release"]) && !reservations.Held.Contains(heldKey)
    && reservations.LastKey == heldKey, "acknowledged cancellation releases the same order reservation");

steps.Clear();
transport.StockCommitted = true;
await Dispatch("sale-committed", "order.vendor.created", IntegrationSyncItem.Sale, Sale("order-committed"));
var committedKey = IntegrationReservationIdentity.ForOrder(shop, connection.Id, "order-committed");
Check(steps.SequenceEqual(["preflight", "reserve", "http:vendor-orders", "commit", "receipt"])
    && reservations.Committed.Contains(committedKey) && !reservations.Held.Contains(committedKey),
    "acknowledged stock write consumes the hold");
Check(receipts.Preflights == 2 && receipts.Receipts == 2,
    "each sale preflights its scope and records its acknowledged invoice");

steps.Clear();
await Dispatch("purchase", "order.customer.created", IntegrationSyncItem.Purchase,
    new { externalOrderId = "purchase-1", paymentStatus = "Paid", totalAmount = 30m,
        lines = new[] { new { hyperProductId = 41, externalProductId = "product-a", quantity = 3m, unitPrice = 10m } } });
Check(steps.SequenceEqual(["http:customer-orders"]), "customer purchase reaches its accounting route");
Console.WriteLine($"{passed} happy-path checks passed. SQL and live services were not used.");

sealed class ReservationProbe(List<string> steps) : IIntegrationInventoryReservation
{
    public HashSet<string> Held { get; } = [];
    public HashSet<string> Committed { get; } = [];
    public string? LastKey;
    public OwnedIntegrationShop? LastShop;
    public IReadOnlyCollection<IntegrationOrderLineCommand> LastLines = [];
    public Task ReserveAsync(OwnedIntegrationShop shop, string key, IReadOnlyCollection<IntegrationOrderLineCommand> lines, CancellationToken ct)
    { steps.Add("reserve"); LastShop = shop; LastKey = key; LastLines = lines; Held.Add(key); return Task.CompletedTask; }
    public Task ReleaseAsync(OwnedIntegrationShop shop, string key, CancellationToken ct)
    { steps.Add("release"); LastShop = shop; LastKey = key; Held.Remove(key); return Task.CompletedTask; }
    public Task CommitAsync(OwnedIntegrationShop shop, string key, CancellationToken ct)
    { steps.Add("commit"); LastShop = shop; LastKey = key; Held.Remove(key); Committed.Add(key); return Task.CompletedTask; }
}
// This observer checks orchestration only. OrderMappingChecks exercises the
// production SQL receipt store and its concurrency/identity guarantees.
sealed class ReceiptProbe(List<string> steps, ExternalIntegrationConnection expected) : IIntegrationOrderMappingReceipt
{
    public int Preflights;
    public int Receipts;
    public Task PreflightAsync(ExternalIntegrationConnection connection, IntegrationVendorOrderCommand command, CancellationToken ct)
    {
        Scope(connection, command);
        steps.Add("preflight");
        Preflights++;
        return Task.CompletedTask;
    }
    public Task RecordAsync(ExternalIntegrationConnection connection, IntegrationVendorOrderCommand command,
        BusinessCommandResult result, CancellationToken ct)
    {
        Scope(connection, command);
        if (result.Status != BusinessCommandStatus.Applied || result.InternalReference != "12001")
            throw new InvalidOperationException("UnexpectedAccountingReceipt");
        steps.Add("receipt");
        Receipts++;
        return Task.CompletedTask;
    }
    private void Scope(ExternalIntegrationConnection connection, IntegrationVendorOrderCommand command)
    {
        if (connection.Id != expected.Id || command.ConnectionId != expected.Id
            || command.ShopId != expected.ShopId || command.TenantId != expected.TenantId
            || command.ExternalOrderId is not ("order-held" or "order-committed"))
            throw new InvalidOperationException("UnexpectedReceiptScope");
    }
}
sealed class AccountingReply(List<string> steps) : HttpMessageHandler
{
    public bool StockCommitted;
    public string LastBody = "";
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
    {
        const string prefix = "/api/hyperyek/v1/accounting/";
        var path = request.RequestUri!.AbsolutePath;
        if (request.Method != HttpMethod.Post || !path.StartsWith(prefix, StringComparison.Ordinal))
            throw new InvalidOperationException("UnexpectedAccountingRequest");
        var route = path[prefix.Length..];
        if (route is not ("vendor-orders" or "customer-orders" or "orders/cancel"))
            throw new InvalidOperationException("UnexpectedAccountingRoute");
        steps.Add("http:" + route);
        LastBody = await request.Content!.ReadAsStringAsync(ct);
        return new(HttpStatusCode.OK) { Content = new StringContent(JsonSerializer.Serialize(new
        { status = 1, internalReference = "12001", stockCommitted = StockCommitted }), Encoding.UTF8, "application/json") };
    }
}
