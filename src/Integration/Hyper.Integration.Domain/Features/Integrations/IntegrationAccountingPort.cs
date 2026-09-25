namespace Hyper.Integration.Domain.Features.Integrations;

/// <summary>Provider-neutral customer identity supplied by an external marketplace.</summary>
public sealed record IntegrationCustomerIdentity(
    int ShopId,
    string TenantId,
    string Name,
    string? Mobile,
    string? IdentifierNumber,
    string ExternalCustomerId);

/// <summary>
/// Anti-corruption port for operations owned by Hyperyek accounting.
/// The Integration domain knows only scalar identifiers and this contract.
/// </summary>
public interface IIntegrationAccountingPort
{
    Task<bool> ValidateLinkedCustomerAsync(IntegrationCustomerIdentity customer, int personId,
        CancellationToken cancellationToken);

    Task<int> ResolveOrCreateCustomerAsync(IntegrationCustomerIdentity customer,
        CancellationToken cancellationToken);
}
