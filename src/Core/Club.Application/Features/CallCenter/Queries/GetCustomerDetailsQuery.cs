namespace Hyper.Application.Features.CallCenter.Queries;

/// <summary>
/// دریافت اطلاعات کامل مشتری با تعاملات
/// </summary>
public record GetCustomerDetailsQuery : IRequest<CustomerDetailsDto?>
{
    public int CustomerTenantId { get; init; }
    public bool IncludeInteractions { get; init; } = true;
    public int? InteractionsPageSize { get; init; } = 10;
}

public record CustomerDetailsDto
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
    
    // RFM Analysis
    public int? RecencyScore { get; init; }
    public int? FrequencyScore { get; init; }
    public int? MonetaryScore { get; init; }
    public decimal? CustomerLifetimeValue { get; init; }
    public decimal? EngagementScore { get; init; }
    public decimal? LoyaltyScore { get; init; }
    
    // Points
    public long? CurrentPointsBalance { get; init; }
    public long? TotalPointsEarned { get; init; }
    public long? TotalPointsRedeemed { get; init; }
    
    // Recent Interactions
    public List<CustomerInteractionDto> RecentInteractions { get; init; } = [];
}

internal sealed class GetCustomerDetailsQueryHandler(
    IQueryRepository<CustomerTenant, int> customerTenantRepository,
    IQueryRepository<CustomerInteraction, long> interactionRepository
) : IRequestHandler<GetCustomerDetailsQuery, CustomerDetailsDto?>
{
    public async Task<CustomerDetailsDto?> Handle(
        GetCustomerDetailsQuery request,
        CancellationToken cancellationToken)
    {
        // چک کردن مشتری دمو
        const int DemoCustomerTenantId = -1;
        if (request.CustomerTenantId == DemoCustomerTenantId)
        {
            // برگرداندن داده‌های دمو
            return new CustomerDetailsDto
            {
                CustomerId = -1,
                CustomerTenantId = DemoCustomerTenantId,
                FirstName = "علی",
                LastName = "احمدی",
                FullName = "علی احمدی",
                MobileNo = "09123456789",
                NationalCode = 1234567890,
                BirthDate = new DateTime(1990, 1, 1),
                TenantId = 1,
                JoinDate = DateTime.UtcNow.AddMonths(-6),
                LastInteractionDate = DateTime.UtcNow.AddDays(-2),
                TotalInteractions = 15,
                IsActive = true,
                RecencyScore = 5,
                FrequencyScore = 4,
                MonetaryScore = 5,
                CustomerLifetimeValue = 2500000,
                EngagementScore = 85.5m,
                LoyaltyScore = 90.0m,
                CurrentPointsBalance = 75,
                TotalPointsEarned = 75,
                TotalPointsRedeemed = 0,
                RecentInteractions = []
            };
        }

        var customerTenant = await customerTenantRepository
            .Query()
            .Include(x => x.Customer)
            .FirstOrDefaultAsync(x => x.Id == request.CustomerTenantId, cancellationToken);

        if (customerTenant == null)
            return null;

        List<CustomerInteractionDto> recentInteractions = [];

        if (request.IncludeInteractions)
        {
            var interactions = await interactionRepository
                .Query()
                .Include(x => x.InteractionType)
                .Include(x => x.AgentUser)
                .Include(x => x.InteractionOutcome)
                .Where(x => x.CustomerTenantId == request.CustomerTenantId)
                .OrderByDescending(x => x.StartTime)
                .Take(request.InteractionsPageSize ?? 10)
                .Select(x => new CustomerInteractionDto
                {
                    Id = x.Id,
                    CustomerTenantId = x.CustomerTenantId,
                    CustomerId = customerTenant.CustomerId,
                    CustomerName = (customerTenant.Customer.FirstName ?? "") + " " + (customerTenant.Customer.LastName ?? ""),
                    CustomerMobile = customerTenant.Customer.MobileNo ?? "",
                    TenantId = x.TenantId,
                    InteractionTypeId = x.InteractionTypeId,
                    InteractionTypeTitle = x.InteractionType.Title,
                    InteractionTypeIcon = x.InteractionType.Icon,
                    InteractionTypeColor = x.InteractionType.Color,
                    AgentUserId = x.AgentUserId,
                    AgentName = x.AgentUser != null ? (x.AgentUser.FirstName ?? "") + " " + (x.AgentUser.LastName ?? "") : null,
                    Title = x.Title,
                    Description = x.Description,
                    StartTime = x.StartTime,
                    EndTime = x.EndTime,
                    DurationMinutes = x.DurationMinutes,
                    InteractionOutcomeId = x.InteractionOutcomeId,
                    InteractionOutcomeTitle = x.InteractionOutcome != null ? x.InteractionOutcome.Title : null,
                    InteractionOutcomeColor = x.InteractionOutcome != null ? x.InteractionOutcome.Color : null,
                    OutcomeNotes = x.OutcomeNotes,
                    SatisfactionScore = x.SatisfactionScore,
                    CustomerFeedback = x.CustomerFeedback,
                    RequiresFollowUp = x.RequiresFollowUp,
                    FollowUpDate = x.FollowUpDate,
                    Status = x.Status,
                    Priority = x.Priority,
                    ContactPhone = x.ContactPhone,
                    ContactEmail = x.ContactEmail,
                    TicketId = x.TicketId,
                    CallId = x.CallId
                })
                .ToListAsync(cancellationToken);

            recentInteractions = interactions;
        }

        return new CustomerDetailsDto
        {
            CustomerId = customerTenant.CustomerId,
            CustomerTenantId = customerTenant.Id,
            FirstName = customerTenant.Customer.FirstName,
            LastName = customerTenant.Customer.LastName,
            FullName = (customerTenant.Customer.FirstName ?? "") + " " + (customerTenant.Customer.LastName ?? ""),
            MobileNo = customerTenant.Customer.MobileNo,
            NationalCode = customerTenant.Customer.NationalCode,
            BirthDate = customerTenant.Customer.BirthDate,
            TenantId = customerTenant.TenantId,
            JoinDate = customerTenant.JoinDate,
            LastInteractionDate = customerTenant.LastInteractionDate,
            TotalInteractions = customerTenant.TotalInteractions,
            IsActive = customerTenant.IsActive,
            RecencyScore = customerTenant.RecencyScore,
            FrequencyScore = customerTenant.FrequencyScore,
            MonetaryScore = customerTenant.MonetaryScore,
            CustomerLifetimeValue = customerTenant.CustomerLifetimeValue,
            EngagementScore = customerTenant.EngagementScore,
            LoyaltyScore = customerTenant.LoyaltyScore,
            CurrentPointsBalance = customerTenant.CurrentPointsBalance,
            TotalPointsEarned = customerTenant.TotalPointsEarned,
            TotalPointsRedeemed = customerTenant.TotalPointsRedeemed,
            RecentInteractions = recentInteractions
        };
    }
}

