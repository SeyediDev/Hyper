namespace Hyper.CustomerPortal.Application.Extensions;

/// <summary>
/// Extension methods برای IRequesterUser
/// </summary>
public static class RequesterUserExtensions
{
    /// <summary>
    /// دریافت UserId با exception اگر User login نکرده باشد
    /// </summary>
    public static UserId GetUserId(this IRequesterUser requesterUser)
    {
        if (!requesterUser.Id.HasValue)
        {
            throw new UnauthorizedAccessException("کاربر احراز هویت نشده است");
        }
        
        return requesterUser.Id.Value;
    }
}