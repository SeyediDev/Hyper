namespace Hyper.Domain.Features.Customers;

public interface ICustomerTenantService
{
    [Telemetry]
    Task<CustomerTenant?> GetAsync(
        int tenantId, string mobile, CancellationToken cancellationToken);

    [Telemetry]
    Task<CustomerTenant> GetOrCreateAndSetReservedAttributesAsync(
        int tenantId, string mobile, AttributesValues? attributes, CancellationToken cancellationToken);

    [Telemetry]
    Task FetchReservedAttributesAsync(
        CustomerTenant customerTenant, AttributesValues attributes, CancellationToken cancellationToken);

    [Telemetry]
    Task<IEnumerable<CustomerTransaction>> GetPointBalancesAsync(
        int customerTenantId, bool extractAllPoints, CancellationToken cancellationToken);

    [Telemetry]
    Task<IEnumerable<CustomerPointLevel>> GetPointLevelsAsync(
        int customerTenantId, bool extractAllPoints, CancellationToken cancellationToken);

    [Telemetry]
    Task<long> GetPointBalanceAsync(
        int customerTenantId, int pointId, CancellationToken cancellationToken);
}

internal class CustomerTenantService(
      ICustomerService customerService
    , IQueryRepository<CustomerTransaction, long> customerTransactionQuery
    , IQueryRepository<CustomerPointLevel> customerPointLevelQuery
    , IQueryRepository<CustomerTenant> customerTenantQuery
    , IQueryRepository<Country> countryQuery
    , IQueryRepository<Province> provinceQuery
    , IQueryRepository<City> cityQuery
    , IQueryRepository<Point> pointQuery
    , IQueryRepository<PointLevel> pointLevelQuery
    , ICommandRepository<CustomerTenant> customerTenantCommand
    , ICommandRepository<CustomerTransaction, long> customerTransactionCommand
    ) : ICustomerTenantService
{
    public async Task<CustomerTenant?> GetAsync(
        int tenantId, string mobile, CancellationToken cancellationToken)
    {
        Customer? customer = await customerService.GetAsync(mobile, cancellationToken);
        CustomerTenant? customerTenant = customer == null ? null :
            await customerTenantQuery.FirstOrDefaultAsync(
            x => x.CustomerId == customer.Id && x.TenantId == tenantId, cancellationToken);
        return customerTenant;
    }
    public async Task<CustomerTenant> GetOrCreateAndSetReservedAttributesAsync(
        int tenantId, string mobile, AttributesValues? attributes, CancellationToken cancellationToken)
    {
        attributes ??= [];
        attributes.Add(CustomerReservedAttribute.MobileNo, mobile);
        Customer customer = await customerService.GetOrCreateAndSetReservedAttributesAsync(
            mobile, attributes, cancellationToken);
        CustomerTenant? customerTenant = await customerTenantQuery.FirstOrDefaultAsync(
            x => x.CustomerId == customer.Id && x.TenantId == tenantId, cancellationToken);

        var countryId = await GetCountryId(attributes, cancellationToken);
        var provinceId = await GetProvinceId(attributes, cancellationToken);
        var cityId = await GetCityId(attributes, cancellationToken);
        if (customerTenant == null)
        {
            customerTenant = new CustomerTenant()
            {
                CustomerId = customer.Id,
                TenantId = tenantId,
                JoinDate = DateTime.UtcNow,
                IsActive = true,
                LastName = customer.LastName,
                FirstName = customer.FirstName,
                CountryId = countryId,
                ProvinceId = provinceId,
                CityId = cityId
            };
            customerTenantCommand.Add(customerTenant);
            await customerTenantCommand.UnitOfWork.SaveChangesAsync(cancellationToken);
        }
        else
        {
            bool changed = false;
            if (!customerTenant.IsActive || !customerTenant.IsDeleted)
            {
                //TODO مشتری دوباره برگشته : هوورا . این را ثبت کن
                customerTenant.IsActive = true;
                customerTenant.LeaveDate = null;
                customerTenant.IsDeleted = false;
                customerTenant.ExpireDate = null;
                changed = true;
            }
            if (customer.FirstName == null && customer.FirstName != customerTenant.FirstName)
            {
                customerTenant.FirstName = customer.FirstName;
                changed = true;
            }
            if (customer.LastName == null && customer.LastName != customerTenant.LastName)
            {
                customerTenant.LastName = customer.LastName;
                changed = true;
            }
            if (countryId == null && countryId != customerTenant.CountryId)
            {
                customerTenant.CountryId = countryId;
                changed = true;
            }
            if (provinceId == null && provinceId != customerTenant.ProvinceId)
            {
                customerTenant.ProvinceId = provinceId;
                changed = true;
            }
            if (cityId == null && cityId != customerTenant.CityId)
            {
                customerTenant.CityId = cityId;
                changed = true;
            }
            if (changed)
            {
                customerTenantCommand.Update(customerTenant);
                _ = await customerTenantCommand.UnitOfWork.SaveChangesAsync(cancellationToken);
            }
        }
        await FetchReservedAttributesAsync(customerTenant, attributes!, cancellationToken);
        return customerTenant;
    }

    private async Task<int?> GetCountryId(AttributesValues? attributes, CancellationToken cancellationToken)
    {
        object? code = attributes.GetReservedAttribute(CustomerTenantReservedAttribute.Country);
        int? id = code?.ToNullableInt32();
        string? iso2 = code?.ToString();
        var item = await countryQuery.FirstOrDefaultAsync(x => x.Id == id || x.Iso2 == iso2, cancellationToken);
        return item?.Id;
    }

    private async Task<int?> GetProvinceId(AttributesValues? attributes, CancellationToken cancellationToken)
    {
        object? code = attributes.GetReservedAttribute(CustomerTenantReservedAttribute.Province);
        int? id = code?.ToNullableInt32();
        string? iso2 = code?.ToString();
        var item = await provinceQuery.FirstOrDefaultAsync(x => x.Id == id || x.Iso2 == iso2, cancellationToken);
        return item?.Id;
    }

    private async Task<int?> GetCityId(AttributesValues? attributes, CancellationToken cancellationToken)
    {
        object? code = attributes.GetReservedAttribute(CustomerTenantReservedAttribute.City);
        int? id = code?.ToNullableInt32();
        string? iso2 = code?.ToString();
        var item = await cityQuery.FirstOrDefaultAsync(x => x.Id == id || x.Iso2 == iso2, cancellationToken);
        return item?.Id;
    }

    public async Task<IEnumerable<CustomerTransaction>> GetPointBalancesAsync(
        int customerTenantId, bool extractAllPoints, CancellationToken cancellationToken)
    {
        var customerPoints = (await customerTransactionQuery.GetAllAsync(
            cancellationToken, x => x.CustomerTenantId == customerTenantId && !x.IsDeleted && !x.IsSpent)).ToList();
        if (extractAllPoints)
        {
            var points = await pointQuery.GetAllAsync(cancellationToken, x => !x.IsDeleted);
            foreach (var point in points)
            {
                if (!customerPoints.Any(x => x.PointId == point.Id))
                {
                    customerPoints.Add(new CustomerTransaction { PointId = point.Id, Point = point, CustomerTenantId = customerTenantId });
                }
            }
        }
        return customerPoints;
    }

    public async Task<IEnumerable<CustomerPointLevel>> GetPointLevelsAsync(
        int customerTenantId, bool extractAllPoints, CancellationToken cancellationToken)
    {
        var customerPointLevels = (await customerPointLevelQuery.GetAllWithIncludeAsync(
            x => x.PointLevel,
            cancellationToken, x => x.CustomerTenantId == customerTenantId && !x.IsDeleted)).ToList();
        if (extractAllPoints)
        {
            var points = await pointQuery.GetAllAsync(cancellationToken, x => !x.IsDeleted);
            var pointsLevels = await pointLevelQuery.GetAllAsync(cancellationToken, x => !x.IsDeleted);
            foreach (var point in points)
            {
                if (!customerPointLevels.Any(x => x.PointLevel.PointId == point.Id))
                {
                    var level = pointsLevels.Where(x => x.PointId == point.Id).OrderBy(x => x.Level).FirstOrDefault();
                    if (level != null)
                    {
                        customerPointLevels.Add(new CustomerPointLevel { PointLevelId = level.Id, PointLevel = level, CustomerTenantId = customerTenantId });
                    }
                }
            }
        }
        return customerPointLevels;
    }

    public async Task<long> GetPointBalanceAsync(int customerTenantId, int pointId, CancellationToken cancellationToken)
    {
        CustomerTransaction? customerTransaction =
            await customerTransactionCommand.FirstOrDefaultAsync(
                    x => x.PointId == pointId &&
                         x.CustomerTenantId == customerTenantId, cancellationToken);
        return customerTransaction?.Balance ?? 0;
    }

    public async Task FetchReservedAttributesAsync(
        CustomerTenant customerTenant, AttributesValues attributes, CancellationToken cancellationToken)
    {
        if (customerTenant == null)
            return;
        customerTenant.Customer ??= (await customerService.GetAsync(customerTenant.CustomerId, cancellationToken))!;
        if (customerTenant.Customer != null)
        {
            attributes.Add(CustomerReservedAttribute.FirstName, customerTenant.Customer.FirstName!);
            attributes.Add(CustomerReservedAttribute.LastName, customerTenant.Customer.LastName!);
            attributes.Add(CustomerReservedAttribute.MobileNo, customerTenant.Customer.MobileNo!);
            attributes.Add(CustomerReservedAttribute.NationalCode, customerTenant.Customer.NationalCode!);
            attributes.Add(CustomerReservedAttribute.BirthDate, customerTenant.Customer.BirthDate!);
            attributes.Add(CustomerReservedAttribute.Age, CalculateAge(customerTenant.Customer.BirthDate!));
        }
        attributes.Add(CustomerTenantReservedAttribute.Country, customerTenant.CountryId!);
        attributes.Add(CustomerTenantReservedAttribute.Province, customerTenant.ProvinceId!);
        attributes.Add(CustomerTenantReservedAttribute.City, customerTenant.CityId!);
        attributes.Add(CustomerTenantReservedAttribute.JoinDate, customerTenant.JoinDate!);
    }

    private static int CalculateAge(DateTime? birthDate)
    {
        if (!birthDate.HasValue) return 0;
        var today = DateTime.Today;
        var age = today.Year - birthDate.Value.Year;
        if (birthDate.Value.Date > today.AddYears(-age)) age--;
        return age;
    }
}