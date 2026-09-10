namespace Hyper.Domain.Features.Customers;

public interface ICustomerService
{
    Task<Customer?> GetAsync(int customerId, CancellationToken cancellationToken);
    Task<Customer?> GetAsync(string mobile, CancellationToken cancellationToken);
    Task<Customer> GetOrCreateAndSetReservedAttributesAsync(
        string mobile, AttributesValues? attributes, CancellationToken cancellationToken);
}

internal class CustomerService(
      IQueryRepository<Customer, int> customerQuery
    , ICommandRepository<Customer, int> customerCommand
    ) : ICustomerService
{
    public async Task<Customer?> GetAsync(int customerId, CancellationToken cancellationToken)
    {
        Customer? customer = await customerQuery.GetByIdAsync(customerId, cancellationToken);
        return customer;
    }

    public async Task<Customer?> GetAsync(string mobile, CancellationToken cancellationToken)
    {
        Customer? customer = await customerQuery.FirstOrDefaultAsync(x=>x.MobileNo==mobile, cancellationToken);
        return customer;
    }

    public async Task<Customer> GetOrCreateAndSetReservedAttributesAsync(string mobile,
        AttributesValues? attributes, CancellationToken cancellationToken)
    {
        object? firstName = attributes.GetReservedAttribute(CustomerReservedAttribute.FirstName);
        object? lastName = attributes.GetReservedAttribute(CustomerReservedAttribute.LastName);
        long? nationalCode = attributes.GetReservedAttribute(CustomerReservedAttribute.NationalCode)?.ToNullableInt64();
        DateTime? birthDate = attributes.GetReservedAttribute(CustomerReservedAttribute.BirthDate)?.ToDateTimeOrDefault();

        Customer? customer = await customerCommand.FirstOrDefaultAsync(x => x.MobileNo == mobile, cancellationToken);
        if (customer == null)
        {
            customer = new()
            {
                MobileNo = mobile,
                FirstName = firstName?.ToString() ?? mobile,
                LastName = lastName?.ToString() ?? mobile,
                NationalCode = nationalCode,
                BirthDate = birthDate
            };
            customerCommand.Add(customer);
            _ = await customerCommand.UnitOfWork.SaveChangesAsync(cancellationToken);
        }
        else
        {
            bool changed = false;
            if (!customer.IsDeleted)
            {
                customer.IsDeleted = false;
                customer.ExpireDate = null;
                changed = true;
            }
            if (firstName != null && firstName?.ToString() != customer.FirstName)
            {
                customer.FirstName = firstName?.ToString();
                changed = true;
            }

            if (lastName != null && lastName?.ToString() != customer.LastName)
            {
                customer.LastName = lastName?.ToString();
                changed = true;
            }

            if (nationalCode != null && nationalCode != customer.NationalCode)
            {
                customer.NationalCode = nationalCode;
                changed = true;
            }

            if (birthDate != null && birthDate != customer.BirthDate)
            {
                customer.BirthDate = birthDate;
                changed = true;
            }
            if (changed)
            {
                customerCommand.Update(customer);
                _ = await customerCommand.UnitOfWork.SaveChangesAsync(cancellationToken);
            }
        }

        return customer;
    }
}