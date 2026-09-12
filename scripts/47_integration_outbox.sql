SET XACT_ABORT ON;
BEGIN TRANSACTION;
IF OBJECT_ID(N'dbo.IntegrationOutbox', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.IntegrationOutbox (
        Id bigint IDENTITY(1,1) NOT NULL CONSTRAINT PK_IntegrationOutbox PRIMARY KEY,
        ConnectionId bigint NOT NULL,
        MappingId bigint NOT NULL,
        SourceVersion bigint NOT NULL,
        Operation nvarchar(60) NOT NULL,
        PayloadJson nvarchar(max) NOT NULL,
        Status tinyint NOT NULL CONSTRAINT DF_IntegrationOutbox_Status DEFAULT 0,
        Attempts int NOT NULL CONSTRAINT DF_IntegrationOutbox_Attempts DEFAULT 0,
        CreatedAtUtc datetime2(7) NOT NULL CONSTRAINT DF_IntegrationOutbox_Created DEFAULT SYSUTCDATETIME(),
        NextAttemptAtUtc datetime2(7) NOT NULL,
        LeaseId uniqueidentifier NULL,
        LeaseExpiresAtUtc datetime2(7) NULL,
        CompletedAtUtc datetime2(7) NULL,
        LastError nvarchar(200) NULL,
        CONSTRAINT FK_IntegrationOutbox_Connection FOREIGN KEY(ConnectionId) REFERENCES dbo.ExternalIntegrationConnections(Id),
        CONSTRAINT FK_IntegrationOutbox_Mapping FOREIGN KEY(MappingId) REFERENCES dbo.ExternalProductMappings(Id),
        CONSTRAINT CK_IntegrationOutbox_State CHECK(Status IN (0,1,2,3)),
        CONSTRAINT CK_IntegrationOutbox_Version CHECK(SourceVersion > 0 AND Attempts >= 0),
        CONSTRAINT CK_IntegrationOutbox_Json CHECK(ISJSON(PayloadJson) = 1)
    );
    CREATE UNIQUE INDEX UX_IntegrationOutbox_Source ON dbo.IntegrationOutbox(MappingId, SourceVersion);
    CREATE INDEX IX_IntegrationOutbox_Due ON dbo.IntegrationOutbox(Status, NextAttemptAtUtc, ConnectionId);
    CREATE INDEX IX_IntegrationOutbox_Connection ON dbo.IntegrationOutbox(ConnectionId, Id);
END;
COMMIT;
