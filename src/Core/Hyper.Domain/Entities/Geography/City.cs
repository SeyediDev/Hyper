namespace Hyper.Domain.Entities.Geography;

[DisplayName("شهر")]
[SBVR(SBVRModality.Recommended, "پایش جغرافیایی", "ثبت شهر برای تحلیل‌های تفصیلی مشتری توصیه می‌شود")]
[Microsoft.EntityFrameworkCore.IndexAttribute(nameof(ProvinceId))]
[Microsoft.EntityFrameworkCore.IndexAttribute(nameof(ProvinceId), nameof(Title), IsUnique = true)]
public class City : HyperBaseCoreConfigAuditableEntity<int>
{
    [DisplayName("کشور")]
    public int CountryId { get; set; }

    [DisplayName("کشور")]
    public Country Country { get; set; } = null!;

    [DisplayName("استان")]
    public int ProvinceId { get; set; }

    [DisplayName("استان")]
    public Province Province { get; set; } = null!;

    [DisplayName("عنوان")]
    [InDisplayString]
    [MaxLength(128)]
    public string Title { get; set; } = null!;

    [DisplayName("عنوان انگلیسی")]
    [MaxLength(128)]
    public string? EnglishTitle { get; set; }

    [DisplayName("Iso2")]
    [MaxLength(32)]
    [OldDbMap("Code")]
    public string? Iso2 { get; set; }

    [DisplayName("عرض جغرافیایی")]
    public decimal? Latitude { get; set; }

    [DisplayName("طول جغرافیایی")]
    public decimal? Longitude { get; set; }

    [DisplayName("جمعیت")]
    public long? Pop { get; set; }

    [DisplayName("TAM - جمعیت/بازار کل")]
    public long? Tam { get; set; }

    [DisplayName("SAM - جمعیت/بازار در دسترس")]
    public long? Sam { get; set; }

    [DisplayName("SOM - جمعیت/بازار قابل دستیابی")]
    public long? Som { get; set; }
}
