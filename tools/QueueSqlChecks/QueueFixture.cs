using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Hyper.Infrastructure.Data.Repository.Hyper;
using Hyper.Infrastructure.Features.Integrations;
using Hyper.Integration.Contracts;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Options;
using Provider = Hyper.Integration.Domain.Entities.Integrations.IntegrationProvider;

internal sealed class QueueFixture : IAsyncDisposable
{
    private const string Secret = "queue-sql-checks-synthetic-webhook-secret";
    private readonly string masterConnection;
    private readonly string fixtureConnection;
    private readonly DbContextOptions<HyperIntegrationContext> options;
    private bool created;
    private int nextShop = 9000;
    internal string DatabaseName { get; } = "HyperQueueChecks_" + Guid.NewGuid().ToString("N");
    internal ControlledOwner Owner { get; } = new();

    internal QueueFixture(string connection)
    {
        GuardName();
        var builder = new SqlConnectionStringBuilder(connection)
        {
            InitialCatalog = "master", Pooling = false, ConnectTimeout = 15
        };
        masterConnection = builder.ConnectionString;
        builder.InitialCatalog = DatabaseName;
        fixtureConnection = builder.ConnectionString;
        options = new DbContextOptionsBuilder<HyperIntegrationContext>()
            .UseSqlServer(fixtureConnection, sql => sql.CommandTimeout(20))
            .ReplaceService<IModelCustomizer, QueueFixtureSchema>().Options;
    }

    internal async Task InitializeAsync()
    {
        GuardName();
        Console.WriteLine("Creating isolated SQL fixture " + DatabaseName);
        try
        {
            await MasterAsync($"CREATE DATABASE [{DatabaseName}]");
            created = true;
        }
        catch
        {
            Console.Error.WriteLine("CREATE was not acknowledged; automatic cleanup cannot prove ownership. Inspect possible fixture " + DatabaseName);
            throw;
        }
        await using var db = Open();
        await db.Database.EnsureCreatedAsync();
    }

    internal HyperIntegrationContext Open()
    {
        var db = new HyperIntegrationContext(options);
        if (db.Database.GetDbConnection().Database != DatabaseName)
        {
            db.Dispose();
            throw new QueueCheckFailure("Fixture database guard failed.");
        }
        return db;
    }

    internal IntegrationScenarioQueue Queue(HyperIntegrationContext db)
    {
        var unavailable = new ForbiddenDependencies();
        var dispatcher = new IntegrationBusinessEventDispatcher(unavailable, Owner, unavailable, db, unavailable);
        var demo = new BasalamDemoProvisioner(db, unavailable, unavailable);
        var processor = new IntegrationScenarioProcessor(db, unavailable, unavailable,
            Options.Create(new IntegrationInventoryCaptureOptions()), Options.Create(new BasalamOAuthSettings()),
            demo, dispatcher, unavailable);
        return new(db, processor);
    }

    internal async Task<ExternalIntegrationConnection[]> ConnectionsAsync(int count = 1)
    {
        var shop = Interlocked.Increment(ref nextShop);
        var rows = Enumerable.Range(0, count).Select(index => new ExternalIntegrationConnection
        {
            ShopId = shop, TenantId = "queue-fixture", Provider = Provider.Basalam,
            CredentialType = IntegrationCredentialType.BearerToken, AccountIdentifier = $"{shop}{index}",
            DisplayName = "Queue SQL fixture", IsEnabled = true,
            CredentialsJson = JsonSerializer.Serialize(new { webhookSecret = Secret })
        }).ToArray();
        await using var db = Open();
        db.ExternalIntegrationConnections.AddRange(rows);
        await db.SaveChangesAsync();
        return rows;
    }

    internal static byte[] Body(string eventId, int rating = 4) => Encoding.UTF8.GetBytes(
        JsonSerializer.Serialize(new { externalReviewId = eventId, rating, text = "synthetic queue fixture" }));

    internal async Task<WebhookIngressResult> ReceiveAsync(ExternalIntegrationConnection connection,
        string eventId, int rating = 4)
    {
        await using var db = Open();
        var ingress = new IntegrationWebhookIngress(db, new IntegrationWebhookVerifier());
        return await ingress.ReceiveAsync(new(Hyper.Integration.Contracts.IntegrationProvider.Basalam,
            connection.AccountIdentifier, eventId, "review.created", null, null, Body(eventId, rating),
            Authorization: "Bearer " + Secret));
    }

