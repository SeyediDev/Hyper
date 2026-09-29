using System.Text.RegularExpressions;
using Hyper.Infrastructure.Data.Repository.Hyper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

internal sealed class VaultSqlFixture : IAsyncDisposable
{
    private readonly string masterConnection;
    private readonly DbContextOptions<HyperIntegrationContext> options;
    private bool created;
    private int shop = 16000;
    internal string DatabaseName { get; } = "HyperCredentialChecks_" + Guid.NewGuid().ToString("N");
    internal VaultSqlFixture(string connection)
    {
        Guard();
        var builder = new SqlConnectionStringBuilder(connection)
        { InitialCatalog = "master", Pooling = false, ConnectTimeout = 15 };
        masterConnection = builder.ConnectionString;
        builder.InitialCatalog = DatabaseName;
        options = new DbContextOptionsBuilder<HyperIntegrationContext>()
            .UseSqlServer(builder.ConnectionString, sql => sql.CommandTimeout(20))
            .ReplaceService<IModelCustomizer, VaultSchema>().Options;
    }
    internal async Task InitializeAsync()
    {
        Guard();
        Console.WriteLine("Creating isolated SQL fixture " + DatabaseName);
        try { await MasterAsync($"CREATE DATABASE [{DatabaseName}]"); created = true; }
        catch
        {
            Console.Error.WriteLine("CREATE not acknowledged; automatic cleanup cannot prove ownership of " + DatabaseName);
            throw;
        }
        await using var db = Open();
        await db.Database.EnsureCreatedAsync();
    }
    internal HyperIntegrationContext Open()
    {
        Guard();
        if (!created) throw new VaultCheckFailure("Fixture ownership not established.");
        var db = new HyperIntegrationContext(options);
        if (db.Database.GetDbConnection().Database != DatabaseName)
        { db.Dispose(); throw new VaultCheckFailure("Fixture database guard failed."); }
        return db;
    }
    internal int NextShop() => Interlocked.Increment(ref shop);
    internal async Task<ExternalIntegrationConnection> SeedAsync(string credentials)
    {
        var row = PureChecks.Connection();
        row.Id = 0; row.ShopId = NextShop(); row.CredentialsJson = credentials;
        await using var db = Open();
        db.Add(row); await db.SaveChangesAsync(); return row;
    }
    internal async Task<ExternalIntegrationConnection> ReadAsync(long id)
    {
        await using var db = Open();
        return await db.ExternalIntegrationConnections.AsNoTracking().SingleAsync(c => c.Id == id);
    }
    internal async Task SetDocumentAsync(long id, string json)
    {
        await using var db = Open();
        await db.ExternalIntegrationConnections.Where(c => c.Id == id).ExecuteUpdateAsync(set => set.SetProperty(c => c.CredentialsJson, json));
    }
    private void Guard()
    {
        if (!Regex.IsMatch(DatabaseName, "\\AHyperCredentialChecks_[0-9a-f]{32}\\z", RegexOptions.CultureInvariant))
            throw new VaultCheckFailure("Unsafe generated SQL fixture name.");
    }
    private async Task MasterAsync(string sql)
    {
        Guard();
        await using var connection = new SqlConnection(masterConnection);
        await connection.OpenAsync();
        if (connection.Database != "master") throw new VaultCheckFailure("Master database guard failed.");
        await using var command = new SqlCommand(sql, connection) { CommandTimeout = 30 };
        await command.ExecuteNonQueryAsync();
    }
    public async ValueTask DisposeAsync()
    {
        if (!created) return;
        Guard();
        await MasterAsync($"IF DB_ID(N'{DatabaseName}') IS NOT NULL BEGIN ALTER DATABASE [{DatabaseName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [{DatabaseName}]; END;");
        created = false;
        Console.WriteLine("Removed isolated SQL fixture " + DatabaseName);
    }
}

internal sealed class VaultSchema(ModelCustomizerDependencies dependencies) : ModelCustomizer(dependencies)
{
    public override void Customize(ModelBuilder builder, DbContext context)
    {
        base.Customize(builder, context);
        // Only opt the production Integration model into fixture DDL; preserve
        // the actual columns, collations, relationships and constraints.
        foreach (var entity in builder.Model.GetEntityTypes()) entity.SetIsTableExcludedFromMigrations(false);
    }
}
