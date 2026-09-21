using Hyper.Domain.Entities.Database;

[DisplayName("عمومی تنظیمات")]
[DbMap("TBL_GlobalConfig")]
public sealed class SqlTblGlobalconfig : SqlServerEntity
{
    [DisplayName("کلید")]
    [DbMap("KEY_")]
    public string Key { get; set; } = null!;
    [DisplayName("مقدار")]
    [DbMap("VALUE_")]
    public string Value { get; set; } = null!;
    [DisplayName("مستاجر شناسه")]
    [DbMap("TENANT_ID_")]
    public string? TenantId { get; set; }
}
[DisplayName("تغییرات نسخه")]
[DbMap("TBL_ReleaseNote")]
public sealed class SqlTblReleasenote : SqlServerEntity
{
    [DisplayName("شناسه تغییرات نسخه")]
    [DbMap("RELEASEID_")]
    public byte Releaseid { get; set; }
    [DisplayName("تاریخ نسخه")]
    [DbMap("RELEASEDATE_")]
    public DateOnly Releasedate { get; set; }
    [DisplayName("شماره نسخه")]
    [DbMap("RELEASEVERSION_")]
    public string Releaseversion { get; set; } = null!;
    [DisplayName("مستاجر شناسه")]
    [DbMap("TENANT_ID_")]
    public string? TenantId { get; set; }
}
[DisplayName("آیتم تغییرات نسخه")]
[DbMap("TBL_ReleaseNoteItem")]
public sealed class SqlTblReleasenoteitem : SqlServerEntity
{
    [DisplayName("شناسه آیتم تغییرات نسخه")]
    [DbMap("RELEASEITEMID_")]
    public short Releaseitemid { get; set; }
    [DisplayName("شناسه نسخه")]
    [DbMap("RELEASEID_")]
    public byte Releaseid { get; set; }
    [DisplayName("ترتیب")]
    [DbMap("RELEASEITEMORDER_")]
    public byte Releaseitemorder { get; set; }
    [DisplayName("عنوان")]
    [DbMap("RELEASEITEMTITLE_")]
    public string Releaseitemtitle { get; set; } = null!;
    [DisplayName("توضیحات")]
    [DbMap("RELEASEITEMDESCRIPTION_")]
    public string? Releaseitemdescription { get; set; }
    [DisplayName("مستاجر شناسه")]
    [DbMap("TENANT_ID_")]
    public string? TenantId { get; set; }
}