    internal async Task<long> EventAsync(ExternalIntegrationConnection connection, string eventId)
    {
        var result = await ReceiveAsync(connection, eventId);
        if (result.Status != WebhookIngressStatus.Accepted || result.ScenarioJobId is not { } id)
            throw new QueueCheckFailure("Fixture ingress did not accept the unique review.");
        return id;
    }

    internal async Task<long> EnqueueAsync(ExternalIntegrationConnection connection, string eventId,
        IntegrationSyncItem item = IntegrationSyncItem.Review)
    {
        await using var db = Open();
        return await Queue(db).EnqueueAsync(new(connection.ShopId, connection.TenantId), connection.Id,
            new(eventId, item, IntegrationSyncTrigger.BoothChanged), default);
    }

    internal async Task AddInboxAsync(ExternalIntegrationConnection connection, string eventId)
    {
        await using var db = Open();
        db.IntegrationWebhookInbox.Add(new()
        {
            ConnectionId = connection.Id, ExternalEventId = eventId, EventType = "review.created",
            PayloadJson = Encoding.UTF8.GetString(Body(eventId)), ReceivedAtUtc = DateTime.UtcNow
        });
        await db.SaveChangesAsync();
    }

    internal async Task<bool> ProcessAsync(ExternalIntegrationConnection connection, CancellationToken ct = default)
    {
        // Cancel the operation itself, not merely its caller's await. Each caller
        // must await this task through disposal before the fixture can be removed.
        using var operation = CancellationTokenSource.CreateLinkedTokenSource(ct);
        operation.CancelAfter(TimeSpan.FromSeconds(30));
        await using var db = Open();
        return await Queue(db).ProcessConnectionAsync(connection.Id, operation.Token);
    }

    internal async Task<IntegrationScenarioJob> JobAsync(long id)
    {
        await using var db = Open();
        return await db.IntegrationScenarioJobs.AsNoTracking().SingleAsync(x => x.Id == id);
    }

    internal async Task<IntegrationWebhookInbox> InboxAsync(ExternalIntegrationConnection connection, string eventId)
    {
        await using var db = Open();
        return await db.IntegrationWebhookInbox.AsNoTracking()
            .SingleAsync(x => x.ConnectionId == connection.Id && x.ExternalEventId == eventId);
    }

    internal async Task DueAsync(long id)
    {
        await using var db = Open();
        await db.IntegrationScenarioJobs.Where(x => x.Id == id).ExecuteUpdateAsync(set => set
            .SetProperty(x => x.NextAttemptAtUtc, DateTime.UtcNow.AddMinutes(-1)));
    }

    internal async Task ExpireAsync(long id)
    {
        await using var db = Open();
        await db.IntegrationScenarioJobs.Where(x => x.Id == id).ExecuteUpdateAsync(set => set
            .SetProperty(x => x.LeaseExpiresAtUtc, DateTime.UtcNow.AddMinutes(-1)));
    }

    internal async Task FixtureSqlAsync(string sql)
    {
        await using var db = Open();
        await db.Database.ExecuteSqlRawAsync(sql);
    }

    private void GuardName()
    {
        if (!Regex.IsMatch(DatabaseName, "\\AHyperQueueChecks_[0-9a-f]{32}\\z", RegexOptions.CultureInvariant))
            throw new QueueCheckFailure("Unsafe generated fixture name.");
    }

    private async Task MasterAsync(string sql)
    {
        GuardName();
        await using var connection = new SqlConnection(masterConnection);
        await connection.OpenAsync();
        if (connection.Database != "master") throw new QueueCheckFailure("Master database guard failed.");
        await using var command = new SqlCommand(sql, connection) { CommandTimeout = 30 };
        await command.ExecuteNonQueryAsync();
    }

    public async ValueTask DisposeAsync()
    {
        if (!created) return;
        GuardName();
        // Only the exact GUID fixture generated in this process is ever removed.
        // Pooling is disabled. ROLLBACK IMMEDIATE affects this fixture alone.
        await MasterAsync($"IF DB_ID(N'{DatabaseName}') IS NOT NULL BEGIN ALTER DATABASE [{DatabaseName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [{DatabaseName}]; END;");
        Console.WriteLine("Removed isolated SQL fixture " + DatabaseName);
    }
}

internal sealed class QueueFixtureSchema(ModelCustomizerDependencies dependencies) : ModelCustomizer(dependencies)
{
    public override void Customize(ModelBuilder builder, DbContext context)
    {
        base.Customize(builder, context);
        // Production mappings exclude deployed tables from migrations. The fixture
        // creates that exact EF model; it does not rehearse production migrations.
        foreach (var entity in builder.Model.GetEntityTypes()) entity.SetIsTableExcludedFromMigrations(false);
    }
}

