using Hyper.Domain.Entities.Integrations;

namespace Hyper.Infrastructure.Features.Integrations;

/// <summary>Provider-specific authorization capabilities. Unsupported providers never fall back to Basalam.</summary>
public sealed record IntegrationAuthorizationOption(IntegrationProvider Provider, string Title,
    IntegrationCredentialType CredentialType, string? StartPath, string? UnavailableReason);

public static class IntegrationAuthorizationCatalog
{
    public static IReadOnlyList<IntegrationAuthorizationOption> Options { get; } =
    [
        new(IntegrationProvider.Basalam, "باسلام", IntegrationCredentialType.OAuth2,
            "/api/auth/basalam/login", null),
        new(IntegrationProvider.Digikala, "دیجی‌کالا", IntegrationCredentialType.ApiKey,
            null, "دریافت و اعتبارسنجی دسترسی دیجی‌کالا هنوز پیاده‌سازی نشده است."),
        new(IntegrationProvider.Torob, "ترب", IntegrationCredentialType.ApiKey,
            null, "دریافت و اعتبارسنجی دسترسی ترب هنوز پیاده‌سازی نشده است."),
        new(IntegrationProvider.Custom, "سامانه دیگر", IntegrationCredentialType.ApiKey,
            null, "ابتدا آداپتر و روش احراز هویت این سامانه باید تعریف شود.")
    ];

    public static IntegrationAuthorizationOption? Find(IntegrationProvider provider) =>
        Options.SingleOrDefault(x => x.Provider == provider);
}
