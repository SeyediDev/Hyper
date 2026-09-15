SET XACT_ABORT ON;
BEGIN TRANSACTION;
IF OBJECT_ID(N'dbo.IntegrationScenarioJobs', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.IntegrationScenarioJobs (
        Id bigint IDENTITY(1,1) NOT NULL PRIMARY KEY,
        ConnectionId bigint NOT NULL,
        ShopId int NOT NULL,
        TenantId nvarchar(128) NOT NULL,
        EventId nvarchar(128) COLLATE Latin1_General_100_BIN2 NOT NULL,
        Item tinyint NOT NULL,
        [Trigger] tinyint NOT NULL,
        Status tinyint NOT NULL,
        Attempts int NOT NULL,
        CreatedAtUtc datetime2 NOT NULL,
        NextAttemptAtUtc datetime2 NOT NULL,
        LeaseId uniqueidentifier NULL,
        LeaseExpiresAtUtc datetime2 NULL,
        CompletedAtUtc datetime2 NULL,
        ErrorCode nvarchar(100) NULL,
        ResultJson nvarchar(max) NULL,
        CONSTRAINT CK_IntegrationScenarioJobs_Item CHECK (Item BETWEEN 1 AND 5),
        CONSTRAINT CK_IntegrationScenarioJobs_Trigger CHECK ([Trigger] BETWEEN 1 AND 5),
        CONSTRAINT CK_IntegrationScenarioJobs_Status CHECK (Status BETWEEN 0 AND 4),
        CONSTRAINT FK_IntegrationScenarioJobs_Connection FOREIGN KEY (ConnectionId) REFERENCES dbo.ExternalIntegrationConnections(Id)
    );
    CREATE UNIQUE INDEX UX_IntegrationScenarioJobs_Event ON dbo.IntegrationScenarioJobs(ConnectionId, EventId);
    CREATE INDEX IX_IntegrationScenarioJobs_Pending ON dbo.IntegrationScenarioJobs(Status, NextAttemptAtUtc, Id);
END;
COMMIT;
