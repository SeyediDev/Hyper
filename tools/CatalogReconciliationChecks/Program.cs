using System.Text.Json;
using Hyper.Infrastructure.Data.Repository.Hyper;
using Hyper.Infrastructure.Features.Integrations;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Options;
using VersionSource = Hyper.Integration.Contracts.IntegrationVersionSource;

try { return await Run(); }
catch (Exception e) { Console.Error.WriteLine(e); return 1; }

static async Task<int> Run()
{
    var name = "HyperCatalogChecks_" + Guid.NewGuid().ToString("N");
    var builder = new SqlConnectionStringBuilder(Environment.GetEnvironmentVariable("SYNC_CHECKS_CONNECTION")
        ?? "Server=localhost;Integrated Security=true;TrustServerCertificate=true;Connect Timeout=120") { InitialCatalog = "master", Pooling = false };
    var masterConnection = builder.ConnectionString;
    async Task Execute(string sql)
    {
        await using var master = new SqlConnection(masterConnection);
        await master.OpenAsync();
        await using var command = new SqlCommand(sql, master) { CommandTimeout = 120 };
        await command.ExecuteNonQueryAsync();
    }
    var failed = false;
    try
    {
        Console.WriteLine("Creating isolated fixture " + name);
        await Execute($"CREATE DATABASE [{name}]");
        builder.InitialCatalog = name;
        var options = new DbContextOptionsBuilder<HyperIntegrationContext>().UseSqlServer(builder.ConnectionString,
            sql => sql.CommandTimeout(120)).ReplaceService<IModelCustomizer, FixtureSchema>().Options;
        await using var db = new HyperIntegrationContext(options);
        await db.Database.EnsureCreatedAsync();
        // Exercise the actual upgrade from a pre-creation schema, not only EF's
        // generated model. This database is uniquely generated above for this run.
        if (db.Database.GetDbConnection().Database != name) throw new InvalidOperationException("Unsafe migration test target");
        await db.Database.ExecuteSqlRawAsync("DROP TABLE dbo.IntegrationProductCreations; ALTER TABLE dbo.IntegrationScenarioJobs ADD CONSTRAINT CK_IntegrationScenarioJobs_Item CHECK (Item BETWEEN 1 AND 8);");
        var creationSchema = await File.ReadAllTextAsync(Path.Combine(AppContext.BaseDirectory, "ensure-integration-product-creations.sql"));
        await db.Database.ExecuteSqlRawAsync(creationSchema);
        await db.Database.ExecuteSqlRawAsync(creationSchema);
        var connection = new ExternalIntegrationConnection { ShopId = 7, TenantId = "tenant-a", Provider = IntegrationProvider.Basalam,
            CredentialType = IntegrationCredentialType.BearerToken, AccountIdentifier = "71", DisplayName = "fixture", CredentialsJson = "{}" };
        db.ExternalIntegrationConnections.Add(connection);
        await db.SaveChangesAsync();
        var mapping = new ExternalProductMapping { ConnectionId = connection.Id, ShopId = 7, HyperProductId = 44, ExternalProductId = "12" };
        db.ExternalProductMappings.Add(mapping);
        db.InventoryReservationLogs.AddRange(
            new() { ShopId = 7, HyperProductId = 44, Quantity = 2, ReservationKey = "held", Status = 0, Source = "fixture" },
            new() { ShopId = 7, HyperProductId = 44, Quantity = 5, ReservationKey = "committed", Status = 2, Source = "fixture" });
        await db.SaveChangesAsync();
        var source = new AccountingFixture();
        var remote = new ProviderFixture();
        var resolver = new IntegrationStrategyResolver([remote]);
        var checks = 0;
        void Check(bool ok, string message) { if (!ok) throw new InvalidOperationException(message); Console.WriteLine($"PASS {++checks}: {message}"); }
        (IntegrationScenarioQueue Queue, IntegrationOutbox Outbox) Services(HyperIntegrationContext context, bool stockVerified = true)
        {
            var outbox = new IntegrationOutbox(context, resolver);
            var settings = Options.Create(new IntegrationInventoryCaptureOptions { AccountingStockSourceVerified = stockVerified });
            var capture = new IntegrationInventoryCapture(context, outbox, resolver, settings, source);
            var dispatcher = new IntegrationBusinessEventDispatcher(source, new UnregisteredIntegrationEngagementPort(), null!, context, resolver);
            var processor = new IntegrationScenarioProcessor(context, resolver, capture, settings,
                Options.Create(new BasalamOAuthSettings()), null!, dispatcher, source);
            return (new(context, processor), outbox);
        }
        async Task<long> Enqueue(IntegrationSyncItem item, IntegrationSyncTrigger trigger = IntegrationSyncTrigger.Manual)
        {
            await using var context = new HyperIntegrationContext(options);
            return await Services(context).Queue.EnqueueAsync(new(7, "tenant-a"), connection.Id,
                new(Guid.NewGuid().ToString("N"), item, trigger), default);
        }
        async Task<IntegrationScenarioJob> Process(long id, bool stockVerified = true)
        {
            await using var context = new HyperIntegrationContext(options);
            await context.IntegrationScenarioJobs.Where(x => x.Id == id).ExecuteUpdateAsync(s => s.SetProperty(x => x.NextAttemptAtUtc, DateTime.UtcNow.AddSeconds(-1)));
            Check(await Services(context, stockVerified).Queue.ProcessConnectionAsync(connection.Id, default), "worker processes durable scenario in fresh context");
            return await context.IntegrationScenarioJobs.AsNoTracking().SingleAsync(x => x.Id == id);
        }
        async Task Deliver()
        {
            await using var context = new HyperIntegrationContext(options);
            Check(await Services(context).Outbox.ProcessConnectionAsync(connection.Id, default), "independent outbox delivers persisted operation");
        }
        async Task<int> Messages() => await db.IntegrationOutbox.CountAsync();
        async Task Switch(VersionSource target)
        {
            await using var context = new HyperIntegrationContext(options);
            var api = new IntegrationVersionSourceApi(context);
            var scope = new Hyper.Integration.Contracts.IntegrationConnectionCommandRequest(7, "tenant-a", connection.Id);
            var old = (await api.ReadAsync(scope, mapping.Id, default))!;
            Check((await api.ChangeAsync(scope, mapping.Id, new(target, old.Source, old.LastVersion), default))?.Status == "Changed", "explicit source switch above current high-water mark");
        }

        var product = await Enqueue(IntegrationSyncItem.Product, IntegrationSyncTrigger.Initial);
        var job = await Process(product, stockVerified: false);
        var receipt = JsonSerializer.Deserialize<IntegrationScenarioResult>(job.ResultJson!)!;
        Check(job.Status == IntegrationScenarioStatus.Pending && job.ErrorCode == "AwaitingOutboxDelivery"
            && receipt.OutboxMessageIds?.Count == 1 && receipt.Enqueued == 1 && job.CompletedAtUtc is null,
            "initial product without inbox queues one title+price patch and does not falsely complete");
        Check(source.Commands == 0 && remote.ProductWrites == 0 && await Messages() == 1,
            "accounting-to-provider reconciliation does not call inbound accounting command or publish inline");
        for (var i = 0; i < 7; i++) job = await Process(product, stockVerified: false);
        Check(job.Status == IntegrationScenarioStatus.Pending && job.Attempts == 0 && await Messages() == 1,
            "delivery polling survives retry budget without duplicate messages");
        await Deliver();
        job = await Process(product, stockVerified: false);
        Check(job.Status == IntegrationScenarioStatus.Completed && remote.Title == source.Product.Name && remote.Price == 125
            && remote.Stock == 3 && remote.ProductWrites == 1, "read-back confirms title/base price without touching stock");

        var inventory = await Enqueue(IntegrationSyncItem.Inventory);
        job = await Process(inventory);
        Check(job.Status == IntegrationScenarioStatus.Pending && await Messages() == 2, "inventory shares capture ownership after product reconciliation");
        await Deliver();
        job = await Process(inventory);
        Check(job.Status == IntegrationScenarioStatus.Completed && remote.Stock == 8 && remote.StockWrites == 1,
            "gross10 minus active2, not committed5, is delivered and verified");
        var versions = await db.IntegrationOutbox.OrderBy(x => x.Id).Select(x => x.SourceVersion).ToArrayAsync();
        Check(versions.SequenceEqual([1L, 2L]), "product/inventory allocate one increasing stream");
        Check((await Process(await Enqueue(IntegrationSyncItem.Product))).Status == IntegrationScenarioStatus.Completed
            && (await Process(await Enqueue(IntegrationSyncItem.Inventory))).Status == IntegrationScenarioStatus.Completed
            && await Messages() == 2, "repeating already synchronized jobs makes no provider writes");

        var request = new Hyper.Integration.Contracts.IntegrationCatalogReconciliationRequest(7, "tenant-a", connection.Id, Guid.NewGuid());
        var syncApi = new IntegrationSyncApi(db, null!, Services(db).Queue);
        var pair = (await syncApi.ReconcileCatalogAsync(request, default))!;
        var samePair = (await syncApi.ReconcileCatalogAsync(request, default))!;
        Check(pair.Product.JobId == samePair.Product.JobId && pair.Inventory.JobId == samePair.Inventory.JobId
            && pair.Product.Status == "Pending" && pair.Inventory.Status == "Pending",
            "combined start uses stable request identity and atomically queues two jobs without duplicates");
        Check(await syncApi.ReconcileCatalogAsync(request with { TenantId = "other" }, default) is null,
            "combined start cannot select a foreign connection");
        await Process(pair.Product.JobId); await Process(pair.Inventory.JobId);
        var donePair = (await syncApi.ReconcileCatalogAsync(request, default))!;
        Check(donePair.Product.Status == "Completed" && donePair.Inventory.Status == "Completed" && await Messages() == 2,
            "repeating the same start request reports actual completion without starting new jobs");

        var conflictRequest = request with { RequestId = Guid.NewGuid() };
        var conflictPrefix = $"catalog:{conflictRequest.RequestId:N}:";
        var conflictingJob = await Services(db).Queue.EnqueueAsync(new(7, "tenant-a"), connection.Id,
            new(conflictPrefix + "inventory", IntegrationSyncItem.Product, IntegrationSyncTrigger.Manual), default);
        var rejected = false;
        try { await syncApi.ReconcileCatalogAsync(conflictRequest, default); }
        catch (InvalidOperationException e) when (e.Message == "EventIdentityConflict") { rejected = true; }
        Check(rejected && !await db.IntegrationScenarioJobs.AnyAsync(x => x.EventId == conflictPrefix + "product"),
            "second-job conflict rolls back the first job instead of leaving a partial pair");
        await Process(conflictingJob);

        source.Product = source.Product with { Name = "Rice changed", Price = 0 };
        product = await Enqueue(IntegrationSyncItem.Product, IntegrationSyncTrigger.Periodic);
        await Process(product);
        await Deliver();
        Check((await Process(product)).Status == IntegrationScenarioStatus.Completed && remote.Price == 0 && remote.Stock == 8,
            "periodic product scenario carries explicit zero price and changed title");
        source.Product = source.Product with { CanSell = false };
        inventory = await Enqueue(IntegrationSyncItem.Inventory);
        await Process(inventory); await Deliver();
        Check((await Process(inventory)).Status == IntegrationScenarioStatus.Completed && remote.Stock == 0,
            "non-sellable source publishes and verifies explicit zero stock");

        var count = await Messages();
        job = await Process(await Enqueue(IntegrationSyncItem.Inventory), stockVerified: false);
        Check(job.Status == IntegrationScenarioStatus.DeadLetter && job.ErrorCode == "AccountingStockSourceUnverified" && await Messages() == count,
            "unverified accounting stock remains a hard gate");
        job = await Process(await Enqueue(IntegrationSyncItem.Product, IntegrationSyncTrigger.BoothChanged));
        Check(job.Status == IntegrationScenarioStatus.DeadLetter && job.ErrorCode == "WebhookInboxNotFound" && await Messages() == count,
            "missing booth notification cannot be reinterpreted as outbound overwrite");

        remote.Title = "Rice from booth"; remote.Price = 210;
        var incoming = new IntegrationScenarioJob { ConnectionId = connection.Id, ShopId = 7, TenantId = "tenant-a",
            Item = IntegrationSyncItem.Product, Trigger = IntegrationSyncTrigger.BoothChanged, EventId = "inbound-rice", CreatedAtUtc = DateTime.UtcNow };
        db.IntegrationWebhookInbox.Add(new() { ConnectionId = connection.Id, ExternalEventId = incoming.EventId,
            EventType = "product.updated", PayloadJson = "{\"product_id\":12,\"title\":\"untrusted\",\"stock\":999}", ReceivedAtUtc = DateTime.UtcNow });
        db.IntegrationScenarioJobs.Add(incoming); await db.SaveChangesAsync();
        Check((await Process(incoming.Id)).Status == IntegrationScenarioStatus.Completed && source.Commands == 1
            && source.Product.Name == "Rice from booth" && source.Product.Price == 210 && source.Product.Stock == 10,
            "inbound mapped product rereads provider and uses accounting port, never adopts marketplace stock");
        Check((await Process(await Enqueue(IntegrationSyncItem.Product))).Status == IntegrationScenarioStatus.Completed && await Messages() == count,
            "unchanged inbound projection does not trigger an outbound echo patch");

        remote.Price = null;
        product = await Enqueue(IntegrationSyncItem.Product);
        job = await Process(product);
        Check(job.Status == IntegrationScenarioStatus.Pending && job.ResultJson!.Contains("ExternalPriceUnknown"), "unknown remote price is not silently equal");
        await Deliver(); await Process(product);
        remote.Sku = "other";
        count = await Messages();
        job = await Process(await Enqueue(IntegrationSyncItem.Product));
        Check(job.Status == IntegrationScenarioStatus.NeedsAttention && job.ResultJson!.Contains("ProductSkuMismatch") && await Messages() == count,
            "unsupported SKU differences remain actionable, not reported synchronized");
        remote.Sku = "rice";

        await Switch(VersionSource.AccountingEvents);
        remote.Title = "event-owned difference";
        job = await Process(await Enqueue(IntegrationSyncItem.Product));
        Check(job.Status == IntegrationScenarioStatus.NeedsAttention && job.ResultJson!.Contains("VersionSourceConflict") && await Messages() == count,
            "capture cannot take over event-owned product stream");
        await Switch(VersionSource.InventoryCapture);
        product = await Enqueue(IntegrationSyncItem.Product); await Process(product);
        remote.PermanentFailure = true; await Deliver(); remote.PermanentFailure = false;
        count = await Messages();
        job = await Process(product);
        Check(job.Status == IntegrationScenarioStatus.NeedsAttention && job.ResultJson!.Contains("ReconciliationDeliveryFailed") && await Messages() == count,
            "terminal delivery failure is visible and never automatically resurrected by read-back");

        product = await Enqueue(IntegrationSyncItem.Product); await Process(product); await Deliver();
        count = await Messages(); remote.FailRead = true;
        job = await Process(product); remote.FailRead = false;
        Check(job.Status == IntegrationScenarioStatus.Pending && JsonSerializer.Deserialize<IntegrationScenarioResult>(job.ResultJson!)!.OutboxMessageIds?.Count == 1,
            "transient read-back error retains durable delivery checkpoint");
        Check((await Process(product)).Status == IntegrationScenarioStatus.Completed && await Messages() == count,
            "recovered read-back completes without resending the acknowledged patch");

        remote.Title = "unreflected write"; remote.IgnoreProductWrites = true;
        product = await Enqueue(IntegrationSyncItem.Product); await Process(product); await Deliver();
        count = await Messages();
        job = await Process(product); remote.IgnoreProductWrites = false;
        Check(job.Status == IntegrationScenarioStatus.NeedsAttention && job.ResultJson!.Contains("ProductTitleMismatch")
            && await Messages() == count, "provider ACK without a reflected change is not completion or automatic resend");

        remote.Title = "variant mismatch"; remote.Variant = "8";
        await db.ExternalProductMappings.Where(x => x.Id == mapping.Id).ExecuteUpdateAsync(s => s.SetProperty(x => x.ExternalVariantId, "8"));
        job = await Process(await Enqueue(IntegrationSyncItem.Product));
        Check(job.Status == IntegrationScenarioStatus.NeedsAttention && job.ResultJson!.Contains("ProductVariantUpdateUnsupported") && await Messages() == count,
            "variant details are not misapplied to the parent");
        await db.ExternalProductMappings.Where(x => x.Id == mapping.Id).ExecuteUpdateAsync(s => s.SetProperty(x => x.IsActive, false));
        job = await Process(await Enqueue(IntegrationSyncItem.Product));
        Check(job.Status == IntegrationScenarioStatus.NeedsAttention && job.ResultJson!.Contains("ExternalProductUnmapped") && await Messages() == count,
            "unmapped product requires explicit mapping, not guessed creation");
        await ProductCreationChecks.Run(options, Check);
        Console.WriteLine($"{checks} catalog reconciliation checks passed. SQL fixture and controlled ports only; no real provider/accounting writes.");
        return 0;
    }
    catch { failed = true; throw; }
    finally
    {
        if (!name.StartsWith("HyperCatalogChecks_", StringComparison.Ordinal) || !Guid.TryParseExact(name[19..], "N", out _))
            throw new InvalidOperationException("Unsafe fixture target");
        try
        {
            await Execute($"IF DB_ID(N'{name}') IS NOT NULL BEGIN ALTER DATABASE [{name}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [{name}]; END");
            Console.WriteLine("Removed only this run's disposable SQL fixture.");
        }
        catch (Exception e) when (failed)
        { Console.Error.WriteLine($"Fixture cleanup failed for {name}: {e.Message}"); }
    }
}

