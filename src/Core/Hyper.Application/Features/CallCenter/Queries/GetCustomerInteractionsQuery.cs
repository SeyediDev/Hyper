namespace Hyper.Application.Features.CallCenter.Queries;

/// <summary>
/// دریافت لیست تعاملات مشتری
/// </summary>
public record GetCustomerInteractionsQuery : IRequest<GetCustomerInteractionsQueryResponse>
{
    public int? CustomerTenantId { get; init; }
    public int? CustomerId { get; init; }
    public UserId? AgentUserId { get; init; }
    public int? InteractionTypeId { get; init; }
    public InteractionStatus? Status { get; init; }
    public DateTime? StartDateFrom { get; init; }
    public DateTime? StartDateTo { get; init; }
    public int PageNumber { get; init; } = 1;
    public int PageSize { get; init; } = 20;
    public string? Search { get; init; }
}

public record GetCustomerInteractionsQueryResponse
{
    public PaginatedList<CustomerInteractionDto> Interactions { get; init; } = null!;
}

public record CustomerInteractionDto
{
    public long Id { get; init; }
    public int CustomerTenantId { get; init; }
    public int CustomerId { get; init; }
    public string CustomerName { get; init; } = null!;
    public string CustomerMobile { get; init; } = null!;
    public int TenantId { get; init; }
    public int InteractionTypeId { get; init; }
    public string InteractionTypeTitle { get; init; } = null!;
    public string? InteractionTypeIcon { get; init; }
    public string? InteractionTypeColor { get; init; }
    public UserId? AgentUserId { get; init; }
    public string? AgentName { get; init; }
    public string? Title { get; init; }
    public string? Description { get; init; }
    public DateTime StartTime { get; init; }
    public DateTime? EndTime { get; init; }
    public int? DurationMinutes { get; init; }
    public int? InteractionOutcomeId { get; init; }
    public string? InteractionOutcomeTitle { get; init; }
    public string? InteractionOutcomeColor { get; init; }
    public string? OutcomeNotes { get; init; }
    public int? SatisfactionScore { get; init; }
    public string? CustomerFeedback { get; init; }
    public bool RequiresFollowUp { get; init; }
    public DateTime? FollowUpDate { get; init; }
    public InteractionStatus Status { get; init; }
    public PriorityLevel Priority { get; init; }
    public string? ContactPhone { get; init; }
    public string? ContactEmail { get; init; }
    public string? TicketId { get; init; }
    public string? CallId { get; init; }
}

internal sealed class GetCustomerInteractionsQueryHandler(
    IQueryRepository<CustomerInteraction, long> interactionRepository
) : IRequestHandler<GetCustomerInteractionsQuery, GetCustomerInteractionsQueryResponse>
{
    public async Task<GetCustomerInteractionsQueryResponse> Handle(
        GetCustomerInteractionsQuery request,
        CancellationToken cancellationToken)
    {
        var query = interactionRepository
            .Query()
            .Include(x => x.CustomerTenant)
                .ThenInclude(x => x.Customer)
            .Include(x => x.InteractionType)
            .Include(x => x.AgentUser)
            .Include(x => x.InteractionOutcome)
            .AsQueryable();

        if (request.CustomerTenantId.HasValue)
            query = query.Where(x => x.CustomerTenantId == request.CustomerTenantId.Value);

        if (request.CustomerId.HasValue)
            query = query.Where(x => x.CustomerTenant.CustomerId == request.CustomerId.Value);

        if (request.AgentUserId.HasValue)
            query = query.Where(x => x.AgentUserId == request.AgentUserId.Value);

        if (request.InteractionTypeId.HasValue)
            query = query.Where(x => x.InteractionTypeId == request.InteractionTypeId.Value);

        if (request.Status.HasValue)
            query = query.Where(x => x.Status == request.Status.Value);

        if (request.StartDateFrom.HasValue)
            query = query.Where(x => x.StartTime >= request.StartDateFrom.Value);

        if (request.StartDateTo.HasValue)
            query = query.Where(x => x.StartTime <= request.StartDateTo.Value);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.Trim();
            query = query.Where(x =>
                (x.Title != null && x.Title.Contains(search)) ||
                (x.Description != null && x.Description.Contains(search)) ||
                (x.CustomerTenant.Customer.FirstName != null && x.CustomerTenant.Customer.FirstName.Contains(search)) ||
                (x.CustomerTenant.Customer.LastName != null && x.CustomerTenant.Customer.LastName.Contains(search)) ||
                (x.CustomerTenant.Customer.MobileNo != null && x.CustomerTenant.Customer.MobileNo.Contains(search)) ||
                (x.TicketId != null && x.TicketId.Contains(search)) ||
                (x.CallId != null && x.CallId.Contains(search)));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var interactions = await query
            .OrderByDescending(x => x.StartTime)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(x => new CustomerInteractionDto
            {
                Id = x.Id,
                CustomerTenantId = x.CustomerTenantId,
                CustomerId = x.CustomerTenant.CustomerId,
                CustomerName = (x.CustomerTenant.Customer.FirstName ?? "") + " " + (x.CustomerTenant.Customer.LastName ?? ""),
                CustomerMobile = x.CustomerTenant.Customer.MobileNo ?? "",
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

        return new GetCustomerInteractionsQueryResponse
        {
            Interactions = new PaginatedList<CustomerInteractionDto>(
                interactions,
                totalCount,
                request.PageNumber,
                request.PageSize)
        };
    }
}

