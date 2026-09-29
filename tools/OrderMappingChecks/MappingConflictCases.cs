using Hyper.Infrastructure.Features.Integrations;
using Microsoft.EntityFrameworkCore;

internal sealed partial class MappingCases
{
    private async Task PreflightConflictsAsync()
    {
        foreach (var kind in new[] { "shop", "tenant-case", "account-case", "credential", "provider", "disabled", "stored-disabled", "order-case", "row-shop", "parcel", "zero-invoice", "negative-invoice" })
        {
            var connection = (await fixture.ConnectionsAsync())[0];
            var job = await fixture.SaleAsync(connection, "scope-order", kind == "parcel" ? "contradictory-parcel" : null);
            var code = "OrderMappingScopeMismatch";
            switch (kind)
            {
                case "shop": connection.ShopId++; break;
                case "tenant-case": connection.TenantId = connection.TenantId.ToUpperInvariant(); break;
                case "account-case": connection.AccountIdentifier = connection.AccountIdentifier.ToUpperInvariant(); break;
                case "credential": connection.CredentialType = IntegrationCredentialType.ApiKey; break;
                case "provider": connection.Provider = IntegrationProvider.Custom; break;
                case "disabled": connection.IsEnabled = false; break;
                case "stored-disabled":
                    await using (var db = fixture.Open())
                        await db.ExternalIntegrationConnections.Where(x => x.Id == connection.Id).ExecuteUpdateAsync(s => s.SetProperty(x => x.IsEnabled, false));
                    break;
                case "order-case":
                    await fixture.SeedMappingAsync(connection, "SCOPE-ORDER", 501);
                    code = "OrderMappingIdentityConflict";
                    break;
                case "row-shop":
                    await fixture.SeedMappingAsync(connection, "scope-order", 501, shop: connection.ShopId + 50);
                    code = "OrderMappingIdentityConflict";
                    break;
                case "parcel":
                    await fixture.SeedMappingAsync(connection, "scope-order", 501, "trusted-parcel");
                    code = "OrderMappingParcelConflict";
                    break;
                case "zero-invoice":
                case "negative-invoice":
                    await fixture.SeedMappingAsync(connection, "scope-order", kind == "zero-invoice" ? 0 : -1);
                    code = "OrderMappingInvoiceConflict";
                    break;
            }
            var before = await fixture.RowsAsync(connection);
            var commands = new CommandProbe();
            var reservations = new ReservationProbe();
            await ExpectCodeAsync(() => DispatchAsync(connection, job, commands, reservations), code);
            Check(commands.Calls == 0 && reservations.ReserveCalls == 0 && reservations.CommitCalls == 0 && reservations.ReleaseCalls == 0,
                "Preflight " + kind + " must reject before any accounting/reservation mutation.");
            var after = await fixture.RowsAsync(connection);
            Check(before.Length == after.Length && before.Zip(after).All(pair => SameReceipt(pair.First, pair.Second)),
                "Preflight " + kind + " must preserve persisted receipt fields.");
        }
        Console.WriteLine("PASS scope, ordinal case, invoice and explicit parcel conflicts before remote mutation");
    }

    private async Task ScopeRevalidationAsync()
    {
        var connection = (await fixture.ConnectionsAsync())[0];
        var command = MappingFixture.Command(connection, "scope-revalidation");
        await using (var db = fixture.Open())
            await new IntegrationOrderMappingReceipt(db).PreflightAsync(connection, command, default);
        await using (var db = fixture.Open())
            await db.ExternalIntegrationConnections.Where(x => x.Id == connection.Id)
                .ExecuteUpdateAsync(s => s.SetProperty(x => x.IsEnabled, false));
        await ExpectCodeAsync(() => RecordAsync(connection, command, Ack(601)), "OrderMappingScopeMismatch");
        Check((await fixture.RowsAsync(connection)).Length == 0, "Record must recheck stored scope after preflight.");

        connection = (await fixture.ConnectionsAsync())[0];
        command = MappingFixture.Command(connection, "identity-bounds");
        foreach (var invalid in new[]
        {
            command with { ConnectionId = connection.Id + 900000 },
            command with { ShopId = connection.ShopId + 1 },
            command with { TenantId = "another-tenant" },
            command with { TenantId = " " },
            command with { TenantId = new string('t', 31) },
            command with { ExternalOrderId = "" },
            command with { ExternalOrderId = " order" },
            command with { ExternalOrderId = "order " },
            command with { ExternalOrderId = "order\nvalue" },
            command with { ExternalOrderId = new string('o', 129) }
        })
        {
            await using var db = fixture.Open();
            await ExpectCodeAsync(() => new IntegrationOrderMappingReceipt(db).PreflightAsync(connection, invalid, default),
                "OrderMappingScopeMismatch");
        }
        Check((await fixture.RowsAsync(connection)).Length == 0, "Invalid command identities must not produce receipts.");
        Console.WriteLine("PASS record-time scope revalidation and command identity limits");
    }

