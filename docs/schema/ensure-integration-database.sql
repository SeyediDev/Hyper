/*
  به نام خدا
  Owned schema for Hyper.Integration.Domain.
  Run this script against the database configured by ConnectionStrings:IntegrationConnection.
  It intentionally creates no table in the Hyperyek/platform database.
*/
SET XACT_ABORT ON;
BEGIN TRANSACTION;

IF OBJECT_ID(N'dbo.ExternalIntegrationConnections', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.ExternalIntegrationConnections
    (
        Id bigint IDENTITY(1,1) NOT NULL CONSTRAINT PK_ExternalIntegrationConnections PRIMARY KEY,
        ShopId int NOT NULL,
        TenantId nvarchar(30) NOT NULL,
        Provider tinyint NOT NULL,
        DisplayName nvarchar(200) NOT NULL,
        AccountIdentifier nvarchar(200) NOT NULL,
        CredentialType tinyint NOT NULL,
        CredentialsJson nvarchar(max) NOT NULL,
        ExpiresAtUtc datetime2 NULL,
        IsEnabled bit NOT NULL CONSTRAINT DF_ExternalIntegrationConnections_IsEnabled DEFAULT (1),
        ConnectedAtUtc datetime2 NOT NULL CONSTRAINT DF_ExternalIntegrationConnections_ConnectedAtUtc DEFAULT (sysutcdatetime()),
        LastSyncAtUtc datetime2 NULL,
        LastError nvarchar(2000) NULL,
        CONSTRAINT UQ_ExternalIntegrationConnections UNIQUE (ShopId, TenantId, Provider, AccountIdentifier)
    );
END;

-- Align an existing installation with the tenant-aware connection scope.
IF EXISTS (
    SELECT 1
    FROM sys.key_constraints kc
    INNER JOIN sys.tables t ON t.object_id = kc.parent_object_id
    INNER JOIN sys.schemas s ON s.schema_id = t.schema_id
    WHERE kc.name = N'UQ_ExternalIntegrationConnections'
      AND t.name = N'ExternalIntegrationConnections'
      AND s.name = N'dbo'
      AND NOT EXISTS (
          SELECT 1
          FROM sys.indexes i
          INNER JOIN sys.index_columns ic ON ic.object_id = i.object_id AND ic.index_id = i.index_id
          INNER JOIN sys.columns c ON c.object_id = ic.object_id AND c.column_id = ic.column_id
          WHERE i.object_id = kc.parent_object_id
            AND i.name = kc.name
            AND c.name = N'TenantId'
      )
)
BEGIN
    ALTER TABLE dbo.ExternalIntegrationConnections
        DROP CONSTRAINT UQ_ExternalIntegrationConnections;
END;

IF NOT EXISTS (
    SELECT 1
    FROM sys.key_constraints kc
    INNER JOIN sys.tables t ON t.object_id = kc.parent_object_id
    INNER JOIN sys.schemas s ON s.schema_id = t.schema_id
    WHERE kc.name = N'UQ_ExternalIntegrationConnections'
      AND t.name = N'ExternalIntegrationConnections'
      AND s.name = N'dbo'
)
BEGIN
    ALTER TABLE dbo.ExternalIntegrationConnections
        ADD CONSTRAINT UQ_ExternalIntegrationConnections
        UNIQUE (ShopId, TenantId, Provider, AccountIdentifier);
END;

IF OBJECT_ID(N'dbo.ExternalOAuthTokens', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.ExternalOAuthTokens
    (
        Id bigint IDENTITY(1,1) NOT NULL CONSTRAINT PK_ExternalOAuthTokens PRIMARY KEY,
        ConnectionId bigint NOT NULL,
        ShopId int NOT NULL,
        TenantId nvarchar(30) NOT NULL,
        Provider tinyint NOT NULL,
        AccessToken nvarchar(max) NOT NULL,
        RefreshToken nvarchar(max) NULL,
        TokenType nvarchar(50) NOT NULL,
        ExpiresAtUtc datetime2 NULL,
        Scopes nvarchar(2000) NULL,
        IssuedAtUtc datetime2 NOT NULL CONSTRAINT DF_ExternalOAuthTokens_IssuedAtUtc DEFAULT (sysutcdatetime()),
        UpdatedAtUtc datetime2 NULL,
        IsActive bit NOT NULL CONSTRAINT DF_ExternalOAuthTokens_IsActive DEFAULT (1),
        RawTokenResponse nvarchar(max) NULL
    );
    CREATE UNIQUE INDEX UX_ExternalOAuthTokens_ShopId_Provider ON dbo.ExternalOAuthTokens(ShopId, TenantId, Provider);
    CREATE INDEX IX_ExternalOAuthTokens_ConnectionId ON dbo.ExternalOAuthTokens(ConnectionId);
    CREATE INDEX IX_ExternalOAuthTokens_ExpiresAtUtc ON dbo.ExternalOAuthTokens(ExpiresAtUtc);
    CREATE INDEX IX_ExternalOAuthTokens_TenantId ON dbo.ExternalOAuthTokens(TenantId);
END;

-- Align existing OAuth tokens with the same tenant scope as connections.
IF EXISTS (
    SELECT 1
    FROM sys.indexes i
    WHERE i.object_id = OBJECT_ID(N'dbo.ExternalOAuthTokens')
      AND i.name = N'UX_ExternalOAuthTokens_ShopId_Provider'
      AND NOT EXISTS (
          SELECT 1
          FROM sys.index_columns ic
          INNER JOIN sys.columns c ON c.object_id = ic.object_id AND c.column_id = ic.column_id
          WHERE ic.object_id = i.object_id AND ic.index_id = i.index_id AND c.name = N'TenantId'
      )
)
BEGIN
    DROP INDEX UX_ExternalOAuthTokens_ShopId_Provider ON dbo.ExternalOAuthTokens;
END;
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID(N'dbo.ExternalOAuthTokens')
    AND name = N'UX_ExternalOAuthTokens_ShopId_Provider')
BEGIN
    CREATE UNIQUE INDEX UX_ExternalOAuthTokens_ShopId_Provider
        ON dbo.ExternalOAuthTokens(ShopId, TenantId, Provider);
END;

IF OBJECT_ID(N'dbo.IntegrationCustomerMappings', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.IntegrationCustomerMappings
    (
        Id bigint IDENTITY(1,1) NOT NULL CONSTRAINT PK_IntegrationCustomerMappings PRIMARY KEY,
        ShopId int NOT NULL,
        TenantId nvarchar(30) NOT NULL,
        ExternalCustomerId nvarchar(128) NOT NULL,
        PersonId int NOT NULL
    );
    CREATE UNIQUE INDEX UX_IntegrationCustomerMappings_ExternalIdentity
        ON dbo.IntegrationCustomerMappings(ShopId, TenantId, ExternalCustomerId);
    CREATE UNIQUE INDEX UX_IntegrationCustomerMappings_Person
        ON dbo.IntegrationCustomerMappings(ShopId, TenantId, PersonId);
END;

IF OBJECT_ID(N'dbo.IntegrationScenarioJobs', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.IntegrationScenarioJobs
    (
        Id bigint IDENTITY(1,1) NOT NULL CONSTRAINT PK_IntegrationScenarioJobs PRIMARY KEY,
        ConnectionId bigint NOT NULL,
        ShopId int NOT NULL,
        TenantId nvarchar(128) NOT NULL,
        EventId nvarchar(128) COLLATE Latin1_General_100_BIN2 NOT NULL,
        Item tinyint NOT NULL,
        [Trigger] tinyint NOT NULL,
        Status tinyint NOT NULL CONSTRAINT DF_IntegrationScenarioJobs_Status DEFAULT (0),
        Attempts int NOT NULL CONSTRAINT DF_IntegrationScenarioJobs_Attempts DEFAULT (0),
        CreatedAtUtc datetime2 NOT NULL CONSTRAINT DF_IntegrationScenarioJobs_Created DEFAULT (sysutcdatetime()),
        NextAttemptAtUtc datetime2 NOT NULL,
        LeaseId uniqueidentifier NULL,
        LeaseExpiresAtUtc datetime2 NULL,
        CompletedAtUtc datetime2 NULL,
        ErrorCode nvarchar(100) NULL,
        ResultJson nvarchar(max) NULL,
        CONSTRAINT UQ_IntegrationScenarioJobs_Connection_Event UNIQUE (ConnectionId, EventId),
        CONSTRAINT FK_IntegrationScenarioJobs_Connection FOREIGN KEY (ConnectionId)
            REFERENCES dbo.ExternalIntegrationConnections(Id)
    );
    CREATE INDEX IX_IntegrationScenarioJobs_Due
        ON dbo.IntegrationScenarioJobs(Status, NextAttemptAtUtc, ConnectionId);
END;

IF OBJECT_ID(N'dbo.ExternalProductMappings', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.ExternalProductMappings
    (
        Id bigint IDENTITY(1,1) NOT NULL CONSTRAINT PK_ExternalProductMappings PRIMARY KEY,
        ConnectionId bigint NOT NULL,
        ShopId int NOT NULL,
        HyperProductId int NOT NULL,
        ExternalProductId nvarchar(200) NOT NULL,
        ExternalSku nvarchar(200) NULL,
        ExternalVariantId nvarchar(200) NULL,
        LastExternalPrice decimal(18,2) NULL,
        LastExternalInventory decimal(18,3) NULL,
        LastSyncAtUtc datetime2 NULL,
        IsActive bit NOT NULL CONSTRAINT DF_ExternalProductMappings_IsActive DEFAULT (1),
        CONSTRAINT UQ_ExternalProductMappings UNIQUE (ConnectionId, ExternalProductId, ExternalVariantId),
        CONSTRAINT FK_ExternalProductMappings_Connection FOREIGN KEY (ConnectionId) REFERENCES dbo.ExternalIntegrationConnections(Id)
    );
END;

IF OBJECT_ID(N'dbo.ExternalOrderMappings', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.ExternalOrderMappings
    (
        Id bigint IDENTITY(1,1) NOT NULL CONSTRAINT PK_ExternalOrderMappings PRIMARY KEY,
        ConnectionId bigint NOT NULL,
        ShopId int NOT NULL,
        HyperSaleOrderId bigint NULL,
        ExternalOrderId nvarchar(200) NOT NULL,
        ExternalParcelId nvarchar(200) NULL,
        Status tinyint NOT NULL CONSTRAINT DF_ExternalOrderMappings_Status DEFAULT (0),
        LastSyncAtUtc datetime2 NULL,
        CONSTRAINT UQ_ExternalOrderMappings UNIQUE (ConnectionId, ExternalOrderId)
    );
END;

IF OBJECT_ID(N'dbo.IntegrationSyncRuns', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.IntegrationSyncRuns
    (
        Id bigint IDENTITY(1,1) NOT NULL CONSTRAINT PK_IntegrationSyncRuns PRIMARY KEY,
        ConnectionId bigint NOT NULL,
        StartedAtUtc datetime2 NOT NULL CONSTRAINT DF_IntegrationSyncRuns_StartedAtUtc DEFAULT (sysutcdatetime()),
        FinishedAtUtc datetime2 NULL,
        Status tinyint NOT NULL,
        ItemsRead int NOT NULL CONSTRAINT DF_IntegrationSyncRuns_ItemsRead DEFAULT (0),
        ItemsWritten int NOT NULL CONSTRAINT DF_IntegrationSyncRuns_ItemsWritten DEFAULT (0),
        ItemsFailed int NOT NULL CONSTRAINT DF_IntegrationSyncRuns_ItemsFailed DEFAULT (0),
        Error nvarchar(4000) NULL,
        CONSTRAINT FK_IntegrationSyncRuns_Connection FOREIGN KEY (ConnectionId) REFERENCES dbo.ExternalIntegrationConnections(Id)
    );
END;

IF OBJECT_ID(N'dbo.IntegrationWebhookInbox', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.IntegrationWebhookInbox
    (
        Id bigint IDENTITY(1,1) NOT NULL CONSTRAINT PK_IntegrationWebhookInbox PRIMARY KEY,
        ConnectionId bigint NOT NULL,
        ExternalEventId nvarchar(200) NOT NULL,
        EventType nvarchar(200) NOT NULL,
        PayloadJson nvarchar(max) NOT NULL,
        ReceivedAtUtc datetime2 NOT NULL CONSTRAINT DF_IntegrationWebhookInbox_ReceivedAtUtc DEFAULT (sysutcdatetime()),
        ProcessedAtUtc datetime2 NULL,
        Status tinyint NOT NULL CONSTRAINT DF_IntegrationWebhookInbox_Status DEFAULT (0),
        Error nvarchar(4000) NULL,
        CONSTRAINT UQ_IntegrationWebhookInbox UNIQUE (ConnectionId, ExternalEventId),
        CONSTRAINT FK_IntegrationWebhookInbox_Connection FOREIGN KEY (ConnectionId) REFERENCES dbo.ExternalIntegrationConnections(Id)
    );
END;

IF OBJECT_ID(N'dbo.IntegrationOutbox', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.IntegrationOutbox
    (
        Id bigint IDENTITY(1,1) NOT NULL CONSTRAINT PK_IntegrationOutbox PRIMARY KEY,
        ConnectionId bigint NOT NULL,
        MappingId bigint NOT NULL,
        SourceVersion bigint NOT NULL,
        Operation nvarchar(60) NOT NULL,
        PayloadJson nvarchar(max) NOT NULL,
        Status tinyint NOT NULL CONSTRAINT DF_IntegrationOutbox_Status DEFAULT (0),
        Attempts int NOT NULL CONSTRAINT DF_IntegrationOutbox_Attempts DEFAULT (0),
        CreatedAtUtc datetime2 NOT NULL CONSTRAINT DF_IntegrationOutbox_CreatedAtUtc DEFAULT (sysutcdatetime()),
        NextAttemptAtUtc datetime2 NOT NULL,
        LeaseId uniqueidentifier NULL,
        LeaseExpiresAtUtc datetime2 NULL,
        CompletedAtUtc datetime2 NULL,
        LastError nvarchar(200) NULL,
        CONSTRAINT FK_IntegrationOutbox_Connection FOREIGN KEY (ConnectionId) REFERENCES dbo.ExternalIntegrationConnections(Id),
        CONSTRAINT FK_IntegrationOutbox_Mapping FOREIGN KEY (MappingId) REFERENCES dbo.ExternalProductMappings(Id)
    );
    CREATE UNIQUE INDEX UX_IntegrationOutbox_Source ON dbo.IntegrationOutbox(MappingId, SourceVersion);
    CREATE INDEX IX_IntegrationOutbox_Due ON dbo.IntegrationOutbox(Status, NextAttemptAtUtc, ConnectionId);
END;

IF OBJECT_ID(N'dbo.IntegrationMerchantAccess', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.IntegrationMerchantAccess
    (
        Id bigint IDENTITY(1,1) NOT NULL CONSTRAINT PK_IntegrationMerchantAccess PRIMARY KEY,
        Issuer nvarchar(300) NOT NULL,
        SubjectId nvarchar(128) NOT NULL,
        ShopId int NOT NULL,
        IsEnabled bit NOT NULL CONSTRAINT DF_IntegrationMerchantAccess_IsEnabled DEFAULT (1),
        CreatedAtUtc datetime2 NOT NULL CONSTRAINT DF_IntegrationMerchantAccess_CreatedAtUtc DEFAULT (sysutcdatetime()),
        CreatedBy nvarchar(128) NOT NULL,
        ExpiresAtUtc datetime2 NULL,
        RevokedAtUtc datetime2 NULL,
        CONSTRAINT UQ_IntegrationMerchantAccess_IdentityShop UNIQUE (Issuer, SubjectId, ShopId)
    );
END;

IF OBJECT_ID(N'dbo.IntegrationAdminSimulations', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.IntegrationAdminSimulations
    (
        Id uniqueidentifier NOT NULL CONSTRAINT PK_IntegrationAdminSimulations PRIMARY KEY,
        AdminUserId nvarchar(128) NOT NULL,
        ShopId int NOT NULL,
        MerchantIdentifier nvarchar(128) NOT NULL,
        TenantId nvarchar(30) NOT NULL,
        ShopName nvarchar(400) NOT NULL,
        CreatedAtUtc datetime2 NOT NULL,
        ExpiresAtUtc datetime2 NOT NULL,
        EndedAtUtc datetime2 NULL
    );
END;

IF OBJECT_ID(N'dbo.IntegrationTokenRequests', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.IntegrationTokenRequests
    (
        Id uniqueidentifier NOT NULL CONSTRAINT PK_IntegrationTokenRequests PRIMARY KEY,
        SimulationId uniqueidentifier NOT NULL,
        Provider tinyint NOT NULL,
        CredentialType tinyint NOT NULL,
        Status tinyint NOT NULL,
        RequestedAtUtc datetime2 NOT NULL,
        CONSTRAINT FK_IntegrationTokenRequests_Simulation FOREIGN KEY (SimulationId) REFERENCES dbo.IntegrationAdminSimulations(Id)
    );
END;

IF OBJECT_ID(N'dbo.IntegrationEventAudits', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.IntegrationEventAudits
    (
        Id bigint IDENTITY(1,1) NOT NULL CONSTRAINT PK_IntegrationEventAudits PRIMARY KEY,
        ConnectionId bigint NULL,
        Direction varchar(20) NOT NULL,
        EventType nvarchar(200) NOT NULL,
        ExternalEventId nvarchar(200) NOT NULL,
        PayloadHash varchar(128) NOT NULL,
        SignatureValid bit NOT NULL,
        CorrelationId varchar(100) NULL,
        ReceivedAtUtc datetime2 NOT NULL CONSTRAINT DF_IntegrationEventAudits_ReceivedAtUtc DEFAULT (sysutcdatetime()),
        CONSTRAINT UQ_IntegrationEventAudits UNIQUE (ConnectionId, ExternalEventId, Direction)
    );
END;

IF OBJECT_ID(N'dbo.InventoryReservationLogs', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.InventoryReservationLogs
    (
        Id bigint IDENTITY(1,1) NOT NULL CONSTRAINT PK_InventoryReservationLogs PRIMARY KEY,
        ShopId int NOT NULL,
        HyperProductId int NOT NULL,
        ReservationKey nvarchar(200) NOT NULL,
        Quantity decimal(18,3) NOT NULL,
        Status tinyint NOT NULL CONSTRAINT DF_InventoryReservationLogs_Status DEFAULT (0),
        Source varchar(50) NOT NULL,
        CreatedAtUtc datetime2 NOT NULL CONSTRAINT DF_InventoryReservationLogs_CreatedAtUtc DEFAULT (sysutcdatetime()),
        ReleasedAtUtc datetime2 NULL,
        CONSTRAINT UQ_InventoryReservationLogs UNIQUE (ShopId, ReservationKey)
    );
END;

COMMIT TRANSACTION;