internal sealed class ControlledOwner : IIntegrationEngagementPort
{
    private readonly ConcurrentDictionary<(long, string), byte> effects = new();
    private readonly ConcurrentDictionary<(long, string), int> calls = new();
    internal ConcurrentQueue<IntegrationReviewCommand> Received { get; } = new();
    internal ConcurrentDictionary<string, Func<IntegrationReviewCommand, CancellationToken, Task<EngagementCommandResult>>> Behavior { get; } = new();

    internal int Calls(ExternalIntegrationConnection connection, string eventId) => calls.GetValueOrDefault((connection.Id, eventId));
    internal int Effects(ExternalIntegrationConnection connection, string eventId) => effects.ContainsKey((connection.Id, eventId)) ? 1 : 0;
    internal EngagementCommandResult Acknowledge(IntegrationReviewCommand command) => new(
        effects.TryAdd((command.ConnectionId, command.EventId), 0) ? EngagementCommandStatus.Applied : EngagementCommandStatus.Duplicate);

    public Task<EngagementCommandResult> ApplyReviewAsync(IntegrationReviewCommand command, CancellationToken ct)
    {
        calls.AddOrUpdate((command.ConnectionId, command.EventId), 1, (_, count) => count + 1);
        Received.Enqueue(command);
        return Behavior.TryGetValue(command.EventId, out var action)
            ? action(command, ct) : Task.FromResult(Acknowledge(command));
    }
    public Task<EngagementCommandResult> ApplySubscriptionAsync(IntegrationSubscriptionCommand command, CancellationToken ct) => throw new UnexpectedDependency();
    public Task<EngagementCommandResult> ApplyChatMessageAsync(IntegrationChatMessageCommand command, CancellationToken ct) => throw new UnexpectedDependency();
}

// This exception is deliberately outside the queue's handled provider/validation
// exception types, so an unrelated path fails the executable instead of becoming
// an apparently expected DeadLetter result.
internal sealed class UnexpectedDependency : Exception;
internal sealed class QueueCheckFailure(string message) : Exception(message);

internal sealed class ForbiddenDependencies : IIntegrationStrategyResolver, IIntegrationInventoryCapture,
    IIntegrationInventoryReservation, IIntegrationBusinessCommandPort, IIntegrationPlatformCatalogPort
{
    public IExternalIntegrationAdapter Resolve(Provider provider, IntegrationCredentialType credentialType) => throw new UnexpectedDependency();
    public Task<int> CaptureAsync(CancellationToken ct) => throw new UnexpectedDependency();
    public Task<bool> ReconcileOneAsync(long connectionId, long mappingId, CancellationToken ct) => throw new UnexpectedDependency();
    public Task ReserveAsync(OwnedIntegrationShop shop, string key, IReadOnlyCollection<IntegrationOrderLineCommand> lines, CancellationToken ct) => throw new UnexpectedDependency();
    public Task ReleaseAsync(OwnedIntegrationShop shop, string key, CancellationToken ct) => throw new UnexpectedDependency();
    public Task CommitAsync(OwnedIntegrationShop shop, string key, CancellationToken ct) => throw new UnexpectedDependency();
    public Task<BusinessCommandResult> ApplyCounterpartyAsync(IntegrationCounterpartyCommand command, CancellationToken ct) => throw new UnexpectedDependency();
    public Task<BusinessCommandResult> ApplyVendorOrderAsync(IntegrationVendorOrderCommand command, CancellationToken ct) => throw new UnexpectedDependency();
    public Task<BusinessCommandResult> ApplyCustomerOrderAsync(IntegrationCustomerOrderCommand command, CancellationToken ct) => throw new UnexpectedDependency();
    public Task<BusinessCommandResult> CancelOrderAsync(IntegrationOrderCancellationCommand command, CancellationToken ct) => throw new UnexpectedDependency();
    public Task<BusinessCommandResult> ApplyParcelStatusAsync(IntegrationParcelStatusCommand command, CancellationToken ct) => throw new UnexpectedDependency();
    public Task<BusinessCommandResult> ApplyExternalProductChangedAsync(IntegrationExternalProductChangedCommand command, CancellationToken ct) => throw new UnexpectedDependency();
    public Task<IReadOnlyList<IntegrationPlatformProduct>> GetProductsAsync(int shopId, string tenantId, CancellationToken ct) => throw new UnexpectedDependency();
}
