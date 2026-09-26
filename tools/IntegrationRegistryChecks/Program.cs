using System.Text.Json;
using Hyper.Infrastructure.Data.Repository.Hyper;
using Hyper.Infrastructure.Features.Integrations;
using Hyper.Integration.Contracts;
using Hyper.Integration.Domain.Entities.Integrations;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Provider = Hyper.Integration.Domain.Entities.Integrations.IntegrationProvider;

// Only this newly created, uniquely named database is changed or removed.
// The configured Integration database is never opened.
var source = Environment.GetEnvironmentVariable("REGISTRY_CHECKS_CONNECTION");
if (args.Length == 2 && args[0] == "--settings")
{
    using var settings = JsonDocument.Parse(await File.ReadAllTextAsync(args[1]));
    source = settings.RootElement.GetProperty("ConnectionStrings").GetProperty("IntegrationConnection").GetString();
}
if (string.IsNullOrWhiteSpace(source))
    throw new ArgumentException("Set REGISTRY_CHECKS_CONNECTION or pass --settings <local appsettings.json>.");
var database = "HyperRegistryChecks_" + Guid.NewGuid().ToString("N");
var masterBuilder = new SqlConnectionStringBuilder(source) { InitialCatalog = "master", Pooling = false };
await using var master = new SqlConnection(masterBuilder.ConnectionString);
await master.OpenAsync();
await Sql(master, $"CREATE DATABASE [{database}]");
var checks = 0;
try
{
    var testBuilder = new SqlConnectionStringBuilder(source) { InitialCatalog = database, Pooling = false };
    await using var sql = new SqlConnection(testBuilder.ConnectionString);
    await sql.OpenAsync();
    var schema = await File.ReadAllTextAsync(Path.Combine(AppContext.BaseDirectory, "schema", "integration.sql"));
    var oauthSchema = await File.ReadAllTextAsync(Path.Combine(AppContext.BaseDirectory, "schema", "oauth.sql"));
    Console.WriteLine("Applying fresh Integration schema...");
    await Sql(sql, schema);
    var options = new DbContextOptionsBuilder<HyperIntegrationContext>().UseSqlServer(testBuilder.ConnectionString).Options;
    using var http = new HttpClient(new NoNetwork());
    var oauth = new BasalamOAuthService(Options.Create(new BasalamOAuthSettings()), http, new EphemeralDataProtectionProvider());
    const int shopId = 991;
    const string vendor = "registry-vendor";

    // Persist through the real registry and OAuth store before simulating an old deployment.
    var a = await Register("tenant-a");
    Check(a is not null && !a.Connection.IsEnabled, "new registry connection starts disabled");
    var aId = await Authorize("tenant-a", vendor, "a-original");
    Check(aId == a!.Connection.Id, "OAuth reuses the matching registry connection");
    await using (var db = new HyperIntegrationContext(options))
    {
        Check(db.Model.FindEntityType(typeof(ExternalIntegrationConnection))!.GetKeys().Any(k =>
            k.Properties.Select(p => p.Name).SequenceEqual(new[] { "ShopId", "TenantId", "Provider", "AccountIdentifier" })),
            "EF connection key includes tenant");
        Check(db.Model.FindEntityType(typeof(ExternalOAuthToken))!.GetIndexes().Any(i => i.IsUnique &&
            i.Properties.Select(p => p.Name).SequenceEqual(new[] { "ShopId", "TenantId", "Provider" })),
            "EF token identity includes tenant");
    }
    var originalTokenId = await TokenId("tenant-a");
    // Safe here: the only records are this test's tenant-a fixtures.
    await Sql(sql, """
        ALTER TABLE dbo.ExternalIntegrationConnections DROP CONSTRAINT UQ_ExternalIntegrationConnections;
        ALTER TABLE dbo.ExternalIntegrationConnections ADD CONSTRAINT UQ_ExternalIntegrationConnections UNIQUE (ShopId, Provider, AccountIdentifier);
        DROP INDEX UX_ExternalOAuthTokens_ShopId_Provider ON dbo.ExternalOAuthTokens;
        CREATE UNIQUE INDEX UX_ExternalOAuthTokens_ShopId_Provider ON dbo.ExternalOAuthTokens(ShopId, Provider);
        """);
    await Sql(sql, oauthSchema);
    await Sql(sql, oauthSchema);
    Check(await TokenId("tenant-a") == originalTokenId && await Access("tenant-a") == "a-original",
        "standalone OAuth upgrade is repeatable and preserves existing token");
    // Exercise the full provisioner's OAuth upgrade independently too.
    await Sql(sql, """
        DROP INDEX UX_ExternalOAuthTokens_ShopId_Provider ON dbo.ExternalOAuthTokens;
        CREATE UNIQUE INDEX UX_ExternalOAuthTokens_ShopId_Provider ON dbo.ExternalOAuthTokens(ShopId, Provider);
        """);
    Console.WriteLine("Upgrading legacy Integration schema...");
    await Sql(sql, schema);
    Console.WriteLine("Repeating upgraded Integration schema...");
    await Sql(sql, schema);
    Check(await TokenId("tenant-a") == originalTokenId && await Access("tenant-a") == "a-original",
        "full schema upgrade is repeatable and preserves existing token");

    var b = await Register("tenant-b");
    Check(b is not null && b.Connection.Id != aId, "same shop/provider/vendor can register in another tenant");
    Check(await Register("tenant-a") is null, "duplicate registry identity in the same tenant is rejected");
    var bId = await Authorize("tenant-b", vendor, "b-original");
    Check(bId == b!.Connection.Id && await Access("tenant-a") == "a-original" && await Access("tenant-b") == "b-original",
        "OAuth callback stores independent tokens for both tenants");
    Check(await Authorize("tenant-a", vendor, "a-renewed") == aId && await TokenId("tenant-a") == originalTokenId
        && await Access("tenant-b") == "b-original", "renewal updates only the matching tenant and preserves token identity");
    try
    {
        await Authorize("tenant-a", "different-vendor", "unexpected");
        throw new Exception("A second vendor unexpectedly replaced the existing token.");
    }
    catch (InvalidOperationException)
    {
        Check(await Access("tenant-a") == "a-renewed", "same-tenant different vendor cannot replace the token");
    }
    await using (var db = new HyperIntegrationContext(options))
    {
        var api = new IntegrationManagementApi(db);
        var listed = await api.ListConnectionsAsync(new(shopId, "tenant-a"));
        Check(listed.Count == 1 && listed[0].Id == aId, "listing isolates tenant and failed OAuth rolls back its connection");
        Check(!await api.SetConnectionEnabledAsync(new(shopId, "tenant-a", bId), false), "cross-tenant disable rejected");
        Check(!await api.SetConnectionEnabledAsync(new(shopId + 1, "tenant-b", bId), false), "cross-shop disable rejected");
        Check(await api.SetConnectionEnabledAsync(new(shopId, "tenant-b", bId), false), "owner can disable connection");
        Check((await api.ListConnectionsAsync(new(shopId, "tenant-a")))[0].IsEnabled, "disabling tenant-b leaves tenant-a enabled");
        var json = JsonSerializer.Serialize(listed);
        Check(!json.Contains("CredentialsJson") && !json.Contains("AccessToken") && !json.Contains("RefreshToken"),
            "registry response excludes credentials");
    }
    await RejectDuplicate("""
        INSERT dbo.ExternalIntegrationConnections(ShopId,TenantId,Provider,DisplayName,AccountIdentifier,CredentialType,CredentialsJson)
        VALUES(991,N'tenant-a',1,N'duplicate',N'registry-vendor',1,N'{}');
        """, "SQL rejects duplicate connection within tenant");
    await RejectDuplicate("""
        INSERT dbo.ExternalOAuthTokens(ConnectionId,ShopId,TenantId,Provider,AccessToken,TokenType)
        SELECT ConnectionId,ShopId,TenantId,Provider,AccessToken,TokenType FROM dbo.ExternalOAuthTokens WHERE TenantId=N'tenant-a';
        """, "SQL rejects duplicate token within tenant");
    Console.WriteLine($"{checks} registry checks passed against freshly built production code.");

    async Task<IntegrationConnectionResponse?> Register(string tenant)
    {
        await using var db = new HyperIntegrationContext(options);
        return await new IntegrationManagementApi(db).CreateConnectionAsync(new(shopId, tenant,
            Hyper.Integration.Contracts.IntegrationProvider.Basalam, "Registry checks", vendor, 1));
    }
    async Task<long> Authorize(string tenant, string vendorId, string access)
    {
        await using var db = new HyperIntegrationContext(options);
        var simulation = new IntegrationAdminSimulation { AdminUserId = "registry-checks", ShopId = shopId,
            TenantId = tenant, MerchantIdentifier = "registry-checks", ShopName = "Registry checks", ExpiresAtUtc = DateTime.UtcNow.AddMinutes(10) };
        var request = new IntegrationTokenRequest { SimulationId = simulation.Id, Provider = Provider.Basalam,
            CredentialType = IntegrationCredentialType.OAuth2, Status = 1 };
        db.AddRange(simulation, request);
        await db.SaveChangesAsync();
        return await new BasalamOAuthStore(db, oauth).SaveAsync(
            new(request.Id, simulation.Id, simulation.AdminUserId, "nonce", null, "http://localhost/callback"), simulation,
            new(vendorId, "Registry checks"), new() { AccessToken = access, RefreshToken = "test-refresh", ExpiresIn = 3600 }, default);
    }
    async Task<long> TokenId(string tenant)
    {
        await using var db = new HyperIntegrationContext(options);
        return await db.ExternalOAuthTokens.Where(t => t.ShopId == shopId && t.TenantId == tenant).Select(t => t.Id).SingleAsync();
    }
    async Task<string> Access(string tenant)
    {
        await using var db = new HyperIntegrationContext(options);
        return oauth.DecryptToken(await db.ExternalOAuthTokens.Where(t => t.ShopId == shopId && t.TenantId == tenant)
            .Select(t => t.AccessToken).SingleAsync());
    }
    async Task RejectDuplicate(string command, string name)
    {
        try { await Sql(sql, command); }
        catch (SqlException ex) when (ex.Number is 2601 or 2627) { Check(true, name); return; }
        throw new Exception(name);
    }
}
finally
{
    // database was generated above and successfully created by this invocation.
    await Sql(master, $"DROP DATABASE [{database}]");
    Console.WriteLine("Removed this run's disposable registry-check database.");
}
void Check(bool condition, string name)
{
    if (!condition) throw new Exception(name);
    checks++;
    Console.WriteLine("PASS " + name);
}
async Task Sql(SqlConnection connection, string sql)
{
    await using var command = connection.CreateCommand();
    command.CommandText = sql;
    command.CommandTimeout = 180;
    var execution = command.ExecuteNonQueryAsync();
    if (await Task.WhenAny(execution, Task.Delay(TimeSpan.FromSeconds(20))) != execution
        && connection.Database == database)
    {
        // Diagnose only this verifier's database; never print other sessions' SQL.
        await using var diagnostics = new SqlConnection(masterBuilder.ConnectionString);
        try
        {
            await diagnostics.OpenAsync();
            await using var inspect = diagnostics.CreateCommand();
            inspect.CommandText = "SELECT session_id, status, wait_type, blocking_session_id FROM sys.dm_exec_requests WHERE database_id = DB_ID(@database)";
            inspect.Parameters.AddWithValue("@database", connection.Database);
            await using var reader = await inspect.ExecuteReaderAsync();
            while (await reader.ReadAsync())
                Console.WriteLine($"SQL diagnostic: session={reader[0]}, status={reader[1]}, wait={reader[2]}, blocker={reader[3]}");
        }
        catch (SqlException ex) { Console.WriteLine($"SQL diagnostics unavailable ({ex.Number})."); }
    }
    await execution;
}
sealed class NoNetwork : HttpMessageHandler
{
    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken ct) =>
        throw new InvalidOperationException("External HTTP is prohibited in registry checks.");
}