    private async Task ExistingReceiptsAsync()
    {
        var connection = (await fixture.ConnectionsAsync())[0];
        await fixture.SeedMappingAsync(connection, "existing-invoice", 701, "trusted-parcel");
        var before = (await fixture.RowsAsync(connection)).Single();
        await ExpectCodeAsync(() => RecordAsync(connection, MappingFixture.Command(connection, "existing-invoice"), Ack(702)),
            "OrderMappingInvoiceConflict");
        Check(SameReceipt(before, (await fixture.RowsAsync(connection)).Single()), "Different invoice ACK must not overwrite any receipt field.");

        foreach (var badInvoice in new long[] { 0, -15 })
        {
            var corrupt = (await fixture.ConnectionsAsync())[0];
            await fixture.SeedMappingAsync(corrupt, "corrupt-invoice", badInvoice);
            var snapshot = (await fixture.RowsAsync(corrupt)).Single();
            await ExpectCodeAsync(() => RecordAsync(corrupt, MappingFixture.Command(corrupt, "corrupt-invoice"), Ack(703)),
                "OrderMappingInvoiceConflict");
            Check(SameReceipt(snapshot, (await fixture.RowsAsync(corrupt)).Single()), "Nonpositive existing invoice must stay unchanged for explicit repair.");
        }

        var nullInvoice = (await fixture.ConnectionsAsync())[0];
        await fixture.SeedMappingAsync(nullInvoice, "fill-invoice", null, "trusted-parcel", 9);
        var job = await fixture.SaleAsync(nullInvoice, "fill-invoice"); // Parcel property omitted.
        var commands = new CommandProbe { Behavior = (_, _) => Task.FromResult(Ack(704, BusinessCommandStatus.Duplicate)) };
        await DispatchAsync(nullInvoice, job, commands, new());
        var filled = (await fixture.RowsAsync(nullInvoice)).Single();
        Check(filled.HyperSaleOrderId == 704 && filled.ExternalParcelId == "trusted-parcel" && filled.Status == 9,
            "Missing incoming parcel must allow null invoice fill and preserve trusted parcel/status.");
        Check(filled.LastSyncAtUtc > new DateTime(2020, 1, 1) && commands.Calls == 1
            && commands.Received.Single().ExternalParcelId is null,
            "Omitted parcel must reach receipt as null and refresh receipt timestamp.");
        await RecordAsync(nullInvoice, MappingFixture.Command(nullInvoice, "fill-invoice", "trusted-parcel"), Ack(704));
        var replayed = (await fixture.RowsAsync(nullInvoice)).Single();
        Check(replayed.Id == filled.Id && replayed.ExternalParcelId == "trusted-parcel" && replayed.Status == 9,
            "Exact trusted parcel and invoice replay must be accepted without changing binding/status.");

        var nullParcel = (await fixture.ConnectionsAsync())[0];
        await fixture.SeedMappingAsync(nullParcel, "raw-parcel", null);
        job = await fixture.SaleAsync(nullParcel, "raw-parcel", "untrusted-webhook-parcel");
        await DispatchAsync(nullParcel, job, new() { Behavior = (_, _) => Task.FromResult(Ack(705)) }, new());
        var raw = (await fixture.RowsAsync(nullParcel)).Single();
        Check(raw.HyperSaleOrderId == 705 && raw.ExternalParcelId is null && raw.Status == 7,
            "Raw parcel must never fill an existing null binding while invoice is filled.");
        Console.WriteLine("PASS existing invoice conflicts, null invoice fill, trusted parcel/status preservation");
    }

    private async Task ConnectionIsolationAsync()
    {
        var connections = await fixture.ConnectionsAsync(2);
        var a = connections[0];
        var b = connections[1];
        await RecordAsync(a, MappingFixture.Command(a, "shared-order"), Ack(801));
        await RecordAsync(b, MappingFixture.Command(b, "shared-order"), Ack(802));
        var first = (await fixture.RowsAsync(a)).Single();
        var second = (await fixture.RowsAsync(b)).Single();
        Check(first.HyperSaleOrderId == 801 && second.HyperSaleOrderId == 802 && first.Id != second.Id,
            "The same external order on distinct connections must retain independent invoice receipts.");
        Check(first.ConnectionId == a.Id && second.ConnectionId == b.Id && first.ShopId == second.ShopId,
            "Connection identity must isolate receipts even inside one shop/tenant.");
        Console.WriteLine("PASS identical order IDs isolated across connections");
    }

