using Hyper.Infrastructure.Features.Integrations;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Options;

// Explicit test-only composition. Production has no ephemeral or plaintext
// fallback. Old fixtures opt into legacy reads to retain their declared data.
internal static class FixtureProtection
{
    private static readonly IDataProtectionProvider SharedKeys = new EphemeralDataProtectionProvider();
    internal static IIntegrationProtectionProvider Provider(IDataProtectionProvider? keys = null) => new ProviderAdapter(keys ?? SharedKeys);
    internal static IntegrationCredentialVault Vault(bool allowLegacy = false, IDataProtectionProvider? keys = null) =>
        new(Provider(keys), Options.Create(new IntegrationProtectionOptions { AllowLegacyPlaintextRead = allowLegacy }));
    internal static IntegrationCredentialVault LegacyVault() => Vault(allowLegacy: true);

    private sealed class ProviderAdapter(IDataProtectionProvider keys) : IIntegrationProtectionProvider
    {
        public IDataProtector CreateProtector(string purpose) => keys.CreateProtector(purpose);
    }
}
