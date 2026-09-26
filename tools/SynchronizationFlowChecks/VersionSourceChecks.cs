using System.Security.Claims;
using Hyper.Infrastructure.Data.Repository.Hyper;
using Hyper.Infrastructure.Features.Integrations;
using Hyper.Integration.Api;
using Hyper.Integration.Contracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

internal static class VersionSourceChecks
{
    public static async Task Run(HyperIntegrationContext db, DbContextOptions<HyperIntegrationContext> options,
        IIntegrationStrategyResolver strategies, long connectionId, Action<bool, string> check)
    {
        var mappings = Enumerable.Range(901, 3).Select(id => new ExternalProductMapping
        { ConnectionId = connectionId, ShopId = 7, HyperProductId = 45, ExternalProductId = id.ToString() }).ToArray();
        db.ExternalProductMappings.AddRange(mappings);
        await db.SaveChangesAsync();
        var mapping = mappings[0];
        var scope = new IntegrationConnectionCommandRequest(7, "tenant-a", connectionId);
        var api = new IntegrationVersionSourceApi(db);
        var outbox = new IntegrationOutbox(db, strategies);
        var controller = new IntegrationVersionSourceController(api, new IntegrationScopeAuthorization())
        { ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() } };
        var chooseEvents = new IntegrationVersionSourceChange(IntegrationVersionSource.AccountingEvents, IntegrationVersionSource.Unassigned, 0);
        check(await controller.Change(connectionId, mapping.Id, 7, "tenant-a", chooseEvents, default) is ForbidResult,
            "anonymous source selection is forbidden");
        controller.HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity([new Claim("scope", "shop:7;tenant:tenant-b")], "fixture"));
        check(await controller.Read(connectionId, mapping.Id, 7, "tenant-a", default) is ForbidResult,
            "wrong tenant claim cannot inspect source state");
        check(await controller.Change(connectionId, mapping.Id, 7, "tenant-b", chooseEvents, default) is NotFoundResult,
            "authorized foreign scope cannot change another tenant mapping");
        controller.HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity([new Claim("scope", "shop:7;tenant:tenant-a")], "fixture"));
        check((await controller.Read(connectionId, mapping.Id, 7, "tenant-a", default) as OkObjectResult)?.Value is
            IntegrationVersionSourceResult { Source: IntegrationVersionSource.Unassigned, LastVersion: 0 }, "new mapping reports unassigned source");
        check(await controller.Change(connectionId, mapping.Id, 7, "tenant-a", chooseEvents with { Source = (IntegrationVersionSource)99 }, default)
            is BadRequestObjectResult, "undefined version owner cannot be selected");
        check((await controller.Change(connectionId, mapping.Id, 7, "tenant-a", chooseEvents, default) as OkObjectResult)?.Value is
            IntegrationVersionSourceResult { Status: "Changed", VersionFloor: 0 }, "scoped owner selection is persisted");

        async Task Reject(Func<Task> action, string expected)
        {
            try { await action(); }
            catch (InvalidOperationException ex) when (ex.Message == expected) { check(true, expected); return; }
            throw new InvalidOperationException("Expected " + expected);
        }
        var message = await outbox.EnqueueInventoryAsync(connectionId, mapping.Id, 10, 3, default);
        await Reject(() => outbox.EnqueueCapturedInventoryAsync(connectionId, mapping.Id, 11, 3, default), "VersionSourceConflict");
        check(await db.IntegrationOutbox.CountAsync(x => x.MappingId == mapping.Id) == 1, "rejected competing source creates no message");
        var change = new IntegrationVersionSourceChange(IntegrationVersionSource.InventoryCapture, IntegrationVersionSource.AccountingEvents, 10);
        check((await api.ChangeAsync(scope, mapping.Id, change with { ExpectedLastVersion = 9 }, default))?.ErrorCode == "VersionSourceSnapshotChanged",
            "stale high-water snapshot cannot change source");
        check((await api.ChangeAsync(scope, mapping.Id, change with { ExpectedSource = IntegrationVersionSource.Unassigned }, default))?.ErrorCode == "VersionSourceSnapshotChanged",
            "stale owner snapshot cannot change source");
        check(await controller.Change(connectionId, mapping.Id, 7, "tenant-a", change, default) is ConflictObjectResult,
            "pending messages block source switch with HTTP 409");
        await db.IntegrationOutbox.Where(x => x.Id == message).ExecuteUpdateAsync(s => s.SetProperty(x => x.Status, (byte)1));
        check((await api.ChangeAsync(scope, mapping.Id, change, default))?.ErrorCode == "VersionSourceHasPendingMessages",
            "an in-flight leased message also blocks source switch");
        // Controlled terminal failure, not a simulated provider acknowledgment.
        await db.IntegrationOutbox.Where(x => x.Id == message).ExecuteUpdateAsync(s => s.SetProperty(x => x.Status, (byte)3));
        check((await api.ChangeAsync(scope, mapping.Id, change, default)) is { Status: "Changed", VersionFloor: 10, LastVersion: 10 },
            "switch preserves last version as the new owner floor");
        check(!await outbox.RetryAsync(connectionId, message, default), "old-owner dead letter cannot be replayed after a source switch");
        await Reject(() => outbox.EnqueueCapturedInventoryAsync(connectionId, mapping.Id, 10, 3, default), "StaleSourceVersion");
        var captureService = new IntegrationInventoryCapture(db, outbox, strategies,
            Options.Create(new IntegrationInventoryCaptureOptions { AccountingStockSourceVerified = true }), new SourceCatalog());
        check(await captureService.CaptureOneAsync(connectionId, mapping.Id, default),
            "first capture after source switch does not treat identical old-owner failed payload as its checkpoint");
        var captured = await db.IntegrationOutbox.Where(x => x.MappingId == mapping.Id && x.SourceVersion == 11)
            .Select(x => x.Id).SingleAsync();
        check(captured > message, "new owner continues above the previous source floor");
        check(!await captureService.CaptureOneAsync(connectionId, mapping.Id, default),
            "subsequent unchanged capture uses the new owner checkpoint without duplicating the message");
        await Reject(() => outbox.EnqueueInventoryAsync(connectionId, mapping.Id, 11, 3, default), "VersionSourceConflict");
        await using (var restarted = new HyperIntegrationContext(options))
            await Reject(() => new IntegrationOutbox(restarted, strategies).EnqueueInventoryAsync(connectionId, mapping.Id, 12, 3, default),
                "VersionSourceConflict");
        var inventoryController = new IntegrationAccountingEventController(new IntegrationAccountingEventIngress(db, outbox), new IntegrationScopeAuthorization())
        { ControllerContext = controller.ControllerContext };
        check(await inventoryController.InventoryChanged(new(7, "tenant-a", connectionId, "901", null, 12, 3), default) is ConflictObjectResult,
            "accounting inventory endpoint exposes ownership conflict as HTTP 409, not a server error");
        check((await new IntegrationAccountingProductEventIngress(db, outbox).ReceiveAsync(new(7, "tenant-a", connectionId, "901", null, 12, "title", null)))
            is { Status: "Conflict", ErrorCode: "VersionSourceConflict" }, "product and inventory share the same durable owner");

        var legacy = mappings[1];
        db.IntegrationOutbox.Add(new() { ConnectionId = connectionId, MappingId = legacy.Id, SourceVersion = 50,
            Operation = IntegrationOutbox.InventoryOperation, PayloadJson = "{}", Status = 2, CreatedAtUtc = DateTime.UtcNow, NextAttemptAtUtc = DateTime.UtcNow });
        await db.SaveChangesAsync();
        await Reject(() => outbox.EnqueueInventoryAsync(connectionId, legacy.Id, 51, 1, default), "VersionSourceUnassigned");
        await Reject(() => outbox.EnqueueCapturedInventoryAsync(connectionId, legacy.Id, 51, 1, default), "VersionSourceUnassigned");
        check((await api.ChangeAsync(scope, legacy.Id, chooseEvents with { ExpectedLastVersion = 50 }, default)) is { Status: "Changed", VersionFloor: 50 },
            "pre-migration history requires explicit ownership selection without guessing origin");
        await outbox.EnqueueInventoryAsync(connectionId, legacy.Id, 51, 1, default);
        check((await api.ReadAsync(scope, legacy.Id, default)) is { LastVersion: 51, VersionFloor: 50 }, "legacy stream continues monotonically after source selection");

        var raceMapping = mappings[2];
        async Task<string> Compete(bool capture)
        {
            await using var context = new HyperIntegrationContext(options);
            var publisher = new IntegrationOutbox(context, strategies);
            try
            {
                if (capture) await publisher.EnqueueCapturedInventoryAsync(connectionId, raceMapping.Id, 1, 4, default);
                else await publisher.EnqueueInventoryAsync(connectionId, raceMapping.Id, 1, 4, default);
                return capture ? "Capture" : "Events";
            }
            catch (InvalidOperationException ex) when (ex.Message == "VersionSourceConflict") { return "Conflict"; }
        }
        var outcomes = await Task.WhenAll(Compete(false), Compete(true));
        check(outcomes.Count(x => x == "Conflict") == 1
            && await db.IntegrationOutbox.CountAsync(x => x.MappingId == raceMapping.Id) == 1,
            "concurrent API/capture first writers atomically elect one source and one message");
        check((await api.ReadAsync(scope, raceMapping.Id, default))?.Source ==
            (outcomes.Contains("Events") ? IntegrationVersionSource.AccountingEvents : IntegrationVersionSource.InventoryCapture),
            "the elected source survives fresh contexts and matches the committed winner");
    }

    private sealed class SourceCatalog : IIntegrationPlatformCatalogPort
    {
        public Task<IReadOnlyList<IntegrationPlatformProduct>> GetProductsAsync(int shopId, string tenantId, CancellationToken ct) =>
            Task.FromResult<IReadOnlyList<IntegrationPlatformProduct>>([new(45, "fixture", null, 1, 3, true, true, null, true)]);
    }
}
