using System.Text.Json;
using System.Text.RegularExpressions;
using Hyper.Infrastructure.Data.Repository.Hyper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

internal sealed class MappingFixture : IAsyncDisposable
{
    private readonly string masterConnection;
    private readonly DbContextOptions<HyperIntegrationContext> options;
    private bool created;
    private int nextShop = 12000;
    internal string DatabaseName { get; } = "HyperOrderMappingChecks_" + Guid.NewGuid().ToString("N");

    internal MappingFixture(string connection)
    {
        GuardName();
        var builder = new SqlConnectionStringBuilder(connection)
        {
            InitialCatalog = "master", Pooling = false, ConnectTimeout = 15
        };
        masterConnection = builder.ConnectionString;
        builder.InitialCatalog = DatabaseName;
        options = new DbContextOptionsBuilder<HyperIntegrationContext>()
            .UseSqlServer(builder.ConnectionString, sql => sql.CommandTimeout(20))
            .ReplaceService<IModelCustomizer, MappingFixtureSchema>().Options;
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
        GuardName();
        if (!created) throw new MappingCheckFailure("Fixture ownership not established.");
        var db = new HyperIntegrationContext(options);
        if (db.Database.GetDbConnection().Database != DatabaseName)
        {
            db.Dispose();
            throw new MappingCheckFailure("Fixture database guard failed.");
        }
        return db;
    }

    internal async Task<ExternalIntegrationConnection[]> ConnectionsAsync(int count = 1)
    {
        var shop = Interlocked.Increment(ref nextShop);
        var rows = Enumerable.Range(0, count).Select(index => new ExternalIntegrationConnection
        {
            ShopId = shop, TenantId = "mapping-fixture", Provider = IntegrationProvider.Basalam,
            CredentialType = IntegrationCredentialType.BearerToken, AccountIdentifier = $"account-{shop}-{index}",
            DisplayName = "Order mapping SQL fixture", IsEnabled = true, CredentialsJson = "{}"
        }).ToArray();
        await using var db = Open();
        db.ExternalIntegrationConnections.AddRange(rows);
        await db.SaveChangesAsync();
        foreach (var row in rows)
            db.ExternalProductMappings.Add(new()
            {
                ConnectionId = row.Id, ShopId = shop, HyperProductId = 41,
                ExternalProductId = "product-a", IsActive = true
            });
        db.IntegrationCustomerMappings.Add(new()
        {
            ShopId = shop, TenantId = rows[0].TenantId, ExternalCustomerId = "buyer-a", PersonId = 91
        });
        await db.SaveChangesAsync();
        return rows;
    }

    internal async Task<IntegrationScenarioJob> SaleAsync(ExternalIntegrationConnection connection,
        string order, string? parcel = null)
    {
        var eventId = "event-" + Guid.NewGuid().ToString("N");
        await using var db = Open();
        // Omit the property entirely when no parcel is supplied, as thin events do.
        var payload = new Dictionary<string, object>
        {
            ["externalOrderId"] = order, ["externalCustomerId"] = "buyer-a",
            ["paymentStatus"] = "Paid", ["totalAmount"] = 25m,
            ["lines"] = new[] { new { externalProductId = "product-a", quantity = 2.5m, unitPrice = 10m } }
        };
        if (parcel is not null) payload["externalParcelId"] = parcel;
        db.IntegrationWebhookInbox.Add(new()
        {
            ConnectionId = connection.Id, ExternalEventId = eventId, EventType = "order.vendor.created",
            PayloadJson = JsonSerializer.Serialize(payload), ReceivedAtUtc = DateTime.UtcNow
        });
        await db.SaveChangesAsync();
        return new()
        {
            ConnectionId = connection.Id, ShopId = connection.ShopId,
            TenantId = connection.TenantId, EventId = eventId, Item = IntegrationSyncItem.Sale
        };
    }

    internal static IntegrationVendorOrderCommand Command(ExternalIntegrationConnection connection,
        string order, string? parcel = null) => new("direct-event", connection.ShopId, connection.TenantId,
        connection.Id, order, parcel, "buyer-a", [new(41, "product-a", null, 2.5m, 10m)],
        25m, SalePaymentStatus.Paid, 91);

    internal async Task SeedMappingAsync(ExternalIntegrationConnection connection, string order,
        long? invoice, string? parcel = null, byte status = 7, int? shop = null)
    {
        await using var db = Open();
        db.ExternalOrderMappings.Add(new()
        {
            ConnectionId = connection.Id, ShopId = shop ?? connection.ShopId, ExternalOrderId = order,
            HyperSaleOrderId = invoice, ExternalParcelId = parcel, Status = status,
            LastSyncAtUtc = new DateTime(2020, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        });
        await db.SaveChangesAsync();
    }

    internal async Task<ExternalOrderMapping[]> RowsAsync(ExternalIntegrationConnection connection)
    {
        await using var db = Open();
        return await db.ExternalOrderMappings.AsNoTracking().Where(x => x.ConnectionId == connection.Id).ToArrayAsync();
    }

    internal async Task SqlAsync(string sql)
    {
        await using var db = Open();
        await db.Database.ExecuteSqlRawAsync(sql);
    }

    private void GuardName()
    {
        if (!Regex.IsMatch(DatabaseName, "\\AHyperOrderMappingChecks_[0-9a-f]{32}\\z", RegexOptions.CultureInvariant))
            throw new MappingCheckFailure("Unsafe generated fixture name.");
    }

    private async Task MasterAsync(string sql)
    {
        GuardName();
        await using var connection = new SqlConnection(masterConnection);
        await connection.OpenAsync();
        if (connection.Database != "master") throw new MappingCheckFailure("Master database guard failed.");
        await using var command = new SqlCommand(sql, connection) { CommandTimeout = 30 };
        await command.ExecuteNonQueryAsync();
    }

    public async ValueTask DisposeAsync()
    {
        if (!created) return;
        GuardName();
        await MasterAsync($"IF DB_ID(N'{DatabaseName}') IS NOT NULL BEGIN ALTER DATABASE [{DatabaseName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [{DatabaseName}]; END;");
        created = false;
        Console.WriteLine("Removed isolated SQL fixture " + DatabaseName);
    }
}

internal sealed class MappingFixtureSchema(ModelCustomizerDependencies dependencies) : ModelCustomizer(dependencies)
{
    public override void Customize(ModelBuilder builder, DbContext context)
    {
        base.Customize(builder, context);
        // Preserve actual columns, keys, collations and relationships. Only allow
        // EnsureCreated to create the deployed tables in the owned fixture.
        foreach (var entity in builder.Model.GetEntityTypes()) entity.SetIsTableExcludedFromMigrations(false);
    }
}

internal sealed class MappingCheckFailure(string message) : Exception(message);
internal sealed class UnexpectedDependency : Exception;
