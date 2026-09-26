using Hyper.Infrastructure.Data.Repository.Hyper;
using Hyper.Infrastructure.Features.Integrations;
using Hyper.Integration.Contracts;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Provider = Hyper.Integration.Domain.Entities.Integrations.IntegrationProvider;

static class SqlChecks
{
    public static async Task Run(Checks checks)
    {
        var input = Environment.GetEnvironmentVariable("ISOLATION_TEST_SQL") ?? throw new InvalidOperationException("ISOLATION_TEST_SQL required");
        var database = "HyperIsolationChecks_" + Guid.NewGuid().ToString("N");
        var cs = new SqlConnectionStringBuilder(input) { InitialCatalog = database };
        var options = new DbContextOptionsBuilder<HyperIntegrationContext>().UseSqlServer(cs.ConnectionString)
            .ReplaceService<IModelCustomizer, FixtureSchema>().Options;
        await using var db = new HyperIntegrationContext(options);
        try
        {
            await db.Database.EnsureCreatedAsync();
            ExternalIntegrationConnection Connection(int shopId, string tenantId) => new()
            {
                ShopId = shopId, TenantId = tenantId, Provider = Provider.Basalam, AccountIdentifier = "fixture",
                DisplayName = tenantId + shopId, CredentialsJson = "{}", IsEnabled = true
            };
            var own = Connection(7, "tenant-a");
            var foreignTenant = Connection(7, "tenant-b");
            var foreignShop = Connection(8, "tenant-a");
            db.AddRange(own, foreignTenant, foreignShop);
            await db.SaveChangesAsync();
            ExternalProductMapping Mapping(ExternalIntegrationConnection c, string key = "product") => new()
            { ConnectionId = c.Id, ShopId = c.ShopId, HyperProductId = 1, ExternalProductId = key, IsActive = true };
            IntegrationWebhookInbox Inbox(ExternalIntegrationConnection c) => new()
            { ConnectionId = c.Id, ExternalEventId = "fixture", EventType = "product.updated", PayloadJson = "{}", Status = 2, Error = "fixture" };
            ExternalOAuthToken Token(ExternalIntegrationConnection c) => new()
            { ConnectionId = c.Id, ShopId = c.ShopId, TenantId = c.TenantId, Provider = Provider.Basalam, AccessToken = "fixture-not-a-real-token", IsActive = true };
            var connections = new[] { own, foreignTenant, foreignShop };
            var mappings = connections.Select(c => Mapping(c)).ToArray();
            var inboxes = connections.Select(Inbox).ToArray();
            db.AddRange(mappings);
            db.AddRange(inboxes);
            db.AddRange(connections.Select(Token));
            // Imported/inconsistent children must never override the connection's actual owner.
            var inconsistentMapping = Mapping(foreignShop, "inconsistent");
            inconsistentMapping.ShopId = 7;
            var inconsistentToken = Token(foreignShop);
            inconsistentToken.ShopId = 7; inconsistentToken.Provider = Provider.Custom;
            db.AddRange(inconsistentMapping, inconsistentToken);
            await db.SaveChangesAsync();

            var management = new IntegrationManagementApi(db);
            var mappingApi = new IntegrationMappingApi(db);
            var syncCalls = 0;
            var sync = new IntegrationSyncApi(db, Probe.Create<IIntegrationSynchronizationService>((_, _) =>
            { syncCalls++; return Task.FromResult(123L); }));
            var outboxCalls = 0;
            var events = new IntegrationAccountingEventIngress(db, Probe.Create<IIntegrationOutbox>((_, _) =>
            { outboxCalls++; return Task.FromResult(234L); }));
            var tokens = new IntegrationTokenApi(db, Probe.Create<IAdminMerchantSimulationService>((_, _) =>
                throw new InvalidOperationException("UnexpectedSimulationCall")));
            var dashboard = new IntegrationDashboardQuery(db, Probe.Create<IIntegrationPlatformCatalogPort>((_, _) =>
                Task.FromResult<IReadOnlyList<IntegrationPlatformProduct>>([])));

            await checks.Test("SQL connection list is shop/tenant scoped", async () =>
                (await management.ListConnectionsAsync(new(7, "tenant-a"))).Select(x => x.Id).SequenceEqual([own.Id]));
            await checks.Test("SQL token list excludes foreign and inconsistent owners", async () =>
                (await tokens.ListAsync(7, "tenant-a", default)).Select(x => x.ConnectionId).SequenceEqual([own.Id]));
            await checks.Test("SQL dashboard isolates connection and child records", async () =>
            {
                var result = await dashboard.GetAsync(7, "tenant-a", default);
                return result.Connections == 1 && result.MappedProducts == 1 && result.EnabledConnections == 1
                    && result.RecentInbox.Count == 1 && result.RecentInbox[0].Id == inboxes[0].Id
                    && result.ConnectionsHealth.Single().ConnectionId == own.Id;
            });
            for (var i = 1; i < connections.Length; i++)
            {
                var target = connections[i];
                var mapping = mappings[i];
                var inbox = inboxes[i];
                var command = new IntegrationConnectionCommandRequest(7, "tenant-a", target.Id);
                var kind = i == 1 ? "foreign tenant" : "foreign shop";
                await checks.Test($"SQL {kind} enable denied", async () =>
                    !await management.SetConnectionEnabledAsync(command, false)
                    && await db.ExternalIntegrationConnections.AsNoTracking().AnyAsync(x => x.Id == target.Id && x.IsEnabled));
                await checks.Test($"SQL {kind} inbox replay denied", async () =>
                    await management.ReplayWebhookAsync(command, inbox.Id) is null
                    && await db.IntegrationWebhookInbox.AsNoTracking().AnyAsync(x => x.Id == inbox.Id && x.Status == 2));
                await checks.Test($"SQL {kind} mapping list empty", async () =>
                    (await mappingApi.ListProductMappingsAsync(command, default)).Count == 0);
                await checks.Test($"SQL {kind} mapping create denied", async () =>
                    await mappingApi.CreateProductMappingAsync(new(7, "tenant-a", target.Id, 2, "new"), default) is null
                    && !await db.ExternalProductMappings.AnyAsync(x => x.ConnectionId == target.Id && x.ExternalProductId == "new"));
                await checks.Test($"SQL {kind} mapping deactivate denied", async () =>
                    !await mappingApi.DeactivateProductMappingAsync(command, mapping.Id, default)
                    && await db.ExternalProductMappings.AsNoTracking().AnyAsync(x => x.Id == mapping.Id && x.IsActive));
                await checks.Test($"SQL {kind} sync denied before downstream call", async () =>
                    await sync.TriggerAsync(new(7, "tenant-a", target.Id), default) is null && syncCalls == 0);
                await checks.Test($"SQL {kind} inventory event denied before outbox", async () =>
                    await events.ReceiveInventoryChangedAsync(new(7, "tenant-a", target.Id, "product", null, 1, 1)) is null && outboxCalls == 0);
            }
            await checks.Test("SQL inconsistent mapping cannot publish for another shop", async () =>
                await events.ReceiveInventoryChangedAsync(new(7, "tenant-a", foreignShop.Id, "inconsistent", null, 1, 1)) is null
                && outboxCalls == 0);
            await checks.Test("SQL inconsistent token cannot revoke another shops connection", async () =>
                !await tokens.RevokeAsync(7, "tenant-a", foreignShop.Id, default)
                && await db.ExternalOAuthTokens.AsNoTracking().AnyAsync(x => x.Id == inconsistentToken.Id && x.IsActive)
                && await db.ExternalIntegrationConnections.AsNoTracking().AnyAsync(x => x.Id == foreignShop.Id && x.IsEnabled));
            await checks.Test("SQL foreign tenant token revoke denied", async () =>
                !await tokens.RevokeAsync(7, "tenant-a", foreignTenant.Id, default)
                && await db.ExternalOAuthTokens.AsNoTracking().AnyAsync(x => x.ConnectionId == foreignTenant.Id && x.IsActive));
            await checks.Test("SQL own connection cannot replay foreign inbox ID", async () =>
                await management.ReplayWebhookAsync(new(7, "tenant-a", own.Id), inboxes[1].Id) is null);
            await checks.Test("SQL own connection cannot deactivate foreign mapping ID", async () =>
                !await mappingApi.DeactivateProductMappingAsync(new(7, "tenant-a", own.Id), mappings[1].Id, default));

            await checks.Test("SQL own mapping can be created", async () =>
                await mappingApi.CreateProductMappingAsync(new(7, "tenant-a", own.Id, 2, "owned-new"), default) is not null);
            await checks.Test("SQL own inbox replay permitted", async () =>
                await management.ReplayWebhookAsync(new(7, "tenant-a", own.Id), inboxes[0].Id) is not null);
            await checks.Test("SQL own sync permitted", async () =>
            {
                var before = syncCalls;
                return await sync.TriggerAsync(new(7, "tenant-a", own.Id), default) is not null && syncCalls == before + 1;
            });
            await checks.Test("SQL own inventory publication permitted", async () =>
            {
                var before = outboxCalls;
                return await events.ReceiveInventoryChangedAsync(new(7, "tenant-a", own.Id, "product", null, 1, 1)) is not null && outboxCalls == before + 1;
            });
            await checks.Test("SQL own mapping deactivate permitted", async () =>
                await mappingApi.DeactivateProductMappingAsync(new(7, "tenant-a", own.Id), mappings[0].Id, default));
            await checks.Test("SQL own token revoke disables token and connection only", async () =>
                await tokens.RevokeAsync(7, "tenant-a", own.Id, default)
                && !await db.ExternalOAuthTokens.AnyAsync(x => x.ConnectionId == own.Id && x.IsActive)
                && !await db.ExternalIntegrationConnections.AnyAsync(x => x.Id == own.Id && x.IsEnabled)
                && await db.ExternalIntegrationConnections.CountAsync(x => x.IsEnabled) == 2);
        }
        finally
        {
            if (db.Database.GetDbConnection().Database != database || !database.StartsWith("HyperIsolationChecks_", StringComparison.Ordinal))
                throw new InvalidOperationException("FixtureCleanupTargetMismatch");
            await db.Database.EnsureDeletedAsync();
            Console.WriteLine("Disposable isolation database removed.");
        }
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
