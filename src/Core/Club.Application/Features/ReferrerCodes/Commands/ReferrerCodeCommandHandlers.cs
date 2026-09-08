//namespace Hyper.Application.Features.ReferrerCodes.Commands;

///// <summary>
///// Handler برای ایجاد کد معرف
///// </summary>
//public class CreateReferrerCodeCommandHandler(
//    //ICustomerService customerService,
//    //ILogger<CreateReferrerCodeCommandHandler> logger
//    ) 
//    : IRequestHandler<CreateReferrerCodeCommand, Result<int>>
//{
//    public Task<Result<int>> Handle(CreateReferrerCodeCommand request, CancellationToken cancellationToken)
//    {
//        //try
//        //{
//            // بررسی وجود مشتری
//            //var customer = await customerService.GetCustomer(request.Customer, false, null, cancellationToken);
//            //if (customer == null)
//            //    return Result<int>.Failure("مشتری یافت نشد");

//            //// تولید کد منحصر به فرد
//            //var referrerCode = GenerateUniqueCode(customer.Id, request.TenantKey);

//            //var newReferrerCode = new ReferrerCode
//            //{
//            //    TenantId = request.TenantId,
//            //    CustomerId = request.CustomerId,
//            //    Code = referrerCode,
//            //};

//            //await referrerCodeCmdRepo.AddAsync(newReferrerCode);

//            //logger.LogInformation("کد معرف جدید ایجاد شد: {Code} برای مشتری {CustomerId}", 
//            //    referrerCode, request.CustomerId);

//            //return Result<int>.Success(newReferrerCode.Id);
//            return Result<int>.Success(0);
//        //}
//        //catch (Exception 
//        ////ex
//        //)
//        //{
//        //    //logger.LogError(ex, "خطا در ایجاد کد معرف برای مشتری {CustomerId}", request.CustomerId);
//        //    return Result<int>.Failure("خطا در ایجاد کد معرف");
//        //}
//    }

//    private string GenerateUniqueCode(int customerId, int tenantId)
//    {
//        var timestamp = DateTime.UtcNow.ToString("MMddHHmm");
//        return $"REF{customerId:D6}{tenantId:D3}{timestamp}";
//    }
//}