-- Adds simulation/audit tables only; no administrator or legacy accounting table is altered.
SET XACT_ABORT ON;
BEGIN TRANSACTION;
IF OBJECT_ID(N'dbo.IntegrationAdminSimulations', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.IntegrationAdminSimulations
    (
        Id uniqueidentifier NOT NULL CONSTRAINT PK_IntegrationAdminSimulations PRIMARY KEY,
        AdminUserId nvarchar(128) COLLATE Latin1_General_100_BIN2 NOT NULL,
        ShopId int NOT NULL,
        MerchantIdentifier nvarchar(128) NOT NULL,
        TenantId nvarchar(30) NOT NULL,
        ShopName nvarchar(400) NOT NULL,
        CreatedAtUtc datetime2(7) NOT NULL,
        ExpiresAtUtc datetime2(7) NOT NULL,
        EndedAtUtc datetime2(7) NULL
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
        RequestedAtUtc datetime2(7) NOT NULL,
        CONSTRAINT FK_IntegrationTokenRequests_Simulation FOREIGN KEY (SimulationId)
            REFERENCES dbo.IntegrationAdminSimulations(Id)
    );
END;
COMMIT;
