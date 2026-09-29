// Never starts an application host or connects to its configured database.
if (args.Length != 1 || args[0] != "--sql")
{
    Console.Error.WriteLine("SQL not run. Set ORDER_MAPPING_SQL_TEST_CONNECTION for an authorized test SQL Server, then pass --sql.");
    return 2;
}
var connection = Environment.GetEnvironmentVariable("ORDER_MAPPING_SQL_TEST_CONNECTION");
if (string.IsNullOrWhiteSpace(connection))
{
    Console.Error.WriteLine("SQL not run: ORDER_MAPPING_SQL_TEST_CONNECTION is missing.");
    return 2;
}
try
{
    await using var fixture = new MappingFixture(connection);
    await fixture.InitializeAsync();
    var cases = new MappingCases(fixture);
    await cases.RunAsync();
    Console.WriteLine($"{cases.Checks} order mapping SQL assertions passed. Production dispatcher/receipt/context; controlled accounting ACK and reservation probes; no HTTP or native accounting effects.");
    // Successful fixture disposal is required for a successful exit.
    return 0;
}
catch (Exception error)
{
    Console.Error.WriteLine(error switch
    {
        MappingCheckFailure => $"FAIL: {error.Message}",
        Microsoft.Data.SqlClient.SqlException sql => $"FAIL: SqlException Number={sql.Number}, State={sql.State}, Class={sql.Class}",
        _ => $"FAIL: {error.GetType().Name}"
    });
    return 1;
}
