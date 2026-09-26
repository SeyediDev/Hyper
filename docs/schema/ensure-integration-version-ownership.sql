-- Run only on ConnectionStrings:IntegrationConnection. No existing source is guessed.
SET XACT_ABORT ON;
BEGIN TRANSACTION;
IF OBJECT_ID(N'dbo.ExternalProductMappings', N'U') IS NULL
    THROW 51000, 'Integration mapping schema is required; select the Integration database.', 1;
IF OBJECT_ID(N'dbo.IntegrationVersionOwnership', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.IntegrationVersionOwnership
    (
        MappingId bigint NOT NULL CONSTRAINT PK_IntegrationVersionOwnership PRIMARY KEY,
        Source tinyint NOT NULL,
        VersionFloor bigint NOT NULL,
        UpdatedAtUtc datetime2 NOT NULL,
        CONSTRAINT FK_IntegrationVersionOwnership_Mapping FOREIGN KEY (MappingId) REFERENCES dbo.ExternalProductMappings(Id),
        CONSTRAINT CK_IntegrationVersionOwnership_Source CHECK (Source IN (1,2) AND VersionFloor >= 0)
    );
END;
COMMIT TRANSACTION;
