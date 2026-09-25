using Hyper.Integration.Domain.Entities.Integrations;
using Hyper.Integration.Domain.Features.Integrations;

var checks = 0;
void Check(bool condition, string name)
{
    if (!condition) throw new InvalidOperationException(name);
    checks++;
    Console.WriteLine($"PASS {name}");
}

void Reject(Action action, string name)
{
    try { action(); }
    catch (ArgumentException) { Check(true, name); return; }
    catch (InvalidOperationException) { Check(true, name); return; }
    catch (NotSupportedException) { Check(true, name); return; }
    throw new InvalidOperationException(name);
}

var shop = new OwnedIntegrationShop(100, "tenant-100");
Reject(() => IntegrationScenarioRules.Validate(shop, 1,
    new("event", (IntegrationSyncItem)99, IntegrationSyncTrigger.Manual)),
    "unknown scenario item rejected");
Reject(() => IntegrationScenarioRules.Validate(shop, 1,
    new(" event", IntegrationSyncItem.Product, IntegrationSyncTrigger.Manual)),
    "noncanonical event id rejected");
Reject(() => IntegrationScenarioRules.Validate(shop, 1,
    new("event\n1", IntegrationSyncItem.Product, IntegrationSyncTrigger.Manual)),
    "control character rejected");

var local = new[]
{
    new IntegrationLocalProduct(1, "One", 4, "SKU-1", 100),
    new IntegrationLocalProduct(2, "Two", 0)
};
var remote = new[]
{
    new ExternalCatalogItem("11", "SKU-2", "Changed", 90, 8, null),
    new ExternalCatalogItem("12", null, "Remote", null, 1, null)
};
var mapping = new ExternalProductMapping { Id = 1, HyperProductId = 1, ExternalProductId = "11" };
var comparison = IntegrationCatalogComparison.Compare(local, remote, [mapping], IntegrationSyncItem.Product);
Check(comparison.Compared == 1 && comparison.Differences.Any(x => x.Code == "ProductTitleMismatch"),
    "product drift detected");
Check(comparison.Differences.Any(x => x.Code == "ProductSkuMismatch")
    && comparison.Differences.Any(x => x.Code == "ProductPriceMismatch"),
    "catalog sku and price drift detected");
Check(comparison.Differences.Any(x => x.Code == "LocalProductUnmapped")
    && comparison.Differences.Any(x => x.Code == "ExternalProductUnmapped"),
    "unmapped records detected on both sides");
Check(IntegrationCatalogComparison.Compare(local, remote, [mapping], IntegrationSyncItem.Inventory)
    .Differences.Any(x => x.Code == "InventoryMismatch"), "inventory drift detected");
Check(IntegrationCatalogComparison.Compare(local, [], [mapping], IntegrationSyncItem.Product)
    .Differences.Any(x => x.Code == "ExternalProductMissing"), "remote deletion detected");
Check(IntegrationCatalogComparison.Compare([], remote, [mapping], IntegrationSyncItem.Product)
    .Differences.Any(x => x.Code == "LocalProductMissing"), "local deletion detected");
Check(IntegrationCatalogComparison.Compare(local, remote, [mapping, mapping], IntegrationSyncItem.Product)
    .Differences.Any(x => x.Code == "DuplicateMapping"), "duplicate mapping is not merged");

Check(IntegrationAvailableInventory.Calculate(3, 5, true) == 0
    && IntegrationAvailableInventory.Calculate(3, 0, false) == 0,
    "available inventory is never negative or sellable when disabled");
Check(IntegrationRetryPolicy.Delay(1, TimeSpan.FromMinutes(4)) == TimeSpan.FromMinutes(4),
    "provider retry-after is honored");
Check(IntegrationRetryPolicy.Delay(4, null) > IntegrationRetryPolicy.Delay(1, null),
    "retry delay uses exponential backoff");

var resolver = new IntegrationStrategyResolver([new ResolverAdapter(IntegrationProvider.Basalam, true, true)]);
Check(ReferenceEquals(resolver.Resolve(IntegrationProvider.Basalam, IntegrationCredentialType.OAuth2),
    resolver.Resolve(IntegrationProvider.Basalam, IntegrationCredentialType.OAuth2)),
    "provider strategy resolves supported credential");
Reject(() => resolver.Resolve(IntegrationProvider.Digikala, IntegrationCredentialType.ApiKey),
    "unsupported provider strategy rejected");
Reject(() => new IntegrationStrategyResolver([
    new ResolverAdapter(IntegrationProvider.Basalam, true, true),
    new ResolverAdapter(IntegrationProvider.Basalam, true, true)])
    .Resolve(IntegrationProvider.Basalam, IntegrationCredentialType.OAuth2),
    "ambiguous provider strategy rejected");

var workflow = new IntegratedSaleWorkflow();
var boothCommand = new CreateIntegratedSale("sale-1", SaleChannel.Booth,
    new SaleCustomerIdentity(42, "basalam-user-42", "09120000000", null, false),
    "order-42", [new SaleLine(10, 2, 125)], 250, SalePaymentStatus.Pending);
var boothSale = workflow.Create(boothCommand, DateTime.UtcNow);
Check(boothSale.Status == SaleWorkflowStatus.AwaitingPayment, "booth sale waits for payment");
Check(ReferenceEquals(boothSale, workflow.Create(boothCommand)), "sale event is idempotent");
Reject(() => workflow.Transition("sale-1", SaleWorkflowStatus.Reserved), "unpaid booth sale cannot reserve stock");
workflow.ConfirmPayment("sale-1");
Check(workflow.Transition("sale-1", SaleWorkflowStatus.Reserved).Status == SaleWorkflowStatus.Reserved,
    "paid booth sale can reserve stock");
Reject(() => workflow.Create(new CreateIntegratedSale("sale-public", SaleChannel.Booth, null,
    "order-public", [new SaleLine(10, 1, 10)], 10, SalePaymentStatus.Paid)),
    "booth sale requires identified customer");
var storeSale = workflow.Create(new CreateIntegratedSale("sale-store", SaleChannel.Store, null,
    null, [new SaleLine(11, 1, 20)], 20, SalePaymentStatus.Pending));
Check(storeSale.Customer.CustomerId == IntegratedSaleWorkflow.PublicCustomerId
    && storeSale.Status == SaleWorkflowStatus.Confirmed, "store sale uses public customer safely");
Check(workflow.Events.Any(x => x.Type == "SaleConfirmed"), "sale workflow emits domain events");

Console.WriteLine($"{checks} integration-domain checks passed.");

sealed class ResolverAdapter(IntegrationProvider provider, bool oauth, bool implemented) : IExternalIntegrationAdapter
{
    public IntegrationProvider Provider => provider;
    public bool IsImplemented => implemented;
    public bool SupportsCredentialType(IntegrationCredentialType credentialType) =>
        oauth && credentialType == IntegrationCredentialType.OAuth2;
    public Task<IReadOnlyCollection<ExternalCatalogItem>> ReadCatalogAsync(
        ExternalIntegrationConnection connection, CancellationToken cancellationToken) =>
        Task.FromResult<IReadOnlyCollection<ExternalCatalogItem>>([]);
    public Task PublishInventoryAsync(ExternalIntegrationConnection connection,
        IReadOnlyCollection<ExternalInventoryUpdate> updates, CancellationToken cancellationToken) =>
        Task.CompletedTask;
}
