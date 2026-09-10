using Hyper.CustomerPortal.Application.Interfaces;

namespace Hyper.CustomerPortal.Application.Features.Auth.Queries;

public record GetProfileQuery : IRequest<GetProfileQueryResponse>;

public record GetProfileQueryResponse
{
    public required CustomerDto Customer { get; set; }
}

public class GetProfileQueryHandler(
    ICustomerService customerService,
    ICustomerRequesterUser requesterUser) : IRequestHandler<GetProfileQuery, GetProfileQueryResponse>
{
    public async Task<GetProfileQueryResponse> Handle(GetProfileQuery request, CancellationToken cancellationToken)
    {
        var customerId = requesterUser.CustomerId;
        
        var customer = await customerService.GetCustomerByIdAsync(customerId, cancellationToken);
        
        if (customer == null)
        {
            throw new InvalidOperationException("مشتری یافت نشد");
        }

        var tenantId = requesterUser.TenantId;
        var customerTenant = await customerService.GetCustomerTenantAsync(customer.Id, tenantId, cancellationToken);

        return new GetProfileQueryResponse
        {
            Customer = CustomerDtoFactory.Create(customer, customerTenant)
        };
    }
}

