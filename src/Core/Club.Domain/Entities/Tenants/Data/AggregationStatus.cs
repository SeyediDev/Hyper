namespace Hyper.Domain.Entities.Tenants.Data;

public enum AggregationStatus
{
    [Description("در حال پردازش")]
    Processing = 1,
    
    [Description("تکمیل شده")]
    Completed = 2,
    
    [Description("خطا")]
    Failed = 3
}
