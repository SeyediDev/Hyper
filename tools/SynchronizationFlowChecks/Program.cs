using System.Net;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Basalam.SDK;
using Basalam.SDK.Config;
using Hyper.Infrastructure.Data.Repository.Hyper;
using Hyper.Infrastructure.Features.Integrations;
using Hyper.Integration.Api;
using Hyper.Integration.Contracts;
using Hyperyek.Accounting.Contracts;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Options;
using Provider = Hyper.Integration.Domain.Entities.Integrations.IntegrationProvider;
using ApiProvider = Hyper.Integration.Contracts.IntegrationProvider;

try { return await Run(); }
catch (Exception ex) { Console.Error.WriteLine($"Flow check failed: {ex}"); return 1; }

static async Task<int> Run()
{
    var source = Environment.GetEnvironmentVariable("SYNC_CHECKS_CONNECTION")
        ?? "Server=localhost;Integrated Security=true;TrustServerCertificate=true";
    var database = "HyperSyncChecks_" + Guid.NewGuid().ToString("N");
    var sqlOptions = new SqlConnectionStringBuilder(source) { InitialCatalog = database, Pooling = false };
    var masterOptions = new SqlConnectionStringBuilder(source) { InitialCatalog = "master", Pooling = false };
    await using var master = new SqlConnection(masterOptions.ConnectionString);
    await master.OpenAsync();
    await Execute(master, $"CREATE DATABASE [{database}]");
    try
    {
        var options = new DbContextOptionsBuilder<HyperIntegrationContext>().UseSqlServer(sqlOptions.ConnectionString)
            .ReplaceService<IModelCustomizer, FixtureSchema>().Options;
        await using var db = new HyperIntegrationContext(options);
        await db.Database.EnsureCreatedAsync();
        var checks = 0;
        void Check(bool ok, string name)
        {
            if (!ok) throw new InvalidOperationException(name);
            Console.WriteLine($"PASS {++checks}: {name}");
        }
        async Task Reject(Func<Task> action, string error)
        {
            try { await action(); }
            catch (InvalidOperationException ex) when (ex.Message == error) { Check(true, error); return; }
            throw new InvalidOperationException($"Expected {error}");
        }
        var connection = new ExternalIntegrationConnection
        {
            ShopId = 7, TenantId = "tenant-a", Provider = Provider.Basalam, DisplayName = "fixture-a",
            AccountIdentifier = "71", CredentialType = IntegrationCredentialType.OAuth2,
            CredentialsJson = "{\"webhookSecret\":\"fixture-secret-a\"}", IsEnabled = true
        };
        var other = new ExternalIntegrationConnection
        {
            ShopId = 7, TenantId = "tenant-b", Provider = Provider.Basalam, DisplayName = "fixture-b",
            AccountIdentifier = "71", CredentialType = IntegrationCredentialType.OAuth2,
            CredentialsJson = "{\"webhookSecret\":\"fixture-secret-b\"}", IsEnabled = true
        };
        db.ExternalIntegrationConnections.AddRange(connection, other);
        await db.SaveChangesAsync();
        db.ExternalProductMappings.Add(new() { ConnectionId = connection.Id, ShopId = 7, HyperProductId = 44, ExternalProductId = "12" });
        var providerHttp = new ProviderTransport();
        using var basalamHttp = new HttpClient(providerHttp);
        using var sdk = new BasalamClient(new BasalamConfig(), httpClient: basalamHttp);
        var oauth = new BasalamOAuthService(Options.Create(new BasalamOAuthSettings()), basalamHttp,
            new EphemeralDataProtectionProvider());
        db.ExternalOAuthTokens.Add(oauth.CreateTokenEntity(connection.Id, 7, "tenant-a", Provider.Basalam,
            new() { AccessToken = "fixture-grant", ExpiresIn = 3600, Scope = "vendor.product.read vendor.product.write" }));
        await db.SaveChangesAsync();
        var adapter = new BasalamSdkAdapter(sdk, new BasalamOAuthStore(db, oauth));
        var resolver = new IntegrationStrategyResolver([adapter]);
        var accountingHttp = new AccountingTransport();
        using var commandsHttp = new HttpClient(accountingHttp) { BaseAddress = new Uri("https://accounting.fixture.invalid/") };
        var commands = new HyperyekAccountingApiClient(commandsHttp);
        var dispatcher = new IntegrationBusinessEventDispatcher(commands, new UnregisteredIntegrationEngagementPort(),
            new NoReservations(), db, resolver);
        var processor = new IntegrationScenarioProcessor(db, resolver, new NoCapture(),
            Options.Create(new IntegrationInventoryCaptureOptions()), Options.Create(new BasalamOAuthSettings()), null!, dispatcher, commands);
        var queue = new IntegrationScenarioQueue(db, processor);
        var ingress = new IntegrationWebhookIngress(db, new IntegrationWebhookVerifier());
        WebhookIngressRequest Event(string id, string product = "12", string? sourceMarker = null) =>
            new(ApiProvider.Basalam, "71", id, "product.updated", null, null,
                JsonSerializer.SerializeToUtf8Bytes(new { externalProductId = product, hyperProductId = 999,
                    title = "Untrusted webhook title", sku = "untrusted-sku", price = 999, source = sourceMarker }),
                Authorization: "Bearer fixture-secret-a");
        var request = Event("product-1");
        var accepted = await ingress.ReceiveAsync(request);
        Check(accepted.Status == WebhookIngressStatus.Accepted && accepted.ScenarioJobId.HasValue,
            "verified Basalam event creates durable inbox and worker job");
        var job = await db.IntegrationScenarioJobs.AsNoTracking().SingleAsync(x => x.Id == accepted.ScenarioJobId);
        Check(job.ConnectionId == connection.Id && job.TenantId == "tenant-a", "per-connection secret selects correct tenant for shared vendor ID");
        var duplicate = await ingress.ReceiveAsync(request);
        Check(duplicate.Status == WebhookIngressStatus.Duplicate && duplicate.InboxId == accepted.InboxId
            && await db.IntegrationScenarioJobs.CountAsync() == 1, "identical replay has one durable effect");
        Check((await ingress.ReceiveAsync(request with { Body = Encoding.UTF8.GetBytes("{\"title\":\"different\"}") })).ErrorCode == "EventIdentityConflict",
            "event ID cannot be reused for different content");
        Check((await ingress.ReceiveAsync(Event("invalid-auth") with { Authorization = "Bearer wrong" })).ErrorCode == "SignatureInvalid",
            "wrong connection credential rejected");
        Check((await ingress.ReceiveAsync(Event("array") with { Body = Encoding.UTF8.GetBytes("[]") })).Status == WebhookIngressStatus.Invalid,
            "non-object webhook rejected before persistence");
        Check((await ingress.ReceiveAsync(Event(new string('x', 129)))).ErrorCode == "InvalidHeader", "event ID respects worker storage limit");
        Check(await queue.ProcessConnectionAsync(connection.Id, default), "worker processes the queued product event");
        var inbox = await db.IntegrationWebhookInbox.AsNoTracking().SingleAsync(x => x.Id == accepted.InboxId);
        job = await db.IntegrationScenarioJobs.AsNoTracking().SingleAsync(x => x.Id == accepted.ScenarioJobId);
        Check(inbox.Status == 1 && inbox.ProcessedAtUtc.HasValue && job.Status == IntegrationScenarioStatus.Completed,
            "successful accounting response acknowledges both job and inbox");
        Check(accountingHttp.LastCommand is { HyperProductId: 44, Title: "Fixture product", Price: 125, Sku: "catalog-sku" }
            && accountingHttp.LastCommand.Scope == new AccountingScope(7, "tenant-a"), "accounting HTTP contract uses trusted mapping and scope");
        Check(accountingHttp.LastCommand?.SourceVersion == accepted.ScenarioJobId,
            "Basalam catalog overrides untrusted webhook fields and missing provider version uses stable job sequence");
        Check(!await queue.ProcessConnectionAsync(connection.Id, default) && accountingHttp.Calls == 1,
            "completed product is not applied again");

        var missing = await ingress.ReceiveAsync(Event("missing", "99"));
        await queue.ProcessConnectionAsync(connection.Id, default);
        inbox = await db.IntegrationWebhookInbox.AsNoTracking().SingleAsync(x => x.Id == missing.InboxId);
        Check(inbox.Status == 2 && inbox.Error == "ProductMappingUnavailable" && inbox.ProcessedAtUtc.HasValue
            && accountingHttp.Calls == 1, "missing mapping is visible as failed inbox without accounting mutation");
        accountingHttp.Fail = true;
        var retry = await ingress.ReceiveAsync(Event("retry"));
        await queue.ProcessConnectionAsync(connection.Id, default);
        inbox = await db.IntegrationWebhookInbox.AsNoTracking().SingleAsync(x => x.Id == retry.InboxId);
        Check(inbox.Status == 0 && inbox.ProcessedAtUtc is null && inbox.Error == "ProviderTransport",
            "transient transport failure retains pending inbox for retry");
        accountingHttp.Fail = false;
        await db.IntegrationScenarioJobs.Where(x => x.Id == retry.ScenarioJobId)
            .ExecuteUpdateAsync(s => s.SetProperty(x => x.NextAttemptAtUtc, DateTime.UtcNow.AddSeconds(-1)));
        await queue.ProcessConnectionAsync(connection.Id, default);
        Check(await db.IntegrationWebhookInbox.AsNoTracking().AnyAsync(x => x.Id == retry.InboxId && x.Status == 1 && x.Error == null),
            "successful retry clears inbox error and completes processing");
        var loop = await ingress.ReceiveAsync(Event("echo", sourceMarker: "Hyperyek"));
        Check(loop.ScenarioJobId is null && await db.IntegrationWebhookInbox.AsNoTracking().AnyAsync(x => x.Id == loop.InboxId && x.Status == 1),
            "explicit Hyperyek echo is acknowledged without another command");
        var unknown = await ingress.ReceiveAsync(Event("unknown") with { EventType = "unknown.event" });
        Check(unknown.ScenarioJobId is null && await db.IntegrationWebhookInbox.AsNoTracking().AnyAsync(x => x.Id == unknown.InboxId && x.Error == "UnsupportedEventType"),
            "unsupported event is actionable instead of pending forever");

        var outbox = new IntegrationOutbox(db, resolver);
        var accountingIngress = new IntegrationAccountingEventIngress(db, outbox);
        var controller = new IntegrationAccountingEventController(accountingIngress, new IntegrationScopeAuthorization())
        { ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() } };
        var inventory = new AccountingInventoryChangedRequest(7, "tenant-a", connection.Id, "12", null, 1, 3);
        Check(await controller.InventoryChanged(inventory, default) is ForbidResult && !await db.IntegrationOutbox.AnyAsync(),
            "unauthenticated accounting event cannot queue a live stock change");
        controller.HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity([new Claim("integration_scope", "shop:7;tenant:tenant-b")], "fixture"));
        Check(await controller.InventoryChanged(inventory, default) is ForbidResult, "another tenant cannot queue a stock change");
        controller.HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity([new Claim("integration_scope", "shop:7;tenant:tenant-a")], "fixture"));
        var outbound = (await controller.InventoryChanged(inventory, default) as AcceptedResult)?.Value as AccountingInventoryChangedResponse;
        Check(outbound is not null && await db.IntegrationOutbox.CountAsync() == 1, "authorized accounting event creates durable outbox");
        Check((await accountingIngress.ReceiveInventoryChangedAsync(inventory))?.OutboxMessageId == outbound!.OutboxMessageId,
            "source version replay reuses outbox message");
        await Reject(() => accountingIngress.ReceiveInventoryChangedAsync(inventory with { AvailableQuantity = 4 }), "SourceVersionConflict");
        Check(await outbox.ProcessConnectionAsync(connection.Id, default), "worker delivers accounting inventory through production Basalam adapter");
        Check(providerHttp.Patches == 1 && providerHttp.Stock == 3 && providerHttp.Authenticated
            && await db.IntegrationOutbox.AsNoTracking().AnyAsync(x => x.Id == outbound.OutboxMessageId && x.Status == 2),
            "token vault to SDK to stock PATCH is acknowledged delivered");
        await accountingIngress.ReceiveInventoryChangedAsync(inventory with { SourceVersion = 3, AvailableQuantity = 0 });
        await Reject(() => accountingIngress.ReceiveInventoryChangedAsync(inventory with { SourceVersion = 2 }), "StaleSourceVersion");
        await outbox.ProcessConnectionAsync(connection.Id, default);
        Check(providerHttp.Patches == 2 && providerHttp.Stock == 0, "newer zero stock is delivered without rounding or omission");
        Check(!await outbox.ProcessConnectionAsync(connection.Id, default) && providerHttp.Patches == 2, "delivered outbox is not resent");
        providerHttp.FailStatus = HttpStatusCode.ServiceUnavailable;
        var temporarilyFailed = await accountingIngress.ReceiveInventoryChangedAsync(inventory with { SourceVersion = 4, AvailableQuantity = 5 });
        await outbox.ProcessConnectionAsync(connection.Id, default);
        Check(await db.IntegrationOutbox.AsNoTracking().AnyAsync(x => x.Id == temporarilyFailed!.OutboxMessageId
            && x.Status == 0 && x.LastError == "Http503" && x.Attempts == 1), "SDK HTTP 503 is scheduled for retry without provider response body");
        providerHttp.FailStatus = null;
        await db.IntegrationOutbox.Where(x => x.Id == temporarilyFailed!.OutboxMessageId)
            .ExecuteUpdateAsync(s => s.SetProperty(x => x.NextAttemptAtUtc, DateTime.UtcNow.AddSeconds(-1)));
        await outbox.ProcessConnectionAsync(connection.Id, default);
        Check(providerHttp.Patches == 3 && providerHttp.Stock == 5, "provider recovery delivers the pending absolute inventory");
        var requestsBeforeInvalid = providerHttp.Requests;
        var fractional = await accountingIngress.ReceiveInventoryChangedAsync(inventory with { SourceVersion = 5, AvailableQuantity = 0.5m });
        await outbox.ProcessConnectionAsync(connection.Id, default);
        Check(await db.IntegrationOutbox.AsNoTracking().AnyAsync(x => x.Id == fractional!.OutboxMessageId
            && x.Status == 3 && x.LastError == "InvalidInventoryQuantity") && providerHttp.Requests == requestsBeforeInvalid,
            "fractional stock fails explicitly before HTTP instead of truncating to zero");
        var verified = Options.Create(new IntegrationInventoryCaptureOptions { AccountingStockSourceVerified = true });
        var capture = new IntegrationInventoryCapture(db, outbox, resolver, verified, commands);
        var mapping = await db.ExternalProductMappings.AsNoTracking().SingleAsync(x => x.ConnectionId == connection.Id);
        db.InventoryReservationLogs.AddRange(
            new() { ShopId = 7, HyperProductId = 44, ReservationKey = "active", Quantity = 2, Status = 0, Source = "fixture" },
            new() { ShopId = 7, HyperProductId = 44, ReservationKey = "consumed", Quantity = 5, Status = 2, Source = "fixture" },
            new() { ShopId = 8, HyperProductId = 44, ReservationKey = "other-shop", Quantity = 9, Status = 0, Source = "fixture" });
        await db.SaveChangesAsync();
        Check(await capture.ReconcileOneAsync(connection.Id, mapping.Id, default), "capture reads accounting over HTTP and enqueues available stock");
        async Task<decimal> LatestQuantity()
        {
            var message = await db.IntegrationOutbox.AsNoTracking().Where(x => x.MappingId == mapping.Id)
                .OrderByDescending(x => x.SourceVersion).FirstAsync();
            return JsonSerializer.Deserialize<ExternalInventoryUpdate>(message.PayloadJson)!.Quantity;
        }
        Check(await LatestQuantity() == 8 && accountingHttp.LastCatalogQuery == "?tenantId=tenant-a",
            "capture subtracts only active same-shop holds from scoped accounting stock");
        Check(!await capture.CaptureOneAsync(connection.Id, mapping.Id, default), "unchanged capture does not duplicate outbox");
        var inventoryProcessor = new IntegrationScenarioProcessor(db, resolver, capture, verified,
            Options.Create(new BasalamOAuthSettings()), null!, dispatcher, commands);
        var compared = await inventoryProcessor.ProcessAsync(new() { ConnectionId = connection.Id, ShopId = 7,
            TenantId = "tenant-a", EventId = "catalog-inventory", Item = IntegrationSyncItem.Inventory }, default);
        Check(compared.Differences.Any(x => x.Code == "InventoryMismatch"),
            "inventory scenario compares remote inventory to accounting API plus local holds");
        accountingHttp.CanSell = false;
        Check(await capture.CaptureOneAsync(connection.Id, mapping.Id, default) && await LatestQuantity() == 0,
            "non-sellable accounting product publishes zero available inventory");
        accountingHttp.LegacyCatalog = true;
        Check(!(await commands.GetProductsAsync(7, "tenant-a", default)).Single().CanSell,
            "older accounting response without CanSell fails closed");
        accountingHttp.FailCatalog = true;
        var beforeFailure = await db.IntegrationOutbox.CountAsync();
        try { await capture.CaptureOneAsync(connection.Id, mapping.Id, default); throw new Exception("Expected catalog failure"); }
        catch (HttpRequestException) { }
        Check(await db.IntegrationOutbox.CountAsync() == beforeFailure, "accounting outage cannot enqueue invented stock");
        Check(!await db.Database.SqlQueryRaw<int>("SELECT COUNT(*) AS Value FROM sys.tables WHERE name LIKE 'TBL[_]%' ").AnyAsync(x => x != 0),
            "both paths run with only Integration tables, no accounting tables in Integration database");
        Console.WriteLine($"{checks} synchronization flow checks passed. HTTP/accounting responses are controlled fixtures, not live-provider acceptance.");
        return 0;
    }
    finally
    {
        if (!database.StartsWith("HyperSyncChecks_", StringComparison.Ordinal)
            || !Guid.TryParseExact(database["HyperSyncChecks_".Length..], "N", out _)
            || sqlOptions.InitialCatalog != database) throw new InvalidOperationException("Unsafe fixture cleanup target");
        await Execute(master, $"DROP DATABASE [{database}]");
        Console.WriteLine("Removed only this run's disposable SQL fixture database.");
    }
}

