using Hyper.CustomerPortal.Application.Interfaces;

namespace Hyper.CustomerPortal.Application.Services;

public class MockPromotionService : IPromotionService
{
    public Task<PaginatedList<PromotionDto>> GetActivePromotionsAsync(int pageNumber = 1, int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        var promotions = new List<PromotionDto>
        {
            new()
            {
                Id = 1,
                Title = "جشنواره تابستانی",
                Description = "50% تخفیف ویژه تابستان برای خریدهای بالای ۱۰۰ هزار تومان.",
                ShortDescription = "کسب امتیاز و تخفیف ویژه تابستانی!",
                Benefits = "✨ 50% امتیاز بیشتر\n✨ قرعه‌کشی هفتگی\n✨ تخفیف خرید بعدی",
                ParticipationGuide = "۱. خرید بالای ۱۰۰ هزار تومان انجام دهید.\n۲. به صورت خودکار در قرعه‌کشی شرکت داده می‌شوید.",
                StartDate = now.AddDays(-5),
                EndDate = now.AddDays(25),
                DaysUntilEnd = 25,
                ImageUrl = "https://cdn.example.com/promo/summer-main.jpg",
                CardImageUrl = "https://cdn.example.com/promo/summer-card.jpg",
                BannerImageUrl = "https://cdn.example.com/promo/summer-banner.jpg",
                IconUrl = "https://cdn.example.com/promo/summer-icon.png",
                PrimaryColor = "#FF6B6B",
                RewardType = "Points",
                IsActive = true,
                CanParticipate = true
            },
            new()
            {
                Id = 2,
                Title = "کمپین بازگشت به مدرسه",
                Description = "با خرید لوازم‌التحریر، امتیاز دوبرابر بگیرید و در قرعه‌کشی تبلت شرکت کنید.",
                ShortDescription = "امتیاز دوبرابر و جایزه ویژه!",
                Benefits = "✨ امتیاز دوبرابر\n✨ قرعه‌کشی تبلت\n✨ ارسال رایگان",
                ParticipationGuide = "۱. خرید لوازم‌التحریر از فروشگاه.\n۲. ثبت فاکتور در پرتال.",
                StartDate = now.AddDays(-2),
                EndDate = now.AddDays(10),
                DaysUntilEnd = 10,
                ImageUrl = "https://cdn.example.com/promo/school-main.jpg",
                CardImageUrl = "https://cdn.example.com/promo/school-card.jpg",
                BannerImageUrl = "https://cdn.example.com/promo/school-banner.jpg",
                IconUrl = "https://cdn.example.com/promo/school-icon.png",
                PrimaryColor = "#4C6EF5",
                RewardType = "GiftItem",
                IsActive = true,
                CanParticipate = true
            },
            new()
            {
                Id = 3,
                Title = "کمپین معرفی دوستان",
                Description = "با معرفی هر دوست، ۲۰ هزار تومان تخفیف بگیرید.",
                ShortDescription = "تخفیف برای هر معرفی موفق!",
                Benefits = "✨ ۲۰ هزار تومان تخفیف\n✨ بدون محدودیت تعداد معرفی",
                ParticipationGuide = "۱. لینک دعوت خود را به دوستان بدهید.\n۲. پس از ثبت‌نام و خرید دوست، تخفیف بگیرید.",
                StartDate = now.AddDays(-10),
                EndDate = now.AddDays(5),
                DaysUntilEnd = 5,
                ImageUrl = "https://cdn.example.com/promo/referral-main.jpg",
                CardImageUrl = "https://cdn.example.com/promo/referral-card.jpg",
                BannerImageUrl = "https://cdn.example.com/promo/referral-banner.jpg",
                IconUrl = "https://cdn.example.com/promo/referral-icon.png",
                PrimaryColor = "#00B894",
                RewardType = "Discount",
                IsActive = true,
                CanParticipate = false
            }
        };

        return Task.FromResult(new PaginatedList<PromotionDto>(promotions, promotions.Count, pageNumber, pageSize));
    }

    public Task<PromotionDto?> GetPromotionByIdAsync(int promotionId, CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        var promotion = new PromotionDto
        {
            Id = promotionId,
            Title = "جشنواره تابستانی",
            Description = "50% تخفیف ویژه تابستان برای خریدهای بالای ۱۰۰ هزار تومان.",
            ShortDescription = "کسب امتیاز و تخفیف ویژه تابستانی!",
            Benefits = "✨ 50% امتیاز بیشتر\n✨ قرعه‌کشی هفتگی\n✨ تخفیف خرید بعدی",
            ParticipationGuide = "۱. خرید بالای ۱۰۰ هزار تومان انجام دهید.\n۲. به صورت خودکار در قرعه‌کشی شرکت داده می‌شوید.",
            StartDate = now.AddDays(-5),
            EndDate = now.AddDays(25),
            DaysUntilEnd = 25,
            ImageUrl = "https://cdn.example.com/promo/summer-main.jpg",
            CardImageUrl = "https://cdn.example.com/promo/summer-card.jpg",
            BannerImageUrl = "https://cdn.example.com/promo/summer-banner.jpg",
            IconUrl = "https://cdn.example.com/promo/summer-icon.png",
            PrimaryColor = "#FF6B6B",
            RewardType = "Points",
            IsActive = true,
            CanParticipate = true
        };
        return Task.FromResult<PromotionDto?>(promotion);
    }

    public Task<ParticipationResultDto> ParticipateAsync(int customerId, int promotionId, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new ParticipationResultDto
        {
            Success = true,
            Message = "شما با موفقیت در کمپین شرکت کردید"
        });
    }

    public Task<PaginatedList<MyParticipationDto>> GetMyParticipationsAsync(int customerId, int pageNumber = 1, int pageSize = 20, CancellationToken cancellationToken = default)
    {
        var participations = new List<MyParticipationDto>();
        return Task.FromResult(new PaginatedList<MyParticipationDto>(participations, 0, pageNumber, pageSize));
    }
}

