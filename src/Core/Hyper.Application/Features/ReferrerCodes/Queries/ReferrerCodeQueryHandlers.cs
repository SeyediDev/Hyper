//using Neo.Domain.Repository;

//namespace Hyper.Application.Features.ReferrerCodes.Queries;

///// <summary>
///// Handler برای دریافت کد معرف بر اساس شناسه
///// </summary>
//public class GetReferrerCodeByIdQueryHandler : IRequestHandler<GetReferrerCodeByIdQuery, Result<ReferrerCodeDto>>
//{
//    private readonly IQueryRepository<ReferrerCode, int> _referrerCodeQueryRepo;
//    private readonly ILogger<GetReferrerCodeByIdQueryHandler> _logger;

//    public GetReferrerCodeByIdQueryHandler(
//        IQueryRepository<ReferrerCode, int> referrerCodeQueryRepo,
//        ILogger<GetReferrerCodeByIdQueryHandler> logger)
//    {
//        _referrerCodeQueryRepo = referrerCodeQueryRepo;
//        _logger = logger;
//    }

//    public async Task<Result<ReferrerCodeDto>> Handle(GetReferrerCodeByIdQuery request, CancellationToken cancellationToken)
//    {
//        try
//        {
//            var referrerCode = await _referrerCodeQueryRepo.GetByIdAsync(request.Id);
//            if (referrerCode == null)
//                return Result<ReferrerCodeDto>.Failure("کد معرف یافت نشد");

//            var dto = new ReferrerCodeDto
//            {
//                Id = referrerCode.Id,
//                TenantId = referrerCode.TenantId,
//                CustomerId = referrerCode.CustomerId,
//                CustomerName = $"{referrerCode.Customer.FirstName} {referrerCode.Customer.LastName}".Trim(),
//                Code = referrerCode.Code,
//                IsActive = referrerCode.IsActive,
//                ExpiryDate = referrerCode.ExpiryDate,
//                UsedAt = referrerCode.UsedAt,
//                UsageCount = referrerCode.UsageCount,
//                MaxUsageCount = referrerCode.MaxUsageCount,
//                Description = referrerCode.Description,
//                CreatedAt = referrerCode.CreatedAt,
//                ModifiedAt = referrerCode.ModifiedAt
//            };

//            return Result<ReferrerCodeDto>.Success(dto);
//        }
//        catch (Exception ex)
//        {
//            _logger.LogError(ex, "خطا در دریافت کد معرف {Id}", request.Id);
//            return Result<ReferrerCodeDto>.Failure("خطا در دریافت کد معرف");
//        }
//    }
//}

///// <summary>
///// Handler برای دریافت کد معرف بر اساس کد
///// </summary>
//public class GetReferrerCodeByCodeQueryHandler : IRequestHandler<GetReferrerCodeByCodeQuery, Result<ReferrerCodeDto>>
//{
//    private readonly IQueryRepository<ReferrerCode, int> _referrerCodeQueryRepo;
//    private readonly ILogger<GetReferrerCodeByCodeQueryHandler> _logger;

//    public GetReferrerCodeByCodeQueryHandler(
//        IQueryRepository<ReferrerCode, int> referrerCodeQueryRepo,
//        ILogger<GetReferrerCodeByCodeQueryHandler> logger)
//    {
//        _referrerCodeQueryRepo = referrerCodeQueryRepo;
//        _logger = logger;
//    }

//    public async Task<Result<ReferrerCodeDto>> Handle(GetReferrerCodeByCodeQuery request, CancellationToken cancellationToken)
//    {
//        try
//        {
//            var referrerCode = await _referrerCodeQueryRepo.GetFirstOrDefaultAsync(
//                rc => rc.Code == request.Code && rc.TenantId == request.TenantId);

//            if (referrerCode == null)
//                return Result<ReferrerCodeDto>.Failure("کد معرف یافت نشد");

//            var dto = new ReferrerCodeDto
//            {
//                Id = referrerCode.Id,
//                TenantId = referrerCode.TenantId,
//                CustomerId = referrerCode.CustomerId,
//                CustomerName = $"{referrerCode.Customer.FirstName} {referrerCode.Customer.LastName}".Trim(),
//                Code = referrerCode.Code,
//                IsActive = referrerCode.IsActive,
//                ExpiryDate = referrerCode.ExpiryDate,
//                UsedAt = referrerCode.UsedAt,
//                UsageCount = referrerCode.UsageCount,
//                MaxUsageCount = referrerCode.MaxUsageCount,
//                Description = referrerCode.Description,
//                CreatedAt = referrerCode.CreatedAt,
//                ModifiedAt = referrerCode.ModifiedAt
//            };