static async Task Execute(SqlConnection sql, string text)
{
    await using var command = sql.CreateCommand();
    command.CommandText = text;
    await command.ExecuteNonQueryAsync();
}

sealed class FixtureSchema(ModelCustomizerDependencies dependencies) : ModelCustomizer(dependencies)
{
    public override void Customize(ModelBuilder builder, DbContext context)
    {
        base.Customize(builder, context);
        foreach (var entity in builder.Model.GetEntityTypes()) entity.SetIsTableExcludedFromMigrations(false);
    }
}

sealed class AccountingTransport : HttpMessageHandler
{
    public int Calls; public bool Fail; public ExternalProductChangedCommand? LastCommand;
    public bool CanSell = true; public bool LegacyCatalog; public bool FailCatalog; public string? LastCatalogQuery;
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
    {
        if (request.RequestUri?.AbsolutePath == "/api/hyperyek/v1/accounting/platform/shops/7/products")
        {
            if (FailCatalog) throw new HttpRequestException("Controlled catalog outage");
            LastCatalogQuery = request.RequestUri.Query;
            return new HttpResponseMessage(HttpStatusCode.OK) { Content = LegacyCatalog
                ? new StringContent("[{\"productId\":44,\"name\":\"Fixture product\",\"price\":125,\"stock\":10,\"isEnabled\":true,\"isStockable\":true}]", Encoding.UTF8, "application/json")
                : JsonContent.Create(new[] { new AccountingProductRead(44, "Fixture product", "catalog-sku", 125, 10, true, true, null, CanSell) }) };
        }
        if (request.RequestUri?.AbsolutePath != "/api/hyperyek/v1/accounting/products/external-changed")
            throw new InvalidOperationException("Unexpected accounting route");
        Calls++;
        if (Fail) throw new HttpRequestException("Controlled fixture transport failure");
        LastCommand = await request.Content!.ReadFromJsonAsync<ExternalProductChangedCommand>(cancellationToken: ct);
        return new HttpResponseMessage(HttpStatusCode.OK) { Content = JsonContent.Create(new AccountingCommandResult(AccountingCommandStatus.Applied, "44")) };
    }
}

