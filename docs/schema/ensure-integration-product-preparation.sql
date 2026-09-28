-- Integration database only; additive/idempotent, does not touch accounting.
SET XACT_ABORT ON;
BEGIN TRANSACTION;
IF OBJECT_ID(N'dbo.IntegrationProductCreations',N'U') IS NULL
    THROW 51000, 'Apply product creation schema first on the Integration database', 1;
IF OBJECT_ID(N'dbo.IntegrationProductPreparations',N'U') IS NULL
CREATE TABLE dbo.IntegrationProductPreparations(
    Id bigint IDENTITY NOT NULL PRIMARY KEY, ConnectionId bigint NOT NULL,
    Direction tinyint NOT NULL, SourceProductId nvarchar(128) COLLATE Latin1_General_100_BIN2 NOT NULL,
    Revision int NOT NULL, InputJson nvarchar(max) NOT NULL, PreparedJson nvarchar(max) NOT NULL,
    IssuesJson nvarchar(max) NOT NULL, ChangesJson nvarchar(max) NOT NULL,
    WasAdapted bit NOT NULL, DraftRequestId uniqueidentifier NULL, JobId bigint NULL,
    UpdatedAtUtc datetime2 NOT NULL,
    CONSTRAINT FK_ProductPreparations_Connection FOREIGN KEY(ConnectionId) REFERENCES dbo.ExternalIntegrationConnections(Id),
    CONSTRAINT UQ_ProductPreparations_Source UNIQUE(ConnectionId,Direction,SourceProductId),
    CONSTRAINT CK_ProductPreparations_Direction CHECK(Direction IN (1,2)),
    CONSTRAINT CK_ProductPreparations_Revision CHECK(Revision>0)
);
IF OBJECT_ID(N'dbo.IntegrationProductPreparationHistory',N'U') IS NULL
CREATE TABLE dbo.IntegrationProductPreparationHistory(
    Id bigint IDENTITY NOT NULL PRIMARY KEY, PreparationId bigint NOT NULL,
    Revision int NOT NULL, AuditJson nvarchar(max) NOT NULL,
    CONSTRAINT FK_ProductPreparationHistory_Item FOREIGN KEY(PreparationId) REFERENCES dbo.IntegrationProductPreparations(Id),
    CONSTRAINT UQ_ProductPreparationHistory_Revision UNIQUE(PreparationId,Revision)
);
IF OBJECT_ID(N'dbo.IntegrationProductPreparationPolicies',N'U') IS NULL
CREATE TABLE dbo.IntegrationProductPreparationPolicies(
    Id bigint IDENTITY NOT NULL PRIMARY KEY, ConnectionId bigint NOT NULL,
    Direction tinyint NOT NULL, PolicyJson nvarchar(max) NOT NULL,
    UpdatedBy nvarchar(256) NOT NULL, UpdatedAtUtc datetime2 NOT NULL,
    CONSTRAINT FK_ProductPreparationPolicies_Connection FOREIGN KEY(ConnectionId) REFERENCES dbo.ExternalIntegrationConnections(Id),
    CONSTRAINT UQ_ProductPreparationPolicies_Direction UNIQUE(ConnectionId,Direction),
    CONSTRAINT CK_ProductPreparationPolicies_Direction CHECK(Direction IN (1,2))
);
COMMIT;
