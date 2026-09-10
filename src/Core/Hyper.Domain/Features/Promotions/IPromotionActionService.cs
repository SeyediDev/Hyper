namespace Hyper.Domain.Features.Promotions;

/// <summary>
/// سرویس اجرای اقدامات پویش - اجرای اقدامات OnCondition و OnCompletion
/// </summary>
public interface IPromotionActionService
{
    /// <summary>
    /// اجرای یک اقدام پویش
    /// </summary>
    /// <param name="request">درخواست پردازش پویش</param>
    /// <param name="action">اقدام پویش</param>
    /// <param name="cancellationToken">توکن لغو</param>
    [Telemetry]
    Task DoActionAsync(
        PromotionProcessingRequest request, 
        PromotionAction action, 
        CancellationToken cancellationToken = default);
}

