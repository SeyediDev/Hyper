namespace Hyper.Domain.Entities.Promotions;

public interface ISubOfPromotion
{
    int PromotionId { get; set; }
    Promotion Promotion { get; set; }
}