sealed class FixtureSchema(ModelCustomizerDependencies dependencies) : ModelCustomizer(dependencies)
{
    public override void Customize(ModelBuilder builder, DbContext context)
    {
        base.Customize(builder, context);
        foreach (var entity in builder.Model.GetEntityTypes()) entity.SetIsTableExcludedFromMigrations(false);
    }
}
sealed class ProviderFixture : IExternalIntegrationAdapter, IExternalProductPublisher
{
    public string Title = "Old rice"; public decimal? Price = 99; public decimal? Stock = 3; public string Sku = "rice";
    public string? Variant; public bool PermanentFailure, FailRead, IgnoreProductWrites; public int ProductWrites, StockWrites;
    public IntegrationProvider Provider => IntegrationProvider.Basalam;
    public bool IsImplemented => true;
    public bool SupportsCredentialType(IntegrationCredentialType type) => type == IntegrationCredentialType.BearerToken;
    public Task<IReadOnlyCollection<ExternalCatalogItem>> ReadCatalogAsync(ExternalIntegrationConnection c, CancellationToken ct)
    {
        if (FailRead) throw new HttpRequestException("fixture read unavailable");
        return Task.FromResult<IReadOnlyCollection<ExternalCatalogItem>>([new("12", Sku, Title, Price, Stock, Variant)]);
    }
    public Task PublishProductAsync(ExternalIntegrationConnection c, ExternalProductUpdate update, CancellationToken ct)
    {
        if (PermanentFailure) throw new IntegrationProviderException("FixtureRejected", false);
        if (IgnoreProductWrites) { ProductWrites++; return Task.CompletedTask; }
        Title = update.Title ?? Title; Price = update.PrimaryPrice ?? Price; ProductWrites++; return Task.CompletedTask;
    }
    public Task PublishInventoryAsync(ExternalIntegrationConnection c, IReadOnlyCollection<ExternalInventoryUpdate> updates, CancellationToken ct)
    { Stock = updates.Single().Quantity; StockWrites++; return Task.CompletedTask; }
}
sealed class AccountingFixture : IIntegrationPlatformCatalogPort, IIntegrationBusinessCommandPort
{
    public IntegrationPlatformProduct Product = new(44, "Rice", "rice", 125, 10, true, true, null, true);
    public int Commands;
    public Task<IReadOnlyList<IntegrationPlatformProduct>> GetProductsAsync(int shopId, string tenantId, CancellationToken ct)
    {
        if (shopId != 7 || tenantId != "tenant-a") throw new InvalidOperationException("Foreign accounting scope");
        return Task.FromResult<IReadOnlyList<IntegrationPlatformProduct>>([Product]);
    }
    public Task<BusinessCommandResult> ApplyExternalProductChangedAsync(IntegrationExternalProductChangedCommand c, CancellationToken ct)
    {
        if (c.ShopId != 7 || c.TenantId != "tenant-a" || c.HyperProductId != 44) throw new InvalidOperationException("Foreign command scope");
        Commands++; Product = Product with { Name = c.Title, Price = c.Price ?? Product.Price, Sku = c.Sku ?? Product.Sku };
        return Task.FromResult(new BusinessCommandResult(BusinessCommandStatus.Applied, "44"));
    }
    public Task<BusinessCommandResult> ApplyCounterpartyAsync(IntegrationCounterpartyCommand c, CancellationToken ct) => throw new NotSupportedException();
    public Task<BusinessCommandResult> ApplyVendorOrderAsync(IntegrationVendorOrderCommand c, CancellationToken ct) => throw new NotSupportedException();
    public Task<BusinessCommandResult> ApplyCustomerOrderAsync(IntegrationCustomerOrderCommand c, CancellationToken ct) => throw new NotSupportedException();
    public Task<BusinessCommandResult> CancelOrderAsync(IntegrationOrderCancellationCommand c, CancellationToken ct) => throw new NotSupportedException();
    public Task<BusinessCommandResult> ApplyParcelStatusAsync(IntegrationParcelStatusCommand c, CancellationToken ct) => throw new NotSupportedException();
}
