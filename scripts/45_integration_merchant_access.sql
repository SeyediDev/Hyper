-- ACCESS-REGISTRY-001: adds only a new independent table. No existing table is altered.
SET XACT_ABORT ON;
BEGIN TRANSACTION;
IF OBJECT_ID(N'dbo.IntegrationMerchantAccess', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.IntegrationMerchantAccess
    (
        Id bigint IDENTITY(1,1) NOT NULL CONSTRAINT PK_IntegrationMerchantAccess PRIMARY KEY,
        Issuer nvarchar(300) COLLATE Latin1_General_100_BIN2 NOT NULL,
        SubjectId nvarchar(128) COLLATE Latin1_General_100_BIN2 NOT NULL,
        ShopId int NOT NULL,
        IsEnabled bit NOT NULL CONSTRAINT DF_IntegrationMerchantAccess_Enabled DEFAULT ((1)),
        CreatedAtUtc datetime2(7) NOT NULL CONSTRAINT DF_IntegrationMerchantAccess_Created DEFAULT (sysutcdatetime()),
        CreatedBy nvarchar(128) NOT NULL,
        ExpiresAtUtc datetime2(7) NULL,
        RevokedAtUtc datetime2(7) NULL,
        CONSTRAINT UQ_IntegrationMerchantAccess_IdentityShop UNIQUE (Issuer, SubjectId, ShopId),
        CONSTRAINT CK_IntegrationMerchantAccess_Shop CHECK (ShopId > 0)
    );
END;
COMMIT;
