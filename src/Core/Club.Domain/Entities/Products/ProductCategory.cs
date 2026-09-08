namespace Hyper.Domain.Entities.Products;

/// <summary>
/// دسته‌بندی محصولات - ساختار درختی برای سازماندهی محصولات
/// </summary>
[DisplayName("دسته‌بندی محصولات")]
[SBVR(SBVRModality.Obligatory, "سازماندهی محصولات", "هر دسته‌بندی باید برای سازماندهی و نمایش محصولات در کاتالوگ قابل شناسایی باشد")]
[SBVR(SBVRModality.Recommended, "سازماندهی محصولات", "دسته‌بندی‌ها باید برای بهبود تجربه کاربری و سهولت جستجوی محصولات طراحی شوند")]
public class ProductCategory : HyperBaseCoreConfigAuditableEntity<int>, ISubOfTenant
{
    public int TenantId { get; set; }
    [DisplayName("اکوسیستم")]
    [SBVR(SBVRModality.Obligatory, "چندین اکوسیستم", "هر دسته‌بندی باید به یک اکوسیستم مشخص تعلق داشته باشد")]
    public Tenant Tenant { get; set; } = null!;

    /// <summary>
    /// عنوان دسته‌بندی
    /// </summary>
    [DisplayName("عنوان")]
    [InDisplayString]
    [MaxLength(81)]
    [SBVR(SBVRModality.Obligatory, "شناسایی دسته‌بندی", "عنوان برای نمایش در کاتالوگ و مدیریت دسته‌بندی‌ها ضروری است")]
    public string Title { get; set; } = null!;

    [MaxLength(40)]
    [DisplayName("کلید")]
    public string Key { get; set; } = null!;

    /// <summary>
    /// توضیحات دسته‌بندی
    /// </summary>
    [DisplayName("توضیحات")]
    [MaxLength(1000)]
    [SBVR(SBVRModality.Recommended, "مستندسازی دسته‌بندی", "توضیحات برای ارائه اطلاعات کامل به مشتریان استفاده می‌شود")]
    public string? Description { get; set; }

    /// <summary>
    /// شناسه دسته‌بندی والد (برای ساختار درختی)
    /// </summary>
    [DisplayName("دسته‌بندی والد")]
    [SBVR(SBVRModality.Permitted, "ساختار درختی", "دسته‌بندی والد برای ایجاد ساختار درختی و سازماندهی بهتر محصولات استفاده می‌شود")]
    public int? ParentCategoryId { get; set; }

    /// <summary>
    /// دسته‌بندی والد
    /// </summary>
    [DisplayName("دسته‌بندی والد")]
    public ProductCategory? ParentCategory { get; set; }

    /// <summary>
    /// ترتیب نمایش
    /// </summary>
    [DisplayName("ترتیب نمایش")]
    [SBVR(SBVRModality.Permitted, "سازماندهی نمایش", "ترتیب نمایش برای کنترل ترتیب نمایش دسته‌بندی‌ها در کاتالوگ استفاده می‌شود")]
    public int DisplayOrder { get; set; }

    /// <summary>
    /// آیا دسته‌بندی فعال است
    /// </summary>
    [DisplayName("فعال")]
    [SBVR(SBVRModality.Permitted, "مدیریت چرخه حیات", "وضعیت فعال برای کنترل نمایش دسته‌بندی در کاتالوگ استفاده می‌شود")]
    public bool IsActive { get; set; } = true;

    public int? PictureId { get; set; }
    [DisplayName("تصویر")]
    [SBVR(SBVRModality.Permitted, "نمایش بصری", "تصویر برای بهبود تجربه کاربری استفاده می‌شود")]
    public Document? Picture { get; set; }

    /// <summary>
    /// دسته‌بندی‌های فرزند
    /// </summary>
    [DisplayName("دسته‌بندی‌های فرزند")]
    public ICollection<ProductCategory> ChildCategories { get; set; } = [];

    /// <summary>
    /// محصولات این دسته‌بندی
    /// </summary>
    [DisplayName("محصولات")]
    public ICollection<Product> Products { get; set; } = [];
}