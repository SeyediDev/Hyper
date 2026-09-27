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
    try
    {
        Console.WriteLine("Creating isolated SQL fixture " + database);
        await Execute(master, $"CREATE DATABASE [{database}]");
        var options = new DbContextOptionsBuilder<HyperIntegrationContext>().UseSqlServer(sqlOptions.ConnectionString, sql => sql.CommandTimeout(120))
            .ReplaceService<IModelCustomizer, FixtureSchema>().Options;
        await using var db = new HyperIntegrationContext(options);
        await db.Database.EnsureCreatedAsync();
        var sourceSchema = await File.ReadAllTextAsync(Path.Combine(AppContext.BaseDirectory, "ensure-integration-version-ownership.sql"));
        await db.Database.ExecuteSqlRawAsync(sourceSchema);
        await db.Database.ExecuteSqlRawAsync(sourceSchema);
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
        await AccountingHttpChecks.Run(Check);
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
        await BasalamCatalogChecks.Run(connection, new BasalamOAuthStore(db, oauth), Check);
        var registration = new BasalamWebhookRegistration(db, new BasalamOAuthStore(db, oauth), sdk);
        await registration.RegisterForConnectionAsync(connection.Id, 7, "tenant-a", "71", "https://callback.fixture.invalid/oauth/callback", default);
        Check(providerHttp.WebhookRegistration is { } registered
            && registered.GetProperty("event_ids").EnumerateArray().Select(x => x.GetInt32()).SequenceEqual(Enumerable.Range(1, 9))
            && registered.GetProperty("url").GetString() == "https://callback.fixture.invalid/api/integrations/v1/webhooks/basalam/71"
            && registered.GetProperty("request_headers").GetString() == "Authorization: Bearer fixture-secret-a"
            && registered.GetProperty("register_me").GetBoolean(),
            "registration sends shared event catalog and connection callback/secret using the scoped grant (intercepted HTTP)");
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
        await WebhookContractChecks.Run(ingress, Check);
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

        providerHttp.CatalogOverride = """{"data":[],"hasMore":true}""";
        var invalidCatalog = await ingress.ReceiveAsync(Event("invalid-catalog"));
        await queue.ProcessConnectionAsync(connection.Id, default);
        Check(await db.IntegrationWebhookInbox.AsNoTracking().AnyAsync(x => x.Id == invalidCatalog.InboxId
                && x.Status == 2 && x.Error == "CatalogInvalidResponse" && x.ProcessedAtUtc != null)
            && accountingHttp.Calls == 1,
            "invalid catalog ends inbox with safe error and no accounting mutation");
        providerHttp.CatalogOverride = null;

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
        foreach (var code in new[] { HttpStatusCode.ServiceUnavailable, HttpStatusCode.TooManyRequests })
        {
            accountingHttp.FailStatus = code;
            accountingHttp.RetryAfter = TimeSpan.FromMinutes(9);
            var delayed = await ingress.ReceiveAsync(Event("accounting-http-" + (int)code));
            var beforeAttempt = DateTime.UtcNow;
            await queue.ProcessConnectionAsync(connection.Id, default);
            var pending = await db.IntegrationScenarioJobs.AsNoTracking().SingleAsync(x => x.Id == delayed.ScenarioJobId);
            var originalCommand = accountingHttp.LastCommand;
            Check(pending.Status == IntegrationScenarioStatus.Pending && pending.Attempts == 1
                && pending.ErrorCode == "AccountingApi_" + (int)code && pending.NextAttemptAtUtc >= beforeAttempt.AddMinutes(9)
                && pending.CompletedAtUtc is null && pending.LeaseId is null
                && await db.IntegrationWebhookInbox.AsNoTracking().AnyAsync(x => x.Id == delayed.InboxId && x.Status == 0
                    && x.ProcessedAtUtc == null && x.Error == pending.ErrorCode),
                "accounting HTTP " + (int)code + " preserves pending SQL inbox/job and Retry-After without response body");
            var callsBeforeDelay = accountingHttp.Calls;
            Check(!await queue.ProcessConnectionAsync(connection.Id, default) && accountingHttp.Calls == callsBeforeDelay,
                "durable accounting retry does not send before its scheduled delay");
            accountingHttp.FailStatus = null;
            await db.IntegrationScenarioJobs.Where(x => x.Id == delayed.ScenarioJobId)
                .ExecuteUpdateAsync(s => s.SetProperty(x => x.NextAttemptAtUtc, DateTime.UtcNow.AddSeconds(-1)));
            await using (var restarted = new HyperIntegrationContext(options))
            {
                var restartedDispatcher = new IntegrationBusinessEventDispatcher(commands, new UnregisteredIntegrationEngagementPort(),
                    new NoReservations(), restarted, resolver);
                var restartedProcessor = new IntegrationScenarioProcessor(restarted, resolver, new NoCapture(),
                    Options.Create(new IntegrationInventoryCaptureOptions()), Options.Create(new BasalamOAuthSettings()), null!, restartedDispatcher, commands);
                await new IntegrationScenarioQueue(restarted, restartedProcessor).ProcessConnectionAsync(connection.Id, default);
            }
            Check(accountingHttp.LastCommand == originalCommand
                && await db.IntegrationScenarioJobs.AsNoTracking().AnyAsync(x => x.Id == delayed.ScenarioJobId
                    && x.Status == IntegrationScenarioStatus.Completed && x.Attempts == 2 && x.ErrorCode == null)
                && await db.IntegrationWebhookInbox.AsNoTracking().AnyAsync(x => x.Id == delayed.InboxId && x.Status == 1 && x.Error == null),
                "fresh worker context recovers accounting retry with the same command identity/version and acknowledges original inbox");
        }
        accountingHttp.FailStatus = HttpStatusCode.ServiceUnavailable;
        var exhausted = await ingress.ReceiveAsync(Event("accounting-http-exhausted"));
        await db.IntegrationScenarioJobs.Where(x => x.Id == exhausted.ScenarioJobId)
            .ExecuteUpdateAsync(s => s.SetProperty(x => x.Attempts, IntegrationRetryPolicy.MaxAttempts - 1));
        await queue.ProcessConnectionAsync(connection.Id, default);
        Check(await db.IntegrationScenarioJobs.AsNoTracking().AnyAsync(x => x.Id == exhausted.ScenarioJobId
                && x.Status == IntegrationScenarioStatus.DeadLetter && x.Attempts == IntegrationRetryPolicy.MaxAttempts)
            && await db.IntegrationWebhookInbox.AsNoTracking().AnyAsync(x => x.Id == exhausted.InboxId && x.Status == 2),
            "accounting transient failure respects the existing finite durable retry limit");
        accountingHttp.FailStatus = null;
        accountingHttp.RetryAfter = null;
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
        var versionSources = new IntegrationVersionSourceApi(db);
        var versionScope = new IntegrationConnectionCommandRequest(7, "tenant-a", connection.Id);
        async Task SwitchSource(IntegrationVersionSource target)
        {
            var current = (await versionSources.ReadAsync(versionScope, mapping.Id, default))!;
            var switched = await versionSources.ChangeAsync(versionScope, mapping.Id,
                new(target, current.Source, current.LastVersion), default);
            Check(switched?.Status == "Changed", "explicit drained-stream switch to " + target + " preserves source high-water mark");
        }
        await Reject(() => capture.CaptureOneAsync(connection.Id, mapping.Id, default), "VersionSourceConflict");
        Check(await capture.CaptureAsync(default) == 0, "automatic capture excludes externally owned mappings before accounting reads");
        await SwitchSource(IntegrationVersionSource.InventoryCapture);
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
        accountingHttp.FailCatalog = false;
        // Drain earlier inventory checks before testing the independent product operation.
        while (await outbox.ProcessConnectionAsync(connection.Id, default)) { }
        await SwitchSource(IntegrationVersionSource.AccountingEvents);
        var productIngress = new IntegrationAccountingProductEventIngress(db, outbox);
        var productController = new IntegrationAccountingProductEventController(productIngress, new IntegrationScopeAuthorization())
        { ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() } };
        var version = await db.IntegrationOutbox.MaxAsync(x => x.SourceVersion) + 1;
        var changedProduct = new AccountingProductChangedRequest(7, "tenant-a", connection.Id, "12", null, version, "Changed title", 250);
        var messagesBefore = await db.IntegrationOutbox.CountAsync();
        Check(await productController.ProductChanged(changedProduct, default) is ForbidResult,
            "anonymous caller cannot queue a product mutation");
        productController.HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity([new Claim("scope", "shop:7;tenant:tenant-b")], "fixture"));
        Check(await productController.ProductChanged(changedProduct, default) is ForbidResult,
            "other tenant cannot queue a product mutation");
        productController.HttpContext.User = controller.HttpContext.User;
        Check((await productIngress.ReceiveAsync(changedProduct with { ExternalProductId = "999" })).ErrorCode == "MappingOrScopeNotFound",
            "product mutation requires a trusted active mapping");
        Check((await productIngress.ReceiveAsync(changedProduct with { Title = null, PrimaryPrice = null })).ErrorCode == "EmptyProductChange",
            "empty product patches are rejected");
        Check((await productIngress.ReceiveAsync(changedProduct with { Title = " " })).ErrorCode == "InvalidProductTitle",
            "blank title cannot clear the product name");
        Check((await productIngress.ReceiveAsync(changedProduct with { PrimaryPrice = -1 })).ErrorCode == "InvalidPrimaryPrice"
            && (await productIngress.ReceiveAsync(changedProduct with { PrimaryPrice = 1.5m })).ErrorCode == "InvalidPrimaryPrice",
            "negative or fractional provider prices are rejected without conversion");
        Check((await productIngress.ReceiveAsync(changedProduct with { ExternalVariantId = "8" })).ErrorCode == "ProductVariantUpdateUnsupported"
            && await db.IntegrationOutbox.CountAsync() == messagesBefore,
            "unsupported variant patches never affect the parent product or queue");
        var productAccepted = (await productController.ProductChanged(changedProduct, default) as AcceptedResult)?.Value as AccountingProductChangedResponse;
        Check(productAccepted?.OutboxMessageId is not null && await db.IntegrationOutbox.AsNoTracking().AnyAsync(x =>
                x.Id == productAccepted.OutboxMessageId && x.Operation == IntegrationOutbox.ProductOperation),
            "authorized product change is persisted as typed Outbox operation");
        Check((await productIngress.ReceiveAsync(changedProduct)).OutboxMessageId == productAccepted!.OutboxMessageId,
            "identical product replay returns the same message");
        Check(await productController.ProductChanged(changedProduct with { Title = "Conflict" }, default) is ConflictObjectResult,
            "changed content for an existing source version returns HTTP conflict");
        Check((await productIngress.ReceiveAsync(changedProduct with { SourceVersion = 1 })).ErrorCode == "StaleSourceVersion",
            "product cannot reuse a version from a previous source ownership period");
        var stockBeforeProduct = providerHttp.Stock;
        Check(await outbox.ProcessConnectionAsync(connection.Id, default) && providerHttp.ProductPatches == 1
            && providerHttp.ProductPatch!.GetProperty("name").GetString() == "Changed title"
            && providerHttp.ProductPatch.GetProperty("primary_price").GetInt64() == 250
            && !providerHttp.ProductPatch.TryGetProperty("stock", out _) && providerHttp.Stock == stockBeforeProduct,
            "product operation sends official name/primary_price fields without touching stock");
        Check(await db.IntegrationOutbox.AsNoTracking().AnyAsync(x => x.Id == productAccepted.OutboxMessageId && x.Status == 2)
            && !await outbox.ProcessConnectionAsync(connection.Id, default),
            "product success is persisted and not redelivered");
        await Reject(() => capture.CaptureOneAsync(connection.Id, mapping.Id, default), "VersionSourceConflict");
        var priceOnly = changedProduct with { SourceVersion = ++version, Title = null, PrimaryPrice = 0 };
        await productIngress.ReceiveAsync(priceOnly);
        await outbox.ProcessConnectionAsync(connection.Id, default);
        Check(providerHttp.ProductPatch!.GetProperty("primary_price").GetInt64() == 0
            && !providerHttp.ProductPatch.TryGetProperty("name", out _), "zero base price is explicit and absent title is omitted");
        var titleOnly = changedProduct with { SourceVersion = ++version, Title = "Only name", PrimaryPrice = null };
        var productRetry = await productIngress.ReceiveAsync(titleOnly);
        providerHttp.FailStatus = HttpStatusCode.ServiceUnavailable;
        await outbox.ProcessConnectionAsync(connection.Id, default);
        Check(await db.IntegrationOutbox.AsNoTracking().AnyAsync(x => x.Id == productRetry.OutboxMessageId && x.Status == 0 && x.LastError == "Http503"),
            "product transport failure uses durable retry without sensitive response contents");
        providerHttp.FailStatus = null;
        await db.IntegrationOutbox.Where(x => x.Id == productRetry.OutboxMessageId)
            .ExecuteUpdateAsync(s => s.SetProperty(x => x.NextAttemptAtUtc, DateTime.UtcNow.AddSeconds(-1)));
        await outbox.ProcessConnectionAsync(connection.Id, default);
        Check(providerHttp.ProductPatch!.GetProperty("name").GetString() == "Only name"
            && !providerHttp.ProductPatch.TryGetProperty("primary_price", out _), "retried title-only patch omits price");
        var productPatchCount = providerHttp.ProductPatches;
        providerHttp.VendorId = 999;
        var wrongVendor = await productIngress.ReceiveAsync(changedProduct with { SourceVersion = ++version });
        await outbox.ProcessConnectionAsync(connection.Id, default);
        Check(providerHttp.ProductPatches == productPatchCount && await db.IntegrationOutbox.AsNoTracking().AnyAsync(x =>
                x.Id == wrongVendor.OutboxMessageId && x.Status == 3 && x.LastError == "VendorMismatch"),
            "remote vendor identity is verified before any product write");
        providerHttp.VendorId = 71;
        var changedMapping = await productIngress.ReceiveAsync(changedProduct with { SourceVersion = ++version });
        await db.ExternalProductMappings.Where(x => x.Id == mapping.Id).ExecuteUpdateAsync(s => s.SetProperty(x => x.IsActive, false));
        await outbox.ProcessConnectionAsync(connection.Id, default);
        Check(providerHttp.ProductPatches == productPatchCount && await db.IntegrationOutbox.AsNoTracking().AnyAsync(x =>
                x.Id == changedMapping.OutboxMessageId && x.Status == 3 && x.LastError == "MappingChanged"),
            "mapping deactivation before delivery prevents product mutation");
        await db.ExternalProductMappings.Where(x => x.Id == mapping.Id).ExecuteUpdateAsync(s => s.SetProperty(x => x.IsActive, true));
        await SwitchSource(IntegrationVersionSource.InventoryCapture);
        Check(await capture.CaptureOneAsync(connection.Id, mapping.Id, default, true)
            && await db.IntegrationOutbox.MaxAsync(x => x.SourceVersion) == version + 1,
            "inventory capture allocates a version after product messages in the shared stream");
        // Repair a missing mapping, then prove replay wakes the original worker job.
        var management = new IntegrationManagementApi(db);
        var managementController = new IntegrationManagementController(management, new IntegrationScopeAuthorization())
        { ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() } };
        var scope = new IntegrationConnectionCommandRequest(7, "tenant-a", connection.Id);
        var accountingCalls = accountingHttp.Calls;
        await db.ExternalProductMappings.Where(x => x.Id == mapping.Id).ExecuteUpdateAsync(s => s.SetProperty(x => x.IsActive, false));
        var repair = await ingress.ReceiveAsync(Event("repair-mapping"));
        await queue.ProcessConnectionAsync(connection.Id, default);
        Check(await db.IntegrationScenarioJobs.AsNoTracking().AnyAsync(x => x.Id == repair.ScenarioJobId
            && x.Status == IntegrationScenarioStatus.NeedsAttention && x.ErrorCode == "ProductMappingUnavailable")
            && accountingHttp.Calls == accountingCalls, "missing mapping needs attention without a guessed accounting mutation");
        Check(await managementController.Replay(connection.Id, repair.InboxId!.Value, 7, "tenant-a", default) is ForbidResult,
            "anonymous replay is forbidden");
        Check(await management.ReplayWebhookAsync(scope with { TenantId = "tenant-b" }, repair.InboxId.Value) is null,
            "another tenant cannot replay the original event");
        await db.ExternalProductMappings.Where(x => x.Id == mapping.Id).ExecuteUpdateAsync(s => s.SetProperty(x => x.IsActive, true));
        managementController.HttpContext.User = controller.HttpContext.User;
        var repaired = await managementController.Replay(connection.Id, repair.InboxId.Value, 7, "tenant-a", default) as AcceptedResult;
        Check(repaired?.Value is IntegrationReplayResponse { Status: "Queued" }
            && await db.IntegrationScenarioJobs.AsNoTracking().AnyAsync(x => x.Id == repair.ScenarioJobId
                && x.Status == IntegrationScenarioStatus.Pending && x.Attempts == 0 && x.CompletedAtUtc == null && x.ErrorCode == null)
            && await db.IntegrationWebhookInbox.AsNoTracking().AnyAsync(x => x.Id == repair.InboxId && x.Status == 0 && x.Error == null),
            "manual replay atomically resets the original job and Inbox after mapping repair");
        Check((await management.ReplayWebhookAsync(scope, repair.InboxId.Value))?.Status == "AlreadyQueued",
            "repeated replay does not duplicate pending work");
        await queue.ProcessConnectionAsync(connection.Id, default);
        Check(accountingHttp.Calls == accountingCalls + 1 && accountingHttp.LastCommand!.SourceVersion == repair.ScenarioJobId
            && await db.IntegrationWebhookInbox.AsNoTracking().AnyAsync(x => x.Id == repair.InboxId && x.Status == 1),
            "repaired mapping completes the original event with stable identity and sequence");
        Check(await managementController.Replay(connection.Id, repair.InboxId.Value, 7, "tenant-a", default) is OkObjectResult
            && !await queue.ProcessConnectionAsync(connection.Id, default), "completed webhook replay does not reapply accounting effects");
        Check(await managementController.Replay(connection.Id, unknown.InboxId!.Value, 7, "tenant-a", default) is ConflictObjectResult
            && await db.IntegrationWebhookInbox.AsNoTracking().AnyAsync(x => x.Id == unknown.InboxId && x.Status == 2),
            "unsupported event remains actionable instead of falsely requeued");
        var running = await ingress.ReceiveAsync(Event("running-replay"));
        var activeLease = Guid.NewGuid();
        await db.IntegrationScenarioJobs.Where(x => x.Id == running.ScenarioJobId).ExecuteUpdateAsync(s =>
            s.SetProperty(x => x.Status, IntegrationScenarioStatus.Running).SetProperty(x => x.LeaseId, activeLease)
             .SetProperty(x => x.LeaseExpiresAtUtc, DateTime.UtcNow.AddMinutes(5)));
        Check(await managementController.Replay(connection.Id, running.InboxId!.Value, 7, "tenant-a", default) is ConflictObjectResult
            && await db.IntegrationScenarioJobs.AsNoTracking().AnyAsync(x => x.Id == running.ScenarioJobId && x.LeaseId == activeLease),
            "replay cannot steal or reset a running worker lease");
        // End the lease fixture through the normal expired-lease recovery path;
        // otherwise it intentionally blocks subsequent jobs on this connection.
        await db.IntegrationScenarioJobs.Where(x => x.Id == running.ScenarioJobId).ExecuteUpdateAsync(s =>
            s.SetProperty(x => x.LeaseExpiresAtUtc, DateTime.UtcNow.AddSeconds(-1)));
        Check(await queue.ProcessConnectionAsync(connection.Id, default)
            && await db.IntegrationScenarioJobs.AsNoTracking().AnyAsync(x => x.Id == running.ScenarioJobId
                && x.Status == IntegrationScenarioStatus.Completed), "expired lease is recovered before later connection events");
        var owner = new RecordingEngagementOwner();
        var ownerDispatcher = new IntegrationBusinessEventDispatcher(commands, owner, new NoReservations(), db, resolver);
        var ownerProcessor = new IntegrationScenarioProcessor(db, resolver, new NoCapture(),
            Options.Create(new IntegrationInventoryCaptureOptions()), Options.Create(new BasalamOAuthSettings()), null!, ownerDispatcher, commands);
        var ownerQueue = new IntegrationScenarioQueue(db, ownerProcessor);
        var engagementCases = new[]
        {
            ("subscription.renewed", "{\"externalSubscriptionId\":\"subscription-1\",\"status\":\"active\"}"),
            ("review.created", "{\"externalReviewId\":\"review-1\",\"rating\":4,\"text\":\"fixture\"}"),
            ("chat.message.received", "{\"externalConversationId\":\"chat-1\",\"externalMessageId\":\"message-in\",\"text\":\"fixture\"}"),
            ("chat.message.sent", "{\"externalConversationId\":\"chat-1\",\"externalMessageId\":\"message-out\",\"text\":\"fixture\"}")
        };
        foreach (var (eventType, payload) in engagementCases)
        {
            var beforeProvider = providerHttp.Requests;
            var beforeAccounting = accountingHttp.Calls;
            var engagementEvent = await ingress.ReceiveAsync(Event("engagement-" + eventType) with
                { EventType = eventType, Body = Encoding.UTF8.GetBytes(payload) });
            await queue.ProcessConnectionAsync(connection.Id, default);
            var pendingOwner = await db.IntegrationScenarioJobs.AsNoTracking().SingleAsync(x => x.Id == engagementEvent.ScenarioJobId);
            Check(pendingOwner.Status == IntegrationScenarioStatus.NeedsAttention
                && pendingOwner.ResultJson!.Contains("EngagementOwnerApiNotRegistered")
                && providerHttp.Requests == beforeProvider && accountingHttp.Calls == beforeAccounting,
                eventType + " reaches engagement boundary, not catalog/accounting or false completion");
            await management.ReplayWebhookAsync(scope, engagementEvent.InboxId!.Value);
            await ownerQueue.ProcessConnectionAsync(connection.Id, default);
            Check(await db.IntegrationScenarioJobs.AsNoTracking().AnyAsync(x => x.Id == engagementEvent.ScenarioJobId
                    && x.Status == IntegrationScenarioStatus.Completed)
                && await db.IntegrationWebhookInbox.AsNoTracking().AnyAsync(x => x.Id == engagementEvent.InboxId && x.Status == 1),
                eventType + " completes only after an acknowledged owner result (fixture)");
        }
        Check(owner.Subscription is { ShopId: 7, TenantId: "tenant-a", ExternalSubscriptionId: "subscription-1" }
            && owner.Review is { ExternalReviewId: "review-1", Rating: 4 }
            && owner.Chats.Count == 2 && !owner.Chats[0].Outbound && owner.Chats[1].Outbound,
            "normalized engagement contracts retain scoped identities and chat direction");
        // Official sample_data shape, with positive synthetic entity IDs. Headers
        // below are our explicit delivery contract, not captured Basalam metadata.
        foreach (var eventType in new[] { "CHAT_RECEIVED_MESSAGE", "CHAT_SEND_MESSAGE" })
        {
            var payload = "{\"id\":123,\"chat_id\":456,\"message\":{\"text\":\"official-shape fixture\",\"files\":[],\"links\":{}},\"sender_id\":789}";
            var controllerEvent = WebhookContractChecks.Controller(ingress, payload, "official-" + eventType, eventType);
            var delivered = (await controllerEvent.Receive("basalam", "71", default) as AcceptedResult)?.Value as WebhookIngressResult;
            Check(delivered?.ScenarioJobId is not null, eventType + " accepted by real controller/ingress into SQL queue");
            await ownerQueue.ProcessConnectionAsync(connection.Id, default);
            Check(owner.Chats.Last() is { ExternalConversationId: "456", ExternalMessageId: "123", Text: "official-shape fixture" } chat
                && chat.Outbound == (eventType == "CHAT_SEND_MESSAGE")
                && await db.IntegrationScenarioJobs.AsNoTracking().AnyAsync(x => x.Id == delivered!.ScenarioJobId
                    && x.Status == IntegrationScenarioStatus.Completed), eventType + " numeric identities, nested text and direction reach owner fixture");
            var again = WebhookContractChecks.Controller(ingress, payload, "official-" + eventType, eventType);
            Check((await again.Receive("basalam", "71", default) as OkObjectResult)?.Value is WebhookIngressResult { Status: WebhookIngressStatus.Duplicate },
                eventType + " identical controller delivery is deduplicated");
        }
        var chatCount = owner.Chats.Count;
        var invalidChat = await ingress.ReceiveAsync(Event("invalid-chat-id") with { EventType = "CHAT_SEND_MESSAGE",
            Body = Encoding.UTF8.GetBytes("{\"id\":0,\"chat_id\":456,\"message\":{\"text\":\"fixture\"}}") });
        await ownerQueue.ProcessConnectionAsync(connection.Id, default);
        Check(owner.Chats.Count == chatCount && await db.IntegrationScenarioJobs.AsNoTracking().AnyAsync(x =>
            x.Id == invalidChat.ScenarioJobId && x.Status == IntegrationScenarioStatus.DeadLetter),
            "invalid official message identity never reaches downstream owner");
        var richChat = await ingress.ReceiveAsync(Event("rich-chat") with { EventType = "CHAT_RECEIVED_MESSAGE",
            Body = Encoding.UTF8.GetBytes("{\"id\":124,\"chat_id\":456,\"message\":{\"text\":\"fixture\",\"files\":[{\"id\":1}]}}") });
        await ownerQueue.ProcessConnectionAsync(connection.Id, default);
        Check(owner.Chats.Count == chatCount && await db.IntegrationScenarioJobs.AsNoTracking().AnyAsync(x =>
            x.Id == richChat.ScenarioJobId && x.Status == IntegrationScenarioStatus.NeedsAttention
            && x.ResultJson!.Contains("ChatRichContentOwnerContractRequired")),
            "rich chat content remains actionable instead of silently dropping attachments");
        var callsBeforeSuccessiveUpdates = accountingHttp.Calls;
        foreach (var eventId in new[] { "product-delivery-a", "product-delivery-b" })
        {
            var notification = WebhookContractChecks.Controller(ingress, "{\"data\":{\"id\":12,\"name\":\"untrusted\"}}",
                eventId, "PRODUCT_CREATE_CHANGES");
            var delivered = (await notification.Receive("basalam", "71", default) as AcceptedResult)?.Value as WebhookIngressResult;
            await queue.ProcessConnectionAsync(connection.Id, default);
            Check(delivered?.ScenarioJobId is not null
                && accountingHttp.LastCommand is { HyperProductId: 44, Title: "Fixture product", Price: 125 }
                && accountingHttp.LastCommand.SourceVersion == delivered.ScenarioJobId
                && await db.IntegrationScenarioJobs.AsNoTracking().AnyAsync(x => x.Id == delivered.ScenarioJobId
                    && x.Status == IntegrationScenarioStatus.Completed),
                eventId + " thin normalized product notification re-reads authoritative catalog before accounting HTTP");
        }
        Check(accountingHttp.Calls == callsBeforeSuccessiveUpdates + 2,
            "two explicit delivery IDs for one product are not collapsed by its entity ID");
        foreach (var definition in BasalamWebhookEvents.All)
        {
            var routed = await ingress.ReceiveAsync(Event("catalog-" + definition.Id) with { EventType = definition.Name });
            Check(await db.IntegrationScenarioJobs.AsNoTracking().AnyAsync(x => x.Id == routed.ScenarioJobId
                && x.Item == definition.Item), definition.Name + " routes to " + definition.Item + " (routing only)");
        }
        Check(!await db.Database.SqlQueryRaw<int>("SELECT COUNT(*) AS Value FROM sys.tables WHERE name LIKE 'TBL[_]%' ").AnyAsync(x => x != 0),
            "both paths run with only Integration tables, no accounting tables in Integration database");
        await VersionSourceChecks.Run(db, options, resolver, connection.Id, Check);
        await BasalamRetryChecks.Run(db, options, Check);
        Console.WriteLine($"{checks} synchronization flow checks passed. HTTP/accounting responses are controlled fixtures, not live-provider acceptance.");
        return 0;
    }
    finally
    {
        if (!database.StartsWith("HyperSyncChecks_", StringComparison.Ordinal)
            || !Guid.TryParseExact(database["HyperSyncChecks_".Length..], "N", out _)
            || sqlOptions.InitialCatalog != database) throw new InvalidOperationException("Unsafe fixture cleanup target");
        await Execute(master, $"IF DB_ID(N'{database}') IS NOT NULL DROP DATABASE [{database}]");
        Console.WriteLine("Removed only this run's disposable SQL fixture database.");
    }
}

