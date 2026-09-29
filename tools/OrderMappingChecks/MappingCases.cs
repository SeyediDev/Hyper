using Hyper.Infrastructure.Data.Repository.Hyper;
using Hyper.Infrastructure.Features.Integrations;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

internal sealed partial class MappingCases(MappingFixture fixture)
{
    internal int Checks { get; private set; }

    internal async Task RunAsync()
    {
        await AcknowledgementsAsync();
        await UnacknowledgedAsync();
        await ReceiptFailureReplayAsync();
        await PreflightConflictsAsync();
        await ScopeRevalidationAsync();
        await ExistingReceiptsAsync();
        await ConnectionIsolationAsync();
        await ConcurrentReceiptsAsync();
        await TrackedChangesAsync();
    }

    private void Check(bool condition, string message)
    {
        if (!condition) throw new MappingCheckFailure(message);
        Checks++;
    }

    private async Task ExpectCodeAsync(Func<Task> action, string code)
    {
        try { await action(); }
        catch (IntegrationProviderException error)
        {
            Check(error.Code == code && !error.Retryable, "Expected nonretryable " + code);
            return;
        }
        throw new MappingCheckFailure("Missing expected " + code);
    }

    private static IntegrationBusinessEventDispatcher Dispatcher(HyperIntegrationContext db,
        CommandProbe commands, ReservationProbe reservations) =>
        new(commands, new ForbiddenEngagement(), reservations, db);

