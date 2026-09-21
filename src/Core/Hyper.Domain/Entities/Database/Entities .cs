using Hyper.Domain.Entities.Database;

[DisplayName("خارجی یکسان‌سازی اتصال‌ها")]
[DbMap("ExternalIntegrationConnections")]
public sealed class SqlExternalintegrationconnections : SqlServerEntity<long>
{
    [DisplayName("مغازه شناسه")]
    [DbMap("ShopId")]
    public int Shopid { get; set; }
    [DisplayName("مستاجر شناسه")]
    [DbMap("TenantId")]
    public string Tenantid { get; set; } = null!;
    [DisplayName("پلتفرم")]
    [DbMap("Provider")]
    public byte Provider { get; set; }
    [DisplayName("نمایشی نام")]
    [DbMap("DisplayName")]
    public string Displayname { get; set; } = null!;
    [DisplayName("حساب شناسه")]
    [DbMap("AccountIdentifier")]
    public string Accountidentifier { get; set; } = null!;
    [DisplayName("اعتبارنامه نوع")]
    [DbMap("CredentialType")]
    public byte Credentialtype { get; set; }
    [DisplayName("اعتبارنامه‌ها جی‌سان")]
    [DbMap("CredentialsJson")]
    public string Credentialsjson { get; set; } = null!;
    [DisplayName("انقضا در زمان یوتی‌سی")]
    [DbMap("ExpiresAtUtc")]
    public DateTime? Expiresatutc { get; set; }
    [DisplayName("آیا فعال")]
    [DbMap("IsEnabled")]
    public bool Isenabled { get; set; }
    [DisplayName("متصل‌شده در زمان یوتی‌سی")]
    [DbMap("ConnectedAtUtc")]
    public DateTime Connectedatutc { get; set; }
    [DisplayName("آخرین یکسان‌سازی در زمان یوتی‌سی")]
    [DbMap("LastSyncAtUtc")]
    public DateTime? Lastsyncatutc { get; set; }
    [DisplayName("آخرین خطا")]
    [DbMap("LastError")]
    public string? Lasterror { get; set; }
}
[DisplayName("خارجی سفارش نگاشت‌ها")]
[DbMap("ExternalOrderMappings")]
public sealed class SqlExternalordermappings : SqlServerEntity<long>
{
    [DisplayName("اتصال شناسه")]
    [DbMap("ConnectionId")]
    public long Connectionid { get; set; }
    [DisplayName("مغازه شناسه")]
    [DbMap("ShopId")]
    public int Shopid { get; set; }
    [DisplayName("هایپریک فروش سفارش شناسه")]
    [DbMap("HyperSaleOrderId")]
    public long? Hypersaleorderid { get; set; }
    [DisplayName("خارجی سفارش شناسه")]
    [DbMap("ExternalOrderId")]
    public string Externalorderid { get; set; } = null!;
    [DisplayName("خارجی مرسوله شناسه")]
    [DbMap("ExternalParcelId")]
    public string? Externalparcelid { get; set; }
    [DisplayName("وضعیت")]
    [DbMap("Status")]
    public byte Status { get; set; }
    [DisplayName("آخرین یکسان‌سازی در زمان یوتی‌سی")]
    [DbMap("LastSyncAtUtc")]
    public DateTime? Lastsyncatutc { get; set; }
}
[DisplayName("خارجی کالا نگاشت‌ها")]
[DbMap("ExternalProductMappings")]
public sealed class SqlExternalproductmappings : SqlServerEntity<long>
{
    [DisplayName("اتصال شناسه")]
    [DbMap("ConnectionId")]
    public long Connectionid { get; set; }
    [DisplayName("مغازه شناسه")]
    [DbMap("ShopId")]
    public int Shopid { get; set; }
    [DisplayName("هایپریک کالا شناسه")]
    [DbMap("HyperProductId")]
    public int Hyperproductid { get; set; }
    [DisplayName("خارجی کالا شناسه")]
    [DbMap("ExternalProductId")]
    public string Externalproductid { get; set; } = null!;
    [DisplayName("خارجی کد کالا")]
    [DbMap("ExternalSku")]
    public string? Externalsku { get; set; }
    [DisplayName("خارجی گونه کالا شناسه")]
    [DbMap("ExternalVariantId")]
    public string? Externalvariantid { get; set; }
    [DisplayName("آخرین خارجی قیمت")]
    [DbMap("LastExternalPrice")]
    public decimal? Lastexternalprice { get; set; }
    [DisplayName("آخرین خارجی موجودی")]
    [DbMap("LastExternalInventory")]
    public decimal? Lastexternalinventory { get; set; }
    [DisplayName("آخرین یکسان‌سازی در زمان یوتی‌سی")]
    [DbMap("LastSyncAtUtc")]
    public DateTime? Lastsyncatutc { get; set; }
    [DisplayName("آیا فعال")]
    [DbMap("IsActive")]
    public bool Isactive { get; set; }
}
[DisplayName("یکسان‌سازی ادمین شبیه‌سازی‌ها")]
[DbMap("IntegrationAdminSimulations")]
public sealed class SqlIntegrationadminsimulations : SqlServerEntity<Guid>
{
    [DisplayName("ادمین کاربر شناسه")]
    [DbMap("AdminUserId")]
    public string Adminuserid { get; set; } = null!;
    [DisplayName("مغازه شناسه")]
    [DbMap("ShopId")]
    public int Shopid { get; set; }
    [DisplayName("مغازه‌دار شناسه")]
    [DbMap("MerchantIdentifier")]
    public string Merchantidentifier { get; set; } = null!;
    [DisplayName("مستاجر شناسه")]
    [DbMap("TenantId")]
    public string Tenantid { get; set; } = null!;
    [DisplayName("مغازه نام")]
    [DbMap("ShopName")]
    public string Shopname { get; set; } = null!;
    [DisplayName("ایجادشده در زمان یوتی‌سی")]
    [DbMap("CreatedAtUtc")]
    public DateTime Createdatutc { get; set; }
    [DisplayName("انقضا در زمان یوتی‌سی")]
    [DbMap("ExpiresAtUtc")]
    public DateTime Expiresatutc { get; set; }
    [DisplayName("پایان‌یافته در زمان یوتی‌سی")]
    [DbMap("EndedAtUtc")]
    public DateTime? Endedatutc { get; set; }
}
[DisplayName("یکسان‌سازی رویداد ممیزی‌ها")]
[DbMap("IntegrationEventAudits")]
public sealed class SqlIntegrationeventaudits : SqlServerEntity<long>
{
    [DisplayName("اتصال شناسه")]
    [DbMap("ConnectionId")]
    public long? Connectionid { get; set; }
    [DisplayName("جهت")]
    [DbMap("Direction")]
    public string Direction { get; set; } = null!;
    [DisplayName("رویداد نوع")]
    [DbMap("EventType")]
    public string Eventtype { get; set; } = null!;
    [DisplayName("خارجی رویداد شناسه")]
    [DbMap("ExternalEventId")]
    public string Externaleventid { get; set; } = null!;
    [DisplayName("محتوای رویداد هش")]
    [DbMap("PayloadHash")]
    public string Payloadhash { get; set; } = null!;
    [DisplayName("امضا معتبر")]
    [DbMap("SignatureValid")]
    public bool Signaturevalid { get; set; }
    [DisplayName("همبستگی شناسه")]
    [DbMap("CorrelationId")]
    public string? Correlationid { get; set; }
    [DisplayName("دریافت‌شده در زمان یوتی‌سی")]
    [DbMap("ReceivedAtUtc")]
    public DateTime Receivedatutc { get; set; }
}
[DisplayName("یکسان‌سازی مغازه‌دار دسترسی")]
[DbMap("IntegrationMerchantAccess")]
public sealed class SqlIntegrationmerchantaccess : SqlServerEntity<long>
{
    [DisplayName("صادرکننده")]
    [DbMap("Issuer")]
    public string Issuer { get; set; } = null!;
    [DisplayName("موضوع شناسه")]
    [DbMap("SubjectId")]
    public string Subjectid { get; set; } = null!;
    [DisplayName("مغازه شناسه")]
    [DbMap("ShopId")]
    public int Shopid { get; set; }
    [DisplayName("آیا فعال")]
    [DbMap("IsEnabled")]
    public bool Isenabled { get; set; }
    [DisplayName("ایجادشده در زمان یوتی‌سی")]
    [DbMap("CreatedAtUtc")]
    public DateTime Createdatutc { get; set; }
    [DisplayName("ایجادشده توسط")]
    [DbMap("CreatedBy")]
    public string Createdby { get; set; } = null!;
    [DisplayName("انقضا در زمان یوتی‌سی")]
    [DbMap("ExpiresAtUtc")]
    public DateTime? Expiresatutc { get; set; }
    [DisplayName("لغوشده در زمان یوتی‌سی")]
    [DbMap("RevokedAtUtc")]
    public DateTime? Revokedatutc { get; set; }
}
[DisplayName("یکسان‌سازی صف خروجی")]
[DbMap("IntegrationOutbox")]
public sealed class SqlIntegrationoutbox : SqlServerEntity<long>
{
    [DisplayName("اتصال شناسه")]
    [DbMap("ConnectionId")]
    public long Connectionid { get; set; }
    [DisplayName("نگاشت شناسه")]
    [DbMap("MappingId")]
    public long Mappingid { get; set; }
    [DisplayName("منبع نسخه")]
    [DbMap("SourceVersion")]
    public long Sourceversion { get; set; }
    [DisplayName("عملیات")]
    [DbMap("Operation")]
    public string Operation { get; set; } = null!;
    [DisplayName("محتوای رویداد جی‌سان")]
    [DbMap("PayloadJson")]
    public string Payloadjson { get; set; } = null!;
    [DisplayName("وضعیت")]
    [DbMap("Status")]
    public byte Status { get; set; }
    [DisplayName("تلاش‌ها")]
    [DbMap("Attempts")]
    public int Attempts { get; set; }
    [DisplayName("ایجادشده در زمان یوتی‌سی")]
    [DbMap("CreatedAtUtc")]
    public DateTime Createdatutc { get; set; }
    [DisplayName("بعدی تلاش در زمان یوتی‌سی")]
    [DbMap("NextAttemptAtUtc")]
    public DateTime Nextattemptatutc { get; set; }
    [DisplayName("اجاره پردازش شناسه")]
    [DbMap("LeaseId")]
    public Guid? Leaseid { get; set; }
    [DisplayName("اجاره پردازش انقضا در زمان یوتی‌سی")]
    [DbMap("LeaseExpiresAtUtc")]
    public DateTime? Leaseexpiresatutc { get; set; }
    [DisplayName("تکمیل‌شده در زمان یوتی‌سی")]
    [DbMap("CompletedAtUtc")]
    public DateTime? Completedatutc { get; set; }
    [DisplayName("آخرین خطا")]
    [DbMap("LastError")]
    public string? Lasterror { get; set; }
}
[DisplayName("یکسان‌سازی یکسان‌سازی اجراها")]
[DbMap("IntegrationSyncRuns")]
public sealed class SqlIntegrationsyncruns : SqlServerEntity<long>
{
    [DisplayName("اتصال شناسه")]
    [DbMap("ConnectionId")]
    public long Connectionid { get; set; }
    [DisplayName("شروع‌شده در زمان یوتی‌سی")]
    [DbMap("StartedAtUtc")]
    public DateTime Startedatutc { get; set; }
    [DisplayName("پایان‌یافته در زمان یوتی‌سی")]
    [DbMap("FinishedAtUtc")]
    public DateTime? Finishedatutc { get; set; }
    [DisplayName("وضعیت")]
    [DbMap("Status")]
    public byte Status { get; set; }
    [DisplayName("اقلام خواندن")]
    [DbMap("ItemsRead")]
    public int Itemsread { get; set; }
    [DisplayName("اقلام نوشته‌شده")]
    [DbMap("ItemsWritten")]
    public int Itemswritten { get; set; }
    [DisplayName("اقلام ناموفق")]
    [DbMap("ItemsFailed")]
    public int Itemsfailed { get; set; }
    [DisplayName("خطا")]
    [DbMap("Error")]
    public string? Error { get; set; }
}
[DisplayName("یکسان‌سازی توکن درخواست‌ها")]
[DbMap("IntegrationTokenRequests")]
public sealed class SqlIntegrationtokenrequests : SqlServerEntity<Guid>
{
    [DisplayName("شبیه‌سازی شناسه")]
    [DbMap("SimulationId")]
    public Guid Simulationid { get; set; }
    [DisplayName("پلتفرم")]
    [DbMap("Provider")]
    public byte Provider { get; set; }
    [DisplayName("اعتبارنامه نوع")]
    [DbMap("CredentialType")]
    public byte Credentialtype { get; set; }
    [DisplayName("وضعیت")]
    [DbMap("Status")]
    public byte Status { get; set; }
    [DisplayName("درخواست‌شده در زمان یوتی‌سی")]
    [DbMap("RequestedAtUtc")]
    public DateTime Requestedatutc { get; set; }
}
[DisplayName("یکسان‌سازی وب‌هوک صندوق ورودی")]
[DbMap("IntegrationWebhookInbox")]
public sealed class SqlIntegrationwebhookinbox : SqlServerEntity<long>
{
    [DisplayName("اتصال شناسه")]
    [DbMap("ConnectionId")]
    public long Connectionid { get; set; }
    [DisplayName("خارجی رویداد شناسه")]
    [DbMap("ExternalEventId")]
    public string Externaleventid { get; set; } = null!;
    [DisplayName("رویداد نوع")]
    [DbMap("EventType")]
    public string Eventtype { get; set; } = null!;
    [DisplayName("محتوای رویداد جی‌سان")]
    [DbMap("PayloadJson")]
    public string Payloadjson { get; set; } = null!;
    [DisplayName("دریافت‌شده در زمان یوتی‌سی")]
    [DbMap("ReceivedAtUtc")]
    public DateTime Receivedatutc { get; set; }
    [DisplayName("پردازش‌شده در زمان یوتی‌سی")]
    [DbMap("ProcessedAtUtc")]
    public DateTime? Processedatutc { get; set; }
    [DisplayName("وضعیت")]
    [DbMap("Status")]
    public byte Status { get; set; }
    [DisplayName("خطا")]
    [DbMap("Error")]
    public string? Error { get; set; }
}
[DisplayName("موجودی رزرو سوابق")]
[DbMap("InventoryReservationLogs")]
public sealed class SqlInventoryreservationlogs : SqlServerEntity<long>
{
    [DisplayName("مغازه شناسه")]
    [DbMap("ShopId")]
    public int Shopid { get; set; }
    [DisplayName("هایپریک کالا شناسه")]
    [DbMap("HyperProductId")]
    public int Hyperproductid { get; set; }
    [DisplayName("رزرو کلید")]
    [DbMap("ReservationKey")]
    public string Reservationkey { get; set; } = null!;
    [DisplayName("تعداد")]
    [DbMap("Quantity")]
    public decimal Quantity { get; set; }
    [DisplayName("وضعیت")]
    [DbMap("Status")]
    public byte Status { get; set; }
    [DisplayName("منبع")]
    [DbMap("Source")]
    public string Source { get; set; } = null!;
    [DisplayName("ایجادشده در زمان یوتی‌سی")]
    [DbMap("CreatedAtUtc")]
    public DateTime Createdatutc { get; set; }
    [DisplayName("آزادشده در زمان یوتی‌سی")]
    [DbMap("ReleasedAtUtc")]
    public DateTime? Releasedatutc { get; set; }
}
