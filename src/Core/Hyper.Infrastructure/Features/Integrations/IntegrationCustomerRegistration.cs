using Hyper.Domain.Entities.Database;
using Hyper.Infrastructure.Data.Repository.Hyper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Data;

namespace Hyper.Infrastructure.Features.Integrations;

/// <summary>The accounting installation must supply its actual Person TYPE_/ROLES_ convention.</summary>
public sealed class IntegrationCustomerOptions
{
    public byte? PersonType { get; set; }
    public string? CustomerRole { get; set; }
    public string? DetailAccountEntityType { get; set; }
}

public sealed record IdentifiedBoothCustomer(int ShopId, string TenantId, string Name,
    string? Mobile, string? IdentifierNumber, string? BasalamUserId);

public interface IIntegrationCustomerRegistration
{
    Task<int> ResolveOrCreateAsync(IdentifiedBoothCustomer customer, CancellationToken ct);
}

/// <summary>Returns TBL_Person.ID_ for use as TBL_SaleOrder.CUSTOMERID_.</summary>
public sealed class IntegrationCustomerRegistration(HyperSqlServerContext db,
    IOptions<IntegrationCustomerOptions> options) : IIntegrationCustomerRegistration
{
    public async Task<int> ResolveOrCreateAsync(IdentifiedBoothCustomer customer, CancellationToken ct)
    {
        var policy = options.Value;
        if (policy.PersonType is null || string.IsNullOrWhiteSpace(policy.CustomerRole)
            || policy.CustomerRole.Length > 20 || string.IsNullOrWhiteSpace(policy.DetailAccountEntityType)
            || policy.DetailAccountEntityType.Length > 50)
            throw new InvalidOperationException("AccountingCustomerConventionNotConfigured");
        if (customer.ShopId <= 0 || string.IsNullOrWhiteSpace(customer.TenantId) || customer.TenantId.Length > 30
            || string.IsNullOrWhiteSpace(customer.Name) || customer.Name.Length > 100
            || string.IsNullOrWhiteSpace(customer.BasalamUserId)
            || (string.IsNullOrWhiteSpace(customer.Mobile) && string.IsNullOrWhiteSpace(customer.IdentifierNumber)))
            throw new ArgumentException("InvalidIdentifiedBoothCustomer", nameof(customer));
        var mobile = customer.Mobile?.Trim();
        var identifier = customer.IdentifierNumber?.Trim();
        if (mobile?.Length > 15 || identifier?.Length > 12 || customer.Name.Length > 100)
            throw new ArgumentException("InvalidCustomerFieldLength", nameof(customer));

        // Serializable protects the negative lookup against concurrent duplicate registrations.
        await using var transaction = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable, ct);
        var linked = await db.IntegrationCustomerMappings.SingleOrDefaultAsync(x => x.ShopId == customer.ShopId
            && x.TenantId == customer.TenantId && x.BasalamUserId == customer.BasalamUserId, ct);
        if (linked is not null)
        {
            var linkedPerson = await db.TblPersons.SingleOrDefaultAsync(x => x.Id == linked.PersonId
                && x.Shopid == customer.ShopId && x.TenantId == customer.TenantId && x.Isenabled, ct);
            if (linkedPerson is null || linkedPerson.Type != policy.PersonType || !HasRole(linkedPerson.Roles, policy.CustomerRole)
                || (mobile != null && linkedPerson.Mobilenumber != null && linkedPerson.Mobilenumber != mobile)
                || (identifier != null && linkedPerson.Identifiernumber != null && linkedPerson.Identifiernumber != identifier))
                throw new InvalidOperationException("BasalamCustomerMappingNeedsReview");
            if (!await db.TblDetailaccounts.AnyAsync(a => a.Detailaccountid == linkedPerson.Detailaccountid
                && a.Shopid == customer.ShopId && a.TenantId == customer.TenantId, ct))
                throw new InvalidOperationException("AccountingCustomerDetailAccountMismatch");
            if (!await db.TblDetailaccounts.AnyAsync(a => a.Detailaccountid == linkedPerson.Detailaccountid
                && a.Shopid == customer.ShopId && a.TenantId == customer.TenantId, ct))
                throw new InvalidOperationException("AccountingCustomerDetailAccountMismatch");
            await transaction.CommitAsync(ct);
            return linkedPerson.Id;
        }
        var matches = await db.TblPersons.Where(p => p.Shopid == customer.ShopId
                && (p.TenantId == customer.TenantId || p.TenantId == null || p.TenantId == "")
                && ((mobile != null && p.Mobilenumber == mobile)
                    || (identifier != null && p.Identifiernumber == identifier)))
            .ToListAsync(ct);
        if (matches.Count > 1) throw new InvalidOperationException("AmbiguousAccountingCustomer");
        if (matches.Count == 1)
        {
            var person = matches[0];
            // A phone number alone is not proof that a marketplace account owns this accounting person.
            if (identifier is null || person.Identifiernumber != identifier)
                throw new InvalidOperationException("CustomerIdentityNeedsReview");
            if (await db.IntegrationCustomerMappings.AnyAsync(x => x.ShopId == customer.ShopId
                && x.TenantId == customer.TenantId && x.PersonId == person.Id, ct))
                throw new InvalidOperationException("AccountingCustomerAlreadyLinkedToAnotherBasalamUser");
            if (await db.IntegrationCustomerMappings.AnyAsync(x => x.ShopId == customer.ShopId
                && x.TenantId == customer.TenantId && x.PersonId == person.Id, ct))
                throw new InvalidOperationException("AccountingCustomerAlreadyLinkedToAnotherBasalamUser");
            if (!person.Isenabled || (mobile != null && person.Mobilenumber != null && person.Mobilenumber != mobile)
                || (identifier != null && person.Identifiernumber != null && person.Identifiernumber != identifier))
                throw new InvalidOperationException("ConflictingAccountingCustomer");
            if (person.Type != policy.PersonType || !HasRole(person.Roles, policy.CustomerRole))
                throw new InvalidOperationException("AccountingCustomerRoleNeedsReview");
            var accountValid = await db.TblDetailaccounts.AnyAsync(a => a.Detailaccountid == person.Detailaccountid
                && a.Shopid == customer.ShopId && (a.TenantId == customer.TenantId || a.TenantId == null || a.TenantId == ""), ct);
            if (!accountValid) throw new InvalidOperationException("AccountingCustomerDetailAccountMismatch");
            db.IntegrationCustomerMappings.Add(new IntegrationCustomerMapping
            {
                ShopId = customer.ShopId, TenantId = customer.TenantId,
                BasalamUserId = customer.BasalamUserId, PersonId = person.Id
            });
            await db.SaveChangesAsync(ct);
            await transaction.CommitAsync(ct);
            return person.Id;
        }

        var account = new SqlTblDetailaccount
        {
            Shopid = customer.ShopId, TenantId = customer.TenantId,
            Entitytype = policy.DetailAccountEntityType, Name = customer.Name.Trim()
        };
        db.TblDetailaccounts.Add(account);
        await db.SaveChangesAsync(ct);
        var personNew = new SqlTblPerson
        {
            Shopid = customer.ShopId, TenantId = customer.TenantId, Detailaccountid = account.Detailaccountid,
            Name = customer.Name.Trim()[..Math.Min(50, customer.Name.Trim().Length)],
            Nickname = customer.Name.Trim(), Type = policy.PersonType.Value, Roles = policy.CustomerRole,
            Isenabled = true, Mobilenumber = mobile, Identifiernumber = identifier
        };
        db.TblPersons.Add(personNew);
        await db.SaveChangesAsync(ct);
        account.Referenceid = personNew.Id;
        db.IntegrationCustomerMappings.Add(new IntegrationCustomerMapping
        {
            ShopId = customer.ShopId, TenantId = customer.TenantId,
            BasalamUserId = customer.BasalamUserId, PersonId = personNew.Id
        });
        await db.SaveChangesAsync(ct);
        await transaction.CommitAsync(ct);
        return personNew.Id;
    }

    private static bool HasRole(string? roles, string role) => roles?.Split(',', StringSplitOptions.TrimEntries)
        .Contains(role, StringComparer.OrdinalIgnoreCase) == true;
}

public sealed class IntegrationCustomerMapping
{
    public long Id { get; set; }
    public int ShopId { get; set; }
    public string TenantId { get; set; } = null!;
    public string BasalamUserId { get; set; } = null!;
    public int PersonId { get; set; }
}
