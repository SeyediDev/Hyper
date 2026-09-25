using System.Text.Json;
using Microsoft.Data.SqlClient;

var settingsPath = GetOption("--settings")
    ?? Path.Combine(AppContext.BaseDirectory, "appsettings.json");
var scriptPath = GetOption("--script")
    ?? Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "docs", "schema", "ensure-integration-database.sql"));

var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__IntegrationConnection")
    ?? ReadConnectionString(settingsPath)
    ?? throw new InvalidOperationException("IntegrationConnection was not found in settings or environment.");

var builder = new SqlConnectionStringBuilder(connectionString)
{
    TrustServerCertificate = true,
    Encrypt = false
};
if (string.IsNullOrWhiteSpace(builder.InitialCatalog))
    builder.InitialCatalog = "HyperyekIntegration";
var database = builder.InitialCatalog;

var master = new SqlConnectionStringBuilder(builder.ConnectionString) { InitialCatalog = "master" };
await using (var masterConnection = new SqlConnection(master.ConnectionString))
{
    await masterConnection.OpenAsync();
    await using var command = masterConnection.CreateCommand();
    command.CommandText = "IF DB_ID(@database) IS NULL BEGIN DECLARE @sql nvarchar(512) = N'CREATE DATABASE ' + QUOTENAME(@database); EXEC(@sql); END;";
    command.Parameters.AddWithValue("@database", database);
    await command.ExecuteNonQueryAsync();
}

var script = await File.ReadAllTextAsync(scriptPath);
await using (var connection = new SqlConnection(builder.ConnectionString))
{
    await connection.OpenAsync();
    await using var command = connection.CreateCommand();
    command.CommandTimeout = 180;
    command.CommandText = script;
    await command.ExecuteNonQueryAsync();

    var expected = new[]
    {
        "ExternalIntegrationConnections", "ExternalOAuthTokens", "IntegrationCustomerMappings",
        "IntegrationScenarioJobs", "ExternalProductMappings", "ExternalOrderMappings",
        "IntegrationSyncRuns", "IntegrationWebhookInbox", "IntegrationOutbox",
        "IntegrationMerchantAccess", "IntegrationAdminSimulations", "IntegrationTokenRequests",
        "IntegrationEventAudits", "InventoryReservationLogs"
    };
    await using var verify = connection.CreateCommand();
    verify.CommandText = "SELECT name FROM sys.tables WHERE schema_id = SCHEMA_ID(N'dbo')";
    await using var reader = await verify.ExecuteReaderAsync();
    var actual = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
    while (await reader.ReadAsync()) actual.Add(reader.GetString(0));
    var missing = expected.Where(x => !actual.Contains(x)).ToArray();
    if (missing.Length != 0)
        throw new InvalidOperationException($"Schema verification failed. Missing: {string.Join(", ", missing)}");
    Console.WriteLine($"Integration database '{database}' is ready.");
    Console.WriteLine($"Verified {expected.Length} Integration tables; existing data was preserved.");
}

string? GetOption(string name)
{
    var index = Array.IndexOf(args, name);
    return index >= 0 && index + 1 < args.Length ? args[index + 1] : null;
}

static string? ReadConnectionString(string path)
{
    if (!File.Exists(path)) return null;
    using var document = JsonDocument.Parse(File.ReadAllText(path));
    return document.RootElement.TryGetProperty("ConnectionStrings", out var section)
        && section.TryGetProperty("IntegrationConnection", out var value)
        ? value.GetString()
        : null;
}