    private async Task DispatchAsync(ExternalIntegrationConnection connection, IntegrationScenarioJob job,
        CommandProbe commands, ReservationProbe reservations)
    {
        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(30));
        await using var db = fixture.Open();
        await Dispatcher(db, commands, reservations).DispatchAsync(job, connection, timeout.Token);
    }

    private async Task RecordAsync(ExternalIntegrationConnection connection, IntegrationVendorOrderCommand command,
        BusinessCommandResult result, CancellationToken ct = default)
    {
        using var timeout = CancellationTokenSource.CreateLinkedTokenSource(ct);
        timeout.CancelAfter(TimeSpan.FromSeconds(30));
        await using var db = fixture.Open();
        await new IntegrationOrderMappingReceipt(db).RecordAsync(connection, command, result, timeout.Token);
    }

    private static BusinessCommandResult Ack(long invoice, BusinessCommandStatus status = BusinessCommandStatus.Applied,
        bool stockCommitted = false) => new(status, invoice.ToString(System.Globalization.CultureInfo.InvariantCulture), StockCommitted: stockCommitted);

    private async Task AcknowledgementsAsync()
    {
        foreach (var status in new[] { BusinessCommandStatus.Applied, BusinessCommandStatus.Duplicate })
        {
            var connection = (await fixture.ConnectionsAsync())[0];
            var job = await fixture.SaleAsync(connection, "numeric-ack", "untrusted-raw-parcel");
            var reservations = new ReservationProbe();
            await using var db = fixture.Open();
            var commands = new CommandProbe
            {
                Behavior = (command, _) =>
                {
                    Check(db.Database.CurrentTransaction is null, "Accounting call must have no Integration SQL transaction.");
                    Check(reservations.Held.Count == 1, "Order hold must exist before accounting call.");
                    Check(command.AccountingCustomerId == 91 && command.Lines.Single().HyperProductId == 41,
                        "Dispatcher must resolve real customer/product mappings.");
                    return Task.FromResult(Ack(201, status));
                }
            };
            using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(30));
            await Dispatcher(db, commands, reservations).DispatchAsync(job, connection, timeout.Token);
            var rows = await fixture.RowsAsync(connection);
            Check(rows.Length == 1 && rows[0].HyperSaleOrderId == 201 && rows[0].ExternalOrderId == "numeric-ack",
                "Applied and Duplicate ACKs must persist their numeric invoice.");
            Check(rows[0].ShopId == connection.ShopId && rows[0].Status == 0 && rows[0].LastSyncAtUtc > DateTime.UtcNow.AddMinutes(-5),
                "New receipt must have correct shop, initial status and timestamp.");
            Check(rows[0].ExternalParcelId is null, "Raw event parcel must never establish trusted parcel binding.");
            Check(commands.Calls == 1 && reservations.ReserveCalls == 1 && reservations.CommitCalls == 0 && reservations.ReleaseCalls == 0,
                "Non-stock ACK must retain its hold without commit or release.");
            var receiptId = rows[0].Id;
            await DispatchAsync(connection, job, new() { Behavior = (_, _) => Task.FromResult(Ack(201, BusinessCommandStatus.Duplicate)) }, reservations);
            rows = await fixture.RowsAsync(connection);
            Check(rows.Length == 1 && rows[0].Id == receiptId, "Duplicate replay must reuse the one persisted receipt.");
        }
        Console.WriteLine("PASS numeric Applied/Duplicate receipts, raw parcel, transaction boundary, replay");
    }

    private async Task UnacknowledgedAsync()
    {
        foreach (var status in new[] { BusinessCommandStatus.PendingDependency, BusinessCommandStatus.Rejected })
        {
            var connection = (await fixture.ConnectionsAsync())[0];
            var job = await fixture.SaleAsync(connection, "no-ack");
            var reservations = new ReservationProbe();
            var commands = new CommandProbe { Behavior = (_, _) => Task.FromResult(new BusinessCommandResult(status, "302", "ControlledReject", true)) };
            if (status == BusinessCommandStatus.Rejected)
                await ExpectCodeAsync(() => DispatchAsync(connection, job, commands, reservations), "ControlledReject");
            else
                await DispatchAsync(connection, job, commands, reservations);
            Check((await fixture.RowsAsync(connection)).Length == 0, "Unacknowledged result must not persist a receipt.");
            Check(commands.Calls == 1 && reservations.Held.Count == 1 && reservations.CommitCalls == 0 && reservations.ReleaseCalls == 0,
                "Pending/rejected result must retain the hold even if stock flag is set.");
        }
        {
            var connection = (await fixture.ConnectionsAsync())[0];
            var job = await fixture.SaleAsync(connection, "timeout");
            var reservations = new ReservationProbe();
            var commands = new CommandProbe { Behavior = (_, _) => throw new TimeoutException() };
            var timedOut = false;
            try { await DispatchAsync(connection, job, commands, reservations); }
            catch (TimeoutException) { timedOut = true; }
            Check(timedOut && commands.Calls == 1, "Controlled remote timeout must propagate.");
            Check((await fixture.RowsAsync(connection)).Length == 0 && reservations.Held.Count == 1
                && reservations.CommitCalls == 0 && reservations.ReleaseCalls == 0,
                "Ambiguous remote timeout must leave no receipt and retain the hold.");
        }
        foreach (var reference in new string?[] { null, "", "invoice-x", "0", "-1", "01", "+1", " 1", "1 ", "1.0", "9223372036854775808" })
        {
            var connection = (await fixture.ConnectionsAsync())[0];
            var job = await fixture.SaleAsync(connection, "invalid-reference");
            var reservations = new ReservationProbe();
            var commands = new CommandProbe { Behavior = (_, _) => Task.FromResult(new BusinessCommandResult(BusinessCommandStatus.Applied, reference)) };
            await ExpectCodeAsync(() => DispatchAsync(connection, job, commands, reservations), "AccountingInvoiceReferenceInvalid");
            Check((await fixture.RowsAsync(connection)).Length == 0, "Malformed invoice ACK must not create a receipt.");
            Check(commands.Calls == 1 && reservations.Held.Count == 1 && reservations.ReleaseCalls == 0,
                "Malformed ACK must not cause remote compensation.");
        }
        Console.WriteLine("PASS pending/rejected/timeout/malformed ACKs preserve receipt absence");
    }

    private async Task ReceiptFailureReplayAsync()
    {
        var connection = (await fixture.ConnectionsAsync())[0];
        var job = await fixture.SaleAsync(connection, "receipt-failure");
        var reservations = new ReservationProbe();
        var commands = new CommandProbe
        {
            Behavior = (_, _) =>
            {
                reservations.Steps.Enqueue("accounting");
                return Task.FromResult(Ack(401, BusinessCommandStatus.Applied, true));
            }
        };
        await fixture.SqlAsync("""
            CREATE TRIGGER dbo.OrderMappingChecks_ReceiptFailure ON dbo.ExternalOrderMappings
            AFTER INSERT, UPDATE AS
            BEGIN
                SET NOCOUNT ON;
                IF EXISTS (SELECT 1 FROM inserted WHERE ExternalOrderId = N'receipt-failure')
                    THROW 51021, 'Controlled receipt failure', 1;
            END
            """);
        try
        {
            var failed = false;
            try { await DispatchAsync(connection, job, commands, reservations); }
            catch (SqlException error) when (error.Errors.Cast<SqlError>().Any(x => x.Number == 51021)) { failed = true; }
            Check(failed, "Injected SQL receipt failure must reach the caller.");
            Check(reservations.Steps.ToArray().SequenceEqual(new[] { "reserve", "accounting", "commit" }),
                "Stock ACK must consume its hold before local receipt failure.");
            Check(reservations.Held.Count == 0 && reservations.Committed.Count == 1 && reservations.ReleaseCalls == 0,
                "Consumed stock hold must never be released as compensation.");
            Check((await fixture.RowsAsync(connection)).Length == 0, "Failed receipt transaction must roll back.");
        }
        finally { await fixture.SqlAsync("DROP TRIGGER dbo.OrderMappingChecks_ReceiptFailure"); }
        commands.Behavior = (_, _) => Task.FromResult(Ack(401, BusinessCommandStatus.Duplicate, true));
        await DispatchAsync(connection, job, commands, reservations); // Always opens a fresh DbContext.
        var rows = await fixture.RowsAsync(connection);
        Check(rows.Length == 1 && rows[0].HyperSaleOrderId == 401, "Fresh-context Duplicate must repair missing receipt.");
        Check(commands.Calls == 2 && reservations.Held.Count == 0 && reservations.Committed.Count == 1
            && reservations.CommitCalls == 2 && reservations.ReleaseCalls == 0,
            "Repair replay must acknowledge one probe effect with no cancellation or release.");
        Console.WriteLine("PASS stock consumption before SQL failure and fresh-context Duplicate repair");
    }
}
