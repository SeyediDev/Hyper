-- Run ONLY on the Integration database, before deploying the draft API/worker.
SET XACT_ABORT ON;
BEGIN TRANSACTION;
IF OBJECT_ID(N'dbo.ExternalIntegrationConnections',N'U') IS NULL
    THROW 51000, 'Integration database required', 1;
IF OBJECT_ID(N'dbo.IntegrationScenarioJobs',N'U') IS NULL
    THROW 51000, 'Integration scenario schema required', 1;
IF OBJECT_ID(N'dbo.CK_IntegrationScenarioJobs_Item',N'C') IS NOT NULL
    ALTER TABLE dbo.IntegrationScenarioJobs DROP CONSTRAINT CK_IntegrationScenarioJobs_Item;
ALTER TABLE dbo.IntegrationScenarioJobs WITH CHECK ADD CONSTRAINT CK_IntegrationScenarioJobs_Item CHECK (Item BETWEEN 1 AND 9);
IF OBJECT_ID(N'dbo.IntegrationProductCreations',N'U') IS NULL
BEGIN
    CREATE TABLE dbo.IntegrationProductCreations(
        Id bigint IDENTITY NOT NULL PRIMARY KEY,
        ConnectionId bigint NOT NULL,
        RequestId uniqueidentifier NOT NULL,
        HyperProductId int NOT NULL,
        AccountIdentifier nvarchar(200) NOT NULL,
        JobId bigint NULL,
        RequestJson nvarchar(max) NOT NULL,
        PayloadJson nvarchar(max) NOT NULL,
        State tinyint NOT NULL,
        ExternalProductId nvarchar(200) NULL,
        MappingId bigint NULL,
        ErrorCode nvarchar(100) NULL,
        CreatedAtUtc datetime2 NOT NULL,
        UpdatedAtUtc datetime2 NOT NULL,
        CONSTRAINT FK_ProductCreations_Connection FOREIGN KEY(ConnectionId) REFERENCES dbo.ExternalIntegrationConnections(Id),
        CONSTRAINT UQ_ProductCreations_Request UNIQUE(ConnectionId,RequestId),
        CONSTRAINT UQ_ProductCreations_Source UNIQUE(ConnectionId,HyperProductId)
    );
END;
COMMIT;
