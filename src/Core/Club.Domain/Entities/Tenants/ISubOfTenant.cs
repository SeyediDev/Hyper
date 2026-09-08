namespace Hyper.Domain.Entities.Tenants;

public interface ISubOfTenant
{
    int TenantId { get; set; }
    Tenant Tenant { get; set; }
}
