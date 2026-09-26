-- Only the new OAuth token table. Never alter the accounting/legacy schema.
SET XACT_ABORT ON;
BEGIN TRANSACTION;
IF OBJECT_ID(N'dbo.ExternalOAuthTokens', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.ExternalOAuthTokens (
        Id bigint IDENTITY(1,1) NOT NULL CONSTRAINT PK_ExternalOAuthTokens PRIMARY KEY,
        ConnectionId bigint NOT NULL,
        ShopId int NOT NULL,
        TenantId nvarchar(30) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
        Provider tinyint NOT NULL,
        AccessToken nvarchar(max) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
        RefreshToken nvarchar(max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
        TokenType nvarchar(50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL DEFAULT N'Bearer',
        ExpiresAtUtc datetime2(7) NULL,
        Scopes nvarchar(2000) COLLATE SQL_Latin1_General_CP1_CI_AS NULL,
        IssuedAtUtc datetime2(7) NOT NULL DEFAULT SYSUTCDATETIME(),
        UpdatedAtUtc datetime2(7) NULL,
        IsActive bit NOT NULL DEFAULT 1,
        RawTokenResponse nvarchar(max) COLLATE SQL_Latin1_General_CP1_CI_AS NULL
    );
    CREATE UNIQUE INDEX UX_ExternalOAuthTokens_ShopId_Provider ON dbo.ExternalOAuthTokens(ShopId, TenantId, Provider);
    CREATE INDEX IX_ExternalOAuthTokens_ConnectionId ON dbo.ExternalOAuthTokens(ConnectionId);
    CREATE INDEX IX_ExternalOAuthTokens_TenantId ON dbo.ExternalOAuthTokens(TenantId);
    CREATE INDEX IX_ExternalOAuthTokens_ExpiresAtUtc ON dbo.ExternalOAuthTokens(ExpiresAtUtc);
END;

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
COMMIT TRANSACTION;
