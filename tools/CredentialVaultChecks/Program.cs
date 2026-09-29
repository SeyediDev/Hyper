using Microsoft.Data.SqlClient;

if (args.Length != 1 || args[0] is not ("--pure" or "--sql"))
{
    Console.Error.WriteLine("Use --pure for local cryptography/intercepted HTTP checks, or --sql for the owned SQL fixture too.");
    return 2;
}
var connection = Environment.GetEnvironmentVariable("CREDENTIAL_VAULT_SQL_TEST_CONNECTION");
if (args[0] == "--sql" && string.IsNullOrWhiteSpace(connection))
{
    Console.Error.WriteLine("Set CREDENTIAL_VAULT_SQL_TEST_CONNECTION securely. Its database is never used.");
    return 2;
}
try
{
    PureChecks.Run();
    if (args[0] == "--sql")
    {
        await using var fixture = new VaultSqlFixture(connection!);
        await fixture.InitializeAsync();
        await PersistenceChecks.Run(fixture);
    }
    Console.WriteLine($"{VaultCheck.Count} credential checks passed; no live provider or native accounting calls.");
    return 0;
}
catch (Exception ex)
{
    Console.Error.WriteLine(ex switch
    {
        VaultCheckFailure failure => failure.Message,
        SqlException sql => $"SQL failure Number={sql.Number} State={sql.State} Class={sql.Class}",
        _ => "Credential check failed: " + ex.GetType().Name
    });
    return 1;
}

internal static class VaultCheck
{
    internal static int Count { get; private set; }
    internal static void That(bool value, string name)
    {
        if (!value) throw new VaultCheckFailure(name);
        Count++;
        Console.WriteLine("PASS " + name);
    }
    internal static void Reject(Action action, string name)
    {
        try { action(); }
        catch (Hyper.Infrastructure.Features.Integrations.IntegrationCredentialException ex)
        {
            That(ex.Message == ex.Code && ex.InnerException is null, name + " (sanitized)");
            return;
        }
        throw new VaultCheckFailure(name);
    }
    internal static async Task RejectAsync(Func<Task> action, string name)
    {
        try { await action(); }
        catch (Hyper.Infrastructure.Features.Integrations.IntegrationCredentialException ex)
        {
            That(ex.Message == ex.Code && ex.InnerException is null, name + " (sanitized)");
            return;
        }
        throw new VaultCheckFailure(name);
    }
}
internal sealed class VaultCheckFailure(string message) : Exception(message);
