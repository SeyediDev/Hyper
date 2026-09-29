using Hyper.Infrastructure.Data.Repository.Hyper;
using Hyper.Infrastructure.Features.Integrations;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

// An explicit single-row maintenance entry point. No hosts, automatic migration,
// database creation, provider requests or command-line secret values.
if (args.Length != 9 || args[0] != "--apply" || args[1] != "--database"
    || args[3] != "--connection-id" || args[5] != "--shop-id" || args[7] != "--tenant"
    || string.IsNullOrWhiteSpace(args[2]) || !long.TryParse(args[4], out var connectionId) || connectionId <= 0
    || !int.TryParse(args[6], out var shopId) || shopId <= 0 || string.IsNullOrWhiteSpace(args[8])
    || args[8].Length > 30 || args[8] != args[8].Trim() || args[8].Any(char.IsControl))
{
    Console.Error.WriteLine("No migration run. Required: --apply --database NAME --connection-id ID --shop-id ID --tenant TENANT; privately supply INTEGRATION_CREDENTIAL_MIGRATION_SQL and IntegrationProtection environment settings.");
    return 2;
}
var supplied = Environment.GetEnvironmentVariable("INTEGRATION_CREDENTIAL_MIGRATION_SQL");
if (string.IsNullOrWhiteSpace(supplied))
{
    Console.Error.WriteLine("No migration run: INTEGRATION_CREDENTIAL_MIGRATION_SQL is missing.");
    return 2;
}
try
{
    var connection = new SqlConnectionStringBuilder(supplied) { Pooling = false, ConnectTimeout = 15 };
    if (!string.Equals(connection.InitialCatalog, args[2], StringComparison.Ordinal)
        || new[] { "master", "model", "msdb", "tempdb" }.Contains(connection.InitialCatalog, StringComparer.OrdinalIgnoreCase))
        throw new IntegrationCredentialException("MigrationDatabaseMismatch");
    var configuration = new ConfigurationBuilder().AddEnvironmentVariables().Build();
    var protectionOptions = new IntegrationProtectionOptions();
    configuration.GetSection("IntegrationProtection").Bind(protectionOptions);
    var configured = Options.Create(protectionOptions);
    var vault = new IntegrationCredentialVault(new IntegrationProtectionProvider(configured), configured);
    var options = new DbContextOptionsBuilder<HyperIntegrationContext>()
        .UseSqlServer(connection.ConnectionString, sql => sql.CommandTimeout(20)).Options;
    await using var db = new HyperIntegrationContext(options);
    if (!string.Equals(db.Database.GetDbConnection().Database, args[2], StringComparison.Ordinal))
        throw new IntegrationCredentialException("MigrationDatabaseMismatch");
    using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(60));
    var migration = new IntegrationCredentialMigration(db, vault);
    var plan = await migration.PrepareAsync(new(shopId, args[8]), connectionId, timeout.Token);
    var result = await migration.ApplyAsync(plan, timeout.Token);
    Console.WriteLine($"ConnectionId={plan.ConnectionId}; Result={result}");
    return result == IntegrationCredentialMigrationResult.Conflict ? 3 : 0;
}
catch (Exception error)
{
    // No connection strings, ciphertext, documents, server messages or inner errors.
    Console.Error.WriteLine(error switch
    {
        IntegrationCredentialException credential => "Migration failed: " + credential.Code,
        SqlException sql => $"Migration failed: SqlException Number={sql.Number}",
        _ => "Migration failed: " + error.GetType().Name
    });
    return 1;
}
