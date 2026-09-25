using Hyper.Integration.Domain.Entities.Integrations;
using Hyper.Integration.Domain.Features.Integrations;
using Hyper.Infrastructure.Data.Repository.Hyper;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Hyper.Infrastructure.Features.Integrations;

public sealed record IdentifiedBoothCustomer(int ShopId, string TenantId, string Name,
    string? Mobile, string? IdentifierNumber, string? BasalamUserId);

public interface IIntegrationCustomerRegistration
{
    Task<int> ResolveOrCreateAsync(IdentifiedBoothCustomer customer, CancellationToken ct);
}

/// <summary>
/// Integration-side mapping orchestration. Accounting entity access belongs to
/// <see cref="HyperyekAccountingCustomerAdapter"/> behind the domain port.
/// </summary>
public sealed class IntegrationCustomerRegistration(
    HyperIntegrationContext integrations,
    IIntegrationAccountingPort accounting) : IIntegrationCustomerRegistration
{
    public async Task<int> ResolveOrCreateAsync(IdentifiedBoothCustomer customer, CancellationToken ct)
    {
        Validate(customer);
        var identity = new IntegrationCustomerIdentity(customer.ShopId, customer.TenantId, customer.Name,
            customer.Mobile?.Trim(), customer.IdentifierNumber?.Trim(), customer.BasalamUserId!);

        await using var transaction = await integrations.Database.BeginTransactionAsync(IsolationLevel.Serializable, ct);
        var linked = await integrations.IntegrationCustomerMappings.SingleOrDefaultAsync(x =>
            x.ShopId == identity.ShopId && x.TenantId == identity.TenantId &&
            x.ExternalCustomerId == identity.ExternalCustomerId, ct);
        if (linked is not null)
        {
            if (!await accounting.ValidateLinkedCustomerAsync(identity, linked.PersonId, ct))
                throw new InvalidOperationException("BasalamCustomerMappingNeedsReview");
            await transaction.CommitAsync(ct);
            return linked.PersonId;
        }

        var personId = await accounting.ResolveOrCreateCustomerAsync(identity, ct);
        integrations.IntegrationCustomerMappings.Add(new IntegrationCustomerMapping
        {
            ShopId = identity.ShopId,
            TenantId = identity.TenantId,
            ExternalCustomerId = identity.ExternalCustomerId,
            PersonId = personId
        });
        await integrations.SaveChangesAsync(ct);
        await transaction.CommitAsync(ct);
        return personId;
    }

    private static void Validate(IdentifiedBoothCustomer customer)
    {
        if (customer.ShopId <= 0 || string.IsNullOrWhiteSpace(customer.TenantId) || customer.TenantId.Length > 30
            || string.IsNullOrWhiteSpace(customer.Name) || customer.Name.Length > 100
            || string.IsNullOrWhiteSpace(customer.BasalamUserId)
            || (string.IsNullOrWhiteSpace(customer.Mobile) && string.IsNullOrWhiteSpace(customer.IdentifierNumber)))
            throw new ArgumentException("InvalidIdentifiedBoothCustomer", nameof(customer));
        if (customer.Mobile?.Trim().Length > 15 || customer.IdentifierNumber?.Trim().Length > 12)
            throw new ArgumentException("InvalidCustomerFieldLength", nameof(customer));
    }
}