sealed class ProviderTransport : HttpMessageHandler
{
    public int Patches; public int Stock; public bool Authenticated;
    public int Requests; public HttpStatusCode? FailStatus;
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
    {
        Requests++;
        if (request.RequestUri?.Host != "openapi.basalam.com")
            throw new InvalidOperationException("Unexpected Basalam fixture route");
        Authenticated = request.Headers.Authorization?.ToString() == "Bearer fixture-grant";
        if (!Authenticated) throw new InvalidOperationException("Missing connection-specific authentication");
        if (FailStatus is { } failure) return new HttpResponseMessage(failure) { Content = new StringContent("provider-sensitive-fixture") };
        if (request.Method == HttpMethod.Get && request.RequestUri.AbsolutePath == "/v1/vendors/71/products")
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"data\":[{\"id\":12,\"vendor\":{\"id\":71},\"title\":\"Fixture product\",\"sku\":\"catalog-sku\",\"price\":125,\"inventory\":3}],\"page\":1,\"total_page\":1}", Encoding.UTF8, "application/json")
            };
        if (request.RequestUri.AbsolutePath != "/v1/products/12") throw new InvalidOperationException("Unexpected Basalam fixture route");
        if (request.Method == HttpMethod.Patch)
        {
            using var body = JsonDocument.Parse(await request.Content!.ReadAsStringAsync(ct));
            Stock = body.RootElement.GetProperty("stock").GetInt32(); Patches++;
        }
        return new HttpResponseMessage(HttpStatusCode.OK)
        { Content = new StringContent("{\"id\":12,\"vendor\":{\"id\":71},\"title\":\"Fixture product\",\"inventory\":3}", Encoding.UTF8, "application/json") };
    }
}

sealed class NoCapture : IIntegrationInventoryCapture
{
    public Task<int> CaptureAsync(CancellationToken ct) => throw new InvalidOperationException("Unexpected legacy capture");
    public Task<bool> ReconcileOneAsync(long connectionId, long mappingId, CancellationToken ct) => throw new InvalidOperationException("Unexpected legacy reconciliation");
}
sealed class NoReservations : IIntegrationInventoryReservation
{
    public Task ReserveAsync(OwnedIntegrationShop shop, string key, IReadOnlyCollection<IntegrationOrderLineCommand> lines, CancellationToken ct) => throw new InvalidOperationException("Unexpected order reservation");
    public Task ReleaseAsync(OwnedIntegrationShop shop, string key, CancellationToken ct) => throw new InvalidOperationException("Unexpected release");
    public Task CommitAsync(OwnedIntegrationShop shop, string key, CancellationToken ct) => throw new InvalidOperationException("Unexpected stock commit");
}
