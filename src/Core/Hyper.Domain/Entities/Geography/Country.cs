namespace Hyper.Domain.Entities.Geography;

[DisplayName("کشور")]
[SBVR(SBVRModality.Obligatory, "پایش جغرافیایی", "هر کشور باید برای تحلیل جغرافیایی مشتریان و گزارش‌های نقشه‌ای قابل شناسایی باشد")]
[Microsoft.EntityFrameworkCore.IndexAttribute(nameof(Iso2), IsUnique = true)]
public class Country : HyperBaseCoreConfigAuditableEntity<int>
{
    [DisplayName("عنوان")]
    [InDisplayString]
    [MaxLength(128)]
    public string Title { get; set; } = null!;

    [DisplayName("عنوان انگلیسی")]
    [MaxLength(128)]
    public string? EnglishTitle { get; set; }

    [DisplayName("کد ISO-2")]
    [MaxLength(4)]
    [SBVR(SBVRModality.Obligatory, "پایش جغرافیایی", "کد ISO-2 برای انطباق با نقشه‌های جهانی و گزارش‌های WorldMap مورد نیاز است")]
    public string Iso2 { get; set; } = null!;

    [DisplayName("کد ISO-3")]
    [MaxLength(4)]
    public string? Iso3 { get; set; }

    [DisplayName("کد عددی ISO")]
    public int? IsoNumeric { get; set; }

    [DisplayName("کد تلفن بین‌المللی")]
    [MaxLength(8)]
    public string? PhoneCode { get; set; }

    [DisplayName("عرض جغرافیایی")]
    public decimal? Latitude { get; set; }

    [DisplayName("طول جغرافیایی")]
    public decimal? Longitude { get; set; }

    public ICollection<Province> Provinces { get; set; } = [];
    public ICollection<City> Cities { get; set; } = [];

    [DisplayName("جمعیت")]
    public long? Pop { get; set; }

    [DisplayName("TAM - جمعیت/بازار کل")]
    public long? Tam { get; set; }

    [DisplayName("SAM - جمعیت/بازار در دسترس")]
    public long? Sam { get; set; }

    [DisplayName("SOM - جمعیت/بازار قابل دستیابی")]
    public long? Som { get; set; }
}
