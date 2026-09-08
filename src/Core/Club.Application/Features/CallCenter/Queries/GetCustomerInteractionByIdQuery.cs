namespace Hyper.Application.Features.CallCenter.Queries;

/// <summary>
/// دریافت جزئیات یک تعامل
/// </summary>
public record GetCustomerInteractionByIdQuery(long InteractionId) : IRequest<CustomerInteractionDetailDto?>;

public record CustomerInteractionDetailDto
{
    public long Id { get; init; }
    public int CustomerTenantId { get; init; }
    public int CustomerId { get; init; }
    public string CustomerName { get; init; } = null!;
    public string CustomerMobile { get; init; } = null!;
    public long? CustomerNationalCode { get; init; }
    public DateTime? CustomerBirthDate { get; init; }
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
    public long? RelatedInteractionId { get; init; }
    public List<InteractionFollowUpDto> FollowUps { get; init; } = [];
    public List<InteractionAttachmentDto> Attachments { get; init; } = [];
}

public record InteractionFollowUpDto
{
    public long Id { get; init; }
    public UserId? AgentUserId { get; init; }
    public string? AgentName { get; init; }
    public DateTime FollowUpDate { get; init; }
    public string? Notes { get; init; }
    public string? Result { get; init; }
    public bool IsCompleted { get; init; }
    public DateTime? CompletedDate { get; init; }
}

public record InteractionAttachmentDto
{
    public long Id { get; init; }
    public int DocumentId { get; init; }
    public string? Title { get; init; }
    public string? Description { get; init; }
    public string? AttachmentType { get; init; }
    public string? DocumentUrl { get; init; }
}

internal sealed class GetCustomerInteractionByIdQueryHandler(
    IQueryRepository<CustomerInteraction, long> interactionRepository
) : IRequestHandler<GetCustomerInteractionByIdQuery, CustomerInteractionDetailDto?>
{
    public async Task<CustomerInteractionDetailDto?> Handle(
        GetCustomerInteractionByIdQuery request,
        CancellationToken cancellationToken)
    {
        var interaction = await interactionRepository
            .Query()
            .Include(x => x.CustomerTenant)
                .ThenInclude(x => x.Customer)
            .Include(x => x.InteractionType)
            .Include(x => x.AgentUser)
            .Include(x => x.InteractionOutcome)
            .Include(x => x.FollowUps)
                .ThenInclude(x => x.AgentUser)
            .Include(x => x.Attachments)
                .ThenInclude(x => x.Document)
            .FirstOrDefaultAsync(x => x.Id == request.InteractionId, cancellationToken);

        if (interaction == null)
            return null;

        return new CustomerInteractionDetailDto
        {
            Id = interaction.Id,
            CustomerTenantId = interaction.CustomerTenantId,
            CustomerId = interaction.CustomerTenant.CustomerId,
            CustomerName = (interaction.CustomerTenant.Customer.FirstName ?? "") + " " + (interaction.CustomerTenant.Customer.LastName ?? ""),
            CustomerMobile = interaction.CustomerTenant.Customer.MobileNo ?? "",
            CustomerNationalCode = interaction.CustomerTenant.Customer.NationalCode,
            CustomerBirthDate = interaction.CustomerTenant.Customer.BirthDate,
            TenantId = interaction.TenantId,
            InteractionTypeId = interaction.InteractionTypeId,
            InteractionTypeTitle = interaction.InteractionType.Title,
            InteractionTypeIcon = interaction.InteractionType.Icon,
            InteractionTypeColor = interaction.InteractionType.Color,
            AgentUserId = interaction.AgentUserId,
            AgentName = interaction.AgentUser != null ? (interaction.AgentUser.FirstName ?? "") + " " + (interaction.AgentUser.LastName ?? "") : null,
            Title = interaction.Title,
            Description = interaction.Description,
            StartTime = interaction.StartTime,
            EndTime = interaction.EndTime,
            DurationMinutes = interaction.DurationMinutes,
            InteractionOutcomeId = interaction.InteractionOutcomeId,
            InteractionOutcomeTitle = interaction.InteractionOutcome != null ? interaction.InteractionOutcome.Title : null,
            InteractionOutcomeColor = interaction.InteractionOutcome != null ? interaction.InteractionOutcome.Color : null,
            OutcomeNotes = interaction.OutcomeNotes,
            SatisfactionScore = interaction.SatisfactionScore,
            CustomerFeedback = interaction.CustomerFeedback,
            RequiresFollowUp = interaction.RequiresFollowUp,
            FollowUpDate = interaction.FollowUpDate,
            Status = interaction.Status,
            Priority = interaction.Priority,
            ContactPhone = interaction.ContactPhone,
            ContactEmail = interaction.ContactEmail,
            TicketId = interaction.TicketId,
            CallId = interaction.CallId,
            RelatedInteractionId = interaction.RelatedInteractionId,
            FollowUps = interaction.FollowUps.Select(f => new InteractionFollowUpDto
            {
                Id = f.Id,
                AgentUserId = f.AgentUserId,
                AgentName = f.AgentUser != null ? (f.AgentUser.FirstName ?? "") + " " + (f.AgentUser.LastName ?? "") : null,
                FollowUpDate = f.FollowUpDate,
                Notes = f.Notes,
                Result = f.Result,
                IsCompleted = f.IsCompleted,
                CompletedDate = f.CompletedDate
            }).ToList(),
            Attachments = interaction.Attachments.Select(a => new InteractionAttachmentDto
            {
                Id = a.Id,
                DocumentId = a.DocumentId,
                Title = a.Title,
                Description = a.Description,
                AttachmentType = a.AttachmentType,
                DocumentUrl = null // TODO: Build document URL
            }).ToList()
        };
    }
}

