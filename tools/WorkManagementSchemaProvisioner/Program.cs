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
var backlogSeed = await File.ReadAllTextAsync(Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..", "docs", "schema", "seed-work-management-backlog.sql")));
await using (var c = new SqlConnection(cs.ConnectionString))
{
    await c.OpenAsync();
    foreach (var batch in Regex.Split(script + Environment.NewLine + backlogSeed, @"^\s*GO\s*$", RegexOptions.Multiline | RegexOptions.IgnoreCase)
        .Where(x => !string.IsNullOrWhiteSpace(x)))
    {
        await using var batchCommand = c.CreateCommand(); batchCommand.CommandText = batch; batchCommand.CommandTimeout = 180; await batchCommand.ExecuteNonQueryAsync();
    }
    await using var cmd = c.CreateCommand();
    cmd.CommandText = "SELECT COUNT(*) FROM sys.tables WHERE name IN ('Projects','WorkItems','WorkRoles','WorkItemLogs','ChatWorkIntakes','WorkItemDependencies','WorkItemCommits','WorkItemTestEvidence')";
    var count = Convert.ToInt32(await cmd.ExecuteScalarAsync());
    if (count != 8) throw new InvalidOperationException($"Expected 8 work-management tables, found {count}.");
    cmd.CommandText = "SELECT COUNT(*) FROM dbo.WorkItems WHERE ProjectId = (SELECT Id FROM dbo.Projects WHERE [Key]=N'HYPER')";
    var workItemCount = Convert.ToInt32(await cmd.ExecuteScalarAsync());
    if (workItemCount < 60) throw new InvalidOperationException($"Expected seeded backlog, found {workItemCount} work items.");
    Console.WriteLine($"Seeded backlog items: {workItemCount}.");

    if (Environment.GetEnvironmentVariable("WORK_MANAGEMENT_CLAIM_KEY") is { Length: > 0 } claimKey)
    {
        var agent = Environment.GetEnvironmentVariable("WORK_MANAGEMENT_AGENT") ?? "codex";
        var branch = Environment.GetEnvironmentVariable("WORK_MANAGEMENT_BRANCH") ?? "develop";
        await using var claim = c.CreateCommand();
        claim.CommandText = """
            SELECT Id, Status, OwnerRole, CommitSha FROM dbo.WorkItems
            WHERE ProjectId=(SELECT Id FROM dbo.Projects WHERE [Key]=N'HYPER') AND [Key]=@key;
            """;
        claim.Parameters.AddWithValue("@key", claimKey);
        await using var reader = await claim.ExecuteReaderAsync();
        if (!await reader.ReadAsync()) throw new InvalidOperationException($"Work item '{claimKey}' was not found.");
        var itemId = reader.GetInt64(0);
        var status = reader.GetByte(1);
        var ownerRole = reader.IsDBNull(2) ? null : reader.GetString(2);
        var commitSha = reader.IsDBNull(3) ? null : reader.GetString(3);
        await reader.CloseAsync();
        if (status is 6 or 7)
        {
            Console.WriteLine($"Work item {claimKey} is already terminal (status={status}, commit={commitSha ?? "none"}).");
        }
        else if (status == 3)
        {
            Console.WriteLine($"Work item {claimKey} is already InProgress (role={ownerRole ?? "none"}).");
        }
        else
        {
            await using var busy = c.CreateCommand();
            busy.CommandText = "SELECT COUNT(*) FROM dbo.WorkItems WHERE OwnerRole=@role AND Status=3";
            busy.Parameters.AddWithValue("@role", ownerRole ?? "");
            if (Convert.ToInt32(await busy.ExecuteScalarAsync()) > 0)
                throw new InvalidOperationException($"Owner role '{ownerRole}' already has an InProgress item.");
            await using var update = c.CreateCommand();
            update.CommandText = """
                UPDATE dbo.WorkItems SET Status=3, OwnerAgent=@agent, ChatId=N'codex', Branch=@branch, UpdatedAtUtc=SYSUTCDATETIME() WHERE Id=@id;
                INSERT dbo.WorkItemLogs(WorkItemId,Author,Message,ChatId,CreatedAtUtc) VALUES(@id,@agent,N'Claimed by Codex for priority implementation.',N'codex',SYSUTCDATETIME());
                """;
            update.Parameters.AddWithValue("@id", itemId);
            update.Parameters.AddWithValue("@agent", agent);
            update.Parameters.AddWithValue("@branch", branch);
            await update.ExecuteNonQueryAsync();
            Console.WriteLine($"Claimed work item {claimKey} (id={itemId}, role={ownerRole}, agent={agent}, branch={branch}).");
        }

        if (Environment.GetEnvironmentVariable("WORK_MANAGEMENT_COMMIT_SHA") is { Length: > 0 } sha)
        {
            await using var evidence = c.CreateCommand();
            evidence.CommandText = "INSERT dbo.WorkItemCommits(WorkItemId,Sha,Message,CreatedAtUtc) VALUES(@id,@sha,@message,SYSUTCDATETIME())";
            evidence.Parameters.AddWithValue("@id", itemId);
            evidence.Parameters.AddWithValue("@sha", sha);
            evidence.Parameters.AddWithValue("@message", Environment.GetEnvironmentVariable("WORK_MANAGEMENT_COMMIT_MESSAGE") ?? "Implementation evidence");
            await evidence.ExecuteNonQueryAsync();
            Console.WriteLine($"Recorded commit evidence for {claimKey}: {sha}.");
        }
        if (Environment.GetEnvironmentVariable("WORK_MANAGEMENT_TEST_NAME") is { Length: > 0 } testName)
        {
            await using var test = c.CreateCommand();
            test.CommandText = "INSERT dbo.WorkItemTestEvidence(WorkItemId,TestName,Result,Details,CreatedAtUtc) VALUES(@id,@name,@result,@details,SYSUTCDATETIME())";
            test.Parameters.AddWithValue("@id", itemId);
            test.Parameters.AddWithValue("@name", testName);
            test.Parameters.AddWithValue("@result", Environment.GetEnvironmentVariable("WORK_MANAGEMENT_TEST_RESULT") ?? "Passed");
            test.Parameters.AddWithValue("@details", Environment.GetEnvironmentVariable("WORK_MANAGEMENT_TEST_DETAILS") ?? "");
            await test.ExecuteNonQueryAsync();
            Console.WriteLine($"Recorded test evidence for {claimKey}: {testName}.");
        }
    }
}
Console.WriteLine($"WorkManagement database '{database}' is ready; verified 8 tables.");