//            return Result<ReferrerCodeDto>.Success(dto);
//        }
//        catch (Exception ex)
//        {
//            _logger.LogError(ex, "خطا در دریافت کد معرف {Code}", request.Code);
//            return Result<ReferrerCodeDto>.Failure("خطا در دریافت کد معرف");
//        }
//    }
//}

///// <summary>
///// Handler برای دریافت لیست کدهای معرف مشتری
///// </summary>
//public class GetReferrerCodesByCustomerQueryHandler : IRequestHandler<GetReferrerCodesByCustomerQuery, Result<List<ReferrerCodeDto>>>
//{
//    private readonly IQueryRepository<ReferrerCode, int> _referrerCodeQueryRepo;
//    private readonly ILogger<GetReferrerCodesByCustomerQueryHandler> _logger;

//    public GetReferrerCodesByCustomerQueryHandler(
//        IQueryRepository<ReferrerCode, int> referrerCodeQueryRepo,
//        ILogger<GetReferrerCodesByCustomerQueryHandler> logger)
//    {
//        _referrerCodeQueryRepo = referrerCodeQueryRepo;
//        _logger = logger;
//    }

//    public async Task<Result<List<ReferrerCodeDto>>> Handle(GetReferrerCodesByCustomerQuery request, CancellationToken cancellationToken)
//    {
//        try
//        {
//            var referrerCodes = await _referrerCodeQueryRepo.GetAllAsync(
//                rc => rc.CustomerId == request.CustomerId && rc.TenantId == request.TenantId);

//            var dtos = referrerCodes.Select(rc => new ReferrerCodeDto
//            {
//                Id = rc.Id,
//                TenantId = rc.TenantId,
//                CustomerId = rc.CustomerId,
//                CustomerName = $"{rc.Customer.FirstName} {rc.Customer.LastName}".Trim(),
//                Code = rc.Code,
//                IsActive = rc.IsActive,
//                ExpiryDate = rc.ExpiryDate,
//                UsedAt = rc.UsedAt,
//                UsageCount = rc.UsageCount,
//                MaxUsageCount = rc.MaxUsageCount,
//                Description = rc.Description,
//                CreatedAt = rc.CreatedAt,
//                ModifiedAt = rc.ModifiedAt
//            }).ToList();

//            return Result<List<ReferrerCodeDto>>.Success(dtos);
//        }
//        catch (Exception ex)
//        {
//            _logger.LogError(ex, "خطا در دریافت کدهای معرف مشتری {CustomerId}", request.CustomerId);
//            return Result<List<ReferrerCodeDto>>.Failure("خطا در دریافت کدهای معرف");
//        }
//    }
//}

///// <summary>
///// Handler برای دریافت لیست کدهای معرف فعال
///// </summary>
//public class GetActiveReferrerCodesQueryHandler : IRequestHandler<GetActiveReferrerCodesQuery, Result<List<ReferrerCodeDto>>>
//{
//    private readonly IQueryRepository<ReferrerCode, int> _referrerCodeQueryRepo;
//    private readonly ILogger<GetActiveReferrerCodesQueryHandler> _logger;

//    public GetActiveReferrerCodesQueryHandler(
//        IQueryRepository<ReferrerCode, int> referrerCodeQueryRepo,
//        ILogger<GetActiveReferrerCodesQueryHandler> logger)
//    {
//        _referrerCodeQueryRepo = referrerCodeQueryRepo;
//        _logger = logger;
//    }

//    public async Task<Result<List<ReferrerCodeDto>>> Handle(GetActiveReferrerCodesQuery request, CancellationToken cancellationToken)
//    {
//        try
//        {
//            var referrerCodes = await _referrerCodeQueryRepo.GetAllAsync(
//                rc => rc.TenantId == request.TenantId && 
//                      rc.IsActive && 
//                      (rc.ExpiryDate == null || rc.ExpiryDate > DateTime.UtcNow));

//            var dtos = referrerCodes.Select(rc => new ReferrerCodeDto
//            {
//                Id = rc.Id,
//                TenantId = rc.TenantId,
//                CustomerId = rc.CustomerId,
//                CustomerName = $"{rc.Customer.FirstName} {rc.Customer.LastName}".Trim(),
//                Code = rc.Code,
//                IsActive = rc.IsActive,
//                ExpiryDate = rc.ExpiryDate,
//                UsedAt = rc.UsedAt,
//                UsageCount = rc.UsageCount,
//                MaxUsageCount = rc.MaxUsageCount,
//                Description = rc.Description,
//                CreatedAt = rc.CreatedAt,
//                ModifiedAt = rc.ModifiedAt
//            }).ToList();

//            return Result<List<ReferrerCodeDto>>.Success(dtos);
//        }
//        catch (Exception ex)
//        {
//            _logger.LogError(ex, "خطا در دریافت کدهای معرف فعال");
//            return Result<List<ReferrerCodeDto>>.Failure("خطا در دریافت کدهای معرف");
//        }
//    }
//}