static async Task Execute(SqlConnection sql, string text)
{
    await using var command = sql.CreateCommand();
    command.CommandText = text;
    command.CommandTimeout = 120; // Local fixture DDL can be slow while other builds share the disk.
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
    public HttpStatusCode? FailStatus; public TimeSpan? RetryAfter;
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
        if (FailStatus is { } failure)
        {
            var response = new HttpResponseMessage(failure) { Content = new StringContent("sensitive-accounting-body") };
            if (RetryAfter is { } delay) response.Headers.RetryAfter = new System.Net.Http.Headers.RetryConditionHeaderValue(delay);
            return response;
        }
        return new HttpResponseMessage(HttpStatusCode.OK) { Content = JsonContent.Create(new AccountingCommandResult(AccountingCommandStatus.Applied, "44")) };
    }
}

sealed class ProviderTransport : HttpMessageHandler
{
    public int Patches; public int Stock; public bool Authenticated;
    public int ProductPatches; public JsonElement? ProductPatchValue;
    public JsonElement ProductPatch => ProductPatchValue!.Value;
    public int VendorId = 71;
    public JsonElement? WebhookRegistration;
    public int Requests; public HttpStatusCode? FailStatus;
    public string? CatalogOverride;
    public string? RetryAfterHeader;
    public HttpMethod? FailMethod;
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct)
    {
        Requests++;
        if (request.RequestUri?.Host == "webhook.basalam.com" && request.RequestUri.AbsolutePath == "/v1/webhooks"
            && request.Method == HttpMethod.Post)
        {
            if (request.Headers.Authorization?.ToString() != "Bearer fixture-grant")
                throw new InvalidOperationException("Registration must use the selected connection grant");
            using var body = JsonDocument.Parse(await request.Content!.ReadAsStringAsync(ct));
            WebhookRegistration = body.RootElement.Clone();
            return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent("{}", Encoding.UTF8, "application/json") };
        }
        if (request.RequestUri?.Host != "openapi.basalam.com")
            throw new InvalidOperationException("Unexpected Basalam fixture route");
        Authenticated = request.Headers.Authorization?.ToString() == "Bearer fixture-grant";
        if (!Authenticated) throw new InvalidOperationException("Missing connection-specific authentication");
        if (FailStatus is { } failure && (FailMethod is null || request.Method == FailMethod))
        {
            var response = new HttpResponseMessage(failure) { Content = new StringContent("provider-sensitive-fixture") };
            if (RetryAfterHeader is not null) response.Headers.TryAddWithoutValidation("Retry-After", RetryAfterHeader);
            return response;
        }
        if (request.Method == HttpMethod.Get && request.RequestUri.AbsolutePath == "/v1/vendors/71/products")
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(CatalogOverride ?? "{\"data\":[{\"id\":12,\"vendor\":{\"id\":71},\"title\":\"Fixture product\",\"sku\":\"catalog-sku\",\"price\":125,\"inventory\":3}],\"page\":1,\"total_page\":1}", Encoding.UTF8, "application/json")
            };
        if (request.RequestUri.AbsolutePath != "/v1/products/12") throw new InvalidOperationException("Unexpected Basalam fixture route");
        if (request.Method == HttpMethod.Patch)
        {
            using var body = JsonDocument.Parse(await request.Content!.ReadAsStringAsync(ct));
            if (body.RootElement.TryGetProperty("stock", out var stock)) { Stock = stock.GetInt32(); Patches++; }
            else { ProductPatchValue = body.RootElement.Clone(); ProductPatches++; }
        }
        return new HttpResponseMessage(HttpStatusCode.OK)
        { Content = JsonContent.Create(new { id = 12, vendor = new { id = VendorId }, title = "Fixture product", inventory = 3 }) };
    }
}

