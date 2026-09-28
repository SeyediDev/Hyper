// This executable never starts a host or opens an application database.
// SQL execution requires both an explicit switch and a process-scoped connection.
if (args.Length != 1 || args[0] != "--sql")
{
    Console.Error.WriteLine("SQL not run. Set QUEUE_SQL_TEST_CONNECTION for an authorized test SQL Server, then pass --sql.");
    return 2;
}
var connection = Environment.GetEnvironmentVariable("QUEUE_SQL_TEST_CONNECTION");
if (string.IsNullOrWhiteSpace(connection))
{
    Console.Error.WriteLine("SQL not run: QUEUE_SQL_TEST_CONNECTION is missing.");
    return 2;
}

try
{
    await using var fixture = new QueueFixture(connection);
    await fixture.InitializeAsync();
    var cases = new QueueCases(fixture);
    await cases.RunAsync();
    Console.WriteLine($"{cases.Checks} queue SQL assertions passed. Production queue/processor/dispatcher; controlled engagement owner; no HTTP or accounting effects.");
    // DisposeAsync must finish successfully before this exit code is returned.
    return 0;
}
catch (Exception error)
{
    // Do not print connection strings, provider payloads, SQL text or credentials.
    Console.Error.WriteLine(error switch
    {
        QueueCheckFailure => $"FAIL: {error.Message}",
        Microsoft.Data.SqlClient.SqlException sql => $"FAIL: SqlException Number={sql.Number}, State={sql.State}, Class={sql.Class}",
        _ => $"FAIL: {error.GetType().Name}"
    });
    return 1;
}