    private async Task ConcurrentReceiptsAsync()
    {
        foreach (var conflict in new[] { false, true })
        {
            var connection = (await fixture.ConnectionsAsync())[0];
            var command = MappingFixture.Command(connection, "concurrent-order");
            using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(30));
            var start = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            var arrived = 0;
            async Task<string?> WriteAsync(long invoice)
            {
                await using var db = fixture.Open();
                if (Interlocked.Increment(ref arrived) == 2) start.TrySetResult();
                // Cancellation controls the real SQL operation; both writers are
                // joined before their contexts or the fixture are disposed.
                await start.Task.WaitAsync(timeout.Token);
                try
                {
                    await new IntegrationOrderMappingReceipt(db).RecordAsync(connection, command, Ack(invoice), timeout.Token);
                    return null;
                }
                catch (IntegrationProviderException error) when (!error.Retryable) { return error.Code; }
            }
            var results = await Task.WhenAll(WriteAsync(901), WriteAsync(conflict ? 902 : 901));
            var rows = await fixture.RowsAsync(connection);
            Check(rows.Length == 1, "Concurrent receipt attempts must converge on one persisted row.");
            if (!conflict)
            {
                Check(results.All(x => x is null) && rows[0].HyperSaleOrderId == 901,
                    "Concurrent identical ACKs must both succeed with the same invoice.");
            }
            else
            {
                Check(results.Count(x => x is null) == 1 && results.Count(x => x == "OrderMappingInvoiceConflict") == 1,
                    "Concurrent different ACKs must yield one winner and one invoice conflict.");
                Check(rows[0].HyperSaleOrderId is 901 or 902, "Conflicting ACK winner must keep its acknowledged invoice.");
                var loser = rows[0].HyperSaleOrderId == 901 ? 902 : 901;
                await ExpectCodeAsync(() => RecordAsync(connection, command, Ack(loser, BusinessCommandStatus.Duplicate)),
                    "OrderMappingInvoiceConflict");
                Check(SameReceipt(rows[0], (await fixture.RowsAsync(connection)).Single()),
                    "Rejected conflicting Duplicate must never overwrite the winning receipt.");
            }
        }
        Console.WriteLine("PASS simultaneous same/conflicting ACK convergence without winner overwrite");
    }

    private async Task TrackedChangesAsync()
    {
        var connection = (await fixture.ConnectionsAsync())[0];
        await using (var db = fixture.Open())
        {
            var tracked = await db.ExternalIntegrationConnections.SingleAsync(x => x.Id == connection.Id);
            tracked.DisplayName = "Unrelated unsaved change";
            var unrelated = new IntegrationCustomerMapping
            {
                ShopId = connection.ShopId, TenantId = connection.TenantId,
                ExternalCustomerId = "unrelated-unsaved-buyer", PersonId = 92
            };
            db.IntegrationCustomerMappings.Add(unrelated);
            await new IntegrationOrderMappingReceipt(db).RecordAsync(connection, MappingFixture.Command(connection, "tracked-state"), Ack(1001), default);
            Check(db.Entry(tracked).State == EntityState.Modified && db.Entry(unrelated).State == EntityState.Added,
                "Receipt persistence must leave unrelated tracked changes pending.");
        }
        await using (var verify = fixture.Open())
        {
            var stored = await verify.ExternalIntegrationConnections.AsNoTracking().SingleAsync(x => x.Id == connection.Id);
            Check(stored.DisplayName == "Order mapping SQL fixture", "Receipt must not flush unrelated tracked updates.");
            Check(!await verify.IntegrationCustomerMappings.AnyAsync(x => x.ExternalCustomerId == "unrelated-unsaved-buyer"),
                "Receipt must not flush unrelated tracked inserts.");
            Check(await verify.ExternalOrderMappings.AnyAsync(x => x.ConnectionId == connection.Id && x.HyperSaleOrderId == 1001),
                "Receipt itself must commit independently of unrelated tracked state.");
        }
        Console.WriteLine("PASS unrelated tracked updates/inserts not flushed by receipt persistence");
    }

    private static bool SameReceipt(ExternalOrderMapping a, ExternalOrderMapping b) =>
        a.Id == b.Id && a.ConnectionId == b.ConnectionId && a.ShopId == b.ShopId
        && a.ExternalOrderId == b.ExternalOrderId && a.HyperSaleOrderId == b.HyperSaleOrderId
        && a.ExternalParcelId == b.ExternalParcelId && a.Status == b.Status && a.LastSyncAtUtc == b.LastSyncAtUtc;
}
