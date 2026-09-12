using System.Text.Json;
using Hyper.Domain.Entities.Integrations;
using Hyper.Domain.Features.Integrations;
using Hyper.Infrastructure.Data.Repository.Hyper;
using Hyper.Infrastructure.Features.Integrations;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

internal static class OutboxSqlChecks
{
    public static async Task Run(string settingsPath)
    {
        using var settings = JsonDocument.Parse(await File.ReadAllTextAsync(settingsPath),
            new JsonDocumentOptions { CommentHandling = JsonCommentHandling.Skip, AllowTrailingCommas = true });
        var builder = new SqlConnectionStringBuilder(settings.RootElement.GetProperty("ConnectionStrings").GetProperty("Domain").GetString());
        if (builder.InitialCatalog != "Hyperyek" || builder.DataSource is not ("." or "(local)" or "localhost"))
            throw new Exception("This fixture requires local Hyperyek.");
        builder.DataSource = "lpc:."; builder.Encrypt = SqlConnectionEncryptOption.Optional;
        HyperIntegrationContext Context() => new(new DbContextOptionsBuilder<HyperIntegrationContext>().UseSqlServer(builder.ConnectionString).Options);
        await using var setup = Context();
        await using var legacy = new HyperSqlServerContext(new DbContextOptionsBuilder<HyperSqlServerContext>().UseSqlServer(builder.ConnectionString).Options);
        var productAndShop = await (from product in legacy.TblProducts.AsNoTracking()
                                   join sourceShop in legacy.TblShops.AsNoTracking() on product.Shopid equals sourceShop.Shopid
                                   where product.TenantId == sourceShop.TenantId
                                   select new { Product = product, Id = sourceShop.Shopid, sourceShop.TenantId }).FirstAsync();
        var shop = productAndShop;
        var connection = new ExternalIntegrationConnection
        {
            ShopId = shop.Id, TenantId = IntegrationConnectionScope.CanonicalTenant(shop.Id, shop.TenantId),
            Provider = IntegrationProvider.Custom, AccountIdentifier = "outbox-fixture-" + Guid.NewGuid().ToString("N"),
            DisplayName = "outbox-test-only", CredentialType = IntegrationCredentialType.ApiKey, CredentialsJson = "{}"
        };
        setup.ExternalIntegrationConnections.Add(connection);
        await setup.SaveChangesAsync();
        var mapping = new ExternalProductMapping { ConnectionId = connection.Id, ShopId = shop.Id, HyperProductId = 1, ExternalProductId = "10" };
        var adapter = new FakeAdapter();
        long reservationId = 0;
        IntegrationOutbox Service(HyperIntegrationContext db) => new(db, new IntegrationStrategyResolver([adapter]));
        try
        {
            setup.ExternalProductMappings.Add(mapping); await setup.SaveChangesAsync();
            async Task<long> Enqueue(long version, decimal quantity)
            {
                await using var context = Context();
                return await Service(context).EnqueueInventoryAsync(connection.Id, mapping.Id, version, quantity, default);
            }
            var ids = await Task.WhenAll(Enqueue(1, 8), Enqueue(1, 8));
            if (ids.Distinct().Count() != 1 || await setup.IntegrationOutbox.CountAsync(x => x.ConnectionId == connection.Id) != 1)
                throw new Exception("Concurrent replay created duplicates.");
            try { await Enqueue(1, 9); throw new Exception("Conflicting payload accepted."); }
            catch (InvalidOperationException error) when (error.Message == "SourceVersionConflict") { }
            var secondId = await Enqueue(3, 4);
            try { await Enqueue(2, 6); throw new Exception("Stale version accepted."); }
            catch (InvalidOperationException error) when (error.Message == "StaleSourceVersion") { }

            // Two independent SQL sessions: one holds the connection lock across a paused provider call.
            var entered = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            var release = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            adapter.Send = async () => { entered.TrySetResult(); await release.Task.WaitAsync(TimeSpan.FromSeconds(15)); };
            await using var first = Context(); await using var second = Context();
            var firstCall = Service(first).ProcessConnectionAsync(connection.Id, default);
            try
            {
                await entered.Task.WaitAsync(TimeSpan.FromSeconds(15));
                // Even with an expired lease, an active sender's SQL session lock prevents overtaking.
                await setup.IntegrationOutbox.Where(x => x.Id == ids[0]).ExecuteUpdateAsync(s => s.SetProperty(x => x.LeaseExpiresAtUtc, DateTime.UtcNow.AddMinutes(-1)));
                if (await Service(second).ProcessConnectionAsync(connection.Id, default)) throw new Exception("Concurrent sender acquired active connection.");
            }
            finally { release.TrySetResult(); }
            if (!await firstCall) throw new Exception("First send did not process.");
            if (adapter.Quantities.Count != 1 || adapter.Quantities[0] != 8) throw new Exception("Wrong first stock.");

            adapter.Send = () => throw new IntegrationProviderException("Http429", true, TimeSpan.FromMinutes(2));
            await using (var context = Context()) await Service(context).ProcessConnectionAsync(connection.Id, default);
            var retried = await setup.IntegrationOutbox.AsNoTracking().SingleAsync(x => x.Id == secondId);
            if (retried.Status != 0 || retried.Attempts != 1 || retried.LastError != "Http429" || retried.NextAttemptAtUtc < DateTime.UtcNow.AddSeconds(110))
                throw new Exception("Retry-After was ignored.");
            var thirdId = await Enqueue(4, 2);
            await using (var context = Context())
                if (await Service(context).ProcessConnectionAsync(connection.Id, default)) throw new Exception("Later command overtook retry.");
            // Simulate process crash after committing a claim; a new session recovers expired lease.
            await setup.IntegrationOutbox.Where(x => x.Id == secondId).ExecuteUpdateAsync(s => s.SetProperty(x => x.Status, (byte)1)
                .SetProperty(x => x.LeaseId, Guid.NewGuid()).SetProperty(x => x.LeaseExpiresAtUtc, DateTime.UtcNow.AddMinutes(-1)));
            adapter.Send = () => Task.CompletedTask;
            await using (var context = Context()) await Service(context).ProcessConnectionAsync(connection.Id, default);
            if ((await setup.IntegrationOutbox.AsNoTracking().SingleAsync(x => x.Id == secondId)).Status != 2) throw new Exception("Expired lease not recovered.");
            adapter.Send = () => throw new IntegrationProviderException("Http401", false);
            await using (var context = Context()) await Service(context).ProcessConnectionAsync(connection.Id, default);
            if ((await setup.IntegrationOutbox.AsNoTracking().SingleAsync(x => x.Id == thirdId)).Status != 3) throw new Exception("Permanent error not dead-lettered.");
            var dashboard = await new IntegrationDashboardQuery(setup).GetAsync(connection.ShopId, connection.TenantId, default);
            if (!dashboard.RecentOutbox.Any(x => x.Id == thirdId && x.Status == 3) || dashboard.OutboxCount(2) < 2)
                throw new Exception("Dashboard omitted worker outcomes.");
            var otherTenant = await new IntegrationDashboardQuery(setup).GetAsync(connection.ShopId, "ungranted-fixture", default);
            if (otherTenant.RecentOutbox.Any(x => x.Id == thirdId)) throw new Exception("Dashboard leaked outbox tenant.");
            await using (var context = Context())
            {
                if (await Service(context).RetryAsync(connection.Id + 1, thirdId, default)) throw new Exception("Cross-connection retry accepted.");
                if (!await Service(context).RetryAsync(connection.Id, thirdId, default)) throw new Exception("Latest dead letter cannot retry.");
                if (await Service(context).RetryAsync(connection.Id, thirdId, default)) throw new Exception("Non-dead message retried.");
            }
            adapter.Send = () => throw new IntegrationProviderException("Http401", false);
            await using (var context = Context()) await Service(context).ProcessConnectionAsync(connection.Id, default);
            await Enqueue(5, 1);
            await using (var context = Context())
                if (await Service(context).RetryAsync(connection.Id, thirdId, default)) throw new Exception("Old dead letter could overwrite newer stock.");
            // Source capture reads existing product data, and writes only fixture integration rows.
            await setup.IntegrationOutbox.Where(x => x.ConnectionId == connection.Id).ExecuteDeleteAsync();
            await setup.ExternalProductMappings.Where(x => x.Id == mapping.Id)
                .ExecuteUpdateAsync(s => s.SetProperty(x => x.HyperProductId, productAndShop.Product.Id));
            async Task<bool> Capture(bool verified = true)
            {
                await using var context = Context();
                return await new IntegrationInventoryCapture(context, Service(context), new IntegrationStrategyResolver([adapter]),
                    Options.Create(new IntegrationInventoryCaptureOptions { AccountingStockSourceVerified = verified }))
                    .CaptureOneAsync(connection.Id, mapping.Id, default);
            }
            if (await Capture(false)) throw new Exception("Unverified SQL stock source was activated.");
            if (!await Capture() || await Capture()) throw new Exception("Initial capture or unchanged checkpoint incorrect.");
            var baselinePayload = await setup.IntegrationOutbox.AsNoTracking().SingleAsync(x => x.ConnectionId == connection.Id);
            var baselineQuantity = JsonSerializer.Deserialize<ExternalInventoryUpdate>(baselinePayload.PayloadJson)!.Quantity;
            var reservation = new InventoryReservationLog { ShopId = shop.Id, HyperProductId = productAndShop.Product.Id,
                ReservationKey = "outbox-fixture-" + Guid.NewGuid().ToString("N"), Source = "outbox-fixture", Quantity = Math.Abs(productAndShop.Product.Accountingstock) + 1 };
            setup.InventoryReservationLogs.Add(reservation); await setup.SaveChangesAsync(); reservationId = reservation.Id;
            if (await Capture() != (baselineQuantity != 0)) throw new Exception("Reservation availability capture incorrect.");
            var reservedPayload = await setup.IntegrationOutbox.AsNoTracking().Where(x => x.ConnectionId == connection.Id).OrderByDescending(x => x.SourceVersion).FirstAsync();
            if (JsonSerializer.Deserialize<ExternalInventoryUpdate>(reservedPayload.PayloadJson)!.Quantity != 0)
                throw new Exception("Whole-shop reserved stock was not clamped at zero.");
            await setup.InventoryReservationLogs.Where(x => x.Id == reservationId).ExecuteUpdateAsync(s => s.SetProperty(x => x.Status, (byte)1).SetProperty(x => x.ReleasedAtUtc, DateTime.UtcNow));
            if (await Capture() != (baselineQuantity != 0)) throw new Exception("Released reservation did not restore availability.");
            Console.WriteLine("PASS SQL inventory capture: real product read, explicit source verification gate, initial/unchanged snapshots, reservations and release; no legacy writes.");
            Console.WriteLine("PASS SQL outbox: concurrent idempotency, payload conflict, stale version, session lock across expired lease, ordering, Retry-After, crash recovery, dead letter, dashboard and tenant scope. Provider mocked; no external calls.");
        }
        finally
        {
            if (reservationId != 0) await setup.InventoryReservationLogs.Where(x => x.Id == reservationId && x.Source == "outbox-fixture").ExecuteDeleteAsync();
            // Delete only this fixture's newly committed integration records; never touch legacy business data.
            await setup.IntegrationOutbox.Where(x => x.ConnectionId == connection.Id).ExecuteDeleteAsync();
            await setup.ExternalProductMappings.Where(x => x.ConnectionId == connection.Id).ExecuteDeleteAsync();
            await setup.ExternalIntegrationConnections.Where(x => x.Id == connection.Id).ExecuteDeleteAsync();
        }
        if (await setup.IntegrationOutbox.AnyAsync(x => x.ConnectionId == connection.Id)
            || await setup.ExternalIntegrationConnections.AnyAsync(x => x.Id == connection.Id)) throw new Exception("Fixture cleanup failed.");
        Console.WriteLine("PASS SQL outbox fixture cleanup verified.");
    }

    private sealed class FakeAdapter : IExternalIntegrationAdapter
    {
        public IntegrationProvider Provider => IntegrationProvider.Custom;
        public bool IsImplemented => true;
        public bool SupportsCredentialType(IntegrationCredentialType type) => type == IntegrationCredentialType.ApiKey;
        public Func<Task> Send { get; set; } = () => Task.CompletedTask;
        public List<decimal> Quantities { get; } = [];
        public Task<IReadOnlyCollection<ExternalCatalogItem>> ReadCatalogAsync(ExternalIntegrationConnection connection, CancellationToken ct) => throw new NotSupportedException();
        public async Task PublishInventoryAsync(ExternalIntegrationConnection connection, IReadOnlyCollection<ExternalInventoryUpdate> updates, CancellationToken ct)
        {
            await Send(); Quantities.Add(updates.Single().Quantity);
        }
    }
}
