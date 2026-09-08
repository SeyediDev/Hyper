namespace Hyper.Application.Features.ReferrerCodes.Commands;

/// <summary>
/// دستور ایجاد کد معرف جدید
/// </summary>
public class CreateReferrerCodeCommand : IRequest<Result<int>>
{
    public string Customer { get; set; } = null!;
    public string TenantKey { get; set; } = null!;
}