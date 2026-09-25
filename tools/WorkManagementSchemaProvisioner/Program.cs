using Microsoft.Data.SqlClient;
using System.Text.RegularExpressions;

var input = Environment.GetEnvironmentVariable("ConnectionStrings__WorkManagement")
    ?? throw new InvalidOperationException("ConnectionStrings__WorkManagement is required.");
var cs = new SqlConnectionStringBuilder(input) { TrustServerCertificate = true, Encrypt = false };
if (string.IsNullOrWhiteSpace(cs.InitialCatalog)) cs.InitialCatalog = "WorkManagement";
var database = cs.InitialCatalog;
var master = new SqlConnectionStringBuilder(cs.ConnectionString) { InitialCatalog = "master" };
await using (var c = new SqlConnection(master.ConnectionString))
{
    await c.OpenAsync();
    await using var cmd = c.CreateCommand();
    cmd.CommandText = "IF DB_ID(@name) IS NULL BEGIN DECLARE @sql nvarchar(512)=N'CREATE DATABASE '+QUOTENAME(@name); EXEC(@sql); END";
    cmd.Parameters.AddWithValue("@name", database);
    await cmd.ExecuteNonQueryAsync();
}
var script = await File.ReadAllTextAsync(Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..", "docs", "schema", "ensure-work-management-database.sql")));
await using (var c = new SqlConnection(cs.ConnectionString))
{
    await c.OpenAsync();
    foreach (var batch in Regex.Split(script, @"^\s*GO\s*$", RegexOptions.Multiline | RegexOptions.IgnoreCase)
        .Where(x => !string.IsNullOrWhiteSpace(x)))
    {
        await using var batchCommand = c.CreateCommand(); batchCommand.CommandText = batch; batchCommand.CommandTimeout = 180; await batchCommand.ExecuteNonQueryAsync();
    }
    await using var cmd = c.CreateCommand();
    cmd.CommandText = "SELECT COUNT(*) FROM sys.tables WHERE name IN ('Projects','WorkItems','WorkRoles','WorkItemLogs','ChatWorkIntakes','WorkItemDependencies','WorkItemCommits','WorkItemTestEvidence')";
    var count = Convert.ToInt32(await cmd.ExecuteScalarAsync());
    if (count != 8) throw new InvalidOperationException($"Expected 8 work-management tables, found {count}.");
}
Console.WriteLine($"WorkManagement database '{database}' is ready; verified 8 tables.");
