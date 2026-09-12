using Hyper.Domain.Entities.Integrations;
using Hyper.Domain.Features.Integrations;

namespace Hyper.AdminPanel.Web.ViewModels;

public sealed record AdminOverviewViewModel(AdminOverviewSnapshot Statistics, string ScopeName, bool IsPreview, bool HasSelectedShop)
{
    public static AdminOverviewViewModel Preview(int days)
    {
        var now = DateTime.UtcNow;
        var trend = Enumerable.Range(0, days).Select(i => new AdminOverviewDay(now.Date.AddDays(i + 1 - days),
            18 + (i * 17 % 43), 48 + (i * 13 % 64), i % 6 == 0 ? 3 : 0)).ToArray();
        IntegrationRecentRun[] recent =
        [
            new(1006,"فروشگاه نارون",IntegrationProvider.Basalam,1,now.AddMinutes(-3),now.AddMinutes(-2),36,36,0,null),
            new(1005,"خانه و زندگی",IntegrationProvider.Basalam,3,now.AddMinutes(-8),now.AddMinutes(-7),24,21,3,"سه کالا نیازمند تأیید نگاشت است."),
            new(1004,"فروشگاه سپید",IntegrationProvider.Custom,2,now.AddMinutes(-14),now.AddMinutes(-13),0,0,1,"اتصال نیازمند بررسی است."),
            new(1003,"فروشگاه نارون",IntegrationProvider.Basalam,1,now.AddMinutes(-25),now.AddMinutes(-24),18,18,0,null)
        ];
        return new(new(now,days,24,1842,1726,936,trend.Sum(x=>x.Invoices),17,18,15,2,
            trend.Sum(x=>x.SuccessfulRuns),trend.Sum(x=>x.FailedRuns),12,2,1258,3,4,1,trend,
            [new(IntegrationProvider.Basalam,14,12,now.AddMinutes(-3)),new(IntegrationProvider.Custom,4,3,now.AddMinutes(-14))],recent),
            "همهٔ مغازه‌ها",true,false);
    }
}
