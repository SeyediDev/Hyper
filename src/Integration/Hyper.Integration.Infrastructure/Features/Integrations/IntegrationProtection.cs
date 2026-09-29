using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.DataProtection.XmlEncryption;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace Hyper.Infrastructure.Features.Integrations;

public sealed class IntegrationProtectionOptions
{
    public string KeyRingPath { get; set; } = "";
    public string ApplicationName { get; set; } = "";
    // Explicit because selecting a directory disables automatic key encryption.
    public string KeyEncryption { get; set; } = "";
    public string? CertificateThumbprint { get; set; }
    public bool AllowLegacyPlaintextRead { get; set; }
}

// A separate service type keeps host cookies and timed OAuth state on their
// original provider. This provider is only for persistent Integration secrets.
public interface IIntegrationProtectionProvider : IDataProtectionProvider;

public sealed class IntegrationProtectionProvider(IOptions<IntegrationProtectionOptions> options)
    : IIntegrationProtectionProvider
{
    private readonly Lazy<IDataProtectionProvider> provider = new(() => Create(options.Value));

    public IDataProtector CreateProtector(string purpose) => provider.Value.CreateProtector(purpose);

    private static IDataProtectionProvider Create(IntegrationProtectionOptions options)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(options.KeyRingPath) || !Path.IsPathFullyQualified(options.KeyRingPath)
                || string.IsNullOrWhiteSpace(options.ApplicationName) || options.ApplicationName.Length > 256
                || options.ApplicationName != options.ApplicationName.Trim() || options.ApplicationName.Any(char.IsControl))
                throw new IntegrationCredentialException("IntegrationProtectionConfigurationInvalid");
            var directory = new DirectoryInfo(Path.GetFullPath(options.KeyRingPath));
            // Never silently invent an unrelated key ring when configuration is
            // missing or points to an incorrect deployment location.
            if (!directory.Exists) throw new IntegrationCredentialException("IntegrationProtectionKeyRingUnavailable");
            return DataProtectionProvider.Create(directory, builder =>
            {
                builder.SetApplicationName(options.ApplicationName);
                IntegrationProtectionConfiguration.ApplyKeyEncryption(builder, options);
            });
        }
        catch (Exception error) when (error is IOException or UnauthorizedAccessException
            or CryptographicException or ArgumentException or NotSupportedException)
        {
            // Deliberately omit inner exceptions, paths and key material.
            throw new IntegrationCredentialException("IntegrationProtectionKeyRingUnavailable");
        }
    }
}

public static class IntegrationProtectionConfiguration
{
    public static IDataProtectionBuilder ApplyKeyEncryption(IDataProtectionBuilder builder, IntegrationProtectionOptions options)
    {
        try
        {
            return options.KeyEncryption switch
            {
                "DpapiCurrentUser" when OperatingSystem.IsWindows() => builder.ProtectKeysWithDpapi(protectToLocalMachine: false),
                "DpapiLocalMachine" when OperatingSystem.IsWindows() => builder.ProtectKeysWithDpapi(protectToLocalMachine: true),
                "Certificate" when !string.IsNullOrWhiteSpace(options.CertificateThumbprint) =>
                    ConfigureCertificate(builder, options.CertificateThumbprint),
                _ => throw new IntegrationCredentialException("IntegrationProtectionKeyEncryptionInvalid")
            };
        }
        catch (Exception error) when (error is not IntegrationCredentialException
            && error is CryptographicException or ArgumentException or InvalidOperationException)
        {
            throw new IntegrationCredentialException("IntegrationProtectionKeyEncryptionInvalid");
        }
    }

    private static IDataProtectionBuilder ConfigureCertificate(IDataProtectionBuilder builder, string thumbprint)
    {
        // The framework resolver can find a public-only certificate. A vault
        // writer must also be able to reopen its key files after restart.
        using var certificate = new CertificateResolver().ResolveCertificate(thumbprint);
        if (certificate is null || !certificate.HasPrivateKey)
            throw new IntegrationCredentialException("IntegrationProtectionKeyEncryptionInvalid");
        using var privateKey = certificate.GetRSAPrivateKey();
        using var publicKey = certificate.GetRSAPublicKey();
        if (privateKey is null || publicKey is null)
            throw new IntegrationCredentialException("IntegrationProtectionKeyEncryptionInvalid");
        var probe = RandomNumberGenerator.GetBytes(32);
        byte[]? decoded = null;
        try
        {
            decoded = privateKey.Decrypt(publicKey.Encrypt(probe, RSAEncryptionPadding.Pkcs1), RSAEncryptionPadding.Pkcs1);
            if (!CryptographicOperations.FixedTimeEquals(probe, decoded))
                throw new IntegrationCredentialException("IntegrationProtectionKeyEncryptionInvalid");
        }
        finally
        {
            CryptographicOperations.ZeroMemory(probe);
            if (decoded is not null) CryptographicOperations.ZeroMemory(decoded);
        }
        return builder.ProtectKeysWithCertificate(thumbprint);
    }

    // All writers sharing a ring must encrypt newly generated keys. This only
    // configures key encryption; it never changes a host's ring/app identity.
    public static IDataProtectionBuilder ApplySharedKeyRingEncryption(this IDataProtectionBuilder builder,
        IntegrationProtectionOptions options, string hostKeyRingPath)
    {
        if (string.IsNullOrWhiteSpace(options.KeyRingPath)) return builder;
        if (!Path.IsPathFullyQualified(options.KeyRingPath))
            throw new IntegrationCredentialException("IntegrationProtectionConfigurationInvalid");
        var comparison = OperatingSystem.IsWindows() ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;
        var integrationPath = Path.TrimEndingDirectorySeparator(Path.GetFullPath(options.KeyRingPath));
        var hostPath = Path.TrimEndingDirectorySeparator(Path.GetFullPath(hostKeyRingPath));
        return string.Equals(integrationPath, hostPath, comparison) ? ApplyKeyEncryption(builder, options) : builder;
    }
}

public sealed class IntegrationCredentialException(string code) : InvalidOperationException(code)
{
    public string Code { get; } = code;
}
