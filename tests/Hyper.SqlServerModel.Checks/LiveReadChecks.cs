using System.Reflection;
using System.Text.Json;
using Hyper.Infrastructure.Data.Repository.Hyper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

internal static class LiveReadChecks
{
    public static async Task Run(string settingsPath, bool localSharedMemory)
    {
        using var settings = JsonDocument.Parse(await File.ReadAllTextAsync(settingsPath),
            new JsonDocumentOptions { CommentHandling = JsonCommentHandling.Skip, AllowTrailingCommas = true });
        var connectionString = settings.RootElement.GetProperty("ConnectionStrings").GetProperty("Domain").GetString()!;
        var builder = new SqlConnectionStringBuilder(connectionString);
        if (builder.InitialCatalog != "Hyperyek") throw new Exception("Live checks require the configured Hyperyek database.");
        if (localSharedMemory)
        {
            if (builder.DataSource is not ("." or "localhost" or "(local)"))
                throw new Exception("Local checks only support the local default SQL instance.");
            // Isolated-machine fallback when sandbox TLS is unavailable. Never used for remote SQL.
            builder.DataSource = "lpc:.";
            builder.Encrypt = SqlConnectionEncryptOption.Optional;
            connectionString = builder.ConnectionString;
            Console.WriteLine("Live checks use local shared memory with optional encryption; application settings are unchanged.");
        }
        await using var db = new HyperSqlServerContext(new DbContextOptionsBuilder<HyperSqlServerContext>()
            .UseSqlServer(connectionString).Options);
        var read = typeof(LiveReadChecks).GetMethod(nameof(ReadOne), BindingFlags.Static | BindingFlags.NonPublic)!;
        var objects = 0;
        var populated = 0;
        foreach (var entity in db.Model.GetEntityTypes())
        {
            // Materialize actual CLR values; SELECT COUNT cannot catch binary, date or precision mismatches.
            var result = (Task<int>)read.MakeGenericMethod(entity.ClrType).Invoke(null, [db])!;
            populated += await result;
            objects++;
        }
        Console.WriteLine($"PASS live read/materialization from {objects} SQL objects; {populated} returned a sample row. Values and credentials were not logged. No writes.");
    }

    private static async Task<int> ReadOne<T>(HyperSqlServerContext db) where T : class =>
        (await db.Set<T>().AsNoTracking().Take(1).ToListAsync()).Count;
}
