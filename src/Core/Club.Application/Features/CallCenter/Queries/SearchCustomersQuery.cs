namespace Hyper.Application.Features.CallCenter.Queries;

/// <summary>
/// جستجوی مشتریان برای کال سنتر
/// </summary>
public record SearchCustomersQuery : IRequest<SearchCustomersQueryResponse>
{
    public string? Search { get; init; }
    public string? MobileNo { get; init; }
    public long? NationalCode { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}

public record SearchCustomersQueryResponse
{
    public PaginatedList<CustomerSearchResultDto> Customers { get; init; } = null!;
}

public record CustomerSearchResultDto
{
    public int CustomerId { get; init; }
    public int CustomerTenantId { get; init; }
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public string FullName { get; init; } = null!;
    public string? MobileNo { get; init; }
    public long? NationalCode { get; init; }
    public DateTime? BirthDate { get; init; }
    public int TenantId { get; init; }
    public DateTime? JoinDate { get; init; }
    public DateTime? LastInteractionDate { get; init; }
    public int? TotalInteractions { get; init; }
    public bool IsActive { get; init; }
}

internal sealed class SearchCustomersQueryHandler(
    IQueryRepository<CustomerTenant, int> customerTenantRepository
) : IRequestHandler<SearchCustomersQuery, SearchCustomersQueryResponse>
{
    public async Task<SearchCustomersQueryResponse> Handle(
        SearchCustomersQuery request,
        CancellationToken cancellationToken)
    {
        // چک کردن شماره دمو
        const string DemoMobileNumber = "09123456789";
        if (!string.IsNullOrWhiteSpace(request.MobileNo) && 
            request.MobileNo.Trim() == DemoMobileNumber)
        {
            // برگرداندن مشتری دمو
            var demoCustomer = new CustomerSearchResultDto
            {
                CustomerId = -1,
                CustomerTenantId = -1, // شناسه دمو
                FirstName = "علی",
                LastName = "احمدی",
                FullName = "علی احمدی",
                MobileNo = DemoMobileNumber,
                NationalCode = 1234567890,
                BirthDate = new DateTime(1990, 1, 1),
                TenantId = 1,
                JoinDate = DateTime.UtcNow.AddMonths(-6),
                LastInteractionDate = DateTime.UtcNow.AddDays(-2),
                TotalInteractions = 15,
                IsActive = true
            };

            return new SearchCustomersQueryResponse
            {
                Customers = new PaginatedList<CustomerSearchResultDto>(
                    [demoCustomer],
                    1,
                    request.PageNumber,
                    request.PageSize)
            };
        }

        var query = customerTenantRepository
            .Query()
            .Include(x => x.Customer)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.MobileNo))
        {
            query = query.Where(x => x.Customer.MobileNo != null && x.Customer.MobileNo.Contains(request.MobileNo));
        }

        if (request.NationalCode.HasValue)
        {
            query = query.Where(x => x.Customer.NationalCode == request.NationalCode.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim();
            query = query.Where(x =>
                (x.Customer.FirstName != null && x.Customer.FirstName.Contains(search)) ||
                (x.Customer.LastName != null && x.Customer.LastName.Contains(search)) ||
                (x.Customer.MobileNo != null && x.Customer.MobileNo.Contains(search)) ||
                (x.Customer.NationalCode.HasValue && x.Customer.NationalCode.Value.ToString().Contains(search)));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var customers = await query
            .OrderByDescending(x => x.LastInteractionDate ?? x.JoinDate)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(x => new CustomerSearchResultDto
            {
                CustomerId = x.CustomerId,
                CustomerTenantId = x.Id,
                FirstName = x.Customer.FirstName,
                LastName = x.Customer.LastName,
                FullName = (x.Customer.FirstName ?? "") + " " + (x.Customer.LastName ?? ""),
                MobileNo = x.Customer.MobileNo,
                NationalCode = x.Customer.NationalCode,
                BirthDate = x.Customer.BirthDate,
                TenantId = x.TenantId,
                JoinDate = x.JoinDate,
                LastInteractionDate = x.LastInteractionDate,
                TotalInteractions = x.TotalInteractions,
                IsActive = x.IsActive
            })
            .ToListAsync(cancellationToken);

        return new SearchCustomersQueryResponse
        {
            Customers = new PaginatedList<CustomerSearchResultDto>(
                customers,
                totalCount,
                request.PageNumber,
                request.PageSize)
        };
    }
}