sealed class NoCapture : IIntegrationInventoryCapture
{
    public Task<int> CaptureAsync(CancellationToken ct) => throw new InvalidOperationException("Unexpected legacy capture");
    public Task<bool> ReconcileOneAsync(long connectionId, long mappingId, CancellationToken ct) => throw new InvalidOperationException("Unexpected legacy reconciliation");
}
sealed class RecordingEngagementOwner : IIntegrationEngagementPort
{
    public IntegrationSubscriptionCommand? Subscription;
    public IntegrationReviewCommand? Review;
    public List<IntegrationChatMessageCommand> Chats = [];
    public Task<EngagementCommandResult> ApplySubscriptionAsync(IntegrationSubscriptionCommand command, CancellationToken ct)
    { Subscription = command; return Task.FromResult(new EngagementCommandResult(EngagementCommandStatus.Applied)); }
    public Task<EngagementCommandResult> ApplyReviewAsync(IntegrationReviewCommand command, CancellationToken ct)
    { Review = command; return Task.FromResult(new EngagementCommandResult(EngagementCommandStatus.Applied)); }
    public Task<EngagementCommandResult> ApplyChatMessageAsync(IntegrationChatMessageCommand command, CancellationToken ct)
    { Chats.Add(command); return Task.FromResult(new EngagementCommandResult(EngagementCommandStatus.Applied)); }
}
sealed class NoReservations : IIntegrationInventoryReservation
{
    public Task ReserveAsync(OwnedIntegrationShop shop, string key, IReadOnlyCollection<IntegrationOrderLineCommand> lines, CancellationToken ct) => throw new InvalidOperationException("Unexpected order reservation");
    public Task ReleaseAsync(OwnedIntegrationShop shop, string key, CancellationToken ct) => throw new InvalidOperationException("Unexpected release");
    public Task CommitAsync(OwnedIntegrationShop shop, string key, CancellationToken ct) => throw new InvalidOperationException("Unexpected stock commit");
}
