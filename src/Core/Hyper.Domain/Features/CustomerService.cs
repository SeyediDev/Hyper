namespace Hyper.Domain.Features;

public interface ICustomerService
{
    Task<Customer?> GetCustomer(string nationalId, bool createIfNotExists, Dictionary<string, string>? parameters, CancellationToken cancellationToken);
    Task<Customer?> GetCustomerByMobile(string mobile, bool createIfNotExists, Dictionary<string, string>? parameters, CancellationToken cancellationToken);
    Task<IEnumerable<CustomerTransaction>> GetCustomerPointBalances(int customerId, int? tenantId, CancellationToken cancellationToken);
    Task<IEnumerable<CustomerPointLevel>> GetCustomerPointLevels(int customerId, int? tenantId, CancellationToken cancellationToken);
    Task<long> GetPointBalanceAsync(int tenantId, int pointId, int customerId, CancellationToken cancellationToken);
    [Telemetry]
    Task SaveCustomerParameter(Customer customer, CustomerTenant customerTenant, int customerParameterId, string parameterValue, long eventLogId, CancellationToken cancellationToken);
}

internal class CustomerService(
    IQueryRepository<CustomerTransaction, long> customerTransactionRepo,
    IQueryRepository<CustomerPointLevel, int> customerPointLevelRepo,
    IQueryRepository<CustomerTenant, int> customerTenantQueryRepo,
    ICommandRepository<Customer, int> customerCmdRepo,
    ICommandRepository<CustomerParameterValue, int> customerParameterValueCmdRepo,
    ICommandRepository<CustomerTransaction, long> customerTransactionCmdRepo
    ) : ICustomerService
{
    public async Task<Customer?> GetCustomerByMobile(string mobile, bool createIfNotExists,
        Dictionary<string, string>? parameters, CancellationToken cancellationToken)
    {
        string? firstName = null;
        string? lastName = null;
        long? nationalCode = null;
        DateTime? birthDate = null;

        if (parameters != null)
        {
            _ = parameters.TryGetValue(CustomerReservedParameter.firstName.ToString(), out firstName);
            _ = parameters.TryGetValue(CustomerReservedParameter.lastName.ToString(), out lastName);
            _ = parameters.TryGetValue(CustomerReservedParameter.nationalCode.ToString(), out string? sNationalCode);
            _ = parameters.TryGetValue(CustomerReservedParameter.birthDate.ToString(), out string? sBirthDate);
            nationalCode = sNationalCode?.ToNullableInt64();
            birthDate = sBirthDate?.ToDateTimeOrDefault();
        }

        Customer? customerRecord = await customerCmdRepo.FirstOrDefaultAsync(x => x.MobileNo == mobile, cancellationToken);
        if (customerRecord == null)
        {
            if (createIfNotExists)
            {
                customerRecord = new()
                {
                    MobileNo = mobile,
                    FirstName = firstName ?? mobile,
                    LastName = lastName ?? mobile,
                    NationalCode = nationalCode,
                    BirthDate = birthDate
                };
                customerCmdRepo.Add(customerRecord);
                _ = await customerCmdRepo.UnitOfWork.SaveChangesAsync(cancellationToken);
            }
        }
        else
        {
            bool changed = false;
            if (firstName != null && firstName != customerRecord.FirstName)
            {
                customerRecord.FirstName = firstName;
                changed = true;
            }

            if (lastName != null && lastName != customerRecord.LastName)
            {
                customerRecord.LastName = lastName;
                changed = true;
            }

            if (nationalCode != null && nationalCode != customerRecord.NationalCode)
            {
                customerRecord.NationalCode = nationalCode;
                changed = true;
            }

            if (birthDate != null && birthDate != customerRecord.BirthDate)
            {
                customerRecord.BirthDate = birthDate;
                changed = true;
            }
            if (changed)
            {
                customerCmdRepo.Update(customerRecord);
                _ = await customerCmdRepo.UnitOfWork.SaveChangesAsync(cancellationToken);
            }
        }

        return customerRecord;
    }

    public async Task<Customer?> GetCustomer(string nationalId, bool createIfNotExists, 
        Dictionary<string, string>? parameters, CancellationToken cancellationToken)
    {
        long customerNationalCode = nationalId.ToInt64OrDefault();
        string? firstName = customerNationalCode.ToString().PadLeft(10, '0');
        string? lastName = customerNationalCode.ToString().PadLeft(10, '0');
        string? mobileNo = null;
        DateTime? birthDate = null;
        Customer? customerRecord = await customerCmdRepo.FirstOrDefaultAsync(x => x.NationalCode == customerNationalCode, cancellationToken);
        if (customerRecord == null)
        {
            if (createIfNotExists)
            {
                if (parameters != null)
                {
                    _ = parameters.TryGetValue(CustomerReservedParameter.firstName.ToString(), out firstName);
                    _ = parameters.TryGetValue(CustomerReservedParameter.lastName.ToString(), out lastName);
                    _ = parameters.TryGetValue(CustomerReservedParameter.mobileNo.ToString(), out mobileNo);
                    _ = parameters.TryGetValue(CustomerReservedParameter.birthDate.ToString(), out string? sBirthDate);
                    birthDate = sBirthDate?.ToDateTimeOrDefault();
                }
                customerRecord = new()
                {
                    NationalCode = customerNationalCode,
                    FirstName = firstName,
                    LastName = lastName,
                    MobileNo = mobileNo,
                    BirthDate = birthDate
                };
                customerCmdRepo.Add(customerRecord);
                _ = await customerCmdRepo.UnitOfWork.SaveChangesAsync(cancellationToken);
            }
        }
        else
        {
            bool changed = false;
            if (firstName != null && firstName != customerRecord.FirstName)
            {
                customerRecord.FirstName = firstName;
                changed = true;
            }

            if (lastName != null && lastName != customerRecord.LastName)
            {
                customerRecord.LastName = lastName;
                changed = true;
            }

            if (mobileNo != null && mobileNo != customerRecord.MobileNo)
            {
                customerRecord.BirthDate = birthDate;
                changed = true;
            }

            if (birthDate != null && birthDate != customerRecord.BirthDate)
            {
                customerRecord.BirthDate = birthDate;
                changed = true;
            }
            if (changed)
            {
                customerCmdRepo.Update(customerRecord);
                _ = await customerCmdRepo.UnitOfWork.SaveChangesAsync(cancellationToken);
            }
        }

        return customerRecord;
    }

    public async Task<IEnumerable<CustomerTransaction>> GetCustomerPointBalances(int customerId, int? tenantId, CancellationToken cancellationToken)
    {
        if (tenantId is null)
        {
            return await customerTransactionRepo.GetAllAsync(
                cancellationToken,
                x => x.CustomerTenant.CustomerId == customerId);
        }

        CustomerTenant? customerTenant = await customerTenantQueryRepo.FirstOrDefaultAsync(
            x => x.CustomerId == customerId && x.TenantId == tenantId.Value, cancellationToken);

        if (customerTenant == null)
        {
            return Enumerable.Empty<CustomerTransaction>();
        }

        return await customerTransactionRepo.GetAllAsync(
            cancellationToken,
            x => x.CustomerTenantId == customerTenant.Id && x.TenantId == tenantId);
    }

    public async Task<IEnumerable<CustomerPointLevel>> GetCustomerPointLevels(int customerId, int? tenantId, CancellationToken cancellationToken)
    {
        if (tenantId is null)
        {
            return await customerPointLevelRepo.GetAllWithIncludeAsync(
                x => x.PointLevel,
                cancellationToken,
                x => x.CustomerTenant.CustomerId == customerId);
        }

        CustomerTenant? customerTenant = await customerTenantQueryRepo.FirstOrDefaultAsync(
            x => x.CustomerId == customerId && x.TenantId == tenantId.Value, cancellationToken);

        if (customerTenant == null)
        {
            return Enumerable.Empty<CustomerPointLevel>();
        }

        return await customerPointLevelRepo.GetAllWithIncludeAsync(
            x => x.PointLevel,
            cancellationToken,
            x => x.CustomerTenantId == customerTenant.Id);
    }

    public async Task<long> GetPointBalanceAsync(int tenantId, int pointId, int customerId, CancellationToken cancellationToken)
    {
        CustomerTenant? customerTenant = await customerTenantQueryRepo.FirstOrDefaultAsync(
            x => x.CustomerId == customerId && x.TenantId == tenantId, cancellationToken);

        if (customerTenant == null)
        {
            return 0;
        }

        CustomerTransaction? customerTransaction =
            await customerTransactionCmdRepo.FirstOrDefaultAsync(
                    x => x.TenantId == tenantId &&
                    x.PointId == pointId &&
                    x.CustomerTenantId == customerTenant.Id, cancellationToken);
        return customerTransaction?.Balance ?? 0;
    }

    public async Task SaveCustomerParameter(Customer customer, CustomerTenant customerTenant, int customerParameterId, 
        string parameterValue, long eventLogId, CancellationToken cancellationToken)
    {
        CustomerParameterValue? customerParameterValue = await customerParameterValueCmdRepo.FirstOrDefaultAsync(
            x => x.CustomerTenantId == customerTenant.Id &&
            x.ParameterId == customerParameterId, cancellationToken);
        if (customerParameterValue == null)
        {
            customerParameterValue = new()
            {
                CustomerTenantId = customerTenant.Id,
                ParameterId = customerParameterId,
                Value = parameterValue,
                EventLogId = eventLogId
            };
            customerParameterValueCmdRepo.Add(customerParameterValue);
        }
        else
        {
            customerParameterValue.Value = parameterValue;
            customerParameterValue.EventLogId = eventLogId;
        }
        _ = await customerParameterValueCmdRepo.UnitOfWork.SaveChangesAsync(cancellationToken);
    }
}
