SET XACT_ABORT ON;
BEGIN TRANSACTION;
IF OBJECT_ID(N'dbo.IntegrationParcelCommands',N'U') IS NULL
BEGIN
    CREATE TABLE dbo.IntegrationParcelCommands (
        Id bigint IDENTITY NOT NULL PRIMARY KEY,
        ConnectionId bigint NOT NULL REFERENCES dbo.ExternalIntegrationConnections(Id),
        RequestId uniqueidentifier NOT NULL,
        ParcelId nvarchar(128) COLLATE Latin1_General_100_BIN2 NOT NULL,
        Target tinyint NOT NULL,
        AccountIdentifier nvarchar(200) NOT NULL,
        RequestJson nvarchar(max) NOT NULL,
        Actor nvarchar(256) NOT NULL,
        JobId bigint NULL,
        State tinyint NOT NULL,
        ErrorCode nvarchar(100) NULL,
        CreatedAtUtc datetime2 NOT NULL,
        UpdatedAtUtc datetime2 NOT NULL,
        CONSTRAINT UQ_IntegrationParcelCommands_Request UNIQUE(ConnectionId,RequestId),
        CONSTRAINT UQ_IntegrationParcelCommands_Target UNIQUE(ConnectionId,ParcelId,Target),
        CONSTRAINT CK_IntegrationParcelCommands_State CHECK(State BETWEEN 0 AND 3 AND Target IN(1,2))
    );
END;
COMMIT;
