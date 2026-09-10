USE [Hyperyek];
IF OBJECT_ID(N'dbo.BasalamConnections',N'U') IS NULL
BEGIN
CREATE TABLE dbo.BasalamConnections (Id int IDENTITY(1,1) NOT NULL PRIMARY KEY, ShopId int NOT NULL, TenantId nvarchar(30) NOT NULL, BasalamShopId nvarchar(128) NOT NULL, AccessToken nvarchar(max) NOT NULL, RefreshToken nvarchar(max) NULL, TokenExpiresAtUtc datetime2 NULL, IsEnabled bit NOT NULL DEFAULT(1), ConnectedAtUtc datetime2 NOT NULL DEFAULT SYSUTCDATETIME(), LastSynchronizedAtUtc datetime2 NULL, CONSTRAINT UQ_BasalamConnections UNIQUE(ShopId,BasalamShopId));
END
IF OBJECT_ID(N'dbo.BoothAccountingMappings',N'U') IS NULL
BEGIN
CREATE TABLE dbo.BoothAccountingMappings (Id bigint IDENTITY(1,1) NOT NULL PRIMARY KEY, ShopId int NOT NULL, TenantId nvarchar(30) NOT NULL, HyperProductId int NOT NULL, BasalamProductId nvarchar(128) NOT NULL, BasalamSku nvarchar(128) NULL, DetailAccountId bigint NULL, AccountingCode varchar(50) NULL, LastKnownPrice decimal(18,2) NULL, LastKnownInventory decimal(18,3) NOT NULL DEFAULT(0), LastSynchronizedAtUtc datetime2 NULL, IsActive bit NOT NULL DEFAULT(1), CONSTRAINT UQ_BoothAccountingMappings UNIQUE(ShopId,BasalamProductId));
END
