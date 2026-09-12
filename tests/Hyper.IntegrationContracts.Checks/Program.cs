using Hyper.Domain.Entities.Integrations;
using Hyper.Domain.Features.Integrations;
using Hyper.Infrastructure.Features.Integrations;

var passed = 0;
void Check(string name, Action test) { test(); passed++; Console.WriteLine($"PASS {name}"); }
void Throws<T>(Action action) where T : Exception
{
    try { action(); } catch (T) { return; }
    throw new Exception($"Expected {typeof(T).Name}.");
}
var now = new DateTime(2026, 9, 10, 0, 0, 0, DateTimeKind.Utc);
ExternalIntegrationConnection Ready() => new()
{
    ShopId = 1, TenantId = "shop-one", AccountIdentifier = "vendor-one",
    Provider = IntegrationProvider.Basalam, CredentialType = IntegrationCredentialType.OAuth2,
    CredentialsJson = "test-only-credential", ExpiresAtUtc = now.AddHours(1)
};
Check("enabled unexpired connection passes", () => IntegrationConnectionReadiness.Validate(Ready(), now));
Check("disabled rejected", () => { var c = Ready(); c.IsEnabled = false; Throws<InvalidOperationException>(() => IntegrationConnectionReadiness.Validate(c, now)); });
Check("missing credentials rejected", () => { var c = Ready(); c.CredentialsJson = " "; Throws<InvalidOperationException>(() => IntegrationConnectionReadiness.Validate(c, now)); });
Check("expiry boundary rejected", () => { var c = Ready(); c.ExpiresAtUtc = now; Throws<InvalidOperationException>(() => IntegrationConnectionReadiness.Validate(c, now)); });
Check("missing owner metadata rejected", () => { var c = Ready(); c.ShopId = 0; Throws<InvalidOperationException>(() => IntegrationConnectionReadiness.Validate(c, now)); });
var oauth = new StubAdapter(IntegrationProvider.Basalam, IntegrationCredentialType.OAuth2);
var bearer = new StubAdapter(IntegrationProvider.Basalam, IntegrationCredentialType.BearerToken);
var resolver = new IntegrationStrategyResolver([oauth, bearer]);
Check("resolver selects exact credential strategy", () =>
{
    if (!ReferenceEquals(resolver.Resolve(IntegrationProvider.Basalam, IntegrationCredentialType.BearerToken), bearer))
        throw new Exception("Wrong credential strategy selected.");
});
Check("wrong credential rejected", () => Throws<NotSupportedException>(() => resolver.Resolve(IntegrationProvider.Basalam, IntegrationCredentialType.Basic)));
Check("wrong provider rejected", () => Throws<NotSupportedException>(() => resolver.Resolve(IntegrationProvider.Torob, IntegrationCredentialType.OAuth2)));
Check("unknown enum rejected", () => Throws<NotSupportedException>(() => resolver.Resolve((IntegrationProvider)99, IntegrationCredentialType.OAuth2)));
Check("ambiguous registration rejected", () => Throws<InvalidOperationException>(() => new IntegrationStrategyResolver([oauth, oauth]).Resolve(oauth.Provider, IntegrationCredentialType.OAuth2)));
Check("unimplemented registration rejected", () => Throws<NotSupportedException>(() => new IntegrationStrategyResolver([new StubAdapter(IntegrationProvider.Basalam, IntegrationCredentialType.OAuth2, false)]).Resolve(oauth.Provider, IntegrationCredentialType.OAuth2)));
IExternalIntegrationAdapter[] adapters = [new DigikalaIntegrationAdapter(new NoNetwork()), new TorobIntegrationAdapter(new NoNetwork())];
foreach (var adapter in adapters)
{
    Check($"{adapter.Provider} cannot report successful empty catalog", () => Throws<NotSupportedException>(() => adapter.ReadCatalogAsync(Ready(), default).GetAwaiter().GetResult()));
    Check($"{adapter.Provider} cannot report successful inventory publish", () => Throws<NotSupportedException>(() => adapter.PublishInventoryAsync(Ready(), [], default).GetAwaiter().GetResult()));
    Check($"{adapter.Provider} advertises no verified credentials", () =>
    {
        if (adapter.IsImplemented || Enum.GetValues<IntegrationCredentialType>().Any(adapter.SupportsCredentialType))
            throw new Exception("Unavailable adapter advertises support.");
    });
}
passed += await BasalamChecks.Run();
passed += WebhookChecks.Run();
passed += AccessChecks.Run();
Console.WriteLine($"{passed} checks passed; no database or provider calls made.");

sealed class NoNetwork : IHttpClientFactory
{
    public HttpClient CreateClient(string name) => throw new Exception("Unexpected network access.");
}
sealed class StubAdapter(IntegrationProvider provider, IntegrationCredentialType credential, bool implemented = true) : IExternalIntegrationAdapter
{
    public IntegrationProvider Provider => provider;
    public bool IsImplemented => implemented;
    public bool SupportsCredentialType(IntegrationCredentialType type) => type == credential;
    public Task<IReadOnlyCollection<ExternalCatalogItem>> ReadCatalogAsync(ExternalIntegrationConnection connection, CancellationToken cancellationToken) => throw new Exception("Selection should not call the provider.");
    public Task PublishInventoryAsync(ExternalIntegrationConnection connection, IReadOnlyCollection<ExternalInventoryUpdate> updates, CancellationToken cancellationToken) => throw new Exception("Selection should not call the provider.");
}
