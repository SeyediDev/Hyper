namespace Hyper.Domain.Entities.Geography;

[DisplayName("استان")]
[SBVR(SBVRModality.Obligatory, "پایش جغرافیایی", "استان باید به کشور متصل شود تا گزارش‌های نقشه ایران قابل تولید باشد")]
[Microsoft.EntityFrameworkCore.IndexAttribute(nameof(CountryId))]
[Microsoft.EntityFrameworkCore.IndexAttribute(nameof(CountryId), nameof(Iso2), IsUnique = true)]
public class Province : HyperBaseCoreConfigAuditableEntity<int>
{
    [DisplayName("کشور")]
    public int CountryId { get; set; }

    [DisplayName("کشور")]
    public Country Country { get; set; } = null!;

    [DisplayName("عنوان")]
    [InDisplayString]
    [MaxLength(128)]
    public string Title { get; set; } = null!;

    [DisplayName("عنوان انگلیسی")]
    [MaxLength(128)]
    public string? EnglishTitle { get; set; }

    [DisplayName("کد ISO-2")]
    [MaxLength(8)]
    [SBVR(SBVRModality.Obligatory, "پایش جغرافیایی", "کد ISO-2 استان برای رنگ‌آمیزی نقشه ایران الزامی است")]
    public string Iso2 { get; set; } = null!;

    [DisplayName("عرض جغرافیایی")]
    public decimal? Latitude { get; set; }

    [DisplayName("طول جغرافیایی")]
    public decimal? Longitude { get; set; }

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

