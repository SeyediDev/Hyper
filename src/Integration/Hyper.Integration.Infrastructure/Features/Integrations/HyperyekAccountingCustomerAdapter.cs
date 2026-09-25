using Hyper.Domain.Entities.Database;
using Hyper.Integration.Domain.Features.Integrations;
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

/// <summary>Infrastructure anti-corruption adapter for the Hyperyek accounting schema.</summary>
public sealed class HyperyekAccountingCustomerAdapter(
    HyperSqlServerContext db,
    IOptions<IntegrationCustomerOptions> options) : IIntegrationAccountingPort
{
    public async Task<bool> ValidateLinkedCustomerAsync(IntegrationCustomerIdentity customer, int personId,
        CancellationToken ct)
    {
        var policy = GetPolicy();
        var person = await db.TblPersons.SingleOrDefaultAsync(x => x.Id == personId
            && x.Shopid == customer.ShopId && x.TenantId == customer.TenantId && x.Isenabled, ct);
        if (person is null || person.Type != policy.PersonType || !HasRole(person.Roles, policy.CustomerRole)
            || (customer.Mobile != null && person.Mobilenumber != null && person.Mobilenumber != customer.Mobile)
            || (customer.IdentifierNumber != null && person.Identifiernumber != null
                && person.Identifiernumber != customer.IdentifierNumber))
            return false;
        return await db.TblDetailaccounts.AnyAsync(a => a.Detailaccountid == person.Detailaccountid
            && a.Shopid == customer.ShopId && a.TenantId == customer.TenantId, ct);
    }

    public async Task<int> ResolveOrCreateCustomerAsync(IntegrationCustomerIdentity customer,
        CancellationToken ct)
    {
        var policy = GetPolicy();
        await using var transaction = await db.Database.BeginTransactionAsync(IsolationLevel.Serializable, ct);
        var matches = await db.TblPersons.Where(p => p.Shopid == customer.ShopId
                && (p.TenantId == customer.TenantId || p.TenantId == null || p.TenantId == "")
                && ((customer.Mobile != null && p.Mobilenumber == customer.Mobile)
                    || (customer.IdentifierNumber != null && p.Identifiernumber == customer.IdentifierNumber)))
            .ToListAsync(ct);
        if (matches.Count > 1) throw new InvalidOperationException("AmbiguousAccountingCustomer");
        if (matches.Count == 1)
        {
            var person = matches[0];
            if (customer.IdentifierNumber is null || person.Identifiernumber != customer.IdentifierNumber)
                throw new InvalidOperationException("CustomerIdentityNeedsReview");
            if (!person.Isenabled || (customer.Mobile != null && person.Mobilenumber != null
                    && person.Mobilenumber != customer.Mobile)
                || (customer.IdentifierNumber != null && person.Identifiernumber != null
                    && person.Identifiernumber != customer.IdentifierNumber))
                throw new InvalidOperationException("ConflictingAccountingCustomer");
            if (person.Type != policy.PersonType || !HasRole(person.Roles, policy.CustomerRole))
                throw new InvalidOperationException("AccountingCustomerRoleNeedsReview");
            var accountValid = await db.TblDetailaccounts.AnyAsync(a => a.Detailaccountid == person.Detailaccountid
                && a.Shopid == customer.ShopId && (a.TenantId == customer.TenantId || a.TenantId == null || a.TenantId == ""), ct);
            if (!accountValid) throw new InvalidOperationException("AccountingCustomerDetailAccountMismatch");
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
            Nickname = customer.Name.Trim(), Type = policy.PersonType, Roles = policy.CustomerRole,
            Isenabled = true, Mobilenumber = customer.Mobile, Identifiernumber = customer.IdentifierNumber
        };
        db.TblPersons.Add(personNew);
        await db.SaveChangesAsync(ct);
        account.Referenceid = personNew.Id;
        await db.SaveChangesAsync(ct);
        await transaction.CommitAsync(ct);
        return personNew.Id;
    }

    private IntegrationCustomerPolicy GetPolicy()
    {
        var value = options.Value;
        if (value.PersonType is null || string.IsNullOrWhiteSpace(value.CustomerRole)
            || value.CustomerRole.Length > 20 || string.IsNullOrWhiteSpace(value.DetailAccountEntityType)
            || value.DetailAccountEntityType.Length > 50)
            throw new InvalidOperationException("AccountingCustomerConventionNotConfigured");
        return new IntegrationCustomerPolicy(value.PersonType.Value, value.CustomerRole, value.DetailAccountEntityType);
    }

    private static bool HasRole(string? roles, string role) => roles?.Split(',', StringSplitOptions.TrimEntries)
        .Contains(role, StringComparer.OrdinalIgnoreCase) == true;
}

internal sealed record IntegrationCustomerPolicy(byte PersonType, string CustomerRole, string DetailAccountEntityType);
