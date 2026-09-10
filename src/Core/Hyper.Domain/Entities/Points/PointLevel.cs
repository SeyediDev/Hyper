namespace Hyper.Domain.Entities.Points;

/// <summary>
/// می خواهیم یک سری سطوح امتیاز تعریف کنیم
/// فعلا برای نوع امتیاز تجربه سطوح معنا دارد ولی در محصول آن را محدود نکردیم
/// TODO اگر کاربر در یک مدت زمانی خاص این سطح را ارتقاء نداد این سطح را از دست بدهد ؟ چگونه ؟ 
/// </summary>
[DisplayName("سطح‌امتیاز")]
public class PointLevel : HyperBaseCoreConfigAuditableEntity<int>
{
    public int PointId { get; set; }
    [DisplayName("امتیاز")]
    public Point Point { get; set; } = null!;

    [DisplayName("عنوان")]
    [InDisplayString]
    [MaxLength(41)]
    public string Title { get; set; } = null!;

    [DisplayName("کلید")]
    [MaxLength(41)]
    public string Key { get; set; } = null!;

    [DisplayName("حداقل امتیاز")]
    public int MinXp { get; set; }
    [DisplayName("حداکثر امتیاز")]
    public int MaxXp { get; set; }

    [DisplayName("سطح")]
    public int Level { get; set; }
}
