namespace Hyper.Application.Features.Surveys.Commands;

/// <summary>
/// شرکت در نظرسنجی و ثبت رأی
/// </summary>
public record ParticipateSurveyCommand : IRequest<ParticipateSurveyResponse>
{
    [Required]
    public int SurveyId { get; set; }

    [Required]
    public int CustomerTenantId { get; set; }

    [Required]
    public int SelectedItemId { get; set; }

    [MaxLength(1000)]
    public string? Comment { get; set; }
}

public record ParticipateSurveyResponse
{
    public int ParticipationId { get; set; }
    public bool IsCorrect { get; set; }
    public long PointsEarned { get; set; }
    public string Message { get; set; } = null!;
}

public class ParticipateSurveyCommandValidator : AbstractValidator<ParticipateSurveyCommand>
{
    public ParticipateSurveyCommandValidator(IMultiLingualService multiLingual)
    {
        RuleFor(x => x.SurveyId)
            .NotEmpty().WithMessage(multiLingual.GetMessage("SurveyIdIsRequired"));

        RuleFor(x => x.CustomerTenantId)
            .NotEmpty().WithMessage("رابطه مشتری-اکوسیستم الزامی است");

        RuleFor(x => x.SelectedItemId)
            .NotEmpty().WithMessage("گزینه انتخابی الزامی است");

        RuleFor(x => x.Comment)
            .MaximumLength(1000).WithMessage("نظر نباید بیشتر از 1000 کاراکتر باشد");
    }
}

public class ParticipateSurveyCommandHandler(
    IHyperUnitOfWorkCommand unitOfWork
) : IRequestHandler<ParticipateSurveyCommand, ParticipateSurveyResponse>
{
    public async Task<ParticipateSurveyResponse> Handle(ParticipateSurveyCommand request, CancellationToken cancellationToken)
    {
        // 1. بارگذاری نظرسنجی با گزینه‌ها
        var surveyRepo = unitOfWork.Repository<Survey, int>();
        var surveyEntity = await surveyRepo.FirstOrDefaultWithIncludeAsync(
            include: s => s.Items,
            predicate: s => s.Id == request.SurveyId,
            cancellationToken: cancellationToken);
        if (surveyEntity == null)
            throw new FluentValidation.ValidationException("نظرسنجی یافت نشد");

        // 2. بررسی فعال بودن نظرسنجی
        if (!surveyEntity.IsActive)
            throw new FluentValidation.ValidationException("این نظرسنجی غیرفعال است");

        // 3. بررسی تاریخ
        var now = DateTime.UtcNow;
        if (surveyEntity.StartDate.HasValue && now < surveyEntity.StartDate.Value)
            throw new FluentValidation.ValidationException("این نظرسنجی هنوز شروع نشده است");

        if (surveyEntity.EndDate.HasValue && now > surveyEntity.EndDate.Value)
            throw new FluentValidation.ValidationException("مهلت شرکت در این نظرسنجی به پایان رسیده است");

        // 4. بررسی گزینه انتخاب شده
        var selectedItem = surveyEntity.Items.FirstOrDefault(i => i.Id == request.SelectedItemId);
        if (selectedItem == null)
            throw new FluentValidation.ValidationException("گزینه انتخاب شده معتبر نیست");

        // 5. بررسی شرکت قبلی
        var participationRepo = unitOfWork.Repository<SurveyParticipation, int>();
        var customerTenantRepo = unitOfWork.Repository<CustomerTenant, int>();
        CustomerTenant? customerTenant = await customerTenantRepo.FirstOrDefaultAsync(
            ct => ct.Id == request.CustomerTenantId,
            cancellationToken) ?? throw new FluentValidation.ValidationException("رابطه مشتری-اکوسیستم یافت نشد");

        if (customerTenant.TenantId != surveyEntity.Promotion.TenantId)
            throw new FluentValidation.ValidationException("این مشتری به اکوسیستم نظرسنجی تعلق ندارد");

        var previousParticipation = await participationRepo.FirstOrDefaultAsync(
            p => p.SurveyId == request.SurveyId && p.CustomerTenantId == customerTenant.Id,
            cancellationToken);

        if (previousParticipation != null)
            throw new FluentValidation.ValidationException("شما قبلاً در این نظرسنجی شرکت کرده‌اید");

        // 6. محاسبه امتیاز
        long pointsEarned = 0;
        bool? isCorrect = null;

        if (surveyEntity.SurveyType == SurveyType.Contest)
        {
            // برای مسابقه
            isCorrect = selectedItem.IsCorrectAnswer;
            if (isCorrect.Value && surveyEntity.CorrectAnswerPoints.HasValue)
            {
                pointsEarned = surveyEntity.CorrectAnswerPoints.Value;
            }
        }
        else
        {
            // برای نظرسنجی عادی - امتیاز شرکت
            if (surveyEntity.ParticipationPoints.HasValue)
            {
                pointsEarned = surveyEntity.ParticipationPoints.Value;
            }
        }

        // 7. ثبت شرکت
        var participation = new SurveyParticipation
        {
            SurveyId = request.SurveyId,
            CustomerTenantId = customerTenant.Id,
            CustomerTenant = customerTenant,
            SelectedItemId = request.SelectedItemId,
            ParticipationDate = DateTime.UtcNow,
            IsCorrect = isCorrect,
            PointsEarned = pointsEarned,
            Comment = request.Comment
        };

        participationRepo.Add(participation);

        // 8. به‌روزرسانی آمار
        selectedItem.VoteCount++;
        surveyEntity.TotalParticipants++;

        unitOfWork.Repository<SurveyItem, int>().Update(selectedItem);
        surveyRepo.Update(surveyEntity);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        // 9. ثبت رویداد و اعطای امتیاز (اگر امتیازی وجود داشته باشد)
        if (pointsEarned > 0)
        {
            // TODO: باید Point مناسب را پیدا کنیم (باید در Survey تعریف شود)
            // فعلاً این قسمت را comment می‌کنیم تا بعداً کامل شود
            /*
            var eventRequest = new EventRequest(
                TriggerType.Event, 
                request.CustomerId.ToString(), 
                new Dictionary<string, string>
                {
                    ["SurveyId"] = request.SurveyId.ToString(),
                    ["Points"] = pointsEarned.ToString()
                }
            );
            await eventService.RecordEventAsync(eventRequest, cancellationToken);
            */
        }

        return new ParticipateSurveyResponse
        {
            ParticipationId = participation.Id,
            IsCorrect = isCorrect ?? false,
            PointsEarned = pointsEarned,
            Message = surveyEntity.SurveyType == SurveyType.Contest
                ? (isCorrect == true ? "پاسخ شما صحیح بود! " : "پاسخ شما نادرست بود. ") + $"امتیاز کسب شده: {pointsEarned}"
                : $"با تشکر از شرکت شما در نظرسنجی. امتیاز کسب شده: {pointsEarned}"
        };
    }
}
