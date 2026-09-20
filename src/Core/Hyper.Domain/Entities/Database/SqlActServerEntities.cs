namespace Hyper.Domain.Entities.Database;

[DisplayName("موتور: داده باینری")]
[DbMap("ACT_GE_BYTEARRAY")]
public sealed class SqlActGeBytearray : SqlServerEntity<string>
{
    [DisplayName("بازنگری")]
    [DbMap("REV_")]
    public int? Rev { get; set; }
    [DisplayName("نام")]
    [DbMap("NAME_")]
    public string? Name { get; set; }
    [DisplayName("استقرار شناسه")]
    [DbMap("DEPLOYMENT_ID_")]
    public string? DeploymentId { get; set; }
    [DisplayName("داده باینری")]
    [DbMap("BYTES_")]
    public byte[]? Bytes { get; set; }
    [DisplayName("تولیدشده")]
    [DbMap("GENERATED_")]
    public byte? Generated { get; set; }
    [DisplayName("مستاجر شناسه")]
    [DbMap("TENANT_ID_")]
    public string? TenantId { get; set; }
    [DisplayName("نوع")]
    [DbMap("TYPE_")]
    public int? Type { get; set; }
    [DisplayName("ایجاد زمان")]
    [DbMap("CREATE_TIME_")]
    public DateTime? CreateTime { get; set; }
    [DisplayName("ریشه فرایند نمونه شناسه")]
    [DbMap("ROOT_PROC_INST_ID_")]
    public string? RootProcInstId { get; set; }
    [DisplayName("حذف زمان")]
    [DbMap("REMOVAL_TIME_")]
    public DateTime? RemovalTime { get; set; }
}
