-- Idempotent migration of the active, release-level backlog into WorkManagement.
-- Completed historical checklist items remain in docs/BACKLOG.md; active work is
-- operationally managed here and can be claimed by one role at a time.
DECLARE @ProjectId bigint = (SELECT Id FROM dbo.Projects WHERE [Key] = N'HYPER');
DECLARE @Items TABLE ([Key] nvarchar(40), Title nvarchar(300), Domain nvarchar(80), Priority tinyint, OwnerRole nvarchar(120));
INSERT @Items VALUES
(N'ARCH-NEO-001',N'حذف وابستگی غیرمستقیم خارج از پنل',N'architecture',3,N'architecture-lead'),
(N'PLATFORM-API-001',N'ایجاد API مالک commandهای حسابداری',N'accounting',4,N'accounting-platform'),
(N'ARCH-DOMAIN-002',N'تکمیل persistence مستقل Integration و migrationها',N'architecture',3,N'architecture-lead'),
(N'ARCH-DOMAIN-003',N'تکمیل قرارداد ارتباط Integration و Hyperyek',N'architecture',4,N'architecture-lead'),
(N'ARCH-DOMAIN-004',N'تکمیل API و Worker مستقل Integration',N'architecture',4,N'architecture-lead'),
(N'ARCH-DOMAIN-005',N'طراحی قرارداد versioned قابل واگذاری',N'architecture',3,N'architecture-lead'),
(N'ARCH-DOMAIN-006',N'تست مرز dependency دامین‌ها',N'quality',3,N'quality'),
(N'ARCH-TRANSFER-001',N'انتقال کامل کدهای legacy Integration',N'architecture',3,N'architecture-lead'),
(N'ARCH-PORT-001',N'جایگزینی دسترسی مستقیم به Hyperyek با port/API',N'architecture',4,N'architecture-lead'),
(N'INT-001',N'Integration Registry API',N'integration',4,N'basalam-integration'),
(N'INT-002',N'Credential Vault و refresh token',N'integration',4,N'basalam-integration'),
(N'INT-003',N'Provider Strategy Resolver',N'integration',3,N'basalam-integration'),
(N'INT-005',N'Webhook Inbox و idempotency',N'integration',4,N'basalam-integration'),
(N'INT-006',N'Outbox/CDC تغییرات حسابداری',N'worker',4,N'worker-operations'),
(N'FLOW-C',N'چرخه حیات parcel و tracking',N'basalam',3,N'basalam-integration'),
(N'FLOW-D',N'فروش غرفه و اعمال در حسابداری',N'accounting',4,N'accounting-platform'),
(N'FLOW-E',N'خرید مشتری و release رزرو',N'accounting',4,N'accounting-platform'),
(N'FLOW-FG',N'اصلاح و مرجوعی سفارش',N'accounting',3,N'accounting-platform'),
(N'ADP-BASALAM',N'تکمیل adapter رسمی باسلام',N'basalam',4,N'basalam-integration'),
(N'ADP-HTTP',N'HTTP policy شامل retry و rate limit',N'worker',3,N'worker-operations'),
(N'WRK-001',N'میزبان مستقل IntegrationSync.Worker',N'worker',4,N'worker-operations'),
(N'WRK-002',N'پردازش idempotent با lock و ترتیب رویداد',N'worker',4,N'worker-operations'),
(N'CMD-101',N'ApplyExternalProductChanged',N'accounting',3,N'accounting-platform'),
(N'CMD-102',N'ApplyVendorOrderCreated',N'accounting',4,N'accounting-platform'),
(N'CMD-103',N'ApplyCustomerOrderChanged',N'accounting',3,N'accounting-platform'),
(N'CMD-104',N'ApplyParcelStatusChanged',N'accounting',3,N'accounting-platform'),
(N'DATA-101',N'ExternalOrderMappings عمومی',N'accounting',3,N'accounting-platform'),
(N'DATA-102',N'InventoryReservationLog',N'accounting',4,N'accounting-platform'),
(N'SEC-201',N'ثبت hash و نتیجه signature وب‌هوک',N'security',3,N'quality'),
(N'SEC-202',N'timestamp tolerance و replay protection',N'security',3,N'quality'),
(N'SEC-203',N'redaction credential و payload حساس',N'security',3,N'quality'),
(N'SEC-204',N'tenant/shop isolation تمام endpointها',N'security',4,N'quality'),
(N'API-104',N'داشبورد اختلاف قیمت/موجودی و سلامت صف',N'panel',3,N'panel-operations'),
(N'DASH-201',N'query اتصال‌ها، sync و خطا',N'panel',2,N'panel-operations'),
(N'DASH-202',N'query اختلاف موجودی و قیمت',N'panel',2,N'panel-operations'),
(N'DASH-203',N'query inbox/outbox و dead-letter',N'panel',3,N'panel-operations'),
(N'E2E-101',N'تست فروش غرفه و جلوگیری از overselling',N'quality',4,N'quality'),
(N'E2E-102',N'تست catalog دوطرفه و loop prevention',N'quality',3,N'quality'),
(N'E2E-103',N'تست رزرو و release',N'quality',4,N'quality'),
(N'E2E-104',N'تست parcel و tracking',N'quality',3,N'quality');

INSERT dbo.WorkItems(ProjectId,[Key],Title,Domain,Status,Priority,OwnerRole,CreatedAtUtc,UpdatedAtUtc)
SELECT @ProjectId,i.[Key],i.Title,i.Domain,2,i.Priority,i.OwnerRole,SYSUTCDATETIME(),SYSUTCDATETIME()
FROM @Items i
WHERE NOT EXISTS (SELECT 1 FROM dbo.WorkItems w WHERE w.ProjectId=@ProjectId AND w.[Key]=i.[Key]);