///// <summary>
///// Handler برای اعتبارسنجی کد معرف
///// </summary>
//public class ValidateReferrerCodeQueryHandler : IRequestHandler<ValidateReferrerCodeQuery, Result<bool>>
//{
//    private readonly IQueryRepository<ReferrerCode, int> _referrerCodeQueryRepo;
//    private readonly ILogger<ValidateReferrerCodeQueryHandler> _logger;

//    public ValidateReferrerCodeQueryHandler(
//        IQueryRepository<ReferrerCode, int> referrerCodeQueryRepo,
//        ILogger<ValidateReferrerCodeQueryHandler> logger)
//    {
//        _referrerCodeQueryRepo = referrerCodeQueryRepo;
//        _logger = logger;
//    }

//    public async Task<Result<bool>> Handle(ValidateReferrerCodeQuery request, CancellationToken cancellationToken)
//    {
//        try
//        {
//            var referrerCode = await _referrerCodeQueryRepo.GetFirstOrDefaultAsync(
//                rc => rc.Code == request.Code && 
//                      rc.TenantId == request.TenantId && 
//                      rc.IsActive);

//            if (referrerCode == null)
//                return Result<bool>.Success(false);

//            // بررسی انقضا
//            if (referrerCode.ExpiryDate.HasValue && referrerCode.ExpiryDate.Value < DateTime.UtcNow)
//                return Result<bool>.Success(false);

//            // بررسی حداکثر استفاده
//            if (referrerCode.MaxUsageCount.HasValue && referrerCode.UsageCount >= referrerCode.MaxUsageCount.Value)
//                return Result<bool>.Success(false);

//            return Result<bool>.Success(true);
//        }
//        catch (Exception ex)
//        {
//            _logger.LogError(ex, "خطا در اعتبارسنجی کد معرف {Code}", request.Code);
//            return Result<bool>.Failure("خطا در اعتبارسنجی کد معرف");
//        }
//    }
//}

///// <summary>
///// Handler برای دریافت آمار کد معرف
///// </summary>
//public class GetReferrerCodeStatsQueryHandler : IRequestHandler<GetReferrerCodeStatsQuery, Result<ReferrerCodeStatsDto>>
//{
//    private readonly IQueryRepository<ReferrerCode, int> _referrerCodeQueryRepo;
//    private readonly IQueryRepository<CustomerReferrer, int> _customerReferrerQueryRepo;
//    private readonly ILogger<GetReferrerCodeStatsQueryHandler> _logger;

//    public GetReferrerCodeStatsQueryHandler(
//        IQueryRepository<ReferrerCode, int> referrerCodeQueryRepo,
//        IQueryRepository<CustomerReferrer, int> customerReferrerQueryRepo,
//        ILogger<GetReferrerCodeStatsQueryHandler> logger)
//    {
//        _referrerCodeQueryRepo = referrerCodeQueryRepo;
//        _customerReferrerQueryRepo = customerReferrerQueryRepo;
//        _logger = logger;
//    }

//    public async Task<Result<ReferrerCodeStatsDto>> Handle(GetReferrerCodeStatsQuery request, CancellationToken cancellationToken)
//    {
//        try
//        {
//            var referrerCode = await _referrerCodeQueryRepo.GetByIdAsync(request.Id);
//            if (referrerCode == null)
//                return Result<ReferrerCodeStatsDto>.Failure("کد معرف یافت نشد");

//            var referrals = await _customerReferrerQueryRepo.GetAllAsync(
//                cr => cr.ReferrerCodeId == request.Id);

//            var stats = new ReferrerCodeStatsDto
//            {
//                Id = referrerCode.Id,
//                Code = referrerCode.Code,
//                TotalUsage = referrerCode.UsageCount,
//                SuccessfulReferrals = referrals.Count(cr => cr.Status == ReferrerStatus.Successful),
//                TotalEarnedPoints = referrals.Sum(cr => cr.ReferrerEarnedPoints),
//                LastUsedAt = referrerCode.UsedAt,
//                IsExpired = referrerCode.ExpiryDate.HasValue && referrerCode.ExpiryDate.Value < DateTime.UtcNow,
//                IsMaxUsageReached = referrerCode.MaxUsageCount.HasValue && referrerCode.UsageCount >= referrerCode.MaxUsageCount.Value
//            };

//            return Result<ReferrerCodeStatsDto>.Success(stats);
//        }
//        catch (Exception ex)
//        {
//            _logger.LogError(ex, "خطا در دریافت آمار کد معرف {Id}", request.Id);
//            return Result<ReferrerCodeStatsDto>.Failure("خطا در دریافت آمار کد معرف");
//        }
//    }
//}
